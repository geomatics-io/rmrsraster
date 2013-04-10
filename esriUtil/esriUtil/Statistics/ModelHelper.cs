﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using esriUtil.Statistics;

namespace esriUtil.Statistics
{
    public class ModelHelper
    {
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
                default:
                    Console.WriteLine("Can't make a raster out of model");
                    break;
            }
            return outRs;
        }

        private IRaster getTTestRaster()
        {
            dataPrepTTest ttest = new dataPrepTTest();
            ttest.buildModel(mdlp);
            return rsUtil.calcTTestFunction(coefRst, ttest);
        }

        private IRaster getClusterRaster()
        {
            dataPrepCluster clus = new dataPrepCluster();
            clus.buildModel(mdlp);
            return rsUtil.calcClustFunction(coefRst, clus);
        }

        private IRaster getPcaRaster()
        {
            dataPrepPrincipleComponents pca = new dataPrepPrincipleComponents();
            pca.buildModel(mdlp);
            return rsUtil.calcPrincipleComponentsFunction(coefRst, pca);
        }

        private IRaster getLogisticRegression()
        {
            dataPrepLogisticRegression lr = new dataPrepLogisticRegression();
            lr.getLrModel(mdlp);
            indvar = lr.IndependentFieldNames;
            depvar = lr.DependentFieldNames;
            double[][] coef = new double[1][];
            coef[0] = lr.Coefficients;
            return rsUtil.calcPolytomousLogisticRegressFunction(coefRst, coef);
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
            return rsUtil.calcSoftMaxNnetFunction(coefRst, sm);
        }

        private IRaster getRandomForestRaster()
        {
            dataPrepRandomForest rf = new dataPrepRandomForest();
            rf.getDfModel(mdlp);
            indvar = rf.IndependentFieldNames;
            depvar = rf.DependentFieldNames;
            return rsUtil.calcRandomForestFunction(coefRst, rf);
        }

        private IRaster getPlrRaster()
        {
            dataPrepMultinomialLogisticRegression plr = new dataPrepMultinomialLogisticRegression();
            plr.getPlrModel(mdlp);
            indvar = plr.IndependentFieldNames;
            depvar = plr.DependentFieldNames;
            double[][] coef = plr.Coefficients;
            return rsUtil.calcPolytomousLogisticRegressFunction(coefRst, coef);
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
            return rsUtil.calcRegressFunction(coefRst, coefLst);
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
            return rsUtil.calcRegressFunction(coefRst, coefLst);
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
                default:
                    break;
            }

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
            dataPrepCluster clus = new dataPrepCluster();
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
            if (report) rf.getReport();
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
        public void predictNewData(ITable inputTable)
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
                    predictLrData(inputTable,fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.MvlRegression:
                    predictMlrData(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.PLR:
                    predictPlrData(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.RandomForest:
                    predictRfData(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.SoftMax:
                    predictSmData(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.Cart:
                    predictCartData(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.L3:
                    predictL3Data(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.LogisticRegression:
                    predictLogisticReg(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.PCA:
                    predictPca(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.Cluster:
                    predictCluster(inputTable, fldIndexArr);
                    break;
                case dataPrepBase.modelTypes.TTEST:
                    predictTTest(inputTable, fldIndexArr);
                    break;
                default:
                    break;
            }
        }

        private void predictTTest(ITable inputTable, int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
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

        }

        private void predictCluster(ITable inputTable, int[] fldIndexArr)
        {
            dataPrepCluster clus = new dataPrepCluster();
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
            ICursor cur = inputTable.Update(null, false);
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

        private void predictPca(ITable inputTable, int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
                double[] nVl = pca.computNew(input);
                for (int i = 0; i < outFldNameIndex.Length; i++)
                {
                    double vl = nVl[i];
                    rw.set_Value(outFldNameIndex[i], vl);
                }
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictLogisticReg(ITable inputTable, int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
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
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictL3Data(ITable inputTable,int [] fldIndexArr)
        {
            throw new NotImplementedException();
        }

        private void predictCartData(ITable inputTable,int[] fldIndexArr)
        {
            throw new NotImplementedException();
        }

        private void predictSmData(ITable inputTable,int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
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
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictRfData(ITable inputTable,int[] fldIndexArr)
        {
            //need to simplify for regression analysis (don't need the mlc class)
            dataPrepRandomForest rf = new dataPrepRandomForest();
            rf.getDfModel(mdlp);
            string depVarName = DependentVariables[0];
            //System.Windows.Forms.MessageBox.Show("number of categories = " + rf.NumberOfClasses.ToString());
            string[] outFldNameArr = new string[rf.NumberOfClasses+1];
            int[] outFldNameIndex = new int[outFldNameArr.Length];
            string nName = depVarName + "_mlc";
            outFldNameArr[rf.NumberOfClasses] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeString,false);
            outFldNameIndex[rf.NumberOfClasses] = inputTable.Fields.FindField(outFldNameArr[rf.NumberOfClasses]);
            for (int i = 0; i < outFldNameArr.Length-1; i++)
            {
                nName = "p_"+rf.Categories[i];
                outFldNameArr[i] = geoUtil.createField(inputTable, nName, esriFieldType.esriFieldTypeDouble,false);
                outFldNameIndex[i] = inputTable.Fields.FindField(outFldNameArr[i]);
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
                double[] nVl = rf.computNew(input);
                double maxVl = 0;
                int maxP = 0;
                for (int i = 0; i < outFldNameIndex.Length-1; i++)
                {
                    double vl = nVl[i];
                    if (vl > maxVl)
                    {
                        maxVl = vl;
                        maxP = i;
                    }
                    rw.set_Value(outFldNameIndex[i], vl);
                }
                rw.set_Value(outFldNameIndex[rf.NumberOfClasses],rf.Categories[maxP]);
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictPlrData(ITable inputTable,int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
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
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictMlrData(ITable inputTable,int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
                for (int i = 0; i < mvlr.multivariateRegression.Length; i++)
                {
                    double nVl = mvlr.multivariateRegression[i].computeNew(input);
                    rw.set_Value(outFldNameIndex[i], nVl);
                }
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private void predictLrData(ITable inputTable,int[] fldIndexArr)
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
            ICursor cur = inputTable.Update(null, false);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    double vl = System.Convert.ToDouble(rw.get_Value(fldIndexArr[i]));
                    input[i] = vl;
                }
                double nVl = lr.computeNew(input);
                rw.set_Value(outFldIndex, nVl);
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);

        }
    }
}
