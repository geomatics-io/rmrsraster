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
    class pcaArguments
    {
        public pcaArguments()
        {
            rsUtil = new rasterUtil();
        }
        public pcaArguments(rasterUtil rasterUtility)
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
        private Statistics.dataPrepPrincipleComponents pca = null;
        public Statistics.dataPrepPrincipleComponents PCA
        {
            get
            {
                return pca;
            }
            set
            {
                pca = value;
            }
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < pca.VariableFieldNames.Length; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                return rsUtil.compositeBandFunction(rsBc);
            }
        }
    }
}