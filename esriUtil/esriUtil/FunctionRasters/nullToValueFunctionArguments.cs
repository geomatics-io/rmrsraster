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
                inrs = rsUtil.returnRaster(value, rstPixelType.PT_DOUBLE);
                IRasterProps rsP = (IRasterProps)inrs;
                nodataArr = (System.Array)((System.Array)rsP.NoDataValue).Clone();
                System.Array neNullData = (System.Array)rsP.NoDataValue;
                for(int i = 0;i<neNullData.Length;i++)
                {
                    double oldNodataValue = System.Convert.ToDouble(neNullData.GetValue(i));

                    neNullData.SetValue(oldNodataValue + 1, i);
                }
                rsP.NoDataValue = neNullData;
            } 
        }
        
        private double newvalue = 0;
        private System.Array nodataArr = null;
        public double NewValue { get { return newvalue; } set { newvalue = value;} }
        public System.Array NoDataArray { get { return nodataArr; } }
    }
}

