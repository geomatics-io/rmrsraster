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
    public class dataPrepClusterKmean : dataPrepClusterBase
    {
        public dataPrepClusterKmean()
        {
        }
        public dataPrepClusterKmean(IRaster raster, int numberOfClasses)
        {
            InRaster = raster;
            IRasterBandCollection bc = (IRasterBandCollection)InRaster;
            int bcCnt = bc.Count;
            VariableFieldNames = new string[bcCnt];
            for (int i = 0; i < bcCnt; i++)
            {
                VariableFieldNames[i] = "band_" + (i + 1).ToString();
            }
            K = numberOfClasses;
            cType = esriUtil.Statistics.clusterType.KMEANS;
            buildModel();
        }
        public dataPrepClusterKmean(ITable table, string[] variables, int numberOfClasses)
        {
            InTable = table;
            VariableFieldNames = variables;
            K = numberOfClasses;
            cType = esriUtil.Statistics.clusterType.KMEANS;
            buildModel();
        }
        public override int computNew(double[] input)
        {
            return ((KMeans)Model).Clusters.Nearest(input);
        }
    }
}