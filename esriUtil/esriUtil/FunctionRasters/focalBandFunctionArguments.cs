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
    public class focalBandFunctionArguments
    {
        public focalBandFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public focalBandFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public IFunctionRasterDataset InRaster 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {

                inrs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
            } 
        }

        private int bb = 1;
        private int ba = 1;
        public int BandsBefore { get { return bb; } set { bb = value; } }

        public int BandsAfter { get { return ba; } set { ba = value; } }
    }
}
