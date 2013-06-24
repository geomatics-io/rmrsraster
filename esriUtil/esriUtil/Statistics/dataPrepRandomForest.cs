using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Statistics
{
    public class dataPrepRandomForest: dataPrepBase
    {
        public dataPrepRandomForest()
        {
        }
        public dataPrepRandomForest(string tablePath,string dependentField, string independentFields, string categoricalFields, int trees, double ratio)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            nTrees = trees;
            r = ratio;
            
        }
        public dataPrepRandomForest(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, int trees, double ratio)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            nTrees = trees;
            r = ratio;   
        }
        public dataPrepRandomForest(string tablePath, string dependentField, string independentFields, string categoricalFields, int trees, double ratio, int nSplitVar)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            nTrees = trees;
            r = ratio;
            advance = true;
            nrndvars = nSplitVar; 
        }
        public dataPrepRandomForest(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, int trees, double ratio, int nSplitVar)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            nTrees = trees;
            r = ratio;
            advance = true;
            nrndvars = nSplitVar;
        }
        private bool reg = false;
        public bool Regression
        {
            get
            {
                return reg;
            }
        }
        private bool advance = false;
        public bool Advanced
        {
            get
            {
                return advance;
            }
        }
        private string[] categories = null;
        public string[] Categories 
        { 
            get 
            {
                if (!reg&&categories == null) getDfModel();
                return categories; 
            } 
        }
        private int nTrees = 75;
        public int NumberOfTrees
        {
            get
            {
                return nTrees;
            }
            set
            {
                nTrees = value;
            }
        }
        private double r = 0.30;
        public double Ratio
        {
            get
            {
                return r;
            }
            set
            {
                r = value;
            }
        }
        private int nrndvars = 2;
        public int NumberOfSplitVariables
        {
            get
            {
                return nrndvars;
            }
            set
            {
                nrndvars = value;
                if (nrndvars > 0)
                {
                    advance = true;
                }
                else
                {
                    advance = false;
                }
            }
        }
        private Dictionary<string, List<string>> unVlDic = null;
        private string[] allFieldNames = null;
        private double[,] outMatrix = null;
        public int NumberOfVariables { get { return nvars; } }
        private int nvars = 0;
        private int nclasses = 1;
        private int n = 0;
        public int SampleSize
        {
            get
            {
                return n;
            }
        }
        public override double[,] getMatrix()
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            checkRegression();
            //Console.WriteLine("Reg value = " + Regression.ToString());
            int indCnt = IndependentFieldNames.Length;
            int depCnt = DependentFieldNames.Length;
            int rws = InTable.RowCount(null);
            n = rws;
            ICursor cur = InTable.Search(null, false);
            int clms = IndependentFieldNames.Length + DependentFieldNames.Length;
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
            
            for (int i = 0; i < depCnt; i++)
            {
                string lu = DependentFieldNames[i];
                if (!reg)
                {
                    categories = unVlDic[lu].ToArray();
                }
                allFieldNames[i + indCnt] = lu;
                allFieldIndexArray[i + indCnt] = cur.FindField(lu);
            }
            
            nvars = clms - 1;
            //Console.WriteLine("Total Rows Columns  = " + rws.ToString() + ", " + clms.ToString());
            outMatrix = new double[rws,clms];
            int rwCnt = 0;
            IRow rw = cur.NextRow();
            while (rw != null)
            {
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
                            if (i == allFieldIndexArray.Length - 1)
                            {
                                dblVl = fldClmCntP;
                            }
                            else
                            {
                                fldClmCntT = unVl.Count() - fldClmCntP;
                                indMatrixClm += fldClmCntP;
                                dblVl = 1;
                            }
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
                    //Console.WriteLine("Matrix R:C = " + rwCnt.ToString() + ", " + indMatrixClm.ToString());
                    outMatrix[rwCnt, indMatrixClm] = dblVl;
                    indMatrixClm += fldClmCntT;

                }
                rw = cur.NextRow();
                rwCnt += 1;
            }
            return outMatrix;
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
        private void checkRegression()
        {
            bool fKey = false;
            string vl="";
            for (int i = 0; i < DependentFieldNames.Length; i++)
            {
                vl = DependentFieldNames[i];
                if (unVlDic.ContainsKey(vl))
                {
                    fKey = true;
                    break;
                }
            }
            if (fKey)
            {
                reg = false;
                nclasses = unVlDic[vl].Count;
            }
            else
            {
                reg = true;
                nclasses = 1;
            }
        }
        public string[] getUniqueValues(string classVariable)
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            return unVlDic[classVariable].ToArray();
        }
        public override double[] getArray(string varName)
        {
            if (outMatrix==null) getMatrix();
            int matrixClmIndex = allFieldNames.ToList().IndexOf(varName);
            double[] outArray = new double[outMatrix.GetUpperBound(0)+1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = outMatrix[matrixClmIndex, i];
            }
            return outArray;
        }
        public double[] getArrayByIndex(int varIndex)
        {
            if (outMatrix == null) getMatrix();
            double[] outArray = new double[outMatrix.GetUpperBound(0) + 1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = outMatrix[i,varIndex];
            }
            return outArray;
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            if (df == null) getDfModel();
            outPath = outModelPath;
            string s_out;
            try
            {
                alglib.dfserialize(df, out s_out);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                s_out = "rebuild";
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.RandomForest.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",",IndependentFieldNames));
                sw.WriteLine(String.Join(",",DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(String.Join(",", Categories));
                sw.WriteLine(NumberOfTrees.ToString());
                sw.WriteLine(Ratio.ToString());
                sw.WriteLine(NumberOfSplitVariables.ToString());
                sw.WriteLine(Regression.ToString());
                sw.WriteLine(Advanced.ToString());
                sw.WriteLine(getAllVariableNames());
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(NumberOfClasses.ToString());
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(AverageCrossEntropyError.ToString());
                sw.WriteLine(AverageError.ToString());
                sw.WriteLine(AverageRelativeError.ToString());
                sw.WriteLine(RMSE.ToString());
                sw.WriteLine(RelativeClassificationError.ToString());
                sw.WriteLine(OOBAverageCrossEntropyError.ToString());
                sw.WriteLine(OOBAverageError.ToString());
                sw.WriteLine(OOBAverageRelativeError.ToString());
                sw.WriteLine(OOBRelativeClassificationError.ToString());
                sw.WriteLine(OOBRMSE.ToString());
                sw.WriteLine(String.Join(",",(from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in sumValues select d.ToString()).ToArray()));
                sw.WriteLine(s_out);
                sw.Close();
            }
            return outPath;
        }

        private string getAllVariableNames()
        {
            string[] allvn = new string[IndependentFieldNames.Length];
            int cnt = 0;
            foreach (string s in IndependentFieldNames)
            {
                string vlS = s;
                List<string> outLst = new List<string>();
                if (unVlDic.TryGetValue(s, out outLst))
                {
                    vlS = String.Join(",", outLst.ToArray());
                }
                allvn[cnt] = vlS;
                cnt += 1;
            }
            return String.Join(",", allvn);
        }
        public int NumberOfClasses { get { return nclasses; } }
        public alglib.decisionforest getDfModel(string modelPath,bool rebuildIfTooBig=true)
        {
            string s_in = null;
            
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.RandomForest)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a Random Forest Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return null;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[]{','});
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                categories = sr.ReadLine().Split(new char[] { ',' });
                nTrees = System.Convert.ToInt32(sr.ReadLine());
                r = System.Convert.ToDouble(sr.ReadLine());
                nrndvars = System.Convert.ToInt32(sr.ReadLine());
                reg = System.Convert.ToBoolean(sr.ReadLine());
                advance = System.Convert.ToBoolean(sr.ReadLine());
                string allvariablenames = sr.ReadLine();
                nvars = System.Convert.ToInt32(sr.ReadLine());
                nclasses = System.Convert.ToInt32(sr.ReadLine());
                n=System.Convert.ToInt32(sr.ReadLine());
                avgcee = System.Convert.ToDouble(sr.ReadLine());
                avge = System.Convert.ToDouble(sr.ReadLine());
                avgre = System.Convert.ToDouble(sr.ReadLine());
                rmse = System.Convert.ToDouble(sr.ReadLine());
                rce = System.Convert.ToDouble(sr.ReadLine());
                oobavgcee = System.Convert.ToDouble(sr.ReadLine());
                oobavge = System.Convert.ToDouble(sr.ReadLine());
                oobavgre = System.Convert.ToDouble(sr.ReadLine());
                oobrmse = System.Convert.ToDouble(sr.ReadLine());
                oobrce = System.Convert.ToDouble(sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                s_in = sr.ReadToEnd();
                //Console.WriteLine("Sin = '"+s_in+"'");
                sr.Close();
            }
            if (!s_in.StartsWith("rebuild"))
            {
                alglib.dfunserialize(s_in, out df);
            }
            else
            {
                if (rebuildIfTooBig)
                {
                    //Console.WriteLine("Rebuilding Model!!");
                    getDfModel();
                }
                else
                {
                    //Console.WriteLine("Not Rebuilding model");
                }
            }
            //Console.WriteLine(df == null);
            return df;
        }
        private alglib.decisionforest df = null;
        public alglib.decisionforest DecisionForest
        {
            get
            {
                if (df == null) getDfModel();
                return df;
            }
        }
        private alglib.dfreport rep = null;
        private int info;
        private double rmse = Double.NaN;
        private double avgcee = Double.NaN;
        private double avge = Double.NaN;
        private double avgre = Double.NaN;
        private double rce = Double.NaN;
        private double oobrmse = Double.NaN;
        private double oobavgcee = Double.NaN;
        private double oobavge = Double.NaN;
        private double oobavgre = Double.NaN;
        private double oobrce = Double.NaN;
        public alglib.decisionforest getDfModel()
        {
            if(outMatrix==null) getMatrix();
            if(advance)
            {
                alglib.dfbuildrandomdecisionforestx1(outMatrix, n, nvars, nclasses, nTrees, nrndvars, r, out info, out df, out rep);
            }
            else
            {
                alglib.dfbuildrandomdecisionforest(outMatrix, n, nvars, nclasses, nTrees,r,out info, out df, out rep);
            }
            rmse = rep.rmserror;
            avgcee = rep.avgce;
            avge = rep.avgerror;
            avgre = rep.avgrelerror;
            rce = rep.relclserror;
            oobrmse = rep.oobrmserror;
            oobavgcee = rep.oobavgce;
            oobavge = rep.oobavgerror;
            oobavgre = rep.oobavgrelerror;
            oobrce = rep.oobrelclserror;

            return df;
        }
        public double RMSE
        {
            get
            {
                if (Double.IsNaN(rmse)) getDfModel();
                return rmse;
            }
        }
        public double AverageError
        {
            get
            {
                if (Double.IsNaN(avge)) getDfModel();
                return avge;
            }
        }
        public double AverageRelativeError
        {
            get
            {
                if (Double.IsNaN(avgre)) getDfModel();
                return avgre;
            }
        }
        public double AverageCrossEntropyError
        {
            get
            {
                if (Double.IsNaN(avgcee)) getDfModel();
                return avgcee;
            }
        }
        public double RelativeClassificationError
        {
            get
            {
                if (Double.IsNaN(rce)) getDfModel();
                return rce;
            }
        }
        public double OOBRMSE
        {
            get
            {
                if (Double.IsNaN(rmse)) getDfModel();
                return oobrmse;
            }
        }
        public double OOBAverageError
        {
            get
            {
                if (Double.IsNaN(avge)) getDfModel();
                return oobavge;
            }
        }
        public double OOBAverageRelativeError
        {
            get
            {
                if (Double.IsNaN(avgre)) getDfModel();
                return oobavgre;
            }
        }
        public double OOBAverageCrossEntropyError
        {
            get
            {
                if (Double.IsNaN(avgcee)) getDfModel();
                return oobavgcee;
            }
        }
        public double OOBRelativeClassificationError
        {
            get
            {
                if (Double.IsNaN(rce)) getDfModel();
                return oobrce;
            }
        }

        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Random Forest Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Sample size = " + n.ToString());
            rd.addMessage("Number of Classes = " + NumberOfClasses.ToString());
            rd.addMessage("Class Names and order = " + String.Join(", ",Categories));
            rd.addMessage("Number of Parameters = " + NumberOfVariables.ToString());
            rd.addMessage("Number of Trees = " + NumberOfTrees.ToString());
            rd.addMessage("Data Ratio = " + Ratio.ToString());
            rd.addMessage("Number of Split Variables = " + NumberOfSplitVariables.ToString());
            rd.addMessage("Regression model = " + Regression.ToString()+"\n\nTraining Errors:\n");
            rd.addMessage("RMSE = " + RMSE.ToString());
            rd.addMessage("Average Error = " + AverageError.ToString());
            rd.addMessage("Average Relative Error = " + AverageRelativeError.ToString());
            rd.addMessage("Average Cross Entropy Error = " + AverageCrossEntropyError.ToString());
            rd.addMessage("Relative Classification Error = " + RelativeClassificationError.ToString()+"\n\nValidation Errors:\n");
            rd.addMessage("OOBRMSE = " + OOBRMSE.ToString());
            rd.addMessage("OOBAverage Error = " + OOBAverageError.ToString());
            rd.addMessage("OOBAverage Relative Error = " + OOBAverageRelativeError.ToString());
            rd.addMessage("OOBAverage Cross Entropy Error = " + OOBAverageCrossEntropyError.ToString());
            rd.addMessage("OOBRelative Classification Error = " + OOBRelativeClassificationError.ToString());
            rd.Show();
            rd.enableClose();
            if (!Regression)
            {
                if (System.Windows.Forms.MessageBox.Show("Do you want to build probability graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (df == null) df = getDfModel();
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
            else
            {
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
                meanArray[i] = mV + ((maxValues[i]-mV) * oVl);
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
            double[] output = new double[NumberOfClasses];
            alglib.dfprocess(df, input, ref output);
            return output;
        }
    }
}
