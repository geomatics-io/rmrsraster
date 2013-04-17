using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using Accord.MachineLearning;

namespace esriUtil.Statistics
{
    public class dataPrepStrata
    {
        public dataPrepStrata()
        {
        }
        public dataPrepStrata(IRaster VariableRaster, IRaster strataRaster)
        {
            InValueRaster = VariableRaster;
            InStrataRaster = strataRaster;
            VariableFieldNames = new string[((IRasterBandCollection)InValueRaster).Count];
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                VariableFieldNames[i] = (i + 1).ToString();
            }
            buildModel();
        }

        public dataPrepStrata(ITable table, string[] variables, string strataField)
        {
            InTable = table;
            VariableFieldNames = variables;
            StrataField = strataField;
            buildModel();
        }

        public void buildModel()
        {
            if (invlrst == null) buildModelftr();
            else buildModelrst();
        }

        private void buildModelrst()
        {
            n = 0;
            IRaster2 rsV2 = (IRaster2)InValueRaster;
            IRaster2 rsS2 = (IRaster2)InStrataRaster;
            IRasterBandCollection rsbc = (IRasterBandCollection)rsV2;
            IRasterCursor rsSCur = rsS2.CreateCursorEx(null);
            IPixelBlock pbS = null;
            double[] vlBandArr = new double[rsbc.Count];
            Dictionary<string,int> stratDic = new Dictionary<string,int>();
            double[] sumClms = null;
            double[,] sumCross = null;
            double[,] cov = null;
            //System.Array[] pbVArrs = new System.Array[rsbc.Count];
            do
            {
                pbS = rsSCur.PixelBlock;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pbS.Width,pbS.Height);
                IPixelBlock pb = InValueRaster.CreatePixelBlock(pbSize);
                IPnt ptLoc = new PntClass();
                double mx, my;
                rsS2.PixelToMap(System.Convert.ToInt32(rsSCur.TopLeft.X), System.Convert.ToInt32(rsSCur.TopLeft.Y), out mx, out my);
                int px,py;
                rsV2.MapToPixel(mx, my, out px, out py);
                ptLoc.SetCoords(px, py);
                InValueRaster.Read(ptLoc, pb);
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object sobj = pbS.GetVal(0, c, r);
                        if (sobj == null) continue;
                        bool chVl = true;
                        for (int p = 0; p < pb.Planes; p++)
                        {
                            
                            object vlobj = pb.GetVal(p,c, r);
                            if (vlobj == null)
                            {
                                chVl = false;
                                break;
                            }
                            vlBandArr[p] = System.Convert.ToDouble(vlobj);
                        }
                        if (chVl)
                        {
                            string strataValue = sobj.ToString();
                            int sCnt;
                            if (stratDic.TryGetValue(strataValue, out sCnt))
                            {
                                stratDic[strataValue] = sCnt + 1;
                                int lIndex = lbl.IndexOf(strataValue);
                                sumClms = sumClmsLst[lIndex];
                                sumCross = sumCrossLst[lIndex];
                            }
                            else
                            {
                                stratDic.Add(strataValue, 1);
                                int lIndex = lbl.Count;
                                lbl.Add(strataValue);
                                sumClms = new double[VariableFieldNames.Length];
                                sumCross = new double[VariableFieldNames.Length, VariableFieldNames.Length];
                                cov = new double[VariableFieldNames.Length, VariableFieldNames.Length];
                                sumClmsLst.Add(sumClms);
                                sumCrossLst.Add(sumCross);
                                covLst.Add(cov);
                            }
                            for (int v = 0; v < vlBandArr.Length; v++)
                            {
                                sumClms[v] += vlBandArr[v];
                                sumCross[v, v] += Math.Pow(vlBandArr[v], 2);
                                for (int j = 0 + v + 1; j < vlBandArr.Length; j++)
                                {
                                    double vl1 = vlBandArr[v];
                                    double vl2 = vlBandArr[j];
                                    double p12 = vl1 * vl2;
                                    sumCross[v, j] += p12;
                                    sumCross[j, v] += p12;
                                }
                            }
                            n++;
                        }
                    }
                }

            } while (rsSCur.Next() == true);
            for (int l = 0; l < lbl.Count; l++)
            {
                string lblVl = lbl[l];
                int sampN = stratDic[lblVl];
                proportionsLst.Add(System.Convert.ToDouble(sampN) / n);
                sumClms = sumClmsLst[l];
                meansLst.Add((from double d in sumClms select d / sampN).ToArray());
                sumCross = sumCrossLst[l];
                cov = covLst[l];
                for (int i = 0; i < sumClms.Length; i++)
                {
                    double var = (sumCross[i, i] / sampN) - (Math.Pow(sumClms[i], 2) / Math.Pow((sampN), 2));
                    cov[i, i] = var;
                    for (int j = 0 + i + 1; j < sumClms.Length; j++)
                    {
                        double vl1 = (sumCross[j, i] / sampN);
                        double vl2 = (sumClms[j] / sampN) * (sumClms[i] / (sampN));
                        double p12 = vl1 - vl2;
                        cov[i, j] = p12;
                        cov[j, i] = p12;
                    }
                }
            }

            k = lbl.Count;
            makeKMeans();
        }
        List<double[]> sumClmsLst = new List<double[]>();
        List<double[,]> sumCrossLst = new List<double[,]>();
        List<double[,]> covLst = new List<double[,]>();
        List<double> proportionsLst = new List<double>();
        List<double[]> meansLst = new List<double[]>();
        private void buildModelftr()
        {
            Dictionary<string, int> stratDic = new Dictionary<string, int>();
            n = 0;
            int[] fldsIndex = new int[VariableFieldNames.Length];
            IQueryFilter qrf = new QueryFilterClass();
            qrf.SubFields = String.Join(",", VariableFieldNames) + "," + StrataField;
            ICursor cur = InTable.Search(qrf, false);
            IFields flds = cur.Fields;
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                //Console.WriteLine(VariableFieldNames[i]);
                fldsIndex[i] = flds.FindField(VariableFieldNames[i]);
            }
            int stratIndex = flds.FindField(StrataField);
            IRow rw = cur.NextRow();
            double[] vlArr = new double[VariableFieldNames.Length];
            double[] sumClms = null;
            double[,] sumCross = null;
            double[,] cov = null;
            while (rw != null)
            {
                bool check = true;
                object sObj = rw.get_Value(stratIndex);
                if (sObj == null)
                {
                }
                else
                {
                    string strataValue = sObj.ToString();
                    int sCnt;
                    if (stratDic.TryGetValue(strataValue, out sCnt))
                    {
                        stratDic[strataValue] = sCnt + 1;
                        int lIndex = lbl.IndexOf(strataValue);
                        sumClms = sumClmsLst[lIndex];
                        sumCross = sumCrossLst[lIndex];
                    }
                    else
                    {
                        stratDic.Add(strataValue, 1);
                        int lIndex = lbl.Count;
                        lbl.Add(strataValue);
                        sumClms = new double[VariableFieldNames.Length];
                        sumCross = new double[VariableFieldNames.Length,VariableFieldNames.Length];
                        cov = new double[VariableFieldNames.Length,VariableFieldNames.Length];
                        sumClmsLst.Add(sumClms);
                        sumCrossLst.Add(sumCross);
                        covLst.Add(cov);
                    }

                    for (int i = 0; i < fldsIndex.Length; i++)
                    {
                        int fldIndex = fldsIndex[i];
                        double vl = System.Convert.ToDouble(rw.get_Value(fldIndex));
                        //Console.WriteLine(vl.ToString());
                        if (Double.IsNaN(vl))
                        {
                            check = false;
                            break;
                        }
                        else
                        {
                            vlArr[i] = vl;
                        }
                    }
                    if (check)
                    {
                        for (int i = 0; i < vlArr.Length; i++)
                        {
                            double vl1 = vlArr[i];
                            sumClms[i] += vlArr[i];
                            sumCross[i, i] += Math.Pow(vlArr[i], 2);
                            for (int j = 0 + i + 1; j < vlArr.Length; j++)
                            {
                                double vl2 = vlArr[j];
                                double p12 = vl1 * vl2;
                                sumCross[i, j] += p12;
                                sumCross[j, i] += p12;
                            }
                        }
                        n++;
                    }
                }
                rw = cur.NextRow();
            }
            for(int l=0;l<lbl.Count;l++)
            {
                string lblVl = lbl[l];
                int sampN = stratDic[lblVl];
                proportionsLst.Add(System.Convert.ToDouble(sampN) / n);
                sumClms = sumClmsLst[l];
                meansLst.Add((from double d in sumClms select d / sampN).ToArray());
                sumCross = sumCrossLst[l];
                cov = covLst[l];
                for (int i = 0; i < sumClms.Length; i++)
                {
                    double var = (sumCross[i, i] / sampN) - (Math.Pow(sumClms[i], 2) / Math.Pow((sampN), 2));
                    cov[i, i] = var;
                    for (int j = 0 + i + 1; j < sumClms.Length; j++)
                    {
                        double vl1 = (sumCross[j, i] / sampN);
                        double vl2 = (sumClms[j] / sampN) * (sumClms[i] / (sampN));
                        double p12 = vl1 - vl2;
                        cov[i, j] = p12;
                        cov[j, i] = p12;
                    }
                }
            }

            k = lbl.Count;
            makeKMeans();
        }
        private void makeKMeans()
        {
            kmeans = new KMeans(k);
            KMeansClusterCollection kmeansColl = kmeans.Clusters;
            for (int i = 0; i < k; i++)
            {
                double[] mns = meansLst[i];
                double p = proportionsLst[i]; ;
                double[,] cov = covLst[i];
                KMeansCluster kc = new KMeansCluster(kmeansColl, i);
                kc.Mean = mns;
                kc.Covariance = cov;
                kc.Proportion = p;
            }
        }
        private double prop = 1;
        private int k = 2;
        public void buildModel(string modelPath)
        {
            outmodelpath = modelPath;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(outmodelpath))
            {
                dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), sr.ReadLine());
                if (mType != dataPrepBase.modelTypes.Cluster)
                {

                    System.Windows.Forms.MessageBox.Show("Not a Cluster Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                inpath = sr.ReadLine();
                VariableFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                prop = System.Convert.ToDouble(sr.ReadLine());
                k = System.Convert.ToInt32(sr.ReadLine());
                lbl = sr.ReadLine().Split(new char[] { ',' }).ToList();
                kmeans = new KMeans(k);
                KMeansClusterCollection kmeansColl = kmeans.Clusters;
                for (int i = 0; i < k; i++)
                {
                    double[] mns = (from s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                    string[] covVlsStr = sr.ReadLine().Split(new char[] { ',' });
                    double p = System.Convert.ToDouble(sr.ReadLine());
                    double[,] cov = new double[VariableFieldNames.Length, VariableFieldNames.Length];
                    for (int j = 0; j < VariableFieldNames.Length; j++)
                    {
                        for (int l = 0; l < VariableFieldNames.Length; l++)
                        {
                            int indexVl = (j * VariableFieldNames.Length) + l;
                            cov[l, j] = System.Convert.ToDouble(covVlsStr[indexVl]);
                        }
                    }

                    KMeansCluster kc = new KMeansCluster(kmeansColl, i);
                    kc.Mean = mns;
                    kc.Covariance = cov;
                    kc.Proportion = p;
                }
                sr.Close();
            }
        }
        private int n = 0;
        public int N { get { return n; } }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private string inpath = "";
        public string InPath
        {
            get
            {
                return inpath;

            }

        }
        private IRaster instratarst = null;
        public IRaster InStrataRaster
        {
            get
            {
                return instratarst;
            }
            set
            {
                instratarst = value;
                IDataset dSet = (IDataset)((IRaster2)instratarst).RasterDataset;
                inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            }
        }
        private IRaster invlrst = null;
        public IRaster InValueRaster
        {
            get
            {
                return invlrst;
            }
            set
            {
                invlrst = value;
            }
        }
        private ITable intable = null;
        public ITable InTable
        {
            get
            {
                return intable;
            }
            set
            {
                intable = value;
                IDataset dSet = (IDataset)intable;
                inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            }
        }
        public string[] VariableFieldNames { get; set; }
        private string outmodelpath = null;
        private string stratafld = "";
        public string StrataField { get { return stratafld; } set { stratafld = value; } }
        private KMeans kmeans = null;
        public KMeans Model { get { return kmeans; } }
        private List<string> lbl = new List<string>();
        public List<string> Labels
        {
            get
            {
                return lbl;
            }
        }
        public void writeModel(string outPath)
        {
            if (kmeans == null) buildModel();
            outmodelpath = outPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outmodelpath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.Cluster.ToString());
                sw.WriteLine(InPath);
                sw.WriteLine(String.Join(",", VariableFieldNames));
                sw.WriteLine(n.ToString());
                sw.WriteLine(prop.ToString());
                sw.WriteLine(k.ToString());
                sw.WriteLine(String.Join(",", lbl.ToArray()));
                KMeansClusterCollection gCol = kmeans.Clusters;
                for (int i = 0; i < gCol.Count; i++)
                {
                    KMeansCluster gClust = gCol[i];
                    sw.WriteLine(String.Join(",", (from double d in gClust.Mean select d.ToString()).ToArray()));
                    sw.WriteLine(String.Join(",", (from double d in gClust.Covariance select d.ToString()).ToArray()));
                    sw.WriteLine(gClust.Proportion.ToString());
                }
                sw.Close();
            }
        }
    }
}
