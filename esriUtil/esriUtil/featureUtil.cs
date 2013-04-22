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
    }
}
