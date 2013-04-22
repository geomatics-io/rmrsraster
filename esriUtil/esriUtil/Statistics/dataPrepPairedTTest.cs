using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using esriUtil.Statistics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using Accord.MachineLearning;

namespace esriUtil.Statistics
{
    public class dataPrepPairedTTest:dataPrepTTest
    {
        public dataPrepPairedTTest()
        {
        }
        public dataPrepPairedTTest(IRaster StrataRaster, IRaster VariableRaster)
        {
            InValueRaster = VariableRaster;
            InStrataRaster = StrataRaster;
            VariableFieldNames = new string[((IRasterBandCollection)InValueRaster).Count];
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                VariableFieldNames[i] = (i + 1).ToString();
            }
            IDataset dSet = (IDataset)((IRaster2)StrataRaster).RasterDataset;
            inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            buildModel();
        }

        public dataPrepPairedTTest(ITable table, string[] variables, string strataField)
        {
            InTable = table;
            VariableFieldNames = variables;
            StrataField = strataField;
            IDataset dSet = (IDataset)table;
            inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            buildModel();
        }
        private int n = -1;
        new public int N { get { return n; } }
        new public List<string> Labels {get{return lbl;}}
        private string inpath = "";
        new public string InPath { get { return inpath; } }
        new public void buildModel()
        {
            if (InValueRaster == null)
            {
                buildModelftr();
            }
            else
            {
                buildModelRst();
            }
            
            
        }
        private List<string> lbl = new List<string>();
        private List<double[]> sumX = new List<double[]>();
        private List<double[]> sumX2 = new List<double[]>();
        private Dictionary<string, int> cateDic = new Dictionary<string, int>();
        private void buildModelRst()
        {
            sumX.Clear();
            sumX2.Clear();
            cateDic.Clear();
            lbl.Clear();
            int v1 = VariableFieldNames.Length;
            double[] s = new double[(v1 * v1 - v1) / 2];
            double[] s2 = new double[s.Length];
            n=0;
            IRasterCursor rsCur = ((IRaster2)InStrataRaster).CreateCursorEx(null);
            IPnt vCurSize = new PntClass();
            do
            {
                IPixelBlock pbS = rsCur.PixelBlock;
                vCurSize.SetCoords(pbS.Width, pbS.Height);
                IPixelBlock pbV = InValueRaster.CreatePixelBlock(vCurSize);
                InValueRaster.Read(rsCur.TopLeft, pbV);
                for (int r = 0; r < pbS.Height; r++)
                {
                    for (int c = 0; c < pbS.Width; c++)
                    {
                        object strata = pbS.GetVal(0, c, r);
                        if (strata == null)
                        {
                            continue;
                        }
                        else
                        { 
                            string strataStr = strata.ToString();
                            int strataIndex;
                            int cnt;
                            if (cateDic.TryGetValue(strataStr, out cnt))
                            {
                                cnt=cnt+1;
                                strataIndex = lbl.IndexOf(strataStr);
                                s = sumX[strataIndex];
                                s2 = sumX2[strataIndex];
                            }
                            else
                            {
                                cnt=1;
                                cateDic.Add(strataStr, 0);
                                lbl.Add(strataStr);
                                strataIndex = lbl.Count - 1;
                                s = new double[s.Length];
                                s2 = new double[s.Length];
                                sumX.Add(s);
                                sumX2.Add(s2);
                            }
                            bool checkVl = true;
                            double[] vlArr = new double[pbV.Planes];
                            for (int p = 0; p < pbV.Planes; p++)
                            {
                                object vl = pbV.GetVal(p,c,r);
                                if (vl == null)
                                {
                                    checkVl = false;
                                    break;
                                }
                                else
                                {
                                    vlArr[p] = System.Convert.ToDouble(vl);
                                }
                            }
                            if (checkVl)
                            {
                                int vlCnter = 1;
                                int sCnter = 0;
                                for (int i = 0; i < vlArr.Length-1; i++)
                                {
                                    for (int k = vlCnter; k < vlArr.Length; k++)
                                    {
                                        double m = vlArr[i] - vlArr[k];
                                        double m2 = m * m;
                                        s[sCnter] = s[sCnter] + m;
                                        s2[sCnter] = s2[sCnter]+ m2;
                                        sCnter += 1;
                                    }
                                    vlCnter += 1;
                                }
                                cateDic[strataStr] = cnt;
                                n+=1;
                            }
                        }
                    }
                }
            } while (rsCur.Next() == true);
        }

