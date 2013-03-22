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
    public class dataPrepLogisticRegression:dataPrepBase
    {
        public dataPrepLogisticRegression()
        {
        }
        public dataPrepLogisticRegression(string tablePath,string dependentField, string independentFields, string categoricalFields)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
        }
        public dataPrepLogisticRegression(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
        }
        private Accord.Statistics.Analysis.LogisticRegressionAnalysis lr = null;
        public Accord.Statistics.Analysis.LogisticRegressionAnalysis LogisticModel
        {
            get
            {
                if (lr == null) buildModel();
                return lr;
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
            dependent = new double[rws];
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
                string dVl = rw.get_Value(depIndex).ToString();
                int index = unVlDic[depFldName].IndexOf(dVl);
                dependent[rwCnt] = index;
                rw = cur.NextRow();
                rwCnt += 1;
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
                        vl = dependent.ToList().IndexOf(1);
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
                    outArr[i] = dependent.ToList().IndexOf(1);
                }
            }
            return outArr; 
        }
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
        private double[] dependent = null;
        private Accord.Statistics.Analysis.LogisticRegressionAnalysis buildModel()
        {
            if (independent == null) formatData();
            lr = new Accord.Statistics.Analysis.LogisticRegressionAnalysis(independent,dependent);
            if (lr.Compute(ConvergenceCriteria, TotalIterations))
            {
                coefficients = lr.CoefficientValues;
                standarderror = lr.StandardErrors;
                waldstat = lr.WaldTests;
                loglikelihood = lr.LogLikelihood;
                deviance = lr.Deviance;
                x2 = lr.ChiSquare.Statistic;
                pv = lr.ChiSquare.PValue;
                return lr;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Could not compute logistic regression!!", "Compute", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }
        }
        private Accord.Statistics.Testing.WaldTest[] waldstat = null;
        public Accord.Statistics.Testing.WaldTest[] WaldStatistic
        {
            get
            {
                if (waldstat == null) buildModel();
                return waldstat;
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
        private double[] coefficients = null;
        public double[] Coefficients 
        { 
            get 
            {
                if (coefficients == null) buildModel();
                return coefficients; 
            } 
        }
        private double[] standarderror;
        public double[] StandardError 
        { 
            get 
            {
                if (standarderror == null) buildModel();
                return standarderror; 
            } 
        }
        public ci[] getConfidenceIntervalues(double alpha)
        {
            double zScore = Accord.Math.Normal.Inverse(1 - alpha / 2);
            int rLen = StandardError.Length;
            ci[] outCi = new ci[rLen];
            int aLen = StandardError.Length;
            for (int j = 0; j < aLen; j++)
            {
                double vl = StandardError[j] * zScore;
                double beta = Coefficients[j];
                double lbeta = beta - vl;
                double ubeta = beta + vl;
                ci oci = new ci();
                oci.LowerBound=lbeta;
                oci.UpperBound=ubeta;
                outCi[j] = oci;
            }
            return outCi;

        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            if (lr == null) buildModel();
            outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.LogisticRegression.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(String.Join(",", Categories));
                sw.WriteLine(NumberOfIndependentVariables.ToString());
                sw.WriteLine(NumberOfCategories.ToString());
                sw.WriteLine(LogLikelihood.ToString());
                sw.WriteLine(Deviance.ToString());
                sw.WriteLine(X2.ToString());
                sw.WriteLine(PValue.ToString());
                sw.WriteLine(String.Join(",",(from double d in Coefficients select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",",(from double d in StandardError select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",",(from Accord.Statistics.Testing.WaldTest d in WaldStatistic select d.Statistic.ToString()).ToArray()));
                sw.WriteLine(String.Join(",",(from Accord.Statistics.Testing.WaldTest d in WaldStatistic select d.PValue.ToString()).ToArray()));
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
                if (m != modelTypes.LogisticRegression)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a Logistic Regression Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                categories = sr.ReadLine().Split(new char[] { ',' });
                nvars = System.Convert.ToInt32(sr.ReadLine());
                ncat = System.Convert.ToInt32(sr.ReadLine());
                loglikelihood = System.Convert.ToDouble(sr.ReadLine());
                deviance = System.Convert.ToDouble(sr.ReadLine());
                x2 = System.Convert.ToDouble(sr.ReadLine());
                pv = System.Convert.ToDouble(sr.ReadLine());
                coefficients = (from string s in (sr.ReadLine().Split(new char[]{','})) select System.Convert.ToDouble(s)).ToArray();
                standarderror = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                waldstat = new Accord.Statistics.Testing.WaldTest[coefficients.Length];
                double[] waldStatVl = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                double[] waldStatPVl = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                for (int i = 0; i < waldStatVl.Length; i++)
                {
                    double wVl = waldStatVl[i];
                    Accord.Statistics.Testing.WaldTest wt = new Accord.Statistics.Testing.WaldTest(wVl);
                    waldstat[i] = wt;
                }
                sr.Close();
                if (BuildModel) buildModel();

            }
        }

        public void getReport(double alpha)
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Logistic Regression Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("LogLikelihood = " + LogLikelihood.ToString());
            rd.addMessage("Deviance = " + Deviance.ToString());
            rd.addMessage("Likelihood Ratio Chi-squared = " + X2.ToString() + " DF = " + DegreesOfFredom.ToString() + " p-value = " + PValue.ToString());
            rd.addMessage("\n\nClasses: " + String.Join(", ", Categories));
            rd.addMessage("Reference Class = " + Categories[0]);
            rd.addMessage("\nParameter coefficients:\n");
            rd.addMessage("intercept, " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Coef: " +String.Join(" ", (from double d in Coefficients select d.ToString()).ToArray()));
            rd.addMessage("STE:  " + String.Join(", ", (from double d in StandardError select d.ToString()).ToArray()));
            rd.addMessage("\n\nWald stats for coefficients:\nchi-sq: "+ String.Join(", ", (from Accord.Statistics.Testing.WaldTest d in WaldStatistic select d.Statistic.ToString()).ToArray()));
            rd.addMessage("p-value: " + String.Join(", ", (from Accord.Statistics.Testing.WaldTest d in WaldStatistic select d.PValue.ToString()).ToArray()));
            rd.Show();
            rd.enableClose();
        }
        /// <summary>
        /// return the probability of getting a positive outcome 1
        /// </summary>
        /// <param name="input">independent variable values</param>
        /// <returns>probability of 1</returns>
        public double computNew(double[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double vl = input[i];
                if (Double.IsNaN(vl)) return 0;
            }
            double ovl = Coefficients[0];
            for (int j = 1; j < Coefficients.Length; j++)
            {
                ovl += Coefficients[j]*input[j - 1];
            }
            double expOvl = Math.Exp(ovl);
            double prob = (expOvl / (1 + expOvl));
            return prob;
        }
    }
}
