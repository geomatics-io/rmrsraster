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
    class regressionFunctionArguments
    {
        public regressionFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public regressionFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset InRasterCoefficients 
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
        private List<float[]> slopes = new List<float[]>();//float array = intercept followed by betas
        public List<float[]> Slopes 
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

                IRasterBandCollection rsbColl = new RasterClass();
                for (int i = 0; i < slopes.Count; i++)
                {
                    rsbColl.AppendBand(((IRasterBandCollection)rsUtil.getBand(inrs, 0)).Item(0));
                }
                
                return rsUtil.compositeBandFunction(rsbColl);

            }
        }
     }
}