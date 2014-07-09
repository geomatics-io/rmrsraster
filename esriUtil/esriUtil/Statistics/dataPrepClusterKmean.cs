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
            cType = clusterType.KMEANS;
        }
        public dataPrepClusterKmean(IRaster raster, int numberOfClasses, double maxRec = 10000000d, double prec = 0.0001)
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
            MaxRecords = maxRec;
            Precision = prec;
            buildModel();
        }
        public dataPrepClusterKmean(ITable table, string[] variables, int numberOfClasses, double maxRec = 10000000d, double prec = 0.0001)
        {
            InTable = table;
            VariableFieldNames = variables;
            K = numberOfClasses;
            cType = esriUtil.Statistics.clusterType.KMEANS;
            MaxRecords = maxRec;
            Precision = prec;
            buildModel();
        }
        public override int computNew(double[] input)
        {
            return ((KMeans)Model).Clusters.Nearest(input);
        }
    }
}