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
    class MathFunctionArguments
    {
        public MathFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public MathFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster InRaster
        {
            get
            {
                return inrs;
            }
            set
            {
                IRaster temp = value;
                IRasterProps rsProps = (IRasterProps)temp;
                if (rsProps.PixelType != rstPixelType.PT_DOUBLE)
                {
                    temp = rsUtil.convertToDifFormatFunction(temp, rstPixelType.PT_DOUBLE);
                }
                inrs = temp;
            }
        }
    }
}
