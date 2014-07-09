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
    class softMaxFunctionArguments
    {
        public softMaxFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public softMaxFunctionArguments(rasterUtil rasterUtility)
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
        private Statistics.dataPrepSoftMaxPlr lm = null;
        public Statistics.dataPrepSoftMaxPlr LogitModel
        {
            get
            {
                return lm;
            }
            set
            {
                lm = value;
            }
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < lm.NumberOfClasses; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                return rsUtil.compositeBandFunction(rsBc);

            }
        }
    }
}
