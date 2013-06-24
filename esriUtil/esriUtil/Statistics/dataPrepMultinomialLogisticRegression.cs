using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;

namespace esriUtil.Statistics
{
    public class dataPrepMultinomialLogisticRegression:dataPrepBase
    {
        public dataPrepMultinomialLogisticRegression()
        {
        }
        public dataPrepMultinomialLogisticRegression(string tablePath,string dependentField, string independentFields, string categoricalFields)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
        }
        public dataPrepMultinomialLogisticRegression(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
        }
        private MultinomialLogisticRegression mlr = null;
        public MultinomialLogisticRegression PlrModel
        {
            get
            {
                if (mlr == null) buildModel();
                return mlr;
            }

        }
        private Dictionary<string,List<string>> unVlDic = null;
        private int n = 0;
        private string[] allFieldNames;
        private void formatData()
        {
            string depFldName = DependentFieldNames[0];
            if (unVlDic == null) unVlDic = UniqueClassValues;
            int indCnt = IndependentFieldNames.Length;
            int depCnt = DependentFieldNames.Length;
            int rws = InTable.RowCount(null);
            n = rws;
            ICursor cur = InTable.Search(null, false);
            int clms = indCnt;
            int[] allFieldIndexArray = new int[clms];
            allFieldNames = new string[clms];
            for (int i = 0; i < indCnt; i++)
			{
                string lu = IndependentFieldNames[i];
                allFieldNames[i] = lu;
                allFieldIndexArray[i] = cur.FindField(lu);
                List<string> outSet = null;
                if(unVlDic.TryGetValue(lu,out outSet))
                {
                    int t = (outSet.Count()-1);
                    clms = clms+ t;
                }

			}
            categories = unVlDic[depFldName].ToArray();
            ncat = categories.Length;
            int depIndex = cur.FindField(depFldName);
            nvars = clms;
            independent = new double[rws][];
            dependent = new double[rws][];
            int rwCnt = 0;
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                int indMatrixClm = 0;
                double[] rwVls = new double[clms];
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
                independent[rwCnt] = rwVls;
                double[] depCode = new double[ncat];
                string dVl = rw.get_Value(depIndex).ToString();
                int index = unVlDic[depFldName].IndexOf(dVl);
                depCode[index] = 1;
                dependent[rwCnt] = depCode;
                rw = cur.NextRow();
                rwCnt += 1;
            }
            
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
        public override double[,] getMatrix()
        {
            if (independent == null) formatData();
            int clms = independent[0].Length;
            int tClms = independent[0].Length+1;
            int tRws = independent.Length;
            double[,] outMatrix = new double[tRws, tClms];
            for (int r = 0; r < tRws; r++)
            {
                for (int c = 0; c < tClms; c++)
                {
                    double vl = 0;
                    if (c < clms)
                    {
                        vl = independent[r][c];
                    }
                    else
                    {
                        vl = dependent[r].ToList().IndexOf(1);
                    }
                    outMatrix[r, c] = vl;
                    
                }
            }
            return outMatrix;
        }
        public override double[] getArray(string varName)
        {
            if (independent == null) formatData();
            int rws = independent[0].Length;
            double[] outArr = new double[rws];
            int index = allFieldNames.ToList().IndexOf(varName);
            if (index > -1)
            {
                for (int i = 0; i < rws; i++)
                {
                    outArr[i] = independent[i][index];
                }

            }
            else
            {
                for (int i = 0; i < rws; i++)
                {
                    outArr[i] = dependent[i].ToList().IndexOf(1);
                }
            }
            return outArr; 
        }
        private double delta = Double.NaN;
        private int iteration = 0;
        private int totit = 100000;
        private double converg = 1e-8;
        string[] categories = null;
        public string[] Categories
        {
            get
            {
                if (categories == null) buildModel();
                return categories;
            }
        }
        public int DegreesOfFredom
        {
            get
            {
                return (NumberOfCategories - 1) * NumberOfIndependentVariables;
            }
        }
        private int nvars = 1;
        public int NumberOfIndependentVariables
        {
            get
            {
                return nvars;
            }
        }
        private int ncat = 2;
        public int NumberOfCategories
        {
            get
            {
                return ncat;
            }
        }
        public double Delta
        {
            get
            {
                if (Double.IsNaN(delta)) buildModel();
                return delta;
            }
        }
        public int NumberOfIterationsToConverge
        {
            get
            {
                if (iteration==0) buildModel();
                return iteration;
            }
        }
        public int TotalIterations 
        { 
            get 
            {
                return totit; 
            } 
            set 
            { 
                totit = value; 
            } 
        }
        public double ConvergenceCriteria { get { return converg; } set { converg = value; } }
        private double[][] independent = null;
        private double[][] dependent = null;
        private MultinomialLogisticRegression buildModel()
        {
            if (independent == null) formatData();
            mlr = new MultinomialLogisticRegression(nvars,ncat);
            LowerBoundNewtonRaphson lbn = new LowerBoundNewtonRaphson(mlr);
            do
            {
                delta = lbn.Run(independent, dependent);
                iteration++;
            } while (iteration < totit && delta > converg);
            coefficients = mlr.Coefficients;
            standarderror = new double[ncat-1][];
            waldstat = new double[ncat - 1][];
            waldpvalue = new double[ncat - 1][];
            for (int i = 0; i < coefficients.Length; i++)
            {
                double[] steArr = new double[nvars + 1];
                double[] waldStatArr = new double[nvars + 1];
                double[] waldPvalueArr = new double[nvars + 1];
                for (int j = 0; j < nvars+1; j++)
			    {
                    Accord.Statistics.Testing.WaldTest wt = mlr.GetWaldTest(i, j);
                    steArr[j] = wt.StandardError;
                    waldStatArr[j] = wt.Statistic;
                    waldPvalueArr[j] = wt.PValue;
			    }
                waldstat[i]=waldStatArr;
                waldpvalue[i]=waldPvalueArr;
                standarderror[i]=steArr;
            }
            loglikelihood = mlr.GetLogLikelihood(independent, dependent);
            deviance = mlr.GetDeviance(independent, dependent);
            x2 = mlr.ChiSquare(independent, dependent).Statistic;
            pv = mlr.ChiSquare(independent, dependent).PValue; 
            return mlr;
        }
        private double[][] waldstat = null;
        public double[][] WaldStatistic
        {
            get
            {
                if (waldstat == null) buildModel();
                return waldstat;
            }

        }
        private double[][] waldpvalue = null;
        public double[][] WaldPvalue
        {
            get
            {
                if (waldpvalue == null) buildModel();
                return waldpvalue;
            }
            
        }
        private double deviance = Double.NaN;
        public double Deviance
        {
            get
            {
                if (Double.IsNaN(deviance)) buildModel();
                return deviance;
                
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
        public double PValue
        {
            get
            {
                if (Double.IsNaN(pv)) buildModel();
                return pv;
            }
        }
        private double x2 = Double.NaN;
        public double X2
        {
            get
            {
                if (Double.IsNaN(x2)) buildModel();
                return x2;
            }
        }
        private double[][] coefficients = null;
        public double[][] Coefficients 
        { 
            get 
            {
                if (coefficients == null) buildModel();
                return coefficients; 
            } 
        }
        private double[][] standarderror;
        public double[][] StandardError 
        { 
            get 
            {
                if (standarderror == null) buildModel();
                return standarderror; 
            } 
        }
        public ci[][] getConfidenceIntervalues(double alpha)
        {
            double zScore = Accord.Math.Normal.Inverse(1 - alpha / 2);
            int rLen = StandardError.Length;
            ci[][] outCi = new ci[rLen][];
            int aLen = StandardError[0].Length;
            for (int i = 0; i < rLen; i++)
            {
                for (int j = 0; j < aLen; j++)
                {
                    double vl = StandardError[i][j] * zScore;
                    double beta = Coefficients[i][j];
                    double lbeta = beta - vl;
                    double ubeta = beta + vl;
                    ci oci = new ci();
                    oci.LowerBound=lbeta;
                    oci.UpperBound=ubeta;
                    outCi[i][j] = oci;
                }
            }
            return outCi;

        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            if (mlr == null) buildModel();
            outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.PLR);
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(String.Join(",", Categories));
                sw.WriteLine(NumberOfIndependentVariables.ToString());
                sw.WriteLine(NumberOfCategories.ToString());
                sw.WriteLine(Delta.ToString());
                sw.WriteLine(NumberOfIterationsToConverge.ToString());
                sw.WriteLine(LogLikelihood.ToString());
                sw.WriteLine(Deviance.ToString());
                sw.WriteLine(X2.ToString());
                sw.WriteLine(PValue.ToString());
                sw.WriteLine(String.Join(",", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in sumValues select d.ToString()).ToArray()));
                for (int i = 0; i < Coefficients.Length; i++)
                {
                    string[] vlArr = (from double d in Coefficients[i] select d.ToString()).ToArray();
                    sw.WriteLine(String.Join(" ", vlArr));
                }
                for (int i = 0; i < StandardError.Length; i++)
                {
                    string[] vlArr = (from double d in StandardError[i] select d.ToString()).ToArray();
                    sw.WriteLine(String.Join(" ", vlArr));
                }
                for (int i = 0; i < WaldStatistic.Length; i++)
                {
                    string[] vlArr = (from double d in WaldStatistic[i] select d.ToString()).ToArray();
                    sw.WriteLine(String.Join(" ", vlArr));
                }
                for (int i = 0; i < WaldPvalue.Length; i++)
                {
                    string[] vlArr = (from double d in WaldPvalue[i] select d.ToString()).ToArray();
                    sw.WriteLine(String.Join(" ", vlArr));
                }
                sw.Close();
            }
            return outPath;
        }
        public void getPlrModel(string modelPath,bool BuildModel=false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.PLR)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a PLR Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                categories = sr.ReadLine().Split(new char[] { ',' });
                nvars = System.Convert.ToInt32(sr.ReadLine());
                ncat = System.Convert.ToInt32(sr.ReadLine());
                delta = System.Convert.ToDouble(sr.ReadLine());
                iteration = System.Convert.ToInt32(sr.ReadLine());
                loglikelihood = System.Convert.ToDouble(sr.ReadLine());
                deviance = System.Convert.ToDouble(sr.ReadLine());
                x2 = System.Convert.ToDouble(sr.ReadLine());
                pv = System.Convert.ToDouble(sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                coefficients = new double[ncat - 1][];
                standarderror = new double[ncat - 1][];
                waldstat = new double[ncat - 1][];
                waldpvalue = new double[ncat - 1][];
                double[] vlArr = new double[nvars + 1];
                for (int i = 0; i < ncat-1; i++)
                {
                    vlArr = (from string s in (sr.ReadLine().Split(new char[]{' '})) select System.Convert.ToDouble(s)).ToArray();
                    coefficients[i] = vlArr;
                }
                for (int i = 0; i < ncat-1; i++)
                {
                    vlArr = (from string s in (sr.ReadLine().Split(new char[] {' '})) select System.Convert.ToDouble(s)).ToArray();
                    standarderror[i] = vlArr;
                }
                for (int i = 0; i < ncat - 1; i++)
                {
                    vlArr = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                    waldstat[i] = vlArr;
                }
                for (int i = 0; i < ncat - 1; i++)
                {
                    vlArr = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                    waldpvalue[i] = vlArr;
                }
                sr.Close();
                if (BuildModel) buildModel();

            }
        }

        public void getReport(double alpha)
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "PLR Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Iterations = " + NumberOfIterationsToConverge.ToString());
            rd.addMessage("Convergence = " + Delta.ToString());
            rd.addMessage("LogLikelihood = " + LogLikelihood.ToString());
            rd.addMessage("Deviance = " + Deviance.ToString());
            rd.addMessage("Likelihood Ratio Chi-squared = " + X2.ToString() + " DF = " + DegreesOfFredom.ToString() + " p-value = " + PValue.ToString());
            rd.addMessage("\n\nClasses: " + String.Join(", ", Categories));
            rd.addMessage("Reference Class = " + Categories[0]);
            rd.addMessage("\nParameter coefficients:\n");
            rd.addMessage("intercept, " + String.Join(", ", IndependentFieldNames));
            for (int i = 0; i < Coefficients.Length; i++)
            {
                double[] c = Coefficients[i];
                rd.addMessage(String.Join(" ", (from double d in c select d.ToString()).ToArray()));
            }
            rd.addMessage("\nParameter standard error:");
            for (int i = 0; i < StandardError.Length; i++)
            {
                double[] c = StandardError[i];
                rd.addMessage(String.Join(", ", (from double d in c select d.ToString()).ToArray()));
            }
            rd.addMessage("\nParameter Wald Chi-Squared:");
            for (int i = 0; i < WaldStatistic.Length; i++)
            {
                double[] c = WaldStatistic[i];
                rd.addMessage(String.Join(", ", (from double d in c select d.ToString()).ToArray()));
            }
            rd.addMessage("\nParameter Wald P-value:");
            for (int i = 0; i < WaldPvalue.Length; i++)
            {
                double[] c = WaldPvalue[i];
                rd.addMessage(String.Join(", ", (from double d in c select d.ToString()).ToArray()));
            }
            rd.Show();
            rd.enableClose();
            if (System.Windows.Forms.MessageBox.Show("Do you want to build probability graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                //if (mlr == null) mlr = buildModel();
                Forms.Stats.frmChart hist = ModelHelper.generateProbabilityGraphic(IndependentFieldNames);
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
            for (int i = 0; i < Categories.Length; i++)
            {
                System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series.Add(Categories[i]);
                s.BorderWidth = 3;
                s.LegendText = Categories[i];
                s.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                s.ChartArea = "Probs";
            }
            for (int j = 0; j < xVlArr.Length; j++)
            {
                meanArray[cmbInd] = xVlArr[j];
                double[] probArr = computNew(meanArray);
                for (int k = 0; k < probArr.Length; k++)
                {
                    System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series[k];
                    s.Points.AddXY(xVlArr[j], probArr[k]);

                }


            }

        }
        private void cmbPrimary_SelectedValueChanged(object sender, EventArgs e)
        {

            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues(frm);




        }

        public double[] computNew(double[] input)
        {
            double[] outVlsArr = new double[NumberOfCategories];
            for (int i = 0; i < input.Length; i++)
            {
                double vl = input[i];
                if (Double.IsNaN(vl)) return outVlsArr;
            }
            double[] expArr = new double[NumberOfCategories - 1];
            double sumExp = 0;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                double[] c = Coefficients[i];
                double ovl = c[0];
                for (int j = 1; j < c.Length; j++)
                {
                    ovl += c[j]*input[j - 1];
                }
                double expOvl = Math.Exp(ovl);
                expArr[i] = expOvl;
                sumExp += expOvl;
            }
            double sumProb = 0;
            for (int i = 0; i < expArr.Length; i++)
            {
                double expVl = expArr[i];
                double prob = expVl / (1 + sumExp);
                outVlsArr[i + 1] = prob;
                sumProb += prob;
            }
            outVlsArr[0] = 1 - sumProb;
            return outVlsArr;
        }
    }
    public class ci
    {
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }

    }
}
