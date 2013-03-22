using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;

namespace esriUtil.Statistics
{
    public class dataPrepMultivariateLinearRegression:dataPrepBase
    {
        public dataPrepMultivariateLinearRegression()
        {
        }
        public dataPrepMultivariateLinearRegression(string tablePath, string dependentField, string independentFields, string categoricalFields, bool origin = false)
        {
            InTablePath = tablePath;
            DependentFieldNames = dependentField.Split(new char[] { ',' });
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            intOrigin = origin;
            getLinearRegressions();
        }
        public dataPrepMultivariateLinearRegression(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, bool origin = false)
        {
            InTable = table;
            DependentFieldNames = dependentField;
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            intOrigin = origin;
            getLinearRegressions();
        }

        private void getLinearRegressions()
        {
            mvr = new dataPrepLinearReg[DependentFieldNames.Length];
            for (int i = 0; i < DependentFieldNames.Length; i++)
            {
                mvr[i] = new dataPrepLinearReg(InTable, new string[] { DependentFieldNames[i] }, IndependentFieldNames, ClassFieldNames, intOrigin);
            }
        }
        public override double[] getArray(string varName)
        {
            throw new NotImplementedException();
        }
        public override double[,] getMatrix()
        {
            throw new NotImplementedException();
        }
        private bool intOrigin = false;
        private dataPrepLinearReg[] mvr = null;
        public dataPrepLinearReg[] multivariateRegression
        {
            get
            {
                return mvr;
            }
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            outPath = outModelPath;
            string outDir = System.IO.Path.GetDirectoryName(outPath);
            string outName = System.IO.Path.GetFileNameWithoutExtension(outPath);
            string outExt = System.IO.Path.GetExtension(outPath);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.MvlRegression.ToString());
                for (int i = 0; i < DependentFieldNames.Length; i++)
                {
                    sw.WriteLine(mvr[i].writeModel(outDir+"\\"+outName+"_"+i.ToString()+outExt));
                }
                sw.Close();
            }
            return outPath;
        }
        public void getMlrModel(string modelPath, bool BuildModel = false)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.MvlRegression)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a Multivariate Linear Regression Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                string ln = sr.ReadLine();
                List<dataPrepLinearReg> lrLst = new List<dataPrepLinearReg>();
                while (ln != null)
                {
                    dataPrepLinearReg lr = new dataPrepLinearReg();
                    lr.getLrModel(ln,BuildModel);
                    lrLst.Add(lr);
                    ln = sr.ReadLine();
                }
                
                sr.Close();
                mvr = lrLst.ToArray();

            }
        }
        public void getReport(double alpha)
        {
            for (int i = 0; i < mvr.Length; i++)
            {
                mvr[i].getReport(alpha);
            }
        }
        public double[] computeNew(double[] input)
        {
            double[] pred = new double[mvr.Length];
            for (int i = 0; i < mvr.Length; i++)
            {
                pred[i] = mvr[i].computeNew(input);
            }
            return pred;
        }
    }
}
