using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesNetCDF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace esriUtil
{
    public class rasterUtil
    {
        public rasterUtil()
        {
            string mainPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RmrsRasterUtilityHelp";
            string globFuncDir = mainPath + "\\func";
            string globMosaicDir = mainPath + "\\mosaic";
            string globConvDir = mainPath + "\\conv";
            System.IO.DirectoryInfo DInfo = new System.IO.DirectoryInfo(globFuncDir);
            if (!DInfo.Exists)
            {
                DInfo.Create();
            }
            if (!System.IO.Directory.Exists(globMosaicDir)) System.IO.Directory.CreateDirectory(globMosaicDir);
            if (!System.IO.Directory.Exists(globConvDir)) System.IO.Directory.CreateDirectory(globConvDir);
            mosaicDir = globMosaicDir + "\\" + newGuid;
            funcDir = globFuncDir + "\\" + newGuid;
            convDir = globConvDir + "\\" + newGuid;
            fp = newGuid.Substring(1, 3);
            System.IO.Directory.CreateDirectory(funcDir);
            System.IO.Directory.CreateDirectory(convDir);
            System.IO.Directory.CreateDirectory(mosaicDir);
        }
        //~rasterUtil()
        //{
        //    try
        //    {
        //        System.IO.Directory.Delete(funcDir, true);
        //    }
        //    catch
        //    {
        //    }
            
        //}
        public bool isNumeric(string s)
        {
            return geoUtil.isNumeric(s);
        }
        
        private string mosaicDir = "";
        public string TempMosaicDir { get { return mosaicDir; } } 
        private string funcDir = "";
        public string TempFuncDir { get { return funcDir; } }
        private string convDir = "";
        public string TempConvDir { get { return convDir; } }
        private int funcCnt = 0;
        private string newGuid = System.Guid.NewGuid().ToString();
        private string fp = "";
        private string FuncCnt
        {
            get
            {
                funcCnt++;
                return fp+funcCnt.ToString();
            }
        }
        /// <summary>
        /// The different GLCM metric types
        /// </summary>
        public enum glcmMetric { CONTRAST, DIS, HOMOG, ASM, ENERGY, MAXPROB, MINPROB, RANGE, ENTROPY, MEAN, VAR, CORR, COV }
        /// <summary>
        /// ouput raster types
        /// </summary>
        public enum rasterType { GRID, TIFF, IMAGINE, JP2, GDB, JPG, PNG, BMP, GIF,PIX, XPM, MAP, MEM, HDF4, BIL, BIP, BSQ, RST, ENV }
        /// <summary>
        /// sampling cluster types
        /// </summary>
        public enum clusterType {SUM,MEAN,MEDIAN,MODE};
        public enum zoneType { MAX, MIN, RANGE, SUM, MEAN, VAR, STD, MEDIAN, MODE, MINORITY, VARIETY, ENTROPY, ASM }
        /// <summary>
        /// focal window functions types
        /// </summary>
        public enum focalType { SUM, MIN, MAX, MEAN, STD, MODE, MEDIAN, VARIANCE, UNIQUE, ENTROPY, ASM }
        /// <summary>
        /// local type of functions
        /// </summary>
        public enum localType { MAX, MIN, MAXBAND, MINBAND, SUM, MULTIPLY, DIVIDE, SUBTRACT, POWER, MEAN, VARIANCE, STD, MODE, MEDIAN, UNIQUE, ENTROPY, ASM }
        public enum localRescaleType { PrcTile }
        /// <summary>
        /// logical type of functions
        /// </summary>
        public enum logicalType { GT, LT, GE, LE, EQ, AND, OR }
        /// <summary>
        /// patch values used in landscape metrics
        /// </summary>
        public enum landscapeType { AREA, EDGE, RATIO, REGION }
        /// <summary>
        /// the window neighborhood window type
        /// </summary>
        public enum windowType {CIRCLE,RECTANGLE};
        /// <summary>
        /// the log type
        /// </summary>
        public enum transType { LOG10, LN, EXP, EXP10, ABS, SIN, COS, TAN, ASIN, ACOS, ATAN, RADIANS, SQRT, SQUARED }
        public enum mergeType { FIRST, LAST, MIN, MAX, MEAN }
        public enum surfaceType { SLOPE, ASPECT, EASTING, NORTHING, FLIP }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        /// <summary>
        /// Creates an in Memory Raster given a raster dataset
        /// </summary>
        /// <param name="rsDset">IRasterDataset</param>
        /// <returns>IRaster</returns>
        public IRaster createRaster(IRasterDataset rsDset)
        {
            if (rsDset == null)
            {
                return null;
            }
            else
            {
                string cNm = rsDset.Format.ToLower();
                if (cNm.EndsWith("hdf4") || cNm.EndsWith("ntif"))
                {

                    IRasterBandCollection rsBc = new RasterClass();
                    IRasterDatasetJukebox rsDsetJu = (IRasterDatasetJukebox)rsDset;
                    List<IRaster> rsLst = new List<IRaster>();
                    int subCnt = rsDsetJu.SubdatasetCount;
                    for (int i = 0; i < subCnt; i++)
                    {
                        rsDsetJu.Subdataset = i;
                        IRasterDataset3 subDset = (IRasterDataset3)rsDsetJu;



                        rsLst.Add(subDset.CreateFullRaster());

                    }
                    IFunctionRasterDataset fDset = compositeBandFunction(rsLst.ToArray());
                    return createRaster(fDset);

                }
                else
                {
                    IRasterDataset3 rDset3 = (IRasterDataset3)rsDset;
                    return rDset3.CreateFullRaster();
                }
            }
        }
        /// <summary>
        /// Opens a raster dataset given a string path
        /// </summary>
        /// <param name="rasterPath">full path to a raster dataset</param>
        /// <returns>IRasterDataset</returns>
        public IRasterDataset openRasterDataset(string rasterPath,out string bnd)
        {
            IRasterDataset rstDset = null;
            bnd = "all";
            try
            {
                IWorkspace wks = null;
                string rstDir = "";
                string extTest = System.IO.Path.GetExtension(rasterPath).ToLower();
                if (extTest == ".nc" || extTest == ".afr")
                {
                    wks = geoUtil.OpenRasterWorkspace(rasterPath);
                    rstDir = System.IO.Path.GetDirectoryName(rasterPath);
                }
                else
                {
                    wks = openRasterDatasetRec(rasterPath);
                    rstDir = wks.PathName;
                }
                
                string rstName = rasterPath.Replace(rstDir, "").TrimStart(new char[] { '\\' });
                string[] rstNameSplit = rstName.Split(new char[] { '\\' });
                string dataSet = "";
                string rsDset = "";

                switch (rstNameSplit.Length)
                {
                    case 1:
                        rsDset = rstNameSplit[0];
                        break;
                    case 2:
                        rsDset = rstNameSplit[0];
                        bnd = rstNameSplit[1];
                        break;
                    default:
                        dataSet = rstNameSplit[0];
                        rsDset = rstNameSplit[1];
                        bnd = rstNameSplit[2];
                        string[] bndsp = bnd.Split(new char[] { '_' });
                        bnd = bndsp[bndsp.Length - 1];
                        break;
                }
                //Console.WriteLine("Raster Dir = " + rstDir);
                //Console.WriteLine("Raster Name = " + rstName);
                //Console.WriteLine("Raster DataSet = " + dataSet);
                //Console.WriteLine("RsDset = " + rsDset);
                //Console.WriteLine("Bnd = " + bnd);
                if (wks.Type == esriWorkspaceType.esriLocalDatabaseWorkspace || wks.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    IRasterWorkspaceEx rsWks = (IRasterWorkspaceEx)wks;
                    rstDset = rsWks.OpenRasterDataset(rsDset);
                }
                else
                {
                    //Console.WriteLine(wks.PathName);
                    //Console.WriteLine(wks.WorkspaceFactory.get_WorkspaceDescription(true));
                    //string ext = System.IO.Path.GetExtension(wks.PathName).ToLower();
                    if (extTest == ".nc")
                    {
                        INetCDFWorkspace rsWks = (INetCDFWorkspace)wks;
                        IMDWorkspace mdWks = (IMDWorkspace)wks;
                        NetCDFRasterDatasetName rsDsetName = new NetCDFRasterDatasetNameClass();
                        IMDRasterDatasetView rsDsetV = (IMDRasterDatasetView)rsDsetName;
                        string xDim, yDim, bandDim, vNm;
                        getDeminsions(rsWks, out xDim, out yDim, out bandDim, out vNm);
                        //Console.WriteLine("BandDim = " + bandDim.ToString());
                        //Console.WriteLine("xDim = " + xDim.ToString());
                        //Console.WriteLine("yDim = " + yDim.ToString());
                        rsDsetV.Variable = vNm;
                        rsDsetV.XDimension = xDim;
                        rsDsetV.YDimension = yDim;
                        rsDsetV.BandDimension = bandDim;
                        rstDset = (IRasterDataset)mdWks.CreateView(rsDset, (IMDDatasetView)rsDsetV);
                    }
                    else if (extTest == ".afr")
                    {
                        IFunctionRasterDatasetName fDsName = new FunctionRasterDatasetNameClass();
                        fDsName.FullName = rasterPath;
                        IName name = (IName)fDsName;
                        IFunctionRasterDataset ds = (IFunctionRasterDataset)name.Open();
                        rstDset = (IRasterDataset)ds;
                    }
                    else
                    {
                        IRasterWorkspace rsWks = (IRasterWorkspace)wks;
                        rstDset = rsWks.OpenRasterDataset(rsDset);
                    }
                }
                return rstDset;
            }
            catch
            {
                return null;
            }

        }
        public IFunctionRasterDataset returnFunctionRasterDatasetNetCDF(string netCdfPath, string var, string x, string y, string band)
        {
            IWorkspace wks = geoUtil.OpenRasterWorkspace(netCdfPath);
            string rsDset = System.IO.Path.GetFileNameWithoutExtension(netCdfPath);
            INetCDFWorkspace rsWks = (INetCDFWorkspace)wks;
            IMDWorkspace mdWks = (IMDWorkspace)wks;
            NetCDFRasterDatasetName rsDsetName = new NetCDFRasterDatasetNameClass();
            IMDRasterDatasetView rsDsetV = (IMDRasterDatasetView)rsDsetName;
            rsDsetV.Variable = var;
            rsDsetV.XDimension = x;
            rsDsetV.YDimension = y;
            rsDsetV.BandDimension = band;
            return createIdentityRaster((IRasterDataset)mdWks.CreateView(rsDset, (IMDDatasetView)rsDsetV));
        }
        public IStringArray getNetCdfVariables(string netCdfPath)
        {
            IWorkspace wks = geoUtil.OpenRasterWorkspace(netCdfPath);
            INetCDFWorkspace ncdf = (INetCDFWorkspace)wks;
            return ncdf.GetVariables();
        }
        private void getDeminsions(INetCDFWorkspace rsWks, out string xDim, out string yDim, out string bandDim, out string variableName)
        {
            string lonDim = "x";
            string latDim = "y";
            xDim = null;
            yDim = null;
            IStringArray sArr = rsWks.GetDimensions();
            bandDim = sArr.get_Element(2);
            IStringArray vArr = rsWks.GetVariablesByDimension(sArr.get_Element(0));
            variableName = vArr.get_Element(vArr.Count - 1);
            for (int i = 0; i < sArr.Count; i++)
            {
                string el = sArr.get_Element(i);
                //Console.WriteLine(el);
                string ell = el.ToLower();
                if (ell.Contains("lon")) lonDim = el;
                else if (ell.Contains("lat")) latDim = el;
                else if (ell.Contains("x")) xDim = el;
                else if (ell.Contains("y")) yDim = el;
            }
            if (xDim == null) xDim = lonDim;
            if (yDim == null) yDim = latDim;
        }

        private IWorkspace openRasterDatasetRec(string rasterPath)
        {
            IWorkspace wks = null;
            string pD = System.IO.Path.GetDirectoryName(rasterPath);
            
            
            //Console.WriteLine(pD);
            try
            {
                wks = geoUtil.OpenRasterWorkspace(pD);
                if (wks == null)
                {
                    if (pD.Length <2)
                    {
                        return wks;
                    }
                    wks = openRasterDatasetRec(pD);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return wks;
        }
        /// <summary>
        /// gets the number of cell counts for each value within the raster
        /// </summary>
        /// <param name="rst">IRaster, IRasterDataset, or full string path</param>
        /// <returns> a dictionary of cell counts by value</returns>
        public Dictionary<string, int> getCountsbyClass(object rst)
        {
            Dictionary<string, int> cnts = new Dictionary<string, int>();
            IRaster inrst = returnRaster(rst);
            IRaster2 rst2 = (IRaster2)inrst;
            if (rst2 ==null)
            {
                return cnts;
            }
            ITable aTbl = rst2.AttributeTable;
            
            if (aTbl == null)
            {
                Console.WriteLine("Creating VAT...");
                IFunctionRasterDataset fd = createIdentityRaster(rst);
                Dictionary<int,int> cntsInt = BuildVatFromScratch(fd);
                foreach (KeyValuePair<int,int> kvp in cntsInt)
                {
                    cnts.Add(kvp.Key.ToString(), kvp.Value);
                }

            }
            else
            {
                ICursor scur = rst2.AttributeTable.Search(null, false);
                Console.WriteLine("Reading VAT...");
                int vlIndex = scur.Fields.FindField("VALUE");
                int cntIndex = scur.Fields.FindField("COUNT");

                IRow srow = scur.NextRow();
                while (srow != null)
                {
                    string vl = srow.get_Value(vlIndex).ToString();
                    int cnt = System.Convert.ToInt32(srow.get_Value(cntIndex));
                    cnts.Add(vl, cnt);
                    srow = scur.NextRow();
                }
            }
            return cnts;
        }
        /// <summary>
        /// Creates a random point feature class with an equal number of sample for each class within the input raster.
        /// The feature class has 3 fields class, category, and weight which are used to identify the class that the sample point originated from,
        /// the category that point represents on the ground, and the weight that each sample caries (calculated as the count of class pixels / the mean of class pixels) 
        /// </summary>
        /// <param name="wks">The workspace to store the point feature class</param>
        /// <param name="rasterPath">the full path of the categorized raster</param>
        /// <param name="sampleSizePerClass">number of samples per class</param>
        /// <param name="numImages">the number of total images (tiles) used to create the full image picture</param>
        /// <returns>IFeatureClass</returns>
        public IFeatureClass createRandomSampleLocationsByClass(IWorkspace wks, object rasterPath, int[] sampleSizePerClass, int numImages, string outName)
        {
            IFeatureClass sampFtrCls = null;
            try
            {
                IRaster rst = returnRaster(rasterPath);
                IRaster2 rst2 = (IRaster2)rst;
                string pointName = "rndSmp_" + ((IDataset)rst2.RasterDataset).BrowseName;
                if (outName != null)
                {
                    pointName = outName;
                }
                Random rndGen = new Random();
                pointName = geoUtil.getSafeOutputNameNonRaster(wks, pointName);
                IRasterProps rstProps = (IRasterProps)rst2;
                int rWidth = rstProps.Width;
                int rHeight = rstProps.Height;
                int[] spcInt = new int[sampleSizePerClass.Length];
                for (int i = 0; i < sampleSizePerClass.Length; i++)
                {
                    int s = sampleSizePerClass[i]/numImages;
                    if (s == 0) s = 1;
                    spcInt[i] = s;
                }
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "Value";
                fldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                fldsE.AddField(fld);
                fld = new FieldClass();
                fldE = (IFieldEdit)fld;
                fldE.Name_2 = "CATEGORY";
                fldE.Type_2 = esriFieldType.esriFieldTypeString;
                fldE.Length_2 = 20;
                fldsE.AddField(fld);
                fld = new FieldClass();
                fldE = (IFieldEdit)fld;
                fldE.Name_2 = "WEIGHT";
                fldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                fldsE.AddField(fld);
                sampFtrCls = geoUtil.createFeatureClass((IWorkspace2)wks, pointName, flds, esriGeometryType.esriGeometryPoint, rstProps.SpatialReference);
                Dictionary<string, int> classCnts = getCountsbyClass(rst);
                Dictionary<string, List<double[]>> xyList = new Dictionary<string, List<double[]>>();
                bool lookingForSamples = true;
                List<double[]> tCoor = new List<double[]>();
                List<string> checkList = new List<string>();
                List<string> selectRowsColums = new List<string>();
                while (lookingForSamples)
                {
                    int x = rndGen.Next(rWidth); //column
                    int y = rndGen.Next(rHeight); //row
                    //Console.WriteLine("Column = " + x.ToString() + " Row = " + y.ToString());
                    object vlTobject = rst2.GetPixelValue(0, x, y);
                    if(vlTobject==null)
                    {
                        continue;
                    }
                    else
                    {
                        string vl = vlTobject.ToString();
                        int vlint = System.Convert.ToInt32(vlTobject);
                        //Console.WriteLine(vlint.ToString());
                        double xC = rst2.ToMapX(x);
                        double yC = rst2.ToMapY(y);
                        string tStr = x.ToString() + ";" + y.ToString();
                        double[] xy = { xC, yC };

                        if (xyList.TryGetValue(vl, out tCoor))
                        {
                            int spc=spcInt[0];
                            if (spcInt.Contains(vlint)) spc = spcInt[vlint];
                            if (tCoor.Count < spc && !selectRowsColums.Contains(tStr))
                            {
                                tCoor.Add(xy);
                                selectRowsColums.Add(tStr);
                                xyList[vl] = tCoor;
                            }
                            else
                            {
                                if (!checkList.Contains(vl))
                                {
                                    checkList.Add(vl);
                                }
                                if (checkList.Count >= classCnts.Count)
                                {
                                    lookingForSamples = false;
                                }

                            } 
                        }
                        else
                        {
                            tCoor = new List<double[]>();
                            tCoor.Add(xy);
                            selectRowsColums.Add(tStr);
                            xyList.Add(vl, tCoor);
                        }
                    }
                }
                int classIndex = sampFtrCls.FindField("Value");
                int weightIndex = sampFtrCls.FindField("WEIGHT");
                double clAv = classCnts.Values.Average();
                IFeatureBuffer ftrBuff = sampFtrCls.CreateFeatureBuffer();
                IFeatureCursor ftrCur = sampFtrCls.Insert(true);
                foreach (KeyValuePair<string, List<double[]>> kVp in xyList)
                {
                    string ky = kVp.Key;
                    List<double[]> vl = kVp.Value;
                    foreach (double[] d in vl)
                    {
                        //IFeature ftr = sampFtrCls.CreateFeature();
                        //IGeometry geo = ftr.Shape;
                        IPoint pnt = new ESRI.ArcGIS.Geometry.PointClass();// (IPoint)geo;
                        pnt.PutCoords(d[0], d[1]);
                        ftrBuff.Shape = pnt;
                        ftrBuff.set_Value(classIndex, System.Convert.ToDouble(ky));
                        ftrBuff.set_Value(weightIndex,(classCnts[ky] / clAv));
                        //ftr.Store();
                        ftrCur.InsertFeature(ftrBuff);
                    }
                }
                ftrCur.Flush();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }

            return sampFtrCls;
        }
        /// <summary>
        /// Creates a random point feature class across the area of the image. The feature class has 1 field called value that contains the cell value at each sample location
        /// </summary>
        /// <param name="wks">the workspace to store the point feature class</param>
        /// <param name="rasterPath">the path to the raster</param>
        /// <param name="TotalSamples">the total number of samples</param>
        /// <returns>newly created IFeatureClass</returns>
        public IFeatureClass createRandomSampleLocations(IWorkspace wks, object rasterPath, int TotalSamples, string outName)
        {
            IFeatureClass sampFtrCls = null;
            try
            {
                IRaster rst = returnRaster(rasterPath);
                IRaster2 rst2 = (IRaster2)rst;
                string pointName = "rndSmp_" + ((IDataset)((IRasterBandCollection)rst).Item(0)).BrowseName;
                if (outName != null)
                {
                    pointName = outName;
                }
                pointName = geoUtil.getSafeOutputNameNonRaster(wks, pointName);
                Random rndGen = new Random();
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                IRasterProps rstProps = (IRasterProps)rst2;
                int rWidth = rstProps.Width;
                int rHeight = rstProps.Height;
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "VALUE";
                fldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                fldsE.AddField(fld);
                sampFtrCls = geoUtil.createFeatureClass((IWorkspace2)wks, pointName, flds, esriGeometryType.esriGeometryPoint, rstProps.SpatialReference);
                int checkSampleSize = 0;
                int classIndex = sampFtrCls.FindField("Value");
                IFeatureBuffer ftrBuff = sampFtrCls.CreateFeatureBuffer();
                IFeatureCursor ftrCur = sampFtrCls.Insert(true);
                while (checkSampleSize<TotalSamples)
                {
                    int x = rndGen.Next(rWidth);
                    int y = rndGen.Next(rHeight);
                    object vlT = rst2.GetPixelValue(0, x, y);
                    if (vlT==null)
                    {
                        continue;
                    }
                    else
                    {
                        double xC = rst2.ToMapX(x);
                        double yC = rst2.ToMapY(y);
                        //IFeature ftr = sampFtrCls.CreateFeature();
                        //IGeometry geo = ftr.Shape;
                        IPoint pnt = new ESRI.ArcGIS.Geometry.PointClass();
                        pnt.PutCoords(xC, yC);
                        try
                        {
                            ftrBuff.Shape = pnt;
                            ftrBuff.set_Value(classIndex, vlT);
                            //ftr.set_Value(classIndex, vlT);
                            //ftr.Store();
                            ftrCur.InsertFeature(ftrBuff);
                            checkSampleSize++;
                        }
                        catch (Exception e)
                        {
                            //System.Windows.Forms.MessageBox.Show("Error:" + e.ToString());
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                ftrCur.Flush();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return sampFtrCls;
        }
        /// <summary>
        /// deletes an existing raster dataset
        /// </summary>
        /// <param name="fullPath">the full path name of the raster dataset</param>
        public void deleteRasterDataset(string fullPath)
        {
            if (geoUtil.ftrExists(fullPath))
            {
                string bnd = "";
                IDataset dSet = (IDataset)openRasterDataset(fullPath,out bnd);
                if (dSet.CanDelete()) dSet.Delete();
            }
        }
        /// <summary>
        /// retrieves a IRaster for a given a raster band
        /// </summary>
        /// <param name="inRaster">template raster</param>
        /// <param name="index">band index zero based</param>
        /// <returns></returns>
        public IFunctionRasterDataset getBand(object inRaster, int index)
        {
            ILongArray lArr = new LongArrayClass();
            lArr.Add(index);
            return getBands(inRaster, lArr);
        }

        public IFunctionRasterDataset getBands(object inRaster, ILongArray lArr)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ExtractBandFunctionClass();
            IExtractBandFunctionArguments args = new ExtractBandFunctionArgumentsClass(); //IExtractBandFunctionArguments2 args = new ExtractBandFunctionArgumentsClass();
            args.Raster = createIdentityRaster(inRaster);
            //args.MissingBandAction = esriMissingBandAction.esriMissingBandActionFindBestMatch;
            args.BandIDs = lArr;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset reSampleRasterFunction(object inRaster, double outCellSize)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterGeometryProc3 geoP3 = new RasterGeometryProcClass();
            geoP3.Resample(rstResamplingTypes.RSP_NearestNeighbor, outCellSize, (IRaster)rs);
            return createIdentityRaster(rs);
        }
        public IFunctionRasterDataset reSizeRasterCellsFunction(object inRaster,int numCells)
        {
            IFunctionRasterDataset rsDset = createIdentityRaster(inRaster);
            IPnt ps = rsDset.RasterInfo.CellSize;
            double cellW = ps.X * numCells;
            return reSampleRasterFunction(inRaster, cellW);
        }
        /// <summary>
        /// performs a x and y shift of the input raster
        /// </summary>
        /// <param name="inRaster">IRaster, IRasterDataset, string path</param>
        /// <param name="shiftX">number of cells to shift positive number move to the east negative number move to the west</param>
        /// <param name="shiftY">number of cells to shift positive number move north negative number move south</param>
        /// <returns></returns>
        public IFunctionRasterDataset shiftRasterFunction(object inRaster, double shiftX, double shiftY)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterGeometryProc3 geoP3 = new RasterGeometryProcClass();
            geoP3.Shift(shiftX,shiftY,(IRaster)rs);
            //System.Windows.Forms.MessageBox.Show(shiftX.ToString() + "\n" + shiftY.ToString());
            return createIdentityRaster(rs); 
        }
        public IFunctionRasterDataset flipRasterFunction(object inRaster)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterGeometryProc3 geoP3 = new RasterGeometryProcClass();
            geoP3.Flip((IRaster)rs);
            return createIdentityRaster(rs);
        }
        public IFunctionRasterDataset reprojectRasterFunction(object inRaster,ISpatialReference spatialReference)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterGeometryProc3 geoP3 = new RasterGeometryProcClass();
            object cellSize = Type.Missing;
            geoP3.ProjectFast(spatialReference,rstResamplingTypes.RSP_NearestNeighbor,cellSize,(IRaster)rs);
            return createIdentityRaster(rs);
            
        }
        public IFunctionRasterDataset RotateRasterFunction(object inRaster,double rotationAngle)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterGeometryProc3 geoP3 = new RasterGeometryProcClass();
            IPoint pPoint = new PointClass();
            IEnvelope env = ((IRasterProps)rs).Extent;
            double hX = (env.Width/2)+env.XMin;
            double hY = (env.Height / 2) + env.YMin;
            pPoint.X = hX;
            pPoint.Y = hY;
            geoP3.Rotate(pPoint,rotationAngle,(IRaster)rs);//.Resample(reSampleType, outCellSize, (IRaster)rs);
            return createIdentityRaster(rs);
        }
        //public IFunctionRasterDataset calcFocalSumRaster(object inRaster, int clms, int rows)
        //{
        //    IFunctionRasterDataset iR1 = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
        //    string tempAr = funcDir + "\\" + FuncCnt + ".afr";
        //    IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
        //    IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
        //    frDsetName.FullName = tempAr;
        //    frDset.FullName = (IName)frDsetName;
        //    IRasterFunction rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperSum()
        //}
        public IFunctionRasterDataset fastGLCMFunction(object inRaster, int radius, bool horizontal, glcmMetric glcmType)
        {
            FunctionRasters.NeighborhoodHelper.glcmHelperBase glcmB = new FunctionRasters.NeighborhoodHelper.glcmHelperBase();
            glcmB.RasterUtility = this;
            glcmB.InRaster = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
            glcmB.Radius = radius;
            glcmB.Horizontal = horizontal;
            glcmB.GlCM_Metric = glcmType;
            glcmB.WindowType = windowType.CIRCLE;
            glcmB.calcGLCM();
            return glcmB.OutRaster;
        }
        public IFunctionRasterDataset fastGLCMFunction(object inRaster, int clms, int rws, bool horizontal, glcmMetric glcmType)
        {
            FunctionRasters.NeighborhoodHelper.glcmHelperBase glcmB = new FunctionRasters.NeighborhoodHelper.glcmHelperBase();
            glcmB.RasterUtility = this;
            glcmB.InRaster = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
            glcmB.Columns = clms;
            glcmB.Rows = rws;
            glcmB.Horizontal = horizontal;
            glcmB.GlCM_Metric = glcmType;
            glcmB.WindowType = windowType.RECTANGLE;
            glcmB.calcGLCM();
            return glcmB.OutRaster;
        }
        public IFunctionRasterDataset createMeanShiftFuction(object inRaster, int minCells)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.meanShiftFunctionDataset();
            FunctionRasters.meanShiftFunctionArguments args = new FunctionRasters.meanShiftFunctionArguments(this);
            args.ValueRaster = rRst;
            args.MinCells = minCells;
            frDset.Init(rsFunc, args);
            return frDset; 
        }
        public IFunctionRasterDataset PixelBlockToRaster(IPixelBlock ValuePb,IPnt TopLeft,object inRasterObject)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRasterObject);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.PixelBlockToRasterFunctionDataset();
            FunctionRasters.PixelBlockToRasterFunctionArguments args = new FunctionRasters.PixelBlockToRasterFunctionArguments(this);
            args.ValuePixelBlock = ValuePb;
            args.ValueRaster = rRst;
            args.TopLeft = TopLeft;
            frDset.Init(rsFunc, args);
            return frDset; 
        }
        /// <summary>
        /// Performs GLMC Analysis. All bands within the input raster will be transformed
        /// </summary>
        /// <param name="inRaster">raster to perform GLCM</param>
        /// <param name="clms">number of Columns within the analysis window</param>
        /// <param name="rws">number of Rows within the analysis window</param>
        /// <param name="horizontal">whether the direction of the GLCM is horizontal</param>
        /// <param name="glcmType">the type of GLCM to calculate</param>
        /// <returns>a transformed raster</returns>
        public IFunctionRasterDataset calcGLCMFunction(object inRaster, int clms, int rws, bool horizontal, glcmMetric glcmType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (glcmType)
            {
                case glcmMetric.CONTRAST:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperContrast();
                    break;
                case glcmMetric.DIS:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperDissimilarity();
                    break;
                case glcmMetric.HOMOG:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperHomogeneity();
                    break;
                case glcmMetric.ASM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperASM();
                    break;
                case glcmMetric.ENERGY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperEnergy();
                    break;
                case glcmMetric.MAXPROB:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMaxProb();
                    break;
                case glcmMetric.MINPROB:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMinProb();
                    break;
                case glcmMetric.RANGE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperRange();
                    break;
                case glcmMetric.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperEntropy();
                    break;
                case glcmMetric.MEAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMean();
                    break;
                case glcmMetric.VAR:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperVariance();
                    break;
                case glcmMetric.CORR:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperCorrelation();
                    break;
                case glcmMetric.COV:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperCovariance();
                    break;
                default:
                    break;
            }
            FunctionRasters.glcmFunctionArguments args = new FunctionRasters.glcmFunctionArguments(this);
            args.Columns = clms;
            args.Rows = rws;
            args.InRaster = iR1;
            args.Horizontal = horizontal;
            args.GLCMMETRICS = glcmType;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Performs GLMC Analysis. All bands within the input raster will be transformed
        /// </summary>
        /// <param name="inRaster">raster to perform GLCM</param>
        /// <param name="radius">number of Columns that define the radius of the analysis window</param>
        /// <param name="horizontal">whether the direction of the GLCM is horizontal</param>
        /// <param name="glcmType">the type of GLCM to calculate</param>
        /// <returns>a transformed raster</returns>
        public IFunctionRasterDataset calcGLCMFunction(object inRaster, int radius, bool horizontal, glcmMetric glcmType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (glcmType)
	        {
		        case glcmMetric.CONTRAST:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperContrast();
                 break;
                case glcmMetric.DIS:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperDissimilarity();
                 break;
                case glcmMetric.HOMOG:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperHomogeneity();
                 break;
                case glcmMetric.ASM:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperASM();
                 break;
                case glcmMetric.ENERGY:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperEnergy();
                 break;
                case glcmMetric.MAXPROB:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMaxProb();
                 break;
                case glcmMetric.MINPROB:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMinProb();
                 break;
                case glcmMetric.RANGE:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperRange();
                 break;
                case glcmMetric.ENTROPY:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperEntropy();
                 break;
                case glcmMetric.MEAN:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperMean();
                 break;
                case glcmMetric.VAR:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperVariance();
                 break;
                case glcmMetric.CORR:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperCorrelation();
                 break;
                case glcmMetric.COV:
                 rsFunc = new FunctionRasters.NeighborhoodHelper.glcmHelperCovariance();
                 break;
                default:
                 break;
	        }
            FunctionRasters.glcmFunctionArguments args = new FunctionRasters.glcmFunctionArguments(this);
            args.Radius = radius;
            args.InRaster = iR1;
            args.Horizontal = horizontal;
            args.GLCMMETRICS = glcmType;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// performs a convolution analysis for a defined kernel
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="kn"></param>
        /// <returns></returns>
        public IFunctionRasterDataset convolutionRasterFunction(object inRaster, int width, int height, double[] kn)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ConvolutionFunctionClass();
            IFunctionRasterDataset rs = createIdentityRaster(inRaster,rstPixelType.PT_FLOAT);
            IConvolutionFunctionArguments args = new ConvolutionFunctionArgumentsClass();
            args.Raster = rs;
            args.Rows = width;
            args.Columns = height;
            IDoubleArray dbArry = new DoubleArrayClass();
            foreach (double d in kn)
            {
                dbArry.Add(d);
            }
            args.Kernel = dbArry;
            args.Type = esriRasterFilterTypeEnum.esriRasterFilterUserDefined;
            frDset.Init(rsFunc, args);
            IFunctionRasterDataset outFrDset = frDset;
            if (width > 3 || height > 3)
            {
                double cSizeX = frDset.RasterInfo.CellSize.X;
                double cSizeY = frDset.RasterInfo.CellSize.Y;
                double addY = 0;
                double addX = 0;
                if (width > 3)
                {
                    addX = (((width + 1) / 2) - 2) * cSizeX;

                }

                if (height > 3)
                {
                    addY = -1 * (((height + 1) / 2) - 2) * cSizeY;

                }
                outFrDset = shiftRasterFunction(frDset, addX, addY);
            }
            return outFrDset;
            
        }

        private double getNFromKernal(double[] kn)
        {
            double n = 0;
            for (int i = 0; i < kn.Length; i++)
            {
                double vl = kn[i];
                n += vl;
            }
            return n;
        }
        /// <summary>
        /// gets the actual cell size of the input raster as opposed to the average cell size
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public IPnt getCellSize(IRaster rs)
        {
            IRasterProps rsProps = (IRasterProps)rs;
            IEnvelope env = rsProps.Extent;
            double w = (env.XMax - env.XMin) / rsProps.Width;
            double h = (env.YMax - env.YMin) / rsProps.Height;
            IPnt pnt = new PntClass();
            pnt.SetCoords(w, h);
            return pnt;
        }
        /// <summary>
        /// Calculates a transform a raster values to a different value via tranType 
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcMathRasterFunction(object inRaster, transType typ)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (typ)
            {
                case transType.LOG10:
                    rsFunc = new FunctionRasters.log10FunctionDataset();
                    break;
                case transType.LN:
                    rsFunc = new FunctionRasters.logFunctionDataset();
                    break;
                case transType.EXP:
                    rsFunc = new FunctionRasters.expFunctionDataset();
                    break;
                case transType.EXP10:
                    rsFunc = new FunctionRasters.exp10FunctionDataset();
                    break;
                case transType.SIN:
                    rsFunc = new FunctionRasters.sinFunctionDataset();
                    break;
                case transType.COS:
                    rsFunc = new FunctionRasters.cosFunctionDataset();
                    break;
                case transType.TAN:
                    rsFunc = new FunctionRasters.tanFunctionDataset();
                    break;
                case transType.ASIN:
                    rsFunc = new FunctionRasters.asinFunctionDataset();
                    break;
                case transType.ACOS:
                    rsFunc = new FunctionRasters.acosFunctionDataset();
                    break;
                case transType.ATAN:
                    rsFunc = new FunctionRasters.atanFunctionDataset();
                    break;
                case transType.RADIANS:
                    rsFunc = new FunctionRasters.radiansFunctionDataset();
                    break;
                case transType.SQRT:
                    rsFunc = new FunctionRasters.sqrtFunctionDataset();
                    break;
                case transType.SQUARED:
                    rsFunc = new FunctionRasters.squaredFunctionDataset();
                    break;
                default:
                    rsFunc = new FunctionRasters.absFunctionDataset();
                    break;
            }
            FunctionRasters.MathFunctionArguments args = new FunctionRasters.MathFunctionArguments(this);
            args.InRaster = rRst;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Creates a new raster dataset based on the template Raster. If a raster with the same outRaster name exist it will be overwritten
        /// </summary>
        /// <param name="templateRaster">a raster that has the size and shape desired</param>
        /// <param name="outWks">the output workspace</param>
        /// <param name="outRasterName">the name of the raster</param>
        /// <param name="numBands">the number of raster bands</param>
        /// <param name="pixelType">the pixel type</param>
        /// <returns></returns>
        public IRasterDataset createNewRaster(object templateRaster, IWorkspace outWks, string outRasterName, rasterType rType)
        {
            IFunctionRasterDataset fDset = createIdentityRaster(templateRaster);
            //IRasterBandCollection rsBC = (IRasterBandCollection)fDset;
            IEnvelope env = fDset.RasterInfo.Extent;
            IPnt meanCellSize = fDset.RasterInfo.CellSize;
            int numBands = fDset.RasterInfo.BandCount;
            rstPixelType pType = fDset.RasterInfo.PixelType;
            ISpatialReference spRef = fDset.RasterInfo.SpatialReference;
            IRasterDataset rsDset = createNewRaster(env, meanCellSize, outWks, outRasterName, numBands, pType, rType, spRef);
            return rsDset;
        }
        /// <summary>
        /// Creates a new raster dataset based on the template Raster. If a raster with the same outRaster name exist it will be overwritten
        /// </summary>
        /// <param name="templateRaster">a raster that has the size and shape desired</param>
        /// <param name="outWks">the output workspace</param>
        /// <param name="outRasterName">the name of the raster</param>
        /// <param name="numBands">the number of raster bands</param>
        /// <param name="pixelType">the pixel type</param>
        /// <param name="env">the extent</param>
        /// <param name="meanCellSize"> the mean Cell Size of the new raster</param>
        /// <param name="spRf"> the spatial reference of the raster</param>
        /// <returns></returns>
        public IRasterDataset createNewRaster(IEnvelope env, IPnt meanCellSize,IWorkspace outWks, string outRasterName, int numBands, rstPixelType pixelType, rasterType rType, ISpatialReference spRf)
        {
            outRasterName = getSafeOutputName(outWks, outRasterName);
            IRasterDataset3 newRstDset = null;
            if (outWks.Type == esriWorkspaceType.esriFileSystemWorkspace)
            {
                outRasterName = getSafeOutputName(outWks, outRasterName);
                string rasterTypeStr = rType.ToString();
                if (rType== rasterType.IMAGINE)
                {
                    rasterTypeStr = "IMAGINE Image";
                    outRasterName = outRasterName + ".img";
                }
                else if (rType == rasterType.TIFF)
                {
                    outRasterName = outRasterName + ".tif";
                }
                else if (rType == rasterType.GRID)
                {
                    
                }
                else if (rType == rasterType.BMP)
                {
                    outRasterName = outRasterName + ".bmp";
                }
                else if (rType == rasterType.RST)
                {
                    outRasterName = outRasterName + ".rst";
                }
                else
                {
                    rasterTypeStr = "IMAGINE Image";
                    outRasterName = outRasterName + ".img";
                }
                double dX = meanCellSize.X;
                double dY = meanCellSize.Y;
                IRasterWorkspace2 rsWks = (IRasterWorkspace2)outWks;
                newRstDset = (IRasterDataset3)rsWks.CreateRasterDataset(outRasterName, rasterTypeStr, env.LowerLeft, System.Convert.ToInt32(env.Width / dX), System.Convert.ToInt32(env.Height / dY), dX, dY, numBands, pixelType, spRf, true);
            }
            else
            {
                IRasterWorkspaceEx rsWks = (IRasterWorkspaceEx)outWks;
                IRasterDef rsDef = new RasterDefClass();
                IRasterStorageDef rsStDef = new RasterStorageDefClass();
                rsStDef.Origin = env.LowerLeft;
                rsStDef.CellSize = meanCellSize;
                rsDef.SpatialReference = spRf;
                newRstDset = (IRasterDataset3)rsWks.CreateRasterDataset(outRasterName, numBands, pixelType, rsStDef, null, rsDef, null);
            }
            return newRstDset;
        }
        /// <summary>
        /// creates a rectangle folcal window that can be used to lookup values
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public int[,] createFocalWindowRectangle(int Width, int Height, out List<int[]> iter)
        {
            return createFocalWindow(Width, Height, windowType.RECTANGLE, out iter);
        }
        /// <summary>
        /// creates a circle folcal window that can be used to lookup values
        /// </summary>
        /// <param name="Radius"></param>
        /// <returns></returns>
        public int[,] createFocalWindowCircle(int Radius, out List<int[]> iter)
        {
            int Width = ((Radius - 1) * 2) + 1;
            return createFocalWindow(Width, Width, windowType.CIRCLE, out iter);
        }
        /// <summary>
        /// creates a focal window array of 0 and 1 that can represent a circle or rectangle
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="WindowType"></param>
        /// <returns></returns>
        public static int[,] createFocalWidow(int Width, int Height, windowType WindowType, out List<int[]> iter)
        {
            iter = new List<int[]>();
            int[,] xAr = new int[Width, Height];
            int x = 0;
            int y = 0;
            switch (WindowType)
            {
                case windowType.CIRCLE:
                    int radius = ((Width - 1) / 2);
                    for (y = 0; y < Height; y++)
                    {
                        for (x = 0; x < Width; x++)
                        {
                            double cD = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow((y - radius), 2));
                            if (cD <= (radius))
                            {
                                xAr[x, y] = 1;
                                iter.Add(new int[] { x, y });
                            }
                            else
                            {
                                xAr[x, y] = 0;
                            }
                        }
                    }
                    break;
                default:
                    for (y = 0; y < Height; y++)
                    {
                        for (x = 0; x < Width; x++)
                        {
                            xAr[x, y] = 1;
                            iter.Add(new int[] { x, y });
                        }
                    }
                    break;
            }
            return xAr;
        }
        public void createFocalWindowRectangleGLCM(int Width, int Height, bool horizontal, out List<int[]> iter)
        {
            createFocalWindowGLCM(Width, Height, windowType.RECTANGLE, horizontal, out iter);
        }
        public void createFocalWindowCircleGLCM(int Radius, bool horizontal, out List<int[]> iter)
        {
            int Width = ((Radius - 1) * 2) + 1;
            createFocalWindowGLCM(Width, Width, windowType.CIRCLE, horizontal, out iter);
        }
        private void createFocalWindowGLCM(int Width, int Height, windowType WindowType, bool horizontal,out List<int[]> iter)//iter = x,y,weight,getNeightbor 1 or 0
        {

            iter = new List<int[]>();
            int x = 0;
            int y = 0;
            int h = 0;
            int w = Width;
            if (horizontal)
            {
                w = w - 1;
            }
            else
            {
                h = 1;
            }
            switch (WindowType)
            {
                case windowType.CIRCLE:
                    int radius = ((Width - 1) / 2);
                    for (y = h; y < Height; y++)
                    {
                        for (x = 0; x < w; x++)
                        {
                            double cD = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow((y - radius), 2));
                            if (cD <= (radius))
                            {
                                int gN = 0;
                                if(horizontal)
                                {
                                    double cDn = Math.Sqrt(Math.Pow(((x+1) - radius), 2) + Math.Pow((y - radius), 2));
                                    if (cDn <= radius)
                                    {
                                        gN = 1;
                                    }

                                }
                                else
                                {
                                    double cDn = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow(((y-1) - radius), 2));
                                    if (cDn <= radius)
                                    {
                                        gN = 1;
                                    }
                                }
                                iter.Add(new int[] { x, y, gN });
                            }
                            
                        }
                    }
                    break;
                default:
                    for (y = h; y < Height; y++)
                    {
                        for (x = 0; x < w; x++)
                        {
                            int gN = 0;
                            if (horizontal)
                            {
                                if ((x+1) <= Width-1)
                                {
                                    gN = 1;
                                }

                            }
                            else
                            {
                                if ((y-1) >= 0)
                                {
                                    gN = 1;
                                }
                            }
                            iter.Add(new int[] { x, y, gN });
                        }
                    }
                    break;
            }
            return;
        }
        private int[,] createFocalWindow(int Width, int Height, windowType WindowType,out List<int[]> iter)
        {
            iter = new List<int[]>();
            int[,] xAr = new int[Width, Height];
            int x = 0;
            int y = 0;
            switch (WindowType)
            {
                case windowType.CIRCLE:
                    int radius = ((Width - 1) / 2);
                    for (y = 0; y < Height; y++)
                    {
                        for (x = 0; x < Width; x++)
                        {
                            double cD = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow((y - radius), 2));
                            if (cD <= (radius))
                            {
                                xAr[x, y] = 1;
                                iter.Add(new int[] { x, y });
                            }
                            else
                            {
                                xAr[x, y] = 0;
                            }
                        }
                    }
                    break;
                default:
                    for (y = 0; y < Height; y++)
                    {
                        for (x = 0; x < Width; x++)
                        {
                            xAr[x, y] = 1;
                            iter.Add(new int[] { x, y });
                        }
                    }
                    break;
            }
            return xAr;

        }
        /// <summary>
        /// Will perform an arithmeticOperation on an input raster all bands
        /// </summary>
        /// <param name="inRaster1">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="inRaster2">either IRaster, IRasterDataset, a numeric value, or a valid path pointing to a raster</param>
        /// <param name="op">the type of operation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IFunctionRasterDataset calcArithmaticFunction(object inRaster1, object inRaster2, esriRasterArithmeticOperation op, rstPixelType outRasterType = rstPixelType.PT_FLOAT)//, esriCellsizeType outCellSize = esriCellsizeType.esriCellsizeMinOf, esriExtentType outExtent = esriExtentType.esriExtentIntersectionOf )
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ArithmeticFunctionClass();
            rsFunc.PixelType = outRasterType;
            //IArithmeticFunctionArguments2 args = new ArithmeticFunctionArgumentsClass();
            IArithmeticFunctionArguments args = new ArithmeticFunctionArgumentsClass();
            if(isNumeric(inRaster1.ToString())&&isNumeric(inRaster2.ToString()))
            {
                Console.WriteLine("Must have at least one raster");
                return null;
            }
            args.Operation = op;
            object iR1, iR2;
            if (isNumeric(inRaster1.ToString())&&!isNumeric(inRaster2.ToString()))
            {
                iR2 = createIdentityRaster(inRaster2,rstPixelType.PT_FLOAT);
                IScalar sc = new ScalarClass();
                int bCnt = ((IRasterBandCollection)iR2).Count;
                float[] d = new float[bCnt];
                for (int i = 0; i < bCnt; i++)
                {
                    d[i] = System.Convert.ToSingle(inRaster1);
                }
                sc.Value = d;
                iR1 = sc;
            }
            else if (isNumeric(inRaster2.ToString()) && !isNumeric(inRaster1.ToString()))
            {
                iR1 = createIdentityRaster(inRaster1, rstPixelType.PT_FLOAT);
                IScalar sc = new ScalarClass();
                int bCnt = ((IRasterBandCollection)iR1).Count;
                float[] d = new float[bCnt];
                for (int i = 0; i < bCnt; i++)
                {
                    d[i] = System.Convert.ToSingle(inRaster2);
                }
                sc.Value = d;
                iR2 = sc;
            }
            else
            {
                iR1 = createIdentityRaster(inRaster1, rstPixelType.PT_FLOAT);
                iR2 = createIdentityRaster(inRaster2, rstPixelType.PT_FLOAT);
                IRasterBandCollection rsBc1 = (IRasterBandCollection)iR1;
                IRasterBandCollection rsBc2 = (IRasterBandCollection)iR2;
                int bCnt1,bCnt2;
                bCnt1 = rsBc1.Count;
                bCnt2 = rsBc2.Count;
                if (bCnt1 != rsBc2.Count)
                {
                    int dif = bCnt1-bCnt2;
                    int absDif = Math.Abs(dif);
                    if (dif > 0)
                    {
                        IRaster rsB = createRaster(getBand(iR2, 0));
                        IRaster[] rsArr = new IRaster[absDif];
                        for (int i = 0; i < absDif; i++)
                        {
                            rsArr[i] = rsB;
                        }
                        iR2 = compositeBandFunction(rsArr);
                    }
                    else
                    {
                        IRaster rsB = createRaster(getBand(iR1, 0));
                        IRaster[] rsArr = new IRaster[absDif];
                        for (int i = 0; i < absDif; i++)
                        {
                            rsArr[i] = rsB;
                        }
                        iR1 = compositeBandFunction(rsArr);
                    }
                }
            }
            args.Raster = iR1;
            args.Raster2 = iR2;
            //args.CellsizeType = esriCellsizeType.esriCellsizeMinOf;
            //args.ExtentType = esriExtentType.esriExtentIntersectionOf;
            frDset.Init(rsFunc, args);
            return frDset;
            
            
        }
        /// <summary>
        /// Saves a subset of a raster 
        /// </summary>
        /// <param name="inRasterPath">the path to the raster can be a batch file</param>
        /// <param name="outName">the name of the new raster</param>
        /// <param name="wks">the workspace it will be saved in</param>
        /// <param name="rastertype">the type of raster</param>
        /// <param name="extent">the extent to save</param>
        /// <param name="noDataVl">the not data value</param>
        /// <param name="IntBlockWidth">the number of cells to read in width</param>
        /// <param name="IntBlockHeight">the number of cells to read in height</param>
        /// <returns>the new raster dataset</returns>
        public IRasterDataset subsetSaveRasterToDataset(object inRasterPath, string outName, IWorkspace wks, rasterType rastertype, IEnvelope extent, object noDataVl = null, int IntBlockWidth = 512, int IntBlockHeight = 512)
        {
            IFunctionRasterDataset fDset = createIdentityRaster(inRasterPath);
            IRasterDataset newRasterDataset = createNewRaster(extent,fDset.RasterInfo.CellSize, wks, outName,fDset.RasterInfo.BandCount,fDset.RasterInfo.PixelType, rastertype,fDset.RasterInfo.SpatialReference);
            IRasterBandCollection rsbc = (IRasterBandCollection)newRasterDataset;
            int bndCnt = rsbc.Count;
            IRaster orgRs = createRaster(fDset);
            IRaster nRs = ((IRasterDataset3)newRasterDataset).CreateFullRaster();
            IRasterProps rsPropOut = (IRasterProps)nRs;
            IPnt mcellSize = fDset.RasterInfo.CellSize;
            int tRasterWidth = (int)(extent.Width / mcellSize.X);
            int tRasterHeight = (int)(extent.Height / mcellSize.Y);
            IPnt pntSize = new PntClass();
            IPnt topLeft = new PntClass();
            IPnt topLeftOffset = new PntClass();
            int wOffset = 0;
            int hOffset = 0;
            IRaster2 orgRs2 = (IRaster2)orgRs;
            orgRs2.MapToPixel(extent.UpperLeft.X, extent.UpperLeft.Y, out wOffset, out hOffset);
            int intW = IntBlockWidth;
            int intH = IntBlockHeight;
            int nw = intW;
            int nh = intH;
            IRasterEdit nRsE = (IRasterEdit)nRs;
            if (rastertype == rasterType.GDB)
            {

                rsPropOut.Extent = extent;
                rsPropOut.Width = (int)(extent.Width / mcellSize.X);
                rsPropOut.Height = (int)(extent.Height / mcellSize.Y);
                for (int pbh = 0; pbh < tRasterHeight; pbh += intW)
                {

                    for (int pbw = 0; pbw < tRasterWidth; pbw += intH)
                    {
                        topLeft.SetCoords(pbw, pbh);
                        topLeftOffset.SetCoords(pbw + wOffset, pbh + hOffset);
                        getPbWidthHeight(tRasterWidth, tRasterHeight, topLeft, intW, intH, out nw, out nh);
                        pntSize.SetCoords(nw, nh);
                        IPixelBlock3 inPb = (IPixelBlock3)orgRs.CreatePixelBlock(pntSize);
                        orgRs.Read(topLeftOffset, (IPixelBlock)inPb);
                        IPixelBlock3 outPb = (IPixelBlock3)nRs.CreatePixelBlock(pntSize);
                        for (int b = 0; b < bndCnt; b++)
                        {
                            outPb.set_PixelData(b, inPb.get_PixelDataByRef(b));
                            outPb.set_NoDataMask(b, inPb.get_NoDataMaskByRef(b));
                        }
                        nRsE.Write(topLeft, (IPixelBlock)outPb);
                        nRsE.Refresh();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(inPb);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(outPb);
                        
                    }
                }
            }
            else
            {
                object uNoDataVl = convertToActualNoDataVl(noDataVl, fDset.RasterInfo.PixelType);
                try
                {
                    double nDv;
                    if (!(Double.TryParse(noDataVl.ToString(), out nDv)))
                    {
                        uNoDataVl = nDv;

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    for (int b = 0; b < bndCnt; b++)
                    {
                        IRasterProps rsPropsOut = (IRasterProps)rsbc.Item(b);
                        rsPropsOut.NoDataValue = uNoDataVl;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                for (int pbh = 0; pbh < tRasterHeight; pbh += intH)
                {
                    for (int pbw = 0; pbw < tRasterWidth; pbw += intW)
                    {
                        topLeft.SetCoords(pbw, pbh);
                        topLeftOffset.SetCoords(pbw + wOffset, pbh + hOffset);
                        getPbWidthHeight(tRasterWidth, tRasterHeight, topLeft, intW, intH, out nw, out nh);
                        pntSize.SetCoords(nw, nh);
                        IPixelBlock3 inPb = (IPixelBlock3)orgRs.CreatePixelBlock(pntSize);
                        orgRs.Read(topLeftOffset, (IPixelBlock)inPb);
                        IPixelBlock3 outPb = (IPixelBlock3)nRs.CreatePixelBlock(pntSize);
                        for (int b = 0; b < bndCnt; b++)
                        {
                            outPb.set_PixelData(b, inPb.get_PixelDataByRef(b));
                            outPb.set_NoDataMask(b, inPb.get_NoDataMaskByRef(b));
                        }
                        nRsE.Write(topLeft, (IPixelBlock)outPb);
                        nRsE.Refresh();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(inPb);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(outPb);
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(nRsE);
            return newRasterDataset;
        }

        private bool checkEdit(IRasterEdit nRsE)
        {
            bool outTest = nRsE.CanEdit();
            
            return outTest;
        }
        
        /// <summary>
        /// Saves a raster dataset using a specified block size
        /// </summary>
        /// <param name="inRaster">the raster to save</param>
        /// <param name="outName">the name of the new raster</param>
        /// <param name="wks">the workspace to save the raster within</param>
        /// <param name="rastertype">type of raster</param>
        /// <param name="noDataVl">the value for no data</param>
        /// <param name="IntBlockWidth">block width</param>
        /// <param name="IntBlockHeight">block height</param>
        /// <returns>the new raster dataset</returns>
        public IRasterDataset saveRasterToDatasetM(object inRaster, string outName, IWorkspace wks, rasterType rastertype, object noDataVl=null, int IntBlockWidth= 512, int IntBlockHeight=512)
        {
            IFunctionRasterDataset fDset = null;
            if ((rastertype == rasterType.GRID) && (((IRasterProps)inRaster).PixelType == rstPixelType.PT_DOUBLE))
            {
                fDset = createIdentityRaster(inRaster, rstPixelType.PT_FLOAT);
            }
            else
            {
                fDset = createIdentityRaster(inRaster);
            }
            IRasterDataset newRasterDataset = createNewRaster(inRaster, wks, outName,rastertype);
            IRasterBandCollection rsbc = (IRasterBandCollection)newRasterDataset;
            int bndCnt = rsbc.Count;
            IRasterFunctionHelper fHelp = new RasterFunctionHelperClass();
            fHelp.Bind(fDset);
            IRaster nRs = ((IRasterDataset3)newRasterDataset).CreateFullRaster();
            int tRasterWidth = fDset.RasterInfo.Width;
            int tRasterHeight = fDset.RasterInfo.Height;
            IPnt pntSize = new PntClass();
            IPnt topLeft = new PntClass();
            int intW = IntBlockWidth;
            int intH = IntBlockHeight;
            int nw = intW;
            int nh = intH;
            IRasterEdit nRsE = (IRasterEdit)nRs;
            if (rastertype == rasterType.GDB)
            {
                IRasterProps rsPropOut = (IRasterProps)nRs;
                IEnvelope env = fDset.RasterInfo.Extent;
                IPnt mcellSize = fDset.RasterInfo.CellSize;
                rsPropOut.Extent = env;
                rsPropOut.Width = (int)(env.Width / mcellSize.X);
                rsPropOut.Height = (int)(env.Height / mcellSize.Y);
                for (int pbh = 0; pbh < tRasterHeight; pbh += intW)
                {
                    
                    for (int pbw = 0; pbw < tRasterWidth; pbw += intH)
                    {
                        topLeft.SetCoords(pbw, pbh);
                        getPbWidthHeight(tRasterWidth, tRasterHeight, topLeft, intW, intH, out nw, out nh);
                        //Console.WriteLine("PBTL = " + pbh.ToString() + ", " + pbw.ToString() + ", PBH = " + nh.ToString() + ", PBW = " + nw.ToString());
                        pntSize.SetCoords(nw, nh);
                        IPixelBlock3 inPb = (IPixelBlock3)fHelp.Raster.CreatePixelBlock(pntSize);
                        fHelp.Raster.Read(topLeft, (IPixelBlock)inPb);
                        IPixelBlock3 outPb = (IPixelBlock3)nRs.CreatePixelBlock(pntSize);
                        for (int b = 0; b < bndCnt; b++)
                        {
                            outPb.set_PixelData(b, inPb.get_PixelDataByRef(b));
                            outPb.set_NoDataMask(b, inPb.get_NoDataMaskByRef(b));
                        }
                        nRsE.Write(topLeft, (IPixelBlock)outPb);
                        nRsE.Refresh();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(inPb);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(outPb);
                    }
                }
            }
            else
            {
                object uNoDataVl = convertToActualNoDataVl(noDataVl,fDset.RasterInfo.PixelType);
                try
                {
                    double nDv;
                    if (!(Double.TryParse(noDataVl.ToString(), out nDv)))
                    {
                        uNoDataVl=nDv;
                        
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    for (int b = 0; b < bndCnt; b++)
                    {
                        IRasterProps rsPropsOut = (IRasterProps)rsbc.Item(b);
                        rsPropsOut.NoDataValue = uNoDataVl;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                for (int pbh = 0; pbh < tRasterHeight; pbh += intH)
                {
                    for (int pbw = 0; pbw < tRasterWidth; pbw += intW)
                    {
                        topLeft.SetCoords(pbw, pbh);
                        getPbWidthHeight(tRasterWidth, tRasterHeight, topLeft, intW, intH, out nw, out nh);
                        //Console.WriteLine("PBTL = " + pbh.ToString() + ", " + pbw.ToString() + ", PBH = " + nh.ToString() + ", PBW = " + nw.ToString());
                        pntSize.SetCoords(nw, nh);
                        IPixelBlock3 inPb = (IPixelBlock3)fHelp.Raster.CreatePixelBlock(pntSize);
                        fHelp.Raster.Read(topLeft, (IPixelBlock)inPb);
                        IPixelBlock3 outPb = (IPixelBlock3)nRs.CreatePixelBlock(pntSize);
                        for (int b = 0; b < bndCnt; b++)
                        {
                            System.Array outSArr = (System.Array)outPb.get_PixelData(b);
                            for (int r = 0; r < nh; r++)
                            {
                                for (int c = 0; c < nw; c++)
                                {
                                    object objVl = inPb.GetVal(b, c, r);
                                    if (objVl == null)
                                    {
                                        objVl = uNoDataVl;
                                    }
                                    outSArr.SetValue(objVl, c, r);
                                }
                            }
                            outPb.set_PixelData(b, outSArr);
                        }
                        nRsE.Write(topLeft, (IPixelBlock)outPb);
                        nRsE.Refresh();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(inPb);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(outPb);
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(nRsE);
            return newRasterDataset;
        }

        private void getPbWidthHeight(int tRasterWidth, int tRasterHeight, IPnt topLeft, int intW, int intH, out int nw, out int nh)
        {
            int totalWidthLeft = (int)((tRasterWidth)-topLeft.X);
            int totalHeightLeft = (int)((tRasterHeight) - topLeft.Y);
            if (totalWidthLeft > intW) nw = intW;
            else nw = totalWidthLeft;
            if (totalHeightLeft > intH) nh = intH;
            else nh = totalHeightLeft;
        }

        private object convertToActualNoDataVl(object noDataVl, rstPixelType rstPixelType)
        {
            object outVl = null;
            try
            {
                if (noDataVl != null)
                {
                    switch (rstPixelType)
                    {
                        case rstPixelType.PT_CHAR:
                            outVl = System.Convert.ToSByte(noDataVl);
                            break;
                        case rstPixelType.PT_DOUBLE:
                            outVl = System.Convert.ToDouble(noDataVl);
                            break;
                        case rstPixelType.PT_FLOAT:
                            outVl = System.Convert.ToSingle(noDataVl);
                            break;
                        case rstPixelType.PT_LONG:
                            outVl = System.Convert.ToInt32(noDataVl);
                            break;
                        case rstPixelType.PT_SHORT:
                            outVl = System.Convert.ToInt16(noDataVl);
                            break;
                        case rstPixelType.PT_UCHAR:
                            outVl = System.Convert.ToByte(noDataVl);
                            break;
                        case rstPixelType.PT_ULONG:
                            outVl = System.Convert.ToUInt32(noDataVl);
                            break;
                        case rstPixelType.PT_UNKNOWN:
                            outVl = System.Convert.ToDouble(noDataVl);
                            break;
                        case rstPixelType.PT_USHORT:
                            outVl = System.Convert.ToUInt16(noDataVl);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                outVl = null;
            }
            
            return outVl;
        }
        private bool noDataValidValue(object noDataVl, rstPixelType pixelType)
        {
            bool rV = true;
            if (noDataVl == null)
            {
                rV = false;
            }
            else
            {
                double vl = System.Convert.ToDouble(noDataVl);
                switch (pixelType)
                {
                    case rstPixelType.PT_CHAR:
                        if (vl <= 128 && vl >= -127)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_FLOAT:
                        if (vl <= Single.MaxValue && vl >= Single.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_LONG:
                        if (vl <= long.MaxValue && vl >= long.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_SHORT:
                        if (vl <= short.MaxValue && vl >= short.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U1:
                        if (vl <= 0 && vl >= 1)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U2:
                        if (vl <= 0 && vl >= 2)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U4:
                        if (vl <= 0 && vl >= 4)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_UCHAR:
                        if (vl <= 0 && vl >= 255)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_ULONG:
                        if (vl <= ulong.MaxValue && vl >= ulong.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_USHORT:
                        if (vl <= ushort.MaxValue && vl >= ushort.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    default:
                        rV = false;
                        break;
                }
            }
            return rV;
        }
        private bool noDataValidValue(object noDataVl, IRaster inRaster)
        {
            bool rV = true;
            if(noDataVl==null)
            {
                rV = false;
            }
            else
            {
                double vl = System.Convert.ToDouble(noDataVl);
                IRasterProps rsp = (IRasterProps)inRaster;
                switch (rsp.PixelType)
                {
                    case rstPixelType.PT_CHAR:
                        if (vl <= 128 && vl >= -128)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_FLOAT:
                        if (vl <= Single.MaxValue && vl >= Single.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_LONG:
                        if (vl <= long.MaxValue && vl >= long.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_SHORT:
                        if (vl <= short.MaxValue && vl >= short.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U1:
                        if (vl <= 2 && vl >= 0)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U2:
                        if (vl <= 4 && vl >= 0)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_U4:
                        if (vl <= 16 && vl >= 0)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_UCHAR:
                        if (vl <= 256 && vl >= 0)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_ULONG:
                        if (vl <= ulong.MaxValue && vl >= ulong.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    case rstPixelType.PT_USHORT:
                        if (vl <= ushort.MaxValue && vl >= ushort.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                    default:
                        if (vl <= double.MaxValue && vl >= double.MinValue)
                        {
                            rV = true;
                        }
                        else
                        {
                            rV = false;
                        }
                        break;
                }
            }
            return rV;
        }
        /// <summary>
        /// saves a raster, rasterDataset, FunctionRasterDataset
        /// </summary>
        /// <param name="inRaster">raster, rasterDataset, FunctionRasterDataset</param>
        /// <param name="outName">new name</param>
        /// <param name="wks">output workspace</param>
        /// <param name="rastertype">type of output</param>
        /// <param name="tiled">storage format</param>
        /// <param name="calcStats"></param>
        /// <param name="buildAttribute"></param>
        /// <returns>RasterDataset</returns>
        public IRasterDataset saveRasterToDataset(object inRaster, string outName, IWorkspace wks,rasterType rastertype, bool tiled=true, bool calcStats=false, bool buildAttribute=false,esriRasterCompressionType comptype = esriRasterCompressionType.esriRasterCompressionUncompressed,int compression=75, int tileWidth = 128, int tileHeight = 128)
        {
            
            if (inRaster is RasterDataset)
            {
            }
            else if (inRaster is Raster)
            {
            }
            else if (inRaster is FunctionRasterDataset)
            {
                return null;
            }
            else
            {
                return null;
            }
            string rsTypeStr = rastertype.ToString();
            outName = getSafeOutputName(wks, outName);
            string ext = "";
            if (rastertype== rasterType.IMAGINE)
            {
                rsTypeStr = "IMAGINE Image";
                ext = ".img";
            }
            else if (rastertype == rasterType.HDF4)
            {
                ext = ".hdf";
            }
            else if (rastertype == rasterType.ENV)
            {
                ext = ".hdr";
            }
            else if (rastertype == rasterType.GRID || rastertype == rasterType.GDB)
            {
                ext = "";
            }
            else
            {
                ext = "." + rastertype.ToString().ToLower();
            }
            esriWorkspaceType tp = wks.Type;
            if (tp == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                rsTypeStr = rasterType.GDB.ToString();
            }
            if (rastertype == rasterType.GRID)
            {
                if (outName.Length > 12)
                {
                    outName.Substring(12);
                }
                if ((rastertype==rasterType.GRID)&&(((IRasterProps)inRaster).PixelType == rstPixelType.PT_DOUBLE))
                {
                    inRaster = createIdentityRaster(inRaster,rstPixelType.PT_FLOAT);

                }
            }
            else
            {
                if (outName.IndexOf(ext) == -1)
                {
                    outName = outName + ext;
                }

            }
            IRasterDataset rsDset = null;
            try
            {
                IRasterStorageDef2 rsStorDef = new RasterStorageDefClass();
                rsStorDef.PyramidLevel = 0;
                rsStorDef.Tiled = tiled;
                rsStorDef.CompressionQuality = compression;
                rsStorDef.CompressionType = comptype;
                rsStorDef.TileHeight = tileHeight;
                rsStorDef.TileWidth = tileWidth;
                ISaveAs2 sv = (ISaveAs2)inRaster;
                rsDset = sv.SaveAsRasterDataset(outName, wks, rsTypeStr,rsStorDef);
                if (calcStats)
                {
                    IRasterDatasetEdit3 rsDsetEdit = (IRasterDatasetEdit3)rsDset;
                    object noData = ((IFunctionRasterDataset)(rsDset)).RasterInfo.NoData;
                    rsDsetEdit.ComputeStatisticsHistogram(1, 1, noData, true);
                }
                if (buildAttribute)
                {
                    IRasterDatasetEdit3 rsDsetEdit = (IRasterDatasetEdit3)rsDset;
                    rsDsetEdit.BuildAttributeTable();
                }
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.ToString());
                Console.WriteLine(e.ToString());
                string bnd;
                rsDset = openRasterDataset(wks.PathName + "\\" + outName, out bnd);
            }
            return rsDset;
        }
        /// <summary>
        /// Save a raster to specified dataset
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="outName"></param>
        /// <param name="wks"></param>
        /// <returns></returns>
        /// 
        public IRasterDataset saveRasterToDataset(IRaster inRaster, string outName, IWorkspace wks)
        {
            rasterType rsType = rasterType.GDB;
            esriWorkspaceType tp = wks.Type;
            if (tp == esriWorkspaceType.esriFileSystemWorkspace)
            {
                rsType = rasterType.IMAGINE;
            }
            return saveRasterToDataset(inRaster, outName, wks, rsType);
        }
        public void calcSatatsAndHistFast(IRasterDataset rsDset, int skipFactor = 1)
        {

        }
        /// <summary>
        /// calculates stats and histogram for rasters
        /// </summary>
        /// <param name="rs"></param>
        public IRaster calcStatsAndHist(IRaster rs)
        {
            return calcStatsAndHist(rs, 1);
        }
        public IRaster calcStatsAndHist(IRaster rs, int skipFactor)
        {
            IRaster outRs = null;
            try
            {
                IRaster2 rs2 = (IRaster2)rs;
                IRasterDataset rsDset = rs2.RasterDataset;
                IRasterDataset3 rsDset3 = (IRasterDataset3)rsDset;
                IRasterDatasetEdit3 rsDset3e = (IRasterDatasetEdit3)rsDset3;
                rsDset3e.DeleteStats();
                rsDset3e.ComputeStatisticsHistogram(skipFactor, skipFactor, new double[] { }, false);
                outRs = rsDset3.CreateFullRaster();
            }
            catch
            {
                try
                {
                    IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                    for (int i = 0; i < rsBc.Count; i++)
                    {
                        IRasterBand rsB = rsBc.Item(i);
                        bool hasStats = true;
                        rsB.HasStatistics(out hasStats);
                        if (hasStats)
                        {
                            IRasterStatistics rsStats = rsB.Statistics;
                            rsStats.SkipFactorX = skipFactor;
                            rsStats.SkipFactorY = skipFactor;
                            rsStats.Recalculate();
                        }
                        else
                        {
                            rsB.ComputeStatsAndHist();
                        }
                    }
                    outRs = rs;
                }
                catch (Exception e)
                {
                    outRs = rs;
                    Console.WriteLine(e.ToString());
                }
            }
            return outRs;
            
        }
        
        public IRasterDataset calcStatsAndHist(IRasterDataset rsDset, bool overwrite=false)
        {
            IRasterDatasetEdit3 rsDe = (IRasterDatasetEdit3)rsDset;
            IRasterInfo rsInfo = ((IFunctionRasterDataset)rsDset).RasterInfo;
            IRasterStatistics rsStats = (IRasterStatistics)rsInfo;
            if (overwrite == true)
            {
                object noDataVl = ((IFunctionRasterDataset)rsDset).RasterInfo.NoData;
                rsDe.ComputeStatisticsHistogram(1, 1, noDataVl, true);
            }
            else
            {
                if (!rsStats.IsValid)
                {
                    object noDataVl = ((IFunctionRasterDataset)rsDset).RasterInfo.NoData;
                    rsDe.ComputeStatisticsHistogram(1, 1, noDataVl, true);
                }
            }
            return rsDset;
        }
        public IRasterDataset calcStatsAndHist(IFunctionRasterDataset rsDset)
        {
            return calcStatsAndHist((IRasterDataset)rsDset);
        }
        /// <summary>
        /// Rescales raster to 8 byte unsigned integer 0-256
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset reScaleRasterFunction(object inRaster)
        {
            return reScaleRasterFunction(inRaster, rstPixelType.PT_UCHAR);
        }
        /// <summary>
        /// Rescales raster to a given raster pixel type min max value
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset reScaleRasterFunction(object inRaster, rstPixelType pType)
        {
            return reScaleRasterFunction(inRaster,pType,esriRasterStretchType.esriRasterStretchMinimumMaximum);
        }
        public IFunctionRasterDataset reScaleRasterFunction(object inRaster,rstPixelType pType,esriRasterStretchType stretchType, double[] min=null, double[] max=null, double[] mean=null, double[] std=null)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            if (min == null)
            {
                calcStatsAndHist(iR1);
            }
            else
            {
                
                IRasterBandCollection rsBC = (IRasterBandCollection)iR1;
                for (int i = 0; i < rsBC.Count; i++)
                {
                    IRasterBand rsBand = rsBC.Item(i);
                    IRasterStatistics rsStats = rsBand.Statistics;
                    if (rsStats == null)
                    {
                        rsStats = new RasterStatisticsClass();
                        IRasterBandEdit rsBandE = (IRasterBandEdit)rsBand;
                        rsBandE.AlterStatistics(rsStats);
                        rsStats = rsBand.Statistics;
                    }
                    if (i < min.Length) rsStats.Minimum = min[i];
                    else rsStats.Minimum = min[0];
                    if (i < min.Length) rsStats.Maximum = max[i];
                    else rsStats.Maximum = max[0];
                    if (i < min.Length) rsStats.Mean = mean[i];
                    else rsStats.Mean = mean[0];
                    if (i < min.Length) rsStats.StandardDeviation = std[i];
                    else rsStats.StandardDeviation = std[0];
                    rsStats.IsValid = true;
                }
            }
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new StretchFunction();
            rsFunc.PixelType = pType;
            IStretchFunctionArguments args = new StretchFunctionArgumentsClass();
            args.Raster = iR1;
            args.StretchType = stretchType;
            args.UseGamma = false;
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    args.Max = 128;
                    args.Min = -127;
                    break;
                case rstPixelType.PT_DOUBLE:
                    args.Max = double.MaxValue - 1;
                    args.Min = double.MinValue;
                    break;
                case rstPixelType.PT_FLOAT:
                    args.Max = float.MaxValue - 1;
                    args.Min = float.MinValue;
                    break;
                case rstPixelType.PT_LONG:
                    args.Max = long.MaxValue - 1;
                    args.Min = long.MinValue;
                    break;
                case rstPixelType.PT_SHORT:
                    args.Max = short.MaxValue - 1;
                    args.Min = short.MinValue;
                    break;
                case rstPixelType.PT_U1:
                    args.Max = 1;
                    args.Min = 0;
                    break;
                case rstPixelType.PT_U2:
                    args.Max = 3;
                    args.Min = 0;
                    break;
                case rstPixelType.PT_U4:
                    args.Max = 15;
                    args.Min = 0;
                    break;
                case rstPixelType.PT_UCHAR:
                    args.Max = 255;
                    args.Min = 0;
                    break;
                case rstPixelType.PT_ULONG:
                    args.Max = ulong.MaxValue - 1;
                    args.Min = 0;
                    break;
                case rstPixelType.PT_USHORT:
                    args.Max = ushort.MaxValue - 1;
                    args.Min = 0;
                    break;
                default:
                    args.Max = 255;
                    args.Min = 0;
                    break;
            }
            switch (stretchType)
            {
                case esriRasterStretchType.esriRasterStretchMinimumMaximum:   
                case esriRasterStretchType.esriRasterStretchNone:
                    break;
                case esriRasterStretchType.esriRasterStretchPercentMinimumMaximum:
                    args.MinPercent = 2;
                    args.MaxPercent = 2;
                    break;
                case esriRasterStretchType.esriRasterStretchStandardDeviation:
                    args.NumberOfStandardDeviations = 2.5;
                    break;
                default:
                    break;
            }

            frDset.Init(rsFunc, args);
            return frDset;
        }
                
        public IFunctionRasterDataset calcFocalSampleFunction(object inRaster, HashSet<string> offset, focalType statType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (statType)
            {
                case focalType.MIN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperMin();
                    break;
                case focalType.SUM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperSum();
                    break;
                case focalType.MEAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperMean();
                    break;
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperMedian();
                    break;
                case focalType.VARIANCE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperVariance();
                    break;
                case focalType.STD:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperStd();
                    break;
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperEntropy();
                    break;
                case focalType.ASM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperASM();
                    break;
                default:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalSampleHelperMax();
                    break;
            }
            FunctionRasters.focalSampleArguments args = new FunctionRasters.focalSampleArguments(this);
            args.OffSets = offset;
            args.Operation = statType;
            //args.WindowType = windowType.RECTANGLE;
            args.InRaster = iR1;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="clm">number of columns (cells)</param>
        /// <param name="rws">number of rows</param>
        /// <param name="statType">the type of operation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IFunctionRasterDataset calcFocalStatisticsFunction(object inRaster, int clm, int rws, focalType statType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster,rstPixelType.PT_FLOAT);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (statType)
            {
                case focalType.MIN:
                case focalType.MAX:
                case focalType.MEAN:
                case focalType.STD:
                    return calcFocalStatisticsRectangle(inRaster, clm, rws, statType);
                case focalType.SUM:
                    IFunctionRasterDataset mRs = calcFocalStatisticsFunction(inRaster, clm, rws, focalType.MEAN);
                    return calcArithmaticFunction(mRs, clm * rws, esriRasterArithmeticOperation.esriRasterMultiply);
                    //rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperSum();
                    //break;
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMedian();
                    break;
                case focalType.VARIANCE:
                    IFunctionRasterDataset rs = calcFocalStatisticsFunction(inRaster, clm, rws,focalType.STD);
                    return calcArithmaticFunction(rs,2,esriRasterArithmeticOperation.esriRasterPower);
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperEntropy();
                    break;
                case focalType.ASM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperProbability();
                    break;
                default:
                    break;
            }
            FunctionRasters.FocalFunctionArguments args = new FunctionRasters.FocalFunctionArguments(this);
            args.Rows = rws;
            args.Columns = clm;
            //args.WindowType = windowType.RECTANGLE;
            args.InRaster = iR1;
            args.Operation = statType;
            frDset.Init(rsFunc, args);
            return frDset;
        }

        private IFunctionRasterDataset calcFocalStatisticsRectangle(object iR1, int clm, int rws, focalType statType)
        {
            //Console.WriteLine(statType);
            //Console.WriteLine((esriFocalStatisticType)statType);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new StatisticsFunctionClass();
            //IStatisticsFunctionArguments2 args = new StatisticsFunctionArgumentsClass();
            IStatisticsFunctionArguments args = new StatisticsFunctionArgumentsClass();
            args.Raster = createIdentityRaster(iR1,rstPixelType.PT_FLOAT);
            args.Columns = clm;
            args.Rows = rws; 
            args.Type = (esriFocalStatisticType)statType;
            //args.FillNoDataOnly = false;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="radius">number of cells that make up the radius of a circle</param>
        /// <param name="statType">the type of opporation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IFunctionRasterDataset calcFocalStatisticsFunction(object inRaster, int radius, focalType statType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            List<int[]> outLst = new List<int[]>();
            int[,] crl = null;
            double[] cArr = null;
            double sumCircle = 0;
            switch (statType)
            {
                case focalType.MIN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMin();
                    break;
                case focalType.SUM:
                    crl = createFocalWindowCircle(radius, out outLst);
                    cArr = (from int i in crl select System.Convert.ToDouble(i)).ToArray();
                    return convolutionRasterFunction(iR1,crl.GetUpperBound(0)+1,crl.GetUpperBound(1)+1,cArr);
                case focalType.MEAN:
                    crl = createFocalWindowCircle(radius, out outLst);
                    sumCircle = (from int i in crl select System.Convert.ToDouble(i)).Sum();
                    cArr = (from int i in crl select System.Convert.ToDouble(i)).ToArray();
                    IFunctionRasterDataset conRsMean = convolutionRasterFunction(iR1, crl.GetUpperBound(0) + 1, crl.GetUpperBound(1) + 1, cArr);
                    return calcArithmaticFunction(conRsMean, sumCircle, esriRasterArithmeticOperation.esriRasterDivide);
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMedian();
                    break;
                case focalType.VARIANCE:
                    crl = createFocalWindowCircle(radius, out outLst);
                    cArr = (from int i in crl select System.Convert.ToDouble(i)).ToArray();
                    double sumCr = cArr.Sum();
                    IFunctionRasterDataset rs2 = calcMathRasterFunction(iR1, transType.SQUARED);
                    IFunctionRasterDataset sumRs2 = convolutionRasterFunction(rs2, crl.GetUpperBound(0) + 1, crl.GetUpperBound(1) + 1, cArr);
                    IFunctionRasterDataset sumRs2M = calcArithmaticFunction(sumRs2, sumCr, esriRasterArithmeticOperation.esriRasterDivide);
                    IFunctionRasterDataset sumRs = convolutionRasterFunction(iR1, crl.GetUpperBound(0) + 1, crl.GetUpperBound(1) + 1, cArr);
                    IFunctionRasterDataset sumRsSquared = calcMathRasterFunction(sumRs, transType.SQUARED);
                    IFunctionRasterDataset difRs = calcArithmaticFunction(sumRsSquared, sumRs2, esriRasterArithmeticOperation.esriRasterMinus);
                    return calcArithmaticFunction(difRs, sumCr, esriRasterArithmeticOperation.esriRasterDivide);
                case focalType.STD:
                    IRaster var = createRaster(calcFocalStatisticsFunction(iR1, radius, focalType.VARIANCE));
                    return calcMathRasterFunction(var, transType.SQRT);
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperEntropy();
                    break;
                case focalType.ASM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperProbability();
                    break;
                default:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMax();
                    break;
            }
            FunctionRasters.FocalFunctionArguments args = new FunctionRasters.FocalFunctionArguments(this);
            args.Radius = radius;
            args.InRaster = iR1;
            args.Operation = statType;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        private double getNFromCircle(int radius)
        {
            int n = 0;
            List<int[]> iterLst = new List<int[]>();
            int[,] kern = createFocalWindowCircle(radius, out iterLst);
            for (int i = 0; i <= kern.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= kern.GetUpperBound(1); j++)
                {
                    int vl = kern[i, j];
                    n += vl;
                }
            }
            return n;
        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="clm">number of columns (cells)</param>
        /// <param name="rws">number of rows</param>
        /// <param name="statType">the type of operation</param>
        /// <param name="landType">the type of metric</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IFunctionRasterDataset calcLandscapeFunction(object inRaster, int clm, int rws, focalType statType, landscapeType landType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.landscapeFunctionDataset();
            FunctionRasters.LandscapeFunctionArguments args = new FunctionRasters.LandscapeFunctionArguments(this);
            args.WindowType = windowType.RECTANGLE;
            args.Rows = rws;
            args.Columns = clm;
            args.InRaster = iR1;
            args.Operation = statType;
            args.LandscapeType = landType;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="radius">number of cells that make up the radius of the moving window</param>
        /// <param name="statType">the type of operation</param>
        /// <param name="landType">the type of metric</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IFunctionRasterDataset calcLandscapeFunction(object inRaster, int radius, focalType statType, landscapeType landType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.landscapeFunctionDataset();
            FunctionRasters.LandscapeFunctionArguments args = new FunctionRasters.LandscapeFunctionArguments(this);
            args.WindowType = windowType.CIRCLE;
            args.Radius = radius;
            args.InRaster = iR1;
            args.Operation = statType;
            args.LandscapeType = landType;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Returns a IRaster given either a path, IRasterdataset, or IRaster
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster returnRaster(object inRaster)
        {
            if (inRaster == null)
            {
                return null;
            }
            else
            {
                IFunctionRasterDataset fDset = createIdentityRaster(inRaster);
                return createRaster(fDset);
            }
        }

        public IRaster createRaster(IFunctionRasterDataset funcDatset)
        {
            if (funcDatset == null)
            {
                return null;
            }
            else
            {
                IRasterDataset3 rsD3 = (IRasterDataset3)funcDatset;
                return rsD3.CreateFullRaster();
            }
        }

        public IFunctionRasterDataset createIdentityRaster(object inRaster,rstPixelType pType)
        {
            if (inRaster == null)
            {
                return null;
            }
            else
            {
                IFunctionRasterDataset trd = createIdentityRaster(inRaster);
                IRasterFunction rsFunc = new IdentityFunctionClass();
                rsFunc.Bind(trd);
                string tempAr = funcDir + "\\" + FuncCnt + ".afr";
                IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
                IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
                frDsetName.FullName = tempAr;
                frDset.FullName = (IName)frDsetName;
                rsFunc.PixelType = pType;
                //rsFunc.Update();
                frDset.Init(rsFunc, trd);
                return frDset;
            }
        }

        public IFunctionRasterDataset createIdentityRaster(object inRaster)
        {
            if (inRaster == null)
            {
                return null;
            }
            else
            {
                if (inRaster is string)
                {
                    if (System.IO.Path.GetExtension(inRaster.ToString()).ToLower() == ".bch")
                    {
                        batchCalculations btch = new batchCalculations(this, new Forms.RunningProcess.frmRunningProcessDialog(true));
                        btch.BatchPath = inRaster.ToString();
                        btch.loadBatchFile();
                        btch.runBatch();
                        string nm, desc;
                        IFunctionRasterDataset rs;
                        btch.GetFinalRaster(out nm, out rs, out desc);
                        //inRaster = rs;
                        return rs;
                    }
                    else
                    {
                        string oStr;
                        inRaster = openRasterDataset(inRaster.ToString(), out oStr);
                        if (inRaster == null) return null;
                        else
                        {
                            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
                            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
                            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
                            frDsetName.FullName = tempAr;
                            frDset.FullName = (IName)frDsetName;
                            IRasterFunction rsFunc = (IRasterFunction)new IdentityFunctionClass();
                            frDset.Init(rsFunc, inRaster);
                            return frDset;
                        }
                    }
                }
                else if (inRaster is FunctionRasterDataset)
                {
                    return (IFunctionRasterDataset)inRaster;
                    //inRaster = (IRasterDataset)inRaster;
                }
                else
                {
                    string tempAr = funcDir + "\\" + FuncCnt + ".afr";
                    IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
                    IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
                    frDsetName.FullName = tempAr;
                    frDset.FullName = (IName)frDsetName;
                    IRasterFunction rsFunc = (IRasterFunction)new IdentityFunctionClass();
                    frDset.Init(rsFunc, inRaster);
                    return frDset;
                }
            }
        }

        public IRaster returnRaster(object inRaster, rstPixelType pType)
        {
            if (inRaster == null)
            {
                return null;
            }
            else
            {
                IFunctionRasterDataset fDset = createIdentityRaster(inRaster, pType);
                return createRaster(fDset);
            }
        }
        /// <summary>
        /// samples all bands of a given raster given a point feature class. Appends those values to a field named by the raster/band.
        /// </summary>
        /// <param name="inFtrCls">Point feature class that is used to sample</param>
        /// <param name="sampleRst">Raster dataset that is going to be sampled</param>
        /// <returns>a list of all the created field names</returns>
        public string[] sampleRaster(IFeatureClass inFtrCls, object sampleRst, string inName, string bandFldName=null)
        {
            IFunctionRasterDataset rs = createIdentityRaster(sampleRst);
            int bndCnt = rs.RasterInfo.BandCount;
            string[] fldsNames;
            int[] fldsIndex;
            if (bandFldName != null)
            {
                fldsNames = new string[1];
                fldsIndex = new int[1];
                string fldName1 = "BAND_VL";
                fldName1 = geoUtil.createField(inFtrCls, fldName1, esriFieldType.esriFieldTypeDouble);
                fldsNames[0] = fldName1;
                fldsIndex[0] = inFtrCls.FindField(fldName1);
            }
            else
            {
                string rsName = inName;
                if (rsName == null)
                {
                    rsName = "VL";
                }
                fldsNames = new string[bndCnt];
                fldsIndex = new int[bndCnt];
                for (int i = 0; i < fldsNames.Length; i++)
                {
                    string fldName = rsName + "_Band" + (i + 1).ToString();
                    //fldName = geoUtil.getSafeFieldName(inFtrCls, fldName);
                    esriFieldType fldType = esriFieldType.esriFieldTypeDouble;
                    fldName = geoUtil.createField(inFtrCls, fldName, fldType);
                    fldsNames[i] = fldName;
                    fldsIndex[i] = inFtrCls.FindField(fldName);
                }
            }
            IGeometry geo = (IGeometry)rs.RasterInfo.Extent;
            IPoint ul = rs.RasterInfo.Extent.UpperLeft;
            IPnt cellSize = rs.RasterInfo.CellSize;
            int width = rs.RasterInfo.Width;
            int height = rs.RasterInfo.Height;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = geo;
            spFlt.GeometryField = inFtrCls.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor sCur = inFtrCls.Update(spFlt, true);   
            int bandFldIndex=0;
            if(bandFldName!=null)
            {
                bandFldIndex = sCur.FindField(bandFldName);
            }
            IFeature sRow = sCur.NextFeature();
            IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            IRasterBand rsB;
            IPnt pSize = new PntClass();
            pSize.SetCoords(1,1);

            IPnt pLoc = new PntClass();
            while (sRow != null)
            {
                geo = sRow.Shape;
                IPoint pnt = (IPoint)geo;
                int clm,rw;
                getClmRw(ul, cellSize, pnt, out clm, out rw);
                pLoc.SetCoords(clm,rw);
                for (int i = 0; i < fldsNames.Length; i++)
                {
                    int bnd = i;
                    if (bandFldName != null)
                    {
                        bnd = System.Convert.ToInt32(sRow.get_Value(bandFldIndex)) - 1;
                        if (bnd < 0 || bnd > bndCnt) bnd = 0;
                    }
                    rsB = rsBc.Item(bnd);
                    IRawPixels rpix = (IRawPixels)rsB;
                    IPixelBlock pb = rpix.CreatePixelBlock(pSize);
                    rpix.Read(pLoc, pb);
                    object rsVl = pb.GetVal(0, 0, 0);
                    int fldIndex = fldsIndex[i];
                    try
                    {
                        sRow.set_Value(fldIndex, rsVl);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                sCur.UpdateFeature(sRow);
                sRow = sCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
            return fldsNames;

        }

        private void getClmRw(IPoint ul, IPnt cellSize, IPoint pnt, out int clm, out int rw)
        {
            double lengthX = System.Math.Abs(pnt.X - ul.X);
            double lengthY = System.Math.Abs(ul.Y - pnt.Y);
            clm = ((int)(lengthX / cellSize.X));
            rw = ((int)(lengthY / cellSize.Y));
        }
        /// <summary>
        /// sample a raster using a given offset
        /// </summary>
        /// <param name="inFtrCls"></param>
        /// <param name="sampleRst"></param>
        /// <param name="inName"></param>
        /// <param name="azmithDistance"></param>
        /// <param name="typeOfCluster"></param>
        /// <returns></returns>
        public string[] sampleRaster(IFeatureClass inFtrCls, IRaster sampleRst, string inName, Dictionary<double,double> azmithDistance, clusterType typeOfCluster)
        {
            IRaster2 sr = (IRaster2)sampleRst;
            IRasterBandCollection rsBC = (IRasterBandCollection)sr;
            IEnumRasterBand rsBE = rsBC.Bands;
            IRasterBand rsB = rsBE.Next();
            string rsName = inName;
            if (rsName == null)
            {
                rsName = ((IDataset)sr.RasterDataset).Name;
            }
            int cntB = 0;
            int[] fldIndex = new int[rsBC.Count];
            string[] fldNames = new string[rsBC.Count];
            while (rsB != null)
            {
                string fldName = rsName + "_Band" + (cntB + 1).ToString();
                //fldName = geoUtil.getSafeFieldName(inFtrCls, fldName);
                esriFieldType fldType = esriFieldType.esriFieldTypeDouble;
                fldName = geoUtil.createField(inFtrCls, fldName, fldType);
                fldNames[cntB] = fldName;
                fldIndex[cntB] = inFtrCls.FindField(fldName);
                cntB++;
                rsB = rsBE.Next();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rsBE);
            IGeometry geo = (IGeometry)((IRasterProps)sampleRst).Extent;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = geo;
            spFlt.GeometryField = inFtrCls.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor sCur = inFtrCls.Update(spFlt, true);
            IFeature sRow = sCur.NextFeature();
            while (sRow != null)
            {
                geo = sRow.Shape;
                IPoint pnt = (IPoint)geo;
                int x, y;
                x = 0;
                y = 0;
                for (int i = 0; i < ((IRasterBandCollection)sr).Count; i++)
                {
                    double rsVl = 0;
                    List<double> rsVlLst = new List<double>();
                    try
                    {
                        sr.MapToPixel(pnt.X, pnt.Y, out x, out y);
                        rsVlLst.Add(System.Convert.ToDouble(sr.GetPixelValue(i, x, y)));
                        foreach (KeyValuePair<double, double> kVp in azmithDistance)
                        {
                            double az = kVp.Key;
                            double ds = kVp.Value;
                            double nX = pnt.X + (System.Math.Sin(az * Math.PI / 180) * ds);
                            double nY = pnt.Y + (System.Math.Cos(az * Math.PI / 180) * ds);
                            sr.MapToPixel(nX, nY, out x, out y);
                            rsVlLst.Add(System.Convert.ToDouble(sr.GetPixelValue(i, x, y)));
                        }
                        switch (typeOfCluster)
                        {
                            case clusterType.SUM:
                                rsVl = rsVlLst.Sum();
                                break;
                            case clusterType.MEAN:
                                rsVl = rsVlLst.Average();
                                break;
                            case clusterType.MEDIAN:
                                rsVlLst.Sort();
                                rsVl = rsVlLst[(rsVlLst.Count-1) / 2];
                                break;
                            case clusterType.MODE:
                                Dictionary<double, int> cntDic = new Dictionary<double, int>();
                                int maxLc = 0;
                                double maxKy = rsVlLst[0];
                                foreach (double d in rsVlLst)
                                {
                                    if (cntDic.ContainsKey(d))
                                    {
                                        int cntVl = cntDic[d] + 1;
                                        if(cntVl>maxLc)
                                        {
                                            maxLc = cntVl;
                                            maxKy = d;
                                        }
                                        cntDic[d] = cntVl;
                                    }
                                    else
                                    {
                                        cntDic.Add(d, 1);
                                    }
                                }
                                rsVl = maxKy;
                                break;
                            default:
                                break;
                        }
                        sRow.set_Value(fldIndex[i], rsVl);
                    }
                    catch
                    {
                        Console.WriteLine(rsVl.ToString());
                    }

                }
                sCur.UpdateFeature(sRow);
                sRow = sCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
            return fldNames;

        }
        /// <summary>
        /// Remaps the values of a given raster to new set of values
        /// </summary>
        /// <param name="inRaster">input raster</param>
        /// <param name="filter">a remap filter</param>
        /// <returns>IRaster with remaped values</returns>
        public IFunctionRasterDataset calcRemapFunction(object inRaster, IRemapFilter filter)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            IDoubleArray rangeArray = new DoubleArrayClass();
            IDoubleArray valueArray = new DoubleArrayClass();
            double min,max,vl;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new RemapFunctionClass();
            IRemapFunctionArguments args = new RemapFunctionArgumentsClass();
            args.AllowUnmatched = filter.AllowUnmatched;
            args.Raster = rRst;
            for (int i = 0; i < filter.ClassCount; i++)
            {
                filter.QueryClass(i, out min, out max, out vl);
                rangeArray.Add(min);
                rangeArray.Add(max);
                valueArray.Add(vl);
            }
            args.InputRanges = rangeArray;
            args.OutputValues = valueArray;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        /// <summary>
        /// Calculates a trend raster from double filter
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="doubleFilter">a double array of plan values</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcTrendFunction(object inRaster, double[] doubleFilter)
        {
            IDoubleArray dbArray = new DoubleArrayClass();
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new TrendFunctionClass();
            ITrendFunctionArguments args = new TrendFunctionArgumentsClass();
            args.Raster = createIdentityRaster(inRaster,rstPixelType.PT_FLOAT);
            for (int i = 0; i < doubleFilter.Length; i++)
            {
                dbArray.Add(doubleFilter[i]);
            }
            args.PlaneParameters = dbArray;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset calcPolytomousLogisticRegressFunction(object inRaster, double[][] slopes)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.polytomousLogisticFunctionDataset();
            FunctionRasters.polytomousLogisticFunctionArguments args = new FunctionRasters.polytomousLogisticFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.Slopes = slopes;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset calcSoftMaxNnetFunction(object inRaster, Statistics.dataPrepSoftMaxPlr sm)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.softMaxFunctionDataset();
            FunctionRasters.softMaxFunctionArguments args = new FunctionRasters.softMaxFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.LogitModel = sm;           
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset calcPrincipleComponentsFunction(object inRaster, Statistics.dataPrepPrincipleComponents pca)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.pcaDataset();
            FunctionRasters.pcaArguments args = new FunctionRasters.pcaArguments(this);
            args.InRasterCoefficients = rRst;
            args.PCA = pca;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcMosaicFunction(IRaster[] inRasters, mergeType mType)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (mType)
            {
                case mergeType.LAST:
                    rsFunc = new FunctionRasters.mergeFunctionDatasetLast();
                    break;
                case mergeType.MIN:
                    rsFunc = new FunctionRasters.mergeFunctionDatasetMin();
                    break;
                case mergeType.MAX:
                    rsFunc = new FunctionRasters.mergeFunctionDatasetMax();
                    break;
                case mergeType.MEAN:
                    rsFunc = new FunctionRasters.mergeFunctionDatasetMean();
                    break;
                default:
                    rsFunc = new FunctionRasters.mergeFunctionDatasetFirst();
                    break;
            }
            FunctionRasters.mergeFunctionArguments args = new FunctionRasters.mergeFunctionArguments(this);
            args.InRaster = inRasters;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcClustFunctionKmean(object inRaster, Statistics.dataPrepClusterKmean clus)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.clusterFunctionKmeanDataset();
            FunctionRasters.clusterFunctionArguments args = new FunctionRasters.clusterFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.ClusterModel = clus;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcClustFunctionBinary(object inRaster, Statistics.dataPrepClusterBinary clus)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.clusterFunctionBinaryDataset();
            FunctionRasters.clusterFunctionArguments args = new FunctionRasters.clusterFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.ClusterModel = clus;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcClustFunctionGaussian(object inRaster, Statistics.dataPrepClusterGaussian clus)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.clusterFunctionGaussianDataset();
            FunctionRasters.clusterFunctionArguments args = new FunctionRasters.clusterFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.ClusterModel = clus;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcPairedTTestFunction(object inRaster, Statistics.dataPrepPairedTTest pairedttest)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.pairedttestFunctionDataset();
            FunctionRasters.pairedttestFunctionArguments args = new FunctionRasters.pairedttestFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.TTestModel = pairedttest;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcLdaFunction(object inRaster, Statistics.dataPrepDiscriminantAnalysisLda lda)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.ldaFunctionDataset();
            FunctionRasters.ldaFunctionArguments args = new FunctionRasters.ldaFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.LDAModel = lda;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcKdaFunction(object inRaster, Statistics.dataPrepDiscriminantAnalysis kda)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.kdaFunctionDataset();
            FunctionRasters.kdaFunctionArguments args = new FunctionRasters.kdaFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.KDAModel = kda;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcGlmFunction(object inRaster, Statistics.dataPrepGlm glm)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.glmFunctionDataset();
            FunctionRasters.glmFunctionArguments args = new FunctionRasters.glmFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.GlmModel = glm;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcTTestFunction(object inRaster, Statistics.dataPrepTTest ttest)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.ttestFunctionDataset();
            FunctionRasters.ttestFunctionArguments args = new FunctionRasters.ttestFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.TTestModel = ttest;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public IFunctionRasterDataset calcRandomForestFunction(object inRaster, Statistics.dataPrepRandomForest rf)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.randomForestDataset();
            FunctionRasters.randomForestArguments args = new FunctionRasters.randomForestArguments(this);
            args.InRasterCoefficients = rRst;
            args.RandomForestModel = rf;
            frDset.Init(rsFunc, args);

            IRasterInfo2 rsInfo2 = (IRasterInfo2)frDset.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            if (rf.Regression)
            {
                double pMin = rf.computNew(rf.minValues)[0];
                double pMax = rf.computNew(rf.MaxValues)[0];
                double pMean = (pMax-pMin)/2;
                rsStats.Maximum = rf.maxValues[0];
                rsStats.Minimum = rf.minValues[0];
                rsStats.Mean = pMean;
                rsStats.StandardDeviation = pMean * 0.5;
            }
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return frDset;

        }
        /// <summary>
        /// regresses sums an intercept value to the sum product of a series of raster bands and corresponding slope values. Number of bands and slope values must match
        /// </summary>
        /// <param name="inRaster">string IRaster, or IRasterDataset that has the same number of bands as the slopes array has values</param>
        /// <param name="slopes">double[] representing the corresponding slope values the first value in the array is the intercept</param>
        /// <returns></returns>
        public IFunctionRasterDataset calcRegressFunction(object inRaster, List<float[]> slopes)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc =  new FunctionRasters.regressionFunctionDataset();      
            FunctionRasters.regressionFunctionArguments args = new FunctionRasters.regressionFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.Slopes = slopes;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset calcCensoredRegressFunction(object inRaster, List<float[]> slopes,float lowerLimit=0)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.tobitFunctionDataset();
            FunctionRasters.tobitFunctionArguments args = new FunctionRasters.tobitFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.Slopes = slopes;
            args.CensoredValue = lowerLimit;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset calcTobitRegressFunction(object inRaster, string modelPath, float lowerLimit = 0)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.tobitFunctionDataset();
            FunctionRasters.tobitFunctionArguments args = new FunctionRasters.tobitFunctionArguments(this);
            args.InRasterCoefficients = rRst;
            args.TobitModelPath = modelPath;
            args.CensoredValue = lowerLimit;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        /// <summary>
        /// Remaps values greater than or equal to the input vl to 1. Values less than vl =0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or raster to compare against</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcGreaterEqualFunction(object inRaster, object compareRaster)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IFunctionRasterDataset outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                //Console.WriteLine("Is Number");
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                rstPixelType pType = rs.RasterInfo.PixelType;
                double max, min;
                max = 0;
                min = 0;
                #region set min max
                getMinMax(pType, ref max, ref min);
                #endregion
                rFilt.AddClass(min, vl, 0);
                rFilt.AddClass(vl, max, 1);
                outRs = calcRemapFunction(rs, rFilt);
            }
            else
            {
                //Console.WriteLine("Is Raster");
                IFunctionRasterDataset minRst = calcArithmaticFunction(rs, compareRaster, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcGreaterEqualFunction(minRst, 0);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// Remaps values greater than to the input vl to 1. Values less than vl =0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or raster to compare against</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcGreaterFunction(object inRaster, object compareRaster)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IFunctionRasterDataset outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                //Console.WriteLine("Is Number");
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                rstPixelType pType = rs.RasterInfo.PixelType;
                double max, min;
                max = 0;
                min = 0;
                #region set min max
                getMinMax(pType, ref max, ref min);
                #endregion
                double vlP1 = vl + 0.000001;
                rFilt.AddClass(min, vlP1, 0);
                rFilt.AddClass(vlP1, max, 1);
                outRs = calcRemapFunction(rs, rFilt);
            }
            else
            {
                IFunctionRasterDataset minRst = calcArithmaticFunction(rs, compareRaster, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcGreaterFunction(minRst, 0);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// returns the potential max min values of a raster given a pixel type by reference
        /// </summary>
        /// <param name="pType">raser pixel type</param>
        /// <param name="max">reference value max</param>
        /// <param name="min">reference value min</param>
        private void getMinMax(rstPixelType pType, ref double max, ref double min)
        {
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    max = 128;
                    min = -128;
                    break;
                case rstPixelType.PT_CLONG:
                case rstPixelType.PT_COMPLEX:
                case rstPixelType.PT_CSHORT:
                case rstPixelType.PT_DCOMPLEX:
                    max = 4294967296;
                    min = -1;
                    break;
                case rstPixelType.PT_LONG:
                case rstPixelType.PT_FLOAT:
                    max = 2147483648;
                    min = -2147483649;
                    break;
                case rstPixelType.PT_SHORT:
                    max = 32768;
                    min = -32769;
                    break;
                case rstPixelType.PT_U1:
                    max = 2;
                    min = -1;
                    break;
                case rstPixelType.PT_U2:
                    max = 4;
                    min = -1;
                    break;
                case rstPixelType.PT_U4:
                    max = 16;
                    min = -1;
                    break;
                case rstPixelType.PT_UCHAR:
                    max = 256;
                    min = -1;
                    break;
                case rstPixelType.PT_ULONG:
                case rstPixelType.PT_UNKNOWN:
                    max = 4294967296;
                    min = -4294967297;
                    break;
                case rstPixelType.PT_USHORT:
                    max = 65536;
                    min = -1;
                    break;
                default:
                    double b64 = Math.Pow(2, 64);
                    max = b64;
                    min = (b64 * -1) + -1;
                    break;
            }
        }
        /// <summary>
        /// Remaps values less than or equal to the compareRaster to 1. Values greater than compareRaster = 0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or Raserter to compare against</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcLessEqualFunction(object inRaster, object compareRaster)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IFunctionRasterDataset outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                rstPixelType pType = rs.RasterInfo.PixelType;
                double max, min;
                max = 0;
                min = 0;
                #region set min max
                getMinMax(pType, ref max, ref min);
                #endregion
                double vlP1 = vl + 0.000001;
                rFilt.AddClass(min, vlP1, 1);
                rFilt.AddClass(vlP1, max, 0);
                outRs = calcRemapFunction(rs, rFilt);
            }
            else
            {
                IFunctionRasterDataset minRst = calcArithmaticFunction(rs, compareRaster, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcLessEqualFunction(minRst, 0);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// Remaps values less than to the compareRaster to 1. Values greater than compareRaster = 0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or Raserter to compare against</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcLessFunction(object inRaster, object compareRaster)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IFunctionRasterDataset outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                rstPixelType pType = rs.RasterInfo.PixelType;
                double max, min;
                max = 0;
                min = 0;
                #region set min max
                getMinMax(pType, ref max, ref min);
                #endregion
                rFilt.AddClass(min, vl, 1);
                rFilt.AddClass(vl, max, 0);
                outRs = calcRemapFunction(rs, rFilt);
            }
            else
            {
                IFunctionRasterDataset minRst = calcArithmaticFunction(rs, compareRaster, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcLessFunction(minRst, 0);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// Remaps values equal to the compare Raster or value cell values = 1. Values less than or greater than compare raster or value = 0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value to compare against</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset calcEqualFunction(object inRaster, object compareRaster)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IFunctionRasterDataset outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                rstPixelType pType = rs.RasterInfo.PixelType;
                double max, min;
                max = 0;
                min = 0;
                #region set min max
                getMinMax(pType, ref max, ref min);
                #endregion
                double vlP1 = vl + 0.000001;
                rFilt.AddClass(min, vl, 0);
                rFilt.AddClass(vl, vlP1, 1);
                rFilt.AddClass(vlP1, max, 0);
                outRs = calcRemapFunction(rs, rFilt);
            }
            else
            {
                IFunctionRasterDataset minRst = calcArithmaticFunction(rs, compareRaster, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcEqualFunction(minRst, 0);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        public IFunctionRasterDataset calcCombineRasterFunction(IRaster inRasters)
        {
            IFunctionRasterDataset funcRaster = createIdentityRaster(inRasters);
            return calcCombineRasterFunction(funcRaster);
        }
        public IFunctionRasterDataset calcCombineRasterFunction(IRaster[] inRasters)
        {
            IFunctionRasterDataset funcRaster = compositeBandFunction(inRasters);
            return calcCombineRasterFunction(funcRaster);
        }
        public IFunctionRasterDataset calcCombineRasterFunction(IFunctionRasterDataset funcRs)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.combineFunctionDataset();
            FunctionRasters.combineFunctionArguments args = new FunctionRasters.combineFunctionArguments(this);
            args.InRasterDataset = funcRs;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        public ITable calcCombinRasterFunctionTable(object raster,IWorkspace wks, string tblName)
        {
            IFunctionRasterDataset fDset = createIdentityRaster(raster);
            IFields flds = new FieldsClass();
            IFieldsEdit fldsE = (IFieldsEdit)flds;
            IField fld1 = new FieldClass();
            IFieldEdit fldE1 = (IFieldEdit)fld1;
            fldE1.Name_2 = "VALUE";
            fldE1.Type_2 = esriFieldType.esriFieldTypeInteger;
            fldsE.AddField(fld1);
            IField fld2 = new FieldClass();
            IFieldEdit fldE2 = (IFieldEdit)fld2;
            fldE2.Name_2 = "COUNT";
            fldE2.Type_2 = esriFieldType.esriFieldTypeInteger;
            fldsE.AddField(fld2);
            
            for (int i = 1; i <= fDset.RasterInfo.BandCount; i++)
            {
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "Band_" + i.ToString();
                fldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                fldsE.AddField(fld);
            }
            ITable outTable = geoUtil.createTable(wks, tblName, flds);
            Dictionary<string,object[]> uDic = getUniqueRasterValues(fDset);
            ICursor cur = outTable.Insert(true);
            int vIndex = cur.FindField("VALUE");
            int cIndex = cur.FindField("COUNT");
            int[] bIndex = new int[fDset.RasterInfo.BandCount];
            for (int i = 1; i <= bIndex.Length; i++)
			{
                bIndex[i-1] = cur.FindField("Band_" + i.ToString());
			}
            IRowBuffer rBuff = outTable.CreateRowBuffer();
            int cnt = 0;
            foreach (KeyValuePair<string,object[]> rw in uDic)
            {
                object[] vlArr = rw.Value;
                rBuff.set_Value(vIndex, cnt);
                rBuff.set_Value(cIndex, vlArr[0]);
                for (int i = 0; i < bIndex.Length; i++)
                {
                    rBuff.set_Value(bIndex[i], vlArr[i+1]);
                }
                cur.InsertRow(rBuff);
                cnt++;
            }
            cur.Flush();
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cur);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rBuff);
            return outTable;
        }

        private Dictionary<string,object[]> getUniqueRasterValues(IFunctionRasterDataset fDset,int IntBlockWidth=512, int IntBlockHeight=512)
        {
            Dictionary<string,object[]> outDic = new Dictionary<string,object[]>();
            //System.Data.DataColumn vlClm = outTbl.Columns.Add("VALUE", typeof(ulong));
            //vlClm.AutoIncrement = true;
            //outTbl.Columns.Add("COUNT", typeof(ulong));
            
            int tRasterWidth = fDset.RasterInfo.Width;
            int tRasterHeight = fDset.RasterInfo.Height;
            int bndCnt = fDset.RasterInfo.BandCount;
            //for (int i = 0; i < bndCnt; i++)
            //{
            //    outTbl.Columns.Add("Band_" + i.ToString(), typeof(double));
            //}
            IPnt pntSize = new PntClass();
            IPnt topLeft = new PntClass();
            int intW = IntBlockWidth;
            int intH = IntBlockHeight;
            int nw = intW;
            int nh = intH;
            IRaster rs = createRaster(fDset);
            IRasterProps rsPropOut = (IRasterProps)rs;
            IEnvelope env = fDset.RasterInfo.Extent;
            IPnt mcellSize = fDset.RasterInfo.CellSize;
            rsPropOut.Extent = env;
            rsPropOut.Width = (int)(env.Width / mcellSize.X);
            rsPropOut.Height = (int)(env.Height / mcellSize.Y);
            int cnt = 0;
            for (int pbh = 0; pbh < tRasterHeight; pbh += intW)
            {

                for (int pbw = 0; pbw < tRasterWidth; pbw += intH)
                {
                    topLeft.SetCoords(pbw, pbh);
                    getPbWidthHeight(tRasterWidth, tRasterHeight, topLeft, intW, intH, out nw, out nh);
                    pntSize.SetCoords(nw, nh);
                    IPixelBlock3 inPb = (IPixelBlock3)rs.CreatePixelBlock(pntSize);
                    rs.Read(topLeft, (IPixelBlock)inPb);
                    for (int r = 0; r < nh; r++)
                    {
                        for (int c = 0; c < nw; c++)
                        {
                            bool ch = true;
                            string[] svlArr = new string[bndCnt];
                            object[] vlArr = new object[bndCnt+1];
                            for (int b = 0; b < bndCnt; b++)
                            {
                                object vl = inPb.GetVal(b, c, r);
                                if (vl == null)
                                {
                                    ch = false;
                                    break;
                                }
                                else
                                {
                                    svlArr[b] = vl.ToString();
                                    vlArr[b + 1] = vl;
                                }

                            }
                            if (ch)
                            {
                                string ky = String.Join(";",svlArr);
                                object[] vlArr2;
                                if (outDic.TryGetValue(ky, out vlArr2))
                                {
                                    vlArr2[0] = System.Convert.ToInt32(vlArr2[0]) + 1;
                                    outDic[ky] = vlArr2;
                                }
                                else
                                {
                                    vlArr[0] = 1;
                                    outDic.Add(ky, vlArr);
                                }
                            }

                        }
                        
                    }
                    
                    
                }
            }
            return outDic;

        }
        public IFunctionRasterDataset calcCombineRasterFunction(IRasterBandCollection rsbc)
        {
            return compositeBandFunction(rsbc);
        }
        /// <summary>
        /// Creates a constant raster given a template raster and a double value
        /// </summary>
        /// <param name="templateRaster">Raster that has the extent and cell size of the desired constant raster</param>
        /// <param name="rasterValue">double value that all cells will have</param>
        /// <returns>a constant raster IRaster</returns>
        public IFunctionRasterDataset constantRasterFunction(object templateRaster, double rasterValue)
        {
            IFunctionRasterDataset tDset = createIdentityRaster(templateRaster, rstPixelType.PT_FLOAT);
            IConstantFunctionArguments rasterFunctionArguments = (IConstantFunctionArguments)new ConstantFunctionArguments();
            rasterFunctionArguments.Constant = rasterValue;
            rasterFunctionArguments.RasterInfo = tDset.RasterInfo;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ConstantFunction();
            frDset.Init(rsFunc, rasterFunctionArguments);
            IRasterInfo2 rsInfo2 = (IRasterInfo2)frDset.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = rasterValue;
            rsStats.Maximum = rasterValue;
            rsStats.Minimum = rasterValue;
            rsStats.StandardDeviation = rasterValue;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return frDset;

        }
        public IFunctionRasterDataset constantRasterFunction(object templateRaster, double rasterValue, rstPixelType outPixelType)
        {

            IFunctionRasterDataset tDset = createIdentityRaster(templateRaster);
            IRasterInfo rsInfo = tDset.RasterInfo;
            //rsInfo.PixelType = outPixelType;
            IConstantFunctionArguments rasterFunctionArguments = (IConstantFunctionArguments)new ConstantFunctionArguments();
            rasterFunctionArguments.Constant = rasterValue;
            rasterFunctionArguments.RasterInfo = rsInfo;
            IRaster tRs = createRaster(tDset);
            rasterFunctionArguments.Init(tRs, rasterValue);
            IFunctionRasterDataset dSet = createIdentityRaster(tRs);
            dSet.RasterInfo.PixelType = outPixelType;
            return dSet;
            //string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            //IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            //IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            //frDsetName.FullName = tempAr;
            //frDset.FullName = (IName)frDsetName;
            //IRasterFunction rsFunc = new ConstantFunction();
            //frDset.Init(rsFunc, rasterFunctionArguments);
            //return frDset;
        }
        public IFunctionRasterDataset constantRasterFunction(IRaster template, IEnvelope NewExtent, double rasterValue, IPnt cellSize)
        {
            IFunctionRasterDataset tDset = createIdentityRaster(template, rstPixelType.PT_FLOAT);
            IConstantFunctionArguments rasterFunctionArguments = (IConstantFunctionArguments)new ConstantFunctionArguments();
            rasterFunctionArguments.Constant = rasterValue;
            IRasterInfo rsInfo = tDset.RasterInfo;
            rsInfo.NativeExtent = NewExtent;
            rsInfo.Extent = NewExtent;
            rsInfo.CellSize = cellSize;
            rasterFunctionArguments.RasterInfo = rsInfo;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ConstantFunction();
            frDset.Init(rsFunc, rasterFunctionArguments);
            IRasterInfo2 rsInfo2 = (IRasterInfo2)frDset.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = rasterValue;
            rsStats.Maximum = rasterValue;
            rsStats.Minimum = rasterValue;
            rsStats.StandardDeviation = rasterValue;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return frDset;
        }
        /// <summary>
        /// Used as an if then else statement. The condRaster raster is meant to have values of 1 or 0. If a cell within the input raster has a value 1
        /// then the cell gets the value of inRaster1's corresponding cell. Otherwise that cell gets the value of the inRaster2's corresponding cell.
        /// </summary>
        /// <param name="condRaster">string path, IRaster, IRasterDataset thats cell values are 0 or 1</param>
        /// <param name="inRaster1">string path, IRaster, IRasterDataset, or a numeric value</param>
        /// <param name="inRaster2">string path, IRaster, IRasterDataset, or a numeric value</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset conditionalRasterFunction(object condRaster, object trueRaster, object falseRaster)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            FunctionRasters.conditionalFunctionDataset rsFunc = new FunctionRasters.conditionalFunctionDataset();
            FunctionRasters.conditionalFunctionArguments args = new FunctionRasters.conditionalFunctionArguments(this);
            IFunctionRasterDataset conRs = createIdentityRaster(condRaster);
            if (conRs==null)
            {
                Console.WriteLine("Condition Raster must be a raster");
                return null;
            }
            IFunctionRasterDataset iR1, iR2;
            if (isNumeric(trueRaster.ToString()) && !isNumeric(falseRaster.ToString()))
            {
                iR2 = createIdentityRaster(falseRaster,rstPixelType.PT_FLOAT);
                iR1 = constantRasterFunction(conRs, System.Convert.ToDouble(trueRaster));
            }
            else if (isNumeric(falseRaster.ToString()) && !isNumeric(trueRaster.ToString()))
            {
                iR1 = createIdentityRaster(trueRaster,rstPixelType.PT_FLOAT);
                iR2 = constantRasterFunction(conRs, System.Convert.ToDouble(falseRaster));
            }
            else if (isNumeric(falseRaster.ToString()) && isNumeric(trueRaster.ToString()))
            {
                iR1 = constantRasterFunction(conRs, System.Convert.ToDouble(trueRaster));
                iR2 = constantRasterFunction(conRs, System.Convert.ToDouble(falseRaster));
            }
            else
            {
                iR1 = createIdentityRaster(trueRaster, rstPixelType.PT_FLOAT);
                iR2 = createIdentityRaster(falseRaster,rstPixelType.PT_FLOAT);
            }
            args.ConditionalRaster = conRs;
            args.TrueRaster = iR1;
            args.FalseRaster = iR2;
            frDset.Init(rsFunc, args);
            return frDset;
            
        }
        public IFunctionRasterDataset localRescalefunction(object inRaster, rasterUtil.localRescaleType op)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (op)
            {
                case localRescaleType.PrcTile:
                    rsFunc = new FunctionRasters.localPrctileDataset();
                    break;
                default:
                    break;
            }
            FunctionRasters.LocalRescaleFunctionArguments args = new FunctionRasters.LocalRescaleFunctionArguments(this);
            IFunctionRasterDataset inRs = createIdentityRaster(inRaster);
            args.InRaster = inRs;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        /// <summary>
        /// LocalStatistics
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or Raster</param>
        /// <returns>IRaster</returns>
        public IFunctionRasterDataset localStatisticsfunction(object inRaster, rasterUtil.localType op)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (op)
            {
                case localType.MAX:
                    rsFunc = new FunctionRasters.localMaxFunctionDataset();
                    break;
                case localType.MIN:
                    rsFunc = new FunctionRasters.localMinFunctionDataset();
                    break;
                case localType.SUM:
                    rsFunc = new FunctionRasters.localSumFunctionDataset();
                    break;
                case localType.MULTIPLY:
                    rsFunc = new FunctionRasters.localMultiplyFunctionDataset();
                    break;
                case localType.DIVIDE:
                    rsFunc = new FunctionRasters.localDividFunctionDataset();
                    break;
                case localType.SUBTRACT:
                    rsFunc = new FunctionRasters.localSubtractFunctionDataset();
                    break;
                case localType.POWER:
                    rsFunc = new FunctionRasters.localPowFunctionDataset();
                    break;
                case localType.MEAN:
                    rsFunc = new FunctionRasters.localMeanFunctionDataset();
                    break;
                case localType.VARIANCE:
                    rsFunc = new FunctionRasters.localVarianceFunctionDataset();
                    break;
                case localType.STD:
                    rsFunc = new FunctionRasters.localStandardDeviationFunctionDataset();
                    break;
                case localType.MODE:
                    rsFunc = new FunctionRasters.localModeFunctionDataset();
                    break;
                case localType.MEDIAN:
                    rsFunc = new FunctionRasters.localMedianFunctionDataset();
                    break;
                case localType.UNIQUE:
                    rsFunc = new FunctionRasters.localUniqueValuesFunctionDataset();
                    break;
                case localType.ENTROPY:
                    rsFunc = new FunctionRasters.localEntropyFunctionDataset();
                    break;
                case localType.MAXBAND:
                    rsFunc = new FunctionRasters.localMaxBandFunction();
                    break;
                case localType.MINBAND:
                    rsFunc = new FunctionRasters.localMinBandFunction();
                    break;
                case localType.ASM:
                    rsFunc = new FunctionRasters.localAsmFunctionDataset();
                    break;
                default:
                    break;
            }
            FunctionRasters.LocalFunctionArguments args = new FunctionRasters.LocalFunctionArguments(this);
            
            IFunctionRasterDataset inRs = createIdentityRaster(inRaster);
            args.InRaster = inRs;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        /// <summary>
        /// Clips a raster to the boundary of a polygon
        /// </summary>
        /// <param name="inRaster">IRaster, IRasterDataset, or string</param>
        /// <param name="geo">Polygon Geometry</param>
        /// <param name="clipType">the type of clip either inside or outside</param>
        /// <returns></returns>
        public IFunctionRasterDataset clipRasterFunction(object inRaster, IGeometry geo, esriRasterClippingType clipType)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            IRaster2 rRst2 = (IRaster2)createRaster(rRst);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ClipFunctionClass();
            IEnvelope env = geo.Envelope;
            double hX = rRst.RasterInfo.CellSize.X / 2;
            double hY = rRst.RasterInfo.CellSize.Y / 2;
            double xMin = env.XMin;
            double xMax = env.XMax;
            double yMin = env.YMin;
            double yMax = env.YMax;
            int clm, rw;
            rRst2.MapToPixel(xMin, yMin,out clm,out rw);
            rRst2.PixelToMap(clm, rw, out xMin, out yMin);
            xMin = xMin - hX;
            yMin = yMin - hY;
            rRst2.MapToPixel(xMax, yMax, out clm, out rw);
            rRst2.PixelToMap(clm, rw, out xMax, out yMax);
            xMax = xMax + hX;
            yMax = yMax + hY;
            env.PutCoords(xMin, yMin, xMax, yMax);
            IClipFunctionArguments args = new ClipFunctionArgumentsClass();
            args.Extent = env;
            args.ClippingGeometry = geo;
            args.ClippingType = clipType;
            args.Raster = rRst;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// Creates an in memory raster
        /// </summary>
        /// <param name="inRaster">template raster</param>
        /// <param name="outName">new name </param>
        /// <param name="wks">workspace</param>
        /// <returns>rasterdataset</returns>
        public IRasterDataset CreateMemoryRaster(IRaster inRaster, string outName, IWorkspace wks)
        {
            string txt = "MEM";
            esriWorkspaceType tp = wks.Type;
            if (outName.Length > 12)
            {
                outName.Substring(12);
            }
            ISaveAs sv = (ISaveAs)inRaster;
            IRasterDataset rsDset = (IRasterDataset)sv.SaveAs(outName, wks, txt);
            calcStatsAndHist(rsDset);
            return rsDset;
        }
        /// <summary>
        /// looks to see if raster exists
        /// </summary>
        /// <param name="wks"></param>
        /// <param name="inName"></param>
        /// <returns></returns>
        public bool rasterExists(IWorkspace wks, string inName)
        {

            return geoUtil.ftrExists(wks,inName);
        }
        /// <summary>
        /// checks to see if a name exists and if so returns a new string name with a prefix _
        /// </summary>
        /// <param name="wks"></param>
        /// <param name="inName"></param>
        /// <returns></returns>
        public string getSafeOutputName(IWorkspace wks, string inName)
        {
            string rstOut = inName;
            if (rstOut.Length > 12) rstOut.Substring(0, 12);
            foreach (string s in new string[] { " ","`", "~", "!", ".", ",", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "-" })
            {
                rstOut = rstOut.Replace(s, "_");
            }
            while (((System.IO.Directory.Exists(wks.PathName+"\\"+rstOut)==true)||(geoUtil.ftrExists(wks, rstOut)==true))==true)
            {
                if (rstOut == "____________") break;
                if (rstOut.Length > 11)
                {
                    rstOut =  "_" + rstOut.Substring(0, 10);
                }
                else
                {
                    rstOut =  "_"+rstOut;
                }
            }
            return rstOut;
        }
        /// <summary>
        /// calculates a and function
        /// </summary>
        /// <param name="rs1"></param>
        /// <param name="rs2"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcAndFunction(object rs1, object rs2)
        {
            IFunctionRasterDataset rs3 = calcGreaterEqualFunction(rs1, 1);
            IFunctionRasterDataset rs4 = calcGreaterEqualFunction(rs2, 1);
            IFunctionRasterDataset rs5 = calcArithmaticFunction(rs3, rs4, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset outRs = calcEqualFunction(rs5,2);
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// calculates a or function
        /// </summary>
        /// <param name="rs1"></param>
        /// <param name="rs2"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcOrFunction(object rs1, object rs2)
        {
            IFunctionRasterDataset rs3 = calcGreaterEqualFunction(rs1, 1);
            IFunctionRasterDataset rs4 = calcGreaterEqualFunction(rs2, 1);
            IFunctionRasterDataset rs5 = calcArithmaticFunction(rs3, rs4, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset outRs = calcGreaterEqualFunction(rs5, 1);
            IRasterInfo2 rsInfo2 = (IRasterInfo2)outRs.RasterInfo;
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.Mean = 0.5;
            rsStats.Maximum = 1;
            rsStats.Minimum = 0;
            rsStats.StandardDeviation = 0.25;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            rsStats.IsValid = true;
            for (int i = 0; i < rsInfo2.BandCount; i++)
            {
                rsInfo2.set_Statistics(i, rsStats);
            }
            return outRs;
        }
        /// <summary>
        /// creates a composite band function
        /// </summary>
        /// <param name="rsArray"></param>
        /// <returns></returns>
        public IFunctionRasterDataset compositeBandFunction(IRasterBandCollection rsBandCollection)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new CompositeBandFunctionClass();
            frDset.Init(rsFunc, rsBandCollection);
            return frDset;
            
        }
        public IFunctionRasterDataset compositeBandFunction(IRaster[] rsArray)
        {
            IRasterBandCollection rsBc = new RasterClass();
            for (int i = 0; i < rsArray.Length; i++)
            {
                rsBc.AppendBands((IRasterBandCollection)rsArray[i]);
            }
            return compositeBandFunction(rsBc);
        }
        /// <summary>
        /// calculates a slope function
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcSlopeFunction(IRaster inRaster)
        {
            IRaster rRst = returnRaster(inRaster,rstPixelType.PT_FLOAT);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new SlopeFunctionClass();
            ISlopeFunctionArguments args = new SlopeFunctionArgumentsClass();
            args.DEM = rRst;
            args.ZFactor = 1;
            frDset.Init(rsFunc, args);
            IRasterStatistics stats = new RasterStatisticsClass();
            stats.Mean = 50;
            stats.Minimum = 0;
            stats.Maximum = 90;
            stats.StandardDeviation = 25;
            stats.IsValid = true;
            for (int i = 0; i < frDset.RasterInfo.BandCount; i++)
            {
                ((IRasterInfo2)frDset.RasterInfo).set_Statistics(i, stats);
            }
            return frDset;
        }
        public IFunctionRasterDataset calcTasseledCap7Function(object inRaster) //assumes at sensor reflectance
        {
            List<float[]> slopes = new List<float[]>();
            slopes.Add(new float[] { 0f, 0.3561f, 0.3972f, 0.3904f, 0.6966f, 0.2286f, 0.1596f });//brightness
            slopes.Add(new float[] { 0f, -0.3344f, -0.3544f, -0.4556f, 0.6966f, -0.0242f, -0.2630f });//greenness
            slopes.Add(new float[] { 0f, 0.2626f, 0.2141f, 0.0926f, 0.0656f, -0.7629f, -0.5388f });//wetness
            return calcRegressFunction(inRaster, slopes);
        }
        public IFunctionRasterDataset calcNDVIFunction(object inRaster, int visibleBandId, int irBandId)
        {
            IFunctionRasterDataset visRs = getBand(inRaster, visibleBandId);
            IFunctionRasterDataset irRs = getBand(inRaster, irBandId);
            IFunctionRasterDataset sRs = calcArithmaticFunction(irRs, visRs, esriRasterArithmeticOperation.esriRasterMinus);
            IFunctionRasterDataset pRs = calcArithmaticFunction(irRs, visRs, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset fDset = calcArithmaticFunction(sRs, pRs, esriRasterArithmeticOperation.esriRasterDivide);
            IRasterStatistics rsStats = new RasterStatisticsClass();
            rsStats.IsValid = true;
            rsStats.Mean = 0;
            rsStats.Maximum = 1;
            rsStats.Minimum = -1;
            rsStats.StandardDeviation = 0.5;
            rsStats.SkipFactorX = 1;
            rsStats.SkipFactorY = 1;
            ((IRasterInfo2)fDset.RasterInfo).set_Statistics(0, rsStats);
            return fDset;
        }
        /// <summary>
        /// calculates an aspect function
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcAspectFunction(IRaster inRaster)
        {
            IFunctionRasterDataset rRst = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new AspectFunctionClass();
            frDset.Init(rsFunc, inRaster);
            return frDset;
        }
        
        /// <summary>
        /// converts an aspect raster to a nortsouth raster
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcNorthSouthFunction(IRaster DEM)
        {
            IFunctionRasterDataset rs = calcAspectFunction(DEM);
            IFunctionRasterDataset rs2 = calcMathRasterFunction(rs,transType.RADIANS);
            IFunctionRasterDataset outRs = calcMathRasterFunction(rs2, transType.COS);
            return outRs;
        }
        /// <summary>
        /// converts an aspect raster to a east west raster 
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcEastWestFunction(IRaster DEM)
        {
            IFunctionRasterDataset rs = calcAspectFunction(DEM);
            IFunctionRasterDataset rs2 = calcMathRasterFunction(rs, transType.RADIANS);
            IFunctionRasterDataset outRs = calcMathRasterFunction(rs2, transType.SIN);
            return outRs;
        }
        /// <summary>
        /// converts a raster to a polygon
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="outWorkSpace"></param>
        /// <param name="outName"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public IFeatureClass convertRasterToPolygon(IRaster rs, IWorkspace outWorkSpace, string outName, bool smooth)
        {
            ESRI.ArcGIS.GeoAnalyst.IConversionOp convOp = new ESRI.ArcGIS.GeoAnalyst.RasterConversionOpClass();
            IGeoDataset geoDset = convOp.RasterDataToPolygonFeatureData((IGeoDataset)rs, outWorkSpace, outName, smooth);
            return (IFeatureClass)geoDset;
        }
        /// <summary>
        /// converts a raster to a polyline
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="outWorkSpace"></param>
        /// <param name="outName"></param>
        /// <param name="zeroAsBackground"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public IFeatureClass convertRasterToPolyLine(IRaster rs, IWorkspace outWorkSpace, string outName, bool zeroAsBackground, bool smooth)
        {
            ESRI.ArcGIS.GeoAnalyst.IConversionOp convOp = new ESRI.ArcGIS.GeoAnalyst.RasterConversionOpClass();
            object minDangel = 0;
            IGeoDataset geoDset = convOp.RasterDataToLineFeatureData((IGeoDataset)rs, outWorkSpace, outName, zeroAsBackground, smooth, ref minDangel);
            return (IFeatureClass)geoDset;
        }
        /// <summary>
        /// converts a raster to a series of points
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="outWorkSpace"></param>
        /// <param name="outName"></param>
        /// <returns></returns>
        public IFeatureClass convertRasterToPoint(IRaster rs, IWorkspace outWorkSpace, string outName)
        {
            ESRI.ArcGIS.GeoAnalyst.IConversionOp convOp = new ESRI.ArcGIS.GeoAnalyst.RasterConversionOpClass();
            object minDangel = 0;
            IGeoDataset geoDset = convOp.RasterDataToPointFeatureData((IGeoDataset)rs, outWorkSpace, outName);
            return (IFeatureClass)geoDset;
        }
        public IRaster convertFeatureClassToRaster(IFeatureClass featureClass, rasterUtil.rasterType rasterType, IWorkspace outWorkSpace, string outName, double cellSize, IRasterDataset snapRaster)
        {
            return convertFeatureClassToRaster(featureClass, rasterType, outWorkSpace, outName, cellSize, snapRaster, null);
        }
        public IRaster convertFeatureClassToRaster(IFeatureClass featureClass, rasterUtil.rasterType rasterType, IWorkspace outWorkSpace, string outName, double cellSize, IRasterDataset snapRaster, IEnvelope extent)
        {
            ESRI.ArcGIS.GeoAnalyst.IConversionOp convOp = new ESRI.ArcGIS.GeoAnalyst.RasterConversionOpClass();
            ESRI.ArcGIS.GeoAnalyst.IRasterAnalysisEnvironment rasterAnalysisEnvironment = (ESRI.ArcGIS.GeoAnalyst.IRasterAnalysisEnvironment)convOp;
            rasterAnalysisEnvironment.OutSpatialReference = ((IGeoDataset)featureClass).SpatialReference;
            rasterAnalysisEnvironment.OutWorkspace = outWorkSpace;
            object cellS = cellSize;
            object ext = ((IGeoDataset)featureClass).Extent;
            object snap = Type.Missing;
            if(snapRaster!=null)
            {
                snap = snapRaster;
            }
            if (extent != null)
            {
                ext = extent;
            }
            rasterAnalysisEnvironment.SetCellSize(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, ref cellS);
            rasterAnalysisEnvironment.SetExtent(ESRI.ArcGIS.GeoAnalyst.esriRasterEnvSettingEnum.esriRasterEnvValue, ref ext,ref snap);
            string fmt = rasterType.ToString();
            if (fmt == "IMAGINE")
            {
                fmt = "IMAGINE image";
                if (!outName.ToLower().EndsWith(".img")) outName = outName + ".img";
            }
            IRasterDataset geoDset = convOp.ToRasterDataset((IGeoDataset)featureClass, fmt, outWorkSpace, outName);
            IGeoDatasetSchemaEdit2 geoSch = (IGeoDatasetSchemaEdit2)geoDset;
            if (geoSch.CanAlterSpatialReference) geoSch.AlterSpatialReference(rasterAnalysisEnvironment.OutSpatialReference);
            return returnRaster(geoDset);
        }
        /// <summary>
        /// Converts a raster to different pixel depth
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public IFunctionRasterDataset convertToDifFormatFunction(object inRaster, rstPixelType pType)
        {
            //IFunctionRasterDataset tDset = calcArithmaticFunction(inRaster, 0, esriRasterArithmeticOperation.esriRasterPlus);
            //IFunctionRasterDataset fDset = createIdentityRaster(inRaster);
            //fDset.RasterInfo.PixelType = pType;
            //return fDset;
            //string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            //IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            //IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            //frDsetName.FullName = tempAr;
            //frDset.FullName = (IName)frDsetName;
            //IRasterFunction rsFunc = new FunctionRasters.convertPixelTypeFunctionDataset();
            //FunctionRasters.convertPixelTypeFunctionArguments args = new FunctionRasters.convertPixelTypeFunctionArguments(this);
            //IFunctionRasterDataset IFdset = createIdentityRaster(inRaster);
            //args.InRaster = IFdset;
            //args.RasterPixelType = pType;
            //frDset.Init(rsFunc, args);
            ////frDset.RasterInfo.PixelType = pType;
            //return frDset;
            return createIdentityRaster(inRaster, pType);
        }
        public IFunctionRasterDataset setnullToValueFunction(object inRaster, double vl)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.nullToValueFunctionDataset();
            FunctionRasters.nullToValueFunctionArguments args = new FunctionRasters.nullToValueFunctionArguments(this);
            IFunctionRasterDataset IFdset = createIdentityRaster(inRaster);
            args.Raster = IFdset;
            args.Caching = true;
            args.RasterInfo = IFdset.RasterInfo;
            args.NewValue = vl;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        /// <summary>
        /// creates a mask of valid values (greater than equal to min and less than equal to max)
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="BandRanges"></param>
        /// <returns></returns>
        public IFunctionRasterDataset maskDataRange(object inRaster, double[][] BandRanges)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new MaskFunctionClass();
            IMaskFunctionArguments args = new MaskFunctionArgumentsClass();
            IDoubleArray dbArray = new DoubleArrayClass();
            foreach (double[] d in BandRanges)
            {
                dbArray.Add(d[0]);
                dbArray.Add(d[1]);
            }
            args.Raster = returnRaster(inRaster);
            args.IncludedRanges = dbArray;
            frDset.Init(rsFunc, args);
            return frDset;

        }
        public IFunctionRasterDataset setNullValue(object inRaster, int vl)
        {
            IFunctionRasterDataset rs = createIdentityRaster(inRaster);
            IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            IStringArray stArr = new StrArrayClass();
            for (int i = 0; i < rsBc.Count; i++)
            {
                stArr.Add(vl.ToString());
                stArr.Add(vl.ToString());
            }
            return setValueRangeToNodata(rs,stArr);
            //IRaster rs = returnRaster(inRaster);
            //IRasterProps rsProps = (IRasterProps)rs;
            //IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            //int[] nodataArray = new int[rsBc.Count];
            //for (int i = 0; i < rsBc.Count; i++)
            //{
            //    nodataArray[i] = vl;
            //}
            //rsProps.NoDataValue = nodataArray;
            //return rs;
        }
        public IFunctionRasterDataset setValueRangeToNodata(object inRaster,IStringArray sArray)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new MaskFunctionClass();
            IMaskFunctionArguments args = new MaskFunctionArgumentsClass(); //IMaskFunctionArguments2 args = new MaskFunctionArgumentsClass();
            args.Raster = returnRaster(inRaster);
            args.NoDataValues = sArray;
            //args.NoDataInterpretation = esriNoDataInterpretation.esriNoDataMatchAll;
            frDset.Init(rsFunc, args);
            return frDset;

            //IRaster rs = returnRaster(inRaster);

            //IRasterProps rsProps = (IRasterProps)rs;
            //IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            //int bCnt = rsBc.Count;
            //System.Array noDataArr = (System.Array)rsProps.NoDataValue;
            //IRasterBandCollection rsBcOut = new RasterClass();
            //for (int i = 0; i < bCnt; i++)
            //{
            //    IRaster brs = getBand(rs, i);
            //    double noData = System.Convert.ToDouble(noDataArr.GetValue(i));
            //    IRemapFilter rFilt = new RemapFilterClass();
            //    foreach (double[] d in minMaxList)
            //    {
            //        rFilt.AddClass(d[0], d[1], noData);
            //    }
            //    rsBcOut.AppendBands((IRasterBandCollection)calcRemapFunction(brs, rFilt));
            //}
            //return (IRaster)rsBcOut;
        }
        /// <summary>
        /// retrieves the appropriate no data value for a given rstPixeltype
        /// </summary>
        /// <param name="pType">type of pixel</param>
        /// <returns></returns>
        public static double getNoDataValue(rstPixelType pType)
        {
            double minVl = Double.MinValue;
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    minVl = -128;
                    break;
                case rstPixelType.PT_FLOAT:
                    minVl = 0.000000000000000000000000000000000000034;
                    break;
                case rstPixelType.PT_LONG:
                    minVl = -2147483648;
                    break;
                case rstPixelType.PT_SHORT:
                    minVl = -32768;
                    break;
                case rstPixelType.PT_U1:
                    minVl = 0;
                    break;
                case rstPixelType.PT_U2:
                    minVl = 0;
                    break;
                case rstPixelType.PT_U4:
                    minVl = 0;
                    break;
                case rstPixelType.PT_UCHAR:
                    minVl = 0;
                    break;
                case rstPixelType.PT_ULONG:
                    minVl = 0;
                    break;
                case rstPixelType.PT_USHORT:
                    minVl = 0;
                    break;
                default:
                    break;
            }
            return minVl;
        }
        /// <summary>
        /// builds a vat table for a raster
        /// </summary>
        /// <param name="inRaster"></param>
        public Dictionary<int,int> buildVat(object inRaster)
        {
            IFunctionRasterDataset fRs = null;
            if(inRaster is IFunctionRasterDataset)
            {
                fRs = (IFunctionRasterDataset)inRaster;
            }
            else
            {
                fRs = createIdentityRaster(inRaster);
            }
            IRasterInfo2 rsInfo2 = (IRasterInfo2)fRs.RasterInfo;
            ITable vatTbl = rsInfo2.AttributeTable;
            //Console.WriteLine("records = " + vatTbl.RowCount(null));
            //for (int i = 0; i < vatTbl.Fields.FieldCount; i++)
            //{
                //IField fld = vatTbl.Fields.get_Field(i);
                //Console.WriteLine(fld.Name);
                //Console.WriteLine(fld.Type.ToString());
            // }
            return BuildVatFromScratch(fRs);
        }
        public ITable convertDicToVat(Dictionary<int, int> vlDic,IWorkspace wks, string tblName)
        {
            IField fld1 = new FieldClass();
            IFieldEdit fld1E = (IFieldEdit)fld1;
            fld1E.Name_2 = "Value";
            fld1E.Type_2 = esriFieldType.esriFieldTypeInteger;
            IField fld2 = new FieldClass();
            IFieldEdit fld2E = (IFieldEdit)fld2;
            fld2E.Name_2 = "Count";
            fld2E.Type_2 = esriFieldType.esriFieldTypeInteger;
            IFields flds = new FieldsClass();
            IFieldsEdit fldsE = (IFieldsEdit)flds;
            fldsE.AddField(fld1);
            fldsE.AddField(fld2);
            ITable outTbl = geoUtil.createTable(wks, tblName, flds);
            int vIndex = outTbl.FindField("Value");
            int cntIndex = outTbl.FindField("Count");
            ICursor inSertCur = outTbl.Insert(true);
            IRowBuffer nR = outTbl.CreateRowBuffer();
            foreach (KeyValuePair<int,int> item in vlDic)
            {
                nR.set_Value(vIndex, item.Key);
                nR.set_Value(cntIndex, item.Value);
                inSertCur.InsertRow(nR);
            }
            inSertCur.Flush();
            return outTbl;
        }
        public void appendVatToRasterDataset(ITable inTable, IRasterDataset rsDset)
        {
            IRasterDatasetEdit3 rsDsetE3 = (IRasterDatasetEdit3)rsDset;
            rsDsetE3.AlterAttributeTable(inTable);
        }
        private Dictionary<int,int> BuildVatFromScratch(IFunctionRasterDataset fRs)
        {

            Dictionary<int, int> dicVl = new Dictionary<int, int>();
            IRasterFunctionHelper rsHelp = new RasterFunctionHelperClass();
            rsHelp.Bind(fRs);
            IRasterCursor rsCur = rsHelp.Raster.CreateCursor();
            do
            {
                IPixelBlock pb = rsCur.PixelBlock;
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object objVl = pb.GetVal(0, c, r);
                        if (objVl != null)
                        {
                            int vl = System.Convert.ToInt32(objVl);
                            int cnt = 0;
                            if (dicVl.TryGetValue(vl, out cnt))
                            {
                                dicVl[vl] = cnt + 1;
                            }
                            else
                            {
                                dicVl.Add(vl, 1);
                            }
                        }
                    }
                }
            } while (rsCur.Next());
            return dicVl;
        }
        /// <summary>
        /// defines unique regions using a 4 neighbor window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="wks"></param>
        /// <param name="outName"></param>
        /// <returns></returns>
        public IFunctionRasterDataset regionGroup(object inRaster)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new FunctionRasters.regionGroupFunctionDataset();
            FunctionRasters.regionGroupFunctionArguments args = new FunctionRasters.regionGroupFunctionArguments(this);
            args.InRaster = iR1;
            frDset.Init(rsFunc, args);
            return frDset;
        }
        
        /// <summary>
        /// performs block summarization
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="outWks"></param>
        /// <param name="outRsName"></param>
        /// <param name="numCells"></param>
        /// <returns></returns>
        public IFunctionRasterDataset calcAggregationFunction(object inRaster, int cells, focalType statType)
        {
            IFunctionRasterDataset iR1 = createIdentityRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (statType)
            {
                case focalType.MIN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperMin();
                    break;
                case focalType.SUM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperSum();
                    break;
                case focalType.MEAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperMean();
                    break;
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperMedian();
                    break;
                case focalType.VARIANCE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperVar();
                    break;
                case focalType.STD:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperStd();
                    break;
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperEntropy();
                    break;
                case focalType.ASM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperASM();
                    break;
                default:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.aggregationHelperMax();
                    break;
            }
            FunctionRasters.aggregationFunctionArguments args = new FunctionRasters.aggregationFunctionArguments(this);
            args.Cells = cells;
            args.InRaster = iR1;
            frDset.Init(rsFunc, args);
            return frDset;
           
        }
       
        /// <summary>
        /// retrieves a safe value for a given a raster pixel type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static object getSafeValue(object outVl, rstPixelType pType)
        {
            object newVl = 0;
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    try
                    {
                        newVl = System.Convert.ToSByte(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>sbyte.MaxValue)newVl=sbyte.MaxValue;
                        else newVl=sbyte.MinValue;
                    }
                    break;
                case rstPixelType.PT_LONG:
                    try
                    {
                        newVl = System.Convert.ToInt32(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>long.MaxValue)newVl=long.MaxValue;
                        else newVl=long.MinValue;
                    }
                    break;
                case rstPixelType.PT_SHORT:
                    try
                    {
                        newVl = System.Convert.ToInt16(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>short.MaxValue)newVl=short.MaxValue;
                        else newVl=short.MinValue;
                    }
                    break;
                case rstPixelType.PT_U1:
                    try
                    {
                        newVl = System.Convert.ToBoolean(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>1)newVl=1;
                        else newVl=0;
                    }
                    break;
                case rstPixelType.PT_U2:
 
                case rstPixelType.PT_U4:
                    
                case rstPixelType.PT_UCHAR:
                    try
                    {
                        newVl = System.Convert.ToByte(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>byte.MaxValue)newVl=byte.MaxValue;
                        else newVl=byte.MinValue;
                    }
                    break;
                case rstPixelType.PT_ULONG:
                    try
                    {
                        newVl = System.Convert.ToUInt32(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>ulong.MaxValue)newVl=ulong.MaxValue;
                        else newVl=ulong.MinValue;
                    }
                    break;
                case rstPixelType.PT_USHORT:
                    try
                    {
                        newVl = System.Convert.ToUInt16(outVl);
                    }
                    catch
                    {
                        double vl = System.Convert.ToDouble(outVl);
                        if(vl>ushort.MaxValue)newVl=ushort.MaxValue;
                        else newVl=ushort.MinValue;
                    }
                    break;
                case rstPixelType.PT_FLOAT:
                    newVl = System.Convert.ToSingle(outVl);
                    break;
                default:
                    newVl = outVl;
                    break;
            }
            return newVl;
        }
        public IRaster mosaicRastersFunction(IWorkspace wks, string mosaicName, IRaster[] rasters)
        {
            return mosaicRastersFunction(wks, mosaicName, rasters,esriMosaicMethod.esriMosaicNone,rstMosaicOperatorType.MT_FIRST,true, true, true, true);

        }
        public IRaster mosaicRastersFunction(IWorkspace wks, string mosaicName, string[] rasterNames, IEnvelope combinedEnvelope, esriMosaicMethod mosaicmethod, rstMosaicOperatorType mosaictype, bool buildfootprint, bool buildboudary, bool seamlines, bool buildOverview)
        {
            IFunctionRasterDataset rs1 = createIdentityRaster(rasterNames[0]);
            IEnvelope env = combinedEnvelope;
            int ht = System.Convert.ToInt32(env.Height);
            int wd = System.Convert.ToInt32(env.Width);
            int rec = System.Convert.ToInt32(ht * wd);
            ISpatialReference sr = rs1.RasterInfo.SpatialReference;
            string mNm = getSafeOutputName(wks, mosaicName);
            IMosaicDataset msDset = createMosaicDataset(wks, sr, mNm, rs1.RasterInfo.PixelType, rs1.RasterInfo.BandCount);
            msDset.MosaicFunction.MaxMosaicImageCount = rasterNames.Length;
            msDset.MosaicFunction.MosaicMethod = mosaicmethod;
            msDset.MosaicFunction.MosaicOperatorType = mosaictype;
            IFunctionRasterDataset fDset = (IFunctionRasterDataset)msDset;
            IPropertySet pSet = (fDset).Properties;
            pSet.SetProperty("MaxImageHeight", ht);
            pSet.SetProperty("MaxImageWidth", wd);
            pSet.SetProperty("MaxRecordCount", rec);
            pSet.SetProperty("DefaultResamplingMethod", 0);
            pSet.SetProperty("MaxMosaicImageCount", rasterNames.Length);
            pSet.SetProperty("MaxDownloadImageCount", rasterNames.Length);
            pSet.SetProperty("IsPreprocessedData", "True");
            pSet.SetProperty("MosaicOperator", 4);
            pSet.SetProperty("MosaicMethod", 0);
            fDset.Properties = pSet;
            IMosaicDatasetOperation msDsetOp = (IMosaicDatasetOperation)msDset;
            addRastersToMosaicDataset(msDset, rasterNames);
            ICalculateCellSizeRangesParameters computeArgs = new CalculateCellSizeRangesParametersClass();
            msDsetOp.CalculateCellSizeRanges(computeArgs, null);
            if (buildfootprint)
            {
                IBuildFootprintsParameters fpArgs = new BuildFootprintsParametersClass();
                fpArgs.Method = esriBuildFootprintsMethods.esriBuildFootprintsByGeometry;
                msDsetOp.BuildFootprints(fpArgs, null);
            }
            if (buildboudary)
            {
                IBuildBoundaryParameters bndArgs = new BuildBoundaryParametersClass();
                bndArgs.AppendToExistingBoundary = true;
                msDsetOp.BuildBoundary(bndArgs, null);
            }
            if (seamlines)
            {
                IBuildSeamlinesParameters smArgs = new BuildSeamlinesParametersClass();
                smArgs.ModifySeamlines = true;
                msDsetOp.BuildSeamlines(smArgs, null);
            }
            if (buildOverview)
            {
                IDefineOverviewsParameters ofPar = new DefineOverviewsParametersClass();
                ofPar.ForceOverviewTiles = true;
                ((IOverviewTileParameters)ofPar).OverviewFactor = 3;
                msDsetOp.DefineOverviews(ofPar, null);
                IGenerateOverviewsParameters ovArgs = new GenerateOverviewsParametersClass();
                ovArgs.GenerateMissingImages = true;
                ovArgs.GenerateStaleImages = true;
                msDsetOp.GenerateOverviews(ovArgs, null);
            }
            fDset.Init((IRasterFunction)msDset.MosaicFunction, msDset.MosaicFunctionArguments);
            IRaster rs = createRaster((IRasterDataset)fDset);
            return rs;
        } 
        public IRaster mosaicRastersFunction(IWorkspace wks, string mosaicName, IRaster[] rasters, esriMosaicMethod mosaicmethod, rstMosaicOperatorType mosaictype, bool buildfootprint, bool buildboudary, bool seamlines, bool buildOverview)
        {
            IRaster rs1 = rasters[0];
            IEnvelope env = getCombinedExtents(rasters);
            int ht = System.Convert.ToInt32(env.Height);
            int wd = System.Convert.ToInt32(env.Width);
            int rec = System.Convert.ToInt32(ht * wd);
            IRasterProps rs1Props = (IRasterProps)rs1;
            IRasterBandCollection rsBc = (IRasterBandCollection)rs1;
            IGeoDataset rs1_2 = (IGeoDataset)rs1;
            ISpatialReference sr = rs1_2.SpatialReference;
            string mNm = getSafeOutputName(wks,mosaicName);
            IMosaicDataset msDset = createMosaicDataset(wks, sr, mNm, rs1Props.PixelType, rsBc.Count);
            msDset.MosaicFunction.MaxMosaicImageCount = rasters.Length;
            msDset.MosaicFunction.MosaicMethod = mosaicmethod;
            msDset.MosaicFunction.MosaicOperatorType = mosaictype;
            IFunctionRasterDataset fDset = (IFunctionRasterDataset)msDset;
            IPropertySet pSet = (fDset).Properties;
            pSet.SetProperty("MaxImageHeight", ht);
            pSet.SetProperty("MaxImageWidth", wd);
            pSet.SetProperty("MaxRecordCount", rec);
            pSet.SetProperty("DefaultResamplingMethod", 0);
            pSet.SetProperty("MaxMosaicImageCount", rasters.Length);
            pSet.SetProperty("MaxDownloadImageCount", rasters.Length);
            pSet.SetProperty("IsPreprocessedData", "True");
            pSet.SetProperty("MosaicOperator", 4);
            pSet.SetProperty("MosaicMethod", 0);
            fDset.Properties = pSet;
            IMosaicDatasetOperation msDsetOp = (IMosaicDatasetOperation)msDset;
            addRastersToMosaicDataset(msDset, rasters);
            ICalculateCellSizeRangesParameters computeArgs = new CalculateCellSizeRangesParametersClass();
            msDsetOp.CalculateCellSizeRanges(computeArgs, null);
            if (buildfootprint)
            {
                IBuildFootprintsParameters fpArgs = new BuildFootprintsParametersClass();
                fpArgs.Method = esriBuildFootprintsMethods.esriBuildFootprintsByGeometry;
                msDsetOp.BuildFootprints(fpArgs, null);
            }
            if(buildboudary)
            {
                IBuildBoundaryParameters bndArgs = new BuildBoundaryParametersClass();
                bndArgs.AppendToExistingBoundary=true;
                msDsetOp.BuildBoundary(bndArgs,null);
            }
            if(seamlines)
            {
                IBuildSeamlinesParameters smArgs = new BuildSeamlinesParametersClass();
                smArgs.ModifySeamlines = true;
                msDsetOp.BuildSeamlines(smArgs, null);
            }
            if (buildOverview)
            {
                IDefineOverviewsParameters ofPar = new DefineOverviewsParametersClass();
                ofPar.ForceOverviewTiles = true;
                ((IOverviewTileParameters)ofPar).OverviewFactor = 3;
                msDsetOp.DefineOverviews(ofPar, null);
                IGenerateOverviewsParameters ovArgs = new GenerateOverviewsParametersClass();
                ovArgs.GenerateMissingImages = true;
                ovArgs.GenerateStaleImages = true;
                msDsetOp.GenerateOverviews(ovArgs, null);
            }
            fDset.Init((IRasterFunction)msDset.MosaicFunction, msDset.MosaicFunctionArguments);
            IRaster rs = createRaster((IRasterDataset)fDset);
            return rs;
        }

        private IEnvelope getCombinedExtents(IRaster[] rasters)
        {
            ISpatialReference sr1 = null;
            IEnvelope env = null;
            foreach (IRaster rs in rasters)
            {
                IRasterProps rsP = (IRasterProps)rs;
                IEnvelope ext = rsP.Extent;
                ISpatialReference sr2 = ext.SpatialReference;
                //Console.WriteLine(ext.SpatialReference.Name);
                if (env == null)
                {
                    env = ext;
                    sr1 = ext.SpatialReference;
                }
                else
                {
                    if (sr1.Name != sr2.Name)
                    {
                        ext.SpatialReference = env.SpatialReference;
                    }
                    env.Union(ext);
                   
                }
            }
            return env;
        }
       
        public void addRastersToMosaicDataset(IMosaicDataset mosaicDataSet, IRaster[] rasters)
        {
            IMosaicDatasetOperation mOp = (IMosaicDatasetOperation)mosaicDataSet;
            foreach(IRaster rs in rasters)
            {
                IAddRastersParameters addRs = new AddRastersParametersClass();
                IRasterDatasetCrawler rsDsetCrawl = new RasterDatasetCrawlerClass();
               
                rsDsetCrawl.RasterDataset = ((IRaster2)rs).RasterDataset;
                IRasterTypeFactory rsFact = new RasterTypeFactoryClass();
                IRasterType rsType = rsFact.CreateRasterType("Raster dataset");
                rsType.FullName = rsDsetCrawl.DatasetName;
                addRs.Crawler = (IDataSourceCrawler)rsDsetCrawl;
                addRs.RasterType = rsType;
                mOp.AddRasters(addRs, null);
            }
            return;

        }
        public void addRastersToMosaicDataset(IMosaicDataset mosaicDataSet, string[] rasterNames)
        {
            IMosaicDatasetOperation mOp = (IMosaicDatasetOperation)mosaicDataSet;
            foreach (string rsName in rasterNames)
            {
                IAddRastersParameters addRs = new AddRastersParametersClass();
                IRasterDatasetCrawler rsDsetCrawl = new RasterDatasetCrawlerClass();
                string bnd;
                IRasterDataset rDset = openRasterDataset(rsName, out bnd);
                IName rDsetName = ((IDataset)rDset).FullName;
                rsDsetCrawl.DatasetName = rDsetName;
                IRasterTypeFactory rsFact = new RasterTypeFactoryClass();
                IRasterType rsType = rsFact.CreateRasterType("Raster dataset");
                rsType.FullName = rsDsetCrawl.DatasetName;
                addRs.Crawler = (IDataSourceCrawler)rsDsetCrawl;
                addRs.RasterType = rsType;
                mOp.AddRasters(addRs, null);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rDset);
            }
            return;

        }
        public IMosaicDataset createMosaicDataset(IWorkspace wks, ISpatialReference spatialReference, string mosaicDataSetName, rstPixelType pType, int numBands)
        {
            ICreateMosaicDatasetParameters crParam = new CreateMosaicDatasetParametersClass();
            crParam.BandCount = numBands;
            crParam.PixelType = pType;
            IMosaicWorkspaceExtensionHelper mosaicExtHelper = new MosaicWorkspaceExtensionHelperClass();
            IMosaicWorkspaceExtension mosaicExt = mosaicExtHelper.FindExtension(wks);
            IMosaicDataset mosaicDset = mosaicExt.CreateMosaicDataset(mosaicDataSetName, spatialReference,(ICreateMosaicDatasetParameters)crParam,"");
            return mosaicDset;
        }
        public IMosaicDataset openMosaicDataset(IWorkspace wks, string mosaicDatasetName)
        {
            IMosaicWorkspaceExtensionHelper mosaicExtHelper = new MosaicWorkspaceExtensionHelperClass();
            IMosaicWorkspaceExtension mosaicExt = mosaicExtHelper.FindExtension(wks);
            IMosaicDataset mosaicDset = mosaicExt.OpenMosaicDataset(mosaicDatasetName);
            return mosaicDset;
        }

        public IRaster mergeRasterFunction(IRaster[] inRasters,rstMosaicOperatorType mergeMethod,string rstNm)
        {
            string dbStr = mosaicDir + "\\rsCatDb.gdb";
            IWorkspace wks = null;
            if (!System.IO.Directory.Exists(dbStr))
            {
                wks = geoUtil.CreateWorkSpace(mosaicDir, "rsCatDb.gdb");
            }
            else
            {
                wks = geoUtil.OpenRasterWorkspace(dbStr);
            }
            rstNm = getSafeOutputName(wks,rstNm);
            IRaster rs = mosaicRastersFunction(wks, rstNm, inRasters,esriMosaicMethod.esriMosaicNone,mergeMethod,false,false,false,false);
            return rs;
        }
        
        public ITable zonalStats(IFeatureClass inFeatureClass, string fieldName, object inValueRaster, string outTableName, zoneType[] zoneTypes,esriUtil.Forms.RunningProcess.frmRunningProcessDialog rd,bool classCounts=false)
        {
            FunctionRasters.zonalHelper zH = new FunctionRasters.zonalHelper(this,rd);
            zH.InValueRaster = createIdentityRaster(inValueRaster);
            //zH.convertFeatureToRaster(inFeatureClass, fieldName);
            zH.InZoneFeatureClass = inFeatureClass;
            zH.InZoneField = fieldName;
            zH.ZoneTypes = zoneTypes;
            zH.OutTableName = outTableName;
            zH.ZoneClassCount = classCounts;
            zH.setZoneValues();
            return zH.OutTable;
        }
        public ITable zonalStats(object inZoneRaster, object inValueRaster, string outTableName, zoneType[] zoneTypes, esriUtil.Forms.RunningProcess.frmRunningProcessDialog rd,bool classCounts=false)
        {
            FunctionRasters.zonalHelper zH = new FunctionRasters.zonalHelper(this,rd);
            zH.InValueRaster = createIdentityRaster(inValueRaster);
            zH.InZoneRaster =createIdentityRaster(inZoneRaster);
            zH.ZoneTypes = zoneTypes;
            zH.OutTableName = outTableName;
            zH.ZoneClassCount = classCounts;
            zH.setZoneValues();
            return zH.OutTable;
        }
        public IGeometry extractDomain(IRaster rs, bool pCenterbased=false)
        {
            IRaster rs1 = rs;
            if (((IRasterBandCollection)rs1).Count > 1)
            {
                rs1 = createRaster(getBand(rs, 0));
            }
            IRasterDomainExtractor dEx = new RasterDomainExtractorClass();
            IPolygon poly = dEx.ExtractDomain(rs1,pCenterbased);
            return (IGeometry)poly;
        }

        public static void cleanupTempDirectories()
        {
            string mainPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string func = mainPath + "\\RmrsRasterUtilityHelp\\func";
            string mos = mainPath + "\\RmrsRasterUtilityHelp\\mosaic";
            string conv = mainPath + "\\RmrsRasterUtilityHelp\\conv";
            string[] dirs = {func,mos,conv};
            foreach (string s in dirs)
            {
                try
                {
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(s);
                    if(dInfo.Exists) dInfo.Delete(true);
                }
                catch
                {
                }
            }
        }
        public void removeLock(IDataset rDset)
        {
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rDset.Workspace);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rDset);
        }
        
        public static bool isNullData(object inValue, object noDataValue)
        {
            try
            {
                double inVl = System.Convert.ToDouble(inValue);
                double ndVl = System.Convert.ToDouble(noDataValue);
                if (inVl.Equals(ndVl) || Double.IsNaN(inVl) || Double.IsInfinity(inVl))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Console.WriteLine("failed isNullData " + inValue.ToString());
                return true;
            }
        }
        
        public IFunctionRasterDataset focalBandfunction(object inRaster, localType op, int bandsBefore, int bandsAfter)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (op)
            {
                case localType.MAX:
                    break;
                case localType.MIN:
                    break;
                case localType.MAXBAND:
                    break;
                case localType.MINBAND:
                    break;
                case localType.SUM:
                    rsFunc = new FunctionRasters.focalBandFunctionDatasetSum();
                    break;
                case localType.MULTIPLY:
                    break;
                case localType.DIVIDE:
                    break;
                case localType.SUBTRACT:
                    break;
                case localType.POWER:
                    break;
                case localType.MEAN:
                    rsFunc = new FunctionRasters.focalBandFunctionDatasetMean();
                    break;
                case localType.VARIANCE:
                    break;
                case localType.STD:
                    rsFunc = new FunctionRasters.focalBandFunctionDatasetStd();
                    break;
                case localType.MODE:
                    break;
                case localType.MEDIAN:
                    break;
                case localType.UNIQUE:
                    break;
                case localType.ENTROPY:
                    break;
                case localType.ASM:
                    break;
                default:
                    break;
            }
            FunctionRasters.focalBandFunctionArguments args = new FunctionRasters.focalBandFunctionArguments(this);
            IFunctionRasterDataset inRs = createIdentityRaster(inRaster);
            args.InRaster = inRs;
            args.BandsBefore = bandsBefore;
            args.BandsAfter = bandsAfter;
            frDset.Init(rsFunc, args);
            return frDset;
        }

    }
}
