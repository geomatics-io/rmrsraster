using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;

namespace esriUtil.Statistics
{
    public class dataPrepLinearReg : dataPrepBase
    {
        public dataPrepLinearReg()
        {
        }
        public dataPrepLinearReg(string tablePath, string dependentField, string independentFields, string categoricalFields, bool origin = false)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            intOrigin = origin;
        }
        public dataPrepLinearReg(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, bool origin = false)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            intOrigin = origin;
        }

        private bool intOrigin = false;
        public bool InterceptThroughOrigin
        {
            get
            {
                return intOrigin;
            }
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
        private double rmse = Double.NaN;
        public double RMSE
        {
            get
            {
                if(Double.IsNaN(rmse))buildModel();
                return rmse;
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
        private MultipleLinearRegressionAnalysis rA = null;
        public MultipleLinearRegressionAnalysis RegressionModel { get { if (rA == null)buildModel(); return rA; } }
        private void buildModel()
        {
            if (independentVls == null) getMatrix();
            if (intOrigin)
            {
                rA = new MultipleLinearRegressionAnalysis(independentVls, dependentVls, false);
            }
            else
            {
                rA = new MultipleLinearRegressionAnalysis(independentVls, dependentVls, true);
            }
            rA.Compute();
            rmse = rA.StandardError;
            ftest = rA.FTest.Statistic;
            pv = rA.FTest.PValue;
            r2 = rA.RSquared;
            ar2 = rA.RSquareAdjusted;
            double[]c = rA.CoefficientValues;
            double[] c2;
            int adj = 1;
            if (intOrigin)
            {
                c2 = new double[c.Length + 1];
                adj = 0;
            }
            else
            {
                c2 = new double[c.Length];
            }
            for (int i = 0; i < c2.Length - adj; i++)
            {
                c2[i + 1] = c[i];
            }
            if (intOrigin) c2[0] = 0;
            else c2[0] = c.Last();
            coefficients = c2;
            double[] s = rA.StandardErrors;
            double[] s2 = new double[c2.Length];
            for (int i = 0; i < s2.Length - adj; i++)
            {
                s2[i + 1] = s[i];
            }
            if (intOrigin) s2[0] = 0;
            else s2[0] = s.Last();
            standarderrors = s2;
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
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.LinearRegression.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(InterceptThroughOrigin.ToString());
                sw.WriteLine(RMSE);
                sw.WriteLine(FValue.ToString());
                sw.WriteLine(PValue.ToString());
                sw.WriteLine(Rsquared.ToString());
                sw.WriteLine(AdjustedRsquared.ToString());
                sw.WriteLine(String.Join(" ",(from double d in Coefficients select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in StandardErrors select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in sumValues select d.ToString()).ToArray()));
                sw.Close();
            }
            return outPath;
        }
        public void getLrModel(string modelPath,bool BuildModel=false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.LinearRegression)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a Linear Regression Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                nvars = System.Convert.ToInt32(sr.ReadLine());
                string vl = sr.ReadLine();
                if (vl.ToLower() == "false") intOrigin = false;
                else intOrigin = true;
                rmse=System.Convert.ToDouble(sr.ReadLine());
                ftest = System.Convert.ToDouble(sr.ReadLine());
                pv = System.Convert.ToDouble(sr.ReadLine());
                r2 = System.Convert.ToDouble(sr.ReadLine());
                ar2 = System.Convert.ToDouble(sr.ReadLine());
                coefficients = (from string s in (sr.ReadLine().Split(new char[]{' '})) select System.Convert.ToDouble(s)).ToArray();
                standarderrors = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sr.Close();

            }
            if(BuildModel)buildModel();
        }
        private double r2 = Double.NaN;
        public double Rsquared
        {
            get
            {
                if (Double.IsNaN(r2)) buildModel();
                return r2;
            }
        }
        private double ar2 = Double.NaN;
        public double AdjustedRsquared
        {
            get
            {
                if (Double.IsNaN(ar2)) buildModel();
                return ar2;
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
        private double[] standarderrors = null; 
        public double[] StandardErrors
        {
            get
            {
                if (standarderrors == null) buildModel();
                return standarderrors;
            }
        }
        private double ftest = Double.NaN;
        public double FValue
        {
            get
            {
                if (Double.IsNaN(ftest)) buildModel();
                return ftest;
            }
        }
        private double pv = Double.NaN;
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
            rd.Text = "Regression Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Dependent field = " + DependentFieldNames[0]);
            rd.addMessage("Independent fields = " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Sample size = " + SampleSize.ToString());
            rd.addMessage("Intercept Through Origin = " + InterceptThroughOrigin.ToString());
            rd.addMessage("F-statistic = " + FValue.ToString() + " p-value = " + PValue.ToString());
            rd.addMessage("RMSE = " + RMSE.ToString());
            rd.addMessage("R2 = " + Rsquared.ToString());
            rd.addMessage("Adj-R2 = " + AdjustedRsquared.ToString() + "\n\nCoefficents and standard errors:\n");
            rd.addMessage("Param: Intercept, " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Coef:  " + string.Join(", ", (from double d in Coefficients select d.ToString()).ToArray()));
            rd.addMessage("STE:   " + string.Join(", ", (from double d in StandardErrors select d.ToString()).ToArray()) + "\n");
            rd.Show();
            rd.enableClose();
            Forms.Stats.frmChart hist = ModelHelper.generateRegressionGraphic(IndependentFieldNames);
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged);
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            tb.Scroll += new EventHandler(tb_RegionChanged);
            hist.chrHistogram.Show();
            cmbPrimary.SelectedItem = IndependentFieldNames[0];
            hist.Show();
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
            double rg = Coefficients[0];
            for (int i = 1; i < Coefficients.Length; i++)
            {
                rg += input[i - 1] * Coefficients[i];
            }
            return rg;
        }
    }
}
