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
        private ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset inrs = null;
        public ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset InRaster 
        { 
            get 
            {
                IFunctionRasterDataset rsDset = rsUtil.createIdentityRaster(origRs, rstPixelType.PT_FLOAT);
                inrs = rsUtil.reSizeRasterCellsFunction(rsDset, cells);
                return inrs;
            } 
            set 
            {
                origRs = value;
            } 
        }
        
        private int cells = 3;
        public int Cells { get { return cells; } set { cells = value;} }
        private ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset origRs = null;
        public ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset OriginalRaster 
        { 
            get 
            {
                return origRs;
            } 
        }
    }
}

