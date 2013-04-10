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
        public void selectStratifiedFeaturesToSample(ITable inputTable, string clusterModelPath, string clusterFieldName="Cluster", double proportionOfMean=0.1, double alpha=0.05)
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
            int[] tsPerCluster = new int[propPerCluster.Length];
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
                tsPerCluster[i] = inputTable.RowCount(qf0);
            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = clusterFieldName + "," + sampleFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            //ITransactions trs = (ITransactions)wks;
            //trs.StartTransaction();
            try
            {
                int[] selectedSamples = new int[samplesPerCluster.Length];
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(clusterFieldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = rw.get_Value(cIndex).ToString();
                    int clust = labels.IndexOf(clustStr);
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = System.Convert.ToDouble(samplesPerCluster[clust]) / tsPerCluster[clust];
                    if (rNum < r)
                    {
                        ss = 1;
                        selectedSamples[clust] = selectedSamples[clust] + 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
                int selectedSamplesTotal = selectedSamples.Sum();
                for (int i = 0; i < selectedSamples.Length; i++)
                {
                    int ss = selectedSamples[i];
                    double expectedS = selectedSamplesTotal * propPerCluster[i];
                    weightsPerCluster[i] = expectedS / ss;
                }
                //System.Windows.Forms.MessageBox.Show("Weights = " + String.Join(", ",(from double d in weightsPerCluster select d.ToString()).ToArray()));
                qf = new QueryFilterClass();
                qf.SubFields = clusterFieldName + "," + weightFldName;
                ICursor cur2 = inputTable.Update(qf, false);
                IRow rw2 = cur2.NextRow();
                cIndex = cur2.FindField(clusterFieldName);
                int wIndex = cur2.FindField(weightFldName);
                while (rw2 != null)
                {
                    string clsStr = rw2.get_Value(cIndex).ToString();
                    int cls = labels.IndexOf(clsStr);
                    rw2.set_Value(wIndex, weightsPerCluster[cls]);
                    cur2.UpdateRow(rw2);
                    rw2 = cur2.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur2);
                //trs.CommitTransaction();
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                //trs.AbortTransaction();
            }
            
        }
    }
}
