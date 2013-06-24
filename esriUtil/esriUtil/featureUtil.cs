using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesNetCDF;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using System.Data.SqlClient;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace esriUtil
{
    public class featureUtil
    {
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        /// <summary>
        /// creates a new field called sample and populates yes or no depending on whether that feature should be sampled based on a previously ran cluster analysis
        /// </summary>
        /// <param name="inputTable"></param>
        /// <param name="clusterModelPath"></param>
        /// <param name="proportionOfMean"></param>
        /// <param name="alpha"></param>
        public void selectClusterFeaturesToSample(ITable inputTable, string clusterModelPath, string clusterFieldName="Cluster", double proportionOfMean=0.1, double alpha=0.05, bool weightsEqual=false)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            esriUtil.Statistics.dataPrepCluster dpC = new Statistics.dataPrepCluster();
            dpC.buildModel(clusterModelPath);
            List<string> labels = dpC.Labels;
            HashSet<string> unqVls = geoUtil.getUniqueValues(inputTable, clusterFieldName);
            System.Random rd = new Random();
            int[] samplesPerCluster = esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxCluster(clusterModelPath, proportionOfMean, alpha);
            double[] propPerCluster = esriUtil.Statistics.dataPrepSampleSize.clusterProportions(clusterModelPath);
            double[] weightsPerCluster = new double[propPerCluster.Length];
            double sSamp = System.Convert.ToDouble(samplesPerCluster.Sum());
            for (int i = 0; i < weightsPerCluster.Length; i++)
            {
                weightsPerCluster[i] = propPerCluster[i] / (samplesPerCluster[i] / sSamp);
            }
            if (weightsEqual)
            {
                double minProp = weightsPerCluster.Min();
                for (int i = 0; i < samplesPerCluster.Length; i++)
                {
                    samplesPerCluster[i] = System.Convert.ToInt32(samplesPerCluster[i] * (weightsPerCluster[i] / minProp));
                    weightsPerCluster[i] = 1;
                }
            }
            int[] tsPerCluster = new int[propPerCluster.Length];
            double[] randomRatioPerClust = new double[propPerCluster.Length];
            if (samplesPerCluster.Length != unqVls.Count)
            {
                System.Windows.Forms.MessageBox.Show("Unique Values in cluster field do not match the number of cluster models!");
                return;
            }
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger,false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble,false);
            IQueryFilter qf0 = new QueryFilterClass();
            qf0.SubFields = clusterFieldName;
            string h = "";
            IField fld = inputTable.Fields.get_Field(inputTable.FindField(clusterFieldName));
            if (fld.Type == esriFieldType.esriFieldTypeString) h = "'";
            for (int i = 0; i < samplesPerCluster.Length; i++)
            {

                qf0.WhereClause = clusterFieldName + " = " + h+labels[i]+h;
                int tCnt = inputTable.RowCount(qf0);
                tsPerCluster[i] = tCnt;
                randomRatioPerClust[i] = System.Convert.ToDouble(samplesPerCluster[i]) / tCnt;
            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = clusterFieldName + "," + sampleFldName + "," + weightFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(clusterFieldName);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = rw.get_Value(cIndex).ToString();
                    int clust = labels.IndexOf(clustStr);
                    double w = weightsPerCluster[clust];
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = randomRatioPerClust[clust];
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, w);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            
        }

        public void selectAccuracyFeaturesToSample(ITable inputTable, string AccuracyAssessmentModelPath, string mapField, double proportionOfMean, double alpha, bool weightsEqual=false)
        {
            esriUtil.Statistics.dataGeneralConfusionMatirx dGc = new Statistics.dataGeneralConfusionMatirx();
            dGc.getXTable(AccuracyAssessmentModelPath);
            List<string> labels = dGc.Labels.ToList();
            int samplesPerClass = esriUtil.Statistics.dataPrepSampleSize.sampleSizeKappa(AccuracyAssessmentModelPath, proportionOfMean, alpha)/labels.Count + 1;
            selectEqualFeaturesToSample(inputTable, mapField, samplesPerClass, weightsEqual);
        }

        public void selectEqualFeaturesToSample(ITable inputTable, string mapField, int SamplesPerClass, bool weightsEqual=false )
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            HashSet<string> unqVls = geoUtil.getUniqueValues(inputTable, mapField);
            System.Random rd = new Random();
            int samplesPerClass = SamplesPerClass;
            double tSamples = System.Convert.ToDouble(samplesPerClass * unqVls.Count);
            double gR = samplesPerClass / tSamples;
            double[] weightsPerClass = new double[unqVls.Count];
            int[] tsPerClass = new int[unqVls.Count];
            double[] ratioPerClass = new double[unqVls.Count];
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble, false);
            IQueryFilter qf0 = new QueryFilterClass();
            qf0.SubFields = mapField;
            string h = "";
            IField fld = inputTable.Fields.get_Field(inputTable.FindField(mapField));
            if (fld.Type == esriFieldType.esriFieldTypeString) h = "'";
            for (int i = 0; i < unqVls.Count; i++)
            {

                qf0.WhereClause = mapField + " = " + h + unqVls.ElementAt(i) + h;
                int tCnt = inputTable.RowCount(qf0);
                tsPerClass[i] = tCnt;
                ratioPerClass[i] = System.Convert.ToDouble(samplesPerClass) / tCnt;
            }
            double tsSamp = System.Convert.ToDouble(tsPerClass.Sum());
            for (int i = 0; i < weightsPerClass.Length; i++)
            {
                weightsPerClass[i] = (tsPerClass[i]/tsSamp) / (gR);
            }
            if (weightsEqual)
            {
                double minW = weightsPerClass.Min();
                for (int i = 0; i < weightsPerClass.Length; i++)
                {
                    double aSamp = samplesPerClass*(weightsPerClass[i]/minW);
                    ratioPerClass[i] = aSamp / tsPerClass[i];
                    weightsPerClass[i] = 1;
                }

            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = mapField + "," + sampleFldName + "," + weightFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(mapField);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                List<string> unqLst = unqVls.ToList();
                while (rw != null)
                {
                    string classStr = rw.get_Value(cIndex).ToString();
                    int cls = unqLst.IndexOf(classStr);
                    double w = weightsPerClass[cls];
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = ratioPerClass[cls];
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, w);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public void selectKSFeaturesToSample(ITable sampledTable,ITable samplesToDrawFromTable, string ksModelPath, string groupFieldName = "")
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)sampledTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Sampled Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            if (samplesToDrawFromTable != null)
            {
                objInfo2 = (IObjectClassInfo2)samplesToDrawFromTable;
                if (!objInfo2.CanBypassEditSession())
                {
                    System.Windows.Forms.MessageBox.Show("Samples to draw from table participates in a composite relationship. Please export this table as a new table and try again!");
                    return;
                }
            }
            if (groupFieldName == null) groupFieldName = "";
            try
            {
                esriUtil.Statistics.dataPrepCompareSamples dpComp = new Statistics.dataPrepCompareSamples(ksModelPath);
                Dictionary<string,object[]> sampledBinPropDic = calcBinValues(dpComp, sampledTable); //key = stratfield_bin, values = [0] ratios {10} for random selection [1] counts {10} from sample
                //bins and ratios calculated next use ratios to select from class and bin

                IWorkspace wks = ((IDataset)sampledTable).Workspace;
                IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
                if (wksE.IsBeingEdited())
                {
                    wksE.StopEditing(true);
                }
           
                System.Random rd = new Random();
                string sampleFldName = geoUtil.createField(sampledTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
                List<string> labels = dpComp.Labels.ToList();
            
                ICursor cur = sampledTable.Update(null, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(groupFieldName);
                int bIndex = cur.FindField("BIN");
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = labels[0];
                    if (cIndex > -1)
                    {
                        clustStr = rw.get_Value(cIndex).ToString();
                    }
                    int b = System.Convert.ToInt32(rw.get_Value(bIndex));
                    double rNum = rd.NextDouble();
                    double r = ((double[])sampledBinPropDic[clustStr][0])[b];

                    int ss = 0;
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
                if (samplesToDrawFromTable != null)
                {
                    appendSamples(sampledTable, samplesToDrawFromTable, sampledBinPropDic,dpComp);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        private void appendSamples(ITable sampledTable, ITable samplesToDrawFromTable, Dictionary<string, object[]> sampledBinPropDic,Statistics.dataPrepCompareSamples dpComp)
        {
            Dictionary<string,object[]> bigSampleDic = calcBinValues(dpComp,samplesToDrawFromTable);
            Dictionary<string, double[]> ratioDic = new Dictionary<string, double[]>();
            bool check = false;
            foreach (KeyValuePair<string, object[]> kvp in sampledBinPropDic)
            {
                string ky = kvp.Key;
                
                int[] cntArr = (int[])kvp.Value[1];
                int totalCnt = cntArr.Sum();
                //double[] ratioArr = (double[])kvp.Value[0];
                int[] cntArr2 = (int[])bigSampleDic[ky][1];
                double[] ratioArr2 = dpComp.binPropDic1[ky][0];// (double[])bigSampleDic[ky][0];
                double[] nrArr = new double[ratioArr2.Length];
                for (int i = 0; i < cntArr.Length; i++)
                {
                    double nr = 0;
                    double r = ratioArr2[i];
                    int tCntS = cntArr[i];
                    int sN = (int)((totalCnt*r) - tCntS);
                    if (sN > 0)
                    {
                        check = true;
                        nr = System.Convert.ToDouble(sN) / cntArr2[i];
                    }
                    nrArr[i] = nr;
                }
                ratioDic.Add(ky, nrArr);
            }
            if (!check) return;
            Random rd = new Random();
            string[] labels = dpComp.Labels;
            List<int> bFldsIndex = new List<int>();
            List<int> sFldsIndex = new List<int>();
            for (int i = 0; i < sampledTable.Fields.FieldCount; i++)
            {
                IField sfld = sampledTable.Fields.get_Field(i);
                string sfldName = sfld.Name;
                int bfldIndex = samplesToDrawFromTable.FindField(sfldName);
                if (bfldIndex > -1 && sfld.Editable)
                {
                    bFldsIndex.Add(bfldIndex);
                    sFldsIndex.Add(i);
                }
            }
            ICursor cur = samplesToDrawFromTable.Search(null, false);
            int cIndex = cur.FindField(dpComp.StrataField);
            int bIndex = cur.FindField("BIN");
            IRow rw = cur.NextRow();
            int sIndex = sampledTable.FindField("Sample");
            int wIndex = sampledTable.FindField("Weight");
            while (rw != null)
            {
                string clustStr = labels[0];
                if (cIndex > -1)
                {
                    clustStr = rw.get_Value(cIndex).ToString();
                }
                int b = System.Convert.ToInt32(rw.get_Value(bIndex));
                double rNum = rd.NextDouble();
                double r = ratioDic[clustStr][b];
                if (rNum < r)
                {
                    IRow srw = sampledTable.CreateRow();
                    for (int i = 0; i < sFldsIndex.Count; i++)
                    {
                        try
                        {
                            srw.set_Value(sFldsIndex[i], rw.get_Value(bFldsIndex[i]));
                        }
                        catch
                        {
                            
                        }
                    }
                    if (sIndex > -1) srw.set_Value(sIndex, 1);
                    if (wIndex > -1) srw.set_Value(wIndex, sampledBinPropDic[clustStr][2]);
                    srw.Store();
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private Dictionary<string, object[]> calcBinValues(Statistics.dataPrepCompareSamples dpComp, ITable inTable)
        {
            Dictionary<string, object[]> outDic = new Dictionary<string, object[]>();//{double[10],int[10],double} ratios, counts, weight
            Dictionary<string, double> minDic = new Dictionary<string, double>();//the min value of each bin
            Dictionary<string, double> spanDic = new Dictionary<string, double>();//the span of each bin
            Dictionary<string, int> cntDic = new Dictionary<string,int>();//counts by class
            Statistics.dataPrepPrincipleComponents pca = dpComp.PCA;
            string[] labels = dpComp.Labels;
            for (int i = 0; i < labels.Length; i++)
            {
                string lbl = labels[i];
                double[][] minmax1 = dpComp.minMaxDic1[lbl];
                double[][] minmax2 = dpComp.minMaxDic2[lbl];
                double min = minmax1[0][0];
                if (minmax2[0][0] < min) min = minmax2[0][0];
                double max = minmax1[1][0];
                if (minmax2[1][0] > max) max = minmax2[1][0];
                double span = (max - min) / 10;
                minDic.Add(lbl, min);
                spanDic.Add(lbl, span);
                cntDic.Add(lbl,0);
                double[] ratios = new double[10];
                int[] cnts = new int[10];
                object[] outObjectValues = new object[3];
                outObjectValues[0] = ratios;
                outObjectValues[1] = cnts;
                outObjectValues[2] = 1;
                outDic.Add(lbl, outObjectValues);
            }
            int[] vArrIndex = new int[dpComp.Variables.Length];
            for (int i = 0; i < vArrIndex.Length; i++)
            {
                vArrIndex[i]=inTable.FindField(dpComp.Variables[i]);
            }
            string binFldName = geoUtil.createField(inTable, "BIN", esriFieldType.esriFieldTypeInteger, false);
            string strataFldName = dpComp.StrataField;
            string weightFldName = "Weight";
            int binFldNameIndex = inTable.FindField(binFldName);
            int strataFldNameIndex = inTable.FindField(strataFldName);
            int weightFldNameINdex = inTable.FindField(weightFldName);
            ICursor cur = inTable.Update(null, false);
            IRow rw = cur.NextRow();
            double[] varr = new double[vArrIndex.Length];
            int totalCnt = 0;
            while (rw != null)
            {
                bool check = true;
                for (int i = 0; i < varr.Length; i++)
			    {
                    object vlObj = rw.get_Value(vArrIndex[i]);
                    if(vlObj==null)
                    {
                        check = false;
                        break;
                    }
                    varr[i]=System.Convert.ToDouble(vlObj);
			    }
                if(check)
                {
                    double vl = dpComp.PCA.computNew(varr)[0];
                    double min;
                    double span;
                    string g = labels[0];
                    double w = 1;
                    if(strataFldNameIndex>-1)
                    {
                        g = rw.get_Value(strataFldNameIndex).ToString();
                    }
                    if (weightFldNameINdex > -1)
                    {
                        w = System.Convert.ToDouble(rw.get_Value(weightFldNameINdex));
                    }
                    object[] oOut = outDic[g];
                    int[] cntArr = (int[])oOut[1];
                    oOut[2] = w;
                    min = minDic[g];
                    span = spanDic[g];
                    //weightDic[g] = w;
                    cntDic[g] += 1;
                    int b = (int)((vl-min)/span);
                    if (b >= cntArr.Length) b = cntArr.Length - 1;
                    if (b < 0) b = 0;
                    //Console.WriteLine("\nValue = " + vl.ToString() + "\nmin = " + min.ToString() + "\nSpan = " + span.ToString() + "\nbin = " + b.ToString());
                    cntArr[b] = cntArr[b]+1;
                    rw.set_Value(binFldNameIndex,b);
                    cur.UpdateRow(rw);
                    totalCnt+=1;
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            foreach(KeyValuePair<string,object[]> kvp in outDic)
            {
                string ky = kvp.Key;
                int gCnt = cntDic[ky];
                double[] prop = dpComp.binPropDic1[ky][0];
                object[] outObj = kvp.Value;
                int[] cntArr = (int[])outObj[1];
                double[] rArr = (double[])outObj[0];
                for (int i = 0; i < cntArr.Length; i++)
			    {
                    double p = System.Convert.ToDouble(cntArr[i])/gCnt;
                    double po = prop[i];
                    rArr[i] = po/p;
			    }
            }

            return outDic;
        }

        public void selectCovCorrFeaturesToSample(ITable inputTable, string covCorrModelPath, double proptionOfMean=0.1, double alpha = 0.05)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            Statistics.dataPrepVarCovCorr covCor = new Statistics.dataPrepVarCovCorr();
            covCor.buildModel(covCorrModelPath);
            System.Random rd = new Random();
            double tSamples = System.Convert.ToDouble(esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxMean(covCor.MeanVector,covCor.StdVector,proptionOfMean,alpha));
            int tRecords = inputTable.RowCount(null);
            double gR = tSamples / tRecords;
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            IQueryFilter qf0 = new QueryFilterClass();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = sampleFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = gR;
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public void selectPcaFeaturesToSample(ITable inputTable, string pcaModelPath, double proportionOfMean, double alpha)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            Statistics.dataPrepPrincipleComponents pca = new Statistics.dataPrepPrincipleComponents();
            pca.buildModel(pcaModelPath);
            System.Random rd = new Random();
            double tSamples = System.Convert.ToDouble(esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxMean(pca.MeanVector, pca.StdVector, proportionOfMean, alpha));
            int tRecords = inputTable.RowCount(null);
            double gR = tSamples / tRecords;
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            IQueryFilter qf0 = new QueryFilterClass();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = sampleFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = gR;
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public IFeatureClass exportFeatures(IFeatureClass inputFeatureClass, string outPath, ISpatialFilter filter)
        {
            
            // Create a name object for the source (shapefile) workspace and open it.
            IDataset inDset = (IDataset)inputFeatureClass;
            IWorkspace sourceWorkspace = (inDset).Workspace;

            // Create a name object for the target (file GDB) workspace and open it.
            string outDbStr = geoUtil.parseDbStr(outPath);
            string outName = System.IO.Path.GetFileName(outPath);
            IWorkspace targetWorkspace = geoUtil.OpenWorkSpace(outDbStr);
            outName = geoUtil.getSafeOutputNameNonRaster(targetWorkspace, outName);
            
            // Create a name object for the source dataset.
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.Name = inDset.Name;
            sourceDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)sourceWorkspace).FullName;

            // Create a name object for the target dataset.
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.Name = outName;
            targetDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)targetWorkspace).FullName; ;

            // Open source feature class to get field definitions.
            //IName sourceName = (IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = inputFeatureClass;

            // Create the objects and references necessary for field validation.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields sourceFields = sourceFeatureClass.Fields;
            IFields targetFields = null;
            IEnumFieldError enumFieldError = null;

            // Set the required properties for the IFieldChecker interface.
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;

            // Validate the fields and check for errors.
            fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
            if (enumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }

            // Find the shape field.
            String shapeFieldName = sourceFeatureClass.ShapeFieldName;
            int shapeFieldIndex = sourceFeatureClass.FindField(shapeFieldName);
            IField shapeField = sourceFields.get_Field(shapeFieldIndex);

            // Get the geometry definition from the shape field and clone it.
            IGeometryDef geometryDef = shapeField.GeometryDef;
            IClone geometryDefClone = (IClone)geometryDef;
            IClone targetGeometryDefClone = geometryDefClone.Clone();
            IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;

            // Cast the IGeometryDef to the IGeometryDefEdit interface.
            IGeometryDefEdit targetGeometryDefEdit = (IGeometryDefEdit)targetGeometryDef;

            // Set the IGeometryDefEdit properties.
            targetGeometryDefEdit.GridCount_2 = 1;
            targetGeometryDefEdit.set_GridSize(0, 0.75);

            // Create the converter and run the conversion.
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertFeatureClass(sourceFeatureClassName, filter, null, targetFeatureClassName, targetGeometryDef, targetFields, "", 1000, 0);

            // Check for errors.
            IInvalidObjectInfo invalidObjectInfo = null;
            enumInvalidObject.Reset();
            while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}", invalidObjectInfo.InvalidObjectID);
            }
            return (IFeatureClass)((IName)targetFeatureClassName).Open();

        }
        public ITable exportTable(ITable inputTable, string outPath, IQueryFilter filter)
        {
            // Create a name object for the source workspace and open it.
            IDataset inDset = (IDataset)inputTable;
            IWorkspace sourceWorkspace = (inDset).Workspace;

            // Create a name object for the target (file GDB) workspace and open it.
            string outDbStr = geoUtil.parseDbStr(outPath);
            string outName = System.IO.Path.GetFileName(outPath);
            IWorkspace targetWorkspace = geoUtil.OpenWorkSpace(outDbStr);
            outName = geoUtil.getSafeOutputNameNonRaster(targetWorkspace, outName);

            // Create a name object for the source dataset.
            ITableName sourceTableName = new TableNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceTableName;
            sourceDatasetName.Name = inDset.Name;
            sourceDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)sourceWorkspace).FullName;

            // Create a name object for the target dataset.
            ITableName targetTableName = new TableNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetTableName;
            targetDatasetName.Name = outName;
            targetDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)targetWorkspace).FullName; ;

            // Open source feature class to get field definitions.
            //IName sourceName = (IName)sourceFeatureClassName;
            ITable sourceTable = inputTable;

            // Create the objects and references necessary for field validation.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields sourceFields = sourceTable.Fields;
            IFields targetFields = null;
            IEnumFieldError enumFieldError = null;

            // Set the required properties for the IFieldChecker interface.
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;

            // Validate the fields and check for errors.
            fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
            if (enumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }

            // Create the converter and run the conversion.
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertTable(sourceDatasetName, filter, targetDatasetName, targetFields,"",1000,0);

            // Check for errors.
            IInvalidObjectInfo invalidObjectInfo = null;
            enumInvalidObject.Reset();
            while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}", invalidObjectInfo.InvalidObjectID);
            }
            return (Table)((IName)targetTableName).Open();
        }
    }
}
