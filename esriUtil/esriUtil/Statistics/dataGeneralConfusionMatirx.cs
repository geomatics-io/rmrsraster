using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;

namespace esriUtil.Statistics
{
    public class dataGeneralConfusionMatirx:dataPrepBase
    {
        public dataGeneralConfusionMatirx()
        {
        }
        public dataGeneralConfusionMatirx(string tablePath,string dependentField, string independentField)
        {
            InTablePath = tablePath;
            DependentFieldNames = new string[] { dependentField };
            IndependentFieldNames = new string[] { independentField };
            ClassFieldNames = new string[]{dependentField,independentField};
        }
        public dataGeneralConfusionMatirx(ITable table, string dependentField, string independentField)
        {
            InTable = table;
            DependentFieldNames = new string[]{dependentField};
            IndependentFieldNames = new string[]{independentField};
            ClassFieldNames = new string[]{dependentField,independentField};
        }
        private string weightfld = "";
        public string WeightFeild { get { return weightfld; } set { weightfld = value; } }
        private GeneralConfusionMatrix gConf = null;
        public GeneralConfusionMatrix GeneralConfusionMatrix
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf;
            }
        }
        public double ChiSquare
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf.ChiSquare;
            }
        }
        int[,] xtable = null;
        public int[,] XTable { get { return gConf.Matrix; } }
        private Dictionary<string, List<string>> unVlDic = null;
        public string[] getUniqueValues(string classVariable)
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            return unVlDic[classVariable].ToArray();
        }
        List<string> labels;
        public string[] Labels { get { return labels.ToArray(); } }
        public override double[,] getMatrix()
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            List<string> aVlLst = new List<string>();
            foreach (KeyValuePair<string, List<string>> kvp in unVlDic)
            {
                List<string> vlLst = kvp.Value;
                aVlLst.AddRange(vlLst);
            }
            labels = aVlLst.Distinct().ToList();
            double[] weights = new double[labels.Count];
            int clms = labels.Count();
            int rws = clms;
            xtable = new int[clms, rws];
            ICursor cur = InTable.Search(null, false);
            int depIndex = cur.FindField(DependentFieldNames[0]);
            int indIndex = cur.FindField(IndependentFieldNames[0]);
            int wIndex = -1;
            if (weightfld != "")
            {
                wIndex = cur.FindField(weightfld);
            }
            //Console.WriteLine(wIndex.ToString());
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string dVl = rw.get_Value(depIndex).ToString();
                string iVl = rw.get_Value(indIndex).ToString();
                double w = 1;
                if (wIndex != -1)
                {
                    w = System.Convert.ToDouble(rw.get_Value(wIndex));
                }
                int mClm = labels.IndexOf(iVl);
                int mRws = labels.IndexOf(dVl);
                weights[mClm] = w;
                xtable[mClm, mRws] += 1;
                rw = cur.NextRow();
            }
            //Console.WriteLine(String.Join(", ", (from d in weights select d.ToString()).ToArray()));
            if (wIndex != -1)
            {
                updateXTable(weights);
            }
            return null;
            
        }

        private void updateXTable(double[] weights)
        {
            for (int r = 0; r < weights.Length; r++)
            {
                for (int c = 0; c < weights.Length; c++)
                {
                    double w = weights[c];
                    //Console.WriteLine(w.ToString());
                    int vl = System.Convert.ToInt32(xtable[c, r] * w);
                    //Console.WriteLine(vl);
                    xtable[c, r] = vl;
                }

            }
        
        }
        public override double[] getArray(string varName)
        {
            if (xtable == null) getMatrix();
            int clms = xtable.GetUpperBound(0) + 1;
            double[] outArr = new double[clms];
            int r = labels.IndexOf(varName);
            for (int i = 0; i < clms; i++)
            {
                outArr[i] = xtable[i, r];
            }
            return outArr;
        }
        private void buildModel()
        {
            if(xtable==null)getMatrix();
            gConf=new GeneralConfusionMatrix(xtable);
        }
        public double Kappa 
        { 
            get 
            {
                if (gConf == null) buildModel();
                return gConf.Kappa;
            } 
        }
        public double STE 
        { 
            get 
            {
                if (gConf == null) buildModel();
                return gConf.StandardError; 
            } 
        }
        public ci getConfidenceInterval(double alpha)
        {
            double a = Accord.Math.Normal.Inverse(1 - (alpha / 2))*STE;
            double u = Kappa + a;
            double l = Kappa - a;
            ci CI = new ci();
            CI.UpperBound = u;
            CI.LowerBound = l;
            return CI;
        }
        public double Overall 
        { 
            get 
            {
                if (gConf == null) buildModel();
                return gConf.OverallAgreement; 
            } 
        }
        public double Tau
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf.Tau;
            }
        }
        public double Phi
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf.Phi;
            }
        }
        public double Pearson
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf.Pearson;
            }
        }
        public double Cramer
        {
            get
            {
                if (gConf == null) buildModel();
                return gConf.Cramer;
            }
        }
        
        public string writeXTable(string outModelPath)
        {
            if (gConf == null) buildModel();
            string outPath = outModelPath;//outDirectory + "\\" + ((IDataset)InTable).BrowseName + "_xt.xtb";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.Accuracy.ToString());
                sw.WriteLine(String.Join(",",ClassFieldNames));
                sw.WriteLine(String.Join(",",labels.ToArray()));
                sw.WriteLine(Overall.ToString());
                sw.WriteLine(Kappa.ToString());
                sw.WriteLine(STE.ToString());
                int rws = xtable.GetUpperBound(1);
                int clms = xtable.GetUpperBound(0);
                for (int r = 0; r <= rws; r++)
                {
                    string[] ln = new string[clms + 1];
                    for (int c = 0; c <= clms; c++)
                    {
                        ln[c] = xtable[c, r].ToString();
                    }
                    sw.WriteLine(String.Join(" ", ln));
                }
                sw.Close();
            }
            return outPath;
        }
        public void getXTable(string modelPath)
        {

            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.Accuracy)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not an Accuracy Assessment!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                string fieldNames = sr.ReadLine();
                string[] fieldNamesSplit = fieldNames.Split(new char[] { ',' });
                IndependentFieldNames = new string[] { fieldNamesSplit[1] };
                DependentFieldNames = new string[] { fieldNamesSplit[0] };
                ClassFieldNames = fieldNamesSplit;
                labels = sr.ReadLine().Split(new char[] { ',' }).ToList();
                double overall = System.Convert.ToDouble(sr.ReadLine());
                double k = System.Convert.ToDouble(sr.ReadLine());
                double kerr = System.Convert.ToDouble(sr.ReadLine());
                xtable = new int[labels.Count, labels.Count];
                string ln = sr.ReadLine();
                int lnCnt = 0;
                while (ln != null)
                {
                    string[] lnArr = ln.Split(new char[] { ' ' });
                    for (int i = 0; i < lnArr.Length; i++)
                    {
                        xtable[i, lnCnt] = System.Convert.ToInt32(lnArr[i]);
                    }
                    lnCnt += 1;
                    ln = sr.ReadLine();
                }
                sr.Close();
            }
            buildModel();
        }
        public double CohenKappaVariance
        {
            get
            {
                if (gConf == null) buildModel();
                return Accord.Statistics.Testing.KappaTest.DeltaMethodKappaVariance(gConf);
            }
        }


        public void getReport(double alpha)
        {
            if(gConf==null)buildModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Accuracy Assessment";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            string[] hd = new string[labels.Count];
            for (int i = 0; i < hd.Length; i++)
            {
                string s = labels[i];
                if(s.Length>4)
                {
                    hd[i] = s.Substring(0, 4);
                }
                else
                {
                    hd[i] = s.PadRight(3);
                }
            }
            rd.addMessage("       "+String.Join("   ", hd));
            rd.addMessage("------".PadRight((labels.Count+1)*7,'-'));
            for (int i = 0; i < labels.Count; i++)
            {
                string[] lnArr = new string[labels.Count+1];
                string vl = labels[i];
                if (vl.Length > 4)
                {
                    vl = vl.Substring(0, 4);
                }
                else
                {
                    vl = vl.PadRight(4);
                }
                lnArr[0] = vl;
                for (int j = 0; j < labels.Count; j++)
                {
                    vl = xtable[i,j].ToString();
                    if (vl.Length < 4)
                    {
                        vl = vl.PadRight(4);
                    }
                    lnArr[j + 1] = vl;
                }
                rd.addMessage(String.Join(" | ", lnArr)+"|");
                rd.addMessage("------".PadRight((labels.Count+1) * 7, '-'));
            }
            Accord.Statistics.Testing.ChiSquareTest ct = new Accord.Statistics.Testing.ChiSquareTest(ChiSquare, System.Convert.ToInt32(Math.Pow(labels.Count - 1, 2)));
            rd.addMessage("Chi-square = " + ChiSquare + " DF = " + ct.DegreesOfFreedom.ToString() + " p-value = " + ct.PValue.ToString());
            rd.addMessage("Overall = " + Overall.ToString());
            rd.addMessage("Kappa = " + Kappa.ToString());
            rd.addMessage("STE = " + STE.ToString());
            ci conf = getConfidenceInterval(alpha);  
            rd.addMessage("Kappa CI = " + conf.LowerBound.ToString() + " - " + conf.UpperBound.ToString());
            rd.addMessage("Cohen Kappa Variance = " + CohenKappaVariance.ToString());
            rd.Show();
            rd.enableClose();
           
        }
    }
}
