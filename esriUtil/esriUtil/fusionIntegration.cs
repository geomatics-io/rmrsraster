using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using Microsoft.Win32;
using ESRI.ArcGIS.GeoDatabaseExtensions;

namespace esriUtil
{
    public class fusionIntegration
    {
        public fusionIntegration(rasterUtil rasterUtility = null)
        {
            if (rasterUtility == null)
            {
                rsUtil = new rasterUtil();
            }
            else
            {
                rsUtil = rasterUtility;
            }
            getFusionDir();
        }
        private rasterUtil rsUtil = null;
        public string[] MetricsArr = {"row","col","center X","center Y","Total return count","Elev minimum","Elev maximum","Elev mean","Elev mode","Elev stddev","Elev variance","Elev CV","Elev IQ","Elev skewness","Elev kurtosis","Elev AAD","Elev L1","Elev L2","Elev L3","Elev L4","Elev L CV","Elev L skewness","Elev L kurtosis","Elev P01","Elev P05","Elev P10","Elev P20","Elev P25","Elev P30","Elev P40","Elev P50","Elev P60","Elev P70","Elev P75","Elev P80","Elev P90","Elev P95","Elev P99","Return 1 count","Return 2 count","Return 3 count","Return 4 count","Return 5 count","Return 6 count","Return 7 count","Return 8 count","Return 9 count","Other return count","Percentage first returns above HT","Percentage all returns above HT","All returns above HT by Total first returns","First returns above HT","All returns above HT","Percentage first returns above mean","Percentage first returns above mode","Percentage all returns above mean","Percentage all returns above mode","All returns above mean by Total first returns","All returns above mode by Total first returns","First returns above mean","First returns above mode","All returns above mean","All returns above mode","Total first returns","Total all returns","Elev MAD median","Elev MAD mode","Canopy relief ratio","Elev quadratic mean","Elev cubic mean"};
        public string[] CloudArr = { "DataFile", "FileTitle", "Total_return_count", "Total return count above HT","Return_1_count", "Return_2_count", "Return_3_count", "Return_4_count", "Return_5_count", "Return_6_count", "Return_7_count", "Return_8_count", "Return_9_count", "Other_return_count", "Elev_minimum", "Elev_maximum", "Elev_mean", "Elev_mode", "Elev_stddev", "Elev_variance", "Elev_CV", "Elev_IQ", "Elev_skewness", "Elev_kurtosis", "Elev_AAD", "Elev_MAD_median", "Elev_MAD_mode", "Elev_L1", "Elev_L2", "Elev_L3", "Elev_L4", "Elev_L_CV", "Elev_L_skewness", "Elev_L_kurtosis", "Elev_P01", "Elev_P05", "Elev_P10", "Elev_P20", "Elev_P25", "Elev_P30", "Elev_P40", "Elev_P50", "Elev_P60", "Elev_P70", "Elev_P75", "Elev_P80", "Elev_P90", "Elev_P95", "Elev_P99", "Canopy_relief_ratio", "Elev_SQRT_mean_SQ", "Elev_CURT_mean_CUBE", "Int_minimum", "Int_maximum", "Int_mean", "Int_mode", "Int_stddev", "Int_variance", "Int_CV", "Int_IQ", "Int_skewness", "Int_kurtosis", "Int_AAD", "Int_L1", "Int_L2", "Int_L3", "Int_L4", "Int_L_CV", "Int_L_skewness", "Int_L_kurtosis", "Int_P01", "Int_P05", "Int_P10", "Int_P20", "Int_P25", "Int_P30", "Int_P40", "Int_P50", "Int_P60", "Int_P70", "Int_P75", "Int_P80", "Int_P90", "Int_P95", "Int_P99" };
        bool fusionInstalled = true;
        public string fusDir = esriUtil.Properties.Settings.Default.FusionDir;
        private string gridMetrics = "gridmetrics.exe";
        private string cloudMetrics = "cloudmetrics.exe";
        private string csv2Grid = "CSV2Grid.exe";
        private string clipData = "clipdata.exe";
        private string mergeData = "MergeData.exe";
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private int prc = System.Convert.ToInt32(System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
        private void getFusionDir()
        {
            if (fusDir == null || fusDir == "")
            {
                fusDir = @"c:\FUSION";
            }
            if (!System.IO.Directory.Exists(fusDir))
            {
                
                System.Windows.Forms.MessageBox.Show("Can't find Fusion's main directory.Please select Fusion's main directory or download and install Fusion.");
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fusDir = fbd.SelectedPath;
                    esriUtil.Properties.Settings.Default.FusionDir = fusDir;
                    esriUtil.Properties.Settings.Default.Save();
                }
                else
                {
                    fusionInstalled = false;
                }
                
            }
        }
        
