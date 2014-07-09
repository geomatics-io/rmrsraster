using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Statistics
{
    public class dataPrepRandomForest: dataPrepBase
    {
        public dataPrepRandomForest()
        {
        }
        public IRaster InputRaster
        {
            get
            {
                return inRs;
            }
            set
            {
                inRs = value;
            }
        }
        IRaster inRs = null;
        //public dataPrepRandomForest(IRaster inputRaster, int Clusters, int trees, double ratio, int nSplitVar, bool buildImportanceGraph = false)
        //{
        //    inRs = inputRaster;
        //    IRasterBandCollection rsBc = (IRasterBandCollection)inputRaster;
        //    IDataset rsDset = (IDataset)((IRaster2)inputRaster).RasterDataset;
        //    InTablePath = rsDset.Workspace.PathName+"\\" + rsDset.BrowseName;
        //    DependentFieldNames = null;
        //    string[] tInd = new string[rsBc.Count];
        //    for (int i = 0; i < rsBc.Count; i++)
        //    {
        //        tInd[i] = "Band_" + i.ToString();
                
        //    }
        //    IndependentFieldNames = tInd;
        //    Clustering = true;
        //    numbCluster = Clusters;
        //    nclasses = numbCluster;
        //    nTrees = trees;
        //    r = ratio;
        //    advance = true;
        //    nrndvars = nSplitVar;
        //    variableImportanceGraph = buildImportanceGraph;

        //}
        public dataPrepRandomForest(string tablePath,string dependentField, string independentFields, string categoricalFields, int trees, double ratio, bool buildImportanceGraph=false, IQueryFilter qry = null)//,int Clusters = 10)
        {
            InTablePath = tablePath;
            if (dependentField == null || dependentField.Length == 0)
            {
                DependentFieldNames = null;
                Clustering = true;
                //numbCluster = Clusters;
            }
            else
            {
                DependentFieldNames = dependentField.Split(new char[] { ',' });
            }
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            nTrees = trees;
            generalQf = qry;
            r = ratio;

            variableImportanceGraph = buildImportanceGraph;
            
        }
        public dataPrepRandomForest(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, int trees, double ratio, bool buildImportanceGraph = false,IQueryFilter qry = null)//, int Clusters = 10)
        {
            InTable = table;
            if (dependentField == null || dependentField.Length == 0)
            {
                DependentFieldNames = null;
                Clustering = true;
                //numbCluster = Clusters;
            }
            else
            {
                DependentFieldNames = dependentField;
            }
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            generalQf = qry;
            nTrees = trees;
            r = ratio;
            variableImportanceGraph = buildImportanceGraph;
        }
        public dataPrepRandomForest(string tablePath, string dependentField, string independentFields, string categoricalFields, int trees, double ratio, int nSplitVar, bool buildImportanceGraph = false, IQueryFilter qry = null)//, int Clusters = 10)
        {
            InTablePath = tablePath;
            if (dependentField == null || dependentField.Length == 0)
            {
                DependentFieldNames = null;
                Clustering = true;
                //numbCluster = Clusters;
            }
            else
            {
                DependentFieldNames = dependentField.Split(new char[] { ',' });
            }
            IndependentFieldNames = independentFields.Split(new char[] { ',' });
            ClassFieldNames = categoricalFields.Split(new char[] { ',' });
            nTrees = trees;
            r = ratio;
            generalQf = qry;
            advance = true;
            nrndvars = nSplitVar;
            variableImportanceGraph = buildImportanceGraph;
        }
        public dataPrepRandomForest(ITable table, string[] dependentField, string[] independentFields, string[] categoricalFields, int trees, double ratio, int nSplitVar, bool buildImportanceGraph = false, IQueryFilter qry = null)//, int Clusters = 10)
        {
            InTable = table;
            if (dependentField == null || dependentField.Length == 0)
            {
                DependentFieldNames = null;
                Clustering = true;
                //numbCluster = Clusters;
            }
            else
            {
                DependentFieldNames = dependentField;
            }
            IndependentFieldNames = independentFields;
            ClassFieldNames = categoricalFields;
            nTrees = trees;
            r = ratio;
            advance = true;
            generalQf = qry;
            nrndvars = nSplitVar;
            variableImportanceGraph = buildImportanceGraph;
        }
        private bool reg = false;
        public bool Regression
        {
            get
            {
                return reg;
            }
        }
        private bool advance = false;
        public bool Advanced
        {
            get
            {
                return advance;
            }
        }
        private bool variableImportanceGraph = false;
        public bool VariableImportanceGraph
        {
            get
            {
                return variableImportanceGraph;
            }
            set
            {
                variableImportanceGraph = value;
            }
        }
        private string[] categories = null;
        public string[] Categories 
        { 
            get 
            {
                if (!reg&&categories == null) getDfModel();
                return categories; 
            } 
        }
        private int nTrees = 75;
        public double[] MinValues { get { return minValues; } }
        public double[] MaxValues { get { return maxValues; } }
        public int NumberOfTrees
        {
            get
            {
                return nTrees;
            }
            set
            {
                nTrees = value;
            }
        }
        private double r = 0.30;
        public double Ratio
        {
            get
            {
                return r;
            }
            set
            {
                r = value;
            }
        }
        private int nrndvars = 2;
        public int NumberOfSplitVariables
        {
            get
            {
                return nrndvars;
            }
            set
            {
                nrndvars = value;
                if (nrndvars > 0)
                {
                    advance = true;
                }
                else
                {
                    advance = false;
                }
            }
        }
        private bool Clustering = false;
        private Dictionary<string, List<string>> unVlDic = null;
        private string[] allFieldNames = null;
        private double[,] outMatrix = null;
        public int NumberOfVariables { get { return nvars; } }
        private int nvars = 0;
        private int nclasses = 1;
        private int n = 0;
        public int SampleSize
        {
            get
            {
                return n;
            }
        }

        public override double[,] getMatrix()
        {
            if (inRs == null)
            {
                if (unVlDic == null) unVlDic = UniqueClassValues;
                checkRegression();
                //Console.WriteLine("Reg value = " + Regression.ToString());
                int indCnt = IndependentFieldNames.Length;
                int depCnt = 0;
                if (DependentFieldNames != null)
                {
                    depCnt = DependentFieldNames.Length;
                }
                int rws = InTable.RowCount(generalQf);
                n = rws;
                ICursor cur = InTable.Search(generalQf, false);
                int clms = indCnt + depCnt;
                int[] allFieldIndexArray = new int[clms];
                allFieldNames = new string[clms];
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

                for (int i = 0; i < depCnt; i++)
                {
                    string lu = DependentFieldNames[i];
                    if (!reg)
                    {
                        categories = unVlDic[lu].ToArray();
                    }
                    allFieldNames[i + indCnt] = lu;
                    allFieldIndexArray[i + indCnt] = cur.FindField(lu);
                }


                if (Clustering)
                {
                    nvars = clms;
                    categories = new string[numbCluster];
                    for (int i = 0; i < numbCluster; i++)
                    {
                        categories[i] = i.ToString();
                    }
                }
                else
                {
                    nvars = clms - 1;
                }
                deltaOobavgcee = new double[nvars];
                deltaOobavge = new double[nvars];
                deltaOobavgre = new double[nvars];
                deltaOobrce = new double[nvars];
                deltaOobrmse = new double[nvars];
                //Console.WriteLine("Total Rows Columns  = " + rws.ToString() + ", " + clms.ToString());
                outMatrix = new double[rws, clms];
                int rwCnt = 0;
                IRow rw = cur.NextRow();
                while (rw != null)
                {
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
                                if (i == allFieldIndexArray.Length - 1)
                                {
                                    dblVl = fldClmCntP;
                                }
                                else
                                {
                                    fldClmCntT = unVl.Count() - fldClmCntP;
                                    indMatrixClm += fldClmCntP;
                                    dblVl = 1;
                                }
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
                        //Console.WriteLine("Matrix R:C = " + rwCnt.ToString() + ", " + indMatrixClm.ToString());
                        outMatrix[rwCnt, indMatrixClm] = dblVl;
                        indMatrixClm += fldClmCntT;

                    }
                    rw = cur.NextRow();
                    rwCnt += 1;
                }
            }
            else
            {
                
                int bCnt = ((IRasterBandCollection)inRs).Count;
                nvars = bCnt;
                categories = new string[numbCluster];
                for (int i = 0; i < numbCluster; i++)
                {
                    categories[i] = i.ToString();
                }
                deltaOobavgcee = new double[bCnt];
                deltaOobavge = new double[bCnt];
                deltaOobavgre = new double[bCnt];
                deltaOobrce = new double[bCnt];
                deltaOobrmse = new double[bCnt];
                getProportionOfSamples();
                getRasterMatrix();
            }
            return outMatrix;
        }
        double prop = 1;
        private void getProportionOfSamples()
        {
            double maxRecords = 10000000d;
            int rec = 0;
            IRasterProps rsp = (IRasterProps)inRs;
            rec = rsp.Height * rsp.Width * ((IRasterBandCollection)inRs).Count;
            if (rec > maxRecords)
            {
                prop = maxRecords / rec;
            }
            else
            {
                prop = 1;
            }
        }
        private void getRasterMatrix()
        {
            n = 0;
            IRaster2 rs2 = (IRaster2)inRs;
            IRasterBandCollection rsbc = (IRasterBandCollection)rs2;
            IRasterProps rsp = (IRasterProps)rs2;
            System.Array nDataVlArr = (System.Array)rsp.NoDataValue;
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            IPixelBlock pb = null;
            Random rand = new Random();
            List<double>[] jagArrLst = new List<double>[rsbc.Count];
            for (int i = 0; i < rsbc.Count; i++)
            {
                jagArrLst[i] = new List<double>();
            }
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
                                double vl = System.Convert.ToDouble(vlObj);
                                updateMinMaxSum(vl, 0);
                                jagArrLst[0].Add(vl);
                                //outMatrix[n, 0] = vl;
                                for (int p = 1; p < pb.Planes; p++)
                                {
                                    vlObj = pb.GetVal(p,c,r);
                                    vl = System.Convert.ToDouble(vlObj);
                                    updateMinMaxSum(vl, p);
                                    jagArrLst[p].Add(vl);
                                    //outMatrix[n, p] = vl;
                                }
                                n++;
                            }
                        }
                    }
                }

            } while (rsCur.Next() == true);
            outMatrix = new double[n, rsbc.Count];
            for (int v = 0; v < rsbc.Count; v++)
            {
                for (int r = 0; r < n; r++)
                {
                    outMatrix[r, v] = jagArrLst[v][r];
                }
                
            }
            
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
        private void checkRegression()
        {
            if (Clustering == true)
            {
                reg = false;
                nclasses = numbCluster;
            }
            else
            {
                bool fKey = false;
                string vl = "";
                for (int i = 0; i < DependentFieldNames.Length; i++)
                {
                    vl = DependentFieldNames[i];
                    if (unVlDic.ContainsKey(vl))
                    {
                        fKey = true;
                        break;
                    }
                }
                if (fKey)
                {
                    reg = false;
                    nclasses = unVlDic[vl].Count;
                }
                else
                {
                    reg = true;
                    nclasses = 1;
                }
            }
        }
        public string[] getUniqueValues(string classVariable)
        {
            if (unVlDic == null) unVlDic = UniqueClassValues;
            return unVlDic[classVariable].ToArray();
        }
        public override double[] getArray(string varName)
        {
            if (outMatrix==null) getMatrix();
            int matrixClmIndex = allFieldNames.ToList().IndexOf(varName);
            double[] outArray = new double[outMatrix.GetUpperBound(0)+1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = outMatrix[matrixClmIndex, i];
            }
            return outArray;
        }
        public double[] getArrayByIndex(int varIndex)
        {
            if (outMatrix == null) getMatrix();
            double[] outArray = new double[outMatrix.GetUpperBound(0) + 1];
            for (int i = 0; i < outArray.Length; i++)
            {
                outArray[i] = outMatrix[i,varIndex];
            }
            return outArray;
        }
        private string outPath = "";
        public string OutModelPath { get { return outPath; } }
        public string writeModel(string outModelPath)
        {
            if (df == null) getDfModel();
            outPath = outModelPath;
            string s_out;
            try
            {
                alglib.dfserialize(df, out s_out);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                s_out = "rebuild";
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(modelTypes.RandomForest.ToString());
                sw.WriteLine(InTablePath);
                sw.WriteLine(String.Join(",",IndependentFieldNames));
                if (DependentFieldNames == null)
                {
                    sw.WriteLine("");
                }
                else
                {
                    sw.WriteLine(String.Join(",", DependentFieldNames));
                }
                if (ClassFieldNames == null || ClassFieldNames.Length == 0) sw.WriteLine();
                else sw.WriteLine(String.Join(",", ClassFieldNames));
                if (Categories == null || Categories.Length == 0) sw.WriteLine();
                else sw.WriteLine(String.Join(",", Categories));
                sw.WriteLine(NumberOfTrees.ToString());
                sw.WriteLine(Ratio.ToString());
                sw.WriteLine(NumberOfSplitVariables.ToString());
                sw.WriteLine(Regression.ToString());
                string nLn = Clustering.ToString();
                if (generalQf != null) nLn = nLn + ":" + generalQf.WhereClause;
                sw.WriteLine(nLn);//need to add this part to model building
                sw.WriteLine(Advanced.ToString());
                if (Clustering)
                {
                    sw.WriteLine(String.Join(",",(from string d in IndependentFieldNames select d).ToArray()));
                }
                else
                {
                    sw.WriteLine(getAllVariableNames());
                }
                sw.WriteLine(NumberOfVariables.ToString());
                sw.WriteLine(NumberOfClasses.ToString());
                sw.WriteLine(SampleSize.ToString());
                sw.WriteLine(AverageCrossEntropyError.ToString());
                sw.WriteLine(AverageError.ToString());
                sw.WriteLine(AverageRelativeError.ToString());
                sw.WriteLine(RMSE.ToString());
                sw.WriteLine(RelativeClassificationError.ToString());
                sw.WriteLine(OOBAverageCrossEntropyError.ToString());
                sw.WriteLine(OOBAverageError.ToString());
                sw.WriteLine(OOBAverageRelativeError.ToString());
                sw.WriteLine(OOBRelativeClassificationError.ToString());
                sw.WriteLine(OOBRMSE.ToString());
                sw.WriteLine(String.Join(",",(from double d in minValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in maxValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in sumValues select d.ToString()).ToArray()));
                sw.WriteLine(VariableImportanceGraph.ToString());
                sw.WriteLine(String.Join(",", (from double d in deltaOobavgcee select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in deltaOobavge select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in deltaOobavgre select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in deltaOobrce select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in deltaOobrmse select d.ToString()).ToArray()));
                sw.WriteLine(s_out);
                sw.Close();
            }
            return outPath;
        }

        private string getAllVariableNames()
        {
            string[] allvn = new string[IndependentFieldNames.Length];
            int cnt = 0;
            foreach (string s in IndependentFieldNames)
            {
                string vlS = s;
                List<string> outLst = new List<string>();
                if (unVlDic.TryGetValue(s, out outLst))
                {
                    vlS = String.Join(",", outLst.ToArray());
                }
                allvn[cnt] = vlS;
                cnt += 1;
            }
            return String.Join(",", allvn);
        }
        public int NumberOfClasses { get { return nclasses; } }
        public alglib.decisionforest getDfModel(string modelPath,bool rebuildIfTooBig=true)
        {
            string s_in = null;
            
            using (System.IO.StreamReader sr = new System.IO.StreamReader(modelPath))
            {
                string mType = sr.ReadLine();
                modelTypes m = (modelTypes)Enum.Parse(typeof(modelTypes), mType);
                if (m != modelTypes.RandomForest)
                {
                    System.Windows.Forms.MessageBox.Show("Model file specified is not a Random Forest Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return null;
                }
                InTablePath = sr.ReadLine();
                IndependentFieldNames = sr.ReadLine().Split(new char[]{','});
                DependentFieldNames = sr.ReadLine().Split(new char[] { ',' });
                ClassFieldNames = sr.ReadLine().Split(new char[] { ',' });
                categories = sr.ReadLine().Split(new char[] { ',' });
                nTrees = System.Convert.ToInt32(sr.ReadLine());
                r = System.Convert.ToDouble(sr.ReadLine());
                nrndvars = System.Convert.ToInt32(sr.ReadLine());
                reg = System.Convert.ToBoolean(sr.ReadLine());
                string nLn = sr.ReadLine();
                string[] nLnArr = nLn.Split(new char[]{':'});
                Clustering = System.Convert.ToBoolean(nLnArr[0]);
                if (nLnArr.Length > 1)
                {
                    generalQf = new QueryFilterClass();
                    generalQf.WhereClause = nLnArr[1];
                }
                advance = System.Convert.ToBoolean(sr.ReadLine());
                string allvariablenames = sr.ReadLine();
                nvars = System.Convert.ToInt32(sr.ReadLine());
                nclasses = System.Convert.ToInt32(sr.ReadLine());
                if (Clustering)
                {
                    DependentFieldNames = null;
                    numbCluster = nclasses;
                }
                n=System.Convert.ToInt32(sr.ReadLine());
                avgcee = System.Convert.ToDouble(sr.ReadLine());
                avge = System.Convert.ToDouble(sr.ReadLine());
                avgre = System.Convert.ToDouble(sr.ReadLine());
                rmse = System.Convert.ToDouble(sr.ReadLine());
                rce = System.Convert.ToDouble(sr.ReadLine());
                oobavgcee = System.Convert.ToDouble(sr.ReadLine());
                oobavge = System.Convert.ToDouble(sr.ReadLine());
                oobavgre = System.Convert.ToDouble(sr.ReadLine());                
                oobrce = System.Convert.ToDouble(sr.ReadLine());
                oobrmse = System.Convert.ToDouble(sr.ReadLine());
                minValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                maxValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                sumValues = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                variableImportanceGraph = System.Convert.ToBoolean(sr.ReadLine());
                deltaOobavgcee = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                deltaOobavge = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                deltaOobavgre = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                deltaOobrce = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                deltaOobrmse = (from string s in (sr.ReadLine().Split(new char[] { ',' })) select System.Convert.ToDouble(s)).ToArray();
                s_in = sr.ReadToEnd();
                //Console.WriteLine("Sin = '"+s_in+"'");
                sr.Close();
            }
            if (!s_in.StartsWith("rebuild"))
            {
                alglib.dfunserialize(s_in, out df);
            }
            else
            {
                if (rebuildIfTooBig)
                {
                    //Console.WriteLine("Rebuilding Model!!");
                    getDfModel();
                }
                else
                {
                    //Console.WriteLine("Not Rebuilding model");
                }
            }
            //Console.WriteLine(df == null);
            return df;
        }
        private alglib.decisionforest df = null;
        public alglib.decisionforest DecisionForest
        {
            get
            {
                if (df == null) getDfModel();
                return df;
            }
        }
        private alglib.dfreport rep = null;
        private int info;
        private double rmse = Double.NaN;
        private double avgcee = Double.NaN;
        private double avge = Double.NaN;
        private double avgre = Double.NaN;
        private double rce = Double.NaN;
        private double oobrmse = Double.NaN;
        private double oobavgcee = Double.NaN;
        private double oobavge = Double.NaN;
        private double oobavgre = Double.NaN;
        private double oobrce = Double.NaN;
        public alglib.decisionforest getDfModel()
        {
            if(outMatrix==null) getMatrix();
            if(advance)
            {
                //Console.WriteLine(n.ToString());
                //Console.WriteLine(nvars.ToString());
                //Console.WriteLine(nclasses.ToString());
                //Console.WriteLine(nTrees.ToString());
                //Console.WriteLine(nrndvars.ToString());
                //Console.WriteLine(outMatrix.GetUpperBound(0).ToString());
                //Console.WriteLine(outMatrix.GetUpperBound(1).ToString());
                alglib.dfbuildrandomdecisionforestx1(outMatrix, n, nvars, nclasses, nTrees, nrndvars, r, out info, out df, out rep);
            }
            else
            {
                alglib.dfbuildrandomdecisionforest(outMatrix, n, nvars, nclasses, nTrees,r,out info, out df, out rep);
            }
            rmse = rep.rmserror;
            avgcee = rep.avgce;
            avge = rep.avgerror;
            avgre = rep.avgrelerror;
            rce = rep.relclserror;
            oobrmse = rep.oobrmserror;
            oobavgcee = rep.oobavgce;
            oobavge = rep.oobavgerror;
            oobavgre = rep.oobavgrelerror;
            oobrce = rep.oobrelclserror;
            if (VariableImportanceGraph) calcVarImportance();
            return df;
        }
        private double[] deltaOobrmse, deltaOobavgcee, deltaOobavge, deltaOobavgre, deltaOobrce;
        private void calcVarImportance()
        {
            int info2;
            alglib.decisionforest df2;
            alglib.dfreport rep2;
            int records,variables;
            records = outMatrix.GetUpperBound(0)+1;
            variables = outMatrix.GetUpperBound(1)+1;
            double[,] outMatrix2 = new double[records, variables-1];
            unsafe
            {
                fixed (double* pSrc = outMatrix, pDst = outMatrix2)
                {
                    for (int c = 1; c< variables; c++)
                    {
                        for (int r = 0; r < records; r++)
                        {
                            int indS = c+variables*r;
                            int indD = ((c-1)+(variables-1)*r);
                            pDst[indD] = pSrc[indS];
                            //outMatrix2[r, v - 1] = outMatrix[r, v];
                        }
                    }
                }
            }
            
            if (advance)
            {
                alglib.dfbuildrandomdecisionforestx1(outMatrix2, n, nvars-1, nclasses, nTrees, nrndvars, r, out info2, out df2, out rep2);
            }
            else
            {
                alglib.dfbuildrandomdecisionforest(outMatrix2, n, nvars-1, nclasses, nTrees, r, out info2, out df2, out rep2);
            }
            deltaOobavgcee[0] = rep2.oobavgce;
            deltaOobavge[0] = rep2.oobavgerror;
            deltaOobavgre[0] = rep2.oobavgrelerror;
            deltaOobrce[0] = rep2.oobrelclserror;
            deltaOobrmse[0] = rep2.oobrmserror;

            for (int v = 0; v < variables-2; v++)
			{
                unsafe
                {
                    fixed (double* pSrc = outMatrix, pDst = outMatrix2)
                    {
                        for (int r = 0; r < records; r++)
                        {
                            int indD = v + (variables-1) * r;
                            int indS = v + (variables * r);
                            pDst[indD] = pSrc[indS];
                            //outMatrix2[r, v - 1] = outMatrix[r, v];
                        }
                    }
                }
                if (advance)
                {
                    alglib.dfbuildrandomdecisionforestx1(outMatrix2, n, nvars - 1, nclasses, nTrees, nrndvars, r, out info2, out df2, out rep2);
                }
                else
                {
                    alglib.dfbuildrandomdecisionforest(outMatrix2, n, nvars - 1, nclasses, nTrees, r, out info2, out df2, out rep2);
                }
                deltaOobavgcee[v + 1] = rep2.oobavgce;
                deltaOobavge[v + 1] = rep2.oobavgerror;
                deltaOobavgre[v + 1] = rep2.oobavgrelerror;
                deltaOobrce[v + 1] = rep2.oobrelclserror;
                deltaOobrmse[v + 1] = rep2.oobrmserror;
			 
			}
            
        }
        public double RMSE
        {
            get
            {
                if (Double.IsNaN(rmse)) getDfModel();
                return rmse;
            }
        }
        public double AverageError
        {
            get
            {
                if (Double.IsNaN(avge)) getDfModel();
                return avge;
            }
        }
        public double AverageRelativeError
        {
            get
            {
                if (Double.IsNaN(avgre)) getDfModel();
                return avgre;
            }
        }
        public double AverageCrossEntropyError
        {
            get
            {
                if (Double.IsNaN(avgcee)) getDfModel();
                return avgcee;
            }
        }
        public double RelativeClassificationError
        {
            get
            {
                if (Double.IsNaN(rce)) getDfModel();
                return rce;
            }
        }
        public double OOBRMSE
        {
            get
            {
                if (Double.IsNaN(rmse)) getDfModel();
                return oobrmse;
            }
        }
        public double OOBAverageError
        {
            get
            {
                if (Double.IsNaN(avge)) getDfModel();
                return oobavge;
            }
        }
        public double OOBAverageRelativeError
        {
            get
            {
                if (Double.IsNaN(avgre)) getDfModel();
                return oobavgre;
            }
        }
        public double OOBAverageCrossEntropyError
        {
            get
            {
                if (Double.IsNaN(avgcee)) getDfModel();
                return oobavgcee;
            }
        }
        public double OOBRelativeClassificationError
        {
            get
            {
                if (Double.IsNaN(rce)) getDfModel();
                return oobrce;
            }
        }

        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "Random Forest Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Sample size = " + n.ToString());
            if (!(Categories == null || Categories.Length == 0))
            {
                rd.addMessage("Number of Classes = " + NumberOfClasses.ToString());
                rd.addMessage("Class Names and order = " + String.Join(", ", Categories));
            }
            rd.addMessage("Number of Parameters = " + NumberOfVariables.ToString());
            rd.addMessage("Number of Trees = " + NumberOfTrees.ToString());
            rd.addMessage("Data Ratio = " + Ratio.ToString());
            rd.addMessage("Number of Split Variables = " + NumberOfSplitVariables.ToString());
            rd.addMessage("Regression model = " + Regression.ToString()+"\n\nTraining Errors:\n");
            rd.addMessage("RMSE = " + RMSE.ToString());
            rd.addMessage("Average Error = " + AverageError.ToString());
            rd.addMessage("Average Relative Error = " + AverageRelativeError.ToString());
            rd.addMessage("Average Cross Entropy Error = " + AverageCrossEntropyError.ToString());
            rd.addMessage("Relative Classification Error = " + RelativeClassificationError.ToString()+"\n\nValidation Errors:\n");
            rd.addMessage("OOBRMSE = " + OOBRMSE.ToString());
            rd.addMessage("OOBAverage Error = " + OOBAverageError.ToString());
            rd.addMessage("OOBAverage Relative Error = " + OOBAverageRelativeError.ToString());
            rd.addMessage("OOBAverage Cross Entropy Error = " + OOBAverageCrossEntropyError.ToString());
            rd.addMessage("OOBRelative Classification Error = " + OOBRelativeClassificationError.ToString());
            try
            {
                if (ModelHelper.chartingAvailable())
                {
                    if (!Regression)
                    {
                        if (System.Windows.Forms.MessageBox.Show("Do you want to build probability graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            getRegChart();
                            if (variableImportanceGraph)
                            {
                                getVariableImportanceChart();
                            }
                            
                        }
                    }
                    else
                    {
                        if (System.Windows.Forms.MessageBox.Show("Do you want to build predicted values graphs?", "Graphs", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            getProbChart();
                            if (variableImportanceGraph)
                            {
                                getVariableImportanceChart();
                            }
                        }
                    }
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Cannot create charts");
            }
            finally
            {
                rd.Show();
                rd.enableClose();
            }

        }

        private void getVariableImportanceChart()
        {
            if (df == null) df = getDfModel();
            string[] errortype = { "RMSE", "Average", "Average Relative", "Average Cross Entropy", "Classification" };
            if (reg)
            {
                errortype = new string[] { "RMSE", "Average", "Average Relative" };
            }
            Forms.Stats.frmChart hist = (Forms.Stats.frmChart)ModelHelper.generateVariableImportanceGraphic(IndependentFieldNames,errortype);
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged_VI);
            hist.chrHistogram.Show();
            if (reg)
            {
                cmbPrimary.SelectedItem = "RMSE";
            }
            else
            {
                cmbPrimary.SelectedItem = "Classification";
            }
            hist.Show();
        }

        private void getProbChart()
        {
            if (df == null) df = getDfModel();
            Forms.Stats.frmChart hist = (Forms.Stats.frmChart)ModelHelper.generateRegressionGraphic(IndependentFieldNames);
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged_R);
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            tb.Scroll += new EventHandler(tb_RegionChanged_R);
            hist.chrHistogram.Show();
            cmbPrimary.SelectedItem = IndependentFieldNames[0];
            hist.Show();
        }

        private void getRegChart()
        {
            if (df == null) df = getDfModel();
            Forms.Stats.frmChart hist = (Forms.Stats.frmChart)ModelHelper.generateProbabilityGraphic(IndependentFieldNames);
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            cmbPrimary.SelectedValueChanged += new EventHandler(cmbPrimary_SelectedValueChanged);
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            tb.Scroll += new EventHandler(tb_RegionChanged);
            hist.chrHistogram.Show();
            cmbPrimary.SelectedItem = IndependentFieldNames[0];
            hist.Show();
        }
        void tb_RegionChanged_R(object sender, EventArgs e)
        {
            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues_R(frm);
        }
        public void updateFormValues_R(Forms.Stats.frmChart hist)
        {
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            System.Windows.Forms.DataVisualization.Charting.Chart ch = hist.chrHistogram;
            ch.Series.Clear();
            string cmbTxt = cmbPrimary.Text;
            int tbVl = tb.Value;
            double oVl = tbVl / 10d;
            int cmbInd = System.Array.IndexOf(IndependentFieldNames, cmbTxt);
            double[] meanArray = new double[sumValues.Length];
            for (int i = 0; i < meanArray.Length; i++)
            {
                double mV = minValues[i];
                meanArray[i] = mV + ((maxValues[i] - mV) * oVl);
            }
            double mVl = minValues[cmbInd];
            double rng = maxValues[cmbInd] - mVl;
            double stp = rng / 10;
            double[] xVlArr = new double[10];
            for (int i = 0; i < 10; i++)
            {
                xVlArr[i] = (i * stp) + mVl;
            }
            System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series.Add(DependentFieldNames[0]);
            s.BorderWidth = 3;
            s.LegendText = DependentFieldNames[0];
            s.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            s.ChartArea = "Y";
            for (int j = 0; j < xVlArr.Length; j++)
            {
                meanArray[cmbInd] = xVlArr[j];
                double vl = computNew(meanArray)[0];
                s.Points.AddXY(xVlArr[j], vl);
            }

        }
        private void cmbPrimary_SelectedValueChanged_VI(object sender, EventArgs e)
        {

            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues_VI(frm);
        }

        private void updateFormValues_VI(Forms.Stats.frmChart hist)
        {
            string[] errorType = {"RMSE","Average","Average Relative", "Average Cross Entropy","Classification"};
            if (reg)
            {
                errorType = new string[]{"RMSE","Average","Average Relative"};
            }
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            System.Windows.Forms.DataVisualization.Charting.Chart ch = hist.chrHistogram;
            ch.Series.Clear();
            string cmbTxt = cmbPrimary.Text;
            double[] arrayValues = deltaOobrce;
            int cmbInd = System.Array.IndexOf(errorType, cmbTxt);
            imVl = oobrce;
            switch (cmbInd)
            {
                case 0:
                    arrayValues = deltaOobrmse;
                    imVl = oobrmse;
                    break;
                case 1:
                    arrayValues = deltaOobavge;
                    imVl = oobavge;
                    break;
                case 2:
                    arrayValues = deltaOobavgre;
                    imVl = oobavgre;
                    break;
                case 3:
                    arrayValues = deltaOobavgcee;
                    imVl = oobavgcee;
                    break;
                default:
                    break;
            }
            List<double> arrayLst = arrayValues.ToList();
            arrayLst.Sort();
            System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series.Add("s1");
            ch.ChartAreas[0].AxisY.Minimum = arrayLst[0] * 0.9;
            ch.ChartAreas[0].AxisY.Maximum = arrayLst[arrayLst.Count - 1] * 1.1;
            ch.ChartAreas[0].AxisY.Title = cmbTxt + " Error Value";
            ch.ChartAreas[0].AxisY.LabelStyle.Format = "#.###";
            s.BorderWidth = 3;
            s.LegendText = cmbTxt;
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            s.ChartArea = "Y";
            s.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;  
            s.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Auto;
            
            Dictionary<string, int> dicCnt = new Dictionary<string, int>();
            for (int k = 0; k <arrayValues.Length; k++)
            {
                System.Windows.Forms.DataVisualization.Charting.DataPoint p = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                double ivValue = arrayLst[k];
                int vIndex = System.Array.IndexOf(arrayValues, ivValue);
                int pVIndex;
                string vLbl = IndependentFieldNames[vIndex];
                if (dicCnt.TryGetValue(vLbl, out pVIndex))
                {
                    vIndex = System.Array.IndexOf(arrayValues, ivValue, pVIndex+1);
                    dicCnt[vLbl] = vIndex;
                    vLbl = IndependentFieldNames[vIndex];
                    dicCnt.Add(vLbl,vIndex);
                }
                else
                {
                    dicCnt.Add(vLbl, vIndex);
                }
                p.Label = vLbl;
                p.XValue = k+1;
                p.YValues = new double[]{ivValue};
                s.Points.Add(p);
            }
            System.Windows.Forms.DataVisualization.Charting.DataPoint pf = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
            pf.Label = "Saturated";
            pf.XValue = arrayValues.Length+1;
            pf.YValues = new double[] { imVl };
            s.Points.Add(pf);
            ch.PostPaint += new EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(ch_PostPaint);
        }
        private double imVl = 0;
        void ch_PostPaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chArea = e.Chart.ChartAreas[0];
            double XMin = chArea.AxisX.Minimum;
            double XMax = chArea.AxisX.Maximum;
            double YMin = chArea.AxisY.Minimum;
            double YMax = chArea.AxisY.Maximum;
            double sXMin = chArea.AxisX.ValueToPixelPosition(XMin);
            double sXMax = chArea.AxisX.ValueToPixelPosition(XMax);
            double sYMin = chArea.AxisY.ValueToPixelPosition(YMin);
            double sYMax = chArea.AxisY.ValueToPixelPosition(YMax);
            double imVlpx = chArea.AxisX.ValueToPixelPosition(imVl);
            double imVlpy = chArea.AxisY.ValueToPixelPosition(imVl);
            e.ChartGraphics.Graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red), (float)imVlpy, (float)sXMin, (float)imVlpy, (float)sXMax);
        }
        private void cmbPrimary_SelectedValueChanged_R(object sender, EventArgs e)
        {

            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues_R(frm);
        }
        void tb_RegionChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues(frm);
        }
        public void updateFormValues(Forms.Stats.frmChart hist)
        {
            System.Windows.Forms.ComboBox cmbPrimary = (System.Windows.Forms.ComboBox)hist.Controls["cmbPrimary"];
            System.Windows.Forms.TrackBar tb = (System.Windows.Forms.TrackBar)hist.Controls["tbQ"];
            System.Windows.Forms.DataVisualization.Charting.Chart ch = hist.chrHistogram;
            ch.Series.Clear();
            string cmbTxt = cmbPrimary.Text;
            int tbVl = tb.Value;
            double oVl = tbVl / 10d;
            int cmbInd = System.Array.IndexOf(IndependentFieldNames, cmbTxt);
            double[] meanArray = new double[sumValues.Length];
            ch.ChartAreas[0].AxisX.Title = cmbTxt + " Values";
            for (int i = 0; i < meanArray.Length; i++)
            {
                double mV = minValues[i];
                meanArray[i] = mV + ((maxValues[i]-mV) * oVl);
            }
            double mVl = minValues[cmbInd];
            double rng = maxValues[cmbInd] - mVl;
            double stp = rng / 10;
            double[] xVlArr = new double[10];
            for (int i = 0; i < 10; i++)
            {
                xVlArr[i] = (i * stp) + mVl;
            }
            for (int i = 0; i < Categories.Length; i++)
            {
                System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series.Add(Categories[i]);
                s.BorderWidth = 3;
                s.LegendText = Categories[i];
                s.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Auto;
                ch.ChartAreas[0].AxisX.LabelStyle.Format = "#.##";
                s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                s.ChartArea = "Probs";
            }
            for (int j = 0; j < xVlArr.Length; j++)
            {
                meanArray[cmbInd] = xVlArr[j];
                double[] probArr = computNew(meanArray);
                for (int k = 0; k < probArr.Length; k++)
                {
                    System.Windows.Forms.DataVisualization.Charting.Series s = ch.Series[k];
                    s.Points.AddXY(xVlArr[j], probArr[k]);

                }


            }

        }
        private void cmbPrimary_SelectedValueChanged(object sender, EventArgs e)
        {
            
            System.Windows.Forms.Control cmb = (System.Windows.Forms.Control)sender;
            Forms.Stats.frmChart frm = (Forms.Stats.frmChart)cmb.Parent;
            updateFormValues(frm);
                    
                
            

        }

        public double[] computNew(double[] input)
        {
            double[] output = new double[NumberOfClasses];
            alglib.dfprocess(df, input, ref output);
            return output;
        }


        //need to add variable importance graphic build a rf model minus each variable and look at oobrelative classification error and oobcrossentropy error
        
        //need to add a delta probability importance graphic. Look at average change in probability for each variable by species holding other variables at mean values
        private int numbCluster = 10;
        public int NumberOfClusters 
        {
            get
            {
                return numbCluster;
            }
            set
            {
                numbCluster = value;
            }
        }
    }
}
