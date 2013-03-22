using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters
{
    class clusterFunctionArguments
    {
        public clusterFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public clusterFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                IRaster temp = value;
                inrs = rsUtil.returnRaster(temp, rstPixelType.PT_FLOAT);
            } 
        }
        private Statistics.dataPrepCluster cluster = null;
        public Statistics.dataPrepCluster ClusterModel
        {
            get
            {
                return cluster;
            }
            set
            {
                cluster = value;
            }
        }
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = rsUtil.getBand(inrs, 0);
                rs = rsUtil.constantRasterFunction(rs, 0);
                return rs;
            }
        }
    }
}
