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
    public class dataPrepPrincipleComponents
    {
        public dataPrepPrincipleComponents()
        {
        }
        public dataPrepPrincipleComponents(string varianceCovariancePath)
        {
            varcovpath = varianceCovariancePath;
            buildModel();
        }

        
        public dataPrepPrincipleComponents(IRaster raster,string varianceCovariancePath=null)
        {
            inraster = raster;
            IRasterBandCollection bc = (IRasterBandCollection)inraster;
            int bcCnt = bc.Count;
            VariableFieldNames = new string[bcCnt];
            for (int i = 0; i < bcCnt; i++)
            {
                VariableFieldNames[i] = "band_" + (i + 1).ToString();
            }
            varcovpath = varianceCovariancePath;
            buildModel();
        }
        public dataPrepPrincipleComponents(ITable table, string[] variables, string varianceCovariancePath = null)
        {
            InTable = table;
            VariableFieldNames = variables;
            varcovpath = varianceCovariancePath;
            buildModel();
        }
        private int n = 0;
        public int N { get { return n; } }
        private string varcovpath = null;
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
        private PrincipalComponentAnalysis pca = null;
        public PrincipalComponentAnalysis PCA { get { return pca; } }
        dataPrepVarCovCorr varCov = null;
        private double[,] egVec = null;
        public double[,] EigenVectors
        {
            get
            {
                if (egVec == null) buildModel();
                return egVec;
            }
        }
        private double[] prop = null;
        public double[] ProportionOfTotalVariance
        {
            get
            {
                if (prop == null) buildModel();
                return prop;
            }
        }
        private double[] egVal = null;

        private double[,] corr = null;
        public double[,] CorralationMatrix
        {
            get
            {
                if (corr == null) buildModel();
                return corr;
            }
        }
        private double[] meanVector=null;
        public double[] MeanVector
        {
            get
            {
                if (meanVector == null) buildModel();
                return meanVector;
            }
        }
        private double[] stdVector = null;
        public double[] StdVector
        {
            get
            {
                if (stdVector == null) buildModel();
                return stdVector;
            }
        }
        public double[] EigenValues
        {
            get
            {
                if (egVal == null) buildModel();
                return egVal;
            }
        }
        private void getCov()
        {
            if (varcovpath != null)
            {
                varCov = new dataPrepVarCovCorr();
                varCov.buildModel(varcovpath);
                if (intable == null && inraster == null)
                {
                    VariableFieldNames = varCov.VariableFieldNames;
                    inpath = varCov.InPath;
                    intable= geoUtil.getTable(inpath);
                    if (intable == null)
                    {
                        inraster = rsUtil.returnRaster(inpath);
                    }
                }
            }
            else
            {
                if (InRaster == null)
                {
                    varCov = new dataPrepVarCovCorr(InTable, VariableFieldNames);
                }
                else
                {
                    varCov = new dataPrepVarCovCorr(InRaster);
                }
            }
            meanVector = varCov.MeanVector;
            stdVector = varCov.StdVector;
            corr = varCov.CorralationMatrix;
            n = varCov.N;
        }
        private void buildModel()
        {
            if (varCov == null) getCov();
            pca = PrincipalComponentAnalysis.FromCorrelationMatrix(MeanVector, StdVector, CorralationMatrix);
            pca.Compute();
            egVec = pca.ComponentMatrix;
            prop = pca.ComponentProportions;
            egVal = pca.Eigenvalues;
            //Console.WriteLine("PCA method = " + pca.Method.ToString());
            
        }
        public void getReport()
        {
            Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.Text = "PCA Results";
            rd.TopLevel = true;
            rd.pgbProcess.Visible = false;
            rd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            rd.addMessage("Sample size = " + n.ToString());
            rd.addMessage("Vars:  " + String.Join(", ", VariableFieldNames));
            rd.addMessage("\nProportions:  " + String.Join(", ", (from double d in ProportionOfTotalVariance select d.ToString()).ToArray()));
            rd.addMessage("\nEigenValues: " + String.Join(", ", (from double d in EigenValues select d.ToString()).ToArray()));
            rd.addMessage("\n\nEigenvectors:\n");
            double[,] scov = EigenVectors;
            for (int i = 0; i < VariableFieldNames.Length; i++)
            {
                string[] vlArr = new string[VariableFieldNames.Length];
                for (int j = 0; j < VariableFieldNames.Length; j++)
                {
                    vlArr[j] = scov[j, i].ToString();
                }
                rd.addMessage(String.Join(", ", vlArr));
            }
            rd.enableClose();
            rd.Show();
        }
        public void writeModel(string outPath)
        {
            if (EigenVectors == null) buildModel();
            outmodelpath = outPath;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outmodelpath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.PCA.ToString());
                sw.WriteLine(InPath);
                sw.WriteLine(String.Join(",", VariableFieldNames));
                sw.WriteLine(n.ToString());
                sw.WriteLine(String.Join(",", (from double d in meanVector select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in stdVector select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in corr select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in ProportionOfTotalVariance select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in EigenValues select d.ToString()).ToArray()));
                sw.WriteLine(String.Join(",", (from double d in EigenVectors select d.ToString()).ToArray()));
                sw.Close();
            }
        }
        public void buildModel(string modelPath)
        {
            outmodelpath = modelPath;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(outmodelpath))
            {
                dataPrepBase.modelTypes mType = (dataPrepBase.modelTypes)Enum.Parse(typeof(dataPrepBase.modelTypes), sr.ReadLine());
                if (mType != dataPrepBase.modelTypes.PCA)
                {
                    egVec = new double[1, 1];
                    System.Windows.Forms.MessageBox.Show("Not a PCA Model!!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                inpath = sr.ReadLine();
                VariableFieldNames = sr.ReadLine().Split(new char[] { ',' });
                corr = new double[VariableFieldNames.Length, VariableFieldNames.Length];
                egVec = new double[VariableFieldNames.Length, VariableFieldNames.Length];
                n = System.Convert.ToInt32(sr.ReadLine());
                meanVector = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                stdVector = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                string[] corrLg = sr.ReadLine().Split(new char[] { ',' });
                prop = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                egVal = (from string s in sr.ReadLine().Split(new char[] { ',' }) select System.Convert.ToDouble(s)).ToArray();
                string[] egVecLg = sr.ReadLine().Split(new char[] { ',' });
                for (int i = 0; i < VariableFieldNames.Length; i++)
                {
                    for (int j = 0; j < VariableFieldNames.Length; j++)
                    {
                        int indexVl = (i * VariableFieldNames.Length) + j;
                        corr[i, j] = System.Convert.ToDouble(corrLg[indexVl]);
                        egVec[i, j] = System.Convert.ToDouble(egVecLg[indexVl]);
                    }
                }
                sr.Close();
            }
            pca = PrincipalComponentAnalysis.FromCorrelationMatrix(meanVector, stdVector, corr);
            pca.Compute();
        }

        public double[] computNew(double[] input)
        {
            return pca.Transform(input,VariableFieldNames.Length);
        }
    }
}
