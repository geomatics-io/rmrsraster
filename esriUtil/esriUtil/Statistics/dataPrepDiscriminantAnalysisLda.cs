using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;

namespace esriUtil.Statistics
{
    public class dataPrepDiscriminantAnalysisLda: dataPrepBase
    {
        public dataPrepDiscriminantAnalysisLda()
        {
        }
        public dataPrepDiscriminantAnalysisLda(string tablePath, string dependentField, string independentFields, string categoricalFields)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            buildModel();
        }

        
        public dataPrepDiscriminantAnalysisLda(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            buildModel();
        }
       
        private LinearDiscriminantAnalysis lda = null;
        public LinearDiscriminantAnalysis LdaModel
        {
            get
            {
                if (lda == null) buildModel();
                return lda;
            }

        }
        private Dictionary<string, List<string>> unVlDic = null;

        private string[] allFieldNames = null;
        private double[][] independentVls = null;
        private int[] dependentVls = null;
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
        private int numCat = 0;
        public string[] Categories
        {
            get
            {
                string[] s = UniqueClassValues[DependentFieldNames[0]].ToArray();
                numCat = s.Length;
                return s;
            }
        }
        public override double[,] getMatrix()
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            int indCnt = IndependentFieldNames.Length;
            int depCnt = DependentFieldNames.Length;

            int rws = InTable.RowCount(null);
            n = rws;
            ICursor cur = InTable.Search(null, true);
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
            dependentVls = new int[rws];
            List<string> depKey = unVlDic[depFldName];
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
                string depStr = rw.get_Value(depIndex).ToString();
                dependentVls[rwCnt] = depKey.IndexOf(depStr);
                rw = cur.NextRow();
                rwCnt += 1;
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
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

        private void buildModel()
        {
            if (independentVls == null) getMatrix();
            lda = new Accord.Statistics.Analysis.LinearDiscriminantAnalysis(independentVls, dependentVls);
            lda.Compute();
            meanValues = lda.Means;
            stdValues = lda.StandardDeviations;
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.LDA.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                sw.WriteLine(String.Join(",", ClassFieldNames));
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(String.Join(" ", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in sumValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in meanValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in stdValues select d.ToString()).ToArray()));
                sw.Close();
            }
            return outPath;
        }
        public void getDaModel(string modelPath, bool BuildModel = false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.LDA)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a DA!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                nvars = System.Convert.ToInt32(sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                meanValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                stdValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sr.Close();

            }
            if (BuildModel)
            {
                buildModel();
            }
        }

        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "LDA Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Dependent field = " + DependentFieldNames[0]);
            rd.addMessage("Independent fields = " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Sample size = " + SampleSize.ToString());
            rd.addMessage("Means:  " + string.Join(", ", (from double d in meanValues select d.ToString()).ToArray()));
            rd.addMessage("Standard Dev:   " + string.Join(", ", (from double d in stdValues select d.ToString()).ToArray()) + "\n");
            rd.Show();
            rd.enableClose();
        }


        public int computeNew(double[] input)
        {
            return lda.Classify(input);
        }

        public int computeNew(double[] input, out double[] prob)
        {
            return lda.Classify(input,out prob);
        }

        public double[] meanValues { get; set; }

        public double[] stdValues { get; set; }
    }
}

