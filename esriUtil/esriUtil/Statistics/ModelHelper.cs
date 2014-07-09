using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using esriUtil.Statistics;
using System.Windows.Forms.DataVisualization;

namespace esriUtil.Statistics
{
    public class ModelHelper
    {
        private static IStatusBar sBar = null;
        private static bool sBarRun;
        private static System.Threading.Thread statusBarThread = null;
        private static ESRI.ArcGIS.Framework.IApplication app = null;
        public static void closeProgressBar()
        {
            sBarRun = false;
            if (statusBarThread != null)
            {
                if (statusBarThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    statusBarThread.Start();
                }
                statusBarThread.Join(100);
                   
            }
            if (sBar != null)
            {
                sBar.HideProgressBar();
            }
            statusBarThread = null;
            
        }
        public static void runProgressBar(string message)
        {
            sBarRun = true;
            statusBarThread = new System.Threading.Thread(() => genericProgressBar(message));
            statusBarThread.Start();
        }
        private static void genericProgressBar(string message)
        {
            IStepProgressor spB = null;
            if (sBar == null)
            {
                Type t = Type.GetTypeFromCLSID(typeof(ESRI.ArcGIS.Framework.AppRefClass).GUID);
                System.Object obj = Activator.CreateInstance(t);
                app = obj as ESRI.ArcGIS.Framework.IApplication;
                sBar = app.StatusBar;
                spB = sBar.ProgressBar;
                spB.MinRange = 0;
                spB.MaxRange = 50;
                spB.StepValue = 1;
            }
            else
            {
                spB = sBar.ProgressBar;
            }

            if (spB.Position != 0) spB.OffsetPosition(spB.Position * -1);
            spB.Message = message;
            spB.Show();
            int cnt = 0;
            while (sBarRun)
            {
                if (cnt < 49)
                {
                    spB.Step();
                    cnt++;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    spB.OffsetPosition(-49);
                    cnt = 0;
                }
            }
        }
        public static string getValue(string vl, int leng)
        {
            string outVl = vl;
            if (vl.Length > leng) outVl = vl.Substring(0, leng);
            else outVl = vl.PadRight(leng, ' ');
            return outVl;
        }
        public static string saveModelFileDialog()
        {
            string mPath = "";
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Model|*.mdl";
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mPath = sfd.FileName;
            }
            return mPath;
        }
        public static string openModelFileDialog()
        {
            string mPath = "";
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Model|*.mdl";
            ofd.Multiselect = false;
            ofd.AddExtension = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mPath = ofd.FileName;
            }
            return mPath;
        }
        public static bool deleteModelFile()
        {
            bool d = true;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Model|*.mdl";
            ofd.Multiselect = true;
            ofd.AddExtension = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    foreach (string s in ofd.FileNames)
                    {
                        System.IO.File.Delete(s);
                    }
                }
                catch
                {
                    d = false;
                }
            }
            return d;
        }
        public ModelHelper(string modelPath)
        {
            rsUtil = new rasterUtil();
            mdlp = modelPath;
        }
        public ModelHelper(string modelPath, IRaster CoefficentRaster)
        {
            rsUtil = new rasterUtil();
            mdlp = modelPath;
            coefRst = CoefficentRaster;
        }
        public ModelHelper(string modelPath,IRaster CoefficentRaster, rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
            mdlp = modelPath;
            coefRst = CoefficentRaster;
        }
        private string mdlp  = "";
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil  = null;
        private IRaster coefRst = null;
        public IRaster getRaster()
        {
            IRaster outRs = null;
            string fLn = getFirstLineOfModel();
            dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), fLn);
            switch (mType)
            {
                case dataPrepBase.modelTypes.LinearRegression:
                    outRs = getLinearRegressionRaster();
                    break;
                case dataPrepBase.modelTypes.MvlRegression:
                    outRs = getMultivariateRegressionRaster();
                    break;
                case dataPrepBase.modelTypes.PLR:
                    outRs = getPlrRaster();
                    break;
                case dataPrepBase.modelTypes.RandomForest:
                    outRs = getRandomForestRaster();
                    break;
                case dataPrepBase.modelTypes.SoftMax:
                    outRs = getSoftMaxNnetRaster();
                    break;
                case dataPrepBase.modelTypes.Cart:
                    outRs = getCartRaster();
                    break;
                case dataPrepBase.modelTypes.L3:
                    outRs = getL3Raster();
                    break;
                case dataPrepBase.modelTypes.LogisticRegression:
                    outRs = getLogisticRegression();
                    break;
                case dataPrepBase.modelTypes.PCA:
                    outRs = getPcaRaster();
                    break;
                case dataPrepBase.modelTypes.Cluster:
                    outRs = getClusterRaster();
                    break;
                case dataPrepBase.modelTypes.TTEST:
                    outRs = getTTestRaster();
                    break;
                case dataPrepBase.modelTypes.PAIREDTTEST:
                    outRs = getPairedTTestRaster();
                    break;
                case dataPrepBase.modelTypes.GLM:
                    outRs = getGLMRaster();
                    break;
                case dataPrepBase.modelTypes.KDA:
                    outRs = getKDARaster();
                    break;
                case dataPrepBase.modelTypes.LDA:
                    outRs = getLDARaster();
                    break;
                default:
                    Console.WriteLine("Can't make a raster out of model");
                    break;
            }
            return outRs;
        }

        private IRaster getLDARaster()
        {
            dataPrepDiscriminantAnalysisLda lda = new dataPrepDiscriminantAnalysisLda();
            lda.getDaModel(mdlp, true);
            return rsUtil.createRaster(rsUtil.calcLdaFunction(coefRst, lda));
        }

        private IRaster getKDARaster()
        {
            dataPrepDiscriminantAnalysis kda = new dataPrepDiscriminantAnalysis();
            kda.getDaModel(mdlp, true);
            return rsUtil.createRaster(rsUtil.calcKdaFunction(coefRst,kda));
        }

        private IRaster getGLMRaster()
        {
            dataPrepGlm glm = new dataPrepGlm();
            glm.getGlmModel(mdlp, true);
            return rsUtil.createRaster(rsUtil.calcGlmFunction(coefRst, glm));
        }

        private IRaster getPairedTTestRaster()
        {
            dataPrepPairedTTest pttest = new dataPrepPairedTTest();
            pttest.buildModel(mdlp);
            return rsUtil.createRaster(rsUtil.calcPairedTTestFunction(coefRst, pttest));
        }

        private IRaster getTTestRaster()
        {
            dataPrepTTest ttest = new dataPrepTTest();
            ttest.buildModel(mdlp);
            return rsUtil.createRaster(rsUtil.calcTTestFunction(coefRst, ttest));
        }

        private IRaster getClusterRaster()
        {
            clusterType cType = (clusterType)System.Enum.Parse(typeof(clusterType), getSecondLine());
            IRaster outRs = null;
            switch (cType)
            {
                case clusterType.KMEANS:
                    dataPrepClusterKmean clusKm = new dataPrepClusterKmean();
                    clusKm.buildModel(mdlp);
                    outRs = rsUtil.createRaster(rsUtil.calcClustFunctionKmean(coefRst, clusKm));
                    break;
                case clusterType.BINARY:
                    dataPrepClusterBinary clusBs = new dataPrepClusterBinary();
                    clusBs.buildModel(mdlp);
                    outRs = rsUtil.createRaster(rsUtil.calcClustFunctionBinary(coefRst, clusBs));
                    break;
                case clusterType.GAUSSIANMIXTURE:
                    dataPrepClusterGaussian clusGa = new dataPrepClusterGaussian();
                    clusGa.buildModel(mdlp);
                    outRs = rsUtil.createRaster(rsUtil.calcClustFunctionGaussian(coefRst, clusGa));
                    break;
                default:
                    break;
            }
            return outRs;
            
        }

        private string getSecondLine()
        {
            string ln = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(mdlp))
            {
                ln = sr.ReadLine();
                ln = sr.ReadLine();
                sr.Close();
            }
            return ln;
        }

        private IRaster getPcaRaster()
        {
            dataPrepPrincipleComponents pca = new dataPrepPrincipleComponents();
            pca.buildModel(mdlp);
            return rsUtil.createRaster(rsUtil.calcPrincipleComponentsFunction(coefRst, pca));
        }

        private IRaster getLogisticRegression()
        {
            dataPrepLogisticRegression lr = new dataPrepLogisticRegression();
            lr.getLrModel(mdlp);
            indvar = lr.IndependentFieldNames;
            depvar = lr.DependentFieldNames;
            double[][] coef = new double[1][];
            coef[0] = lr.Coefficients;
            return rsUtil.createRaster(rsUtil.calcPolytomousLogisticRegressFunction(coefRst, coef));
        }

        private IRaster getL3Raster()
        {
            throw new NotImplementedException();
        }

        private IRaster getCartRaster()
        {
            throw new NotImplementedException();
        }

        private IRaster getSoftMaxNnetRaster()
        {
            dataPrepSoftMaxPlr sm = new dataPrepSoftMaxPlr();
            sm.getMnlModel(mdlp);
            return rsUtil.createRaster(rsUtil.calcSoftMaxNnetFunction(coefRst, sm));
        }

        private IRaster getRandomForestRaster()
        {
            dataPrepRandomForest rf = new dataPrepRandomForest();
            rf.getDfModel(mdlp);
            indvar = rf.IndependentFieldNames;
            depvar = rf.DependentFieldNames;
            return rsUtil.createRaster(rsUtil.calcRandomForestFunction(coefRst, rf));
        }

        private IRaster getPlrRaster()
        {
            dataPrepMultinomialLogisticRegression plr = new dataPrepMultinomialLogisticRegression();
            plr.getPlrModel(mdlp);
            indvar = plr.IndependentFieldNames;
            depvar = plr.DependentFieldNames;
            double[][] coef = plr.Coefficients;
            return rsUtil.createRaster(rsUtil.calcPolytomousLogisticRegressFunction(coefRst, coef));
        }

        private IRaster getMultivariateRegressionRaster()
        {
            dataPrepMultivariateLinearRegression mvlr = new dataPrepMultivariateLinearRegression();
            mvlr.getMlrModel(mdlp);
            indvar = mvlr.IndependentFieldNames;
            depvar = mvlr.DependentFieldNames;
            List<float[]> coefLst = new List<float[]>();
            for (int i = 0; i < mvlr.multivariateRegression.Length; i++)
            {
                double[] coef = mvlr.multivariateRegression[i].Coefficients;
                float[] fCoef = (from double d in coef select System.Convert.ToSingle(d)).ToArray();
                coefLst.Add(fCoef);
            }
            return rsUtil.createRaster(rsUtil.calcRegressFunction(coefRst, coefLst));
        }

        private IRaster getLinearRegressionRaster()
        {
            dataPrepLinearReg lg = new dataPrepLinearReg();
            lg.getLrModel(mdlp);
            indvar = lg.IndependentFieldNames;
            depvar = lg.DependentFieldNames;
            double[] coef = lg.Coefficients;
            float[] fCoef = (from double d in coef select System.Convert.ToSingle(d)).ToArray();
            List<float[]> coefLst = new List<float[]>();
            coefLst.Add(fCoef);
            return rsUtil.createRaster(rsUtil.calcRegressFunction(coefRst, coefLst));
        }

        private string getFirstLineOfModel()
        {
            string ln = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(mdlp))
            {
                ln =sr.ReadLine();
                sr.Close();
            }
            return ln;
        }
        public static dataPrepBase.modelTypes getModelType(string modelPath)
        {
            dataPrepBase.modelTypes mType = dataPrepBase.modelTypes.Accuracy;
            string ln = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                ln = sr.ReadLine();
                sr.Close();
            }
            if (ln != "")
            {
                mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), ln);
            }
            return mType;
        }
        public void openModelReport(string modelPath,double alpha,bool report=true)
        {
            string fLn = getFirstLineOfModel();
            dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), fLn);
            switch (mType)
            {
                case dataPrepBase.modelTypes.Accuracy:
                    openRpAc(alpha,report);
                    break;
                case dataPrepBase.modelTypes.LinearRegression:
                    openRpLr(alpha,report);
                    break;
                case dataPrepBase.modelTypes.MvlRegression:
                    openRpMlr(alpha,report);
                    break;
                case dataPrepBase.modelTypes.PLR:
                    openRpPlr(alpha,report);
                    break;
                case dataPrepBase.modelTypes.RandomForest:
                    openRpDf(report);
                    break;
                case dataPrepBase.modelTypes.SoftMax:
                    openRpSm(report);
                    break;
                case dataPrepBase.modelTypes.Cart:
                    openRpCt(alpha,report);
                    break;
                case dataPrepBase.modelTypes.L3:
                    openRpL3(alpha,report);
                    break;
                case dataPrepBase.modelTypes.LogisticRegression:
                    openLogisticReg(alpha, report);
                    break;
                case dataPrepBase.modelTypes.CovCorr:
                    openCovCorr(alpha, report);
                    break;
                case dataPrepBase.modelTypes.PCA:
                    openPca(alpha, report);
                    break;
                case dataPrepBase.modelTypes.Cluster:
                    openCluster(alpha, report);
                    break;
                case dataPrepBase.modelTypes.TTEST:
                    openTTest(alpha, report);
                    break;
                case dataPrepBase.modelTypes.PAIREDTTEST:
                    openPairedTTest(alpha, report);
                    break;
                case dataPrepBase.modelTypes.KS:
                    openKsTest(alpha, report);
                    break;
                case dataPrepBase.modelTypes.GLM:
                    openGLM(alpha,report);
                    break;
                case dataPrepBase.modelTypes.CompareClassifications:
                    openCompareClass(alpha, report);
                    break;
                case dataPrepBase.modelTypes.LDA:
                    openLDA(alpha, report);
                    break;
                case dataPrepBase.modelTypes.KDA:
                    openKDA(alpha, report);
                    break;
                default:
                    break;
            }

        }

        private void openKDA(double alpha, bool report)
        {
            dataPrepDiscriminantAnalysis kda = new dataPrepDiscriminantAnalysis();
            kda.getDaModel(mdlp);
            depvar = kda.DependentFieldNames;
            indvar = kda.IndependentFieldNames;
            if (report) kda.getReport();
        }

        private void openLDA(double alpha, bool report)
        {
            dataPrepDiscriminantAnalysisLda lda = new dataPrepDiscriminantAnalysisLda();
            lda.getDaModel(mdlp);
            depvar = lda.DependentFieldNames;
            indvar = lda.IndependentFieldNames;
            if (report) lda.getReport();
        }

        private void openCompareClass(double alpha, bool report)
        {
            dataPrepCompareClassifications compClass = new dataPrepCompareClassifications();
            compClass.getModel(mdlp);
            depvar = compClass.DependentFieldNames;
            indvar = compClass.IndependentFieldNames;
            if (report) compClass.getReport();
        }

        private void openGLM(double alpha, bool report)
        {
            dataPrepGlm glm = new dataPrepGlm();
            glm.getGlmModel(mdlp);
            depvar = glm.DependentFieldNames;
            indvar = glm.IndependentFieldNames;
            if (report) glm.getReport(alpha);
        }

        private void openKsTest(double alpha, bool report)
        {
            dataPrepCompareSamples ksTest = new dataPrepCompareSamples(mdlp);
            depvar = ksTest.Variables;
            indvar = ksTest.Variables;
            if (report) ksTest.getReport();
        }

        private void openPairedTTest(double alpha, bool report)
        {
            dataPrepPairedTTest pttest = new dataPrepPairedTTest();
            pttest.buildModel(mdlp);
            depvar = pttest.Labels.ToArray();
            indvar = pttest.VariableFieldNames;
            if (report) pttest.getReport();
        }

        private void openTTest(double alpha, bool report)
        {
            dataPrepTTest ttest = new dataPrepTTest();
            ttest.buildModel(mdlp);
            depvar = ttest.Labels.ToArray();
            indvar = ttest.VariableFieldNames;
            if (report) ttest.getReport();
        }

        private void openCluster(double alpha, bool report)
        {
            dataPrepClusterKmean clus = new dataPrepClusterKmean();
            clus.buildModel(mdlp);
            depvar = clus.VariableFieldNames;
            indvar = clus.VariableFieldNames;
            if (report) clus.getReport();
        }

        private void openPca(double alpha, bool report)
        {
            dataPrepPrincipleComponents pca = new dataPrepPrincipleComponents();
            pca.buildModel(mdlp);
            depvar = pca.VariableFieldNames;
            indvar = pca.VariableFieldNames;
            if (report) pca.getReport();
        }

        private void openCovCorr(double alpha, bool report)
        {
            dataPrepVarCovCorr varCov = new dataPrepVarCovCorr();
            varCov.buildModel(mdlp);
            depvar = varCov.VariableFieldNames;
            indvar = varCov.VariableFieldNames;
            if (report) varCov.getReport();
        }

        private void openLogisticReg(double alpha, bool report)
        {
            dataPrepLogisticRegression lr = new dataPrepLogisticRegression();
            lr.getLrModel(mdlp);
            depvar = lr.DependentFieldNames;
            indvar = lr.IndependentFieldNames;
            if (report) lr.getReport(alpha);
        }

        private void openRpL3(double alpha,bool report = true)
        {
            throw new NotImplementedException();
        }

        private void openRpCt(double alpha, bool report = true)
        {
            throw new NotImplementedException();
        }

        private void openRpSm(bool report = true)
        {
            dataPrepSoftMaxPlr sm = new dataPrepSoftMaxPlr();
            sm.getMnlModel(mdlp);
            indvar = sm.IndependentFieldNames;
            depvar = sm.DependentFieldNames;
            if (report) sm.getReport();
        }

        private void openRpDf(bool report = true)
        {
            dataPrepRandomForest rf = new dataPrepRandomForest();
            alglib.decisionforest df = rf.getDfModel(mdlp,false);
            depvar = rf.DependentFieldNames;
            indvar = rf.IndependentFieldNames;
            if (report)
            {
                rf.getReport();
            }
        }

        private void openRpPlr(double alpha, bool report = true)
        {
            dataPrepMultinomialLogisticRegression mlr = new dataPrepMultinomialLogisticRegression();
            mlr.getPlrModel(mdlp);
            depvar = mlr.DependentFieldNames;
            indvar = mlr.IndependentFieldNames;
            if (report) mlr.getReport(alpha);
        }

        private void openRpMlr(double alpha, bool report = true)
        {
            dataPrepMultivariateLinearRegression mvlr = new dataPrepMultivariateLinearRegression();
            mvlr.getMlrModel(mdlp);
            depvar = mvlr.DependentFieldNames;
            indvar = mvlr.IndependentFieldNames;
            if (report) mvlr.getReport(alpha);
        }

        private void openRpLr(double alpha, bool report = true)
        {
            dataPrepLinearReg lr = new dataPrepLinearReg();
            lr.getLrModel(mdlp);
            depvar = lr.DependentFieldNames;
            indvar = lr.IndependentFieldNames;
            if(report) lr.getReport(alpha);
        }

        private void openRpAc(double alpha, bool report = true)
        {
            dataGeneralConfusionMatirx gf = new dataGeneralConfusionMatirx();
            indvar = gf.IndependentFieldNames;
            depvar = gf.DependentFieldNames;
            gf.getXTable(mdlp);
            if (report) gf.getReport(alpha);
        }
        private string[] indvar = null;
        private string[] depvar = null;
        public string[] DependentVariables
        {
            get
            {
                return depvar;
            }
        }
        public string[] IndependentVariables 
        {
            get
            {
                return indvar;
            }
        }
        public void predictNewData(ITable inputTable,IQueryFilter qf)
        {
            IObjectClassInfo2 ocI2 = (IObjectClassInfo2)inputTable;
            if (!ocI2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input table has a composition relationship. Please export data and try again!");
                return;
            }
            IFields flds = inputTable.Fields;
            openModelReport(mdlp, 0.05, false);
            int[] fldIndexArr = new int[IndependentVariables.Length];
            for (int i = 0; i < fldIndexArr.Length; i++)
            {
                string fldName = IndependentVariables[i];
                int fldIndex = flds.FindField(fldName);
                if (fldIndex == -1)
                {
                    System.Windows.Forms.MessageBox.Show("Could not find required independent variable in the table!!", "Missing Parameter", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                fldIndexArr[i] = fldIndex;
            }
            Statistics.dataPrepBase.modelTypes mType = (Statistics.dataPrepBase.modelTypes)Enum.Parse(typeof(Statistics.dataPrepBase.modelTypes),getFirstLineOfModel());
            switch (mType)
            {
                case dataPrepBase.modelTypes.LinearRegression:
                    predictLrData(inputTable,fldIndexArr,qf);
                    break;
                case dataPrepBase.modelTypes.MvlRegression:
                    predictMlrData(inputTable, fldIndexArr,qf);
                    break;
                case dataPrepBase.modelTypes.PLR:
                    predictPlrData(inputTable, fldIndexArr,qf);
                    break;
                case dataPrepBase.modelTypes.RandomForest:
                    predictRfData(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.SoftMax:
                    predictSmData(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.Cart:
                    predictCartData(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.L3:
                    predictL3Data(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.LogisticRegression:
                    predictLogisticReg(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.PCA:
                    predictPca(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.Cluster:
                    clusterType cType = (clusterType)Enum.Parse(typeof(clusterType), getSecondLine());
                    predictCluster(inputTable, fldIndexArr, cType, qf);
                    break;
                case dataPrepBase.modelTypes.TTEST:
                    predictTTest(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.PAIREDTTEST:
                    predictPairedTTest(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.GLM:
                    predictGLM(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.KDA:
                    predictKda(inputTable, fldIndexArr, qf);
                    break;
                case dataPrepBase.modelTypes.LDA:
                    predictLda(inputTable, fldIndexArr, qf);
                    break;
                default:
                    break;
            }
        }

        private void predictKda(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            string outFldName = DependentVariables[0] + "_P";
            outFldName = geoUtil.createField(inputTable, outFldName, esriFieldType.esriFieldTypeDouble, false);
            int outFldIndex = inputTable.FindField(outFldName);
            dataPrepDiscriminantAnalysis kda = new dataPrepDiscriminantAnalysis();
            kda.getDaModel(mdlp,true);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    int nVl = kda.computeNew(input);
                    rw.set_Value(outFldIndex, kda.Categories[nVl]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictLda(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            string outFldName = DependentVariables[0] + "_P";
            outFldName = geoUtil.createField(inputTable, outFldName, esriFieldType.esriFieldTypeDouble, false);
            int outFldIndex = inputTable.FindField(outFldName);
            dataPrepDiscriminantAnalysisLda kda = new dataPrepDiscriminantAnalysisLda();
            kda.getDaModel(mdlp,true);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    int nVl = kda.computeNew(input);
                    rw.set_Value(outFldIndex, kda.Categories[nVl]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictGLM(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            string outFldName = DependentVariables[0] + "_P";
            outFldName = geoUtil.createField(inputTable, outFldName, esriFieldType.esriFieldTypeDouble, false);
            int outFldIndex = inputTable.FindField(outFldName);
            dataPrepGlm glm = new dataPrepGlm();
            glm.getGlmModel(mdlp,true);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    double nVl = glm.computeNew(input);
                    rw.set_Value(outFldIndex, nVl);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictPairedTTest(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepPairedTTest pttest = new dataPrepPairedTTest();
            pttest.buildModel(mdlp);
            string groupFieldName = pttest.StrataField;
            List<string> labels = pttest.Labels;
            int varCnt = pttest.VariableFieldNames.Length;
            string[] newNameArray = new string[((varCnt * varCnt) - varCnt) / 2];
            int[] newIndexArray = new int[newNameArray.Length];
            int cnt = 1;
            int arrCnt = 0;
            for (int i = 0; i < varCnt - 1; i++)
            {
                for (int j = cnt; j < varCnt; j++)
                {
                    string f1 = pttest.VariableFieldNames[i];
                    string f2 = pttest.VariableFieldNames[j];
                    string nM = f1 + "_" + f2;
                    nM = geoUtil.createField(inputTable, nM, esriFieldType.esriFieldTypeDouble, false);
                    newNameArray[arrCnt] = nM;
                    arrCnt++;
                }
                cnt++;
            }
            Dictionary<string, double[]> tDic = new Dictionary<string, double[]>();
            foreach (string s in labels)
            {
                tDic.Add(s, pttest.computeNew(s));
            }
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            for (int i = 0; i < newNameArray.Length; i++)
            {
                newIndexArray[i] = cur.FindField(newNameArray[i]);
            }
            int groupFieldIndex = cur.FindField(groupFieldName);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string g = rw.get_Value(groupFieldIndex).ToString();
                double[] vlArr;
                if (tDic.TryGetValue(g, out vlArr))
                {
                    for (int i = 0; i < newIndexArray.Length; i++)
                    {
                        int ind = newIndexArray[i];
                        double vl = vlArr[i];
                        rw.set_Value(ind, vl);
                    }
                    cur.UpdateRow(rw);
                }
                else
                {
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictTTest(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            
            dataPrepTTest ttest = new dataPrepTTest();
            ttest.buildModel(mdlp);
            string groupFieldName = ttest.StrataField;
            List<string> labels = ttest.Labels;
            int varCnt = ttest.VariableFieldNames.Length;
            string[] newNameArray = new string[((varCnt*varCnt)-varCnt)/2];
            int[] newIndexArray = new int[newNameArray.Length];
            int cnt = 1;
            int arrCnt = 0;
            for (int i = 0; i < varCnt-1; i++)
            {
                for (int j = cnt; j < varCnt; j++)
                {
                    string f1 = ttest.VariableFieldNames[i];
                    string f2 = ttest.VariableFieldNames[j];
                    string nM = f1 + "_" + f2;
                    nM = geoUtil.createField(inputTable, nM, esriFieldType.esriFieldTypeDouble, false);
                    newNameArray[arrCnt] = nM;
                    arrCnt++;
                }
                cnt++;
            }
            Dictionary<string, double[]> tDic = new Dictionary<string, double[]>();
            foreach (string s in labels)
            {
                tDic.Add(s, ttest.computeNew(s));
            }
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            for (int i = 0; i < newNameArray.Length; i++)
            {
                newIndexArray[i] = cur.FindField(newNameArray[i]);
            }
            int groupFieldIndex = cur.FindField(groupFieldName);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string g = rw.get_Value(groupFieldIndex).ToString();
                double[] vlArr;
                if (tDic.TryGetValue(g, out vlArr))
                {
                    for (int i = 0; i < newIndexArray.Length; i++)
                    {
                        int ind = newIndexArray[i];
                        double vl = vlArr[i];
                        rw.set_Value(ind, vl);
                    }
                    cur.UpdateRow(rw);
                }
                else
                {
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictCluster(ITable inputTable, int[] fldIndexArr, clusterType cType = clusterType.KMEANS, IQueryFilter qf=null)
        {
            switch (cType)
            {
                case clusterType.KMEANS:
                    predKmeans(inputTable, fldIndexArr,qf);
                    break;
                case clusterType.BINARY:
                    predBinary(inputTable, fldIndexArr,qf);
                    break;
                case clusterType.GAUSSIANMIXTURE:
                    predGaussian(inputTable, fldIndexArr,qf);
                    break;
                default:
                    break;
            }
        }

        private void predGaussian(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepClusterGaussian clus = new dataPrepClusterGaussian();
            clus.buildModel(mdlp);
            List<string> lbl = clus.Labels;
            string newFldName = "Cluster";
            newFldName = geoUtil.createField(inputTable, newFldName, esriFieldType.esriFieldTypeString, false);
            int newFldNameIndex = inputTable.Fields.FindField(newFldName);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
                int nVl = clus.computNew(input);
                rw.set_Value(newFldNameIndex, lbl[nVl]);
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predBinary(ITable inputTable, int[] fldIndexArr,IQueryFilter qf)
        {
            dataPrepClusterBinary clus = new dataPrepClusterBinary();
            clus.buildModel(mdlp);
            List<string> lbl = clus.Labels;
            string newFldName = "Cluster";
            newFldName = geoUtil.createField(inputTable, newFldName, esriFieldType.esriFieldTypeString, false);
            int newFldNameIndex = inputTable.Fields.FindField(newFldName);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if(Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    int nVl = clus.computNew(input);
                    rw.set_Value(newFldNameIndex, lbl[nVl]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predKmeans(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepClusterKmean clus = new dataPrepClusterKmean();
            clus.buildModel(mdlp);
            List<string> lbl = clus.Labels;
            string newFldName = "Cluster";
            newFldName = geoUtil.createField(inputTable, newFldName, esriFieldType.esriFieldTypeString, false);
            int newFldNameIndex = inputTable.Fields.FindField(newFldName);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if(Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    int nVl = clus.computNew(input);
                    rw.set_Value(newFldNameIndex, lbl[nVl]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictPca(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepPrincipleComponents pca = new dataPrepPrincipleComponents();
            pca.buildModel(mdlp);
            List<string> oFldLst = new List<string>();
            for (int i = 0; i < IndependentVariables.Length; i++)
            {
                oFldLst.Add("Comp_" + (i + 1).ToString());
            }
            string[] outFldNameArr = oFldLst.ToArray();
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            for (int i = 0; i < outFldNameArr.Length; i++)
            {
                outFldNameArr[i] = geoUtil.createField(inputTable, outFldNameArr[i], esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
            }
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if(Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    double[] nVl = pca.computNew(input);
                    for (int i = 0; i < outFldNameIndex.Length; i++)
                    {
                        double vl = nVl[i];
                        rw.set_Value(outFldNameIndex[i], vl);
                    }
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictLogisticReg(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepLogisticRegression Lr = new dataPrepLogisticRegression();
            Lr.getLrModel(mdlp);
            string depVar = DependentVariables[0];
            List<string> oFldLst = (from string s in Lr.Categories select "p_" + s).ToList();
            oFldLst.Add(depVar + "_mlc");
            string[] outFldNameArr = oFldLst.ToArray();
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            outFldNameArr[Lr.NumberOfCategories] = geoUtil.createField(inputTable, outFldNameArr[Lr.NumberOfCategories], esriFieldType.esriFieldTypeString,false);
            outFldNameIndex[Lr.NumberOfCategories] = inputTable.Fields.FindField(outFldNameArr[Lr.NumberOfCategories]);
            for (int i = 0; i < outFldNameArr.Length - 1; i++)
            {
                outFldNameArr[i] = geoUtil.createField(inputTable, outFldNameArr[i], esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
            }
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if(Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    double nVl1 = Lr.computNew(input);
                    double nVl0 = 1 - nVl1;
                    int maxP = 0;
                    if (nVl1 > nVl0) maxP = 1;
                    double[] nVl = { nVl0, nVl1 };
                    for (int i = 0; i < outFldNameIndex.Length - 1; i++)
                    {
                        double vl = nVl[i];
                        rw.set_Value(outFldNameIndex[i], vl);
                    }
                    rw.set_Value(outFldNameIndex[Lr.NumberOfCategories], Lr.Categories[maxP]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictL3Data(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            throw new NotImplementedException();
        }

        private void predictCartData(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            throw new NotImplementedException();
        }

        private void predictSmData(ITable inputTable,int[] fldIndexArr,IQueryFilter qf)
        {
            dataPrepSoftMaxPlr sm = new dataPrepSoftMaxPlr();
            sm.getMnlModel(mdlp);
            string depVarName = DependentVariables[0];
            string[] outFldNameArr = new string[sm.NumberOfClasses+1];
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            string nName = depVarName + "_mlc";
            outFldNameArr[sm.NumberOfClasses] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeString,false);
            outFldNameIndex[sm.NumberOfClasses] = inputTable.Fields.FindField(outFldNameArr[sm.NumberOfClasses]);
            for (int i = 0; i < outFldNameArr.Length - 1; i++)
            {
                nName = "p_" + sm.Categories[i];
                outFldNameArr[i] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
            }
            double[] input = new double[fldIndexArr.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if(Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    double[] nVl = sm.computNew(input);
                    double maxVl = 0;
                    int maxP = 0;
                    for (int i = 0; i < outFldNameIndex.Length - 1; i++)
                    {
                        double vl = nVl[i];
                        if (vl > maxVl)
                        {
                            maxVl = vl;
                            maxP = i;
                        }
                        rw.set_Value(outFldNameIndex[i], vl);
                    }
                    rw.set_Value(outFldNameIndex[sm.NumberOfClasses], sm.Categories[maxP]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictRfData(ITable inputTable, int[] fldIndexArr, IQueryFilter qf)
        {
            //need to simplify for regression analysis (don't need the mlc class)
            dataPrepRandomForest rf = new dataPrepRandomForest();
            rf.getDfModel(mdlp);
            bool reg = rf.Regression;
            string depVarName = DependentVariables[0];
            //System.Windows.Forms.MessageBox.Show("number of categories = " + rf.NumberOfClasses.ToString());
            string[] outFldNameArr = null;
            if (reg)
            {
                outFldNameArr = new string[1];
            }
            else
            {
                outFldNameArr = new string[rf.NumberOfClasses + 1];
            }
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            string nName = depVarName + "_mlc";
            if (reg)
            {
                nName = depVarName + "_p";
                outFldNameArr[0] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeDouble, false);
                outFldNameIndex[0] = inputTable.Fields.FindField(outFldNameArr[0]);
            }
            else
            { 
                outFldNameArr[rf.NumberOfClasses] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeString, false);
                outFldNameIndex[rf.NumberOfClasses] = inputTable.Fields.FindField(outFldNameArr[rf.NumberOfClasses]);
                for (int i = 0; i < outFldNameArr.Length - 1; i++)
                {
                    nName = "p_" + rf.Categories[i];
                    outFldNameArr[i] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeDouble, false);
                    outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
                }
            }
            
            
            //System.Windows.Forms.MessageBox.Show(String.Join(", ", outFldNameArr));
            //System.Windows.Forms.MessageBox.Show(String.Join(", ", (from int i in outFldNameIndex select i.ToString()).ToArray()));
            double[] input = new double[fldIndexArr.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            if (reg)
            {
                while (rw != null)
                {
                    bool updateCheck = true;
                    for (int i = 0; i < fldIndexArr.Length; i++)
                    {
                        object objVl = rw.get_Value(fldIndexArr[i]);
                        if(Convert.IsDBNull(objVl))
                        {
                            updateCheck = false;
                            break;
                        }
                        else
                        {
                            double vl = System.Convert.ToDouble(objVl);
                            input[i] = vl;
                        }
                    }
                    if (updateCheck)
                    {
                        double[] nVl = rf.computNew(input);
                        rw.set_Value(outFldNameIndex[0], nVl[0]);
                        cur.UpdateRow(rw);
                    }
                    rw = cur.NextRow();
                }
            }
            else
            {
                while (rw != null)
                {
                    bool updateCheck = true;
                    for (int i = 0; i < fldIndexArr.Length; i++)
                    {
                        object objVl = rw.get_Value(fldIndexArr[i]);
                        if(Convert.IsDBNull(objVl))
                        {
                            updateCheck = false;
                            break;
                        }
                        else
                        {
                            double vl = System.Convert.ToDouble(objVl);
                            input[i] = vl;
                        }
                    }
                    if (updateCheck)
                    {
                        double[] nVl = rf.computNew(input);
                        double maxVl = 0;
                        int maxP = 0;
                        for (int i = 0; i < outFldNameIndex.Length - 1; i++)
                        {
                            double vl = nVl[i];
                            if (vl > maxVl)
                            {
                                maxVl = vl;
                                maxP = i;
                            }
                            rw.set_Value(outFldNameIndex[i], vl);
                        }
                        rw.set_Value(outFldNameIndex[rf.NumberOfClasses], rf.Categories[maxP]);
                        cur.UpdateRow(rw);
                    }
                    rw = cur.NextRow();
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictPlrData(ITable inputTable,int[] fldIndexArr,IQueryFilter qf)
        {
            dataPrepMultinomialLogisticRegression plr = new dataPrepMultinomialLogisticRegression();
            plr.getPlrModel(mdlp);
            string depVar=DependentVariables[0];
            List<string> oFldLst = (from string s in plr.Categories select "p_" + s).ToList();
            oFldLst.Add(depVar + "_mlc");
            string[] outFldNameArr = oFldLst.ToArray();
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            outFldNameArr[plr.NumberOfCategories] = geoUtil.createField(inputTable, outFldNameArr[plr.NumberOfCategories], esriFieldType.esriFieldTypeString,false);
            outFldNameIndex[plr.NumberOfCategories] = inputTable.Fields.FindField(outFldNameArr[plr.NumberOfCategories]);
            for (int i = 0; i < outFldNameArr.Length-1; i++)
            {
                outFldNameArr[i] = geoUtil.createField(inputTable, outFldNameArr[i], esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
            }
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    double[] nVl = plr.computNew(input);
                    double maxVl = 0;
                    int maxP = 0;
                    for (int i = 0; i < outFldNameIndex.Length - 1; i++)
                    {
                        double vl = nVl[i];
                        if (vl > maxVl)
                        {
                            maxVl = vl;
                            maxP = i;
                        }
                        rw.set_Value(outFldNameIndex[i], vl);
                    }
                    rw.set_Value(outFldNameIndex[plr.NumberOfCategories], plr.Categories[maxP]);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictMlrData(ITable inputTable,int[] fldIndexArr, IQueryFilter qf)
        {
            dataPrepMultivariateLinearRegression mvlr = new dataPrepMultivariateLinearRegression();
            mvlr.getMlrModel(mdlp);
            List<string> oFldLst = (from string s in DependentVariables select "p_" + s).ToList();
            string[] outFldNameArr = oFldLst.ToArray();
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            for (int i = 0; i < outFldNameArr.Length; i++)
            {
                outFldNameArr[i] = geoUtil.createField(inputTable, outFldNameArr[i], esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
            }

            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCheck = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    object objVl = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(objVl))
                    {
                        updateCheck = false;
                        break;
                    }
                    else
                    {
                        double vl = System.Convert.ToDouble(objVl);
                        input[i] = vl;
                    }
                }
                if (updateCheck)
                {
                    for (int i = 0; i < mvlr.multivariateRegression.Length; i++)
                    {
                        double nVl = mvlr.multivariateRegression[i].computeNew(input);
                        rw.set_Value(outFldNameIndex[i], nVl);
                    }
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictLrData(ITable inputTable,int[] fldIndexArr, IQueryFilter qf=null)
        {
            string outFldName = DependentVariables[0]+"_P";
            outFldName = geoUtil.createField(inputTable, outFldName, esriFieldType.esriFieldTypeDouble,false);
            int outFldIndex = inputTable.FindField(outFldName);
            dataPrepLinearReg lr = new dataPrepLinearReg();
            lr.getLrModel(mdlp);
            double[] input = new double[IndependentVariables.Length];
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            ICursor cur = inputTable.Update(qf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                bool updateCh = true;
                for (int i = 0; i < fldIndexArr.Length; i++)
                {

                    object vlObj = rw.get_Value(fldIndexArr[i]);
                    if (Convert.IsDBNull(vlObj))
                    {
                        updateCh = false;
                        break;
                    }
                    double vl = System.Convert.ToDouble(vlObj);
                    input[i] = vl;
                }
                if (updateCh)
                {
                    double nVl = lr.computeNew(input);
                    rw.set_Value(outFldIndex, nVl);
                    cur.UpdateRow(rw);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);

        }
        private static void testDvAss()
        {
            System.Windows.Forms.DataVisualization.Charting.Chart  ch = new System.Windows.Forms.DataVisualization.Charting.Chart();
        }
        public static bool chartingAvailable(bool askForDownload=true)
        {
            bool av = false;
            try
            {
                testDvAss();
                av = true;
            }
            catch
            {

                if (askForDownload && (System.Windows.Forms.MessageBox.Show("Can't make graphics because you do not have Microsoft Chart Controls for Microsoft .NET Framework 3.5.\nDo you want to download and install the controls?", "Chart", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = "http://www.microsoft.com/en-us/download/details.aspx?id=14422";
                    prc.Start();
                    //prc.WaitForExit();
                    //if (chartingAvailable(false))
                    //{
                    //    av= true;
                    //}
                    //else
                    //{
                    //    av = false;
                    //}
                    //av = false;
                }
                av = false;
            }
            return av;
        }
        public static object generateProbabilityGraphic(string[] IndependentFieldNames)
        {
            object outFrm = null;
            try
            {
                Forms.Stats.frmChart outFrm1 = new Forms.Stats.frmChart();
                System.Windows.Forms.ComboBox cmbPrimary = new System.Windows.Forms.ComboBox();
                System.Windows.Forms.Label cmbPrimaryLbl = new System.Windows.Forms.Label();
                System.Windows.Forms.TrackBar tb = new System.Windows.Forms.TrackBar();
                System.Windows.Forms.Label tbLbl = new System.Windows.Forms.Label();
                tbLbl.Name = "tbLbl";
                tbLbl.SetBounds(470, 236, 70, 23);
                tbLbl.Text = "Bins";
                tbLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                tb.Name = "tbQ";
                tb.SetRange(0, 10);
                tb.SetBounds(350, 236, 120, 30);
                tb.Show();
                tb.TickFrequency = 1;
                tb.LargeChange = 1;
                tb.SmallChange = 1;
                cmbPrimaryLbl.Text = "Variable";
                cmbPrimary.Name = "cmbPrimary";
                cmbPrimaryLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                System.Drawing.Point cmbPt = new System.Drawing.Point(200, 236);
                System.Drawing.Point lblPt = new System.Drawing.Point(150, 236);
                cmbPrimary.Location = cmbPt;
                cmbPrimaryLbl.Location = lblPt;
                cmbPrimaryLbl.Size = new System.Drawing.Size(50, 23);
                cmbPrimary.Size = new System.Drawing.Size(120, 23);
                cmbPrimary.Items.AddRange(IndependentFieldNames);


                outFrm1.Text = "Probability Distribution";
                System.Windows.Forms.DataVisualization.Charting.ChartArea probArea = outFrm1.chrHistogram.ChartAreas.Add("Probs");
                System.Windows.Forms.DataVisualization.Charting.Title chTitle = outFrm1.chrHistogram.Titles.Add("T");
                probArea.AxisX.Title = "Selected Variable Values";
                probArea.AxisY.Title = "Proportion";
                probArea.AxisY.Maximum = 1;
                System.Windows.Forms.DataVisualization.Charting.Legend popLeg = outFrm1.chrHistogram.Legends.Add("Legend");
                chTitle.Alignment = System.Drawing.ContentAlignment.TopCenter;
                chTitle.Text = "Modeled Distributions";
                outFrm1.Controls.Add(cmbPrimaryLbl);
                outFrm1.Controls.Add(cmbPrimary);
                outFrm1.Controls.Add(tbLbl);
                outFrm1.Controls.Add(tb);
                outFrm1.Width = 540;
                outFrm = outFrm1;
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            return outFrm;
        }
        public static object generateRegressionGraphic(string[] IndependentFieldNames)
        {
            object outFrm = null;
            try
            {
                Forms.Stats.frmChart outFrm1 = new Forms.Stats.frmChart();
                System.Windows.Forms.ComboBox cmbPrimary = new System.Windows.Forms.ComboBox();
                System.Windows.Forms.Label cmbPrimaryLbl = new System.Windows.Forms.Label();
                System.Windows.Forms.TrackBar tb = new System.Windows.Forms.TrackBar();
                System.Windows.Forms.Label tbLbl = new System.Windows.Forms.Label();
                tbLbl.Name = "tbLbl";
                tbLbl.SetBounds(470, 236, 70, 23);
                tbLbl.Text = "Bins";
                tbLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                tb.Name = "tbQ";
                tb.SetRange(0, 10);
                tb.SetBounds(350, 236, 120, 30);
                tb.Show();
                tb.TickFrequency = 1;
                tb.LargeChange = 1;
                tb.SmallChange = 1;
                cmbPrimaryLbl.Text = "Variable";
                cmbPrimary.Name = "cmbPrimary";
                cmbPrimaryLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                System.Drawing.Point cmbPt = new System.Drawing.Point(200, 236);
                System.Drawing.Point lblPt = new System.Drawing.Point(150, 236);
                cmbPrimary.Location = cmbPt;
                cmbPrimaryLbl.Location = lblPt;
                cmbPrimaryLbl.Size = new System.Drawing.Size(50, 23);
                cmbPrimary.Size = new System.Drawing.Size(120, 23);
                cmbPrimary.Items.AddRange(IndependentFieldNames);
                outFrm1.Text = "Predicted Distribution";
                System.Windows.Forms.DataVisualization.Charting.ChartArea yArea = outFrm1.chrHistogram.ChartAreas.Add("Y");
                System.Windows.Forms.DataVisualization.Charting.Title chTitle = outFrm1.chrHistogram.Titles.Add("T");
                yArea.AxisX.Title = "Selected Variable Values";
                yArea.AxisY.Title = "Predicted Values";
                System.Windows.Forms.DataVisualization.Charting.Legend popLeg = outFrm1.chrHistogram.Legends.Add("Legend");
                chTitle.Alignment = System.Drawing.ContentAlignment.TopCenter;
                chTitle.Text = "Modeled Distributions";
                outFrm1.Controls.Add(cmbPrimaryLbl);
                outFrm1.Controls.Add(cmbPrimary);
                outFrm1.Controls.Add(tbLbl);
                outFrm1.Controls.Add(tb);
                outFrm1.Width = 540;
                outFrm = outFrm1;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            return outFrm;
        }

        public static object generateVariableImportanceGraphic(string[] IndependentFieldNames,string[] errorTypes)
        {
            object outFrm = null;
            try
            {
                Forms.Stats.frmChart outFrm1 = new Forms.Stats.frmChart();
                System.Windows.Forms.ComboBox cmbPrimary = new System.Windows.Forms.ComboBox();
                System.Windows.Forms.Label cmbPrimaryLbl = new System.Windows.Forms.Label();
                cmbPrimaryLbl.Text = "Error Type";
                cmbPrimary.Name = "cmbPrimary";
                cmbPrimaryLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                System.Drawing.Point cmbPt = new System.Drawing.Point(225, 236);
                System.Drawing.Point lblPt = new System.Drawing.Point(150, 236);
                cmbPrimary.Location = cmbPt;
                cmbPrimaryLbl.Location = lblPt;
                cmbPrimaryLbl.Size = new System.Drawing.Size(75, 23);
                cmbPrimary.Size = new System.Drawing.Size(120, 23);
                cmbPrimary.Items.AddRange(errorTypes);
                outFrm1.Text = "Variable Importance Graphs";
                System.Windows.Forms.DataVisualization.Charting.ChartArea yArea = outFrm1.chrHistogram.ChartAreas.Add("Y");
                System.Windows.Forms.DataVisualization.Charting.Title chTitle = outFrm1.chrHistogram.Titles.Add("T");
                yArea.AxisX.Title = "Variable";
                yArea.AxisY.Title = "Error Value";
                yArea.AxisX.MinorGrid.Enabled = false;
                yArea.AxisX.MajorGrid.Enabled = false;
                yArea.AxisY.MinorGrid.Enabled = false;
                yArea.AxisY.MajorGrid.Enabled = false;
                System.Windows.Forms.DataVisualization.Charting.Legend popLeg = outFrm1.chrHistogram.Legends.Add("Legend");
                chTitle.Alignment = System.Drawing.ContentAlignment.TopCenter;
                chTitle.Text = "Model Error Without Variable";
                outFrm1.Controls.Add(cmbPrimaryLbl);
                outFrm1.Controls.Add(cmbPrimary);
                outFrm1.Width = 540;
                outFrm = outFrm1;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            return outFrm;
        }
    }
}
