using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Testing;
using Accord.MachineLearning;


namespace esriUtil.Statistics
{
    public class dataPrepCompareSamples
    {
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private dataPrepPrincipleComponents pca;
        private double[][] sample1;//first dimension stores variables second dimension stores values [40][8000] array data used for each KS test
        private double[][] sample2;//first dimension stores variables second dimension stores values [40][8000] array data used for each KS test
        public Dictionary<string, double[]> sDic = new Dictionary<string, double[]>(); //statistic dictionary
        public Dictionary<string, double[]> pDic = new Dictionary<string, double[]>(); //pValue dictionary
        public Dictionary<string, int[]> cntDic = new Dictionary<string, int[]>(); //countDictionary 
        public Dictionary<string, double[][]> minMaxDic1 = new Dictionary<string, double[][]>();//min=[0 length = Variables] max=[1 length = Variables] 
        public Dictionary<string, double[][]> minMaxDic2 = new Dictionary<string, double[][]>();//min=[0 length = Variables] max=[1 length = Variables]
        public Dictionary<string, double[][]> binPropDic1 = new Dictionary<string, double[][]>();//[number of Variables][10 bins] bins hold ccd given range split into 10 bins
        public Dictionary<string, double[][]> binPropDic2 = new Dictionary<string, double[][]>();//[number of Variables][10 bins] bins hold ccd given range split into 10 bins
        public Dictionary<string, int[]> clusCountDic = new Dictionary<string, int[]>();//proportion of data within stratum for each cluster. clusters length = number of bins
        public Dictionary<string, int[]> clusSampleCountDic = new Dictionary<string, int[]>();//proportion of data within stratum for each cluster. clusters length = number of bins
        private double maxSample = 200000;
        public double MaxSampleSize
        {
            get
            {
                return maxSample;
            }
        }
        public dataPrepCompareSamples(string modelPath)
        {
            buildModel(modelPath);
        }
        public dataPrepCompareSamples(ITable sample1, ITable sample2, string[] explanitoryVariables, string strataField = "", int maxSampleSize = 200000, int numBins = 100)//, bool oridinate=true)
        {
            Sample1 = sample1;
            Sample2 = sample2;
            Variables = explanitoryVariables;
            StrataField = strataField;
            Oridinate = true;//oridinate;
            maxSample = maxSampleSize;
            //Console.WriteLine(Oridinate);
            //if (Variables.Length == 1) Oridinate = false;
            //Console.WriteLine(Oridinate);
            numberOfBins = numBins;
            buildModel();
        }
        public Dictionary<string, double[]> ClusterSampleProportions
        {
            get
            {
                Dictionary<string, double[]> outDic = new Dictionary<string, double[]>();
                foreach (string s in cntDic.Keys)
                {
                    int[] clusCnt = clusSampleCountDic[s];
                    double tClsCnt = System.Convert.ToDouble(clusCnt.Sum());
                    outDic.Add(s, new double[numberOfBins]);
                    double[] rArr = outDic[s];
                    for (int i = 0; i < numberOfBins; i++)
                    {
                        double r = clusSampleCountDic[s][i] / tClsCnt;
                        rArr[i] = r;
                    }
                }
                return outDic;
            }
        }
        public Dictionary<string,double[]> ClusterProportions
        {
            get
            {
                Dictionary<string, double[]> outDic = new Dictionary<string, double[]>();
                foreach (string s in cntDic.Keys)
                {
                    int[] clusCnt = clusCountDic[s];
                    double tClsCnt = System.Convert.ToDouble(clusCnt.Sum());
                    outDic.Add(s,new double[numberOfBins]);
                    double[] rArr = outDic[s];
                    for (int i = 0; i < numberOfBins; i++)
			        {
                        double r = clusCountDic[s][i]/tClsCnt;
                        rArr[i] = r;
			        }
                }
                return outDic;
            }

        }
        private bool getSampleRatios()
        {
            bool check = true;
            if (StrataField =="")
            {
                int numberSamples1 = Sample1.RowCount(null);
                int numberSamples2 = Sample2.RowCount(null);
                cntDic.Add("1", new int[] { numberSamples1, numberSamples2 });
            }
            else
            {
                Dictionary<string,int> s1DicCnt = getUniqueValueCounts(Sample1);
                Dictionary<string, int> s2DicCnt = getUniqueValueCounts(Sample2);
                foreach (string s in s1DicCnt.Keys)
                {
                    if (!s2DicCnt.Keys.Contains(s))
                    {
                        check = false;
                        return check;
                    }
                    else
                    {
                        int s1Cnt = s1DicCnt[s];
                        int s2Cnt = s2DicCnt[s];
                        cntDic.Add(s, new int[] { s1Cnt, s2Cnt });
                    }
                }
                
            }
            return check;
        }

