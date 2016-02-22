using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace esriUtil
{
    public class batchCalculations
    {
        public batchCalculations()
        {
            rsUtil = new rasterUtil();
            //rp.Show();
        }
        public batchCalculations(rasterUtil rasterUtility, esriUtil.Forms.RunningProcess.frmRunningProcessDialog runningDialog)
        {
            rsUtil = rasterUtility;
            if (rp != null) rp = runningDialog;
        }
        featureUtil ftrUtil = new featureUtil();
        public enum batchGroups { ARITHMETIC, MATH, SETNULL, LOGICAL, CLIP, CONDITIONAL, CONVOLUTION, FOCAL, FOCALSAMPLE, LOCALSTATISTICS, LOCALFOCALSTATISTICS, LINEARTRANSFORM, RESCALE, REMAP, COMPOSITE, EXTRACTBAND, CONVERTPIXELTYPE, GLCM, LANDSCAPE, ZONALSTATS, ZONALCLASSCOUNTS, SAVEFUNCTIONRASTER, BUILDRASTERSTATS, BUILDRASTERVAT, MOSAIC, MERGE, SAMPLERASTER, CLUSTERSAMPLERASTER, CREATERANDOMSAMPLE, CREATESTRATIFIEDRANDOMSAMPLE, MODEL, PREDICT, AGGREGATION, SURFACE, COMBINE, CONSTANT, ROTATE, SHIFT, NULLTOVALUE, SETVALUESTONULL, BUILDMODEL, SELECTSAMPLES, EXPORTTABLE, EXPORTFEATURECLASS, RENAMEFIELD, FORMATZONAL, NDVI, RESAMPLE, LOCALRESCALE, OPENNETCDF, CREATERASTERATTRIBUTE, LOAD, OPENRASTER, OPENFEATURECLASS, OPENTABLE, OPENMODEL }
        private rasterUtil rsUtil = null;
        private System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        private System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string path = "";
        public string BatchPath 
        {
            get 
            { 
                return path; 
            } 
            set 
            { 
                path = value; 
            } 
        }
        private esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new Forms.RunningProcess.frmRunningProcessDialog(false);
        private Dictionary<string, IFunctionRasterDataset> rstDic = new Dictionary<string, IFunctionRasterDataset>();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, ITable> tblDic = new Dictionary<string, ITable>();
        public Dictionary<string, IFunctionRasterDataset> RasterDictionary { get { return rstDic; } set { rstDic = value; } }
        public Dictionary<string, IFeatureClass> FeatureClassDictionary { get { return ftrDic; } set { ftrDic = value; } }
        public Dictionary<string, ITable> TableDictionary { get { return tblDic; } set { tblDic = value; } }
        private Dictionary<string, string> modelDic = new Dictionary<string, string>();
        public Dictionary<string, string> ModelDictionary { get { return modelDic; } set { modelDic = value; } }
        private List<string> lnLst = new List<string>();
        public void manuallyAddBatchLines(string[] lines)
        {
            lnLst.Clear();
            foreach (string s in lines)
            {
                lnLst.Add(s);
            }
        }
        public void saveBatchFile(string[] lines)
        {
            sfd.AddExtension = true;
            sfd.Filter = "Batch File|*.bch";
            sfd.DefaultExt = "bch";
            System.Windows.Forms.DialogResult dr = sfd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string outFileName = sfd.FileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outFileName,false))
                {
                    foreach (string s in lines)
                    {
                        if (s.Length > 0)
                        {
                            sw.WriteLine(s);
                        }
                    }
                    sw.Close();
                }
                path = outFileName;
                loadBatchFile();
            }

        }
        public string openBatchFile()
        {
            string ostr = "";
            ofd.AddExtension = true;
            ofd.DefaultExt = "bch";
            ofd.Multiselect = false;
            ofd.Title = "Open Batch File";
            ofd.Filter = "Batch File|*.bch";
            System.Windows.Forms.DialogResult ds = ofd.ShowDialog();
            if (ds == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
                ostr = loadBatchFile();
            }
            return ostr;
        }
        public string loadBatchFile()
        {
            //Console.WriteLine(path);
            StringBuilder sb = new StringBuilder();
            lnLst.Clear();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string ln = sr.ReadLine();
                while(ln!=null)
                {
                    Console.WriteLine(ln);
                    if (ln.Length > 0)
                    {
                        sb.AppendLine(ln);
                        lnLst.Add(ln);
                    }
                    ln = sr.ReadLine();
                }
                sr.Close();
            }
            return sb.ToString();
        }
        public void runBatch()
        {
            rp.addMessage("Running in batch mode...");
            DateTime dt = DateTime.Now;
            try
            {
                string func = "";
                string param = "";
                string outName = "";
                foreach (string ln in lnLst)
                {
                    rp.stepPGBar(5);
                    rp.Refresh();
                    if (ln.Length > 0)
                    {
                        getFunctionAndParameters(ln, out func, out param, out outName);
                        if (func == "" || param == "" || outName == "")
                        {
                            continue;
                        }
                        else
                        {
                            lstName = outName;
                            try
                            {
                                rp.addMessage("Running process: " + ln);
                                rp.Refresh();
                                batchGroups bg = (batchGroups)Enum.Parse(typeof(batchGroups), func);
                                string[] paramArr = param.Split(new char[] { ';' });
                                switch (bg)
                                {
                                    case batchGroups.ARITHMETIC:
                                        rstDic[outName] = createArithmeticFunction(paramArr);
                                        break;
                                    case batchGroups.MATH:
                                        rstDic[outName] = createMathFunction(paramArr);
                                        break;
                                    case batchGroups.SETNULL:
                                        rstDic[outName] = createSetNullFunction(paramArr);
                                        break;
                                    case batchGroups.LOGICAL:
                                        rstDic[outName] = createLogicalFunction(paramArr);
                                        break;
                                    case batchGroups.CLIP:
                                        rstDic[outName] = createClipFunction(paramArr);
                                        break;
                                    case batchGroups.CONDITIONAL:
                                        rstDic[outName] = createConditionalFunction(paramArr);
                                        break;
                                    case batchGroups.CONVOLUTION:
                                        rstDic[outName] = createConvolutionFunction(paramArr);
                                        break;
                                    case batchGroups.FOCAL:
                                        rstDic[outName] = createFocalFunction(paramArr);//pickback up here
                                        break;
                                    case batchGroups.LOCALSTATISTICS:
                                        rstDic[outName] = createLocalFunction(paramArr);
                                        break;
                                    case batchGroups.LINEARTRANSFORM:
                                        rstDic[outName] = createLinearTransformFunction(paramArr);
                                        break;
                                    case batchGroups.RESCALE:
                                        rstDic[outName] = createRescaleFunction(paramArr);
                                        break;
                                    case batchGroups.REMAP:
                                        rstDic[outName] = createRemapFunction(paramArr);
                                        break;
                                    case batchGroups.COMPOSITE:
                                        rstDic[outName] = createCompositeFunction(paramArr);
                                        break;
                                    case batchGroups.EXTRACTBAND:
                                        rstDic[outName] = createExtractFunction(paramArr);
                                        break;
                                    case batchGroups.GLCM:
                                        rstDic[outName] = createGLCMFunction(paramArr);
                                        break;
                                    case batchGroups.LANDSCAPE:
                                        rstDic[outName] = createLandscapeFunction(paramArr);
                                        break;
                                    case batchGroups.ZONALSTATS:
                                        tblDic[outName] = createZonalStats(paramArr);
                                        break;
                                    case batchGroups.ZONALCLASSCOUNTS:
                                        tblDic[outName] = createZonalClassCounts(paramArr);
                                        break;
                                    case batchGroups.SAVEFUNCTIONRASTER:
                                        rstDic[outName] = saveRaster(paramArr);
                                        break;
                                    case batchGroups.BUILDRASTERSTATS:
                                        rstDic[outName] = buildRasterStats(paramArr);
                                        break;
                                    case batchGroups.BUILDRASTERVAT:
                                        tblDic[outName] = buildRasterVat(paramArr);
                                        break;
                                    case batchGroups.CREATERASTERATTRIBUTE:
                                        tblDic[outName] = createRasterAttribute(paramArr);
                                        break;
                                    case batchGroups.MOSAIC:
                                        rstDic[outName] = createMosaic(paramArr);
                                        break;
                                    case batchGroups.MERGE:
                                        rstDic[outName] = createMerge(paramArr);
                                        break;
                                    case batchGroups.SAMPLERASTER:
                                        ftrDic[outName] = sampleRaster(paramArr);
                                        break;
                                    case batchGroups.CREATERANDOMSAMPLE:
                                        ftrDic[outName] = createRandomSample(paramArr);
                                        break;
                                    case batchGroups.CREATESTRATIFIEDRANDOMSAMPLE:
                                        ftrDic[outName] = createStratifiedRandomSample(paramArr);
                                        break;
                                    case batchGroups.CLUSTERSAMPLERASTER:
                                        ftrDic[outName] = clusterSampleRaster(paramArr);
                                        break;
                                    case batchGroups.FOCALSAMPLE:
                                        rstDic[outName] = createFocalSampleFunction(paramArr);
                                        break;
                                    case batchGroups.MODEL:
                                        rstDic[outName] = createModelFunction(paramArr);
                                        break;
                                    case batchGroups.PREDICT:
                                        tblDic[outName] = predictNewValues(paramArr);
                                        break;
                                    case batchGroups.BUILDMODEL:
                                        modelDic[outName] = buildModels(paramArr);
                                        break;
                                    case batchGroups.AGGREGATION:
                                        rstDic[outName] = createAggregationFunction(paramArr);
                                        break;
                                    case batchGroups.CONVERTPIXELTYPE:
                                        rstDic[outName] = convertPixelType(paramArr);
                                        break;
                                    case batchGroups.SURFACE:
                                        rstDic[outName] = createSurfaceFunction(paramArr);
                                        break;
                                    case batchGroups.COMBINE:
                                        rstDic[outName] = createCombineFunction(paramArr);
                                        break;
                                    case batchGroups.CONSTANT:
                                        rstDic[outName] = createConstantRaster(paramArr);
                                        break;
                                    case batchGroups.ROTATE:
                                        rstDic[outName] = createRotateRaster(paramArr);
                                        break;
                                    case batchGroups.SHIFT:
                                        rstDic[outName] = createShiftRaster(paramArr);
                                        break;
                                    case batchGroups.NULLTOVALUE:
                                        rstDic[outName] = createNullToValueRaster(paramArr);
                                        break;
                                    case batchGroups.SELECTSAMPLES:
                                        tblDic[outName] = selectSamples(paramArr);
                                        break;
                                    case batchGroups.EXPORTTABLE:
                                        tblDic[outName] = exportTable(paramArr);
                                        break;
                                    case batchGroups.EXPORTFEATURECLASS:
                                        ftrDic[outName] = exportFeatureClass(paramArr);
                                        break;
                                    case batchGroups.RENAMEFIELD:
                                        tblDic[outName] = renameField(paramArr);
                                        break;
                                    case batchGroups.FORMATZONAL:
                                        tblDic[outName] = zonalFormatData(paramArr);
                                        break;
                                    case batchGroups.SETVALUESTONULL:
                                        rstDic[outName] = setValuesToNull(paramArr);
                                        break;
                                    case batchGroups.RESAMPLE:
                                        rstDic[outName] = resampleRaster(paramArr);
                                        break;
                                    case batchGroups.NDVI:
                                        rstDic[outName] = ndviRaster(paramArr);
                                        break;
                                    case batchGroups.LOCALRESCALE:
                                        rstDic[outName] = localRescale(paramArr);
                                        break;
                                    case batchGroups.OPENNETCDF:
                                        rstDic[outName] = openNetCdf(paramArr);
                                        break;
                                    case batchGroups.LOAD:
                                        lBatch(paramArr, outName);
                                        break;
                                    case batchGroups.LOCALFOCALSTATISTICS:
                                        rstDic[outName] = localFocalStatistics(paramArr);
                                        break;
                                    case batchGroups.OPENRASTER:
                                        rstDic[outName] = openRaster(paramArr);
                                        break;
                                    case batchGroups.OPENFEATURECLASS:
                                        ftrDic[outName] = openfeatureClass(paramArr);
                                        break;
                                    case batchGroups.OPENTABLE:
                                        tblDic[outName] = openTable(paramArr);
                                        break;
                                    case batchGroups.OPENMODEL:
                                        modelDic[outName] = openModel(paramArr);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                if (rp != null)
                                {
                                    rp.addMessage("Error in function " + func + "\n" + e.ToString());
                                }
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rp.addMessage(e.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string tsStr = ts.Days.ToString() + " days, " + ts.Minutes.ToString() + " minutes, and " + ts.Seconds.ToString() + " seconds";
                rp.stepPGBar(100);
                rp.addMessage("Fished Batch Process in " + tsStr);
                rp.enableClose();
            }
        }

        private IFunctionRasterDataset localFocalStatistics(string[] paramArr)
        {
            IFunctionRasterDataset rsDset = getRaster(paramArr[0]);
            rasterUtil.localType lt = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),paramArr[1]);
            int bBefore = System.Convert.ToInt32(paramArr[2]);
            int bAfter = System.Convert.ToInt32(paramArr[3]);
            return rsUtil.focalBandfunction(rsDset, lt, bBefore, bAfter);
        }

        private string openModel(string[] paramArr)
        {
            return paramArr[0];
        }

        private ITable openTable(string[] paramArr)
        {
            return geoUtil.getTable(paramArr[0]);
        }

        private IFeatureClass openfeatureClass(string[] paramArr)
        {
            return geoUtil.getFeatureClass(paramArr[0]);
        }

        private IFunctionRasterDataset openRaster(string[] paramArr)
        {
            return rsUtil.createIdentityRaster(paramArr[0]);
        }

        private void lBatch(string[] paramArr, string outName)
        {
            batchCalculations btch = new batchCalculations(rsUtil, rp);
            btch.BatchPath = paramArr[0];
            btch.loadBatchFile();
            btch.runBatch();
            string desc;
            object fObj;
            btch.getFinalObject(out outName, out fObj, out desc);
            if (fObj is String)
            {
                modelDic[outName] = fObj.ToString();
            }
            else if (fObj is RasterDataset)
            {
                rstDic[outName] = (IFunctionRasterDataset)fObj;
            }
            else if (fObj is FeatureClass)
            {
                ftrDic[outName] = (IFeatureClass)fObj;
            }
            else
            {
                tblDic[outName] = (ITable)fObj;
            }
        }

        private ITable createRasterAttribute(string[] paramArr)
        {
            string strRsName = paramArr[0];
            string outTblName = paramArr[1];
            IFunctionRasterDataset rs = getRaster(strRsName);
            IWorkspace wks = geoUtil.OpenWorkSpace(geoUtil.getDatabasePath(outTblName));
            string outName = System.IO.Path.GetFileNameWithoutExtension(outTblName);
            return rsUtil.calcCombinRasterFunctionTable(rs, wks, outName);
        }

        private IFunctionRasterDataset openNetCdf(string[] paramArr)
        {
            string path = paramArr[0];
            string var = paramArr[1];
            string x = paramArr[2];
            string y = paramArr[3];
            string band = paramArr[4];
            return rsUtil.returnFunctionRasterDatasetNetCDF(path, var, x, y, band);
        }

        private IFunctionRasterDataset localRescale(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            string rsTypeStr = paramArr[1];
            rasterUtil.localRescaleType rsType = (rasterUtil.localRescaleType)Enum.Parse(typeof(rasterUtil.localRescaleType), rsTypeStr);
            return rsUtil.localRescalefunction(inRs, rsType);
        }

        private IFunctionRasterDataset ndviRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            int bR = System.Convert.ToInt32(paramArr[1]);
            int bIR = System.Convert.ToInt32(paramArr[2]);
            return rsUtil.calcNDVIFunction(inRs,bR,bIR);
        }

        private IFunctionRasterDataset resampleRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            double cellSize = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.reSampleRasterFunction(inRs,cellSize);
        }

        private IFunctionRasterDataset setValuesToNull(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            string[] bmm = paramArr[1].Split(new char[]{':'});
            IStringArray sArr = new ESRI.ArcGIS.esriSystem.StrArrayClass();
            foreach (string s in bmm)
            {
                string[] mm = s.Split(new char[] { ',' });
                int min = System.Convert.ToInt32(mm[1]);
                int max = System.Convert.ToInt32(mm[2]);
                List<string> sLst = new List<string>();
                for (int j = min; j < max; j++)
                {
                    sLst.Add(j.ToString());
                }
                string ln = String.Join(" ", sLst.ToArray());
                sArr.Add(ln);
            } 
            return rsUtil.setValueRangeToNodata(inRs, sArr);
        }

        private ITable zonalFormatData(string[] paramArr)
        {
            ITable inTable = getTable(paramArr[0]);
            string linkFldName = paramArr[1];
            ITable zTbl = getTable(paramArr[2]);
            string pfr = "";
            if (paramArr.Length > 3) pfr = paramArr[3];
            FunctionRasters.zonalHelper.transformData(inTable, linkFldName, zTbl, pfr);
            return inTable;
        }

        private ITable renameField(string[] paramArr)
        {
            ITable inTable = getTable(paramArr[0]);
            ftrUtil.renameField(inTable,paramArr[1],paramArr[2]);
            return inTable;
        }

        private IFeatureClass exportFeatureClass(string[] paramArr)
        {
            IFeatureClass inputFeatureClass = getFeatureClass(paramArr[0]);
            ISpatialFilter filter = new SpatialFilterClass();
            string wC = paramArr[1];
            if (wC == null || wC == "") wC = "*";
            filter.WhereClause = wC;
            string outPath = paramArr[2];
            return ftrUtil.exportFeatures(inputFeatureClass, outPath, filter);
        }

        private ITable exportTable(string[] paramArr)
        {
            ITable inputTable = getTable(paramArr[0]);
            IQueryFilter filter = new  QueryFilterClass();
            string wC = paramArr[1];
            if (wC == null || wC == "") wC = "*";
            filter.WhereClause = wC;
            string outPath = paramArr[2];
            return ftrUtil.exportTable(inputTable, outPath, filter);
        }

        private ITable selectSamples(string[] paramArr)
        {
            ITable inTbl = getTable(paramArr[0]);
            string sFld = paramArr[1];//can be "" for no strata
            string mdl = getModelPath(paramArr[2]);
            Statistics.dataPrepBase.modelTypes mType = Statistics.ModelHelper.getModelType(mdl);
            int ns;
            int pLeng = paramArr.Length;
            bool eW = false;
            double prop = 0.1;
            double alpha = 0.05;
            if (Int32.TryParse(mdl, out ns))
            {
                //MessageBox.Show(ns.ToString());
                
                if (pLeng > 3)
                {
                    if (paramArr[3].ToLower() == "true") eW = true;
                }
                ftrUtil.selectEqualFeaturesToSample(inTbl, sFld, ns, eW);
            }
            else
            {
                switch (mType)
                {
                    case esriUtil.Statistics.dataPrepBase.modelTypes.Accuracy:
                        prop = System.Convert.ToDouble(paramArr[3]);
                        alpha = System.Convert.ToDouble(paramArr[4]);
                        if (paramArr[5].ToLower() == "true") eW = true;
                        ftrUtil.selectAccuracyFeaturesToSample(inTbl, mdl, sFld, prop, alpha, eW);
                        break;
                    case esriUtil.Statistics.dataPrepBase.modelTypes.Cluster:
                        prop = System.Convert.ToDouble(paramArr[3]);
                        alpha = System.Convert.ToDouble(paramArr[4]);
                        if (paramArr[5].ToLower() == "true") eW = true;
                        ftrUtil.selectClusterFeaturesToSample(inTbl, mdl, sFld, prop, alpha, eW);
                        break;
                    case Statistics.dataPrepBase.modelTypes.KS:
                        ITable sts = getTable(paramArr[3]);
                        ftrUtil.selectKSFeaturesToSample(inTbl, sts,mdl, sFld);
                        break;
                    case esriUtil.Statistics.dataPrepBase.modelTypes.CovCorr:
                        prop = System.Convert.ToDouble(paramArr[3]);
                        alpha = System.Convert.ToDouble(paramArr[4]);
                        if (paramArr[5].ToLower() == "true") eW = true;
                        ftrUtil.selectCovCorrFeaturesToSample(inTbl, mdl, prop, alpha);
                        break;
                    case esriUtil.Statistics.dataPrepBase.modelTypes.StrataCovCorr:
                        prop = System.Convert.ToDouble(paramArr[3]);
                        alpha = System.Convert.ToDouble(paramArr[4]);
                        if (paramArr[5].ToLower() == "true") eW = true;
                        ftrUtil.selectStrataFeaturesToSample(inTbl, mdl, sFld, prop, alpha,eW);
                        break;
                    case esriUtil.Statistics.dataPrepBase.modelTypes.PCA:
                        prop = System.Convert.ToDouble(paramArr[3]);
                        alpha = System.Convert.ToDouble(paramArr[4]);
                        if (paramArr[5].ToLower() == "true") eW = true;
                        ftrUtil.selectPcaFeaturesToSample(inTbl, mdl, prop, alpha);
                        break;
                    case esriUtil.Statistics.dataPrepBase.modelTypes.LinearRegression:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.MvlRegression:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.LogisticRegression:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.PLR:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.RandomForest:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.SoftMax:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.Cart:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.L3:
                    case esriUtil.Statistics.dataPrepBase.modelTypes.TTEST:
                    default:
                        rp.addMessage("Sample selection for this model type is not currently supported!");
                        break;
                }
            }
            return inTbl;
        }

        private string buildModels(string[] paramArr)
        {
            //first varible has model type last variable has output model name
            Statistics.dataPrepBase.modelTypes mType = (Statistics.dataPrepBase.modelTypes)Enum.Parse(typeof(Statistics.dataPrepBase.modelTypes),paramArr[0]);
            switch (mType)
            {
                case esriUtil.Statistics.dataPrepBase.modelTypes.Accuracy:
                    createAAModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.AdjustedAccuracy:
                    createAAAModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.LinearRegression:
                    createLinearRegressionModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.MvlRegression:
                    createMvlRegressionModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.LogisticRegression:
                    createLogisticRegressionModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.PLR:
                    createPlrModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.RandomForest:
                    createRfModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.SoftMax:
                    createSmModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.Cart:
                    createCartModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.L3:
                    createL3Model(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.CovCorr:
                    createCovCorrModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.PCA:
                    createPcaModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.Cluster:
                    createClusterModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.TTEST:
                    createTTestModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.PAIREDTTEST:
                    createPTTestModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.KS:
                    createKsModel(paramArr);
                    break;
                case esriUtil.Statistics.dataPrepBase.modelTypes.StrataCovCorr:
                    createStrataVarCovModel(paramArr);
                    break;
                case Statistics.dataPrepBase.modelTypes.GLM:
                    createGLMFunction(paramArr);
                    break;
                case Statistics.dataPrepBase.modelTypes.KDA:
                    createKDAFunction(paramArr);
                    break;
                case Statistics.dataPrepBase.modelTypes.LDA:
                    createLDAFunction(paramArr);
                    break;
                case Statistics.dataPrepBase.modelTypes.CompareClassifications:
                    createCompareClassificationsFunction(paramArr);
                    break;
                default:
                    break;
            }
            return paramArr[paramArr.Length - 1];

        }

        private void createLDAFunction(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepDiscriminantAnalysisLda dpLda = new Statistics.dataPrepDiscriminantAnalysisLda(table, dependentField, independentFields, categoricalFields);
            dpLda.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createKDAFunction(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepDiscriminantAnalysis.KernelType kTyp = (Statistics.dataPrepDiscriminantAnalysis.KernelType)Enum.Parse(typeof(Statistics.dataPrepDiscriminantAnalysis.KernelType),paramArr[5]);
            Statistics.dataPrepDiscriminantAnalysis dpKda = new Statistics.dataPrepDiscriminantAnalysis(table, dependentField, independentFields, categoricalFields,kTyp);
            dpKda.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createCompareClassificationsFunction(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string reference = paramArr[2];
            string mapped1 = paramArr[3];
            string mapped2 = paramArr[4];
            Statistics.dataPrepCompareClassifications dpCompareClassifications = new Statistics.dataPrepCompareClassifications(table,reference, mapped1,mapped2);
            dpCompareClassifications.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createGLMFunction(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepGlm dpGlm = new Statistics.dataPrepGlm(table, dependentField, independentFields, categoricalFields);
            dpGlm.Link = (Statistics.dataPrepGlm.LinkFunction)Enum.Parse(typeof(Statistics.dataPrepGlm.LinkFunction), paramArr[5]);
            dpGlm.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createStrataVarCovModel(string[] paramArr)
        {
            Statistics.dataPrepStrata dps = null;
            if (paramArr.Length > 4)
            {
                ITable tbl = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[] { ',' });
                string strataField = paramArr[3];
                dps = new Statistics.dataPrepStrata(tbl, variables, strataField);
            }
            else
            {
                IRaster StrataRaster = rsUtil.createRaster(getRaster(paramArr[1]));
                IRaster VariableRaster = rsUtil.createRaster(getRaster(paramArr[2]));
                dps = new Statistics.dataPrepStrata(StrataRaster, VariableRaster);
            }
            dps.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createPTTestModel(string[] paramArr)
        {
            Statistics.dataPrepPairedTTest dpTT = null;
            if (paramArr.Length > 4)
            {
                ITable tbl = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[] { ',' });
                string strataField = paramArr[3];
                dpTT = new Statistics.dataPrepPairedTTest(tbl, variables, strataField);
            }
            else
            {
                IRaster StrataRaster = rsUtil.createRaster(getRaster(paramArr[1]));
                IRaster VariableRaster = rsUtil.createRaster(getRaster(paramArr[2]));
                dpTT = new Statistics.dataPrepPairedTTest(StrataRaster, VariableRaster);
            }
            dpTT.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createTTestModel(string[] paramArr)
        {
            Statistics.dataPrepTTest dpTT = null;
            if (paramArr.Length > 4)
            {
                ITable tbl = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[]{','});
                string strataField = paramArr[3];
                dpTT = new Statistics.dataPrepTTest(tbl,variables,strataField);
            }
            else
            {
                IRaster StrataRaster = rsUtil.createRaster(getRaster(paramArr[1]));
                IRaster VariableRaster = rsUtil.createRaster(getRaster(paramArr[2]));
                dpTT = new Statistics.dataPrepTTest(StrataRaster, VariableRaster);
            }
            dpTT.writeModel(paramArr[paramArr.Length-1]);
        }

        private void createCovCorrModel(string[] paramArr)
        {
            Statistics.dataPrepVarCovCorr dpVc = null;
            if (paramArr.Length > 3)
            {
                ITable table = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[] { ',' });
                dpVc = new Statistics.dataPrepVarCovCorr(table, variables);
                
            }
            else
            {
                IRaster rs = rsUtil.createRaster(getRaster(paramArr[1]));
                dpVc = new Statistics.dataPrepVarCovCorr(rs);
            }
            dpVc.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createL3Model(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private void createCartModel(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private void createSmModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepSoftMaxPlr dpSm = new Statistics.dataPrepSoftMaxPlr(table, dependentField, independentFields, categoricalFields);
            dpSm.writeModel(paramArr[paramArr.Length-1]);
        }

        private void createPlrModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepLogisticRegression dpLg = new Statistics.dataPrepLogisticRegression(table, dependentField, independentFields, categoricalFields);
            dpLg.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createLogisticRegressionModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepMultinomialLogisticRegression dpLg = new Statistics.dataPrepMultinomialLogisticRegression(table, dependentField, independentFields, categoricalFields);
            dpLg.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createMvlRegressionModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            Statistics.dataPrepMultivariateLinearRegression dpLg = null;
            if (paramArr.Length > 5)
            {
                bool z = true;
                if (paramArr[5].ToLower() == "false") z = false;
                dpLg = new Statistics.dataPrepMultivariateLinearRegression(table, dependentField, independentFields, categoricalFields, z);
            }
            else
            {
                dpLg = new Statistics.dataPrepMultivariateLinearRegression(table, dependentField, independentFields, categoricalFields);
            }
            dpLg.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createLinearRegressionModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[]{','});
            string[] independentFields = paramArr[3].Split(new char[]{','});
            string[] categoricalFields = paramArr[4].Split(new char[]{','});
            Statistics.dataPrepLinearReg dpLg = null;
            if (paramArr.Length > 5)
            {
                bool z = true;
                if (paramArr[5].ToLower() == "false") z = false;
                dpLg = new Statistics.dataPrepLinearReg(table, dependentField, independentFields, categoricalFields, z);
            }
            else
            {
                dpLg = new Statistics.dataPrepLinearReg(table, dependentField, independentFields, categoricalFields);
            }
            dpLg.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createAAAModel(string[] paramArr)
        {
            IFeatureClass projectArea = getFeatureClass(paramArr[1]);
            IRaster mapRs = rsUtil.createRaster(getRaster(paramArr[2]));
            string orgModel = paramArr[3];
            string outModel = paramArr[4];
            Statistics.dataPrepAdjustAccuracyAssessment dpAaa = null;
            if(mapRs==null)
            {
                IFeatureClass map = getFeatureClass(paramArr[2]);
                dpAaa = new Statistics.dataPrepAdjustAccuracyAssessment(projectArea, map, orgModel, outModel);
            }
            else
            {
                dpAaa = new Statistics.dataPrepAdjustAccuracyAssessment(projectArea,mapRs,orgModel,outModel);
            }
            dpAaa.buildModel();
        }

        private void createAAModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string dependentField = paramArr[2];
            string independentField = paramArr[3];
            Statistics.dataGeneralConfusionMatirx dpAa = new Statistics.dataGeneralConfusionMatirx(table, dependentField, independentField);
            if (paramArr.Length > 5) dpAa.WeightFeild = paramArr[4];
            dpAa.writeXTable(paramArr[paramArr.Length-1]);
        }

        private void createRfModel(string[] paramArr)
        {
            ITable table = getTable(paramArr[1]);
            string[] dependentField = paramArr[2].Split(new char[] { ',' });
            string[] independentFields = paramArr[3].Split(new char[] { ',' });
            string[] categoricalFields = paramArr[4].Split(new char[] { ',' });
            int trees = System.Convert.ToInt32(paramArr[5]);
            double ratio = System.Convert.ToDouble(paramArr[6]);
            int nSplitVar = System.Convert.ToInt32(paramArr[7]);
            Statistics.dataPrepRandomForest dpRf = new Statistics.dataPrepRandomForest(table,dependentField,independentFields,categoricalFields,trees,ratio,nSplitVar);
            dpRf.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createPcaModel(string[] paramArr)
        {
            Statistics.dataPrepPrincipleComponents dpPca = null;
            if (paramArr.Length < 4)
            {
                IRaster rs = rsUtil.createRaster(getRaster(paramArr[1]));
                dpPca = new Statistics.dataPrepPrincipleComponents(rs);
            }
            else
            {
                ITable table = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[] { ',' });
                dpPca = new Statistics.dataPrepPrincipleComponents(table, variables);
            }
            dpPca.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createClusterModel(string[] paramArr)
        {

            Statistics.dataPrepClusterBase dpC = null;
            esriUtil.Statistics.clusterType cType = Statistics.clusterType.KMEANS;
            if(paramArr.Length<6)
            {
                IRaster rs = rsUtil.createRaster(getRaster(paramArr[1]));
                int nCls = System.Convert.ToInt32(paramArr[2]);
                cType = (esriUtil.Statistics.clusterType)Enum.Parse(typeof(esriUtil.Statistics.clusterType), paramArr[3]);
                switch (cType)
                {
                    case esriUtil.Statistics.clusterType.KMEANS:
                        dpC = (Statistics.dataPrepClusterBase)(new Statistics.dataPrepClusterKmean(rs, nCls));
                        break;
                    case esriUtil.Statistics.clusterType.BINARY:
                        dpC = (Statistics.dataPrepClusterBase)(new Statistics.dataPrepClusterBinary(rs, nCls));
                        break;
                    case esriUtil.Statistics.clusterType.GAUSSIANMIXTURE:
                        dpC = (Statistics.dataPrepClusterBase)(new Statistics.dataPrepClusterGaussian(rs, nCls));
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                ITable tbl = getTable(paramArr[1]);
                string[] flds = paramArr[2].Split(new char[]{','});
                int nCls = System.Convert.ToInt32(paramArr[3]);
                cType = (esriUtil.Statistics.clusterType)Enum.Parse(typeof(esriUtil.Statistics.clusterType), paramArr[4]);
                dpC = new Statistics.dataPrepClusterKmean(tbl,flds,nCls);
            }
            dpC.writeModel(paramArr[paramArr.Length - 1]);
        }

        private void createKsModel(string[] paramArr)
        {
            ITable sample1 = getTable(paramArr[1]);
            ITable sample2 = getTable(paramArr[2]);
            string[] explanitoryVariables = paramArr[3].Split(new char[] { ',' });
            string strataField = "";
            if (paramArr.Length > 5) strataField = paramArr[4];
            Statistics.dataPrepCompareSamples dpKs = new Statistics.dataPrepCompareSamples(sample1, sample2, explanitoryVariables, strataField);
            dpKs.writeModel(paramArr[paramArr.Length - 1]);
        }

        private IFunctionRasterDataset createNullToValueRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.setnullToValueFunction(inRs, d);
        }

        private IFunctionRasterDataset createShiftRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            double shiftX = System.Convert.ToDouble(paramArr[1]);
            double shiftY = System.Convert.ToDouble(paramArr[2]);
            return rsUtil.shiftRasterFunction(inRs,shiftX,shiftY);
        }

        private IFunctionRasterDataset createRotateRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.RotateRasterFunction(inRs, d);
        }

        private IFunctionRasterDataset createConstantRaster(string[] paramArr)
        {
            IFunctionRasterDataset inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.constantRasterFunction(inRs, d);
        }

        private IFunctionRasterDataset createCombineFunction(string[] paramArr)
        {
            IRaster[] inRasters = new IRaster[paramArr.Length];
            for (int i = 0; i < paramArr.Length; i++)
            {
                inRasters[i] = rsUtil.createRaster(getRaster(paramArr[i]));
            }
            return rsUtil.calcCombineRasterFunction(inRasters);
        }

        private IFunctionRasterDataset createSurfaceFunction(string[] paramArr)
        {
            IRaster rs = rsUtil.createRaster(getRaster(paramArr[0]));
            string sTypeStr = paramArr[1];
            rasterUtil.surfaceType sType = (rasterUtil.surfaceType)Enum.Parse(typeof(rasterUtil.surfaceType), sTypeStr);
            IFunctionRasterDataset oRs = null;
            switch (sType)
            {
                case rasterUtil.surfaceType.SLOPE:
                    oRs = rsUtil.calcSlopeFunction(rs);
                    break;
                case rasterUtil.surfaceType.ASPECT:
                    oRs = rsUtil.calcAspectFunction(rs);
                    break;
                case rasterUtil.surfaceType.EASTING:
                    oRs = rsUtil.calcEastWestFunction(rs);
                    break;
                case rasterUtil.surfaceType.NORTHING:
                    oRs = rsUtil.calcNorthSouthFunction(rs);
                    break;
                case rasterUtil.surfaceType.FLIP:
                    oRs = rsUtil.flipRasterFunction(rs);
                    break;
                default:
                    break;
            }
            return oRs;
        }

        private ITable createZonalClassCounts(string[] paramArr)
        {
            if (paramArr.Length < 4)
            {
                IFunctionRasterDataset rs1 = getRaster(paramArr[0]);
                IFunctionRasterDataset rs2 = getRaster(paramArr[1]);
                return rsUtil.zonalStats(rs1, rs2, paramArr[2], null, rp,true);
            }
            else
            {
                IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
                string inFtrFld = paramArr[1];
                IFunctionRasterDataset vRs = getRaster(paramArr[2]);
                return rsUtil.zonalStats(inFtrCls, inFtrFld, vRs, paramArr[3], null, rp,true);
            }
        }

        private IFunctionRasterDataset createAggregationFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            int cells = System.Convert.ToInt32(paramArr[1]);
            rasterUtil.focalType ftype = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[2]);
            foreach (string s in rstStr.Split(new char[] { ',' }))
            {
                IFunctionRasterDataset rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return rsUtil.calcAggregationFunction((IRaster)rsBC,cells,ftype);
        }

        private ITable predictNewValues(string[] paramArr)
        {
            string tblStr = paramArr[0];
            ITable tbl = getTable(tblStr);
            string mPath = getModelPath(paramArr[1]);
            IQueryFilter qf = new QueryFilterClass();
            if (paramArr.Length > 2)
            {
                qf.WhereClause = paramArr[2];
            }
            Statistics.ModelHelper mH = new Statistics.ModelHelper(mPath);
            mH.predictNewData(tbl,qf);
            return tbl;
        }

        private IFunctionRasterDataset createModelFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            string mPath = getModelPath(paramArr[1]);
            foreach (string s in rstStr.Split(new char[]{','}))
            {
                IFunctionRasterDataset rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            IFunctionRasterDataset fDset = rsUtil.compositeBandFunction(rsBC);
            Statistics.ModelHelper mH = new Statistics.ModelHelper(mPath, rsUtil.createRaster(fDset), rsUtil);
            return rsUtil.createIdentityRaster(mH.getRaster());
        }

        private IFunctionRasterDataset convertPixelType(string[] paramArr)
        {
            IFunctionRasterDataset inRaster = getRaster(paramArr[0]);
            rstPixelType pType = (rstPixelType)Enum.Parse(typeof(rstPixelType),paramArr[1]);
            return rsUtil.convertToDifFormatFunction(inRaster, pType);
        }

        private IFunctionRasterDataset createFocalSampleFunction(string[] paramArr)
        {
            IFunctionRasterDataset inRaster = getRaster(paramArr[0]);
            HashSet<string> offset = new HashSet<string>();
            foreach(string s in paramArr[1].Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries))//azimuth:distance -> azimuth;distance
            {
                offset.Add(s.Trim().Replace(":",";"));
            }
            rasterUtil.focalType statType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType),paramArr[2]);
            return rsUtil.calcFocalSampleFunction(inRaster, offset, statType);
        }

        private IFeatureClass clusterSampleRaster(string[] paramArr)
        {
            IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
            IRaster sampleRst = rsUtil.createRaster(getRaster(paramArr[1]));
            string inName = paramArr[2];
            if (inName == "" || inName == "null")
            {
                inName = null;
            }
            Dictionary<double, double> azmithDistance = new Dictionary<double, double>();
            foreach (string s in paramArr[3].Split(new char[] { ',' }))
            {
                string[] aA = s.Split(new char[] { ':' });
                azmithDistance.Add(System.Convert.ToDouble(aA[0]), System.Convert.ToDouble(aA[1]));
            }
            rasterUtil.clusterType typeOfCluster = (rasterUtil.clusterType)Enum.Parse(typeof(rasterUtil.clusterType),paramArr[4]);
            rsUtil.sampleRaster(inFtrCls, sampleRst, inName, azmithDistance, typeOfCluster);
            return inFtrCls;
        }

        private IFeatureClass createStratifiedRandomSample(string[] paramArr)
        {
            IWorkspace wks = geoUtil.OpenWorkSpace(paramArr[0]);
            IFunctionRasterDataset rasterPath = getRaster(paramArr[1]);
            int[] sampleSizePerClass = (from string s in paramArr[2].Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
            string outName = paramArr[3];
            return rsUtil.createRandomSampleLocationsByClass(wks, rasterPath, sampleSizePerClass, 1, outName);
        }

        private IFeatureClass createRandomSample(string[] paramArr)
        {
            IWorkspace wks = geoUtil.OpenWorkSpace(paramArr[0]);
            IFunctionRasterDataset rasterPath = getRaster(paramArr[1]);
            int sampleSize = System.Convert.ToInt32(paramArr[2]);
            string outName = paramArr[3];
            return rsUtil.createRandomSampleLocations(wks, rasterPath, sampleSize, outName);
        }

        private IFeatureClass sampleRaster(string[] paramArr)
        {
            IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
            IFunctionRasterDataset sampleRst = getRaster(paramArr[1]);
            string inName = paramArr[2];
            if (inName == "" || inName == "null")
            {
                inName = null;
            }
            string bndFldName = null;
            if (paramArr.Length > 3) bndFldName = paramArr[3];
            rsUtil.sampleRaster(inFtrCls, sampleRst, inName, bndFldName);
            return inFtrCls;
        }

        private IFunctionRasterDataset createMerge(string[] paramArr)
        {
            string rsStr = paramArr[0];
            string[] rsStrArr = rsStr.Split(new char[]{','});
            IRaster[] rstArr = new IRaster[rsStrArr.Length];
            for (int i = 0; i < rsStrArr.Length; i++)
			{
                rstArr[i] = rsUtil.createRaster(getRaster(rsStrArr[i]));
			}
            rasterUtil.mergeType mType = (rasterUtil.mergeType)Enum.Parse(typeof(rasterUtil.mergeType), paramArr[1]);
            return rsUtil.calcMosaicFunction(rstArr, mType);
        }

        private IFunctionRasterDataset createMosaic(string[] paramArr)
        {
            IWorkspace wks = geoUtil.OpenRasterWorkspace(paramArr[0]);
            string mosiacName = paramArr[1];
            string rsStr = paramArr[2];
            string[] rsStrArr = rsStr.Split(new char[] { ',' });
            IRaster[] rstArr = new IRaster[rsStrArr.Length];
            for (int i = 0; i < rsStrArr.Length; i++)
            {
                rstArr[i] = rsUtil.createRaster(getRaster(rsStrArr[i]));
            }
            esriMosaicMethod mosaicmethod = (esriMosaicMethod)Enum.Parse(typeof(esriMosaicMethod), paramArr[3]);
            rstMosaicOperatorType mosaictype = (rstMosaicOperatorType)Enum.Parse(typeof(rstMosaicOperatorType), paramArr[4]);
            bool buildfootprint = System.Convert.ToBoolean(paramArr[5]);
            bool buildboundary = System.Convert.ToBoolean(paramArr[6]);
            bool seemlines = System.Convert.ToBoolean(paramArr[7]);
            bool buildOverview = System.Convert.ToBoolean(paramArr[8]);
            return rsUtil.createIdentityRaster(rsUtil.mosaicRastersFunction(wks, mosiacName, rstArr, mosaicmethod,mosaictype, buildfootprint, buildboundary, seemlines, buildOverview));
        }

        private ITable buildRasterVat(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            IRasterDataset rsDset = (IRasterDataset)rs;
            string tblName = "";
            IWorkspace rsWks = null;
            if(paramArr.Length>1)
            {
                rsWks = geoUtil.OpenWorkSpace(paramArr[1]);
                tblName = paramArr[2] + "_Vat";
            }
            else
            {
                IDataset ds = (IDataset)rsDset;
                rsWks = ds.Workspace;
                tblName = ds.Name + "_Vat";
            }
            Dictionary<int,int> vlDic = rsUtil.buildVat(rs);
            ITable vatTbl = rsUtil.convertDicToVat(vlDic,rsWks,tblName);
            rsUtil.appendVatToRasterDataset(vatTbl,rsDset);
            return vatTbl;

        }

        private IFunctionRasterDataset buildRasterStats(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            IRaster rs2 = rsUtil.createRaster(rs);
            int skipFactor = System.Convert.ToInt32(paramArr[1]);
            return rsUtil.createIdentityRaster(rsUtil.calcStatsAndHist(rs2, skipFactor));
            
        }

        private IFunctionRasterDataset saveRaster(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            IRaster rs2 = rsUtil.createRaster(rs);
            string outName = paramArr[1];
            IWorkspace wks = geoUtil.OpenRasterWorkspace(paramArr[2]);
            rasterUtil.rasterType rastertype = (rasterUtil.rasterType)Enum.Parse(typeof(rasterUtil.rasterType),paramArr[3].ToUpper());
            object noDataVl = null;
            int Bwidth = 512;
            int Bheigth = 512;
            if (paramArr.Length > 4)
            {
                double tD;
                if (Double.TryParse(paramArr[4], out tD))
                {
                    noDataVl = tD;
                }
                else noDataVl = null;
            }
            if(paramArr.Length>5)Bwidth = System.Convert.ToInt32(paramArr[5]);
            if(paramArr.Length>6)Bheigth = System.Convert.ToInt32(paramArr[6]);
            return rsUtil.createIdentityRaster(rsUtil.saveRasterToDatasetM(rs2, outName, wks, rastertype,noDataVl,Bwidth,Bheigth));
        }

        private IFunctionRasterDataset createLandscapeFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            if (paramArr.Length < 4)
            {
                int radius = System.Convert.ToInt32(paramArr[1]);
                rasterUtil.focalType statType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[2]);
                rasterUtil.landscapeType landType = (rasterUtil.landscapeType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[3]);
                return rsUtil.calcLandscapeFunction(rs, radius, statType, landType);
            }
            else
            {
                int clm = System.Convert.ToInt32(paramArr[1]);
                int rws = System.Convert.ToInt32(paramArr[2]);
                rasterUtil.focalType statType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[3]);
                rasterUtil.landscapeType landType = (rasterUtil.landscapeType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[4]);
                return rsUtil.calcLandscapeFunction(rs, clm, rws, statType, landType);
            }
            

        }

        private IFunctionRasterDataset createGLCMFunction(string[] paramArr)
        {
            IFunctionRasterDataset inRaster = getRaster(paramArr[0]);
            bool horizontal = true;
            rasterUtil.glcmMetric glcmType = rasterUtil.glcmMetric.CONTRAST;
            if (paramArr.Length <5)
            {
                int radius = System.Convert.ToInt32(paramArr[1]);
                string horz = paramArr[2].ToLower();
                if (horz == "false")
                {
                    horizontal = false;
                }
                glcmType = (rasterUtil.glcmMetric)Enum.Parse(typeof(rasterUtil.glcmMetric), paramArr[3].ToUpper());
                return rsUtil.fastGLCMFunction(inRaster, radius, horizontal, glcmType);
            }
            else
            {
                int clms = System.Convert.ToInt32(paramArr[1]);
                int rws = System.Convert.ToInt32(paramArr[2]);
                string horz = paramArr[3].ToLower();
                if (horz == "false")
                {
                    horizontal = false;
                }
                glcmType = (rasterUtil.glcmMetric)Enum.Parse(typeof(rasterUtil.glcmMetric), paramArr[4].ToUpper());
                return rsUtil.fastGLCMFunction(inRaster, clms, rws, horizontal, glcmType);
            }
        }

        private IFunctionRasterDataset createExtractFunction(string[] paramArr)
        {
            IFunctionRasterDataset inRaster = getRaster(paramArr[0]);
            ILongArray lArr = new LongArrayClass();
            foreach (string s in paramArr[1].Split(new char[]{','}))
            {
                int bnd = System.Convert.ToInt32(s.Trim()) - 1;
                lArr.Add(bnd);
            }
            return rsUtil.getBands(inRaster,lArr);
        }

        private IFunctionRasterDataset createCompositeFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            foreach(string s in paramArr)
            {
                IFunctionRasterDataset rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return rsUtil.compositeBandFunction(rsBC);
        }

        private IFunctionRasterDataset createRemapFunction(string[] paramArr)
        {
            IFunctionRasterDataset inRaster = getRaster(paramArr[0]);
            IRemapFilter flt = new RemapFilterClass();
            foreach (string s in paramArr[1].Split(new char[] { ',' }))
            {
                double[] rVls = (from string s2 in (s.Split(new char[]{':'})) select System.Convert.ToDouble(s2)).ToArray();
                flt.AddClass(rVls[0], rVls[1], rVls[2]);
            }
            return rsUtil.calcRemapFunction(inRaster,flt);
        }

        private IFunctionRasterDataset createRescaleFunction(string[] paramArr)
        {
            IFunctionRasterDataset fDset = getRaster(paramArr[0]);
            rstPixelType rType = rstPixelType.PT_UCHAR;
            esriRasterStretchType sType = esriRasterStretchType.esriRasterStretchMinimumMaximum;
            double[] min = null;
            double[] max = null;
            double[] mean = null;
            double[] std = null;
            if (paramArr.Length > 1) rType = (rstPixelType)Enum.Parse(typeof(rstPixelType), paramArr[1]);
            if (paramArr.Length > 2)
            {
                min = (from string s in paramArr[2].Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                max = (from string s in paramArr[3].Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                mean = (from string s in paramArr[4].Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                std = (from string s in paramArr[5].Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
            }
            return rsUtil.reScaleRasterFunction(fDset,rType,sType,min,max,mean,std);
        }

        private IFunctionRasterDataset createLinearTransformFunction(string[] paramArr)
        {
            string rrStr = paramArr[0];
            IRasterBandCollection rsBC = new RasterClass();
            foreach (string s in rrStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                rsBC.AppendBands((IRasterBandCollection)getRaster(s.Trim()));
            }
            float[] s1 = (from string s in (paramArr[1].Split(new char[]{','})) select System.Convert.ToSingle(s)).ToArray();
            List<float[]> slopes = new List<float[]>();
            slopes.Add(s1);
            return rsUtil.calcRegressFunction((IRaster)rsBC, slopes);
        }

        private IFunctionRasterDataset createLocalFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            rasterUtil.localType lType = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),paramArr[1]);
            foreach (string s in rstStr.Split(new char[] { ',' }))
            {
                IFunctionRasterDataset rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return rsUtil.localStatisticsfunction((IRaster)rsBC,lType);
        }

        private IFunctionRasterDataset createFocalFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            string fTypeStr = null;
            if (paramArr.Length < 4)
            {
                fTypeStr = paramArr[2];
                rasterUtil.focalType fType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), fTypeStr);
                int rad = System.Convert.ToInt32(paramArr[1]);
                return rsUtil.calcFocalStatisticsFunction(rs, rad, fType);
            }
            else
            {
                fTypeStr = paramArr[3];
                rasterUtil.focalType fType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), fTypeStr);
                int clms = System.Convert.ToInt32(paramArr[1]);
                int rws = System.Convert.ToInt32(paramArr[2]);
                return rsUtil.calcFocalStatisticsFunction(rs, clms, rws, fType);
            }
        }

        private IFunctionRasterDataset createConvolutionFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            int wd = System.Convert.ToInt32(paramArr[1]);
            int ht = System.Convert.ToInt32(paramArr[2]);
            string krn = paramArr[3];
            List<double> dblLst = new List<double>();
            foreach (string s in krn.Split(new char[] { ',' }))
            {
                dblLst.Add(System.Convert.ToDouble(s));
            }
            return rsUtil.convolutionRasterFunction(rs, wd, ht, dblLst.ToArray());
        }

        private IFunctionRasterDataset createConditionalFunction(string[] paramArr)
        {
            string inRs1Str = paramArr[1];
            string inRs2Str = paramArr[2];
            object inRs1 = null;
            object inRs2 = null;
            if (rsUtil.isNumeric(inRs1Str))
            {
                inRs1 = inRs1Str;
            }
            else
            {
                inRs1 = getRaster(inRs1Str);
            }
            if (rsUtil.isNumeric(inRs2Str))
            {
                inRs2 = inRs2Str;
            }
            else
            {
                inRs2 = getRaster(inRs2Str);
            }
            IFunctionRasterDataset conRs = getRaster(paramArr[0]);
            return rsUtil.conditionalRasterFunction(conRs, inRs1, inRs2);
        }

        private IFunctionRasterDataset createClipFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            IFeatureClass ftrCls = getFeatureClass(paramArr[1]);
            IGeometry geo = ((IGeoDataset)ftrCls).Extent;
            return rsUtil.clipRasterFunction(rs, geo, esriRasterClippingType.esriRasterClippingOutside);
        }

        private IFunctionRasterDataset createLogicalFunction(string[] paramArr)
        {
            rasterUtil.logicalType opType = (rasterUtil.logicalType)Enum.Parse(typeof(rasterUtil.logicalType),paramArr[2].ToUpper());
            string p1 = paramArr[0];
            string p2 = paramArr[1];
            object inRs1 = null;
            object inRs2 = null;
            if (rsUtil.isNumeric(p1))
            {
                inRs1 = p1;
            }
            else
            {
                inRs1 = getRaster(p1);
            }
            if (rsUtil.isNumeric(p2))
            {
                inRs2 = p2;
            }
            else
            {
                inRs2 = getRaster(p2);
            }
            IFunctionRasterDataset rs = null;
            switch (opType)
            {
                case rasterUtil.logicalType.GT:
                    rs = rsUtil.calcGreaterFunction(inRs1, inRs2);
                    break;
                case rasterUtil.logicalType.LT:
                    rs = rsUtil.calcLessFunction(inRs1, inRs2);
                    break;
                case rasterUtil.logicalType.GE:
                    rs = rsUtil.calcGreaterEqualFunction(inRs1, inRs2);
                    break;
                case rasterUtil.logicalType.LE:
                    rs = rsUtil.calcLessEqualFunction(inRs1, inRs2);
                    break;
                case rasterUtil.logicalType.EQ:
                    rs = rsUtil.calcEqualFunction(inRs1, inRs2);
                    break;
                case rasterUtil.logicalType.AND:
                    rs = rsUtil.calcAndFunction(inRs1, inRs2);
                    break;
                default:
                    rs = rsUtil.calcOrFunction(inRs1, inRs2);
                    break;
            }
            return rs;
        }

        private IFunctionRasterDataset createSetNullFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            IStringArray strArray = new StrArrayClass();
            string vlStr = paramArr[1];
            string[] vlArr = vlStr.Split(new char[] { ',' });
            foreach (string s in vlArr)
            {
                string[] rngArr = s.Split(new char[] { '-' });
                int min = System.Convert.ToInt32(rngArr[0]);
                int max = System.Convert.ToInt32(rngArr[1]);
                List<string> ndVlsLst = new List<string>(); 
                for (int i = min; i < max; i++)
                {
                    ndVlsLst.Add(i.ToString());
                }
                strArray.Add(String.Join(" ", ndVlsLst.ToArray()));
            }
            return rsUtil.setValueRangeToNodata(rs,strArray);
        }

        private IFunctionRasterDataset createMathFunction(string[] paramArr)
        {
            IFunctionRasterDataset rs = getRaster(paramArr[0]);
            rasterUtil.transType tType = (rasterUtil.transType)Enum.Parse(typeof(rasterUtil.transType), paramArr[1]);
            return rsUtil.calcMathRasterFunction(rs, tType);
            
        }
        private ITable createZonalStats(string[] paramArr)
        {
            string zTStr = paramArr[paramArr.Length-1];
            string[] zTStrArr = zTStr.Split(new char[]{','});
            List<rasterUtil.zoneType> zLst = new List<rasterUtil.zoneType>();
            foreach(string s in zTStrArr)
            {
                rasterUtil.zoneType zT = (rasterUtil.zoneType)Enum.Parse(typeof(rasterUtil.zoneType),s.ToUpper());
                zLst.Add(zT);
            }
            if(paramArr.Length<5)
            {
                IFunctionRasterDataset rs1 = getRaster(paramArr[0]);
                IFunctionRasterDataset rs2 = getRaster(paramArr[1]);
                return rsUtil.zonalStats(rs1, rs2,paramArr[2], zLst.ToArray(),rp);
            }
            else
            {
                IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
                string inFtrFld = paramArr[1];
                IRaster vRs = rsUtil.createRaster(getRaster(paramArr[2]));
                return rsUtil.zonalStats(inFtrCls, inFtrFld, vRs, paramArr[3],zLst.ToArray(),rp);
            }
        }

        private IFeatureClass getFeatureClass(string p)
        {
            string tp = p.Trim();
            if (ftrDic.ContainsKey(tp))
            {
                return ftrDic[tp];
            }
            else
            {
                IFeatureClass outFtrCls = geoUtil.getFeatureClass(tp);
                return outFtrCls;
            }
        }

        private ITable getTable(string p)
        {
            string tp = p.Trim();
            if (tblDic.ContainsKey(tp))
            {
                return tblDic[tp];
            }
            else if (ftrDic.ContainsKey(tp))
            {
                return (ITable)ftrDic[tp];
            }
            else
            {
                ITable outTbl = geoUtil.getTable(tp);
                return outTbl;
            }
        }

        private string getModelPath(string p)
        {
            string tp = p.Trim();
            if (modelDic.ContainsKey(tp))
            {
                return modelDic[tp];
            }
            else
            {
                return p;
            }
        }

        private IFunctionRasterDataset createArithmeticFunction(string[] paramArr)
        {
            string inRs1Str = paramArr[0];
            string inRs2Str = paramArr[1];
            object inRs1 = null;
            object inRs2 = null;
            if (rsUtil.isNumeric(inRs1Str))
            {
                inRs1 = inRs1Str;
            }
            else
            {
                inRs1 = getRaster(inRs1Str);
            }
            if (rsUtil.isNumeric(inRs2Str))
            {
                inRs2 = inRs2Str;
            }
            else
            {
                inRs2 = getRaster(inRs2Str);
            }
            esriRasterArithmeticOperation op = (esriRasterArithmeticOperation)Enum.Parse(typeof(esriRasterArithmeticOperation), paramArr[2]);
            return rsUtil.calcArithmaticFunction(inRs1, inRs2, op);
        }

        private IFunctionRasterDataset getRaster(string p)
        {
            string tp = p.Trim();
            if (rstDic.ContainsKey(tp))
            {
                return rstDic[tp];
            }
            else
            {
                IFunctionRasterDataset outRs = rsUtil.createIdentityRaster(tp);
                return outRs;
            }
        }

        public void getFunctionAndParameters(string ln, out string func, out string param, out string outName)
        {
            func = "";
            param = "";
            outName = "";
            try
            {
                string[] lnArr = ln.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                outName = lnArr[0].Trim();
                List<string> allLnsLst = new List<string>();
                for (int i = 1; i < lnArr.Length; i++)
                {
                    allLnsLst.Add(lnArr[i]);
                }
                string lnS = String.Join("=",allLnsLst.ToArray()).Trim();
                int lP = lnS.IndexOf("(");
                int rP = lnS.IndexOf(")");
                func = lnS.Substring(0, lP).ToUpper();
                param = lnS.Substring(lP+1, lnS.Length - (lP+2));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void syntaxExample(batchCalculations.batchGroups batchFunction)
        {
            string msg = "working on this";
            switch (batchFunction)
            {
                case batchGroups.ARITHMETIC:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;arithmeticFunction)";
                    break;
                case batchGroups.MATH:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;mathFunction)";
                    break;
                case batchGroups.SETNULL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;0-4,8-9,13-15)";
                    break;
                case batchGroups.LOGICAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;logicalOperator)";
                    break;
                case batchGroups.CLIP:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;inFeatureClass)";
                    break;
                case batchGroups.CONDITIONAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;in_Raster3)";
                    break;
                case batchGroups.CONVOLUTION:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;0,1,0,1,0,1,0,1,0)";
                    break;
                case batchGroups.FOCAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;focalStat)\noutRS = " + batchFunction.ToString() + "(in_Raster1;radius;focalStat)";
                    break;
                case batchGroups.LOCALSTATISTICS:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster1,inRaster2;localType)";
                    break;
                case batchGroups.LINEARTRANSFORM:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;betas{0.12,2.25,1,6.3}; intercept should be the first number)";
                    break;
                case batchGroups.RESCALE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;rstPixelType;min1,min2,min3;max1,max2,max3;mean1,mean2,mean3;std1,std2,std3)";
                    break;
                case batchGroups.REMAP:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;0:1:100,1:6:200,6:1000:300)";
                    break;
                case batchGroups.COMPOSITE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;inRaster2;inRaster3)";
                    break;
                case batchGroups.EXTRACTBAND:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;1,2,5)";
                    break;
                case batchGroups.GLCM:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;horizontal(true,false);glcmMetric)\noutRS = " + batchFunction.ToString() + "(in_Raster1;radius;horizontal(true,false);glcmMetric)";
                    break;
                case batchGroups.LANDSCAPE:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;focalType;landType)\noutRS = " + batchFunction.ToString() + "(in_Raster1;radius;focalType;landType)";
                    break;
                case batchGroups.ZONALSTATS:
                    msg = "outTbl = " + batchFunction.ToString() + "(ZoneRaster;ValueRaster;OutTableName;MAX,MIN,SUM)\noutTbl = " + batchFunction.ToString() + "(ZoneFeatureClass;ZoneField;ValueRaster2;OutTableName;MAX,MIN,SUM)";
                    break;
                case batchGroups.ZONALCLASSCOUNTS:
                    msg = "outTbl = " + batchFunction.ToString() + "(ZoneRaster;ValueRaster;OutTableName)\noutTbl = " + batchFunction.ToString() + "(ZoneFeatureClass;ZoneField;ValueRaster2;OutTableName)";
                    break; 
                case batchGroups.SAVEFUNCTIONRASTER:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;outName;outWorkspace;rasterType)";
                    break;
                case batchGroups.BUILDRASTERSTATS:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;skipFactor)";
                    break;
                case batchGroups.BUILDRASTERVAT:
                    msg = "outTable = " + batchFunction.ToString() + "(in_Raster)";
                    break;
                case batchGroups.MOSAIC:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(wks; mosaicDatasetName; inRaster1,inRaster2,inRaster3; mosaicmethod; mosaictype; buildfootprint; buildboundary; seemlines; buildOverview)";
                    break;
                case batchGroups.MERGE:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(inRaster1,inRaster2,inRaster3;mergeType)";
                    break;
                case batchGroups.SAMPLERASTER:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(inFeatureClass;sampleRaster;FieldNamePrefix)";
                    break;
                case batchGroups.CLUSTERSAMPLERASTER:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(inFeatureClass;sampleRaster;FieldNamePrefix;15:56,180:56,274:75;typeOfCluster)";
                    break;
                case batchGroups.CREATERANDOMSAMPLE:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(inFeatureClass;sampleRaster;100;OutName)";
                    break;
                case batchGroups.CREATESTRATIFIEDRANDOMSAMPLE:
                    msg = "outFtrClass = " + batchFunction.ToString() + "(inFeatureClass;sampleRaster;20,65,75,10;OutName)";
                    break;
                case batchGroups.FOCALSAMPLE:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;360:37.56,120:26.25,240:2;focalType)";
                    break;
                case batchGroups.CONVERTPIXELTYPE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;rstPixelType)";
                    break;
                case batchGroups.MODEL:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;modelPath)";
                    break;
                case batchGroups.AGGREGATION:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;cells;Aggregation Type)";
                    break;
                case batchGroups.PREDICT:
                    msg = "outTable = " + batchFunction.ToString() + "(inTable;modelPath)";
                    break;
                case batchGroups.BUILDMODEL:
                    msg = "outModel = " + batchFunction.ToString() + "(modelType;model parameters)\nRandomForest Example:\n\toutModel = " + batchFunction.ToString() + "(RandomForest;featureClassPath;DependentField;IndependentFields(comma separated);categoricalFields(comma separated);Trees;Ratio;NumberOfSplitVariables;outputModelPath)";
                    break;
                case batchGroups.COMBINE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;inRaster2;inRaster3)";
                    break;
                case batchGroups.CONSTANT:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;constantValue)";
                    break;
                case batchGroups.ROTATE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;degrees)";
                    break;
                case batchGroups.SHIFT:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;CellsX,CellsY)";
                    break;
                case batchGroups.NULLTOVALUE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;newValue)";
                    break;
                case batchGroups.SELECTSAMPLES:
                    msg = "outTbl = " + batchFunction.ToString() + "(inTable;inStatumField;inModel;Proportion;Alpha;false)";
                    break;
                case batchGroups.EXPORTTABLE:
                    msg = "outTbl = " + batchFunction.ToString() + "(inTable;query;outTablePath)";
                    break;
                case batchGroups.EXPORTFEATURECLASS:
                    msg = "outFeatureClass = " + batchFunction.ToString() + "(inFeatureClass;query;outFeatureClassPath)";
                    break;
                case batchGroups.SURFACE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;surfaceType)";
                    break;
                case batchGroups.RENAMEFIELD:
                    msg = "outTbl = " + batchFunction.ToString() + "(inTable;oldFieldName;newFieldName)";
                    break;
                case batchGroups.FORMATZONAL:
                    msg = "outTbl = " + batchFunction.ToString() + "(inZoneTable;linkFieldName;inZonalSummaryTable;FieldPrefix)";
                    break;
                case batchGroups.SETVALUESTONULL:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;Band1,min1,max1:Band2,min2,max2:Band3,min3,max3)";
                    break;
                case batchGroups.RESAMPLE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;cellSizeInMapUnits)";
                    break;
                case batchGroups.NDVI:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;redBand;irBand)";
                    break;
                case batchGroups.LOCALRESCALE:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;rasterUtil.localRescaleType)";
                    break;
                case batchGroups.CREATERASTERATTRIBUTE:
                    msg = "outTBL = " + batchFunction.ToString() + "(inRaster;outTablePath)";
                    break;
                case batchGroups.LOCALFOCALSTATISTICS:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;statistic;bandsBefore;bandsAfter)";
                    break;
                case batchGroups.LOAD:
                    msg = "outObject = " + batchFunction.ToString() + "(BatchPath)";
                    break;
                case batchGroups.OPENRASTER:
                    msg = "outRs = " + batchFunction.ToString() + "(RasterPath)";
                    break;
                case batchGroups.OPENFEATURECLASS:
                    msg = "outFtr = " + batchFunction.ToString() + "(FeatureClassPath)";
                    break;
                case batchGroups.OPENTABLE:
                    msg = "outTBL = " + batchFunction.ToString() + "(TablePath)";
                    break;
                case batchGroups.OPENMODEL:
                    msg = "outMDL = " + batchFunction.ToString() + "(ModelPath)";
                    break;
                default:
                    break;
            }
            System.Windows.Forms.MessageBox.Show(msg);
        }
        private string lstName = null;
        public void getFinalObject(out string nm, out object finalObject, out string desc)
        {
            nm="";
            finalObject = null;
            desc = "";
            if (rstDic.Keys.Contains(lstName))
            {
                IFunctionRasterDataset rs;
                GetFinalRaster(out nm, out rs, out desc);
                finalObject = rs;
                
            }
            else if (ftrDic.Keys.Contains(lstName))
            {
                IFeatureClass ftrCls;
                GetFinalFeatureClass(out nm, out ftrCls, out desc);
                finalObject = ftrCls;
            }
            else
            {
                ITable tbl;
                GetFinalTable(out nm, out tbl, out desc);
                finalObject = tbl;
            }
        }
        public void GetFinalRaster(out string nm, out IFunctionRasterDataset rs, out string desc)
        {
            nm = "";
            rs = null;
            desc = "";
            //System.Windows.Forms.MessageBox.Show("LstName = " + lstName+ " and has dictionary value = " + rstDic.ContainsKey(lstName).ToString());
            if (rstDic.ContainsKey(lstName))
            {
                nm = lstName;
                rs = rstDic[lstName];
                desc = lnLst[lnLst.Count - 1];
            }
        }
        public void GetFinalFeatureClass(out string nm, out IFeatureClass fc, out string desc)
        {
            nm = "";
            fc = null;
            desc = "";
            //System.Windows.Forms.MessageBox.Show("LstName = " + lstName+ " and has dictionary value = " + rstDic.ContainsKey(lstName).ToString());
            if (rstDic.ContainsKey(lstName))
            {
                nm = lstName;
                fc = ftrDic[lstName];
                desc = lnLst[lnLst.Count - 1];
            }
        }
        public void GetFinalTable(out string nm, out ITable tb, out string desc)
        {
            nm = "";
            tb = null;
            desc = "";
            //System.Windows.Forms.MessageBox.Show("LstName = " + lstName+ " and has dictionary value = " + rstDic.ContainsKey(lstName).ToString());
            if (rstDic.ContainsKey(lstName))
            {
                nm = lstName;
                tb = tblDic[lstName];
                desc = lnLst[lnLst.Count - 1];
            }
            
        }
        public void ClearRasters()
        {
            rstDic.Clear();
        }
        public void ClearTables()
        {
            tblDic.Clear();
        }
        public void ClearFeatureClass()
        {
            ftrDic.Clear();
        }
        public void ClearModels()
        {
            modelDic.Clear();
        }
    }
}
