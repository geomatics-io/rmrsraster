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
    public class dataPrepCluster
    {
        public dataPrepCluster()
        {
        }
        


        public dataPrepCluster(IRaster raster, int numberOfClasses)
        {
            inraster = raster;
            IRasterBandCollection bc = (IRasterBandCollection)inraster;
            int bcCnt = bc.Count;
            VariableFieldNames = new string[bcCnt];
            for (int i = 0; i < bcCnt; i++)
            {
                VariableFieldNames[i] = "band_" + (i + 1).ToString();
            }
            k = numberOfClasses;
            buildModel();
        }
        public dataPrepCluster(ITable table, string[] variables, int numberOfClasses)
        {
            InTable = table;
            VariableFieldNames = variables;
            k = numberOfClasses;
            buildModel();
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
        private string outmodelpath = null;
        private KMeans kmeans = null;
        public KMeans Model { get { return kmeans; } }
        double[][]inputMatrix = null;
        private int k = 2;
        public int Classes { get { return k; } }
        private List<string> lbl = null;
        public List<string> Labels
        {
            get
            {
                return lbl;
            }
        }
        private void buildModel()
        {
            if (inputMatrix == null) getMatrix();
            kmeans = new KMeans(k);
            kmeans.Compute(inputMatrix,0.0001);
            lbl = new List<string>();
            for (int i = 0; i < k; i++)
            {
                lbl.Add(i.ToString());
            }
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
            System.Array nDataVlArr = (System.Array)rsp.NoDataValue;
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            IPixelBlock pb=null;
            System.Array[] pbArrs=new System.Array[rsbc.Count];
            Random rand = new Random();
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
                        if (rand.NextDouble() <= prop)
                        {
                            double[] vlBandArr = new double[rsbc.Count];
                            for (int p = 0; p < pb.Planes; p++)
                            {
                                System.Array pbArr = pbArrs[p];
                                double vl = System.Convert.ToDouble(pbArr.GetValue(c, r));
                                if (rasterUtil.isNullData(vl, nDataVlArr.GetValue(p)))
                                {
                                    vl = 0;
                                }
                                vlBandArr[p] = vl;
                            }
                            inputMatrixLst.Add(vlBandArr);
                            n++;
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
            qrf.SubFields = String.Join(", ",VariableFieldNames);
            ICursor cur = InTable.Search(qrf, false);
            IFields flds = cur.Fields;
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                //Console.WriteLine(VariableFieldNames[i]);
                fldsIndex[i] = flds.FindField(VariableFieldNames[i]);
            }
            IRow rw = cur.NextRow();
            System.Random r =new Random();
            
            while (rw != null)
            {
                if(r.NextDouble()<=prop)
                {
                    double[] vlArr = new double[VariableFieldNames.Length];
                    for (int i = 0; i < fldsIndex.Length; i++)
                    {
                        int fldIndex = fldsIndex[i];
                        double vl = System.Convert.ToDouble(rw.get_Value(fldIndex));
                        //Console.WriteLine(vl.ToString());
                        if (Double.IsNaN(vl))
                        {
                            vl=0;
                        }
                        vlArr[i] = vl;
                    }
                    inputMatrixLst.Add(vlArr);
                    n++;
                }

                rw = cur.NextRow();
            }
            inputMatrix = inputMatrixLst.ToArray();
        }

        private void getProportionOfSamples()
        {
            double maxRecords = 10000000d;
            int rec = 0;
            if (inraster == null)
            {
                rec = intable.RowCount(null)*VariableFieldNames.Length;
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
            if (kmeans == null) buildModel();
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Cluster Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Input path = " + InPath);
            rd.addMessage("Sample size = " + n.ToString() + " proportion of total records = " + prop.ToString() );
            rd.addMessage("Number of Cluster = " + k.ToString());
            rd.addMessage("Labels = " + String.Join(", ", lbl.ToArray()));
            rd.addMessage("Variables: " + String.Join(" ,", VariableFieldNames));
            KMeansClusterCollection gCol = kmeans.Clusters;
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
            rd.enableClose();
            rd.Show();
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
                    double[] mns =  (from s in (sr.ReadLine().Split(new char[]{','})) select System.Convert.ToDouble(s)).ToArray();
                    string[] covVlsStr = sr.ReadLine().Split(new char[]{','});
                    double p = System.Convert.ToDouble(sr.ReadLine());
                    double[,] cov = new double[VariableFieldNames.Length,VariableFieldNames.Length];
                    for (int j = 0; j < VariableFieldNames.Length; j++)
                    {
                        for (int l = 0; l < VariableFieldNames.Length; l++)
                        {
                            int indexVl = (j * VariableFieldNames.Length) + l;
                            cov[l,j] = System.Convert.ToDouble(covVlsStr[indexVl]);
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

        public int computNew(double[] input)
        {
            return kmeans.Clusters.Nearest(input);
        }
    }
}