        public void RunGridSurfaceCreate(string LasDir, string outDir, int CellSize, string xyunits = "M", string zunits = "M", int coodsys = 0, int zone = 0, int horzDatum=0,int vertDatum=0)
        {
            if (fusionInstalled)
            {
                string[] lasFiles = System.IO.Directory.GetFiles(LasDir, "*.las");
                int cnt = 0;
                string exePath = "\"" + fusDir + "\\gridsurfacecreate.exe\"";
                //Console.WriteLine(exePath);
                //Console.WriteLine(lasFiles.Length.ToString());
                foreach (string s in lasFiles)
                {
                    if (System.IO.Path.GetExtension(s).ToLower() == ".las")
                    {
                        cnt++;
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                        Console.WriteLine("Calculating bare earth DTM for " + fileName);
                        string args = "\"" + outDir + "\\" + fileName + ".dtm\" " + CellSize.ToString() + " " + xyunits.ToString() + " " +zunits.ToString() + " " + coodsys.ToString() + " " + zone.ToString() + " " +horzDatum + " " +vertDatum + " " + "\"" + s + "\"";
                        //Console.WriteLine(args);
                        System.Diagnostics.Process proc = runFusion(exePath, args);
                        if (cnt >= prc)
                        {
                            proc.WaitForExit();
                            cnt = 0;
                        }
                    }
                }
            }
        }
        public string checkAndRenameAllFiles(string LasDir,string ext=".las")
        {
            foreach (string s in System.IO.Directory.GetFiles(LasDir,"*" + ext))
            {
                if(s.ToLower().EndsWith(ext))
                {
                    renameFile(s);
                }

            }
            return LasDir.Replace(" ", "");
        }
        public bool checkPath(string path)
        {
            if (path.Contains(" ")) return true;
            else return false;
        }
        public string renameFile(string inFile)
        {
            string outName = inFile.Replace(" ","");
            string outDir = System.IO.Path.GetDirectoryName(outName);
            if(!System.IO.Directory.Exists(outDir)) System.IO.Directory.CreateDirectory(outDir);
            System.IO.File.Move(inFile, outName);
            return outName;
        }
        public void RunGroundFilter(string LasDir, string outDir, int CellSize)
        {
            if (fusionInstalled)
            {
                string[] lasFiles = System.IO.Directory.GetFiles(LasDir, "*.las");
                int cnt = 0;
                string exePath = "\"" + fusDir + "\\groundfilter.exe\"";
                //Console.WriteLine(exePath);
                //Console.WriteLine(lasFiles.Length.ToString());
                foreach (string s in lasFiles)
                {
                    if (System.IO.Path.GetExtension(s).ToLower() == ".las")
                    {
                        cnt++;
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                        Console.WriteLine("Calculating bare earth DEM for " + fileName);
                        string args = "\"" + outDir + "\\" + fileName + ".las\" " + CellSize.ToString() + " \"" + s + "\"";
                        //Console.WriteLine(args);
                        System.Diagnostics.Process proc = runFusion(exePath, args);
                        if (cnt >= prc)
                        {
                            proc.WaitForExit();
                            cnt = 0;
                        }
                    }
                }
            }
        }
        public void RunCloudMetrics(IFeatureClass sampleLocation, float sampleRadius, string LasDir, string DtmDir="", double cutBelow=0, double cutAbove=150, int shape=0)
        {
            if (fusionInstalled)
            {
                string LasDatasetPath = LasDir + "\\LasFile.lasd";
                string LasFeatureClass = LasDir + "\\LasBoundary.shp";
                ILasDataset lsDset = new LasDatasetClass();
                ((ILasDataset2)lsDset).SaveAs(LasDatasetPath, true);
                ILasDatasetEdit lsDsetE = (ILasDatasetEdit)lsDset;
                IStringArray sArr;
                lsDsetE.AddFolder(LasDir, "las", true, out sArr);
                lsDsetE.Save();
                ISpatialReference sp = lsDset.SpatialReference;
                IFeatureClass extFtrCls = geoUtil.createFeatureClass(LasFeatureClass, null, esriGeometryType.esriGeometryPolygon, sp);
                string lasName = geoUtil.createField(extFtrCls, "LasName", esriFieldType.esriFieldTypeString, false);
                IFeatureCursor ftrCur = extFtrCls.Insert(true);
                IFeatureBuffer ftrBuff = extFtrCls.CreateFeatureBuffer();
                int lasNameIndex = extFtrCls.FindField(lasName);
                for (int i = 0; i < lsDset.FileCount; i++)
                {
                    ILasFile lsFile = lsDset.get_File(i);
                    string lsPath = lsFile.Name;
                    IEnvelope lsEnv = lsFile.Extent;
                    IPolygon poly = new PolygonClass();
                    poly.SpatialReference = sp;
                    IPointCollection pCol = (IPointCollection)poly;
                    pCol.AddPoint(lsEnv.LowerLeft);
                    pCol.AddPoint(lsEnv.UpperLeft);
                    pCol.AddPoint(lsEnv.UpperRight);
                    pCol.AddPoint(lsEnv.LowerRight);
                    poly.Close();
                    ftrBuff.Shape = poly;
                    ftrBuff.set_Value(lasNameIndex, lsPath);
                    ftrCur.InsertFeature(ftrBuff);
                }
                ftrCur.Flush();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
                int[] cloudMetricFieldsIndex = addCloudMetricFields(sampleLocation);
                IFeatureCursor pntCur = sampleLocation.Update(null, true);
                IFeature pntftr = pntCur.NextFeature();
                while (pntftr != null)
                {
                    Console.WriteLine("Working on Plot OID = " + pntftr.OID.ToString());
                    IGeometry geo = pntftr.Shape;
                    IPoint pnt = (IPoint)geo;
                    IEnvelope env = new EnvelopeClass();
                    env.PutCoords(pnt.X - sampleRadius, pnt.Y - sampleRadius, pnt.X + sampleRadius, pnt.Y + sampleRadius);
                    ISpatialFilter sf = new SpatialFilterClass();
                    sf.Geometry = env;
                    sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    IFeatureCursor polyCur = extFtrCls.Search(sf, true);
                    IFeature polyftr = polyCur.NextFeature();
                    List<string> lasFiles = new List<string>();
                    while (polyftr != null)
                    {
                        lasFiles.Add(polyftr.get_Value(lasNameIndex).ToString());
                        polyftr = polyCur.NextFeature();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCur);
                    if (lasFiles.Count > 0)
                    {
                        //Console.WriteLine("Number of las files for point " + pntftr.OID.ToString() + " = " + lasFiles.Count.ToString());
                        object[] metricValues = extractCloudMetrics(env, lasFiles,0,DtmDir,cutBelow,cutAbove,shape);
                        for (int i = 0; i < metricValues.Length; i++)
                        {
                            object vl = metricValues[i];
                            int vlIndex = cloudMetricFieldsIndex[i];
                            if (vlIndex > -1)
                            {
                                pntftr.set_Value(vlIndex, vl);
                            }
                        }
                    }
                    pntCur.UpdateFeature(pntftr);
                    pntftr = pntCur.NextFeature();
                }
            }
        }

