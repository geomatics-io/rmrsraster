using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Statistics
{
    public class dataPrepSoftMaxPlr : dataPrepBase
    {
        public dataPrepSoftMaxPlr()
        {
        }
        public dataPrepSoftMaxPlr(string tablePath,string dependentField, string independentFields, string categoricalFields)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
        }
        public dataPrepSoftMaxPlr(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
        }
        private Dictionary<string, List<string>> unVlDic = null;
        private string[] allFieldNames = null;
        private double[,] outMatrix = null;
        private int nvars = 0;
        private int nclasses = 0;
        private int n = 0;
        private string[] categories = null;
        public string[] Categories
        {
            get
            {
                if (categories == null) getMnlModel();
                return categories;
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
                categories = unVlDic[lu].ToArray();
                nclasses += unVlDic[lu].Count;
                allFieldNames[i + indCnt] = lu;
                allFieldIndexArray[i+indCnt] = cur.FindField(lu);
			}
            nvars = clms - 1;
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
        public int SampleSize { get { return n; } }
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            if (lm == null) getMnlModel();
            outPath = outModelPath;
            double[,] coef = null;
            alglib.mnlunpack(lm, out coef, out nvars, out nclasses);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.SoftMax.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(String.Join(",",IndependentFieldNames));
                sw.WriteLine(String.Join(",",DependentFieldNames));
                sw.WriteLine(String.Join(",",ClassFieldNames));
                sw.WriteLine(string.Join(",", Categories));
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(NumberOfClasses.ToString());
                sw.WriteLine(RMSE.ToString());
                sw.WriteLine(AverageCrossEntropyError.ToString());
                sw.WriteLine(AverageError.ToString());
                sw.WriteLine(AverageRelativeError.ToString());
                sw.WriteLine(ClassificationError.ToString());
                sw.WriteLine(RelativeClassificationError.ToString());
                sw.WriteLine(String.Join(",", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in sumValues select d.ToString()).ToArray()));
                int rws = coef.GetUpperBound(1);
                int clms = coef.GetUpperBound(0);
                for (int r = 0; r <= rws; r++)
		        {
                    string[] ln = new string[clms+1];
                    for (int c = 0; c <= clms; c++)
                    {
                        ln[c] = coef[c, r].ToString();
                    }
                    sw.WriteLine(String.Join(" ", ln)); 
		        }
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
        alglib.logitmodel lm = null;
        int info;
        alglib.mnlreport rep;
        string independentfieldnames;

        public alglib.logitmodel SoftMaxNnetModel
        {
            get
            {
                return lm;
            }
        }
        public alglib.logitmodel getMnlModel(string modelPath)
        {
            double[,] coef = null;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.SoftMax)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a SoftMax Nnet Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return null;
                }
                InTablePath = sr.ReadLine();
                n = System.Convert.ToInt32(sr.ReadLine());
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                categories = sr.ReadLine().Split(new char[] { ',' });
                nvars = System.Convert.ToInt32(sr.ReadLine());
                nclasses = System.Convert.ToInt32(sr.ReadLine());
                rmse = System.Convert.ToDouble(sr.ReadLine());
                avgcee = System.Convert.ToDouble(sr.ReadLine());
                avge = System.Convert.ToDouble(sr.ReadLine());
                avgre = System.Convert.ToDouble(sr.ReadLine());
                clse = System.Convert.ToDouble(sr.ReadLine());
                rce = System.Convert.ToDouble(sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                coef = new double[nclasses - 1, nvars+1 ];
                string ln = sr.ReadLine();
                int lnCnt = 0;
                while(ln!=null)
                {
                    string[] lnArr = ln.Split(new char[] { ' ' });
                    for (int i = 0; i < lnArr.Length; i++)
                    {
                        coef[i, lnCnt] = System.Convert.ToDouble(lnArr[i]);
                    }
                    lnCnt += 1;
                    ln = sr.ReadLine();
                }
                sr.Close();
            }
            alglib.mnlpack(coef, nvars, nclasses, out lm);
            return lm;
        }
        public alglib.logitmodel getMnlModel()
        {
            if(outMatrix==null) getMatrix();
            alglib.mnltrainh(outMatrix, n, nvars, nclasses, out info, out lm, out rep);
            independentfieldnames = getAllVariableNames();
            rmse = alglib.mnlrmserror(lm, outMatrix, n);
            avgcee = alglib.mnlavgce(lm, outMatrix, n);
            avge = alglib.mnlavgerror(lm, outMatrix, n);
            avgre = alglib.mnlavgrelerror(lm, outMatrix, n);
            clse = alglib.mnlclserror(lm, outMatrix, n);
            rce = alglib.mnlrelclserror(lm, outMatrix, n);
            return lm;
        }
        public double[,] getCoefficients(alglib.logitmodel lm, out int nvars, out int nclasses)
        {
            if (lm == null) getMnlModel();
            double[,] coef;
            alglib.mnlunpack(lm, out coef, out nvars, out nclasses);
            return coef;
        }
        private double rmse = Double.NaN;
        public double RMSE
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return rmse;
            }
        }
        private double avge = Double.NaN;
        public double AverageError
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return avge;
            }
        }
        private double avgre = Double.NaN;
        public double AverageRelativeError
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return avgre;
            }
        }
        private double avgcee = Double.NaN;
        public double AverageCrossEntropyError
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return avgcee;
            }
        }
        private double clse = Double.NaN;
        public double ClassificationError
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return clse;
            }
        }
        private double rce = Double.NaN;
        public double RelativeClassificationError
        {
            get
            {
                if (Double.IsNaN(rmse)) getMnlModel();
                return rce;
            }
        }


        public void getReport()
        {
            if (lm == null) getMnlModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Soft Max Nnet Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Sample size = " + n.ToString());
            rd.addMessage("Number of Classes = " + NumberOfClasses.ToString());
            rd.addMessage("Number of Parameters = " + nvars.ToString());
            rd.addMessage("RMSE = " + RMSE.ToString());
            rd.addMessage("Average Error = " + AverageError.ToString());
            rd.addMessage("Average Relative Error = " + AverageRelativeError.ToString());
            rd.addMessage("Average Cross Entropy Error = " + AverageCrossEntropyError.ToString());
            rd.addMessage("Classification Error = " + ClassificationError.ToString());
            rd.addMessage("Relative Classification Error = " + RelativeClassificationError.ToString());
            
            try
            {
                if (ModelHelper.chartingAvailable() && System.Windows.Forms.MessageBox.Show("Do you want to build probability graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    createRegChart();
                    
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Cannot create charts");
            }
            rd.Show();
            rd.enableClose();
        }

        private void createRegChart()
        {
            if (lm == null) lm = getMnlModel();
            Forms.Stats.frmChart hist = (Forms.Stats.frmChart)ModelHelper.generateProbabilityGraphic(IndependentFieldNames);
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
                meanArray[i] = mV+((maxValues[i]-mV) * oVl);
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

        public int NumberOfClasses
        {
            get
            {
                return nclasses;
            }
        }
        public int NumberOfVariables
        {
            get
            {
                return nvars;
            }
        }
        public double[] computNew(double[] input)
        {
            double[] outArr = new double[NumberOfClasses];
            alglib.mnlprocess(lm, input, ref outArr);
            return outArr;
        }
    }
}

