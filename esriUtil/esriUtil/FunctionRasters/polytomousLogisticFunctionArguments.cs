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
    class polytomousLogisticFunctionArguments
    {
        public polytomousLogisticFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public polytomousLogisticFunctionArguments(rasterUtil rasterUtility)
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
        private double[][] slopes = null;//dictionary of classes and float array = intercept followed by betas
        public double[][] Slopes 
        { 
            get 
            { 
                return slopes; 
            } 
            set 
            { 
                slopes = value;
            } 
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                
                IFunctionRasterDataset rs = rsUtil.getBand(inrs,0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < slopes.Length+1; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                
                return rsUtil.compositeBandFunction(rsBc);

            }
        }
     }
}