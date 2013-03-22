using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using Accord.Statistics.Analysis;


namespace esriUtil.Statistics
{
    public class dataPrepVarCovCorr
    {
        public dataPrepVarCovCorr()
        {
        }
        public dataPrepVarCovCorr(IRaster raster)
        {
            inraster = raster;
            IRasterBandCollection bc = (IRasterBandCollection)inraster;
            int bcCnt = bc.Count;
            VariableFieldNames = new string[bcCnt];
            for (int i = 0; i < bcCnt; i++)
            {
                VariableFieldNames[i] = "band_" + (i + 1).ToString();
            }
            sumClms = new double[bcCnt];
            sumCross = new double[bcCnt, bcCnt];
            cov = new double[bcCnt, bcCnt];
            buildModel();
        }
        public dataPrepVarCovCorr(ITable table, string[] variables)
        {
            InTable = table;
            VariableFieldNames = variables;
            sumClms = new double[VariableFieldNames.Length];
            sumCross = new double[VariableFieldNames.Length, VariableFieldNames.Length];
            cov = new double[VariableFieldNames.Length, VariableFieldNames.Length];
            buildModel();
        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string inpath = "";
        public string InPath
        {
            get
            {
                return inpath;

            }
            
        }
        private IRaster inraster = null;
        public IRaster InRaster
        {
            get
            {
                return inraster;
            }
            set
            {
                inraster = value;
                IDataset dSet = (IDataset)((ESRI.ArcGIS.DataSourcesRaster.IRaster2)inraster).RasterDataset;
                inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                intable = null;
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
                inraster = null;
            }
        }
        public string[] VariableFieldNames { get; set; }
        private double[] sumClms = null;
        public double[] SquaredSummes
        {
            get
            {
                if (cov == null) buildModel();
                double[] ss = new double[sumClms.Length];
                for (int i = 0; i < sumClms.Length; i++)
		        {
                    ss[i] = sumCross[i, i];
		        }
                return ss;
            }
        }
        private string outmodelpath = "";
        public void writeModel(string outPath)
        {
            if (cov == null) buildModel();
            outmodelpath = outPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outmodelpath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.CovCorr.ToString());
                sw.WriteLine(InPath);
                sw.WriteLine(String.Join(",",VariableFieldNames));
                sw.WriteLine(n.ToString());
                sw.WriteLine(String.Join(",", (from double d in SumsVector select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in SummedCrossMatrix select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in CovarianceMatrix select d.ToString()).ToArray()));
                sw.Close();
            }
        }
        public void getReport()
        {
            if (cov == null) buildModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Random Forest Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Sample size = " + n.ToString());
            rd.addMessage("Vars:  " + String.Join(", ", VariableFieldNames));
            rd.addMessage("\nSum:  " + String.Join(", ", (from double d in SumsVector select d.ToString()).ToArray()));
            rd.addMessage("\nMean: " + String.Join(", ", (from double d in MeanVector select d.ToString()).ToArray()));
            rd.addMessage("\n\nSample Variance Covariance:\n");
            double[,] scov = CovarianceMatrix;
            for (int i = 0; i < SumsVector.Length; i++)
            {
                string[] vlArr = new string[SumsVector.Length];
                for (int j = 0; j < SumsVector.Length; j++)
                {
                    vlArr[j] = scov[j, i].ToString();
                }
                rd.addMessage(String.Join(", ", vlArr));
            }
            double[,] corr = CorralationMatrix;
            rd.addMessage("\n\nCorr:\n");
            for (int i = 0; i < SumsVector.Length; i++)
            {
                string[] vlArr = new string[SumsVector.Length];
                for (int j = 0; j < SumsVector.Length; j++)
                {
                    vlArr[j] = corr[j, i].ToString();
                }
                rd.addMessage(String.Join(", ", vlArr));
            }
            rd.Show();
            rd.enableClose();
        }
        private void buildModel()
        {
            if (inraster == null) buildVector();
            else buildRaster();
           
        }

