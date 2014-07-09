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
    public enum clusterType { KMEANS, BINARY, GAUSSIANMIXTURE }
    public abstract class dataPrepClusterBase
    {
        public clusterType cType = clusterType.KMEANS;
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
                try
                {
                    inpath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                }
                catch
                {
                    inpath = "unknown";
                }
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
        private string outmodelpath = null;
        private object model = null;
        private object clusterCollection = null;
        public object Model { get { return model; } }
        double[][] inputMatrix = null;
        public double[][] InputMatrix
        {
            get
            {
                return inputMatrix;
            }
            set
            {
                inputMatrix = value;
            }

        }
        private int k = 2;
        public int K { set { k = value; } }
        public int Classes { get { return k; } }
        private List<string> lbl = null;
        public List<string> Labels
        {
            get
            {
                return lbl;
            }
        }
        private double precision = 0.0001;
        public double Precision { get { return precision; } set { precision = value; } }
        public void buildModel()
        {
            if (inputMatrix == null) getMatrix();
            switch (cType)
            {
                case clusterType.KMEANS:
                    KMeans kmeans = new KMeans(k);
                    kmeans.Compute(inputMatrix, precision);
                    clusterCollection = kmeans.Clusters;
                    model = kmeans;
                    break;
                case clusterType.BINARY:
                    BinarySplit bSplit = new BinarySplit(k);
                    bSplit.Compute(inputMatrix, precision);
                    clusterCollection = bSplit.Clusters;
                    model = bSplit;
                    //Console.WriteLine("BinarySplit");
                    break;
                case clusterType.GAUSSIANMIXTURE:
                    GaussianMixtureModel gModel = new GaussianMixtureModel(k);
                    gModel.Compute(inputMatrix, precision);
                    clusterCollection = gModel.Gaussians;
                    model = gModel;
                    break;
                default:
                    break;
            }

            lbl = new List<string>();
            for (int i = 0; i < k; i++)
            {
                lbl.Add(i.ToString());
            }
        }

        private int[][] convertToIntMatrix(double[][] inputMatrix)
        {
            int clm = inputMatrix.Length;
            int[][] oMt = new int[clm][];
            for (int i = 0; i < clm; i++)
            {
                int rws = inputMatrix[0].Length;
                int[] oMt2 = new int[rws];
                for (int j = 0; j < rws; j++)
                {
                    oMt2[j] = System.Convert.ToInt32(inputMatrix[i][j]);
                }
                oMt[i] = oMt2;
            }
            return oMt;
        }
        private double prop = 1;
        private void getMatrix()
        {
            getProportionOfSamples();
            if (inraster == null)
            {
                getTableMatrix();
            }
            else
            {
                getRasterMatrix();
            }
        }

        private void getRasterMatrix()
        {
            n = 0;
            List<double[]> inputMatrixLst = new List<double[]>();
            IRaster2 rs2 = (IRaster2)InRaster;
            IRasterBandCollection rsbc = (IRasterBandCollection)rs2;
            IRasterProps rsp = (IRasterProps)rs2;
            //System.Array nDataVlArr = (System.Array)rsp.NoDataValue;
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            IPixelBlock pb = null;
            Random rand = new Random();
            do
            {
                pb = rsCur.PixelBlock;
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vlObj = pb.GetVal(0, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            if (rand.NextDouble() <= prop)
                            {
                                double[] vlBandArr = new double[rsbc.Count];
                                double vl = System.Convert.ToDouble(vlObj);
                                vlBandArr[0] = vl;
                                for (int p = 1; p < pb.Planes; p++)
                                {
                                    vlObj = pb.GetVal(p,c,r);
                                    vl = System.Convert.ToDouble(vlObj);
                                    
                                    vlBandArr[p] = vl;
                                }
                                inputMatrixLst.Add(vlBandArr);
                                n++;
                            }
                        }
                    }
                }
            
            } while (rsCur.Next() == true);
            inputMatrix = inputMatrixLst.ToArray();
        }

        private void getTableMatrix()
        {
            n = 0;
            List<double[]> inputMatrixLst = new List<double[]>();
            int[] fldsIndex = new int[VariableFieldNames.Length];
            IQueryFilter qrf = new QueryFilterClass();
            qrf.SubFields = String.Join(", ", VariableFieldNames);
            ICursor cur = InTable.Search(qrf, false);
            IFields flds = cur.Fields;
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                //Console.WriteLine(VariableFieldNames[i]);
                fldsIndex[i] = flds.FindField(VariableFieldNames[i]);
            }
            IRow rw = cur.NextRow();
            System.Random r = new Random();

            while (rw != null)
            {
                if (r.NextDouble() <= prop)
                {
                    double[] vlArr = new double[VariableFieldNames.Length];
                    for (int i = 0; i < fldsIndex.Length; i++)
                    {
                        int fldIndex = fldsIndex[i];
                        double vl = System.Convert.ToDouble(rw.get_Value(fldIndex));
                        //Console.WriteLine(vl.ToString());
                        if (Double.IsNaN(vl))
                        {
                            vl = 0;
                        }
                        vlArr[i] = vl;
                    }
                    inputMatrixLst.Add(vlArr);
                    n++;
                }

                rw = cur.NextRow();
            }
            inputMatrix = inputMatrixLst.ToArray();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }
        private double mr = 10000000d;
        public double MaxRecords { get { return mr; } set { mr = value; } }
        private void getProportionOfSamples()
        {
            double maxRecords = MaxRecords;
            int rec = 0;
            if (inraster == null)
            {
                rec = intable.RowCount(null) * VariableFieldNames.Length;
            }
            else
            {
                IRasterProps rsp = (IRasterProps)inraster;
                rec = rsp.Height * rsp.Width * ((IRasterBandCollection)inraster).Count;
            }
            if (rec > maxRecords)
            {
                prop = maxRecords / rec;
            }
            else
            {
                prop = 1;
            }
        }
        public void getReport()
        {
            if (model == null) buildModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Cluster Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Input path = " + InPath);
            rd.addMessage("Sample size = " + n.ToString() + " proportion of total records = " + prop.ToString());
            rd.addMessage("Number of Cluster = " + k.ToString());
            rd.addMessage("Labels = " + String.Join(", ", lbl.ToArray()));
            rd.addMessage("Variables: " + String.Join(" ,", VariableFieldNames));
            rd.addMessage("Cluster Type: " + cType.ToString());
            switch (cType)
            {
                case clusterType.KMEANS:
                    kmeansReport(rd);
                    break;
                case clusterType.BINARY:
                    binaryReport(rd);
                    break;
                case clusterType.GAUSSIANMIXTURE:
                    gaussianReport(rd);
                    break;
                default:
                    break;
            }

            rd.enableClose();
            rd.Show();
        }

        private void gaussianReport(Forms.RunningProcess.frmRunningProcessDialog rd)
        {
            GaussianClusterCollection gCol = (GaussianClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                GaussianCluster gClust = gCol[i];
                double[] mns = gClust.Mean;
                double[,] cov = gClust.Covariance;
                rd.addMessage("\n\nCluster " + Labels[i] + ":\nMeans: " + String.Join(", ", (from double d in mns select d.ToString()).ToArray()) + "\nCovariance:");
                for (int j = 0; j < VariableFieldNames.Length; j++)
                {
                    string[] covStrArr = new string[VariableFieldNames.Length];
                    for (int l = 0; l < covStrArr.Length; l++)
                    {
                        covStrArr[l] = cov[l, j].ToString();
                    }
                    rd.addMessage("\n" + String.Join(",", covStrArr));
                }

            }
        }

        private void binaryReport(Forms.RunningProcess.frmRunningProcessDialog rd)
        {
            KMeansClusterCollection gCol = (KMeansClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                KMeansCluster gClust = gCol[i];
                double[] mns = gClust.Mean;
                rd.addMessage("\n\nCluster " + Labels[i] + ":\nMeans: " + String.Join(", ", (from double d in mns select d.ToString()).ToArray()));
            }
        }

        private void kmeansReport(Forms.RunningProcess.frmRunningProcessDialog rd)
        {
            KMeansClusterCollection gCol = (KMeansClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                KMeansCluster gClust = gCol[i];
                double[] mns = gClust.Mean;
                double[,] cov = gClust.Covariance;
                rd.addMessage("\n\nCluster " + Labels[i] + ":\nMeans: " + String.Join(", ", (from double d in mns select d.ToString()).ToArray()) + "\nCovariance:");
                for (int j = 0; j < VariableFieldNames.Length; j++)
                {
                    string[] covStrArr = new string[VariableFieldNames.Length];
                    for (int l = 0; l < covStrArr.Length; l++)
                    {
                        covStrArr[l] = cov[l, j].ToString();
                    }
                    rd.addMessage("\n" + String.Join(",", covStrArr));
                }

            }
        }
        public void writeModel(string outPath)
        {
            if (model == null) buildModel();
            outmodelpath = outPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outmodelpath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.Cluster.ToString());
                //Console.WriteLine("Cluster type = " + cType.ToString());
                sw.WriteLine(cType.ToString());
                sw.WriteLine(InPath);
                sw.WriteLine(String.Join(",", VariableFieldNames));
                sw.WriteLine(n.ToString());
                sw.WriteLine(prop.ToString());
                sw.WriteLine(k.ToString());
                sw.WriteLine(String.Join(",", lbl.ToArray()));


                switch (cType)
                {
                    case clusterType.KMEANS:
                        writeKmeansData(sw);
                        break;
                    case clusterType.BINARY:
                        writeBinaryData(sw);
                        break;
                    case clusterType.GAUSSIANMIXTURE:
                        writeGaussianData(sw);
                        break;
                    default:
                        break;
                }

                sw.Close();
            }
        }

        private void writeGaussianData(System.IO.StreamWriter sw)
        {
            GaussianClusterCollection gCol = (GaussianClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                GaussianCluster gClust = gCol[i];
                sw.WriteLine(String.Join(",", (from double d in gClust.Mean select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in gClust.Covariance select d.ToString()).ToArray()));
                sw.WriteLine(gClust.Proportion.ToString());
            }
        }

        private void writeBinaryData(System.IO.StreamWriter sw)
        {
            KMeansClusterCollection gCol = (KMeansClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                KMeansCluster gClust = gCol[i];
                sw.WriteLine(String.Join(",", (from double d in gClust.Mean select d.ToString()).ToArray()));
                sw.WriteLine("");//String.Join(",", (from double d in gClust.Covariance select d.ToString()).ToArray()));
                sw.WriteLine(gClust.Proportion.ToString());
            }
        }

        private void writeKmeansData(System.IO.StreamWriter sw)
        {
            KMeansClusterCollection gCol = (KMeansClusterCollection)clusterCollection;
            for (int i = 0; i < gCol.Count; i++)
            {
                KMeansCluster gClust = gCol[i];
                sw.WriteLine(String.Join(",", (from double d in gClust.Mean select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in gClust.Covariance select d.ToString()).ToArray()));
                sw.WriteLine(gClust.Proportion.ToString());
            }
        }
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
                cType = (clusterType)System.Enum.Parse(typeof(clusterType), sr.ReadLine());
                inpath = sr.ReadLine();
                VariableFieldNames = sr.ReadLine().Split(new char[] { ',' });
                n = System.Convert.ToInt32(sr.ReadLine());
                prop = System.Convert.ToDouble(sr.ReadLine());
                k = System.Convert.ToInt32(sr.ReadLine());
                lbl = sr.ReadLine().Split(new char[] { ',' }).ToList();
                switch (cType)
                {
                    case clusterType.KMEANS:
                        setKMeansCluster(sr);
                        break;
                    case clusterType.BINARY:
                        setBinaryCluster(sr);
                        break;
                    case clusterType.GAUSSIANMIXTURE:
                        setGaussianCluster(sr);
                        break;
                    default:
                        break;
                }
                sr.Close();
            }
        }

        private void setGaussianCluster(System.IO.StreamReader sr)
        {
            KMeans tKMeans = new KMeans(k);
            KMeansClusterCollection kmeansColl = tKMeans.Clusters;
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
            GaussianMixtureModel gModel = new GaussianMixtureModel(tKMeans);
            clusterCollection = gModel.Gaussians;
            model = gModel;
        }

        private void setBinaryCluster(System.IO.StreamReader sr)
        {
            BinarySplit bSplit = new BinarySplit(k);
            KMeansClusterCollection kmeansColl = bSplit.Clusters;
            for (int i = 0; i < k; i++)
            {
                double[] mns = (from s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                sr.ReadLine();
                double p = System.Convert.ToDouble(sr.ReadLine());
                KMeansCluster kc = new KMeansCluster(kmeansColl, i);
                kc.Mean = mns;
                kc.Proportion = p;
            }
            clusterCollection = kmeansColl;
            model = bSplit;
        }

        private void setKMeansCluster(System.IO.StreamReader sr)
        {
            KMeans kmeans = new KMeans(k);
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
            clusterCollection = kmeansColl;
            model = kmeans;
        }

        public abstract int computNew(double[] input);
        
    }
}

