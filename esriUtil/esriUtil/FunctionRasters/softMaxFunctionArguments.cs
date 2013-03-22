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
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = rsUtil.getBand(inrs, 0);
                rs = rsUtil.constantRasterFunction(rs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < lm.NumberOfClasses; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                return (IRaster)rsBc;

            }
        }
    }
}