        private void buildRaster()
        {
            n = 0;
            IRaster2 rs2 = (IRaster2)InRaster;
            IRasterBandCollection rsbc = (IRasterBandCollection)rs2;
            IRasterProps rsp = (IRasterProps)rs2;
            System.Array nDataVlArr = (System.Array)rsp.NoDataValue;
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            IPixelBlock pb=null;
            double[] vlBandArr = new double[rsbc.Count];
            System.Array[] pbArrs=new System.Array[rsbc.Count];
            do
            {
                pb = rsCur.PixelBlock;
                for (int i = 0; i < pb.Planes; i++)
                {
                    pbArrs[i] = (System.Array)pb.get_SafeArray(i);
                }
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        bool chVl = true;
                        for (int p = 0; p < pb.Planes; p++)
                        {
                            System.Array pbArr = pbArrs[p];
                            double vl = System.Convert.ToDouble(pbArr.GetValue(c, r));
                            if (rasterUtil.isNullData(vl, nDataVlArr.GetValue(p)))
                            {
                                chVl = false;
                                break;
                            }
                            vlBandArr[p] = vl;
                        }
                        if (chVl)
                        {
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
                
            } while (rsCur.Next() == true);
            int sampN = n;
            for (int i = 0; i < sumClms.Length; i++)
            {
                double var = (sumCross[i, i] / sampN) - (Math.Pow(sumClms[i], 2) / Math.Pow(sampN, 2));
                cov[i, i] = var;
                double sVl = sumClms[i] / sampN;
                for (int j = 0 + i + 1; j < sumClms.Length; j++)
                {
                    double vl1 = (sumCross[i, j] / sampN);
                    double vl2 = (sVl * (sumClms[j] / (sampN)));
                    double p12 = vl1 - vl2;
                    cov[i, j] = p12;
                    cov[j, i] = p12;
                }
            }

        }

        private void buildVector()
        {
            n = 0;
            int[] fldsIndex = new int[VariableFieldNames.Length];
            IQueryFilter qrf = new QueryFilterClass();
            qrf.SubFields = String.Join(", ",VariableFieldNames);
            ICursor cur = InTable.Search(qrf, false);
            IFields flds = cur.Fields;
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                //Console.WriteLine(VariableFieldNames[i]);
                fldsIndex[i] = flds.FindField(VariableFieldNames[i]);
            }
            IRow rw = cur.NextRow();
            double[] vlArr = new double[VariableFieldNames.Length];
            while (rw != null)
            {
                bool check = true;
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
                        sumCross[i, i] += Math.Pow(vlArr[i],2);
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
                rw = cur.NextRow();
            }
            int sampN = n;
            for (int i = 0; i < sumClms.Length; i++)
            {
                double var = (sumCross[i, i] / sampN) - (Math.Pow(sumClms[i], 2) / Math.Pow((sampN), 2));
                cov[i,i] = var;
                for (int j = 0 + i + 1; j < sumClms.Length; j++)
                {
                    double vl1 = (sumCross[j,i]/sampN);
                    double vl2 = (sumClms[j]/sampN) * (sumClms[i]/(sampN));
                    double p12 = vl1 - vl2;
                    cov[i, j] = p12;
                    cov[j, i] = p12;
                }
            }
        }
        public void buildModel(string modelPath)
        {
            outmodelpath = modelPath;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(outmodelpath))
            {
                dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes),sr.ReadLine());
                if (mType != dataPrepBase.modelTypes.CovCorr)
                {
                    cov=new double[1,1];
                    System.Windows.Forms.MessageBox.Show("Not a Variance Covariance Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                inpath = sr.ReadLine();
                VariableFieldNames = sr.ReadLine().Split(new char[]{','});
                n = System.Convert.ToInt32(sr.ReadLine());
                sumClms = (from string s in sr.ReadLine().Split(new char[]{','}) select System.Convert.ToDouble(s)).ToArray();
                cov = new double[sumClms.Length, sumClms.Length];
                sumCross = new double[sumClms.Length,sumClms.Length];
                string[] crossStr = sr.ReadLine().Split(new char[] { ',' });
                string[] covStr = sr.ReadLine().Split(new char[] { ',' });
                for (int i = 0; i < sumClms.Length; i++)
                {
                    for (int j = 0; j < sumClms.Length; j++)
                    {
                        int indexVl = (i * sumClms.Length) + j;
                        sumCross[i, j] = System.Convert.ToDouble(crossStr[indexVl]);
                        cov[i, j] = System.Convert.ToDouble(covStr[indexVl]);
                    }
                }
                sr.Close();
            }
            
        }
        public double[] SumsVector
        {
            get
            {
                if (cov == null) buildModel();
                return sumClms;
            }
        }
        public double[] MeanVector
        {
            get
            {
                if (cov == null) buildModel();
                return (from double d in sumClms select d/n).ToArray();
            }
        }
        public double[] VarianceVector
        {
            get
            {
                if (cov == null) buildModel();
                double[] vr = new double[sumClms.Length];
                double[,] scov = CovarianceMatrix;
                for (int i = 0; i < sumClms.Length; i++)
                {
                    vr[i] = scov[i, i];
                }
                return vr;
            }
        }
        public double[] Pop_VarianceVector
        {
            get
            {
                if (cov == null) buildModel();
                double[] vr = new double[sumClms.Length];
                for (int i = 0; i < sumClms.Length; i++)
                {
                    vr[i] = cov[i, i];
                }
                return vr;
            }
        }
        public double[] StdVector
        {
            get
            {
                if (cov == null) buildModel();
                double[] std = new double[sumClms.Length];
                double[,] scov = CovarianceMatrix;
                for (int i = 0; i < sumClms.Length; i++)
                {
                    std[i] = Math.Sqrt(scov[i, i]);
                }
                return std;
            }
        }
        public double[] Pop_StdVector
        {
            get
            {
                if (cov == null) buildModel();
                double[] std = new double[sumClms.Length];
                for (int i = 0; i < sumClms.Length; i++)
                {
                    std[i] = Math.Sqrt(cov[i, i]);
                }
                return std;
            }
        }
        private double[,] sumCross = null;
        public double[,] SummedCrossMatrix
        {
            get
            {
                if (cov == null) buildModel();
                return sumCross;
            }
        }
        private int n = 0;
        public int N { get { return n; } }
        private double[,] cov = null;
        public double[,] Pop_CovarianceMatrix
        {
            get
            {
                if (cov == null) buildModel();
                return cov;
            }
        }
        public double[,] CovarianceMatrix
        {
            get
            {
                if (cov == null) buildModel();
                double r = System.Convert.ToDouble(n)/(n-1);
                double[,] scov = new double[sumClms.Length, sumClms.Length];
                for (int i = 0; i < sumClms.Length; i++)
                {
                    for (int j = 0; j < sumClms.Length; j++)
                    {
                        scov[j, i] = (cov[j, i] * r);
                    }
                }
                return scov;
            }
        }
        public double[,] CorralationMatrix
        {
            get
            {
                if (cov == null) buildModel();
                return getCorr();
            }
        }

        private double[,] getCorr()
        {
            double[,] corr = new double[sumClms.Length, sumClms.Length];
            for (int i = 0; i < sumClms.Length; i++)
            {
                corr[i, i] = 1;
                for (int j = 0+i+1; j < sumClms.Length; j++)
                {
                    double vl = cov[j, i];
                    double var1 = cov[i, i];
                    double var2 = cov[j, j];
                    double corrVl = vl / Math.Sqrt((var1 * var2));
                    corr[i, j] = corrVl;
                    corr[j, i] = corrVl;
                }
            }
            return corr;
        }

    }
}
