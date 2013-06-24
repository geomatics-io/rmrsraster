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
    public class glmFunctionArguments
    {
        public glmFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public glmFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRasterCoefficients
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
        public Statistics.dataPrepGlm GlmModel { get; set; }
        public IRaster OutRaster
        {
            get
            {

                IRaster rs = rsUtil.getBand(inrs, 0);
                IRaster rsC = rsUtil.constantRasterFunction(rs, 0);
                return (IRaster)rsC;
            }
        }

    }
}