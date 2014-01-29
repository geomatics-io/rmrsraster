using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Analysis;
using Accord.Statistics.Testing;

namespace esriUtil.Statistics
{
    public class dataPrepCompareClassifications:dataPrepBase
    {
        public dataPrepCompareClassifications()
        {
        }
        public dataPrepCompareClassifications(string tablePath,string reference, string mapped1, string mapped2)
        {
            InTablePath = tablePath;
            DependentFieldNames = new string[] { reference };
            IndependentFieldNames = new string[] { mapped1,mapped2 };
            ClassFieldNames = new string[]{reference,mapped1,mapped2};
        }
        public dataPrepCompareClassifications(ITable table, string reference, string mapped1, string mapped2)
        {
            InTable = table;    
            DependentFieldNames = new string[] { reference };
            IndependentFieldNames = new string[] { mapped1, mapped2 };
            ClassFieldNames = new string[] { reference, mapped1, mapped2 };
        }
        private TwoMatrixKappaTest akt = null;
        public TwoMatrixKappaTest TwoMatrixKappaTest
        {
            get
            {
                if (akt == null) buildModel();// akt = new TwoMatrixKappaTest(aa1.GeneralConfusionMatrix, aa2.GeneralConfusionMatrix);
                return akt;
            }
        }
        public double TwoMatrixKappaTestPvalue
        {
            get
            {
                return TwoMatrixKappaTest.PValue;
            }
        }
        private BowkerTest bkt = null;
        public BowkerTest BowkerTestofSymetry
        {
            get
            {
                if (bkt == null) buildModel();//new BowkerTest(aa3.GeneralConfusionMatrix);
                return bkt;
            }
            
        }
        public double BowkerPvalue
        {
            get
            {
                return BowkerTestofSymetry.PValue;
            }
        }
        private BhapkarTest bht = null;
        public BhapkarTest BHapkarTest
        {
            get
            {
                if (bht == null) buildModel();//new BhapkarTest(aa3.GeneralConfusionMatrix);
                return bht;
            }
        }
        public double BHapkarPvalue
        {
            get
            {
                return BHapkarTest.PValue;
            }
        }
        private McNemarTest mct = null;
        public McNemarTest McNemarMatchPairedCorrectTest
        {
            get
            {
                if (mct == null)
                {
                    buildModel();
                }
                return mct;
            }
        }
        public double McNemarPvalue
        {
            get
            {
                return McNemarMatchPairedCorrectTest.PValue;
            }
        }
        int[,] mctable = null;
        int[,] g1table = null;
        int[,] g2table = null;
        int[,] g3table = null;
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
            mctable = new int[2, 2];
            g1table = new int[labels.Count, labels.Count];
            g2table = new int[labels.Count, labels.Count];
            g3table = new int[labels.Count, labels.Count];
            ICursor cur = InTable.Search(null, false);
            int refIndex = cur.FindField(DependentFieldNames[0]);
            int m1Index = cur.FindField(IndependentFieldNames[0]);
            int m2index = cur.FindField(IndependentFieldNames[1]);
            //Console.WriteLine(wIndex.ToString());
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string rVl = rw.get_Value(refIndex).ToString();
                string m1Vl = rw.get_Value(m1Index).ToString();
                string m2Vl = rw.get_Value(m2index).ToString();
                string rVll = rVl.ToLower();
                string m1Vll = m1Vl.ToLower();
                string m2Vll = m2Vl.ToLower();
                int mClm = 0;
                int mRws = 0;
                if (rVll != m1Vll && rVll != m2Vll)
                {
                    mClm = 1;
                    mRws = 1;
                }
                else if (rVll == m1Vll && rVll != m2Vll)
                {
                    mClm = 0;
                    mRws = 1;
                }
                else if (rVll != m1Vll && rVll == m2Vll)
                {
                    mClm = 1;
                    mRws = 0;
                }
                mctable[mClm, mRws] += 1;
                g1table[labels.IndexOf(rVl),labels.IndexOf(m1Vl)]+=1;
                g2table[labels.IndexOf(rVl),labels.IndexOf(m2Vl)]+=1;
                g3table[labels.IndexOf(m1Vl), labels.IndexOf(m2Vl)] += 1;
                rw = cur.NextRow();
            }
            return null;
            
        }

        private ConfusionMatrix cofM = null;
        private GeneralConfusionMatrix gcofM1 = null;
        private GeneralConfusionMatrix gcofM2 = null;
        private GeneralConfusionMatrix gcofM3 = null;
        private void buildModel()
        {
            if(mctable==null)getMatrix();
            cofM = new ConfusionMatrix(mctable);
            gcofM1 = new GeneralConfusionMatrix(g1table);
            gcofM2 = new GeneralConfusionMatrix(g2table);
            gcofM3 = new GeneralConfusionMatrix(g3table);
            mct = new McNemarTest(cofM);
            akt = new TwoMatrixKappaTest(gcofM1, gcofM2);
            bkt = new BowkerTest(gcofM3);
            bht = new BhapkarTest(gcofM3);

        }
        
        public string writeModel(string outModelPath)
        {
            if (mctable == null) buildModel();
            string outPath = outModelPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.CompareClassifications.ToString());
                sw.WriteLine(String.Join(",",ClassFieldNames));
                sw.WriteLine(String.Join(",",labels.ToArray()));
                sw.WriteLine(String.Join(",", (from int d in mctable select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from int d in g1table select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from int d in g2table select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from int d in g3table select d.ToString()).ToArray()));
                sw.Close();
            }
            return outPath;
        }
        public void getModel(string modelPath)
        {

            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.CompareClassifications)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not an Accuracy Assessment!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                string fieldNames = sr.ReadLine();
                string[] fieldNamesSplit = fieldNames.Split(new char[] { ',' });
                IndependentFieldNames = new string[] { fieldNamesSplit[1],fieldNamesSplit[2] };
                DependentFieldNames = new string[] { fieldNamesSplit[0] };
                ClassFieldNames = fieldNamesSplit;
                labels = sr.ReadLine().Split(new char[] { ',' }).ToList();
                mctable = new int[2,2];
                g1table = new int[labels.Count, labels.Count];
                g2table = new int[labels.Count, labels.Count];
                g3table = new int[labels.Count, labels.Count];
                string ln = sr.ReadLine();
                int[] intArr = (from string s in ln.Split(new char[]{','}) select System.Convert.ToInt32(s)).ToArray();
                int cnt = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        mctable[i, j] = intArr[cnt];
                        cnt++;
                    }
                }
                ln = sr.ReadLine();
                int[] intArr1 = (from string s in ln.Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
                ln = sr.ReadLine();
                int[] intArr2 = (from string s in ln.Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
                ln = sr.ReadLine();
                int[] intArr3 = (from string s in ln.Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
                cnt = 0;
                for (int i = 0; i < labels.Count; i++)
                {
                    for (int j = 0; j < labels.Count; j++)
                    {
                        g1table[i, j] = intArr1[cnt];
                        g2table[i, j] = intArr2[cnt];
                        g3table[i, j] = intArr3[cnt];
                        cnt++;
                    }
                }
                sr.Close();
            }
            buildModel();
        }
        
        public void getReport()
        {
            if(mctable==null)buildModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Compare Classifications";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Match Paired Comparisons");
            string l00 = mctable[0, 0].ToString();
            string l01 = mctable[0, 1].ToString();
            string l10 = mctable[1, 0].ToString();
            string l11 = mctable[1, 1].ToString();
            rd.addMessage("-".PadRight(100, '-') + "\n");
            string mhd =  "          Correct   Wrong";
            rd.addMessage(mhd);
            rd.addMessage("-".PadRight(28, '-'));
            rd.addMessage("Correct | " + adjustString(l00, 7) + " | " + adjustString(l10, 7) + " |");
            rd.addMessage("-".PadRight(28, '-'));
            rd.addMessage("Wrong   | " + adjustString(l01, 7) + " | " + adjustString(l11, 7) + " |");
            rd.addMessage("-".PadRight(28, '-'));
            rd.addMessage("McNemar's test for correct values = " + mct.Statistic.ToString() + " DF = " + mct.DegreesOfFreedom.ToString() + " p-value = " + mct.PValue.ToString());
            rd.addMessage("-".PadRight(100, '-')+"\n");
            rd.addMessage("Kappa comparison of two samples = " + akt.Statistic.ToString() + " DF = 1 p-value = " + akt.PValue.ToString());
            rd.addMessage("-".PadRight(100, '-') + "\n");
            string[] hdArr = (from s in labels select adjustString(s,5)).ToArray();
            string hd = "      | " + String.Join(" | ",hdArr);
            rd.addMessage(hd);
            rd.addMessage("-".PadRight(hd.Length,'-'));
            for (int i = 0; i < hdArr.Length; i++)
            {
                string lnStr = hdArr[i] + " | ";
                for (int j = 0; j < hdArr.Length; j++)
			    {
                    lnStr = lnStr + adjustString(g3table[j,i].ToString(),5) + " | ";
			    }
                rd.addMessage(lnStr);
                rd.addMessage("-".PadRight(hd.Length, '-'));
            }
            rd.addMessage("Bhapkar's test of homogeneity = " + bht.Statistic.ToString() + " DF = " + bht.DegreesOfFreedom.ToString() + " p-value = " + bht.PValue.ToString());
            rd.addMessage("Bowker's test of symmetry = " + bkt.Statistic.ToString() + " DF = " + bkt.DegreesOfFreedom.ToString() + " p-value = " + bkt.PValue.ToString());
            rd.Show();
            rd.enableClose();      
        }
        public override double[] getArray(string varName)
        {
            return null;
        } 
        private string adjustString(string s, int length)
        {
            string outs = s;
            if (outs.Length > length) outs = outs.Substring(0, length);
            else if (outs.Length < length) outs = outs.PadRight(length, ' ');
            return outs;
        }
    }
}
