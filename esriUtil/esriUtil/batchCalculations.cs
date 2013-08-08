﻿using System;
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
        public enum batchGroups { ARITHMETIC, MATH, SETNULL, LOGICAL, CLIP, CONDITIONAL, CONVOLUTION, FOCAL, FOCALSAMPLE, LOCALSTATISTICS, LINEARTRANSFORM, RESCALE, REMAP, COMPOSITE, EXTRACTBAND, CONVERTPIXELTYPE, GLCM, LANDSCAPE, ZONALSTATS, ZONALCLASSCOUNTS, SAVEFUNCTIONRASTER, BUILDRASTERSTATS, BUILDRASTERVAT, MOSAIC, MERGE, SAMPLERASTER, CLUSTERSAMPLERASTER, CREATERANDOMSAMPLE, CREATESTRATIFIEDRANDOMSAMPLE, MODEL, PREDICT, AGGREGATION, SURFACE, COMBINE, CONSTANT, ROTATE, SHIFT, NULLTOVALUE, BUILDMODEL, SELECTSAMPLES, EXPORTTABLE, EXPORTFEATURECLASS }
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
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, ITable> tblDic = new Dictionary<string, ITable>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } set { rstDic = value; } }
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
                default:
                    break;
            }
            return paramArr[paramArr.Length - 1];

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
                IRaster StrataRaster = getRaster(paramArr[1]);
                IRaster VariableRaster = getRaster(paramArr[2]);
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
                IRaster StrataRaster = getRaster(paramArr[1]);
                IRaster VariableRaster = getRaster(paramArr[2]);
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
                IRaster StrataRaster = getRaster(paramArr[1]);
                IRaster VariableRaster = getRaster(paramArr[2]);
                dpTT = new Statistics.dataPrepTTest(StrataRaster, VariableRaster);
            }
            dpTT.writeModel(paramArr[paramArr.Length-1]);
        }

        private void createCovCorrModel(string[] paramArr)
        {
            Statistics.dataPrepVarCovCorr dpVc = null;
            if (paramArr.Length > 3)
            {
                IRaster rs = getRaster(paramArr[1]);
                dpVc = new Statistics.dataPrepVarCovCorr(rs);
            }
            else
            {
                ITable table = getTable(paramArr[1]);
                string[] variables = paramArr[2].Split(new char[] { ',' });
                dpVc = new Statistics.dataPrepVarCovCorr(table, variables);
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
            IRaster mapRs = getRaster(paramArr[2]);
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
                IRaster rs = getRaster(paramArr[1]);
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
            Statistics.dataPrepCluster dpC = null;
            if(paramArr.Length<5)
            {
                IRaster rs = getRaster(paramArr[1]);
                int nCls = System.Convert.ToInt32(paramArr[2]);
                dpC = new Statistics.dataPrepCluster(rs,nCls);
            }
            else
            {
                ITable tbl = getTable(paramArr[1]);
                string[] flds = paramArr[2].Split(new char[]{','});
                int nCls = System.Convert.ToInt32(paramArr[3]);
                dpC = new Statistics.dataPrepCluster(tbl,flds,nCls);
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

        private IRaster createNullToValueRaster(string[] paramArr)
        {
            IRaster inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.setnullToValueFunction(inRs, d);
        }

        private IRaster createShiftRaster(string[] paramArr)
        {
            IRaster inRs = getRaster(paramArr[0]);
            double shiftX = System.Convert.ToDouble(paramArr[1]);
            double shiftY = System.Convert.ToDouble(paramArr[2]);
            return rsUtil.shiftRasterFunction(inRs,shiftX,shiftY);
        }

        private IRaster createRotateRaster(string[] paramArr)
        {
            IRaster inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.RotateRasterFunction(inRs, d);
        }

        private IRaster createConstantRaster(string[] paramArr)
        {
            IRaster inRs = getRaster(paramArr[0]);
            double d = System.Convert.ToDouble(paramArr[1]);
            return rsUtil.constantRasterFunction(inRs, d);
        }

        private IRaster createCombineFunction(string[] paramArr)
        {
            IRaster[] inRasters = new IRaster[paramArr.Length];
            for (int i = 0; i < paramArr.Length; i++)
            {
                inRasters[i] = getRaster(paramArr[i]);
            }
            return rsUtil.calcCombineRasterFunction(inRasters);
        }

        private IRaster createSurfaceFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            string sTypeStr = paramArr[1];
            rasterUtil.surfaceType sType = (rasterUtil.surfaceType)Enum.Parse(typeof(rasterUtil.surfaceType), sTypeStr);
            IRaster oRs = null;
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
                IRaster rs1 = getRaster(paramArr[0]);
                IRaster rs2 = getRaster(paramArr[1]);
                return rsUtil.zonalStats(rs1, rs2, paramArr[2], null, rp,true);
            }
            else
            {
                IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
                string inFtrFld = paramArr[1];
                IRaster vRs = getRaster(paramArr[2]);
                return rsUtil.zonalStats(inFtrCls, inFtrFld, vRs, paramArr[3], null, rp,true);
            }
        }

        private IRaster createAggregationFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            int cells = System.Convert.ToInt32(paramArr[1]);
            rasterUtil.focalType ftype = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), paramArr[2]);
            foreach (string s in rstStr.Split(new char[] { ',' }))
            {
                IRaster rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return rsUtil.calcAggregationFunction((IRaster)rsBC,cells,ftype);
        }

        private ITable predictNewValues(string[] paramArr)
        {
            string tblStr = paramArr[0];
            ITable tbl = geoUtil.getTable(tblStr);
            string mPath = getModelPath(paramArr[1]);
            Statistics.ModelHelper mH = new Statistics.ModelHelper(mPath);
            mH.predictNewData(tbl);
            return tbl;
        }

        private IRaster createModelFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            string mPath = getModelPath(paramArr[1]);
            foreach (string s in rstStr.Split(new char[]{','}))
            {
                IRaster rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            Statistics.ModelHelper mH = new Statistics.ModelHelper(mPath, (IRaster)rsBC, rsUtil);
            return mH.getRaster();
        }

        private IRaster convertPixelType(string[] paramArr)
        {
            IRaster inRaster = getRaster(paramArr[0]);
            rstPixelType pType = (rstPixelType)Enum.Parse(typeof(rstPixelType),paramArr[1]);
            return rsUtil.convertToDifFormatFunction(inRaster, pType);
        }

        private IRaster createFocalSampleFunction(string[] paramArr)
        {
            IRaster inRaster = getRaster(paramArr[0]);
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
            IRaster sampleRst = getRaster(paramArr[1]);
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
            IRaster rasterPath = rsUtil.returnRaster(paramArr[1]);
            int[] sampleSizePerClass = (from string s in paramArr[2].Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
            string outName = paramArr[3];
            return rsUtil.createRandomSampleLocationsByClass(wks, rasterPath, sampleSizePerClass, 1, outName);
        }

        private IFeatureClass createRandomSample(string[] paramArr)
        {
            IWorkspace wks = geoUtil.OpenWorkSpace(paramArr[0]);
            IRaster rasterPath = rsUtil.returnRaster(paramArr[1]);
            int sampleSize = System.Convert.ToInt32(paramArr[2]);
            string outName = paramArr[3];
            return rsUtil.createRandomSampleLocations(wks, rasterPath, sampleSize, outName);
        }

        private IFeatureClass sampleRaster(string[] paramArr)
        {
            IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
            IRaster sampleRst = getRaster(paramArr[1]);
            string inName = paramArr[2];
            if (inName == "" || inName == "null")
            {
                inName = null;
            }
            rsUtil.sampleRaster(inFtrCls, sampleRst, inName);
            return inFtrCls;
        }

        private IRaster createMerge(string[] paramArr)
        {
            string rsStr = paramArr[0];
            string[] rsStrArr = rsStr.Split(new char[]{','});
            IRaster[] rstArr = new IRaster[rsStrArr.Length];
            for (int i = 0; i < rsStrArr.Length; i++)
			{
                rstArr[i] = getRaster(rsStrArr[i]);
			}
            rasterUtil.mergeType mType = (rasterUtil.mergeType)Enum.Parse(typeof(rasterUtil.mergeType), paramArr[1]);
            return rsUtil.calcMosaicFunction(rstArr, mType);
        }

        private IRaster createMosaic(string[] paramArr)
        {
            IWorkspace wks = geoUtil.OpenRasterWorkspace(paramArr[0]);
            string mosiacName = paramArr[1];
            string rsStr = paramArr[2];
            string[] rsStrArr = rsStr.Split(new char[] { ',' });
            IRaster[] rstArr = new IRaster[rsStrArr.Length];
            for (int i = 0; i < rsStrArr.Length; i++)
            {
                rstArr[i] = getRaster(rsStrArr[i]);
            }
            esriMosaicMethod mosaicmethod = (esriMosaicMethod)Enum.Parse(typeof(esriMosaicMethod), paramArr[3]);
            rstMosaicOperatorType mosaictype = (rstMosaicOperatorType)Enum.Parse(typeof(rstMosaicOperatorType), paramArr[4]);
            bool buildfootprint = System.Convert.ToBoolean(paramArr[5]);
            bool buildboundary = System.Convert.ToBoolean(paramArr[6]);
            bool seemlines = System.Convert.ToBoolean(paramArr[7]);
            bool buildOverview = System.Convert.ToBoolean(paramArr[8]);
            return rsUtil.mosaicRastersFunction(wks, mosiacName, rstArr, mosaicmethod,mosaictype, buildfootprint, buildboundary, seemlines, buildOverview);
        }

        private ITable buildRasterVat(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            return rsUtil.buildVat(rs);
        }

        private IRaster buildRasterStats(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            int skipFactor = System.Convert.ToInt32(paramArr[1]);
            return rsUtil.calcStatsAndHist(rs, skipFactor);
            
        }

        private IRaster saveRaster(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            string outName = paramArr[1];
            IWorkspace wks = geoUtil.OpenRasterWorkspace(paramArr[2]);
            rasterUtil.rasterType rastertype = (rasterUtil.rasterType)Enum.Parse(typeof(rasterUtil.rasterType),paramArr[3].ToUpper());
            return rsUtil.returnRaster(rsUtil.saveRasterToDataset(rs, outName, wks, rastertype));
        }

        private IRaster createLandscapeFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
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

        private IRaster createGLCMFunction(string[] paramArr)
        {
            IRaster inRaster = getRaster(paramArr[0]);
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

        private IRaster createExtractFunction(string[] paramArr)
        {
            IRaster inRaster = getRaster(paramArr[0]);
            IRasterBandCollection rsBCO = (IRasterBandCollection)inRaster;
            IRasterBandCollection rsBC = new RasterClass();
            foreach (string s in paramArr[1].Split(new char[]{','}))
            {
                int bnd = System.Convert.ToInt32(s.Trim()) - 1;
                rsBC.AppendBand(rsBCO.Item(bnd));
            }
            return (IRaster)rsBC;
        }

        private IRaster createCompositeFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            foreach(string s in paramArr)
            {
                IRaster rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return (IRaster)rsBC;
        }

        private IRaster createRemapFunction(string[] paramArr)
        {
            IRaster inRaster = getRaster(paramArr[0]);
            IRemapFilter flt = new RemapFilterClass();
            foreach (string s in paramArr[1].Split(new char[] { ',' }))
            {
                double[] rVls = (from string s2 in (s.Split(new char[]{':'})) select System.Convert.ToDouble(s2)).ToArray();
                flt.AddClass(rVls[0], rVls[1], rVls[2]);
            }
            return rsUtil.calcRemapFunction(inRaster,flt);
        }

        private IRaster createRescaleFunction(string[] paramArr)
        {
            return rsUtil.reScaleRasterFunction(getRaster(paramArr[0]));
        }

        private IRaster createLinearTransformFunction(string[] paramArr)
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

        private IRaster createLocalFunction(string[] paramArr)
        {
            IRasterBandCollection rsBC = new RasterClass();
            string rstStr = paramArr[0];
            rasterUtil.localType lType = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),paramArr[1]);
            foreach (string s in rstStr.Split(new char[] { ',' }))
            {
                IRaster rs = getRaster(s);
                rsBC.AppendBands((IRasterBandCollection)rs);
            }
            return rsUtil.localStatisticsfunction((IRaster)rsBC,lType);
        }

        private IRaster createFocalFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
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

        private IRaster createConvolutionFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
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

        private IRaster createConditionalFunction(string[] paramArr)
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
            IRaster conRs = getRaster(paramArr[0]);
            return rsUtil.conditionalRasterFunction(conRs, inRs1, inRs2);
        }

        private IRaster createClipFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            IFeatureClass ftrCls = getFeatureClass(paramArr[1]);
            IGeometry geo = ((IGeoDataset)ftrCls).Extent;
            return rsUtil.clipRasterFunction(rs, geo, esriRasterClippingType.esriRasterClippingOutside);
        }

        private IRaster createLogicalFunction(string[] paramArr)
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
            IRaster rs = null;
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

        private IRaster createSetNullFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            List<double[]> minMaxLst = new List<double[]>();
            string vlStr = paramArr[1];
            string[] vlArr = vlStr.Split(new char[] { ',' });
            foreach (string s in vlArr)
            {
                string[] rngArr = s.Split(new char[] { '-' });
                double[] mm = { System.Convert.ToDouble(rngArr[0]), System.Convert.ToDouble(rngArr[1]) };
                minMaxLst.Add(mm);
            }
            return rsUtil.setValueRangeToNodata(rs,minMaxLst);
        }

        private IRaster createMathFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
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
                IRaster rs1 = getRaster(paramArr[0]);
                IRaster rs2 = getRaster(paramArr[1]);
                return rsUtil.zonalStats(rs1, rs2,paramArr[2], zLst.ToArray(),rp);
            }
            else
            {
                IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
                string inFtrFld = paramArr[1];
                IRaster vRs = getRaster(paramArr[2]);
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

        private IRaster createArithmeticFunction(string[] paramArr)
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

        private IRaster getRaster(string p)
        {
            string tp = p.Trim();
            if (rstDic.ContainsKey(tp))
            {
                return rstDic[tp];
            }
            else
            {
                IRaster outRs = rsUtil.returnRaster(tp);
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
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;inFeatureClass;logicalOperator)";
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
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster)";
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
                    msg = "outModel = " + batchFunction.ToString() + "(modelType;model parameters)";
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
                IRaster rs;
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
        public void GetFinalRaster(out string nm, out IRaster rs, out string desc)
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
    }
}