        private int[] addCloudMetricFields(IFeatureClass sampleLocation)
        {
            int[] mIndex = new int[CloudArr.Length];
            for (int i = 0; i < CloudArr.Length; i++)
            {
                string mName = CloudArr[i];
                if (sampleLocation.FindField(mName) == -1)
                {
                    if (i < 2)
                    {
                        mName = geoUtil.createField(sampleLocation, mName, esriFieldType.esriFieldTypeString, false);
                    }
                    else
                    {
                        mName = geoUtil.createField(sampleLocation, mName, esriFieldType.esriFieldTypeDouble, false);
                    }
                }
                mIndex[i] = sampleLocation.FindField(mName);
            }
            return mIndex;
        }

        private object[] extractCloudMetrics(IEnvelope env, List<string> lasFiles, double HeightCuttOff = 0, string DtmDir = "", double cutBelow=0, double cutAbove=150, int shape=0)
        {
            string tempDir = System.IO.Path.GetDirectoryName(lasFiles[0]) + "\\temp";
            string flas = tempDir + "\\flas.las";
            string mPath = tempDir + "\\metrics.csv";
            if (!System.IO.Directory.Exists(tempDir)) System.IO.Directory.CreateDirectory(tempDir);
            System.Diagnostics.Process proc = null;
            if(lasFiles.Count>1)
            {
                string clipText = tempDir + "\\cl.txt";
                using(System.IO.StreamWriter sw = new System.IO.StreamWriter(clipText))
                {
                    for (int i = 0; i < lasFiles.Count; i++)
                    {
                        string lsPath = lasFiles[i];
                        string dtmPath = DtmDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(lsPath) + ".dtm";
                        string cf = tempDir + "\\cl" + i.ToString() + ".las";
                        proc = clipLasFile(lsPath, cf, env, dtmPath,cutBelow,cutAbove,shape);
                        proc.WaitForExit();
                        sw.WriteLine(cf);
                    }
                    sw.Close();
                }
                proc = mergeLasFiles(clipText,flas);
                proc.WaitForExit();
            }
            else
            {
                string lsPath = lasFiles[0];
                string dtmPath = DtmDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(lsPath) + ".dtm";  
                proc = clipLasFile(lsPath, flas, env, dtmPath,cutBelow,cutAbove,shape);
                proc.WaitForExit();
            }
            string exePath = "\"" + fusDir + "\\" + cloudMetrics + "\"";
            string args = "/new /minht:" + HeightCuttOff.ToString() + " /outlier:" + cutBelow.ToString() + "," + cutAbove.ToString() + " \"" + flas + "\" \"" + mPath + "\"";
            //Console.WriteLine("Cloud Metrics Args:\n\t" + args);
            proc = runFusion(exePath, args);
            proc.WaitForExit();
            object[] outVls = returnCloudValues(mPath);
            try
            {
                System.IO.DirectoryInfo dinfo = new System.IO.DirectoryInfo(tempDir);
                dinfo.Delete(true);
            }
            catch
            {
            }
            return outVls;

        }

