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
    class aggregationFunctionArguments
    {
        public aggregationFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public aggregationFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster 
        { 
            get 
            {
                return rsUtil.reSizeRasterCellsFunction(inrs,cells);
            } 
            set 
            {
                IRaster temp = value;
                inrs = rsUtil.constantRasterFunction(temp,0);
                origRs = rsUtil.returnRaster(value,rstPixelType.PT_FLOAT);
            } 
        }
        
        private int cells = 3;
        public int Cells { get { return cells; } set { cells = value;} }
        private ESRI.ArcGIS.Geodatabase.IRaster origRs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OriginalRaster 
        { 
            get 
            {
                return origRs;
            } 
        }
    }
}

