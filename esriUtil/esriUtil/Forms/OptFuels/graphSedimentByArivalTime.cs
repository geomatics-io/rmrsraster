using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.Forms.OptFuels
{
    public class graphSedimentByArivalTime
    {
        public enum ArrivalClasses { Hours, Days }
        public graphSedimentByArivalTime()
        {
            rsUtil = new rasterUtil();
            tempDir = System.Environment.GetEnvironmentVariable("temp") + "\\strConv";
            if(System.IO.Directory.Exists(tempDir))
            {
                try
                {
                    System.IO.Directory.Delete(tempDir, true);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            geoUtil.check_dir(tempDir);
        }
        public graphSedimentByArivalTime(rasterUtil rasterUtility, IMap map)
        {
            mp = map;
            rsUtil = rasterUtility;
            tempDir = System.Environment.GetEnvironmentVariable("temp") + "\\strConv";
            if (System.IO.Directory.Exists(tempDir))
            {
                try
                {
                    System.IO.Directory.Delete(tempDir, true);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            geoUtil.check_dir(tempDir);
        }
        
        private double cellSize = 5;
        public double CellSize { get { return cellSize; } set { cellSize = value; } }
        private IRaster snapRaster = null;
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private IWorkspace wks = null;
        private string resultsDir = null;
        private List<int> iterLst = new List<int>();
        private IPnt treatCellSize = null;
        public string ResultsDir 
        { 
            get 
            { 
                return resultsDir; 
            }
            set
            {
                resultsDir = value;
                graphDir = resultsDir + "\\graph";
                try
                {
                    if (System.IO.Directory.Exists(graphDir))
                    {
                        System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(graphDir);
                        foreach (System.IO.FileInfo fInfo in dInfo.GetFiles())
                        {
                            try
                            {
                                fInfo.Delete();
                            }
                            catch
                            {
                            }
                        }

                        foreach (System.IO.DirectoryInfo dInfo2 in dInfo.GetDirectories())
                        {
                            try
                            {
                                dInfo2.Delete(true);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error in deleting graph directory here is the error:" + e.ToString());
                }
                geoUtil.check_dir(graphDir);
                wks = geoUtil.OpenRasterWorkspace(graphDir);
                setRasterType();
                scenerioDir = System.IO.Path.GetDirectoryName(resultsDir);
                treatGrid = rsUtil.returnRaster(scenerioDir + "\\treatgrid.txt");
                treatCellSize = ((IRasterProps)treatGrid).MeanCellSize();
                //treatGrid = reSampleRasterGrid(treatGrid);
                inputDir = System.IO.Path.GetDirectoryName(scenerioDir);
                sedDir = inputDir + "\\sed";
                geoUtil.check_dir(sedDir);
                projectDir = System.IO.Path.GetDirectoryName(inputDir);
            }
        }

        private IRaster reSampleRasterGrid(IRaster raster)
        {
            IRasterGeometryProc3 rsGeoP3 = new RasterGeometryProcClass();
            rsGeoP3.Resample(rstResamplingTypes.RSP_NearestNeighbor, cellSize, raster);
            return raster;
        }
        private string tempDir = null;
        private string graphDir = null;
        public string GraphDir { get { return graphDir; } }
        public IWorkspace WorkSpace 
        { 
            get
            { 
                return wks;
            }
        }
        private int numPer = 0;
        private int numFir = 0;
        private int numIter = 0;
        private List<IRaster> periodRasterLst = new List<IRaster>();
        private RunningProcess.frmRunningProcessDialog rpForm = null;
        public RunningProcess.frmRunningProcessDialog RunningProcessForm { get { return rpForm; } set { rpForm = value; } }
        private void createPeriodRasters(int iterationNumber)
        {
            string rpFile = resultsDir + "\\Out_Int_RP_" + iterationNumber.ToString() + ".txt";
            List<HashSet<int>> pLst = new List<HashSet<int>>();
            List<IRemapFilter> remapFiltLst = new List<IRemapFilter>();
            for (int i = 1; i <= numPer; i++)
            {
                pLst.Add(new HashSet<int>());
                IRemapFilter flt = new RemapFilterClass();
                remapFiltLst.Add(flt);
            }
            using(System.IO.StreamReader sr = new System.IO.StreamReader(rpFile))
            {
                string ln = sr.ReadLine();
                while((ln=sr.ReadLine())!=null)
                {
                    string[] lnArr = ln.Split(new char[]{'\t'});
                    if ((lnArr.Length) < 6) break;
                    int period = System.Convert.ToInt32(lnArr[3]);
                    int id = System.Convert.ToInt32(lnArr[0]);
                    if (period > 0)
                    {
                        HashSet<int> oLst = pLst[period-1];
                        oLst.Add(id);
                        pLst[period-1] = oLst;
                    }
                }
                sr.Close();
            }
            IRaster2 rs2 = (IRaster2)treatGrid;
            ITable rsVat = rs2.AttributeTable;
            ICursor cur = null;
            try
            {
                cur = rsVat.Search(null, false);
            }
            catch
            {
                rsVat = rsUtil.buildVat(treatGrid);
                cur = rsVat.Search(null, false);
            }
            int valueIndex = cur.FindField("Value");
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                int vl = System.Convert.ToInt32(rw.get_Value(valueIndex));
                for (int i = 1; i <= numPer; i++)
                {
                    IRemapFilter fl = remapFiltLst[i-1];
                    HashSet<int> lst = pLst[i-1];
                    int nVl = 0;
                    if (lst.Contains(vl))
                    {
                        nVl = 1;
                        //Console.WriteLine("Add Value Remap Class = " + vl.ToString() + " to " + nVl.ToString());
                    }
                    
                    fl.AddClass(vl, vl + 0.000001, nVl);
                }
                rw = cur.NextRow();
            }
            periodRasterLst.Clear();
            for (int i = 1; i <= numPer; i++)
            {
                IRemapFilter flt = remapFiltLst[i-1];
                
                IRaster oRs = rsUtil.calcRemapFunction(treatGrid, flt);// reSampleRasterGrid(rsUtil.calcRemapFunction(treatGrid, flt));
                if (createinter)
                {
                    rsUtil.saveRasterToDataset(oRs, "Treat_" + iterationNumber.ToString() + i.ToString(), wks,rasterUtil.rasterType.IMAGINE);
                }
                periodRasterLst.Add(oRs);
            }    
        }
        private IRaster treatGrid = null;
        private void setRasterType()
        {
            string wksPath = wks.PathName;
            if (wksPath.EndsWith(".accdb") || wksPath.EndsWith(".mdb") || wksPath.EndsWith(".sde")) rsType = rasterUtil.rasterType.GDB;
            else rsType = rasterUtil.rasterType.GRID;
            string lastRS = "";
            bool checkNA = false;
            foreach(string s in System.IO.Directory.GetFiles(resultsDir,"arrivaltime*.txt"))
            {
                lastRS = System.IO.Path.GetFileNameWithoutExtension(s);
                string[] sArr = lastRS.Split(new char[]{'_'});
                int perNum = System.Convert.ToInt32(sArr[2]);
                if (perNum > numPer) numPer = perNum;
                int firNum = System.Convert.ToInt32(sArr[1]);
                if(firNum>numFir)numFir= firNum;
                int iterNum = System.Convert.ToInt32(sArr[0].Replace("arrivaltime",""));
                if (iterNum == 0)
                {
                    checkNA = true;
                }
                if(iterNum>numIter)numIter=iterNum;
                lastRS = s;
            }
            if (checkNA && numIter > 0)
            {
                iterLst.Add(0);
            }
            iterLst.Add(numIter);
            snapRaster = rsUtil.returnRaster(lastRS);
        }
        private string scenerioDir = null;
        private string f10fld = "NA10";
        private string f50fld = "NA50";
        private string t10fld = "TR10";
        private string t50fld = "TR50";
        private string hsf10fld = "HS10";
        private string hsf50fld = "HS50";
        public string Fine10Field { get { return f10fld; } set { f10fld = value; } }
        public string Fine50Field { get { return f50fld; } set { f50fld = value; } }
        public string T1_Fine_10Field { get { return t10fld; } set { t10fld = value; } }
        public string T1_Fine_50Field { get { return t50fld; } set { t50fld = value; } }
        public string HSF10Field { get { return hsf10fld; } set { hsf10fld = value; } }
        public string HSF50Field { get { return hsf50fld; } set { hsf50fld = value; } }        
        private IFeatureClass ftrCls = null;
        private rasterUtil.rasterType rsType = rasterUtil.rasterType.GDB;
        private string streamName = "rcz";
        private string inputDir = null;
        private string sedDir = null;
        private string projectDir = null;
        public IFeatureClass StreamPolygon
        {
            get
            {
                return ftrCls;
            }
            set
            {
                IWorkspace tWks = geoUtil.OpenRasterWorkspace(tempDir);
                ftrCls=value;
                if (wks != null)
                {
                    if (System.IO.Directory.Exists(sedDir))
                    {
                        IWorkspace sedWks = geoUtil.OpenRasterWorkspace(sedDir);
                        if (rsUtil.rasterExists(sedWks, "n10.img"))
                        {
                            if (System.Windows.Forms.DialogResult.Yes == System.Windows.Forms.MessageBox.Show("Sediment Raster currently exist. Do you want to replace existing sediment raster?", "Replace Existing Rasters?", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question))
                            {
                                Console.WriteLine("Replacing Existing Rasters");
                                try
                                {
                                    //removelocks();
                                    removeSedFiles();
                                    //System.IO.Directory.Delete(sedDir, true);
                                    geoUtil.check_dir(sedDir);
                                    streamName = rsUtil.getSafeOutputName(tWks, streamName);
                                    IEnvelope ext = ((IGeoDataset)snapRaster).Extent;
                                    sedRaster = rsUtil.convertFeatureClassToRaster(ftrCls, rsType, tWks, streamName, 5, ((IRaster2)snapRaster).RasterDataset, ext);
                                    createSedimentSurfaces();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                    rpForm.addMessage("Lock on existing sediment Raster. Using existing sediment rasters! If you want to replace you must restart ArcMap!");
                                    rsfine10 = rsUtil.returnRaster(sedDir + "\\n10.img");
                                    rsfine50 = rsUtil.returnRaster(sedDir + "\\n50.img");
                                    rst1Fine10 = rsUtil.returnRaster(sedDir + "\\t10.img");
                                    rst1Fine50 = rsUtil.returnRaster(sedDir + "\\t50.img");
                                    rshsf10 = rsUtil.returnRaster(sedDir + "\\h10.img");
                                    rshsf50 = rsUtil.returnRaster(sedDir + "\\h50.img");
                                    if (createinter)
                                    {
                                        rsUtil.saveRasterToDataset(rsfine10, "n10", wks, rasterUtil.rasterType.IMAGINE);
                                        rsUtil.saveRasterToDataset(rsfine50, "n50", wks, rasterUtil.rasterType.IMAGINE);
                                        rsUtil.saveRasterToDataset(rst1Fine10, "t10", wks, rasterUtil.rasterType.IMAGINE);
                                        rsUtil.saveRasterToDataset(rst1Fine50, "t50", wks, rasterUtil.rasterType.IMAGINE);
                                        rsUtil.saveRasterToDataset(rshsf10, "h10", wks, rasterUtil.rasterType.IMAGINE);
                                        rsUtil.saveRasterToDataset(rshsf50, "h50", wks, rasterUtil.rasterType.IMAGINE);
                                    }
                                }
                                                             
                            }
                            else
                            {
                                rsfine10 = rsUtil.returnRaster(sedDir + "\\n10.img");
                                rsfine50 = rsUtil.returnRaster(sedDir + "\\n50.img");
                                rst1Fine10 = rsUtil.returnRaster(sedDir + "\\t10.img");
                                rst1Fine50 = rsUtil.returnRaster(sedDir + "\\t50.img");
                                rshsf10 = rsUtil.returnRaster(sedDir + "\\h10.img");
                                rshsf50 = rsUtil.returnRaster(sedDir + "\\h50.img");
                                if (createinter)
                                {
                                    rsUtil.saveRasterToDataset(rsfine10, "n10", wks,rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(rsfine50, "n50", wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(rst1Fine10, "t10", wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(rst1Fine50, "t50", wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(rshsf10, "h10", wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(rshsf50, "h50", wks, rasterUtil.rasterType.IMAGINE);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Can't find n10.img");
                            streamName = rsUtil.getSafeOutputName(wks, streamName);
                            IEnvelope ext = ((IGeoDataset)snapRaster).Extent;
                            sedRaster = rsUtil.convertFeatureClassToRaster(ftrCls, rsType, tWks, streamName, 5, ((IRaster2)snapRaster).RasterDataset, ext);
                            createSedimentSurfaces();
                        }
                    }
                    else
                    {
                        Console.WriteLine("SedWks does not exist");
                    }
                }
                else
                {
                    streamName = rsUtil.getSafeOutputName(wks, streamName);
                    IEnvelope ext = ((IGeoDataset)snapRaster).Extent;
                    sedRaster = rsUtil.convertFeatureClassToRaster(ftrCls, rsType, tWks, streamName, 5, ((IRaster2)snapRaster).RasterDataset, ext);
                    createSedimentSurfaces();
                }
            }
        }

        private void removeSedFiles()
        {
            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(sedDir);
            foreach (System.IO.FileInfo fInfo in dInfo.GetFiles())
            {
                fInfo.Delete();
            }
        }

        private void removelocks()
        {
            IRaster[] rsArr = { rsfine10, rsfine50, rst1Fine10, rst1Fine50, rshsf10, rshsf50 };
            foreach (IRaster rs in rsArr)
            {
                IRasterDataset rsDset = ((IRaster2)rs).RasterDataset;
                IDataset dSet = (IDataset)rsDset;
                try
                {
                    rsUtil.removeLock(dSet);
                }
                catch
                {
                    rpForm.addMessage("Could not remove lock on " + dSet.BrowseName);
                }
            }
        }
        private IRaster sedRaster = null;
        private bool createinter = false;
        private bool createcore = false;
        public bool CreateCoreRasters { get { return createcore; } set { createcore = value; } }
        public bool CreateIntermediateRasters { get { return createinter; } set { createinter = value; } }
        private void createSedimentSurfaces()
        {
            IWorkspace sedWks = geoUtil.OpenRasterWorkspace(sedDir);
            IFeatureCursor ftrCur = ftrCls.Search(null, false);
            IFeature ftr = ftrCur.NextFeature();
            IRemapFilter fine10 = new RemapFilterClass();
            IRemapFilter fine50 = new RemapFilterClass();
            IRemapFilter t1fine10 = new RemapFilterClass();
            IRemapFilter t1fine50 = new RemapFilterClass();
            IRemapFilter hsf10 = new RemapFilterClass();
            IRemapFilter hsf50 = new RemapFilterClass();
            int fine10Index = ftrCls.FindField(f10fld);
            int fine50Index = ftrCls.FindField(f50fld);
            int t1fine10Index = ftrCls.FindField(t10fld);
            int t1fine50Index = ftrCls.FindField(t50fld);
            int hsf10Index = ftrCls.FindField(hsf10fld);
            int hsf50Index = ftrCls.FindField(hsf50fld);
            IRemapFilter[] remapArr = { fine10, fine50, t1fine10, t1fine50, hsf10, hsf50 };
            int[] fldArr = { fine10Index, fine50Index, t1fine10Index, t1fine50Index, hsf10Index, hsf50Index };
            int cnt = 0;
            int numCells = System.Convert.ToInt32(treatCellSize.X / cellSize); 
            while (ftr != null)
            {
                //need to set up remap filters for each raster and create a new raster for each sediment value
                double oid = System.Convert.ToInt32(ftr.OID);
                cnt = 0;
                foreach (IRemapFilter remap in remapArr)
                {
                    int indexVl = fldArr[cnt];
                    double vl = System.Convert.ToDouble(ftr.get_Value(indexVl)) * (Math.Pow((cellSize * 3.2808399), 2) / 43560);
                    remap.AddClass(oid, oid + 0.00001, vl);
                    // Console.WriteLine("Converting oid " + oid.ToString() + " to " + vl.ToString() );
                    cnt++;
                }
                ftr = ftrCur.NextFeature();
            }
            cnt = 1;
            IRaster[] rsArr = { rsfine10, rsfine50, rst1Fine10, rst1Fine50, rshsf10, rshsf50 };
            foreach (IRemapFilter remap in remapArr)
            {
                string rsNm = "";
                IRaster rs = null;
                switch (cnt)
                {
                    case 1:
                        rsfine10 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rsfine10 = rsUtil.blockSum(rsfine10, sedWks,"n10",numCells,rstPixelType.PT_FLOAT);
                        rs = rsfine10;
                        rsNm = "n10";
                        break;
                    case 2:
                        rsfine50 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rsfine50 = rsUtil.blockSum(rsfine50, sedWks, "n50", numCells, rstPixelType.PT_FLOAT);
                        rs = rsfine50;
                        rsNm = "n50";
                        break;
                    case 3:
                        rst1Fine10 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rst1Fine10 = rsUtil.blockSum(rst1Fine10, sedWks, "t10", numCells, rstPixelType.PT_FLOAT);
                        rs = rst1Fine10;
                        rsNm = "t10";
                        break;
                    case 4:
                        rst1Fine50 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rst1Fine50 = rsUtil.blockSum(rst1Fine50, sedWks, "t50", numCells, rstPixelType.PT_FLOAT);
                        rs = rst1Fine50;
                        rsNm = "t50";
                        break;
                    case 5:
                        rshsf10 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rshsf10 = rsUtil.blockSum(rshsf10, sedWks, "h10", numCells, rstPixelType.PT_FLOAT);
                        rs = rshsf10;
                        rsNm = "h10";
                        break;
                    case 6:
                        rshsf50 = rsUtil.calcRemapFunction(sedRaster, remap);
                        rshsf50 = rsUtil.blockSum(rshsf50, sedWks, "h50", numCells, rstPixelType.PT_FLOAT);
                        rs = rshsf50;
                        rsNm = "h50";
                        break;
                    default:
                        break;
                }
                cnt++;
                if (createinter && rsNm != "" && rs != null)
                {
                    rsUtil.saveRasterToDataset(rs, rsNm, wks);
                }

            }
        }
        private IRaster rsfine10, rsfine50, rst1Fine10, rst1Fine50, rshsf10, rshsf50;
        private double flmlng = 3;
        public double FlameLength { get { return flmlng; } set { flmlng = value; } }
        private IRaster createBooleanFlame(IRaster flameRaster)
        {
            IRaster rs = rsUtil.calcGreaterFunction(flameRaster, FlameLength);
            return rs;// reSampleRasterGrid(rs);
        }
        private ArrivalClasses arrCls = ArrivalClasses.Hours;
        public ArrivalClasses ArrivalClass { get { return arrCls; } set { arrCls = value; } }
        private IRaster createArivaltimeZones(IRaster arrivalTime)
        {
            IRemapFilter arrivalRemap = new RemapFilterClass();
            IRasterStatistics rsStats = ((IRasterBandCollection)arrivalTime).Item(0).Statistics;
            if (rsStats == null)
            {
                rsUtil.calcStatsAndHist(arrivalTime);
                rsStats = ((IRasterBandCollection)arrivalTime).Item(0).Statistics;
            }
            else if (!rsStats.IsValid)
            {
                rsStats.SkipFactorX = 1;
                rsStats.SkipFactorY = 1;
                rsStats.Recalculate();
            }
            else
            {
            }
            double max = rsStats.Maximum;
            double min = rsStats.Minimum;
            double skip = 1;
            switch (arrCls)
	        {
                case ArrivalClasses.Hours:
                    skip = 60;
                    break;
                case ArrivalClasses.Days:
                    skip = 1440;
                    break;
                default:
                    break;
	        }
            for (double i = 0; i <= max; i+= skip)
            {
                double nVl = (i + skip)/skip;
                arrivalRemap.AddClass(min, i + skip, nVl);
            }
            IRaster reRs = rsUtil.calcRemapFunction(arrivalTime, arrivalRemap);
            return reRs;// reSampleRasterGrid(reRs);
        }
        private string outcsvpath = null;
        public string OutSedimentByArivalTimePath { get { return outcsvpath; } }
        public string sumSedimentValues()
        {
            string outCSV = graphDir + "\\SedimentByArivalTime.csv";
            using (System.IO.StreamWriter swr = new System.IO.StreamWriter(outCSV))
            {
                string ln = "Alternative,Fires,Period,Sed10,Sed50,Sed10_A,Sed50_A,ArivalTime,CellCount";
                swr.WriteLine(ln);
                if (iterLst.Count < 2)
                {
                    Console.WriteLine("copying files over");
                    copyNaToResults(ref iterLst);
                }
                int iterCnt = 0;
                string itop = "";
                foreach (int iter in iterLst)
                {
                    if (iterCnt == 0)
                    {
                        itop = "No Action";
                    }
                    else
                    {
                        itop = "Optimum";
                    }
                    iterCnt++;
                    createPeriodRasters(iter);
                    for (int f = 1; f <= numFir; f++)
                    {
                        for (int i = 1; i <= numPer; i++)
                        {
                            string strln = "Building raster for iteration " + iter.ToString() + " fire " + f.ToString() + " period " + i.ToString();
                            if (rpForm != null)
                            {
                                rpForm.addMessage(strln);
                                rpForm.stepPGBar(5);
                                rpForm.Refresh();
                            }
                            else
                            {
                                Console.WriteLine(strln);
                            }
                            //treatment rasters
                            IRaster perRs = periodRasterLst[i - 1];
                            IRaster noActionPeriod = rsUtil.calcEqualFunction(perRs, 0);
                            IRaster t10 = rsUtil.calcArithmaticFunction(perRs, rst1Fine10, esriRasterArithmeticOperation.esriRasterMultiply);
                            IRaster f10 = rsUtil.calcArithmaticFunction(noActionPeriod, rsfine10, esriRasterArithmeticOperation.esriRasterMultiply);
                            IRaster t50 = rsUtil.calcArithmaticFunction(perRs, rst1Fine50, esriRasterArithmeticOperation.esriRasterMultiply);
                            IRaster f50 = rsUtil.calcArithmaticFunction(noActionPeriod, rsfine50, esriRasterArithmeticOperation.esriRasterMultiply);
                            IRaster sum10 = rsUtil.calcArithmaticFunction(t10, f10, esriRasterArithmeticOperation.esriRasterPlus);
                            IRaster sum50 = rsUtil.calcArithmaticFunction(t50, f50, esriRasterArithmeticOperation.esriRasterPlus);
                            //flamelength rasters
                            string flameRasterPath = resultsDir + "\\nodeflamelength" + iter.ToString() + "_" + f.ToString() + "_" + i.ToString() + ".txt";
                            IRaster flmLng = rsUtil.returnRaster(flameRasterPath);
                            IRaster flmBool = createBooleanFlame(flmLng);
                            //combined raster
                            IRaster final10 = rsUtil.conditionalRasterFunction(flmBool, rshsf10, sum10);
                            IRaster final50 = rsUtil.conditionalRasterFunction(flmBool, rshsf50, sum50);
                            //arrival zones
                            string ArivalRasterPath = resultsDir + "\\arrivaltime" + iter.ToString() + "_" + f.ToString() + "_" + i.ToString() + ".txt";
                            IRaster ariv = rsUtil.returnRaster(ArivalRasterPath);
                            IRaster arivZones = createArivaltimeZones(ariv);
                            strln = "Summarizing arrival zones for iteration " + iter.ToString() + " fire " + f.ToString() + " period " + i.ToString();
                            if (rpForm != null)
                            {
                                rpForm.addMessage(strln);
                                rpForm.stepPGBar(5);
                                rpForm.Refresh();
                            }
                            else
                            {
                                Console.WriteLine(strln);
                            }
                            ln = getSummaryValue(itop,f,i, final10, final50, arivZones);
                            swr.Write(ln);
                            if (createinter)
                            {
                                strln = "Saving all intermediate rasters...";
                                if (rpForm != null)
                                {
                                    rpForm.addMessage(strln);
                                    rpForm.stepPGBar(5);
                                    rpForm.Refresh();
                                }
                                else
                                {
                                    Console.WriteLine(strln);
                                }
                                rsUtil.saveRasterToDataset(noActionPeriod, "noAct_" + iter.ToString() + f + i.ToString(), wks,rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(t10, "t10_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(f10, "na10_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(t50, "t50_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(f50, "na50_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(sum10, "s10_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(sum50, "s50_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(flmBool, "flm_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(final10, "fl10_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(final50, "fl50_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                rsUtil.saveRasterToDataset(arivZones, "az" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                            }
                            if (createcore)
                            {
                                if (!createinter)
                                {
                                    strln = "Saving core rasters...";
                                    if (rpForm != null)
                                    {
                                        rpForm.addMessage(strln);
                                        rpForm.stepPGBar(5);
                                        rpForm.Refresh();
                                    }
                                    else
                                    {
                                        Console.WriteLine(strln);
                                    }
                                    rsUtil.saveRasterToDataset(final10, "fl10_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(final50, "fl50_" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                    rsUtil.saveRasterToDataset(arivZones, "az" + iter.ToString() + f + i.ToString(), wks, rasterUtil.rasterType.IMAGINE);
                                }
                            }
                            if(!createcore&&!createinter)
                            {
                                try
                                {
                                    if (sedRaster != null)
                                    {
                                        rsUtil.deleteRasterDataset(((IRaster2)sedRaster).RasterDataset.CompleteName);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        }
                    }
                }
                swr.Close();
            }
            outcsvpath = outCSV;
            removelocks();
            return outCSV;

        }

        private void copyNaToResults(ref List<int> iterLst)
        {
            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(inputDir);
            string nIncNum = (numIter + 1).ToString();
            System.IO.DirectoryInfo rInfo = null;
            System.IO.FileInfo[] fInfos = null;
            bool checkfls = false;
            foreach (System.IO.DirectoryInfo d in dInfo.GetDirectories())
            {
                string rPath = d.FullName + "\\RESULTS";
                //Console.WriteLine(rPath);
                if (d.Name.EndsWith("S1_NA"))
                {
                    if (System.IO.Directory.Exists(rPath))
                    {
                        //Console.WriteLine("getting files");
                        rInfo = new System.IO.DirectoryInfo(rPath);
                        fInfos = rInfo.GetFiles("arrivaltime0*.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = fInfo.Name;
                            fNm = fNm.Replace("arrivaltime0", "arrivaltime" + nIncNum);
                            //Console.WriteLine("\tcopying " + fInfo.Name + " to " + resultsDir + "\\"+ fNm);
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                            checkfls = true;
                        }
                        fInfos = rInfo.GetFiles("nodeflamelength0*.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = fInfo.Name;
                            fNm = fNm.Replace("nodeflamelength0", "nodeflamelength" + nIncNum);
                            //Console.WriteLine("\tcopying " + fInfo.Name + " to " + resultsDir + "\\" + fNm);
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                        }
                        fInfos = rInfo.GetFiles("Out_Int_RP_0.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = "Out_Int_RP_" + nIncNum +".txt";
                            //Console.WriteLine("\tcopying " + fInfo.Name + " to " + fNm);
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                        }
                        if (checkfls)
                        {
                            iterLst.Add(System.Convert.ToInt32(nIncNum));
                        }
                        break;
                    }
                }
                else
                {
                    rInfo = new System.IO.DirectoryInfo(rPath);
                    bool checkMultipleIterations = false;
                    fInfos = rInfo.GetFiles("arrivaltime*.txt");
                    int fCnt = 0;
                    foreach (System.IO.FileInfo fInfo in fInfos)
                    {
                        if (fCnt > 0) checkMultipleIterations = true;
                        fCnt++;
                    }
                    if (!checkMultipleIterations)
                    {
                        continue;
                    }
                    else
                    {
                        fInfos = rInfo.GetFiles("arrivaltime0*.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = fInfo.Name;
                            string[] itVlArr = fNm.Split(new char[] { '_' });
                            int vl = System.Convert.ToInt32(itVlArr[0].Replace("arrivaltime", ""));
                            fNm = fNm.Replace("arrivaltime0", "arrivaltime" + nIncNum);
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                            checkfls = true;
                        }
                        fInfos = rInfo.GetFiles("nodeflamelength0*.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = fInfo.Name;
                            fNm = fNm.Replace("nodeflamelength0", "nodeflamelength" + nIncNum);
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                        }
                        fInfos = rInfo.GetFiles("Out_Int_RP_0.txt");
                        foreach (System.IO.FileInfo fInfo in fInfos)
                        {
                            string fNm = "Out_Int_RP_" + nIncNum + ".txt";
                            fInfo.CopyTo(resultsDir + "\\" + fNm, true);
                        }
                        if (checkfls)
                        {
                            iterLst.Add(System.Convert.ToInt32(nIncNum));
                        }
                        break;
                    }
                }
                
            }
        }

        private string getSummaryValue(string iteration,int fire,int Period, IRaster final10, IRaster final50, IRaster arivZones)
        {
            IRasterProps rsProp = (IRasterProps)arivZones;
            System.Array noDataAr = (System.Array)rsProp.NoDataValue;
            System.Array noDataf50 = (System.Array)((IRasterProps)final50).NoDataValue;
            System.Array noDataf10 = (System.Array)((IRasterProps)final10).NoDataValue;
            double noDataVlAr = System.Convert.ToDouble(noDataAr.GetValue(0));
            double noDataVlf50 = System.Convert.ToDouble(noDataf50.GetValue(0));
            double noDataVlf10 = System.Convert.ToDouble(noDataf10.GetValue(0));
            Dictionary<int, float[]> vlDic = new Dictionary<int, float[]>();
            IRasterCursor rsCur = ((IRaster2)arivZones).CreateCursorEx(null);
            IPnt pnt = new PntClass();
            pnt.X = rsCur.PixelBlock.Width;
            pnt.Y = rsCur.PixelBlock.Height;
            IRasterCursor rsCur2 = ((IRaster2)final10).CreateCursorEx(pnt);
            IRasterCursor rsCur3 = ((IRaster2)final50).CreateCursorEx(pnt);
            while (rsCur.Next() && rsCur2.Next() && rsCur3.Next())
            {
                IPixelBlock3 pb = (IPixelBlock3)rsCur.PixelBlock;
                IPixelBlock3 pb2 = (IPixelBlock3)rsCur2.PixelBlock;
                IPixelBlock3 pb3 = (IPixelBlock3)rsCur3.PixelBlock;
                int ht = pb.Height;
                int wd = pb.Width;
                System.Array arvTArr = (System.Array)pb.get_PixelData(0);
                System.Array f10Arr = (System.Array)pb2.get_PixelData(0);
                System.Array f50Arr = (System.Array)pb3.get_PixelData(0);
                for (int w = 0; w < wd; w++)
                {
                    for (int h = 0; h < ht; h++)
                    {
                        int at = System.Convert.ToInt32(arvTArr.GetValue(w,h));
                        float f10 = System.Convert.ToSingle(f10Arr.GetValue(w, h));
                        float f50 = System.Convert.ToSingle(f50Arr.GetValue(w, h));
                        if (Single.IsNaN(f10) || Single.IsNaN(f50)||Single.IsNegativeInfinity(f10)||Single.IsNegativeInfinity(f50)||Single.IsPositiveInfinity(f10)||Single.IsPositiveInfinity(f50)||at==-9999||at==noDataVlAr||f50==noDataVlf50||f10==noDataVlf50)
                        {
                            //Console.WriteLine(f10.ToString());
                            continue;
                        }
                        float[] fArr = {0,0,0};
                        if (vlDic.TryGetValue(at, out fArr))
                        {
                            fArr[0] = fArr[0] + f10;
                            fArr[1] = fArr[1] + f50;
                            fArr[2] = fArr[2] + 1;
                            vlDic[at] = fArr;
                        }
                        else
                        {
                            fArr = new float[] { f10, f50, 1 };
                            vlDic.Add(at, fArr);
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            //need to sort to do accumulative;
            List<int> keySortLst = vlDic.Keys.ToList();
            keySortLst.Sort();
            float ac10 = 0;
            float ac50 = 0;
            foreach (int ky in keySortLst)
            {
                float[] vlArr = vlDic[ky];
                float s10 = vlArr[0];
                float s50 = vlArr[1];
                ac10 += s10;
                ac50 += s50;
                float cellCnt = vlArr[2];
                string newStr = iteration + "," + fire.ToString() + "," + Period.ToString() + "," + s10.ToString() + "," + s50.ToString() + "," + ac10.ToString() + "," + ac50.ToString() + "," + ky.ToString() + "," + cellCnt.ToString();
                //Console.WriteLine(newStr);
                sb.AppendLine(newStr);
            }
            return sb.ToString();
        }

        
    }
}
