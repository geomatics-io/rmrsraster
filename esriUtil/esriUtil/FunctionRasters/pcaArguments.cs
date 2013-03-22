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
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = rsUtil.getBand(inrs, 0);
                rs = rsUtil.constantRasterFunction(rs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < pca.VariableFieldNames.Length; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                return (IRaster)rsBc;
            }
        }
    }
}