        private Dictionary<string, int> getUniqueValueCounts(ITable SampleTable)
        {
            Dictionary<string, int> outDic = new Dictionary<string, int>();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = StrataField;
            ICursor cur = SampleTable.Search(qf, false);
            int sIndex = cur.FindField(StrataField);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string vlStr = rw.get_Value(sIndex).ToString();
                int cnt;
                if (outDic.TryGetValue(vlStr, out cnt))
                {
                    cnt = cnt + 1;
                    outDic[vlStr]=cnt;
                }
                else
                {
                    outDic.Add(vlStr, 1);
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            return outDic;
        }
        private dataPrepClusterKmean cluster = null;
        public dataPrepClusterKmean Cluster
        {
            get
            {
                return cluster;
            }
        }
        private void buildModel()
        {
            if (!checkTables())
            {
                //Console.WriteLine("CheckTables = false");
                return;
            }
            if (!getSampleRatios())
            {
                //Console.WriteLine("Sample Ratios = false");
                return;
            }
            pca = new dataPrepPrincipleComponents(Sample1, Variables);
            cluster = new dataPrepClusterKmean(Sample1, Variables, numberOfBins);
            foreach(string s in cntDic.Keys)
            {
                buildSamples(s);
                
                double[] pValueArr = new double[Variables.Length];
                double[] sValueArr = new double[Variables.Length];
                //double[] s1Arr = sample1[0];
                //double[] s2Arr = sample2[0];
                //TwoSampleKolmogorovSmirnovTest test = new TwoSampleKolmogorovSmirnovTest(s1Arr, s2Arr, TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);
                //getCdfProp(s, 0, test);
                //pValue = test.PValue;
                //sValue = test.Statistic;
                for (int i = 0; i < Variables.Length; i++)
                {
                    double[] s1Arr = sample1[i];
                    double[] s2Arr = sample2[i];
                    TwoSampleKolmogorovSmirnovTest test = new TwoSampleKolmogorovSmirnovTest(s1Arr, s2Arr, TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);
                    //Console.WriteLine(test.Significant.ToString());
                    getCdfProp(s, i, test);
                    double pValueS = test.PValue;
                    double sValueS = test.Statistic;
                    pValueArr[i] = pValueS;
                    sValueArr[i] = sValueS;
                }
                pDic.Add(s, pValueArr);
                sDic.Add(s, sValueArr);
            }
            
            
        }
        public void writeModel(string outModel)
        {
            if (pDic.Count==0) buildModel();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outModel))
            {
                sw.WriteLine(dataPrepBase.modelTypes.KS.ToString());
                sw.WriteLine(String.Join(",", Variables));
                sw.WriteLine(StrataField);
                sw.WriteLine(Oridinate.ToString());
                string[] lbl = Labels;
                sw.WriteLine(String.Join(",",lbl));
                string[] pArr = new string[lbl.Length];
                string[] sArr = new string[lbl.Length];
                string[] cArr1 = new string[lbl.Length];
                string[] cArr2 = new string[lbl.Length];
                double[][] minmax1, minmax2, bp1,bp2;
                for (int i = 0; i < lbl.Length; i++)
                {
                    string l = lbl[i];
                    pArr[i] = String.Join(";",(from double d in pDic[l] select d.ToString()).ToArray());
                    sArr[i] = String.Join(";", (from double d in sDic[l] select d.ToString()).ToArray());
                    int[] cArr = cntDic[l];
                    cArr1[i] = cArr[0].ToString();
                    cArr2[i] = cArr[1].ToString();
                }
                sw.WriteLine(String.Join(",", pArr));
                sw.WriteLine(String.Join(",", sArr));
                sw.WriteLine(String.Join(",", cArr1));
                sw.WriteLine(String.Join(",", cArr2));
                for(int i=0;i<lbl.Length;i++)
	            {
                    string l = lbl[i];
                    minmax1 = minMaxDic1[l];
                    minmax2 = minMaxDic2[l];
                    sw.WriteLine(String.Join(",", (from double d in minmax1[0] select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from double d in minmax1[1] select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from double d in minmax2[0] select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from double d in minmax2[1] select d.ToString()).ToArray()));
                    bp1 = binPropDic1[l];
                    bp2 = binPropDic2[l];
                    for (int j = 0; j < Variables.Length; j++)
                    {
                        sw.WriteLine(String.Join(",", (from double d in bp1[j] select d.ToString()).ToArray()));
                        sw.WriteLine(String.Join(",", (from double d in bp2[j] select d.ToString()).ToArray()));
                    }
                    sw.WriteLine(String.Join(",", (from int d in clusCountDic[l] select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from int d in clusSampleCountDic[l] select d.ToString()).ToArray()));
	            }
                string pcaPath = System.IO.Path.GetDirectoryName(outModel) + "\\"+System.IO.Path.GetFileNameWithoutExtension(outModel) + "_pca.mdl";
                pca.writeModel(pcaPath);
                sw.WriteLine(pcaPath);
                string clustPath = System.IO.Path.GetDirectoryName(outModel) + "\\" + System.IO.Path.GetFileNameWithoutExtension(outModel) + "_cluster.mdl";
                cluster.writeModel(clustPath);
                sw.WriteLine(clustPath);
                sw.Close();
            }

        }
        private void buildModel(string mdlPath)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(mdlPath))
            {
                dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), sr.ReadLine());
                if (mType != dataPrepBase.modelTypes.KS)
                {
                    System.Windows.Forms.MessageBox.Show("Not a KS Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                Variables = sr.ReadLine().Split(new char[] { ',' });
                StrataField = sr.ReadLine();
                Oridinate = System.Convert.ToBoolean(sr.ReadLine());
                string[] lbl = sr.ReadLine().Split(new char[] { ',' });
                string[] pArr = sr.ReadLine().Split(new char[] { ',' });
                string[] sArr = sr.ReadLine().Split(new char[] {','});
                string[] cArr1 = sr.ReadLine().Split(new char[] { ',' });
                string[] cArr2 = sr.ReadLine().Split(new char[] { ',' });
                for (int i = 0; i < lbl.Length; i++)
                {
                    string l = lbl[i];
                    double[] p = (from string str in pArr[i].Split(new char[]{';'}) select System.Convert.ToDouble(str)).ToArray();
                    double[] s = (from string str in sArr[i].Split(new char[] { ';' }) select System.Convert.ToDouble(str)).ToArray();
                    int c1 = System.Convert.ToInt32(cArr1[i]);
                    int c2 = System.Convert.ToInt32(cArr2[i]);
                    pDic.Add(l, p);
                    sDic.Add(l, s);
                    cntDic.Add(l, new int[]{c1,c2});
                }
                double[][] minmax1, minmax2, bp1, bp2;
                for (int i = 0; i < lbl.Length; i++)
                {
                    string l = lbl[i];
                    double[] min1 = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                    double[] max1 = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                    double[] min2 = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                    double[] max2 = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                    minmax1 = new double[2][];
                    minmax1[0] = min1;
                    minmax1[1] = max1;
                    minMaxDic1.Add(l, minmax1);
                    minmax2 = new double[2][];
                    minmax2[0] = min2;
                    minmax2[1] = max2;
                    minMaxDic2.Add(l,minmax2);
                    bp1 = new double[Variables.Length][];
                    bp2 = new double[Variables.Length][];
                    for (int j = 0; j < Variables.Length; j++)
                    {
                        bp1[j] = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                        bp2[j] = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                    }
                    binPropDic1.Add(l, bp1);
                    binPropDic2.Add(l, bp2);
                    int[] clusCnt = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
                    clusCountDic.Add(l, clusCnt);
                    int[] clusSampCnt = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToInt32(s)).ToArray();
                    clusSampleCountDic.Add(l, clusSampCnt);
                }
                pca = new dataPrepPrincipleComponents();
                string pcPath = System.IO.Path.GetDirectoryName(mdlPath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(mdlPath) + "_pca.mdl";
                pca.buildModel(pcPath);
                cluster = new dataPrepClusterKmean();
                string clusterPath = System.IO.Path.GetDirectoryName(mdlPath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(mdlPath) + "_cluster.mdl";
                cluster.buildModel(clusterPath);
                numberOfBins = cluster.Classes;
                sr.Close();
            }
        }
        private int numberOfBins = 100;
        private void getCdfProp(string s, int i, TwoSampleKolmogorovSmirnovTest test)
        {
            //Console.WriteLine("Key to look up = " + s);
            double[][] variableBinProps;
            double[][] variableBinProps2;
            if(binPropDic1.TryGetValue(s,out variableBinProps))
            {
            }
            else
            {
                variableBinProps = new double[Variables.Length][];
                for (int k = 0; k < Variables.Length; k++)
			    {
                    variableBinProps[k] = new double[numberOfBins];
			    }
                binPropDic1.Add(s,variableBinProps);
            }
            if (binPropDic2.TryGetValue(s, out variableBinProps2))
            {
            }
            else
            {
                variableBinProps2 = new double[Variables.Length][];
                for (int k = 0; k < Variables.Length; k++)
                {
                    variableBinProps2[k] = new double[numberOfBins];
                }
                binPropDic2.Add(s, variableBinProps2);
            }
            //Console.WriteLine("Upper bound of VariableBinProps" + variableBinProps.GetUpperBound(0));
            double[] binProp2 = variableBinProps2[i];
            double[] binProp = variableBinProps[i];
            double[][] minMax = minMaxDic1[s];
            double min = minMax[0][i];
            double max = minMax[1][i];
            //Console.WriteLine(min);
            //Console.WriteLine(max);
            double binD = (max - min);
            //Console.WriteLine(binD);
            double pCdfp = 0;
            double pCdfp2 = 0;
            //int cnt = 0;
            double[][] minMax2 = minMaxDic2[s];
            double min2 = minMax2[0][i];
            double max2 = minMax2[1][i];
            if (min2 < min) min = min2;
            if (max2 > max) max = max2;
            binD = (max - min);
            double dvid = System.Convert.ToDouble(numberOfBins);
            for (int j=1;j<=numberOfBins;j++)//double d = 0.1; d <= 1; d+=0.1)
			{
                double dX = min + binD * (j/dvid);
                double pCdf = test.EmpiricalDistribution1.DistributionFunction(dX);
                double pCdf2 = test.EmpiricalDistribution2.DistributionFunction(dX);
                //Console.WriteLine("Count  = " + j.ToString());
                //Console.WriteLine("Strata = " + s);
                binProp[j-1] = pCdf-pCdfp;
                binProp2[j - 1] = pCdf2 - pCdfp2;
                pCdfp = pCdf;
                pCdfp2 = pCdf2;
                //cnt+=1;
			}
        }

        private void buildSamples(string s)
        {
            Random rn = new Random();
            double mS = maxSample * 1.05; // to make sure we get max sample give random nature of Random function
            //double minS = maxSample / Variables.Length; // to make sure we get min samples given random nature of Random function
            sample1 = new double[Variables.Length][];
            sample2 = new double[Variables.Length][];
            int[] strataCnt = cntDic[s];
            int s1 = strataCnt[0]*Variables.Length;
            int s2 = strataCnt[1]*Variables.Length;
            double r1 = 1;
            double r2 = 1;
            double sr1 = 1;
            double sr2 = 1;
            if (s1 > mS)
            {
                r1 = mS / s1;
                sr1 = maxSample / s1;

            }
            if (s2 > mS)
            {
                r2 = mS / s2;
                sr2 = maxSample / s2;
            }
            int ss1 = System.Convert.ToInt32((sr1 * strataCnt[0]));// / Variables.Length);
            int ss2 = System.Convert.ToInt32((sr2 * strataCnt[1]));// / Variables.Length);
            //Console.WriteLine("ss1,ss2 = " + ss1.ToString() + "," + ss2.ToString());
           
            for (int i = 0; i < Variables.Length; i++)
            {
                double[] vlArr1 = new double[ss1];
                double[] vlArr2 = new double[ss2];
                sample1[i] = vlArr1;
                sample2[i] = vlArr2;
            }
            IQueryFilter qf = new QueryFilterClass();
            if(StrataField=="")
            {
                qf.SubFields = String.Join(",", Variables);
            }
            else
            {
                qf.SubFields = StrataField + "," + String.Join(",", Variables);
                string d= "";
                if(Sample1.Fields.get_Field(Sample1.FindField(StrataField)).Type == esriFieldType.esriFieldTypeString) d = "'";
                qf.WhereClause = StrataField + " = " + d + s + d;
            }
            ICursor cur = Sample1.Search(qf, false);
            int[] fldIndexArr = new int[Variables.Length];
            double[] minArr = new double[Variables.Length];
            double[] maxArr = new double[Variables.Length];
            minArr[0] = Double.MaxValue - 100;
            maxArr[0] = Double.MinValue + 100;
            for (int i = 0; i < fldIndexArr.Length; i++)
            {
                fldIndexArr[i] = cur.FindField(Variables[i]);
            }
            IRow rw = cur.NextRow();
            int tCnt = 0;
            double[] vArr = new double[Variables.Length];
            double[] vArr2 = new double[Variables.Length];
            clusCountDic.Add(s, new int[numberOfBins]);
            int[] clusVArr = clusCountDic[s];
            //Console.WriteLine("Start iteration 1");
            while (rw != null && tCnt < ss1)
            {
                double nRn = rn.NextDouble();
                if (nRn <= r1)
                {
                    bool checkVls = true;
                    for (int i = 0; i < fldIndexArr.Length; i++)
                    {
                        object vlObj = rw.get_Value(fldIndexArr[i]);
                        if (vlObj == null)
                        {
                            checkVls = false;
                            break;
                        }
                        vArr[i] = System.Convert.ToDouble(vlObj);
                    }
                    if (checkVls)
                    {
                        int clusCl = cluster.computNew(vArr);
                        clusVArr[clusCl] = clusVArr[clusCl] + 1;
                        if (Oridinate)
                        {
                            vArr2 = pca.computNew(vArr);
                        }
                        else
                        {
                            vArr2 = vArr;
                        }
                        for (int i = 0; i < fldIndexArr.Length; i++)
                        {
                            double vl = vArr2[i];
                            sample1[i][tCnt] = vl;
                            if (vl < minArr[i]) minArr[i] = vl;
                            if (vl > maxArr[i]) maxArr[i] = vl;
                        }
                        tCnt += 1;
                    }
                }
                rw = cur.NextRow(); 
            }
            double[][] minMax = new double[2][];
            minMax[0] = minArr;
            minMax[1] = maxArr;
            //Console.WriteLine(String.Join(",", (from double d in minArr select d.ToString()).ToArray()));
            //Console.WriteLine(String.Join(",", (from double d in maxArr select d.ToString()).ToArray()));
            minMaxDic1.Add(s, minMax);
            qf = new QueryFilterClass();
            if (StrataField =="")
            {
                qf.SubFields = String.Join(",", Variables);
            }
            else
            {
                qf.SubFields = StrataField + "," + String.Join(",", Variables);
                string d = "";
                if (Sample1.Fields.get_Field(Sample1.FindField(StrataField)).Type == esriFieldType.esriFieldTypeString) d = "'";
                qf.WhereClause = StrataField + " = " + d + s + d;
            }
            cur = Sample2.Search(qf, false);
            minArr = new double[Variables.Length];
            maxArr = new double[Variables.Length];
            minArr[0] = Double.MaxValue;
            maxArr[0] = Double.MinValue;
            for (int i = 0; i < fldIndexArr.Length; i++)
            {
                fldIndexArr[i] = cur.FindField(Variables[i]);
            }
            rw = cur.NextRow();
            tCnt = 0;
            clusSampleCountDic.Add(s, new int[numberOfBins]);
            clusVArr = clusSampleCountDic[s];
            //Console.WriteLine("Start iteration2");
            while (rw != null && tCnt < ss2)
            {
                double nRn = rn.NextDouble();
                if (nRn <= r2)
                {
                    bool checkVls = true;
                    for (int i = 0; i < fldIndexArr.Length; i++)
                    {
                        object vlObj = rw.get_Value(fldIndexArr[i]);
                        if (vlObj == null)
                        {
                            checkVls = false;
                            break;
                        }
                        vArr[i] = System.Convert.ToDouble(vlObj);
                    }
                    if (checkVls)
                    {
                        int clusCl = cluster.computNew(vArr);
                        clusVArr[clusCl] = clusVArr[clusCl] + 1;
                        if (Oridinate)
                        {
                            vArr2 = pca.computNew(vArr);
                        }
                        else
                        {
                            vArr2 = vArr;
                        }
                        for (int i = 0; i < fldIndexArr.Length; i++)
                        {
                            double vl = vArr2[i];
                            sample2[i][tCnt] = vl;
                            if (vl < minArr[i]) minArr[i] = vl;
                            if (vl > maxArr[i]) maxArr[i] = vl;
                        }
                        tCnt += 1;
                    }
                }
                rw = cur.NextRow(); 
            }
            minMax = new double[2][];
            minMax[0] = minArr;
            minMax[1] = maxArr;
            //Console.WriteLine(String.Join(",", (from double d in minArr select d.ToString()).ToArray()));
            //Console.WriteLine(String.Join(",", (from double d in maxArr select d.ToString()).ToArray()));
            minMaxDic2.Add(s, minMax);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            //Console.WriteLine("Finished iterations");
        }

        private bool checkTables()
        {
            bool check = true;
            IFields flds1 = Sample1.Fields;
            IFields flds2 = Sample2.Fields;
            foreach (string s in Variables)
            {
                if (flds1.FindField(s) < 0) return false;
                if (flds2.FindField(s) < 0) return false;
            }
            if (StrataField != "")
            {
                if (flds1.FindField(StrataField) < 0) return false;
                if (flds2.FindField(StrataField) < 0) return false;
            }
            return check;
        }
        public string[] Labels { get { return pDic.Keys.ToArray(); } }

        public double[] WSpvalues
        {
            get
            {
                double[] outArr = new double[Labels.Length];
                for (int i = 0; i < outArr.Length; i++)
                {
                    string ky = Labels[i];
                    double[] tV = pca.ProportionOfTotalVariance;
                    double[] pArr = pDic[ky];
                    double zValue = 0;
                    double weightValues = 0;
                    if (!Oridinate) weightValues = pArr.Length;
                    for (int j = 0; j < pArr.Length; j++)
                    {
                        if (Oridinate)
                        {
                            double w = tV[j];
                            zValue += Accord.Statistics.Testing.ZTest.PValueToStatistic(pArr[j], DistributionTail.OneUpper) * w;
                            weightValues += (w*w);
                        }
                        else
                        {
                            zValue += Accord.Statistics.Testing.ZTest.PValueToStatistic(pArr[j], DistributionTail.OneUpper);
                            
                        }

                    }
                    zValue = zValue / Math.Sqrt(weightValues);
                    outArr[i] = Accord.Statistics.Testing.ZTest.StatisticToPValue(zValue, DistributionTail.OneUpper);
                }
                return outArr; 
            } 
        }
        public double[] PValues 
        { 
            get 
            {
                double[] outArr = new double[Labels.Length];
                for (int i = 0; i < outArr.Length; i++)
                {
                    string ky = Labels[i];
                    double[] tV = pca.ProportionOfTotalVariance;
                    double[] pArr = pDic[ky];
                    double pValue = 0;
                    for (int j = 0; j < pArr.Length; j++)
                    {
                        if (Oridinate)
                        {
                            pValue += pArr[j] * tV[j];
                        }
                        else
                        {
                            pValue += pArr[j] / pArr.Length;
                        }

                    }
                    outArr[i] = pValue;
                }
                return outArr; 
            } 
        }

        public double[] StatisticValues 
        { 
            get 
            {
                double[] outArr = new double[Labels.Length];
                for (int i = 0; i < outArr.Length; i++)
                {
                    string ky = Labels[i];
                    double[] tV = pca.ProportionOfTotalVariance;
                    double[] sArr = sDic[ky];
                    double sValue = 0;
                    for (int j = 0; j < sArr.Length; j++)
                    {
                        if (Oridinate)
                        {
                            sValue += sArr[j] * tV[j];
                        }
                        else
                        {
                            sValue += sArr[j] / sArr.Length;
                        }

                    }
                    outArr[i] = sValue;
                }
                return outArr; 
            } 
        }

        public string[] Variables { get; set; }

        public string StrataField { get; set; }

        public ITable Sample1 { get; set; }

        public ITable Sample2 { get; set; }

        public bool Oridinate { get; set; }

        public dataPrepPrincipleComponents PCA { get { return pca; } }

        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Kolmogorov Smirnov Two sample distribution Results (p-value for unequal distributions)";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Vars:  " + String.Join(", ", Variables));
            rd.addMessage("Perform PCA " + Oridinate.ToString());
            rd.addMessage("Strata Field = " + StrataField);
            rd.addMessage("\n Label   | Statistic  | WP value   | WS p-value");
            rd.addMessage("-".PadRight(48,'-'));
            for (int i = 0; i < Labels.Length; i++)
            {
                string ky = Labels[i];
                double stat = StatisticValues[i];
                string sVl = stat.ToString();
                double vl = PValues[i];
                string fVl = vl.ToString();
                if (vl < 0.0001) fVl = "p < 0.0001";
                double wVl = WSpvalues[i];
                string wspStr = wVl.ToString();
                if (wVl < 0.0001) wspStr = "p < 0.0001";
                string l = ModelHelper.getValue(ky,8);
                string wp = ModelHelper.getValue(fVl,10);
                string s = ModelHelper.getValue(sVl, 10);
                string wsp = ModelHelper.getValue(wspStr, 10);
                rd.addMessage(l + " | " + s +" | " + wp + " | " + wsp);
            }
            rd.addMessage("-".PadRight(48, '-'));
            
            if (ModelHelper.chartingAvailable())
            {
                createKSchart();
            }
            rd.enableClose();
            rd.Show();
        }
        private Forms.Stats.frmChart hs = new Forms.Stats.frmChart();
        private System.Windows.Forms.DataVisualization.Charting.Series srPop =  new System.Windows.Forms.DataVisualization.Charting.Series("Population");
        private System.Windows.Forms.DataVisualization.Charting.Series srSamp = new System.Windows.Forms.DataVisualization.Charting.Series("Sample");
        private System.Windows.Forms.DataVisualization.Charting.Series srPop2 = new System.Windows.Forms.DataVisualization.Charting.Series("Population2");
        private System.Windows.Forms.DataVisualization.Charting.Series srSamp2 = new System.Windows.Forms.DataVisualization.Charting.Series("Sample2");
        private void createKSchart()
        {
            foreach (KeyValuePair<string, double[][]> kVp in minMaxDic1)
            {
                string k = kVp.Key;
                double[][] minMax1 = kVp.Value;
                double[][] minMax2 = minMaxDic2[k];
                double min = minMax1[0][0];
                if (minMax2[0][0] < min) min = minMax2[0][0];
                double max = minMax1[1][0];
                if (minMax2[1][0] > max) max = minMax2[1][0];
                double span = (max - min) / numberOfBins;
                double[] binProp1 = binPropDic1[k][0];
                double[] binProp2 = binPropDic2[k][0];
                double[] xAxes = new double[binProp1.Length];
                double halfMin = min / 2;
                for (int i = 0; i < xAxes.Length; i++)
                {
                    xAxes[i] = min + halfMin + (span * i);
                }
                try
                {
                    System.Windows.Forms.ComboBox cmbPrimary = new System.Windows.Forms.ComboBox();
                    System.Windows.Forms.Label cmbPrimaryLbl = new System.Windows.Forms.Label();
                    cmbPrimary.Name = "cmbPrimary";
                    System.Drawing.Point cmbPt = new System.Drawing.Point(225, 236);
                    System.Drawing.Point lblPt = new System.Drawing.Point(150, 236);
                    cmbPrimaryLbl.Location = lblPt;
                    cmbPrimaryLbl.Size = new System.Drawing.Size(75, 23);
                    cmbPrimary.Size = new System.Drawing.Size(50, 23);
                    string[] comps = (from int i in System.Linq.Enumerable.Range(1, Variables.Length) select i.ToString()).ToArray();
                    cmbPrimary.Items.Add("Bins");
                    cmbPrimary.Items.AddRange(comps);
                    cmbPrimary.Location = cmbPt;
                    cmbPrimaryLbl.Text = "Component";
                    hs.Controls.Add(cmbPrimaryLbl);
                    hs.Controls.Add(cmbPrimary);
                    hs.Text = "Empirical Distribution and Histogram";
                    System.Windows.Forms.DataVisualization.Charting.ChartArea chAreaPop = hs.chrHistogram.ChartAreas.Add(k);
                    System.Windows.Forms.DataVisualization.Charting.ChartArea chAreaCDF = hs.chrHistogram.ChartAreas.Add("Empirical");
                    chAreaCDF.AlignWithChartArea = k;
                    System.Windows.Forms.DataVisualization.Charting.Title chTitle = hs.chrHistogram.Titles.Add("T");
                    chAreaPop.AxisX.Title = "Principle Component 1";
                    chAreaPop.AxisY.Title = "Proportion";
                    chAreaCDF.AxisY.Title = "Proportion";
                    chAreaCDF.AxisX.Title = "Principle Component 1";
                    chAreaCDF.AxisY.Maximum = 1;
                    chTitle.Alignment = System.Drawing.ContentAlignment.TopCenter;
                    chTitle.Text = "Distribution for " + k;
                    hs.chrHistogram.Series.Add(srPop);
                    hs.chrHistogram.Series.Add(srSamp);
                    srPop.Color = System.Drawing.Color.Blue;
                    srSamp.Color = System.Drawing.Color.Orange;
                    srPop.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    srSamp.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    srPop.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                    srSamp.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                    System.Windows.Forms.DataVisualization.Charting.Legend popLeg = hs.chrHistogram.Legends.Add("LP");
                    System.Windows.Forms.DataVisualization.Charting.Legend sampLeg = hs.chrHistogram.Legends.Add("LS");
                    popLeg.DockedToChartArea = k;
                    sampLeg.DockedToChartArea = k;
                    popLeg.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
                    sampLeg.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
                    popLeg.IsDockedInsideChartArea = false;
                    sampLeg.IsDockedInsideChartArea = false;
                    cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged);
                    //EDF graphic
                    hs.chrHistogram.Series.Add(srPop2);
                    hs.chrHistogram.Series.Add(srSamp2);
                    srPop2.Color = System.Drawing.Color.Blue;
                    srSamp2.Color = System.Drawing.Color.Orange;
                    srPop2.BorderWidth = 2;
                    srSamp2.BorderWidth = 2;
                    srPop2.LegendText = "Population";
                    srSamp2.LegendText = "Sample";
                    srPop2.ChartArea = "Empirical";
                    srSamp2.ChartArea = "Empirical";
                    srPop2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    srSamp2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    srPop2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                    srSamp2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
                    hs.chrHistogram.Show();
                    hs.Show();
                    cmbPrimary.SelectedItem = "Bins";
                }
                catch(Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot create charts");
                    System.Windows.Forms.MessageBox.Show("Error\n" + e.ToString());
                }
            }
        }

        void cmbPrimary_SelectedValueChanged(object sender, EventArgs e)
        {
            updateFormValues();
        }

        private void updateFormValues()
        {
            srPop.Points.Clear();
            srSamp.Points.Clear();
            srPop2.Points.Clear();
            srSamp2.Points.Clear();
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hs.Controls["cmbPrimary"];
            string cmbTxt = cmbPrimary.Text;
            System.Windows.Forms.DataVisualization.Charting.Chart ch = hs.chrHistogram;
            string k = ch.Titles[0].Text.Replace("Distribution for ", "");
            if (cmbTxt == "Bins")
            {
                ch.ChartAreas[0].AxisX.Title = "Bins";
                double[] binProp1 = ClusterProportions[k];
                double[] binProp2 = ClusterSampleProportions[k];
                int[] clusCntSamp = clusSampleCountDic[k];
                Accord.Statistics.Testing.MultinomialTest mt = new MultinomialTest(clusCntSamp, binProp1);
                ch.ChartAreas[1].AxisX.Title = "Chi-Square = " + mt.Statistic.ToString() + "\np-value = " + mt.PValue.ToString();
                double[] xAxes = (from int i in System.Linq.Enumerable.Range(0, binProp1.Length) select System.Convert.ToDouble(i)).ToArray();
                double ypSum = 0;
                double ysSum = 0;
                for (int i = 0; i < binProp1.Length; i++)
                {
                    double bp1 = binProp1[i];
                    double bp2 = binProp2[i];
                    ypSum += bp1;
                    ysSum += bp2;
                    srPop.Points.AddXY(xAxes[i], bp1);
                    srSamp.Points.AddXY(xAxes[i], bp2);
                    srPop2.Points.AddXY(xAxes[i], ypSum);
                    srSamp2.Points.AddXY(xAxes[i], ysSum);
                }
            }
            else
            {
                int cmbInd = System.Convert.ToInt32(cmbTxt) - 1;               
                ch.ChartAreas[0].AxisX.Title = "Principle Component " + cmbTxt;
                ch.ChartAreas[1].AxisX.Title = "proportion of variance = " + pca.ProportionOfTotalVariance[cmbInd].ToString() + "\np-value = " + pDic[k][cmbInd].ToString();
                double[][] minMax1 = minMaxDic1[k];
                double[][] minMax2 = minMaxDic2[k];
                double min = minMax1[0][cmbInd];
                if (minMax2[0][cmbInd] < min) min = minMax2[0][cmbInd];
                double max = minMax1[1][cmbInd];
                if (minMax2[1][cmbInd] > max) max = minMax2[1][cmbInd];
                double span = (max - min) / numberOfBins;
                double[] binProp1 = binPropDic1[k][cmbInd];
                double[] binProp2 = binPropDic2[k][cmbInd];
                double[] xAxes = new double[binProp1.Length];
                double halfMin = min / 2;
                for (int i = 0; i < xAxes.Length; i++)
                {
                    xAxes[i] = min + halfMin + (span * i);
                }

                double ypSum = 0;
                double ysSum = 0;
                for (int i = 0; i < binProp1.Length; i++)
                {
                    double bp1 = binProp1[i];
                    double bp2 = binProp2[i];
                    ypSum += bp1;
                    ysSum += bp2;
                    srPop.Points.AddXY(xAxes[i], bp1);
                    srSamp.Points.AddXY(xAxes[i], bp2);
                    srPop2.Points.AddXY(xAxes[i], ypSum);
                    srSamp2.Points.AddXY(xAxes[i], ysSum);
                }
            }
            ch.Show();

            
        }
        
    }
}
