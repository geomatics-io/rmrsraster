using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;

namespace esriUtil.Statistics
{
    public class dataPrepGlm : dataPrepBase
    {
        public enum LinkFunction {Absolute,Cauchit,Identity,Inverse,InverseSquared,Logit,Log,LogLog,Probit,Sin,Threshold}
        public dataPrepGlm()
        {
        }
        public dataPrepGlm(string tablePath, string dependentField, string independentFields, string categoricalFields, bool origin = false)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            
        }
        public dataPrepGlm(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, bool origin = false)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
        }

        private Dictionary<string, List<string>> unVlDic = null;

        private string[] allFieldNames = null;
        private double[][] independentVls = null;
        private double[] dependentVls = null;
        private int nvars = 0;
        public int NumberOfVariables
        { 
            get 
            {
                if (nvars == 0) buildModel();
                return nvars; 
            }
        }
        private int n = 0;
        public int SampleSize
        {
            get
            {
                if (n == 0) buildModel();
                return n;
            }
        }
        private double[] stdError = null;
        public double[] StdError
        {
            get
            {
                if(stdError==null)buildModel();
                return stdError;
            }
        }
        public override double[,] getMatrix()
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            int indCnt = IndependentFieldNames.Length;
            int depCnt = DependentFieldNames.Length;

            int rws = InTable.RowCount(null);
            n = rws;
            ICursor cur = InTable.Search(null, false);
            string depFldName = DependentFieldNames[0];
            int depIndex = cur.FindField(depFldName);
            int clms = indCnt;
            int[] allFieldIndexArray = new int[clms];
            allFieldNames = new string[clms + 1];
            for (int i = 0; i < indCnt; i++)
            {
                string lu = IndependentFieldNames[i];
                allFieldNames[i] = lu;
                allFieldIndexArray[i] = cur.FindField(lu);
                List<string> outSet = null;
                if (unVlDic.TryGetValue(lu, out outSet))
                {
                    int t = (outSet.Count() - 1);
                    clms = clms + t;
                }

            }


            allFieldNames[indCnt] = depFldName;
            nvars = clms;
            independentVls = new double[rws][];
            dependentVls = new double[rws];
            int rwCnt = 0;
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                double[] rwVls = new double[clms];
                int indMatrixClm = 0;
                for (int i = 0; i < allFieldIndexArray.Length; i++)
                {
                    string lu = allFieldNames[i];
                    int fldClmCntT = 1;
                    int indexFld = allFieldIndexArray[i];
                    object vl = rw.get_Value(indexFld);
                    updateMinMaxSum(vl, i);
                    double dblVl = 0;
                    try
                    {
                        string strVl = vl.ToString();
                        List<string> unVl = null;
                        if (unVlDic.TryGetValue(lu, out unVl))
                        {
                            int fldClmCntP = unVl.IndexOf(strVl);
                            fldClmCntT = unVl.Count() - fldClmCntP;
                            indMatrixClm += fldClmCntP;
                            dblVl = 1;
                        }
                        else
                        {
                            dblVl = System.Convert.ToDouble(strVl);
                        }
                    }
                    catch
                    {
                        dblVl = 0;
                    }
                    rwVls[indMatrixClm] = dblVl;
                    indMatrixClm += fldClmCntT;

                }
                independentVls[rwCnt] = rwVls;
                dependentVls[rwCnt] = System.Convert.ToDouble(rw.get_Value(depIndex));
                rw = cur.NextRow();
                rwCnt += 1;
            }
            return null;
        }
        private void updateMinMaxSum(object vl, int i)
        {
            try
            {
                double nVl = System.Convert.ToDouble(vl);
                if (nVl < minValues[i]) minValues[i] = nVl;
                if (nVl > maxValues[i]) maxValues[i] = nVl;
                sumValues[i] = sumValues[i] + nVl;
            }
            catch
            {
            }
        }
        public string[] getUniqueValues(string classVariable)
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            return unVlDic[classVariable].ToArray();
        }
        public override double[] getArray(string varName)
        {
            if (independentVls == null) getMatrix();
            int matrixClmIndex = allFieldNames.ToList().IndexOf(varName);
            double[] outArray = new double[independentVls.GetUpperBound(0) + 1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = independentVls[i][matrixClmIndex];
            }
            return outArray;
        }
        public double[] getArrayByIndex(int varIndex)
        {
            if (independentVls == null) getMatrix();
            double[] outArray = new double[independentVls.GetUpperBound(0) + 1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = independentVls[i][varIndex];
            }
            return outArray;
        }
        private Accord.Statistics.Models.Regression.GeneralizedLinearRegression glm = null;
        public Accord.Statistics.Models.Regression.GeneralizedLinearRegression GlmModel { get { if (glm == null)buildModel(); return glm; } }
        private void buildModel()
        {
            if (independentVls == null) getMatrix();
            Accord.Statistics.Links.ILinkFunction lFunc = new Accord.Statistics.Links.IdentityLinkFunction();
            switch (Link)
            {
                case LinkFunction.Absolute:
                    lFunc = new Accord.Statistics.Links.AbsoluteLinkFunction();
                    break;
                case LinkFunction.Cauchit:
                    lFunc = new Accord.Statistics.Links.CauchitLinkFunction();
                    break;
                case LinkFunction.Inverse:
                    lFunc = new Accord.Statistics.Links.InverseLinkFunction();
                    break;
                case LinkFunction.InverseSquared:
                    lFunc = new Accord.Statistics.Links.InverseSquaredLinkFunction();
                    break;
                case LinkFunction.Logit:
                    lFunc = new Accord.Statistics.Links.LogitLinkFunction();
                    break;
                case LinkFunction.Log:
                    lFunc = new Accord.Statistics.Links.LogLinkFunction();
                    break;
                case LinkFunction.LogLog:
                    lFunc = new Accord.Statistics.Links.LogLogLinkFunction();
                    break;
                case LinkFunction.Probit:
                    lFunc = new Accord.Statistics.Links.ProbitLinkFunction();
                    break;
                case LinkFunction.Sin:
                    lFunc = new Accord.Statistics.Links.SinLinkFunction();
                    break;
                case LinkFunction.Threshold:
                    lFunc = new Accord.Statistics.Links.ThresholdLinkFunction();
                    break;
                default:
                    break;
            }
            glm = new Accord.Statistics.Models.Regression.GeneralizedLinearRegression(lFunc,NumberOfVariables);
            Accord.Statistics.Models.Regression.Fitting.IterativeReweightedLeastSquares isl = new Accord.Statistics.Models.Regression.Fitting.IterativeReweightedLeastSquares(glm);
            delta = 0;
            do
            {
                delta = isl.Run(independentVls,dependentVls);
                iterations += 1;
            }while (delta>converge&&iterations<totiterations);
            coefficients = glm.Coefficients;
            stdError = glm.StandardErrors;
            loglikelihood = glm.GetLogLikelihood(independentVls,dependentVls);
            if (Double.IsNaN(loglikelihood)) loglikelihood = 0;
            loglikelihoodratio = glm.GetLogLikelihoodRatio(independentVls, dependentVls, glm);
            if (Double.IsNaN(loglikelihoodratio)) loglikelihoodratio = 0;
            deviance = glm.GetDeviance(independentVls,dependentVls);
            if (Double.IsNaN(deviance)) deviance = 0;
            Accord.Statistics.Testing.ChiSquareTest chiTest = glm.ChiSquare(independentVls, dependentVls);
            pv = chiTest.PValue;
            if (Double.IsNaN(pv)) pv = 0;
            chisqr = chiTest.Statistic;
            if (double.IsNaN(chisqr)) chisqr = 0;
            waldTestValues = new double[IndependentFieldNames.Length];
            waldTestPValues = new double[waldTestValues.Length];
            for (int i = 0; i < waldTestValues.Length; i++)
            {
                Accord.Statistics.Testing.WaldTest wTest = glm.GetWaldTest(i);
                double wS = wTest.Statistic;
                double wP = wTest.PValue;
                //if (Double.IsNaN(wS)) wS = 0;
                //if (Double.IsNaN(wP)) wP = 0;
                waldTestValues[i] = wS;
                waldTestPValues[i] = wP;
            }
            //if (stdError.Length != coefficients.Length) stdError = new double[coefficients.Length];
            //for (int i = 0; i < stdError.Length; i++)
            //{
            //    double vl = stdError[i];
            //    if (Double.IsNaN(vl)) stdError[i] = 0;
            //}
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.GLM.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(Iterations.ToString());
                sw.WriteLine(DeltaC.ToString());
                sw.WriteLine(LogLikelihood);
                sw.WriteLine(LogLikelihoodratio);
                sw.WriteLine(PValue.ToString());
                sw.WriteLine(Deviance.ToString());
                sw.WriteLine(ChiSquare.ToString());
                sw.WriteLine(linkfunction.ToString());
                sw.WriteLine(String.Join(" ",(from double d in Coefficients select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in StdError select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in waldTestValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in waldTestPValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in sumValues select d.ToString()).ToArray()));
                sw.Close();
            }
            return outPath;
        }
        public void getGlmModel(string modelPath, bool BuildModel=false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.GLM)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a GLM!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                nvars = System.Convert.ToInt32(sr.ReadLine());
                iterations = System.Convert.ToInt32(sr.ReadLine());
                delta = System.Convert.ToDouble(sr.ReadLine());
                loglikelihood=System.Convert.ToDouble(sr.ReadLine());
                loglikelihoodratio = System.Convert.ToDouble(sr.ReadLine());
                pv = System.Convert.ToDouble(sr.ReadLine());
                deviance = System.Convert.ToDouble(sr.ReadLine());
                chisqr = System.Convert.ToDouble(sr.ReadLine());
                linkfunction = (LinkFunction)Enum.Parse(typeof(LinkFunction), sr.ReadLine());
                coefficients = (from string s in (sr.ReadLine().Split(new char[]{' '})) select System.Convert.ToDouble(s)).ToArray();
                stdError = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                waldTestValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                waldTestPValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sr.Close();

            }
            if (BuildModel)
            {
                Accord.Statistics.Links.ILinkFunction lFunc = new Accord.Statistics.Links.IdentityLinkFunction();
                switch (Link)
                {
                    case LinkFunction.Absolute:
                        lFunc = new Accord.Statistics.Links.AbsoluteLinkFunction();
                        break;
                    case LinkFunction.Cauchit:
                        lFunc = new Accord.Statistics.Links.CauchitLinkFunction();
                        break;
                    case LinkFunction.Inverse:
                        lFunc = new Accord.Statistics.Links.InverseLinkFunction();
                        break;
                    case LinkFunction.InverseSquared:
                        lFunc = new Accord.Statistics.Links.InverseSquaredLinkFunction();
                        break;
                    case LinkFunction.Logit:
                        lFunc = new Accord.Statistics.Links.LogitLinkFunction();
                        break;
                    case LinkFunction.Log:
                        lFunc = new Accord.Statistics.Links.LogLinkFunction();
                        break;
                    case LinkFunction.LogLog:
                        lFunc = new Accord.Statistics.Links.LogLogLinkFunction();
                        break;
                    case LinkFunction.Probit:
                        lFunc = new Accord.Statistics.Links.ProbitLinkFunction();
                        break;
                    case LinkFunction.Sin:
                        lFunc = new Accord.Statistics.Links.SinLinkFunction();
                        break;
                    case LinkFunction.Threshold:
                        lFunc = new Accord.Statistics.Links.ThresholdLinkFunction();
                        break;
                    default:
                        break;
                }
                glm = new Accord.Statistics.Models.Regression.GeneralizedLinearRegression(lFunc, coefficients, stdError);
            }
        }
        private double chisqr = Double.NaN;
        public double ChiSquare
        {
            get
            {
                if (Double.IsNaN(chisqr)) buildModel();
                return chisqr;
            }
        }
        private double loglikelihoodratio = Double.NaN;
        public double LogLikelihoodratio
        {
            get
            {
                if (Double.IsNaN(loglikelihoodratio)) buildModel();
                return loglikelihoodratio;
            }
        }
        double[] coefficients = null;
        public double[] Coefficients
        {
            get
            {
                if (coefficients == null) buildModel();
                return coefficients;
                
            }
        }
        
        private double loglikelihood = Double.NaN;
        public double LogLikelihood
        {
            get
            {
                if (Double.IsNaN(loglikelihood)) buildModel();
                return loglikelihood;
            }
        }
        private double pv = Double.NaN;
        private LinkFunction linkfunction = LinkFunction.Identity;
        private int iterations = 0;
        private int totiterations = 100000;
        public int Iterations { get { return iterations; } }
        public int MaxIterations { get { return totiterations; } set { totiterations = value; } }
        private double converge = 0.000001;
        private double deviance = Double.NaN;
        private double[] waldTestValues;
        private double[] waldTestPValues;
        private double delta = Double.NaN;
        public double DeltaC { get { return delta; } }
        public double Deviance { get { if (Double.IsNaN(deviance))buildModel(); return deviance; } }
        public double Converge { get { return converge; } set { converge = value; } }
        public LinkFunction Link { get { return linkfunction; } set { linkfunction = value; } }
        public double PValue
        {
            get
            {
                if (Double.IsNaN(pv)) buildModel();
                return pv;
            }
        }


        public void getReport(double alpha)
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "GLM Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Dependent field = " + DependentFieldNames[0]);
            rd.addMessage("Independent fields = " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Sample size = " + SampleSize.ToString());
            rd.addMessage("Iteration = " + Iterations.ToString());
            rd.addMessage("Delta Convergence " + DeltaC.ToString());
            rd.addMessage("Chi-Sqr = " + ChiSquare.ToString() + " p-value = " + PValue.ToString());
            rd.addMessage("Deviance = " + Deviance.ToString());
            rd.addMessage("Log Likelihood = " + LogLikelihood.ToString());
            rd.addMessage("Log Likelihood Ratio = " + LogLikelihoodratio.ToString() + "\n\nCoefficents and standard errors:\n");
            rd.addMessage("Param: Intercept, " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Coef:  " + string.Join(", ", (from double d in Coefficients select d.ToString()).ToArray()));
            rd.addMessage("STE:   " + string.Join(", ", (from double d in StdError select d.ToString()).ToArray()) + "\n");
            rd.Show();
            rd.enableClose();
            if (System.Windows.Forms.MessageBox.Show("Do you want to build distribution graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (glm == null)
                {
                    Accord.Statistics.Links.ILinkFunction lFunc = new Accord.Statistics.Links.IdentityLinkFunction();
                    switch (Link)
                    {
                        case LinkFunction.Absolute:
                            lFunc = new Accord.Statistics.Links.AbsoluteLinkFunction();
                            break;
                        case LinkFunction.Cauchit:
                            lFunc = new Accord.Statistics.Links.CauchitLinkFunction();
                            break;
                        case LinkFunction.Inverse:
                            lFunc = new Accord.Statistics.Links.InverseLinkFunction();
                            break;
                        case LinkFunction.InverseSquared:
                            lFunc = new Accord.Statistics.Links.InverseSquaredLinkFunction();
                            break;
                        case LinkFunction.Logit:
                            lFunc = new Accord.Statistics.Links.LogitLinkFunction();
                            break;
                        case LinkFunction.Log:
                            lFunc = new Accord.Statistics.Links.LogLinkFunction();
                            break;
                        case LinkFunction.LogLog:
                            lFunc = new Accord.Statistics.Links.LogLogLinkFunction();
                            break;
                        case LinkFunction.Probit:
                            lFunc = new Accord.Statistics.Links.ProbitLinkFunction();
                            break;
                        case LinkFunction.Sin:
                            lFunc = new Accord.Statistics.Links.SinLinkFunction();
                            break;
                        case LinkFunction.Threshold:
                            lFunc = new Accord.Statistics.Links.ThresholdLinkFunction();
                            break;
                        default:
                            break;
                    }
                    glm = new Accord.Statistics.Models.Regression.GeneralizedLinearRegression(lFunc, coefficients, stdError);
                }
                Forms.Stats.frmChart hist = ModelHelper.generateRegressionGraphic(IndependentFieldNames);
                System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
                cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged);
                System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
                tb.Scroll += new EventHandler(tb_RegionChanged);
                hist.chrHistogram.Show();
                cmbPrimary.SelectedItem = IndependentFieldNames[0];
                hist.Show();
            }
        }

        void tb_RegionChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues(frm);
        }
        public void updateFormValues(Forms.Stats.frmChart hist)
        {
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            System.Windows.Forms.DataVisualization.Charting.Chart ch = hist.chrHistogram;
            ch.Series.Clear();
            string cmbTxt = cmbPrimary.Text;
            int tbVl = tb.Value;
            double oVl = tbVl / 10d;
            int cmbInd = System.Array.IndexOf(IndependentFieldNames, cmbTxt);
            double[] meanArray = new double[sumValues.Length];
            for (int i = 0; i < meanArray.Length; i++)
            {
                double mV = minValues[i];
                meanArray[i] = mV + ((maxValues[i] - mV) * oVl);
            }
            double mVl = minValues[cmbInd];
            double rng = maxValues[cmbInd] - mVl;
            double stp = rng / 10;
            double[] xVlArr = new double[10];
            for (int i = 0; i < 10; i++)
            {
                xVlArr[i] = (i * stp) + mVl;
            }
            System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series.Add(DependentFieldNames[0]);
            s.BorderWidth = 3;
            s.LegendText = DependentFieldNames[0];
            s.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            s.ChartArea = "Y";
            for (int j = 0; j < xVlArr.Length; j++)
            {
                meanArray[cmbInd] = xVlArr[j];
                double vl = computeNew(meanArray);
                s.Points.AddXY(xVlArr[j], vl);
            }

        }
        private void cmbPrimary_SelectedValueChanged(object sender, EventArgs e)
        {

            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues(frm);




        }

        public double computeNew(double[] input)
        {
            return glm.Compute(input);
        }
    }
}
