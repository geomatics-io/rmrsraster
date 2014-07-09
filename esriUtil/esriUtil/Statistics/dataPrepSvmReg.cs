using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.VectorMachines;

namespace esriUtil.Statistics
{
    public class dataPrepSvmReg: dataPrepBase
    {
        public enum KernelType { Linear, Quadratic, Sigmoid, Spline, ChiSquared, Gaussian, Multiquadric, InverseMultquadric, Laplacian, Polynomial }
        public dataPrepSvmReg()
        {
        }
        public dataPrepSvmReg(string tablePath, string dependentField, string independentFields, string categoricalFields,KernelType kType= KernelType.Sigmoid)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            kTyp = kType;
            buildModel();
        }


        public dataPrepSvmReg(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields,KernelType kType= KernelType.Sigmoid)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            kTyp = kType;
            buildModel();
        }
        private KernelType kTyp = KernelType.Linear;
        private IKernel kernel = new Linear();
        private void setKernalType(KernelType k)
        {
            switch (k)
            {
                case KernelType.Linear:
                    kernel = new Linear();
                    break;
                case KernelType.Quadratic:
                    kernel = new Quadratic();
                    break;
                case KernelType.Sigmoid:
                    kernel = Sigmoid.Estimate(independentVls);
                    break;
                case KernelType.Spline:
                    kernel = new Spline();
                    break;
                case KernelType.ChiSquared:
                    kernel = new ChiSquare();
                    break;
                case KernelType.Gaussian:
                    kernel = Gaussian.Estimate(independentVls);
                    break;
                case KernelType.Multiquadric:
                    kernel = new Multiquadric();
                    break;
                case KernelType.InverseMultquadric:
                    kernel = new InverseMultiquadric();
                    break;
                case KernelType.Laplacian:
                    kernel = Laplacian.Estimate(independentVls);
                    break;
                default:
                    kernel = new Polynomial(2);
                    break;
            }
        }
        private KernelSupportVectorMachine svmMachine = null;
        public KernelSupportVectorMachine svmModel
        {
            get
            {
                if (svmMachine == null) buildModel();
                return svmMachine;
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
        private double sserror = 0;
        public double SumSquaredError { get { return sserror; } set { sserror = value; } }
        private void buildModel()
        {
            if (independentVls == null) getMatrix();
            setKernalType(kTyp);
            svmMachine = new Accord.MachineLearning.VectorMachines.KernelSupportVectorMachine(kernel,nvars);
            Accord.MachineLearning.VectorMachines.Learning.SequentialMinimalOptimizationRegression svmLearn = new Accord.MachineLearning.VectorMachines.Learning.SequentialMinimalOptimizationRegression(svmMachine, independentVls, dependentVls);
            sserror = svmLearn.Run();
            
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            outPath = outModelPath;
            string outPathSvm = outModelPath.Replace(".mdl", ".svm");
            svmMachine.Save(outPathSvm);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.SVM.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",", IndependentFieldNames));
                sw.WriteLine(String.Join(",", DependentFieldNames));
                if (ClassFieldNames != null)
                {
                    sw.WriteLine(String.Join(",", ClassFieldNames));
                }
                else
                {
                    sw.WriteLine();
                }
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(sserror.ToString());
                sw.WriteLine(kTyp.ToString());
                sw.WriteLine(String.Join(" ", (from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(" ", (from double d in sumValues select d.ToString()).ToArray()));
                sw.Close();
            }
            return outPath;
        }
        public void getSvmModel(string modelPath, bool BuildModel = false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.SVM)
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
                sserror = System.Convert.ToDouble(sr.ReadLine());
                kTyp = (KernelType)Enum.Parse(typeof(KernelType),sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ' ' })) select System.Convert.ToDouble(s)).ToArray();
                sr.Close();

            }
            if (BuildModel)
            {
                string svmMPath = modelPath.Replace(".mdl", ".svm");
                MulticlassSupportVectorMachine.Load(svmMPath);
            }
        }

        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "SVM Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Dependent field = " + DependentFieldNames[0]);
            rd.addMessage("Independent fields = " + String.Join(", ", IndependentFieldNames));
            rd.addMessage("Sample size = " + SampleSize.ToString());
            rd.addMessage("Sum of Squared Error:  " + sserror.ToString());
            rd.Show();
            rd.enableClose();
        }


        public double computeNew(double[] input)
        {
            return svmMachine.Compute(input);
        }
        public int computeNew(double[] input, out double response)
        {
            return svmMachine.Compute(input,out response);
        }
    }
}