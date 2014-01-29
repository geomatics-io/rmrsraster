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
    class regionGroupFunctionArguments
    {
        public regionGroupFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public regionGroupFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster InRaster 
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
        
        public IRaster OutRaster
        {
            get
            {
                IRaster ts = rsUtil.getBand(inrs, 0);
                IRaster rs = rsUtil.constantRasterFunction(ts, 0);
                return rs;

            }
        }
    }
}