        private void buildModelftr()
        {
            sumX.Clear();
            sumX2.Clear();
            cateDic.Clear();
            lbl.Clear();
            int v1 = VariableFieldNames.Length;
            double[] s = new double[(v1 * v1 - v1) / 2];
            double[] s2 = new double[s.Length];
            n = 0;
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = StrataField + "," + String.Join(",", VariableFieldNames);
            ICursor cur = InTable.Search(qf, false);
            int[] varIndex = new int[VariableFieldNames.Length];
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                string var = VariableFieldNames[i];
                varIndex[i] = cur.FindField(var);
            }
            int stIndex = cur.FindField(StrataField);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                object strata = rw.get_Value(stIndex);
                if (strata == null)
                {
                    continue;
                }
                else
                {
                    string strataStr = strata.ToString();
                    int strataIndex;
                    int cnt;
                    if (cateDic.TryGetValue(strataStr, out cnt))
                    {
                        cnt=cnt+1;
                        strataIndex = lbl.IndexOf(strataStr);
                        s = sumX[strataIndex];
                        s2 = sumX2[strataIndex];
                    }
                    else
                    {
                        cnt=1;
                        cateDic.Add(strataStr, 0);
                        lbl.Add(strataStr);
                        strataIndex = lbl.Count - 1;
                        s = new double[s.Length];
                        s2 = new double[s.Length];
                        sumX.Add(s);
                        sumX2.Add(s2);
                    }
                    bool checkVl = true;
                    double[] vlArr = new double[VariableFieldNames.Length];
                    for (int p = 0; p < VariableFieldNames.Length; p++)
                    {
                        int fldIndex = varIndex[p];
                        object vl = rw.get_Value(fldIndex);
                        if (vl == null)
                        {
                            checkVl = false;
                            break;
                        }
                        else
                        {
                            vlArr[p] = System.Convert.ToDouble(vl);
                        }
                    }
                    if (checkVl)
                    {
                        int vlCnter = 1;
                        int sCnter = 0;
                        for (int i = 0; i < vlArr.Length - 1; i++)
                        {
                            for (int k = vlCnter; k < vlArr.Length; k++)
                            {
                                double m = vlArr[i] - vlArr[k];
                                double m2 = m * m;
                                s[sCnter] = s[sCnter] + m;
                                s2[sCnter] = s2[sCnter] + m2;
                                sCnter += 1;
                            }
                            vlCnter += 1;
                        }
                        cateDic[strataStr] = cnt;
                        n += 1;
                    }
                    
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);

        }
        new public void buildModel(string modelPath)
        {
            outmodelpath = modelPath;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(outmodelpath))
            {
                dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), sr.ReadLine());
                if (mType != dataPrepBase.modelTypes.PAIREDTTEST)
                {

                    System.Windows.Forms.MessageBox.Show("Not a PairedTTest Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                inpath = sr.ReadLine();
                StrataField = sr.ReadLine();
                VariableFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                lbl = sr.ReadLine().Split(new char[] { ',' }).ToList();
                int[] sCnt = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToInt32(s)).ToArray();
                cateDic.Clear();
                for (int i = 0; i < lbl.Count; i++)
                {
                    cateDic.Add(lbl[i], sCnt[i]);
                }
                sumX.Clear();
                sumX2.Clear();
                for (int i = 0; i < lbl.Count; i++)
                {
                    sumX.Add((from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray());
                    sumX2.Add((from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray());
                }
                sr.Close();
            }
        }
        new public double[] computeNew(object category)
        {
            
            int lblIndex = Labels.IndexOf(category.ToString());
            int gN = cateDic[category.ToString()];
            Accord.Statistics.Distributions.Univariate.TDistribution tDist = new Accord.Statistics.Distributions.Univariate.TDistribution(gN - 1);
            double[] s = sumX[lblIndex];
            double[] s2 = sumX2[lblIndex];
            double[] mns = (from double d in s select d / gN).ToArray();
            double[] ses = getSe(s, s2, gN);
            double[] outArr = new double[mns.Length];
            for (int i = 0; i < outArr.Length; i++)
            {
                double tStat = mns[i] / ses[i];
                double cdf = tDist.DistributionFunction(tStat);
                double pValue = 0;
                if (tStat > 0)
                {
                    pValue = (1 - cdf) * 2;
                }
                else
                {
                    pValue = (cdf * 2);
                }
                outArr[i] = pValue;
            }
            return outArr;

        }
        private Forms.RunningProcess.frmRunningProcessDialog rd=null;
        new public Forms.RunningProcess.frmRunningProcessDialog ReportForm { get { return rd; } }
        new public void getReport()
        {
            if (cateDic.Count<1) buildModel();
            rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Paired T-Test Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Input path = " + InPath);
            rd.addMessage("Total Sample size = " + n.ToString());
            rd.addMessage("\nLabel   |Compare V1-V2    |N       |Dif     |T-Stat  |P-Value ");
            rd.addMessage("-".PadRight(75, '-'));
            for (int i = 0; i < lbl.Count; i++)
            {
                string labelVl = Labels[i];
                string l = getValue(labelVl, 8);
                int gN = cateDic[labelVl];
                double[] s = sumX[i];
                double[] s2 = sumX2[i];
                double[] mns = (from double d in s select d/gN).ToArray();
                //Console.WriteLine("Means = " + mns.Length.ToString());
                double[] ses = getSe(s, s2, gN);
                //Console.WriteLine("Se = " + ses.Length.ToString());
                double nSample = gN;
                int cnt = 1;
                int sCnt = 0;
                for (int j = 0; j < VariableFieldNames.Length-1; j++)
                {
                    for (int k = cnt; k < VariableFieldNames.Length; k++)
                    {
                        string fN1 = getValue(VariableFieldNames[j], 8);
                        string fN2 = getValue(VariableFieldNames[k], 8);
                        double mD = mns[sCnt];
                        double se = ses[sCnt];
                        double tStat = mD / se;
                        //Console.WriteLine(tStat.ToString());
                        Accord.Statistics.Distributions.Univariate.TDistribution tDist = new Accord.Statistics.Distributions.Univariate.TDistribution(nSample - 1);
                        double cdf = tDist.DistributionFunction(tStat);
                        double pValue = 0;
                        if (tStat > 0)
                        {
                            pValue = (1 - cdf) * 2;
                        }
                        else
                        {
                            pValue = (cdf * 2);
                        }
                        string ln = l + "|" + fN1 + "-" + fN2 + "| " + getValue(nSample.ToString(), 6) + " | " + getValue(mD.ToString(), 6) + " | " + getValue(tStat.ToString(), 6) + " | " + pValue.ToString();
                        rd.addMessage(ln);
                        sCnt += 1;
                    }
                    cnt += 1;
                }
            }
            rd.addMessage("-".PadRight(75, '-'));
            rd.enableClose();
            rd.Show();
        }

        private double[] getSe(double[] s, double[] s2, int gN)
        {
            double sqN = Math.Sqrt(gN);
            double[] seOut = new double[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                double pvar = (s2[i] - ((Math.Pow(s[i], 2)) / gN)) / gN;
                double svar = pvar * gN / (gN - 1);
                seOut[i] = Math.Sqrt(svar) / sqN;
            }
            return seOut;
        }
        private string outmodelpath = "";
        new public void writeModel(string outPath)
        {
            if (sumX.Count==0) buildModel();
            outmodelpath = outPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outmodelpath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.PAIREDTTEST.ToString());
                sw.WriteLine(InPath);
                sw.WriteLine(StrataField);
                sw.WriteLine(String.Join(",", VariableFieldNames));
                sw.WriteLine(n.ToString());
                sw.WriteLine(String.Join(",", lbl.ToArray()));
                sw.WriteLine(String.Join(",", (from int cnt in cateDic.Values select cnt.ToString()).ToArray()));
                for (int i = 0; i < lbl.Count; i++)
                {
                    sw.WriteLine(String.Join(",", (from double d in sumX[i] select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from double d in sumX2[i] select d.ToString()).ToArray()));
                }
                sw.Close();
            }
        }

    }
}