        private object[] returnCloudValues(string mPath)
        {
            string[] vlStrArr = new string[CloudArr.Length];
            object[] vlArr = new object[CloudArr.Length];
            using (System.IO.StreamReader sr = new System.IO.StreamReader(mPath))
            {
                string hd = sr.ReadLine();
                string vls = sr.ReadLine();
                vlStrArr = vls.Split(new char[] { ',' });
                sr.Close();
            }
            for (int i = 0; i < vlArr.Length; i++)
            {
                string vl = vlStrArr[i];
                if (i < 2)
                {
                    vlArr[i] = vl;
                }
                else
                {
                    if (geoUtil.isNumeric(vl))
                    {
                        vlArr[i] = System.Convert.ToDouble(vl);
                    }
                    else
                        vlArr[i] = 0;
                }
            }
            return vlArr;
        }

        private System.Diagnostics.Process mergeLasFiles(string clipText, string flas)
        {
            string exePath = "\"" + fusDir + "\\" + mergeData + "\"";
            string args = "\"" + clipText + "\" \"" + flas + "\"";
            //Console.WriteLine("Merge Args:\n\t" + args);
            System.Diagnostics.Process proc = runFusion(exePath, args);
            return proc;
        }

        private System.Diagnostics.Process clipLasFile(string lsPath, string cf, IEnvelope env, string dtmPath="", double cutMin=0, double cutMax=150, int shape=0)
        {
            string exePath = "\"" + fusDir + "\\" + clipData + "\"";
            string args = "/shape:" + shape.ToString() + " \"" + lsPath + "\" \"" + cf + "\" " + env.XMin.ToString() + " " + env.YMin.ToString() + " " + env.XMax.ToString() + " " + env.YMax.ToString();
            if(dtmPath!=null && System.IO.File.Exists(dtmPath))
            {
                args = "/dtm:\"" + dtmPath + "\" /height /zmin:" + cutMin.ToString() + " /zmax:" + cutMax.ToString() + " " + args;
            }
            //Console.WriteLine("Clip Args:\n\t" + args);
            System.Diagnostics.Process proc = runFusion(exePath, args);
            return proc;
        }
        public void ConvertGridMetricsToRaster(string metricsDir, string outDirectory, string[] metrics)
        {
            int[] m = new int[metrics.Length];
            for (int i = 0; i < metrics.Length; i++)
            {
                string s = metrics[i];
                int vlIndex = System.Array.IndexOf(MetricsArr, s);
                m[i] = vlIndex;
            }
            ConvertGridMetricsToRaster(metricsDir, outDirectory, m);
        }
        public void ConvertGridMetricsToRaster(string metricsDir, string outDir, int[] metrics)
        {
            string[] metricFiles = System.IO.Directory.GetFiles(metricsDir, "*.csv");
            string[] headerFiles = System.IO.Directory.GetFiles(metricsDir, "*.txt");
            IPnt cellSize;
            IPnt[] tlPntArr;
            int[] rwArr;
            int[] clmArr;
            float noDataVl;
            IEnvelope ext = getExtentsFromHeader(headerFiles, out cellSize, out tlPntArr, out rwArr, out clmArr, out noDataVl);
            IWorkspace outWks = geoUtil.OpenRasterWorkspace(outDir);
            string exePath = "\"" + fusDir + "\\" + csv2Grid + "\"";
            IRasterEdit[] rsArr = new IRasterEdit[metrics.Length];
            for (int i = 0; i < metrics.Length; i++)
            {
                int clm = metrics[i];
                string metricName = MetricsArr[clm];
                IRasterDataset rsDset = rsUtil.createNewRaster(ext, cellSize, outWks, metricName, 1, rstPixelType.PT_FLOAT, rasterUtil.rasterType.IMAGINE, null);//rsWk2.CreateRasterDataset(metricName+".img","IMAGINE Image",ext.LowerLeft,w,h,cellSize.X,cellSize.Y,1,rstPixelType.PT_FLOAT,null,true);
                IRasterBand rsB = ((IRasterBandCollection)rsDset).Item(0);
                IRasterProps rsP = (IRasterProps)rsB;
                rsP.NoDataValue = noDataVl;
                IRaster rs = ((IRasterDataset2)rsDset).CreateFullRaster();
                rsArr[i] = (IRasterEdit)rs;

            }
            foreach (string s in metricFiles)
            {
                Console.WriteLine("Working on " + s);
                int pntIndex = System.Array.IndexOf(metricFiles, s);
                IPnt tl = tlPntArr[pntIndex];
                int cl,rw;
                ((IRaster2)rsArr[0]).MapToPixel(tl.X, tl.Y, out cl, out rw);
                IPnt tlp = new PntClass();
                tlp.X = cl;
                tlp.Y = rw;
                int rws = rwArr[pntIndex];
                int clms = clmArr[pntIndex];
                IPnt bSize = new PntClass();
                bSize.SetCoords(clms, rws);
                System.Array[] pbSafeArr = new System.Array[metrics.Length];
                IPixelBlock3[] pbArr = new IPixelBlock3[metrics.Length];
                for (int i = 0; i < metrics.Length; i++)
                {
                    IRasterEdit rsE = rsArr[i];
                    IRaster rs = (IRaster)rsE;
                    IPixelBlock3 pb = (IPixelBlock3)rs.CreatePixelBlock(bSize);
                    rs.Read(tlp, (IPixelBlock)pb);
                    //pb.Mask(0);
                    pbArr[i] = pb;
                    System.Array vlArr = (System.Array)pb.get_PixelData(0);
                    pbSafeArr[i] = vlArr;
                }
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    string hdr = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string ln = sr.ReadLine();
                        string[] lnArr = ln.Split(new char[] { ',' });
                        int r = (rws-1) - System.Convert.ToInt32(lnArr[0]);
                        int c = System.Convert.ToInt32(lnArr[1]);
                        for (int i = 0; i < metrics.Length; i++)
                        {
                            int clIndex = metrics[i];
                            float clVl = System.Convert.ToSingle(lnArr[clIndex]);
                            if (clVl != noDataVl)
                            {
                                pbSafeArr[i].SetValue(clVl, c, r);
                            }
                        }
                    }
                    sr.Close();
                }
                for (int i = 0; i < metrics.Length; i++)
                {
                    IPixelBlock3 pb = pbArr[i];
                    pb.set_PixelData(0, pbSafeArr[i]);
                    rsArr[i].Write(tlp, (IPixelBlock)pb);
                }
            }
            for (int i = 0; i < rsArr.Length; i++)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rsArr[i]);
            }
            
        }

        private IEnvelope getExtentsFromHeader(string[] headerFiles, out IPnt cellSize, out IPnt[] tlPntArr, out int[] rwArr, out int[] clmArr, out float noDataVl)
        {
            double xmin =0, xmax=0, ymin=0, ymax=0;
            tlPntArr = new IPnt[headerFiles.Length];
            rwArr = new int[headerFiles.Length];
            clmArr = new int[headerFiles.Length];
            using (System.IO.StreamReader sr = new System.IO.StreamReader(headerFiles[0]))
            {
                int clms = System.Convert.ToInt32(sr.ReadLine().Split(new char[]{' '})[1]);
                int rws = System.Convert.ToInt32(sr.ReadLine().Split(new char[]{' '})[1]);
                clmArr[0] = clms;
                rwArr[0] = rws;
                double x = System.Convert.ToDouble(sr.ReadLine().Split(new char[]{' '})[1]);
                double y = System.Convert.ToDouble(sr.ReadLine().Split(new char[]{' '})[1]);
                double cs = System.Convert.ToDouble(sr.ReadLine().Split(new char[]{' '})[1]);
                double csh = cs/2;
                double tlx = x-csh;
                xmin = tlx;
                double tly = y-csh+(rws*cs);
                ymax = tly;
                double brx = tlx + clms * cs;
                xmax = brx;
                double bry = y-csh;
                ymin = bry;
                IPnt tl = new PntClass();
                tl.SetCoords(x,tly-csh);
                tlPntArr[0] = tl;
                cellSize = new PntClass();
                cellSize.SetCoords(cs,cs);
                noDataVl = System.Convert.ToSingle(sr.ReadLine().Split(new char[]{' '})[1]);
                sr.Close();
            }
            for (int i = 1; i < headerFiles.Length; i++)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(headerFiles[i]))
                {
                    int clms = System.Convert.ToInt32(sr.ReadLine().Split(new char[] { ' ' })[1]);
                    int rws = System.Convert.ToInt32(sr.ReadLine().Split(new char[] { ' ' })[1]);
                    clmArr[i] = clms;
                    rwArr[i] = rws;
                    double x = System.Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ' })[1]);
                    double y = System.Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ' })[1]);
                    double cs = System.Convert.ToDouble(sr.ReadLine().Split(new char[] { ' ' })[1]);
                    double csh = cs / 2;
                    double tlx = x - csh;
                    if (tlx < xmin) xmin = tlx;
                    double tly = y - csh + (rws * cs);
                    if (tly > ymax) ymax = tly;
                    double brx = tlx + clms * cs;
                    if (brx > xmax) xmax = brx;
                    double bry = y-csh;
                    if (bry < ymin) ymin = bry;
                    IPnt tl = new PntClass();
                    tl.SetCoords(x, tly - csh);
                    tlPntArr[i] = tl;
                    sr.Close();
                }

            }
            IEnvelope env = new EnvelopeClass();
            env.PutCoords(xmin, ymin, xmax, ymax);
            //Console.WriteLine(env.XMin.ToString() + " " + env.XMax.ToString() + " " + env.YMin.ToString() + " " + env.YMax.ToString());
            return env;
        }
        public void ConvertGridMetricsToRasterFusion(string metricsDir, string outDirectory,string[] metrics)
        {
            int[] m = new int[metrics.Length];
            for(int i=0;i<metrics.Length;i++)
            {
                string s  = metrics[i];
                int vlIndex = System.Array.IndexOf(MetricsArr, s);
                m[i] = vlIndex;
            }
            ConvertGridMetricsToRasterFusion(metricsDir, outDirectory, m);
        }
        public void ConvertGridMetricsToRasterFusion(string metricsDir, string outDir, int[] metrics)
        {
            if (fusionInstalled)
            {
                string[] metricFiles = System.IO.Directory.GetFiles(metricsDir, "*.csv");
                int cnt = 0;
                string exePath = "\"" + fusDir + "\\" + csv2Grid + "\"";
                for (int i = 0; i < metrics.Length; i++)
                {
                    int clm = metrics[i];
                    string metricName = MetricsArr[clm];
                    foreach (string s in metricFiles)
                    {
                        cnt++;
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                        Console.WriteLine("Converting grid metrics " + metricName + " to Raster for " + fileName);
                        string args = "\"" + s + "\" " + clm.ToString() + " \"" + outDir + "\\" + fileName + "_" + metricName + ".asc\"";
                        //Console.WriteLine(args);
                        System.Diagnostics.Process proc = runFusion(exePath, args);
                        if (cnt >= prc)
                        {
                            proc.WaitForExit();
                            cnt = 0;
                        }
                    }
                }
            }
        }

        public void RunGridMetrics(string inLidarDir, string outDir, int CellSize = 10, double HeightCuttOff = 0, string DtmDir = "",double cutBelow=0,double cutAbove=150)
        {
            if (fusionInstalled)
            {
                string[] lasFiles = System.IO.Directory.GetFiles(inLidarDir, "*.las");
                int cnt = 0;
                string exePath = "\"" + fusDir + "\\" + gridMetrics + "\"";
                //Console.WriteLine(exePath);
                //Console.WriteLine(lasFiles.Length.ToString());
                foreach (string s in lasFiles)
                {
                    if (System.IO.Path.GetExtension(s).ToLower() == ".las")
                    {
                        cnt++;
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                        string dtmFile = DtmDir + "\\" + fileName + ".dtm";
                        Console.WriteLine("Calculating grid metrics for LidarFile " + fileName);
                        string args = HeightCuttOff + " " + CellSize.ToString() + " \"" + outDir + "\\" + fileName + ".csv\" \"" + s + "\"";
                        
                        if (dtmFile != null && System.IO.File.Exists(dtmFile))
                        {
                            args = "/nointensity /align:\""+dtmFile+"\" /outlier:" + cutBelow.ToString() + "," + cutAbove.ToString() + " \"" + dtmFile + "\" " + args;
                        }
                        else
                        {
                            args = "/noground /nointensity " + args;
                        }
                        Console.WriteLine(args);
                        System.Diagnostics.Process proc = runFusion(exePath, args);
                        if (cnt >= prc)
                        {
                            proc.WaitForExit();
                            cnt = 0;
                        }
                    }
                }
            }
        }

        private System.Diagnostics.Process runFusion(string exePath, string cmd)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo sInfo = proc.StartInfo;
            sInfo.Arguments = cmd;
            sInfo.FileName = exePath;
            sInfo.UseShellExecute = false;
            sInfo.CreateNoWindow = true;
            proc.Start();
            return proc;
        }        
    }
}
