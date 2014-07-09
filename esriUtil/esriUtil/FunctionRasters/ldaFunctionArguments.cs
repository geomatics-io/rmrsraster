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
    public class ldaFunctionArguments
    {
        public ldaFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public ldaFunctionArguments(rasterUtil rasterUtility)
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
        public Statistics.dataPrepDiscriminantAnalysisLda LDAModel { get; set; }
        public IFunctionRasterDataset OutRaster
        {
            get
            {

                IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                return rs;
            }
        }

    }
}