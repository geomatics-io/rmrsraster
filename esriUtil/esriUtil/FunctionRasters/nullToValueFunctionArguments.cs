using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil.FunctionRasters.NeighborhoodHelper;

namespace esriUtil.FunctionRasters
{
    class nullToValueFunctionArguments
    {
        public nullToValueFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public nullToValueFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster 
        { 
            get 
            {
                return inrs;
            } 
            set 
            {
                inrs = rsUtil.returnRaster(value);
            } 
        }
        private double newvalue = 0d;
        public double NewValue { get { return newvalue; } set { newvalue = value;} }
        public System.Array NoDataArray { get { return (System.Array)((IRasterProps)inrs).NoDataValue; } }
    }
}

