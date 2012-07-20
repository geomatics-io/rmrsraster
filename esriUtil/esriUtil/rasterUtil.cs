using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace esriUtil
{
    public class rasterUtil
    {
        public rasterUtil()
        {
            string globFuncDir = esriUtil.Properties.Settings.Default.FuncDir;
            string globMosaicDir = esriUtil.Properties.Settings.Default.MosaicDir;
            if (globFuncDir == "unknown")
            {
                globFuncDir = System.Environment.GetEnvironmentVariable("temp") + "\\func";
                esriUtil.Properties.Settings.Default.FuncDir = globFuncDir;
                esriUtil.Properties.Settings.Default.Save();
            }
            if (globMosaicDir == "unknown")
            {
                globMosaicDir = System.Environment.GetEnvironmentVariable("temp") + "\\mosaic";
                esriUtil.Properties.Settings.Default.MosaicDir = globMosaicDir;
                esriUtil.Properties.Settings.Default.Save();
            }
            System.IO.DirectoryInfo DInfo = new System.IO.DirectoryInfo(globFuncDir);
            if (!DInfo.Exists)
            {
                DInfo.Create();
            }
            if (!System.IO.Directory.Exists(globMosaicDir)) System.IO.Directory.CreateDirectory(globMosaicDir);
            mosaicDir = globMosaicDir + "\\" + newGuid;
            funcDir = globFuncDir + "\\" + newGuid;
            fp = newGuid.Substring(1, 3);
            System.IO.Directory.CreateDirectory(funcDir);
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
        private string funcDir = "";
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
        public enum glcmMetric { CONTRAST, DISSIMILARITY, HOMOGENEITY, ASM, ENERGY, MAXPROBABILITY, MINPROBABILITY, RANGE, ENTROPY, MEAN, VARIANCE, CORRELATION, COVARIANCE }
        /// <summary>
        /// ouput raster types
        /// </summary>
        public enum rasterType { GRID, TIFF, IMAGINE, JP2, GDB, JPG, PNG, BMP, GIF,PIX, XPM, MAP, MEM, HDF4, BIL, BIP, BSQ, RST, ENV }
        /// <summary>
        /// sampling cluster types
        /// </summary>
        public enum clusterType {SUM,MEAN,MEDIAN,MODE};
        /// <summary>
        /// focal window functions types
        /// </summary>
        public enum focalType { MAX, MIN, SUM, MEAN, MODE, MEDIAN, VARIANCE, STANDARD_DEVIATION, UNIQUE, ENTROPY, PROBABILITY }
        /// <summary>
        /// local type of functions
        /// </summary>
        public enum localType { MAX, MIN, MAXBAND, MINBAND, SUM, MULTIPLY, DIVIDE, SUBTRACT, POWER, MEAN, VARIANCE, STANDARD_DEVIATION, MODE, MEDIAN, UNIQUE, ENTROPY }
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
        public enum transType { LOG10, LN, EXP, EXP10, ABS, SIN, COS, TAN, ASIN, ACOS, ATAN, RADIANS }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        /// <summary>
        /// Creates an in Memory Raster given a raster dataset
        /// </summary>
        /// <param name="rsDset">IRasterDataset</param>
        /// <returns>IRaster</returns>
        public IRaster createRaster(IRasterDataset rsDset)
        {
            string cNm = rsDset.Format.ToLower();
            if (cNm.EndsWith("hdf4") || cNm.EndsWith("ntif"))
            {
                IRasterBandCollection rsBc = new RasterClass();
                IRasterDatasetJukebox rsDsetJu = (IRasterDatasetJukebox)rsDset;
                int subCnt = rsDsetJu.SubdatasetCount;
                for (int i = 0; i < subCnt; i++)
                {
                    rsDsetJu.Subdataset = i;
                    IRasterDataset subDset = (IRasterDataset)rsDsetJu;
                    rsBc.AppendBand(((IRasterBandCollection)subDset).Item(0));
                }
                return (IRaster)rsBc;

            }
            else
            {
                IRasterDataset3 rDset3 = (IRasterDataset3)rsDset;
                return rDset3.CreateFullRaster();
            }
        }
        /// <summary>
        /// Opens a raster dataset given a string path
        /// </summary>
        /// <param name="rasterPath">full path to a raster dataset</param>
        /// <returns>IRasterDataset</returns>
        public IRasterDataset openRasterDataset(string rasterPath,out string bnd)
        {
            IWorkspace wks = openRasterDatasetRec(rasterPath);
            string rstDir = wks.PathName;
            string rstName = rasterPath.Replace(rstDir, "").TrimStart(new char[]{'\\'});
            string[] rstNameSplit = rstName.Split(new char[] { '\\' });
            string dataSet = "";
            string rsDset = "";
            bnd = "all";
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
                    string[] bndsp = bnd.Split(new char[]{'_'});
                    bnd = bndsp[bndsp.Length-1];
                    break;
            }
            IRasterDataset rstDset = null;
            if (wks.Type == esriWorkspaceType.esriLocalDatabaseWorkspace || wks.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                IRasterWorkspaceEx rsWks = (IRasterWorkspaceEx)wks;
                rstDset = rsWks.OpenRasterDataset(rsDset);
            }
            else
            {
                IRasterWorkspace rsWks = (IRasterWorkspace)wks;
                rstDset = rsWks.OpenRasterDataset(rsDset);
            }
            return rstDset;

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
            ICursor scur = rst2.AttributeTable.Search(null, false);
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
        public IFeatureClass createRandomSampleLocationsByClass(IWorkspace wks, string rasterPath, int sampleSizePerClass, int numImages, string outName)
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
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                if (((IWorkspace2)wks).get_NameExists(esriDatasetType.esriDTFeatureClass, pointName))
                {

                    IDataset dSet = (IDataset)ftrWks.OpenFeatureClass(pointName);
                    if (dSet.CanDelete())
                    {
                        dSet.Delete();
                    }
                    else
                    {
                        pointName = pointName + "_";
                    }
                }
                IRasterProps rstProps = (IRasterProps)rst2;
                double nullValue = System.Convert.ToDouble(((System.Array)rstProps.NoDataValue).GetValue(0));
                int rWidth = rstProps.Width;
                int rHeight = rstProps.Height;
                int spc = sampleSizePerClass / numImages;
                if (spc < 1)
                {
                    spc = 1;
                }
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "Value";
                fldE.Type_2 = esriFieldType.esriFieldTypeString;
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
                    object vlT = rst2.GetPixelValue(0, x, y);
                    string vlO = null;
                    if (vlT == null)
                    {
                        continue;
                    }
                    else
                    {
                        vlO = vlT.ToString();
                        
                    }
                    if (vlO == null || vlO == Double.NaN.ToString() || vlO == nullValue.ToString()) continue;
                    string vl = vlO;
                    double xC = rst2.ToMapX(x);
                    double yC = rst2.ToMapY(y);
                    string tStr = x.ToString() + ";" + y.ToString();
                    double[] xy = { xC, yC };
                    if (xyList.TryGetValue(vl, out tCoor))
                    {
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
                int classIndex = sampFtrCls.FindField("Value");
                int weightIndex = sampFtrCls.FindField("WEIGHT");
                double clAv = classCnts.Values.Average();
                IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
                bool weStartEdit = true;
                if (wksE.IsBeingEdited())
                {
                    weStartEdit = false;
                }
                else
                {
                    wksE.StartEditing(false);
                }
                wksE.StartEditOperation();
                foreach (KeyValuePair<string, List<double[]>> kVp in xyList)
                {
                    string ky = kVp.Key;
                    List<double[]> vl = kVp.Value;
                    foreach (double[] d in vl)
                    {
                        IFeature ftr = sampFtrCls.CreateFeature();
                        IGeometry geo = ftr.Shape;
                        IPoint pnt = (IPoint)geo;
                        pnt.PutCoords(d[0], d[1]);
                        ftr.set_Value(classIndex, ky);
                        ftr.set_Value(weightIndex,(classCnts[ky] / clAv));
                        ftr.Store();
                    }

                }
                wksE.StopEditOperation();
                if (weStartEdit)
                {
                    wksE.StopEditing(true);
                }
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
        public IFeatureClass createRandomSampleLocations(IWorkspace wks, string rasterPath, int TotalSamples, string outName)
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
                Random rndGen = new Random();
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                if (((IWorkspace2)wks).get_NameExists(esriDatasetType.esriDTFeatureClass, pointName))
                {
                    IDataset dSet = (IDataset)ftrWks.OpenFeatureClass(pointName);
                    if (dSet.CanDelete())
                    {
                        dSet.Delete();
                    }
                    else
                    {
                        pointName = pointName + "_";
                    }
                }
                IRasterProps rstProps = (IRasterProps)rst2;
                System.Array sArr = (System.Array)rstProps.NoDataValue;
                double nullValue = System.Convert.ToDouble(sArr.GetValue(0));
                int rWidth = rstProps.Width;
                int rHeight = rstProps.Height;
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "VALUE";
                fldE.Type_2 = esriFieldType.esriFieldTypeString;
                fldsE.AddField(fld);
                sampFtrCls = geoUtil.createFeatureClass((IWorkspace2)wks, pointName, flds, esriGeometryType.esriGeometryPoint, rstProps.SpatialReference);
                IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
                bool weStartEdit = true;
                if (wksE.IsBeingEdited())
                {
                    weStartEdit = false;
                }
                else
                {
                    wksE.StartEditing(false);
                }
                wksE.StartEditOperation();
                int checkSampleSize = 0;
                int classIndex = sampFtrCls.FindField("Value");
                while (checkSampleSize<TotalSamples)
                {
                    int x = rndGen.Next(rWidth);
                    int y = rndGen.Next(rHeight);
                    object vlT = rst2.GetPixelValue(0, x, y);
                    string vlO = null;
                    if (vlT == null)
                    {
                        continue;
                    }
                    else
                    {
                        vlO = vlT.ToString();

                    }
                    if (vlO == null||vlO == Double.NaN.ToString() || vlO == nullValue.ToString())
                    {
                        continue;
                    }
                    else
                    {
                        double xC = rst2.ToMapX(x);
                        double yC = rst2.ToMapY(y);
                        IFeature ftr = sampFtrCls.CreateFeature();
                        IGeometry geo = ftr.Shape;
                        IPoint pnt = (IPoint)geo;
                        pnt.PutCoords(xC, yC);
                        try
                        {
                            ftr.set_Value(classIndex, vlO);
                            ftr.Store();
                            checkSampleSize++;
                        }
                        catch (Exception e)
                        {
                            //System.Windows.Forms.MessageBox.Show("Error:" + e.ToString());
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                wksE.StopEditOperation();
                if (weStartEdit)
                {
                    wksE.StopEditing(true);
                }
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
        /// <param name="inRaster"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public IRaster getBand(object inRaster, int index)
        {
            IRaster rst = returnRaster(inRaster);
            if (rst != null)
            {
                IRasterBandCollection rsBc = new RasterClass();
                IRasterBandCollection rsBc2 = (IRasterBandCollection)rst;
                rsBc.AppendBand(rsBc2.Item(index));
                rst = (IRaster)rsBc;
            }
            return rst;
        }        
        private IRaster shiftRaster(object inRaster, double shiftX, double shiftY)
        {
            IRaster inRs = returnRaster(inRaster);
            IRasterProps rsProp = (IRasterProps)inRs;
            IEnvelope env = rsProp.Extent;
            IRasterGeometryProc3 rsGeoProc3 = new RasterGeometryProcClass();
            rsGeoProc3.Shift(shiftX, shiftY, inRs);
            return inRs;
        }
        /// <summary>
        /// performs a x and y shift of the input raster
        /// </summary>
        /// <param name="inRaster">IRaster, IRasterDataset, string path</param>
        /// <param name="shiftX">nuber of cells to shift positive number move to the east negative number move to the west</param>
        /// <param name="shiftY">number of cells to shift positve number move north negative numebr move south</param>
        /// <returns></returns>
        public IRaster shiftRasterFunction(object inRaster, double shiftx, double shiftY)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new GeometricFunctionClass();
            IRaster2 rs = (IRaster2)shiftRaster(inRaster,shiftx,shiftY);
            IGeometricFunctionArguments args = new GeometricFunctionArgumentsClass();
            args.Raster = returnRaster(inRaster);
            args.GeodataXform = rs.GeodataXform;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            return outRs;
        }
        /// <summary>
        /// performs GLCM homogeneity moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmHomogeneity(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius,out iterLst);
            int width = ((radius - 1) * 2) + 1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    int crVl = circleKrn[c, r];
                    if (crVl == 0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == (width - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crPVl = circleKrn[crP, r];
                                if (crPVl == 0)
                                {
                                    vl = 0;
                                }

                            }
                        }
                        else
                        {
                            if (r == (height - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crM = r + 1;
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0)
                                {
                                    vl = 0;
                                }
                            }
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            double div = krnLst.Sum();
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster minRs2 = calcArithmaticFunction(minRs, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster pRs = calcArithmaticFunction(minRs2, 1, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster invRs = calcArithmaticFunction(pRs, -1, esriRasterArithmeticOperation.esriRasterPower);
            return convolutionRasterFunction(invRs, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM homogenity moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmHomogeneity(object inRaster, int width, int height, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = (div + ((width - 2) * height)) / 2;
            }
            else
            {
                div = (div + ((height - 2) * width)) / 2;
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    if (horizontal)
                    {
                        if (c == (width - 1))
                        {
                            vl = 0;
                        }
                    }
                    else
                    {
                        if (r == (height - 1))
                        {
                            vl = 0;
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster minRs2 = calcArithmaticFunction(minRs, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster pRs = calcArithmaticFunction(minRs2, 1, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster invRs = calcArithmaticFunction(pRs, -1, esriRasterArithmeticOperation.esriRasterPower);
            return convolutionRasterFunction(invRs, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM Dissimilarity moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmDissimilarity(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius, out iterLst);
            int width = ((radius - 1) * 2) + 1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    int crVl = circleKrn[c, r];
                    if (crVl == 0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == (width - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crPVl = circleKrn[crP, r];
                                if (crPVl == 0)
                                {
                                    vl = 0;
                                }

                            }

                        }
                        else
                        {
                            if (r == (height - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crM = r + 1;
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0)
                                {
                                    vl = 0;
                                }
                            }
                        }
                    }
                    krnLst.Add(vl);

                }
            }
            double div = krnLst.Sum(); 
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster absRs = calcMathRasterFunction(minRs,transType.ABS);
            return convolutionRasterFunction(absRs, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM Dissimilarity moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmDissimilarity(object inRaster, int width, int height, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = (div + ((width - 2) * height)) / 2;
            }
            else
            {
                div = (div + ((height - 2) * width)) / 2;
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    if (horizontal)
                    {
                        if (c == (width - 1))
                        {
                            vl = 0;
                        }
                    }
                    else
                    {
                        if (r == (height - 1))
                        {
                            vl = 0;
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster absRs = calcMathRasterFunction(minRs,transType.ABS);
            return convolutionRasterFunction(absRs, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM Contrast moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmContrast(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius, out iterLst);
            int width = ((radius-1)*2)+1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    int crVl = circleKrn[c, r];
                    if (crVl == 0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == (width - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crPVl = circleKrn[crP, r];
                                if (crPVl == 0)
                                {
                                    vl = 0;
                                }
                                
                            }
                        }
                        else
                        {
                            if (r == (height - 1))
                            {
                                vl = 0;
                            }
                            else
                            {
                                int crM = r + 1;
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0)
                                {
                                    vl = 0;
                                }
                            }
                        }
                    }
                    krnLst.Add(vl);
                }
            }

            double div = krnLst.Sum();
            //Console.WriteLine("Div = " + div.ToString());
            for (int i = 0; i < krnLst.Count; i++)
            {
                //Console.WriteLine(krnLst[i]);
                krnLst[i] = krnLst[i] / div;
                //Console.WriteLine(krnLst[i]);
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster minRs2 = calcArithmaticFunction(minRs, 2, esriRasterArithmeticOperation.esriRasterPower);
            return convolutionRasterFunction(minRs2, width,height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM Contrast moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmContrast(object inRaster, int width, int height, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = (div + ((width - 2) * height))/2;
            }
            else
            {
                div = (div + ((height - 2) * width))/2;
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 1;
                    if (horizontal)
                    {
                        if ( c == (width - 1))
                        {
                            vl = 0;
                        }
                    }
                    else
                    {
                        if (r == (height - 1))
                        {
                            vl = 0;
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            IRaster minRs = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster minRs2 = calcArithmaticFunction(minRs, 2, esriRasterArithmeticOperation.esriRasterPower);
            return convolutionRasterFunction(minRs2, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM Correlation moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmCorrelation(object inRaster, int radius, bool horizontal)
        {
            IRaster cov = glcmCoVariance(inRaster, radius, horizontal);
            IRaster var = glcmVariance(inRaster, radius, horizontal);
            IRemapFilter rmFilt = new RemapFilterClass();
            rmFilt.AddClass((-1 * 0.000000001), 0.0000000001, 1);
            IRaster rmapV = calcRemapFunction(var, rmFilt);
            IRaster rmapC = calcRemapFunction(cov, rmFilt);
            IRaster cor = calcArithmaticFunction(rmapC, rmapV, esriRasterArithmeticOperation.esriRasterDivide);
            return cor;
        }
        /// <summary>
        /// performs GLCM Correlation moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmCorrelation(object inRaster, int width, int height, bool horizontal)
        {
            IRaster cov = glcmCoVariance(inRaster, width, height, horizontal);
            IRaster var = glcmVariance(inRaster, width, height, horizontal);
            IRemapFilter rmFilt = new RemapFilterClass();
            rmFilt.AddClass((-1 * 0.000000001), 0.0000000001, 1);
            IRaster rmapV = calcRemapFunction(var, rmFilt);
            IRaster rmapC = calcRemapFunction(cov, rmFilt);
            IRaster cor = calcArithmaticFunction(rmapC, rmapV, esriRasterArithmeticOperation.esriRasterDivide);
            return cor;
        }
        /// <summary>
        /// performs GLCM CoVariance moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmCoVariance(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<double> sumLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius, out iterLst);
            int width = ((radius - 1) * 2) + 1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2;
                    int crVl = circleKrn[c, r];
                    if (crVl == 0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == 0 || c == (width - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crM = c - 1;
                                int crPVl = circleKrn[crP, r];
                                int crMVl = circleKrn[crM, r];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }

                        }
                        else
                        {
                            if (r == 0 || r == (height - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = r + 1;
                                int crM = r - 1;
                                int crPVl = circleKrn[c, crP];
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    krnLst.Add(vl);
                    vl = 1;
                    if (horizontal)
                    {
                        if (c == (width - 1))
                        {
                            vl = 0;
                        }
                        else
                        {
                            int crP = c + 1;
                            int crPVl = circleKrn[crP, r];
                            if (crPVl == 0)
                            {
                                vl = 0;
                            }
                        }

                    }
                    else
                    {
                        if (r == (height - 1))
                        {
                            vl = 0;
                        }
                        else
                        {
                            int crP = r + 1;
                            int crPVl = circleKrn[c, crP];
                            if (crPVl == 0)
                            {
                                vl = 0;
                            }
                        }
                    }
                    sumLst.Add(vl);
                }
            }
            double div = krnLst.Sum();
            IRaster rsY = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            //SumXY
            IRaster XY = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster SumXy = convolutionRasterFunction(XY, width, height, sumLst.ToArray());
            IRaster SumXy2 = calcArithmaticFunction(SumXy, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            //SumXSumY
            IRaster SumXPlusSumY = convolutionRasterFunction(inRaster, width, height, krnLst.ToArray());
            IRaster SumXPlusSumY2 = calcArithmaticFunction(SumXPlusSumY, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster SumXPlusSumYDiv = calcArithmaticFunction(SumXPlusSumY2, div, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster dif = calcArithmaticFunction(SumXy2, SumXPlusSumYDiv, esriRasterArithmeticOperation.esriRasterMinus);
            return calcArithmaticFunction(dif, div, esriRasterArithmeticOperation.esriRasterDivide);
        }
        /// <summary>
        /// performs GLCM CoVariance moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmCoVariance(object inRaster, int width, int height, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<double> sumLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = div + ((width - 2) * height);
            }
            else
            {
                div = div + ((height - 2) * width);
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2;
                    if (horizontal)
                    {
                        if (c == 0 || c == (width - 1))
                        {
                            vl = 1;
                        }
                    }
                    else
                    {
                        if (r == 0 || r == (height - 1))
                        {
                            vl = 1;
                        }
                    }
                    krnLst.Add(vl);
                    vl = 1;
                    if(horizontal)
                    {
                        if(c==(width-1))
                        {
                            vl=0;
                        }
                    }
                    else
                    {
                        if(r==(height-1))
                        {
                            vl=0;
                        }
                    }
                    sumLst.Add(vl);
                }
            }
            IRaster rsY  = null;
            if (horizontal)
            {
                rsY = shiftRasterFunction(inRaster, -1, 0);
            }
            else
            {
                rsY = shiftRasterFunction(inRaster, 0, -1);
            }
            //SumXY
            IRaster XY = calcArithmaticFunction(inRaster, rsY, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster SumXy = convolutionRasterFunction(XY,width,height,sumLst.ToArray());
            IRaster SumXy2 = calcArithmaticFunction(SumXy,2,esriRasterArithmeticOperation.esriRasterMultiply);
            //SumXSumY
            IRaster SumXPlusSumY = convolutionRasterFunction(inRaster,width,height,krnLst.ToArray());
            IRaster SumXPlusSumY2 = calcArithmaticFunction(SumXPlusSumY, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster SumXPlusSumYDiv = calcArithmaticFunction(SumXPlusSumY2, div, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster dif = calcArithmaticFunction(SumXy2, SumXPlusSumYDiv, esriRasterArithmeticOperation.esriRasterMinus);
            return calcArithmaticFunction(dif,div,esriRasterArithmeticOperation.esriRasterDivide);

        }
        /// <summary>
        /// performs GLCM Variance moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmVariance(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius, out iterLst);
            int width = ((radius - 1) * 2) + 1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2;
                    int crVl = circleKrn[c, r];
                    if (crVl == 0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == 0 || c == (width - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crM = c - 1;
                                int crPVl = circleKrn[crP, r];
                                int crMVl = circleKrn[crM, r];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }

                        }
                        else
                        {
                            if (r == 0 || r == (height - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = r + 1;
                                int crM = r - 1;
                                int crPVl = circleKrn[c, crP];
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            double div = krnLst.Sum();
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i];
            }
            //pow(sum(X),2)/N
            IRaster sumX = convolutionRasterFunction(inRaster, width, height, krnLst.ToArray());
            IRaster sumX2 = calcArithmaticFunction(sumX, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumX2Div = calcArithmaticFunction(sumX2, div, esriRasterArithmeticOperation.esriRasterDivide);
            //sum(pow(X,2))
            IRaster X2 = calcArithmaticFunction(inRaster, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumPowX = convolutionRasterFunction(X2, width, height, krnLst.ToArray());
            IRaster minRs = calcArithmaticFunction(sumPowX, sumX2Div, esriRasterArithmeticOperation.esriRasterMinus);
            return calcArithmaticFunction(minRs, div, esriRasterArithmeticOperation.esriRasterDivide);
        }
        /// <summary>
        /// performs GLCM Variance moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmVariance(object inRaster, int width, int height, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = div + ((width - 2) * height);
            }
            else
            {
                div = div + ((height - 2) * width);
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2;
                    if (horizontal)
                    {
                        if (c == 0 || c == (width - 1))
                        {
                            vl = 1;
                        }
                    }
                    else
                    {
                        if (r == 0 || r == (height - 1))
                        {
                            vl = 1;
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            //pow(sum(X),2)/N
            IRaster sumX = convolutionRasterFunction(inRaster, width, height, krnLst.ToArray());
            IRaster sumX2 = calcArithmaticFunction(sumX, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumX2Div = calcArithmaticFunction(sumX2, div, esriRasterArithmeticOperation.esriRasterDivide);
            //sum(pow(X,2))
            IRaster X2 = calcArithmaticFunction(inRaster, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumPowX = convolutionRasterFunction(X2, width, height, krnLst.ToArray());
            IRaster minRs = calcArithmaticFunction(sumPowX, sumX2Div, esriRasterArithmeticOperation.esriRasterMinus);
            return calcArithmaticFunction(minRs, div, esriRasterArithmeticOperation.esriRasterDivide);
        }
        /// <summary>
        /// performs GLCM Mean moving window analysis using a circular window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="radius"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmMean(object inRaster, int radius, bool horizontal)
        {
            List<double> krnLst = new List<double>();
            List<int[]> iterLst = new List<int[]>();
            int[,] circleKrn = createFocalWindowCircle(radius, out iterLst);
            int width = ((radius - 1) * 2) + 1;
            int height = width;
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2;
                    int crVl = circleKrn[c, r];
                    if(crVl==0)
                    {
                        vl = 0;
                    }
                    else
                    {
                        if (horizontal)
                        {
                            if (c == 0 || c == (width - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = c + 1;
                                int crM = c - 1;
                                int crPVl = circleKrn[crP, r];
                                int crMVl = circleKrn[crM, r];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }

                        }
                        else
                        {
                            if (r == 0 || r == (height - 1))
                            {
                                vl = 1;
                            }
                            else
                            {
                                int crP = r + 1;
                                int crM = r - 1;
                                int crPVl = circleKrn[c, crP];
                                int crMVl = circleKrn[c, crM];
                                if (crMVl == 0 && crPVl == 0)
                                {
                                    vl = 0;
                                }
                                else if (crMVl == 0 || crPVl == 0)
                                {
                                    vl = 1;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            double div = krnLst.Sum();
            for (int i = 0; i < krnLst.Count; i++)
            {
                krnLst[i] = krnLst[i] / div;
            }
            return convolutionRasterFunction(inRaster, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs GLCM mean moving window analysis using a rectangle window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="horizontal">if true will look at horizontal neighbor values</param>
        /// <returns></returns>
        public IRaster glcmMean(object inRaster, int width, int height, bool horizontal )
        {
            List<double> krnLst = new List<double>();
            double div = width * height;
            if (horizontal)
            {
                div = div + ((width - 2) * height);
            }
            else
            {
                div = div + ((height - 2) * width);
            }
            for (int c = 0; c < width; c++)
            {
                for (int r = 0; r < height; r++)
                {
                    double vl = 2/div;
                    if (horizontal)
                    {
                        if (c == 0 || c == (width - 1))
                        {
                            vl = 1/div;
                        }
                    }
                    else
                    {
                        if (r == 0 || r == (height - 1))
                        {
                            vl = 1/div;
                        }
                    }
                    krnLst.Add(vl);
                }
            }
            return convolutionRasterFunction(inRaster, width, height, krnLst.ToArray());
        }
        /// <summary>
        /// performs a convolution analysis for a defined kernal
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="kn"></param>
        /// <returns></returns>
        public IRaster convolutionRasterFunction(object inRaster, int width, int height, double[] kn)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ConvolutionFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
            IRaster rs = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
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
            IRaster outRs = createRaster((IRasterDataset)frDset);
            if (width > 3 || height > 3)
            {
                IRasterProps rsProp = (IRasterProps)outRs;
                IEnvelope env = rsProp.Extent;
                IRasterGeometryProc3 rsGeoProc3 = new RasterGeometryProcClass();
                int addY = 0;
                int addX = 0;
                if (width > 3)
                {
                    addX = (((width + 1) / 2) - 2);
                    
                }

                if (height > 3)
                {
                    addY = -1 * (((height + 1) / 2) - 2);
                    
                }
                outRs = shiftRasterFunction(outRs, addX, addY);
            }
            double cells = getNFromKernal(kn);
            functionModel.estimateStatistics(rs, outRs, focalType.SUM, cells);
            return outRs;
            
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
        public IRaster calcMathRasterFunction(object inRaster, transType typ)
        {
            IRaster rRst = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
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
                default:
                    rsFunc = new FunctionRasters.absFunctionDataset();
                    break;
            }
            FunctionRasters.MathFunctionArguments args = new FunctionRasters.MathFunctionArguments(this);
            args.InRaster = rRst;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(rRst, outRs, typ);
            return outRs;
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
        public IRaster createNewRaster(IRaster templateRaster, IWorkspace outWks, string outRasterName, int numBands, rstPixelType pixelType)
        {
            outRasterName = getSafeOutputName(outWks, outRasterName);
            IRasterProps rstProps = (IRasterProps)templateRaster;
            IRasterDataset2 newRstDset = null;
            IRaster rs = null;
            if (outWks.Type == esriWorkspaceType.esriFileSystemWorkspace)
            {
                if (pixelType == rstPixelType.PT_DOUBLE) pixelType = rstPixelType.PT_FLOAT;
                IPnt mPnt = rstProps.MeanCellSize();
                double dX = mPnt.X;
                double dY = mPnt.Y;
                IRasterWorkspace2 rsWks = (IRasterWorkspace2)outWks;
                newRstDset = (IRasterDataset2)rsWks.CreateRasterDataset(outRasterName, "GRID", (rstProps.Extent.Envelope.LowerLeft), rstProps.Width, rstProps.Height, dX, dY, numBands, pixelType, rstProps.SpatialReference, true);
                rs = newRstDset.CreateFullRaster();
            }
            else
            {
                IRasterWorkspaceEx rsWks = (IRasterWorkspaceEx)outWks;
                IRemapFilter rFilt = new RemapFilterClass();
                rFilt.AddClass(Double.MinValue,Double.MaxValue,getNoDataValue(rstPixelType.PT_DOUBLE));
                IRaster nullRaster = calcRemapFunction(templateRaster, rFilt);
                nullRaster = convertToDifFormatFunction(nullRaster, pixelType);
                IRasterDef rsDef = new RasterDefClass();
                IRasterStorageDef2 rsStDef = new RasterStorageDefClass();
                rsStDef.Tiled = true;
                rsStDef.TileHeight = 128;
                rsStDef.TileWidth = 128;
                rsStDef.CellSize = rstProps.MeanCellSize();
                newRstDset = (IRasterDataset2)rsWks.SaveAsRasterDataset(outRasterName, nullRaster, rsStDef, "", null, null);
                IRasterDatasetEdit2 newRstDset3E = (IRasterDatasetEdit2)newRstDset;
                try
                {
                    newRstDset3E.DeleteStats();
                    newRstDset3E.DeleteAttributeTable();
                }
                catch
                {
                }
                rs = newRstDset.CreateFullRaster();
            }
            return rs;
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
        public IRaster createNewRaster(IEnvelope env, IPnt meanCellSize,IWorkspace outWks, string outRasterName, int numBands, rstPixelType pixelType, ISpatialReference spRf)
        {
            deleteRasterDataset(outWks.PathName + "\\" + outRasterName);
            IRasterDataset3 newRstDset = null;
            IRaster rs = null;
            if (outWks.Type == esriWorkspaceType.esriFileSystemWorkspace)
            {
                double dX = meanCellSize.X;
                double dY = meanCellSize.Y;
                IRasterWorkspace2 rsWks = (IRasterWorkspace2)outWks;
                newRstDset = (IRasterDataset3)rsWks.CreateRasterDataset(outRasterName, "IMAGINE Image", env.LowerLeft, System.Convert.ToInt32(env.Width / dX), System.Convert.ToInt32(env.Height / dY), dX, dY, numBands, pixelType, spRf, true);
                rs = newRstDset.CreateFullRaster();
            }
            else
            {
                IRasterWorkspaceEx rsWks = (IRasterWorkspaceEx)outWks;
                IRasterDef rsDef = new RasterDefClass();
                IRasterStorageDef rsStDef = new RasterStorageDefClass();
                rsStDef.Origin = env.LowerLeft;
                rsStDef.TileHeight = 128;
                rsStDef.TileWidth = 128;
                rsStDef.CellSize = meanCellSize;
                rsDef.SpatialReference = spRf;
                newRstDset = (IRasterDataset3)rsWks.CreateRasterDataset(outRasterName, numBands, pixelType, rsStDef, null, rsDef, null);
                rs = newRstDset.CreateFullRaster();
                IRasterProps rsPr = (IRasterProps)rs;
                rsPr.Height = System.Convert.ToInt32(env.Height);
                rsPr.Width = System.Convert.ToInt32(env.Width);
                rsPr.Extent = env;
            }
            return rs;
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
        /// <param name="op">the type of opporation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IRaster calcArithmaticFunction(object inRaster1, object inRaster2, esriRasterArithmeticOperation op)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ArithmeticFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
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
                iR2 = returnRaster(inRaster2,rstPixelType.PT_DOUBLE);
                IScalar sc = new ScalarClass();
                int bCnt = ((IRasterBandCollection)iR2).Count;
                double[] d = new double[bCnt];
                for (int i = 0; i < bCnt; i++)
                {
                    d[i] = System.Convert.ToDouble(inRaster1);
                }
                sc.Value = d;
                iR1 = sc;
            }
            else if (isNumeric(inRaster2.ToString()) && !isNumeric(inRaster1.ToString()))
            {
                iR1 = returnRaster(inRaster1,rstPixelType.PT_DOUBLE);
                IScalar sc = new ScalarClass();
                int bCnt = ((IRasterBandCollection)iR1).Count;
                double[] d = new double[bCnt];
                for (int i = 0; i < bCnt; i++)
                {
                    d[i] = System.Convert.ToDouble(inRaster2);
                }
                sc.Value = d;
                iR2 = sc;
            }
            else
            {
                iR1 = returnRaster(inRaster1,rstPixelType.PT_DOUBLE);
                iR2 = returnRaster(inRaster2,rstPixelType.PT_DOUBLE);
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
                        for (int i = 0; i < absDif; i++)
                        {
                            IRaster rs = getBand(iR2, 0);
                            rsBc1.AppendBands((IRasterBandCollection)rs);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < absDif; i++)
                        {
                            IRaster rs = getBand(iR1, 0);
                            rsBc1.AppendBands((IRasterBandCollection)rs);
                        }
                    }
                }
            }
            args.Raster = iR1;
            args.Raster2 = iR2;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(iR1, iR2, outRs, op);
            return outRs;
            
            
        }
        public IRasterDataset saveRasterToDataset(IRaster inRaster, string outName, IWorkspace wks,rasterType rastertype)
        {
            string rsTypeStr = rastertype.ToString();
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
            else
            {
                ext = "." + rastertype.ToString().ToLower();
            }
            esriWorkspaceType tp = wks.Type;
            if (tp == esriWorkspaceType.esriLocalDatabaseWorkspace)
            {
                rsTypeStr = rasterType.GDB.ToString();
            }
            if (rastertype == rasterType.GRID || rastertype == rasterType.GDB)
            {
                outName = getSafeOutputName(wks, outName);
                if (outName.Length > 12)
                {
                    outName.Substring(12);
                }
                if ((rastertype==rasterType.GRID)&&(((IRasterProps)inRaster).PixelType == rstPixelType.PT_DOUBLE))
                {
                    inRaster = convertToDifFormatFunction(inRaster, rstPixelType.PT_FLOAT);
                }
            }
            else
            {
                if (outName.IndexOf(ext) == -1)
                {
                    outName = outName + ext;
                }

            }
            if (geoUtil.ftrExists(wks, outName))
            {
                deleteRasterDataset(wks.PathName + "\\" + outName);
            }
            IRasterDataset rsDset = null;
            try
            {
                ISaveAs sv = (ISaveAs)inRaster;
                rsDset = (IRasterDataset)sv.SaveAs(outName, wks, rsTypeStr);
                IRaster2 rs2 = (IRaster2)calcStatsAndHist(rsDset);
                ITable vat = rs2.AttributeTable;
                int rwCnt = 0;
                try
                {
                    rwCnt = vat.RowCount(null);
                }
                catch
                {
                    rwCnt = 0;
                }
                if (rwCnt > 0)
                {
                    IRasterDatasetEdit2 rsDsetE = (IRasterDatasetEdit2)rsDset;
                    rsDsetE.DeleteAttributeTable();
                    if (((IRasterBandCollection)rs2).Count == 1)
                    {
                        rsDsetE.BuildAttributeTable();
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                Console.WriteLine(e.ToString());
                rsDset = ((IRaster2)returnRaster(wks.PathName + "\\" + outName)).RasterDataset;
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
                rsType = rasterType.GRID;
            }
            return saveRasterToDataset(inRaster, outName, wks, rsType);
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
        public IRaster calcStatsAndHist(IRasterDataset rsDset)
        {
            IRaster rs = returnRaster(rsDset);
            rs = calcStatsAndHist(rs);
            return rs;
        }
        /// <summary>
        /// Rescales raster to 8 byte unsigned integer 0-256
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster reScaleRasterFunction(object inRaster)
        {
            return reScaleRasterFunction(inRaster, rstPixelType.PT_UCHAR);
        }
        /// <summary>
        /// Rescales raster to a given raster pixel type min max value
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster reScaleRasterFunction(object inRaster, rstPixelType pType)
        {
            return reScaleRasterFunction(inRaster,pType,esriRasterStretchType.esriRasterStretchMinimumMaximum);
        }
        public IRaster reScaleRasterFunction(object inRaster,rstPixelType pType,esriRasterStretchType stretchType)
        {
            IRaster iR1 = returnRaster(inRaster);
            calcStatsAndHist(iR1);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new StretchFunction();
            rsFunc.PixelType = pType;
            
            //IRasterProps rsProps = (IRasterProps)iR1;
            //rsFunc.RasterInfo.Extent = rsProps.Extent;
            //rsFunc.RasterInfo.CellSize = getCellSize(iR1);
            IStretchFunctionArguments args = new StretchFunctionArgumentsClass();
            args.Raster = iR1;
            args.StretchType = stretchType;
            frDset.Init(rsFunc, args);
            //frDset.Simplify();
            IRaster rs = createRaster((IRasterDataset)frDset);
            return rs;


        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="clm">number of columns (cells)</param>
        /// <param name="rws">number of rows</param>
        /// <param name="statType">the type of opporation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IRaster calcFocalStatisticsFunction(object inRaster, int clm, int rws, focalType statType)
        {
            IRaster iR1 = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (statType)
            {
                case focalType.MIN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMin();
                    break;
                case focalType.SUM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperSum();
                    break;
                case focalType.MEAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMean();
                    break;
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMedian();
                    break;
                case focalType.VARIANCE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperVariance();
                    break;
                case focalType.STANDARD_DEVIATION:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperStd();
                    break;
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperEntropy();
                    break;
                case focalType.PROBABILITY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperProbability();
                    break;
                default:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMax();
                    break;
            }
            FunctionRasters.FocalFunctionArguments args = new FunctionRasters.FocalFunctionArguments(this);
            args.Rows = rws;
            args.Columns = clm;
            //args.WindowType = windowType.RECTANGLE;
            args.InRaster = iR1;
            args.Operation = statType;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            double cells  = clm*rws;
            functionModel.estimateStatistics(iR1, outRs, statType, cells);
            return outRs;
        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="radius">number of cells that make up the radius of a circle</param>
        /// <param name="statType">the type of opporation</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IRaster calcFocalStatisticsFunction(object inRaster, int radius, focalType statType)
        {
            IRaster iR1 = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = null;
            switch (statType)
            {
                case focalType.MIN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMin();
                    break;
                case focalType.SUM:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperSum();
                    break;
                case focalType.MEAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMean();
                    break;
                case focalType.MODE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMode();
                    break;
                case focalType.MEDIAN:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMedian();
                    break;
                case focalType.VARIANCE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperVariance();
                    break;
                case focalType.STANDARD_DEVIATION:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperStd();
                    break;
                case focalType.UNIQUE:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperUnique();
                    break;
                case focalType.ENTROPY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperEntropy();
                    break;
                case focalType.PROBABILITY:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperProbability();
                    break;
                default:
                    rsFunc = new FunctionRasters.NeighborhoodHelper.focalHelperMax();
                    break;
            }
            rsFunc.PixelType = rstPixelType.PT_FLOAT;
            FunctionRasters.FocalFunctionArguments args = new FunctionRasters.FocalFunctionArguments(this);
            args.Radius = radius;
            args.InRaster = iR1;
            //args.WindowType = windowType.CIRCLE;
            args.Operation = statType;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            double cells = getNFromCircle(radius);
            functionModel.estimateStatistics(iR1, outRs, statType, cells);
            return outRs;
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
        /// <param name="statType">the type of opporation</param>
        /// <param name="landType">the type of metric</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IRaster calcLandscapeFunction(object inRaster, int clm, int rws, focalType statType, landscapeType landType)
        {
            IRaster iR1 = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
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
            IRaster rs = createRaster((IRasterDataset)frDset);
            return rs;
        }
        /// <summary>
        /// Will perform a focal raster operation on an input raster all bands
        /// </summary>
        /// <param name="inRaster">either IRaster, IRasterDataset, or a valid path pointing to a raster</param>
        /// <param name="radius">number of cells that make up the radius of the moving window</param>
        /// <param name="statType">the type of opporation</param>
        /// <param name="landType">the type of metric</param>
        /// <returns>a IRaster that can be used for further analysis</returns>
        public IRaster calcLandscapeFunction(object inRaster, int radius, focalType statType, landscapeType landType)
        {
            IRaster iR1 = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
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
            IRaster rs = createRaster((IRasterDataset)frDset);
            return rs;
        }
        /// <summary>
        /// Returns a IRaster given either a path, IRasterdataset, or IRaster
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster returnRaster(object inRaster)
        {
            IRaster iR1 = null;
            try
            {
                if (inRaster is Raster)
                {
                    IRasterBandCollection rsBc = new RasterClass();
                    rsBc.AppendBands((IRasterBandCollection)((IRaster)inRaster));
                    iR1 = (IRaster)rsBc;
                }
                else if (inRaster is RasterDataset)
                {
                    iR1 = createRaster((IRasterDataset)inRaster);
                }
                else if (inRaster is String)
                {
                    string rsNm = System.IO.Path.GetFileNameWithoutExtension(inRaster.ToString());
                    string bnd = "";
                    iR1 = createRaster(openRasterDataset(inRaster.ToString(), out bnd));
                    if (bnd.ToLower()!="all")
                    {
                        if (isNumeric(bnd))
                        {
                            int bndNum = System.Convert.ToInt32(bnd)-1;
                            iR1 = getBand(iR1, bndNum);
                        }
                    }
                }
                else
                {
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return iR1;
        }
        public IRaster returnRaster(object inRaster, rstPixelType pType)
        {
            IRaster rs = returnRaster(inRaster);
            IRasterProps rsProps = (IRasterProps)rs;
            if(rsProps.PixelType!=pType)
            {
                rs = convertToDifFormatFunction(rs, pType);
            }
            return rs;
        }
        /// <summary>
        /// samples all bands of a given raster given a point feature class. Appends those values to a field named by the raster/band.
        /// </summary>
        /// <param name="inFtrCls">Point feature class that is used to sample</param>
        /// <param name="sampleRst">Raster dataset that is going to be sampled</param>
        /// <returns>a list of all the created field names</returns>
        public string[] sampleRaster(IFeatureClass inFtrCls, IRaster sampleRst, string inName)
        {
            List<string> outLst = new List<string>();
            IRaster2 sr = (IRaster2)sampleRst;
            Dictionary<int, string> lc = new Dictionary<int, string>();
            IRasterBandCollection rsBC = (IRasterBandCollection)sr;
            IEnumRasterBand rsBE = rsBC.Bands;
            IRasterBand rsB = rsBE.Next();
            string rsName = inName;
            if(rsName==null)
            {
                rsName = ((IDataset)sr.RasterDataset).Name;
            }
            int cntB = 0;
            while (rsB != null)
            {
                string fldName = rsName + "_Band" + (cntB + 1).ToString();
                //fldName = geoUtil.getSafeFieldName(inFtrCls, fldName);
                outLst.Add(fldName);
                esriFieldType fldType = esriFieldType.esriFieldTypeDouble;
                fldName = geoUtil.createField(inFtrCls, fldName, fldType);
                lc.Add(cntB,fldName);
                cntB++;
                rsB = rsBE.Next();
            }
            IGeometry geo = (IGeometry)((IRasterProps)sampleRst).Extent;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = geo;
            spFlt.GeometryField = inFtrCls.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor sCur = inFtrCls.Search(spFlt, false);
            IFeature sRow = sCur.NextFeature();
            while (sRow != null)
            {
                geo = sRow.Shape;
                IPoint pnt = (IPoint)geo;
                int x,y;
                sr.MapToPixel(pnt.X, pnt.Y, out x, out y);
                for (int i = 0; i < ((IRasterBandCollection)sr).Count; i++)
                {
                    int fldIndex = inFtrCls.FindField(lc[i]);
                    object rsVl = sr.GetPixelValue(i, x, y);
                    try
                    {
                        sRow.set_Value(fldIndex, rsVl);
                    }
                    catch
                    {
                        Console.WriteLine(rsVl.ToString());
                    }

                }
                sRow.Store();
                sRow = sCur.NextFeature();
            }
            return outLst.ToArray();

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
            List<string> outLst = new List<string>();
            IRaster2 sr = (IRaster2)sampleRst;
            Dictionary<int, string> lc = new Dictionary<int, string>();
            IRasterBandCollection rsBC = (IRasterBandCollection)sr;
            IEnumRasterBand rsBE = rsBC.Bands;
            IRasterBand rsB = rsBE.Next();
            string rsName = inName;
            if (rsName == null)
            {
                rsName = ((IDataset)sr.RasterDataset).Name;
            }
            int cntB = 0;
            while (rsB != null)
            {
                string fldName = rsName + "_Band" + (cntB + 1).ToString();
                //fldName = geoUtil.getSafeFieldName(inFtrCls, fldName);
                outLst.Add(fldName);
                esriFieldType fldType = esriFieldType.esriFieldTypeDouble;
                fldName = geoUtil.createField(inFtrCls, fldName, fldType);
                lc.Add(cntB, fldName);
                cntB++;
                rsB = rsBE.Next();
            }
            IGeometry geo = (IGeometry)((IRasterProps)sampleRst).Extent;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = geo;
            spFlt.GeometryField = inFtrCls.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor sCur = inFtrCls.Search(spFlt, false);
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
                        int fldIndex = inFtrCls.FindField(lc[i]);
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
                        sRow.set_Value(fldIndex, rsVl);
                    }
                    catch
                    {
                        Console.WriteLine(rsVl.ToString());
                    }

                }
                sRow.Store();
                sRow = sCur.NextFeature();
            }
            return outLst.ToArray();

        }
        /// <summary>
        /// Remaps the values of a given raster to new set of values
        /// </summary>
        /// <param name="inRaster">input raster</param>
        /// <param name="filter">a remap filter</param>
        /// <returns>IRaster with remaped values</returns>
        public IRaster calcRemapFunction(object inRaster, IRemapFilter filter)
        {
            IRaster rRst = returnRaster(inRaster);
            IRasterProps rsProps = (IRasterProps)rRst;
            double ndV = System.Convert.ToDouble(((System.Array)rsProps.NoDataValue).GetValue(0));
            double nndV = getNoDataValue(rstPixelType.PT_DOUBLE);
            IDoubleArray rangeArray = new DoubleArrayClass();
            IDoubleArray valueArray = new DoubleArrayClass();
            if (ndV != nndV)
            {
                rangeArray.Add(ndV);
                rangeArray.Add(ndV + 0.00000000000001);
                valueArray.Add(nndV);
            }
            double min,max,vl;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new RemapFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
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
            return createRaster((IRasterDataset)frDset);

        }
        /// <summary>
        /// Calculates a trend raster from double filter
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="doubleFilter">a double array of plan values</param>
        /// <returns>IRaster</returns>
        public IRaster calcTrendFunction(object inRaster, double[] doubleFilter)
        {
            IDoubleArray dbArray = new DoubleArrayClass();
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new TrendFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
            ITrendFunctionArguments args = new TrendFunctionArgumentsClass();
            args.Raster = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
            for (int i = 0; i < doubleFilter.Length; i++)
            {
                dbArray.Add(doubleFilter[i]);
            }
            args.PlaneParameters = dbArray;
            frDset.Init(rsFunc, args);
            return createRaster((IRasterDataset)frDset);

        }
        /// <summary>
        /// regresses sums an intercept value to the sum product of a series of raster bands and corresponding slope values. Number of bands and slope values must match
        /// </summary>
        /// <param name="inRaster">string IRaster, or IRasterDataset that has the same number of bands as the slopes array has values</param>
        /// <param name="intercept">double representing the intercept of the regression equation</param>
        /// <param name="slopes">double[] representing the corresponding slope values</param>
        /// <returns></returns>
        public IRaster calcRegressFunction(object inRaster, double intercept, double[] slopes)
        {
            IRaster iR1 = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ArithmeticFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
            IArithmeticFunctionArguments args = new ArithmeticFunctionArgumentsClass();
            args.Operation = esriRasterArithmeticOperation.esriRasterMultiply;
            IScalar sc = new ScalarClass();
            sc.Value = slopes;
            args.Raster = iR1;
            args.Raster2 = sc;
            frDset.Init(rsFunc, args);
            //frDset.Simplify();
            IRasterDataset sRsDataset = (IRasterDataset)frDset;
            IRasterBandCollection sRsBc = (IRasterBandCollection)sRsDataset;
            IRaster sRs = getBand(sRsBc,0);
            IRaster[] rstArr = new IRaster[sRsBc.Count];
            rstArr[0] = calcArithmaticFunction(sRs, intercept,esriRasterArithmeticOperation.esriRasterPlus);
            for (int i = 1; i < sRsBc.Count; i++)
            {
                IRaster nbcR = getBand(sRsBc,i);
                IRaster pR = rstArr[i-1];
                rstArr[i]=calcArithmaticFunction(nbcR,pR,esriRasterArithmeticOperation.esriRasterPlus);
            }
            return rstArr[rstArr.GetUpperBound(0)];

        }
        /// <summary>
        /// Remaps values greater than or equal to the input vl to 1. Values less than vl =0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or raster to compare against</param>
        /// <returns>IRaster</returns>
        public IRaster calcGreaterEqualFunction(object inRaster, object compareRaster)
        {
            IRaster rs = returnRaster(inRaster);
            IRaster outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                //Console.WriteLine("Is Number");
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                IRasterProps rsP = (IRasterProps)rs;
                rstPixelType pType = rsP.PixelType;
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
                IRaster crs = returnRaster(compareRaster);
                //Console.WriteLine("Is Raster");
                IRaster minRst = calcArithmaticFunction(rs, crs, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcGreaterEqualFunction(minRst, 0);
            }
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// Remaps values greater than to the input vl to 1. Values less than vl =0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or raster to compare against</param>
        /// <returns>IRaster</returns>
        public IRaster calcGreaterFunction(object inRaster, object compareRaster)
        {
            IRaster rs = returnRaster(inRaster);
            IRaster outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                //Console.WriteLine("Is Number");
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                IRasterProps rsP = (IRasterProps)rs;
                rstPixelType pType = rsP.PixelType;
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
                IRaster crs = returnRaster(compareRaster);
                //Console.WriteLine("Is Raster");
                IRaster minRst = calcArithmaticFunction(rs, crs, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcGreaterFunction(minRst, 0);
            }
            functionModel.estimateStatistics(outRs);
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
        public IRaster calcLessEqualFunction(object inRaster, object compareRaster)
        {
            IRaster rs = returnRaster(inRaster);
            IRaster outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                IRasterProps rsP = (IRasterProps)rs;
                rstPixelType pType = rsP.PixelType;
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
                IRaster crs = returnRaster(compareRaster);
                IRaster minRst = calcArithmaticFunction(rs, crs, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcLessEqualFunction(minRst, 0);
            }
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// Remaps values less than to the compareRaster to 1. Values greater than compareRaster = 0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value or Raserter to compare against</param>
        /// <returns>IRaster</returns>
        public IRaster calcLessFunction(object inRaster, object compareRaster)
        {
            IRaster rs = returnRaster(inRaster);
            IRaster outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                IRasterProps rsP = (IRasterProps)rs;
                rstPixelType pType = rsP.PixelType;
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
                IRaster crs = returnRaster(compareRaster);
                IRaster minRst = calcArithmaticFunction(rs, crs, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcLessFunction(minRst, 0);
            }
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// Remaps values equal to the compare Raster or value cell values = 1. Values less than or greater than compare raster or value = 0
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or IRaster</param>
        /// <param name="vl">value to compare against</param>
        /// <returns>IRaster</returns>
        public IRaster calcEqualFunction(object inRaster, object compareRaster)
        {
            IRaster rs = returnRaster(inRaster);
            IRaster outRs = null;
            if (isNumeric(compareRaster.ToString()))
            {
                double vl = System.Convert.ToDouble(compareRaster);
                IRemapFilter rFilt = new RemapFilterClass();
                IRasterProps rsP = (IRasterProps)rs;
                rstPixelType pType = rsP.PixelType;
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
                IRaster crs = returnRaster(compareRaster);
                IRaster minRst = calcArithmaticFunction(rs, crs, esriRasterArithmeticOperation.esriRasterMinus);
                outRs = calcEqualFunction(minRst, 0);
            }
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// Creates a constant raster given a template raster and a double value
        /// </summary>
        /// <param name="templateRaster">Raster that has the extent and cell size of the desired constant raster</param>
        /// <param name="rasterValue">double value that all cells will have</param>
        /// <returns>a constant raster IRaster</returns>
        public IRaster constantRasterFunction(object templateRaster, double rasterValue)
        {
            IRaster inRaster = returnRaster(templateRaster,rstPixelType.PT_DOUBLE);
            IRasterFunction identFunction = (IRasterFunction)new IdentityFunction();
            identFunction.Bind(inRaster);
            IConstantFunctionArguments rasterFunctionArguments = (IConstantFunctionArguments)new ConstantFunctionArguments();
            rasterFunctionArguments.Constant = rasterValue;
            rasterFunctionArguments.RasterInfo = identFunction.RasterInfo;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ConstantFunction();
            rsFunc.PixelType = rstPixelType.PT_DOUBLE;
            frDset.Init(rsFunc, rasterFunctionArguments);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(rasterValue,outRs);
            return outRs;

        }
        /// <summary>
        /// Used as an if then else statement. The condRaster raster is meant to have values of 1 or 0. If a cell within the input raster has a value 1
        /// then the cell gets the value of inRaster1's corresponding cell. Otherwise that cell gets the value of the inRaster2's corresponding cell.
        /// </summary>
        /// <param name="condRaster">string path, IRaster, IRasterDataset thats cell values are 0 or 1</param>
        /// <param name="inRaster1">string path, IRaster, IRasterDataset, or a numeric value</param>
        /// <param name="inRaster2">string path, IRaster, IRasterDataset, or a numeric value</param>
        /// <returns>IRaster</returns>
        public IRaster conditionalRasterFunction(object condRaster, object trueRaster, object falseRaster)
        {
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            FunctionRasters.conditionalFunctionDataset rsFunc = new FunctionRasters.conditionalFunctionDataset();
            FunctionRasters.conditionalFunctionArguments args = new FunctionRasters.conditionalFunctionArguments(this);
            IRaster conRs = returnRaster(condRaster,rstPixelType.PT_DOUBLE);
            if (conRs==null)
            {
                Console.WriteLine("Condition Raster must be a raster");
                return null;
            }
            IRaster iR1, iR2;
            if (isNumeric(trueRaster.ToString()) && !isNumeric(falseRaster.ToString()))
            {
                iR2 = returnRaster(falseRaster,rstPixelType.PT_DOUBLE);
                iR1 = constantRasterFunction(conRs, System.Convert.ToDouble(trueRaster));
            }
            else if (isNumeric(falseRaster.ToString()) && !isNumeric(trueRaster.ToString()))
            {
                iR1 = returnRaster(trueRaster,rstPixelType.PT_DOUBLE);
                iR2 = constantRasterFunction(conRs, System.Convert.ToDouble(falseRaster));
            }
            else if (isNumeric(falseRaster.ToString()) && isNumeric(trueRaster.ToString()))
            {
                iR1 = constantRasterFunction(conRs, System.Convert.ToDouble(trueRaster));
                iR2 = constantRasterFunction(conRs, System.Convert.ToDouble(falseRaster));
            }
            else
            {
                iR1 = returnRaster(trueRaster, rstPixelType.PT_DOUBLE);
                iR2 = returnRaster(falseRaster,rstPixelType.PT_DOUBLE);
            }
            args.ConditionalRaster = conRs;
            args.TrueRaster = iR1;
            args.FalseRaster = iR2;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(iR1, iR2, outRs, esriRasterArithmeticOperation.esriRasterPlus);
            return outRs;
            
        }
        /// <summary>
        /// LocalStatistics
        /// </summary>
        /// <param name="inRaster">string, IRasterDataset, or Raster</param>
        /// <returns>IRaster</returns>
        public IRaster localStatisticsfunction(object inRaster, rasterUtil.localType op)
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
                case localType.STANDARD_DEVIATION:
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
                default:
                    break;
            }
            FunctionRasters.LocalFunctionArguments args = new FunctionRasters.LocalFunctionArguments(this);
            
            IRaster inRs = returnRaster(inRaster,rstPixelType.PT_DOUBLE);
            args.InRaster = inRs;
            IRaster outRs = null;
            frDset.Init(rsFunc, args);
            outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(inRs, outRs, op);
            return outRs;

        }
        /// <summary>
        /// Clips a raster to the boundary of a polygon
        /// </summary>
        /// <param name="inRaster">IRaster, IRasterDataset, or string</param>
        /// <param name="geo">Polygon Geometry</param>
        /// <param name="clipType">the type of clip either inside or outside</param>
        /// <returns></returns>
        public IRaster clipRasterFunction(object inRaster,IGeometry geo,esriRasterClippingType clipType)
        {
            IRaster rRst = returnRaster(inRaster);
            IRaster2 rRst2 = (IRaster2)rRst;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new ClipFunctionClass();
            rsFunc.PixelType = ((IRasterProps)rRst).PixelType;
            IEnvelope env = geo.Envelope;
            //IRasterProps rsProps = (IRasterProps)rRst;
            IPnt cSize = getCellSize(rRst);
            double hX = cSize.X / 2;
            double hY = cSize.Y / 2;
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
            //IEnvelope rsExtent = rsProps.Extent;
            //rsFunc.RasterInfo.CellSize = cSize;           
            //double w = (((env.XMax - env.XMin) / cSize.X) + 1)*cSize.X;
            //double h = (((env.YMax - env.YMin) / cSize.Y) + 1)*cSize.Y;
            //double lw = (System.Convert.ToInt32((env.XMin - rsExtent.XMin) / cSize.X) - 1)*cSize.X;
            //double lh = (System.Convert.ToInt32((env.YMin - rsExtent.YMin) / cSize.Y) - 1)*cSize.Y;
            //env.XMin = rsExtent.XMin + lw;
            //env.YMin = rsExtent.YMin + lh;
            //env.XMax = env.XMin + w;
            //env.YMax = env.YMin + h;
            //rsFunc.RasterInfo.Extent = env;
            IClipFunctionArguments args = new ClipFunctionArgumentsClass();
            args.ClippingGeometry = geo;
            args.ClippingType = clipType;
            args.Extent = env;
            args.Raster = rRst;
            frDset.Init(rsFunc, args);
            //frDset.Simplify();
            return createRaster((IRasterDataset)frDset);
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
        public IRaster calcAndFunction(object rs1, object rs2)
        {
            IRaster rs3 = calcGreaterEqualFunction(rs1, 1);
            IRaster rs4 = calcGreaterEqualFunction(rs2, 1);
            IRaster rs5 = calcArithmaticFunction(rs3, rs4, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster outRs = calcEqualFunction(rs5,2);
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// calculates a or function
        /// </summary>
        /// <param name="rs1"></param>
        /// <param name="rs2"></param>
        /// <returns></returns>
        public IRaster calcOrFunction(object rs1, object rs2)
        {
            IRaster rs3 = calcGreaterEqualFunction(rs1, 1);
            IRaster rs4 = calcGreaterEqualFunction(rs2, 1);
            IRaster rs5 = calcArithmaticFunction(rs3, rs4, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster outRs = calcGreaterEqualFunction(rs5, 1);
            functionModel.estimateStatistics(outRs);
            return outRs;
        }
        /// <summary>
        /// creates a composite band function
        /// </summary>
        /// <param name="rsArray"></param>
        /// <returns></returns>
        public IRaster compositeBandFunction(IRaster[] rsArray)
        {
            IRasterBandCollection rsBc = new RasterClass();
            foreach(IRaster rs in rsArray)
            {
                rsBc.AppendBands((IRasterBandCollection)rs);
            }
            return (IRaster)rsBc;
        }
        /// <summary>
        /// calculates a slope function
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster calcSlopeFunction(IRaster inRaster)
        {
            IRaster rRst = returnRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new SlopeFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_FLOAT;
            ISlopeFunctionArguments args = new SlopeFunctionArgumentsClass();
            args.DEM = rRst;
            args.ZFactor = 1;
            frDset.Init(rsFunc, args);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(outRs, functionModel.dem.Slope);
            return outRs;
        }
        /// <summary>
        /// calculates an aspect function
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster calcAspectFunction(IRaster inRaster)
        {
            IRaster rRst = returnRaster(inRaster);
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new AspectFunctionClass();
            rsFunc.PixelType = rstPixelType.PT_FLOAT;
            frDset.Init(rsFunc, inRaster);
            IRaster outRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(outRs,functionModel.dem.Aspect);
            return outRs;
        }
        
        /// <summary>
        /// converts an aspect raster to a nortsouth raster
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster calcNorthSouthFunction(IRaster DEM)
        {
            IRaster rs = calcSlopeFunction(DEM);
            IRaster rs2 = calcMathRasterFunction(rs,transType.RADIANS);
            IRaster outRs = calcMathRasterFunction(rs2, transType.COS);
            return outRs;
        }
        /// <summary>
        /// converts an aspect raster to a east west raster 
        /// </summary>
        /// <param name="inRaster"></param>
        /// <returns></returns>
        public IRaster calcEastWestFunction(IRaster DEM)
        {
            IRaster rs = calcSlopeFunction(DEM);
            IRaster rs2 = calcMathRasterFunction(rs, transType.RADIANS);
            IRaster outRs = calcMathRasterFunction(rs2, transType.SIN);
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
            if (fmt == "IMAGINE") fmt = "IMAGINE image";
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
        public IRaster convertToDifFormatFunction(object inRaster, rstPixelType pType)
        {
            IRaster rs = returnRaster(inRaster);
            IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            IRasterProps rsProps = (IRasterProps)rs;
            System.Array noDataArray = (System.Array)rsProps.NoDataValue;
            double[] newNoDataArray = new double[noDataArray.Length];
            double dbNoDtVl = rasterUtil.getNoDataValue(pType);
            IRasterBandCollection fRsBc = new RasterClass();
            IRaster outRs = (IRaster)fRsBc;
            IRasterProps outRsProps = (IRasterProps)outRs;
            //outRsProps.PixelType = pType;
            //outRsProps.SpatialReference = rsProps.SpatialReference;
            outRsProps.Width = rsProps.Width;
            outRsProps.Height = rsProps.Height;
            outRsProps.Extent = rsProps.Extent;
            for (int i = 0; i < noDataArray.Length; i++)
            {
                double noDataVl = System.Convert.ToDouble(noDataArray.GetValue(i));
                IRasterBand rsBand = rsBc.Item(i);
                if (noDataVl != dbNoDtVl)
                {
                    newNoDataArray.SetValue(dbNoDtVl, i);
                    IRaster nRs = getBand(rs, i);
                    IRemapFilter rFlt = new RemapFilterClass();
                    rFlt.AddClass(noDataVl, noDataVl + 0.00001, dbNoDtVl);
                    nRs = calcRemapFunction(nRs, rFlt);
                    rsBand = ((IRasterBandCollection)nRs).Item(0);
                }
                else
                {
                    newNoDataArray.SetValue(noDataVl,i);
                }
                fRsBc.AppendBand(rsBand);
            }
            outRsProps.NoDataValue = newNoDataArray;
            string tempAr = funcDir + "\\" + FuncCnt + ".afr";
            IFunctionRasterDataset frDset = new FunctionRasterDatasetClass();
            IFunctionRasterDatasetName frDsetName = new FunctionRasterDatasetNameClass();
            frDsetName.FullName = tempAr;
            frDset.FullName = (IName)frDsetName;
            IRasterFunction rsFunc = new IdentityFunctionClass();
            rsFunc.PixelType = pType;
            frDset.Init(rsFunc, outRs);
            IRaster fRs = createRaster((IRasterDataset)frDset);
            functionModel.estimateStatistics(fRs, pType);
            return fRs;
        }
        public IRaster setValueRangeToNodata(object inRaster,List<double[]>minMaxList)
        {
            IRaster rs = returnRaster(inRaster);
            IRasterProps rsProps = (IRasterProps)rs;
            double noData = System.Convert.ToDouble(((System.Array)rsProps.NoDataValue).GetValue(0));
            IRemapFilter rFilt = new RemapFilterClass();
            foreach (double[] d in minMaxList)
            {
                rFilt.AddClass(d[0], d[1], noData);
            }
            IRaster rs2 = calcRemapFunction(rs,rFilt);
            if (rsProps.PixelType != rstPixelType.PT_DOUBLE)
            {
                return convertToDifFormatFunction(rs2, rsProps.PixelType);
            }
            else
            {
                return rs;
            }
        }
        /// <summary>
        /// retrieves the appropriate no data value for a given rstPixeltype
        /// </summary>
        /// <param name="pType">type of pixel</param>
        /// <returns></returns>
        public static double getNoDataValue(rstPixelType pType)
        {
            double minVl = -1.79E+308;
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    minVl = -127;
                    break;
                case rstPixelType.PT_FLOAT:
                    minVl = Single.MinValue;
                    break;
                case rstPixelType.PT_LONG:
                    minVl = Int32.MinValue;
                    break;
                case rstPixelType.PT_SHORT:
                    minVl = short.MinValue;
                    break;
                case rstPixelType.PT_U1:
                    minVl = byte.MaxValue;
                    break;
                case rstPixelType.PT_U2:
                    minVl = Math.Pow(2,2)-1;
                    break;
                case rstPixelType.PT_U4:
                    minVl = Math.Pow(2,4)-1;
                    break;
                case rstPixelType.PT_UCHAR:
                    minVl = char.MinValue;
                    break;
                case rstPixelType.PT_ULONG:
                    minVl = UInt32.MaxValue;
                    break;
                case rstPixelType.PT_USHORT:
                    minVl = ushort.MaxValue;
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
        public ITable buildVat(object inRaster)
        {
            IRaster2 rs = (IRaster2)returnRaster(inRaster);
            IRasterProps prop = (IRasterProps)rs;
            rstPixelType rsType = prop.PixelType;
            if (rsType == rstPixelType.PT_FLOAT || rsType == rstPixelType.PT_DOUBLE)
            {
                return null;
            }
            IRasterDataset rsDset = rs.RasterDataset;
            IRasterDatasetEdit2 rsDsetE = (IRasterDatasetEdit2)rsDset;
            if (rs.AttributeTable != null)
            {
                rsDsetE.DeleteAttributeTable();
            }
            rsDsetE.BuildAttributeTable();
            rs = (IRaster2)((IRasterDataset2)rsDset).CreateFullRaster();
            return rs.AttributeTable;
        }
        /// <summary>
        /// defines unique regions using a 4 neighbor window
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="wks"></param>
        /// <param name="outName"></param>
        /// <returns></returns>
        public IRaster regionGroup(object inRaster, IWorkspace wks, string outName)
        {
            FunctionRasters.NeighborhoodHelper.regionGroup rg = new FunctionRasters.NeighborhoodHelper.regionGroup();
            rg.InRaster = inRaster;
            rg.OutWorkspace = wks;
            rg.OutRasterName = outName;
            rg.PixelBlockHeight = 512;
            rg.PixelBlockWidth = 512;
            rg.executeRegionGroup();
            return rg.OutRaster;
            //#region create new raster
            //IRaster inRs = (IRaster)returnRaster(inRaster);
            //IRasterProps rsProp = (IRasterProps)inRs;
            //if (rsProp.PixelType == rstPixelType.PT_DOUBLE || rsProp.PixelType == rstPixelType.PT_FLOAT)
            //{
            //    inRs = convertToDifFormatFunction(inRs, rstPixelType.PT_LONG);
                
            //    rsProp = (IRasterProps)inRs;
            //}
            //IWorkspace vWks = geoUtil.OpenWorkSpace(wks.PathName);
            //System.Array noDataValues = (System.Array)rsProp.NoDataValue;
            //int noDataVl = System.Convert.ToInt32(noDataValues.GetValue(0));
            //outName = getSafeOutputName(wks, outName);
            //IRaster regRs = createNewRaster(inRs, wks, outName, 1, rstPixelType.PT_LONG);
            //#endregion
            //#region set unique raster values
            //IRasterProps rsProps2 = (IRasterProps)regRs;
            //System.Array noDataValues2 = (System.Array)rsProps2.NoDataValue;
            //int noDataVl2 = System.Convert.ToInt32(noDataValues2.GetValue(0));
            ////Console.WriteLine("noDataVl2 = " + noDataVl2.ToString());
            ////Console.WriteLine("noDataVl = " + noDataVl.ToString());
            //int rws = rsProp.Height;
            //int clms = rsProp.Width;
            //#region create VAT Table
            //IFields flds = new FieldsClass();
            //IField fld = new FieldClass();
            //IFieldsEdit fldsE = (IFieldsEdit)flds;
            //IFieldEdit fldE = (IFieldEdit)fld;
            //fldE.Name_2 = "Value";
            //fldE.Type_2 = esriFieldType.esriFieldTypeInteger;
            //fldE.Precision_2 = 50;
            //fldsE.AddField(fld);
            //IField fld2 = new FieldClass();
            //IFieldEdit fld2E = (IFieldEdit)fld2;
            //fld2E.Name_2 = "Count";
            //fld2E.Type_2 = esriFieldType.esriFieldTypeInteger;
            //fldE.Precision_2 = 50;
            //fldsE.AddField(fld2);
            //IField fld3 = new FieldClass();
            //IFieldEdit fld3E = (IFieldEdit)fld3;
            //fld3E.Name_2 = "Perimeter";
            //fld3E.Type_2 = esriFieldType.esriFieldTypeInteger;
            //fld3E.Precision_2 = 50;
            //fldsE.AddField(fld3);
            //ITable vatTable = geoUtil.createTable(vWks, outName + "_vat", flds);
            //#endregion
            //IWorkspaceEdit wksE = (IWorkspaceEdit)vWks;
            //bool weEdit = true;
            //if (wksE.IsBeingEdited())
            //{
            //    weEdit = false;
            //}
            //else
            //{
            //    wksE.StartEditing(false);
            //}
            //wksE.StartEditOperation();
            //int counter = Int32.MinValue;
            //if (noDataVl2 >= 0)
            //{

            //    counter = Int32.MinValue + 2;
            //}
            //else
            //{
            //    counter = Int32.MinValue + 2;
            //}
            ////Console.WriteLine("Counter = " + counter.ToString());
            //IPnt loc = new PntClass();
            //try
            //{
            //    for (int rp = 0; rp < rws; rp += 511)//rws
            //    {
            //        for (int cp = 0; cp < clms; cp += 511)//clms
            //        {
            //            Console.WriteLine("Column Row = " + cp.ToString() + ":" + rp.ToString());
            //            Console.WriteLine("nodata = " + noDataVl.ToString());
            //            Console.WriteLine("nodata2 = " + noDataVl2.ToString());
            //            loc.SetCoords(cp, rp);
                        
            //            middleRowColumn(inRs, regRs, vatTable, loc, counter, noDataVl, noDataVl2, 1, 1,0,0);
            //        }
            //    }
            //    #endregion
            //    #region update vat
            //    wksE.StopEditOperation();
            //    if (weEdit) wksE.StopEditing(true);
            //    //wksE.StartEditing(false);
            //    //wksE.StartEditOperation();
            //    //Console.WriteLine("Updating Vat Table");
            //    //updateKeyTable(ref keysTable, ref remapRegionTable);
            //    //wksE.StopEditOperation();
            //    //if (weEdit) wksE.StopEditing(true);
            //    //IRaster2 rs2 = (IRaster2)regRs;
            //    //IRasterDataset rsDset = rs2.RasterDataset;
            //    //IRasterDatasetEdit2 rsDsetE = (IRasterDatasetEdit2)rsDset;
            //    //rsDsetE.AlterAttributeTable(keysTable);
            //    //wksE.StopEditOperation();
            //    //wksE.StopEditing(true);
            //    #endregion
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            //finally
            //{
            //}
            //return regRs;
        }

        

        private void middleRowColumn(IRaster inRs, IRaster regRs, ITable vatTable, IPnt loc, int counter, int noDataVl, int noDataVl2, int startRow, int startColumn, int rCnt, int rPerm)
        {
            IRasterEdit regRsE = (IRasterEdit)regRs;
            IPnt pnt = new PntClass();
            pnt.SetCoords(512, 512);
            IPixelBlock pb = inRs.CreatePixelBlock(pnt);
            IPixelBlock pb2 = regRs.CreatePixelBlock(pnt);
            inRs.Read(loc, pb);
            regRs.Read(loc, pb2);
            System.Array inArr = (System.Array)pb.get_SafeArray(0);
            System.Array outArr = (System.Array)pb2.get_SafeArray(0);
            int height = pb.Height;
            int width = pb.Width;
            int valueIndex = vatTable.FindField("Value");
            int countIndex = vatTable.FindField("Count");
            int permIndex = vatTable.FindField("Perimeter");
            for (int c = startRow; c < width; c++)
            {
                for (int r = startColumn; r < height; r++)
                {
                    List<string> cr = new List<string>();
                    cr.Add(c.ToString() + ":" + r.ToString());
                    //Console.WriteLine(cr[0]);
                    int inVl = System.Convert.ToInt32(inArr.GetValue(c, r));
                    if (inVl == noDataVl)
                    {
                        continue;
                    }
                    else
                    {
                        int outVl32 = System.Convert.ToInt32(outArr.GetValue(c, r));
                        if (startRow != 1 || startColumn != 1)
                        {
                            string[] nextArray = { "", "", "", "" };//determines if the next pixel block must be queried {left,top,right,bottom}
                            while (cr.Count > 0)
                            {
                                rCnt++;
                                //Console.WriteLine(cr.Count);
                                string[] crArr = cr[0].Split(new char[] { ':' });
                                int cl = System.Convert.ToInt32(crArr[0]);
                                int rw = System.Convert.ToInt32(crArr[1]);
                                rPerm += findRegion(inVl, counter, noDataVl2, cl, rw, inArr, outArr, cr, nextArray);
                            }
                            Console.WriteLine("Cells area = " + rCnt.ToString());
                            Console.WriteLine("Cells perm = " + rPerm.ToString());
                            pb2.set_SafeArray(0, (System.Array)outArr);
                            regRsE.Write(loc, pb2);
                            for (int i = 0; i < nextArray.Length; i++)
                            {
                                string s = nextArray[i];
                                if (s != "")
                                {
                                    Console.WriteLine("previous location  = " + s);
                                    string[] crArr = s.Split(new char[] { ':' });
                                    int cl = System.Convert.ToInt32(crArr[0]);
                                    int rw = System.Convert.ToInt32(crArr[1]);
                                    IPnt newLoc = new PntClass();
                                    double nClP = loc.X;
                                    double nRwP = loc.Y;
                                    IRasterProps rsProps = (IRasterProps)inRs;
                                    switch (i)
                                    {
                                        case 0:
                                            nClP = nClP - 511;
                                            cl = 511;
                                            break;
                                        case 1:
                                            nRwP = nRwP - 511;
                                            rw = 511;
                                            break;
                                        case 2:
                                            nClP = nClP + 511;
                                            cl = 0;
                                            break;
                                        default:
                                            nRwP = nRwP + 511;
                                            rw = 0;
                                            break;
                                    }
                                    if ((nClP >= 0 && nRwP >= 0 & nClP <= rsProps.Width && nRwP <= rsProps.Height))
                                    {
                                        Console.WriteLine("new location = " + nClP.ToString() + ":" + nRwP.ToString());
                                        pb2.set_SafeArray(0, (System.Array)outArr);
                                        regRsE.Write(loc, pb2);
                                        newLoc.SetCoords(nClP, nRwP);
                                        middleRowColumn(inRs, regRs, vatTable, newLoc, counter, noDataVl, noDataVl2, rw, cl, rCnt, rPerm);
                                    }
                                }
                            }
                            return;
                        }
                        else if (outVl32 == noDataVl2)
                        {
                            rCnt = 0;
                            rPerm = 0;
                            outArr.SetValue(counter, c, r);
                            string[] nextArray = { "", "", "", "" };//determines if the next pixel block must be queried {left,top,right,bottom}
                            while (cr.Count > 0)
                            {
                                rCnt++;
                                string[] crArr = cr[0].Split(new char[] { ':' });
                                int cl = System.Convert.ToInt32(crArr[0]);
                                int rw = System.Convert.ToInt32(crArr[1]);
                                rPerm += findRegion(inVl, counter, noDataVl2, cl, rw, inArr, outArr, cr, nextArray);
                            }
                            pb2.set_SafeArray(0, (System.Array)outArr);
                            regRsE.Write(loc, pb2);
                            for (int i = 0; i < nextArray.Length; i++)
                            {
                                string s = nextArray[i];
                                if (s != "")
                                {
                                    Console.WriteLine("previous location  = " + s);
                                    string[] crArr = s.Split(new char[] { ':' });
                                    int cl = System.Convert.ToInt32(crArr[0]);
                                    int rw = System.Convert.ToInt32(crArr[1]);
                                    IPnt newLoc = new PntClass();
                                    double nClP = loc.X;
                                    double nRwP = loc.Y;
                                    IRasterProps rsProps = (IRasterProps)inRs;
                                    switch (i)
                                    {
                                        case 0:
                                            nClP = nClP - 511;
                                            cl = 511;
                                            break;
                                        case 1:
                                            nRwP = nRwP - 511;
                                            rw = 511;
                                            break;
                                        case 2:
                                            nClP = nClP + 511;
                                            cl = 0;
                                            break;
                                        default:
                                            nRwP = nRwP + 511;
                                            rw = 0;
                                            break;
                                    }
                                    if ((nClP >= 0 && nRwP >= 0 & nClP <= rsProps.Width && nRwP <= rsProps.Height))
                                    {
                                        Console.WriteLine("new location = " + nClP.ToString() + ":" + nRwP.ToString());
                                        pb2.set_SafeArray(0, (System.Array)outArr);
                                        regRsE.Write(loc, pb2);
                                        newLoc.SetCoords(nClP, nRwP);
                                        Console.WriteLine("calling middleRow for pbStartLoc (C:R) = " + nClP.ToString() + ":" + nRwP.ToString() + " start locations = " + cl.ToString() + ":" + rw.ToString());
                                        middleRowColumn(inRs, regRs, vatTable, newLoc, counter, noDataVl, noDataVl2, rw, cl, rCnt, rPerm);
                                    }
                                }
                            }
                            regRs.Read(loc, pb2);
                            outArr = (System.Array)pb2.get_SafeArray(0);
                            IRow row = vatTable.CreateRow();
                            row.set_Value(valueIndex, counter);
                            row.set_Value(countIndex, rCnt);
                            row.set_Value(permIndex, rPerm);
                            row.Store();
                            counter++;
                        }
                        else
                        {
                        }
                    }
                }
            }
            pb2.set_SafeArray(0, (System.Array)outArr);
            regRsE.Write(loc, pb2);
        }
        private int findRegion(int inValue, int newValue, int noDataValue, int clm, int rw, System.Array inArr, System.Array outArr, List<string> columnrow, string[] nextArray)
        {
            int permEdges = 0;
            string ccr = clm.ToString() + ":" + rw.ToString();
            if (columnrow == null)
            {
                columnrow = new List<string>();
                columnrow.Add(ccr);
            }
            int maxCR = 511;
            int minCR = 0;
            for (int i = 0; i < 4; i++)
            {
                int cPlus = clm;
                int rPlus = rw;
                switch (i)
                {
                    case 0:
                        cPlus += 1;
                        break;
                    case 1:
                        rPlus += 1;
                        break;
                    case 2:
                        cPlus -= 1;
                        break;
                    default:
                        rPlus -= 1;
                        break;
                }
                if (cPlus > maxCR || rPlus > maxCR || cPlus < minCR || rPlus < minCR)
                {
                    continue;
                }
                try
                {
                    string cr = cPlus.ToString() + ":" + rPlus.ToString();
                    int cVl = System.Convert.ToInt32(outArr.GetValue(cPlus, rPlus));
                    int pVl = System.Convert.ToInt32(inArr.GetValue(cPlus, rPlus));
                    if (cVl == noDataValue)
                    {

                        if (pVl == inValue)
                        {
                            try
                            {
                                outArr.SetValue(newValue, cPlus, rPlus);
                                if (!columnrow.Contains(cr))
                                {
                                    columnrow.Add(cr);
                                }
                                List<int> nPbLst = new List<int>();
                                bool rPlusMinCheck = (rPlus == minCR);
                                bool rPlusMaxCheck = (rPlus == maxCR);
                                bool cPlusMinCheck = (cPlus == minCR);
                                bool cPlusMaxCheck = (cPlus == maxCR);
                                if (rPlusMinCheck || rPlusMaxCheck || cPlusMinCheck || cPlusMaxCheck)
                                {
                                    string nArrVl = cPlus.ToString() + ":" + rPlus.ToString();
                                    if (rPlusMinCheck)
                                    {
                                        rPlus = maxCR;
                                        //nPbLst.Add(0);
                                        nextArray[0] = nArrVl;
                                    }
                                    if (rPlusMaxCheck)
                                    {
                                        rPlus = minCR;
                                        //nPbLst.Add(1);
                                        nextArray[1] = nArrVl;
                                    }
                                    if (cPlusMinCheck)
                                    {
                                        cPlus = maxCR;
                                        //nPbLst.Add(2);
                                        nextArray[2] = nArrVl;
                                    }
                                    if(cPlusMaxCheck)
                                    {
                                        cPlus = minCR;
                                        //nPbLst.Add(3);
                                        nextArray[3] = nArrVl;
                                    }                                    
                                }
                                //foreach (int j in nPbLst)
                                //{
                                //    nextArray[j] = cPlus.ToString() + ":" + rPlus.ToString();
                                //}
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            permEdges++;
                        }
                    }
                    else
                    {
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            try
            {
                columnrow.Remove(ccr);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                columnrow.Clear();
            }
            return permEdges;

        }

        /// <summary>
        /// eliminates slivers within a raster
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="minCells"></param>
        /// <param name="maxCells"></param>
        /// <returns></returns>
        public IRaster eliminateSlivers(object inRaster, int minCells, int maxCells)
        {
            IRaster rs = returnRaster(inRaster);
            IRasterProps rsProps = (IRasterProps)rs;
            if (rsProps.PixelType == rstPixelType.PT_FLOAT || rsProps.PixelType == rstPixelType.PT_DOUBLE)
            {
                return rs;// = convertToDifFormatFunction(rs,rstPixelType.PT_LONG);
            }
            IRaster2 rs2 = (IRaster2)rs;
            IRasterEdit rsE = (IRasterEdit)rs;
            Dictionary<int, int> idsL = new Dictionary<int, int>();
            Dictionary<int, int> idsG = new Dictionary<int, int>();
            getRasterCellCounts(rs, minCells, out idsL, out idsG);
            IPnt pnt = new PntClass();
            pnt.SetCoords(512,512);
            IRasterCursor rsCur = rs2.CreateCursorEx(pnt);
            bool x = true;
            while (x)
            {
                IPixelBlock3 pb = (IPixelBlock3)rsCur.PixelBlock;
                int ht = pb.Height;
                int wd = pb.Width;
                System.Array inArr = (System.Array)pb.get_PixelData(0);
                for (int c = 0; c < wd; c++)
                {
                    for (int r = 0; r < ht; r++)
                    {
                        int inVl = System.Convert.ToInt32(inArr.GetValue(c, r));
                        int maxCnt = 0;
                        int fmaxCnt = 0;
                        int maxVl = inVl;
                        int fmaxVl = inVl;
                        if (idsL.TryGetValue(inVl, out maxCnt))
                        {
                            //Console.WriteLine(inVl.ToString() + " cnt " + maxCnt.ToString());
                            int cp1 = c+1;
                            int cp2 = c-1;
                            int rp1 = r+1;
                            int rp2 = r-1;
                            if (rp1==ht) rp1 = r;
                            if (rp2<0) rp2 = r;
                            if (cp1==wd) cp1 = c;
                            if (cp2<0) cp2 = c;
                            int inR = System.Convert.ToInt32(inArr.GetValue(cp1, r));
                            
                            int inB = System.Convert.ToInt32(inArr.GetValue(c, rp1));
                            
                            int inT = System.Convert.ToInt32(inArr.GetValue(c, rp2));
                            
                            int inL = System.Convert.ToInt32(inArr.GetValue(cp2, r));
                            if (inB==inR&&inB==inT&&inB==inL)
                            {
                                if (idsG.ContainsKey(inB))
                                {
                                    maxVl = inB;
                                }
                            }
                            else
                            {
                                int[] vlArr = { inR, inB, inT, inL };
                                foreach (int vlIter in vlArr)
                                {
                                    int itCnt = 0;
                                    if (idsG.TryGetValue(vlIter, out itCnt))
                                    {
                                        if (itCnt > maxCnt)
                                        {
                                            fmaxCnt = itCnt;
                                            fmaxVl = vlIter;
                                            if (itCnt < maxCells)
                                            {
                                                maxCnt = itCnt;
                                                maxVl = vlIter;
                                            }
                                        }
                                    }
                                }
                                if (maxVl == inVl)
                                {
                                    maxVl = fmaxVl;
                                }
                            }
                            if (maxVl == inVl)
                            {
                                maxVl = getNextNeighbor(inVl,c,r,cp1,cp2,rp1,rp2,ref idsG,inArr, maxCells);
                            }
                            inArr.SetValue(maxVl, c, r);
                            //Console.WriteLine("seeting c, r, value " +c.ToString()+", "+r.ToString()+", "+inVl.ToString() + " to " + maxVl.ToString());
                        }
                        else
                        {
                        }
                        
                    }
                }
                pb.set_PixelData(0, inArr);
                rsE.Write(rsCur.TopLeft, (IPixelBlock)pb);
                x = rsCur.Next();
            }
            rsE.Refresh();
            return rs;
        }

        private int getNextNeighbor(int inVl, int c, int r, int cpa, int cpb, int rpa, int rpb, ref Dictionary<int,int> idsG, System.Array inArr, int maxCells)
        {
            int ht = inArr.GetUpperBound(1) + 1;
            int wd = inArr.GetUpperBound(0) + 1;
            int maxCnt = 0;
            int maxVl = inVl;
            int fmaxCnt = 0;
            int fmaxVl = inVl;
            int cp1 = cpa + 1;
            int cp2 = cpb - 1;
            int rp1 = rpa + 1;
            int rp2 = rpb - 1;
            if (rp1 == ht) rp1 = r;
            if (rp2 < 0) rp2 = r;
            if (cp1 == wd) cp1 = c;
            if (cp2 < 0) cp2 = c;
            if (cp1 == wd & cp2==0 && rp1 == ht&&rp2==0)
            {
                return inVl;
            }
            int inR = System.Convert.ToInt32(inArr.GetValue(cp1, r));

            int inB = System.Convert.ToInt32(inArr.GetValue(c, rp1));

            int inT = System.Convert.ToInt32(inArr.GetValue(c, rp2));

            int inL = System.Convert.ToInt32(inArr.GetValue(cp2, r));
            if (inB == inR && inB == inT && inB == inL)
            {
                if (idsG.ContainsKey(inB))
                {
                    maxVl = inB;
                }
            }
            else
            {
                int[] vlArr = { inR, inB, inT, inL };
                foreach (int vlIter in vlArr)
                {
                    int itCnt = 0;
                    if (idsG.TryGetValue(vlIter, out itCnt))
                    {

                        if (itCnt > maxCnt)
                        {
                            fmaxCnt = itCnt;
                            fmaxVl = vlIter;
                            if (itCnt < maxCells)
                            {
                                maxCnt = itCnt;
                                maxVl = vlIter;
                            }
                        }
                    }
                }
                if (maxVl == inVl)
                {
                    maxVl = fmaxVl;
                }
            }
            if (maxVl == inVl)
            {
                maxVl = getNextNeighbor(inVl, c, r, cp1, cp2, rp1, rp2, ref idsG, inArr,maxCells);
            }
            return maxVl;
        }

        private void getRasterCellCounts(IRaster rs, int minCells, out Dictionary<int, int> idsL, out Dictionary<int, int> idsG)
        {
            Dictionary<int,int> ids = new Dictionary<int, int>();
            IRasterProps rsp = (IRasterProps)rs;
            int noData = System.Convert.ToInt32(((System.Array)rsp.NoDataValue).GetValue(0));
            idsL = new Dictionary<int, int>();
            idsG = new Dictionary<int, int>();
            IPnt pnt = new PntClass();
            pnt.SetCoords(512, 512);
            //create dictionary
            IRasterCursor rsCur = ((IRaster2)rs).CreateCursorEx(pnt);
            bool x = true;
            while (x)
            {
                IPixelBlock pb = rsCur.PixelBlock;
                System.Array inArr = (System.Array)pb.get_SafeArray(0);
                int ht = pb.Height;
                int wd = pb.Width;
                for (int c = 0; c < wd; c++)
                {
                    for (int r = 0; r < ht; r++)
                    {
                        int vl = System.Convert.ToInt32(inArr.GetValue(c, r));
                        if (vl == noData)
                        {
                            continue;
                        }
                        int cnt = 0;
                        if (ids.TryGetValue(vl, out cnt))
                        {
                            cnt += 1;
                            ids[vl] = cnt;
                        }
                        else
                        {
                            ids.Add(vl, 1);
                        }
                    }
                }
                x = rsCur.Next();
            }
            //seperate dicitonary to greaterthan and less than = 
            foreach (KeyValuePair<int, int> kvp in ids)
            {
                int vl = kvp.Key;
                int cnt = kvp.Value;
                if (cnt >= minCells)
                {
                    idsG.Add(vl, cnt);
                }
                else
                {
                    idsL.Add(vl, cnt);
                }
            }

        }
        /// <summary>
        /// splits region into multiple regions
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="minCells"></param>
        /// <param name="maxCells"></param>
        /// <returns></returns>
        public IRaster splitRegions(object inRaster, int minCells, int maxCells)
        {
            IRaster rs = returnRaster(inRaster);
            IRasterProps rsProps = (IRasterProps)rs;
            if (rsProps.PixelType == rstPixelType.PT_FLOAT || rsProps.PixelType == rstPixelType.PT_DOUBLE)
            {
                return rs;// = convertToDifFormatFunction(rs, rstPixelType.PT_LONG);
            }
            IRasterEdit rsE = (IRasterEdit)rs;
            Dictionary<int, IEnvelope> idsG;
            int mV = 0;
            getRasterRegionCells(rs, maxCells, out idsG, out mV);
            Console.WriteLine("splitting " + idsG.Count.ToString() + " regions");
            int counter = mV+1;
            foreach (KeyValuePair<int,IEnvelope> kvp in idsG)
            {
                int vId = kvp.Key;
                IEnvelope env = kvp.Value;
                double cells = env.ZMax;
                if (cells < 135)
                {
                    Console.WriteLine("Splitting " + vId.ToString() + " number of cells = " + env.ZMax.ToString());
                }
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(env.Width,env.Height);
                IPixelBlock pb = rs.CreatePixelBlock(pbSize);
                IPnt pbLoc = new PntClass();
                double l = env.UpperLeft.X;
                double t = env.UpperLeft.Y;
                pbLoc.SetCoords(l,t);
                //Console.WriteLine("Width and Height = " + env.Width.ToString() + " " + env.Height.ToString());
                //Console.WriteLine("Start locations = " + l.ToString() + " " + t.ToString());
                rs.Read(pbLoc, pb);
                System.Array inArr = (System.Array)pb.get_SafeArray(0);
                getNewRegions(vId, maxCells, minCells, env, ref inArr, ref counter);
                pb.set_SafeArray(0, inArr);
                rsE.Write(pbLoc, pb);
            }
            

            return rs;
        }

        private void getNewRegions(int vId, int maxCells, int minCells, IEnvelope env, ref System.Array inArr, ref int counter)
        {
            double cellCount = env.ZMax;
            int leftover = System.Convert.ToInt32(cellCount - maxCells);
            if (leftover <= minCells)
            {
                return;
            }
            int wd = System.Convert.ToInt32(env.Width);
            int ht = System.Convert.ToInt32(env.Height);
            for (int c = 0; c < wd; c++)
            {
                for (int r = 0; r < ht; r++)
                {
                    int inVl = System.Convert.ToInt32(inArr.GetValue(c, r));
                    if (vId == inVl)
                    {
                        if (leftover <= minCells)
                        {
                            //Console.WriteLine("Can't split values leftover too small");
                            inArr.SetValue(counter, c, r);
                        }
                        else if (leftover > maxCells)
                        {
                            //Console.WriteLine("Trying to get new region");
                            inArr.SetValue(counter, c, r);
                            int numCells = 1;
                            List<IPnt> pntLst = new List<IPnt>();
                            IPnt pnt = new PntClass();
                            pnt.SetCoords(c,r);
                            pntLst.Add(pnt);
                            while (numCells < maxCells)
                            {
                                List<IPnt> pntLst2 = new List<IPnt>();
                                foreach (IPnt p in pntLst)
                                {
                                    int nc = System.Convert.ToInt32(p.X);
                                    int nr = System.Convert.ToInt32(p.Y);
                                    pntLst2.AddRange(getNewRegion(vId, minCells, maxCells, nc, nr, wd, ht, ref inArr, counter, ref numCells));
                                    if (numCells >= maxCells)
                                    {
                                        break;
                                    }
                                }
                                pntLst = pntLst2;
                                if (pntLst.Count < 1)
                                {
                                    break;
                                }
                            }
                            counter++;
                            leftover = leftover - numCells;
                        }
                        else
                        {
                        }
                    }
                }
            } 
        }

        private List<IPnt> getNewRegion(int vId, int minCells, int maxCells, int c, int r, int width, int height, ref System.Array inArr, int counter, ref int numCells)
        {
            List<IPnt> pntLst = new List<IPnt>();
            int cp = c + 1;
            int cm = c - 1;
            int rp = r + 1;
            int rm = r - 1;
            if (cp >= width) cp = c;
            if (cm < 0) cm = 0;
            if (rp >= height) rp = r;
            if(rm<0) rm=0;
            List<int[]> crLst = new List<int[]>();
            int[] cr11 = { cp, r };
            int[] cr01 = { c, rp };
            int[] cr10 = { cm, r };
            int[] cr00 = { c, rm };
            crLst.Add(cr11);
            crLst.Add(cr01);
            crLst.Add(cr10);
            crLst.Add(cr00);
            foreach (int[] cr in crLst)
            {
                int nc = cr[0];
                int nr = cr[1];
                if (nc == c && nr == r)
                {
                    continue;
                }
                int cellVl = System.Convert.ToInt32(inArr.GetValue(nc, nr));
                //Console.WriteLine(nc.ToString() + nr.ToString());
                if (cellVl == vId)
                {
                    IPnt pnt = new PntClass();
                    pnt.SetCoords(nc, nr);
                    inArr.SetValue(counter, nc, nr);
                    pntLst.Add(pnt);
                    numCells= numCells+1;
                    if (numCells >= maxCells)
                    {
                        pntLst.Clear();
                        break;
                    }
                }
            }
            return pntLst;
        }

        private void getRasterRegionCells(IRaster rs, int minCells, out Dictionary<int, IEnvelope> idsG, out int maxValue)
        {
            Dictionary<int, IEnvelope> ids = new Dictionary<int,IEnvelope>();
            IRasterProps rsp = (IRasterProps)rs;
            int noData = System.Convert.ToInt32(((System.Array)rsp.NoDataValue).GetValue(0));
            idsG = new Dictionary<int,IEnvelope>();
            IPnt pnt = new PntClass();
            pnt.SetCoords(512, 512);
            maxValue = Int32.MinValue;
            //create dictionary
            IRasterCursor rsCur = ((IRaster2)rs).CreateCursorEx(pnt);
            bool x = true;
            while (x)
            {
                IPixelBlock pb = rsCur.PixelBlock;
                IPnt topleft = rsCur.TopLeft;
                double tX = topleft.X;
                double tY = topleft.Y;
                System.Array inArr = (System.Array)pb.get_SafeArray(0);
                int ht = pb.Height;
                int wd = pb.Width;
                for (int c = 0; c < wd; c++)
                {
                    for (int r = 0; r < ht; r++)
                    {
                        int vl = System.Convert.ToInt32(inArr.GetValue(c, r));
                        if (vl == noData)
                        {
                            continue;
                        }
                        if (vl > maxValue) maxValue = vl;
                        IEnvelope env;
                        double minx, miny, maxx, maxy, maxz;
                        if (ids.TryGetValue(vl, out env))
                        {
                            double cx = c + tX;
                            double cy = tY-r;
                            minx = env.XMin;
                            miny = env.YMin;
                            maxx = env.XMax;
                            maxy = env.YMax;
                            maxz = env.ZMax;
                            if(cx<minx)env.XMin=cx;
                            if(cx>maxx)env.XMax=cx;
                            if(cy<miny)env.YMin=cy;
                            if(cy>maxy)env.YMax=cy;
                            env.ZMax = maxz+1;
                            ids[vl]=env;
                        }
                        else
                        {
                            env = new EnvelopeClass();
                            double cminx = c + tX;
                            double cminy = r + tY;
                            env.XMin = cminx;
                            env.XMax = cminx;
                            env.YMin = cminy;
                            env.YMax = cminy;
                            env.ZMax = 1;
                            ids.Add(vl, env);
                        }
                    }
                }
                x = rsCur.Next();
            }
            //seperate dicitonary to greaterthan 
            foreach (KeyValuePair<int, IEnvelope> kvp in ids)
            {
                int vl = kvp.Key;
                IEnvelope env = kvp.Value;
                double cnt = env.ZMax;
                if (cnt >= minCells)
                {
                    idsG.Add(vl, env);
                }
            }

        }
        /// <summary>
        /// performs block summarization
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="outWks"></param>
        /// <param name="outRsName"></param>
        /// <param name="numCells"></param>
        /// <returns></returns>
        public IRaster blockSum(IRaster inRaster, IWorkspace outWks, string outRsName, int numCells)
        {
            IRasterProps rsProps = (IRasterProps)inRaster;
            return blockSum(inRaster, outWks, outRsName, numCells, rsProps.PixelType);
        }
        public IRaster blockSum(IRaster inRaster, IWorkspace outWks, string outRsName, int numCells, rstPixelType pType)
        {
            IPnt meanCellSize = new PntClass();
            IRasterProps rsProps = (IRasterProps)inRaster;
            double noDataVl = System.Convert.ToDouble(((System.Array)rsProps.NoDataValue).GetValue(0));
            IPnt inRasterCellSize = rsProps.MeanCellSize();
            meanCellSize.X = inRasterCellSize.X*numCells;
            meanCellSize.Y = inRasterCellSize.Y*numCells;
            IRaster rs = createNewRaster(rsProps.Extent, meanCellSize, outWks, outRsName, 1, pType, ((IGeoDataset)inRaster).SpatialReference);
            IRasterEdit rsE = (IRasterEdit)rs;
            IRasterProps rsProps2 = (IRasterProps)rs;
            double noDataVl2 = System.Convert.ToDouble(((System.Array)rsProps2.NoDataValue).GetValue(0));
            IPnt pbBlockSizeIn = new PntClass();
            IPnt pbBlockSizeOut = new PntClass();
            int bigXCells = 512;
            int bigYCells = bigXCells;
            int xCells = bigXCells*numCells;
            int yCells = bigYCells*numCells;
            pbBlockSizeIn.SetCoords(xCells,yCells);
            pbBlockSizeOut.SetCoords(bigXCells,bigYCells);
            IRasterCursor rsCurIn = ((IRaster2)inRaster).CreateCursorEx(pbBlockSizeIn);
            IRasterCursor rsCurOut = ((IRaster2)rs).CreateCursorEx(pbBlockSizeOut);
            bool x = true;
            while (x)
            {
                IPixelBlock pbIn = rsCurIn.PixelBlock;
                IPixelBlock pbOut = rsCurOut.PixelBlock;
                System.Array inArr = (System.Array)pbIn.get_SafeArray(0);
                System.Array outArr = (System.Array)pbOut.get_SafeArray(0);
                int inWd = pbOut.Width;
                int inHt = pbOut.Height;
                for (int r = 0; r < inHt; r++)
                {
                    for (int c = 0; c < inWd; c++)
                    {
                        double bVl = FunctionRasters.NeighborhoodHelper.blockHelperValue.getBlockSum(inArr, c, r, numCells,noDataVl);    
                        if (bVl == noDataVl2)
                        {
                            bVl = 0;
                        }
                        object uVl = getSafeValue(bVl, rsProps2.PixelType);
                        outArr.SetValue(uVl, c, r);
                    }
                }
                pbOut.set_SafeArray(0, outArr);
                rsE.Write(rsCurOut.TopLeft, pbOut);
                x = rsCurIn.Next() && rsCurOut.Next();
            }
            calcStatsAndHist(rs);
            return rs;
        }
        /// <summary>
        /// retrieves a safe value for a given a raster pixel type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static object getSafeValue(double value, rstPixelType pType)
        {
            object safeValue = value;
            switch (pType)
            {
                case rstPixelType.PT_CHAR:
                    safeValue = System.Convert.ToSByte(value);
                    break;
                case rstPixelType.PT_CLONG:
                case rstPixelType.PT_COMPLEX:
                case rstPixelType.PT_CSHORT:
                case rstPixelType.PT_DCOMPLEX:
                case rstPixelType.PT_DOUBLE:
                    safeValue = value;
                    break;
                case rstPixelType.PT_FLOAT:
                    safeValue = System.Convert.ToSingle(value);
                    break;
                case rstPixelType.PT_LONG:
                    safeValue = System.Convert.ToInt32(value);
                    break;
                case rstPixelType.PT_SHORT:
                    safeValue = System.Convert.ToInt16(value);
                    break;
                case rstPixelType.PT_U1:
                    safeValue = System.Convert.ToByte(value);
                    break;
                case rstPixelType.PT_U2:
                    safeValue = System.Convert.ToByte(value);
                    break;
                case rstPixelType.PT_U4:
                    safeValue = System.Convert.ToByte(value);
                    break;
                case rstPixelType.PT_UCHAR:
                    safeValue = System.Convert.ToByte(value);
                    break;
                case rstPixelType.PT_ULONG:
                    safeValue = System.Convert.ToUInt32(value);
                    break;
                case rstPixelType.PT_UNKNOWN:
                    safeValue = value;
                    break;
                case rstPixelType.PT_USHORT:
                    safeValue = System.Convert.ToUInt16(value);
                    break;
                default:
                    break;
            }
            return safeValue;
        }
        public IRaster mosaicRastersFunction(IWorkspace wks, string mosaicName, IRaster[] rasters)
        {
            return mosaicRastersFunction(wks, mosaicName, rasters,esriMosaicMethod.esriMosaicNone,rstMosaicOperatorType.MT_FIRST,true, true, true, true);

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
        public static void cleanupTempDirectories()
        {
            string func = System.Environment.GetEnvironmentVariable("temp") + "\\func";
            string mos = System.Environment.GetEnvironmentVariable("temp") + "\\mosaic";
            string[] dirs = {func,mos};
            foreach (string s in dirs)
            {
                try
                {
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(s);
                    dInfo.Delete(true);
                }
                catch
                {
                }
            }
        }

    }
}
