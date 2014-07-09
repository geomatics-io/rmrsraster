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
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public IFunctionRasterDataset InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                inrs = rsUtil.createIdentityRaster(value, rstPixelType.PT_FLOAT);
            } 
        }
        private object cluster = null;
        public object ClusterModel
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
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset rs =rsUtil.getBand(inrs, 0);
                return rs;
            }
        }
    }
}
