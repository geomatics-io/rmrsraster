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
     public class conditionalFunctionArguments 
     {
         public conditionalFunctionArguments()
         {
             rsUtil = new rasterUtil();
         }
         public conditionalFunctionArguments(rasterUtil rasterUtilitiy)
         {
             rsUtil = rasterUtilitiy;
         }
         private rasterUtil rsUtil = null;
         private IRaster conRs = null;
         public IRaster ConditionalRaster 
         {
             get
             {
                 return conRs;
             }
             set
             {
                 IRaster temp = value;
                 IRasterProps rsProps = (IRasterProps)temp;
                 if (rsProps.PixelType != rstPixelType.PT_DOUBLE)
                 {
                     temp = rsUtil.convertToDifFormatFunction(temp, rstPixelType.PT_DOUBLE);
                 }
                 conRs = temp;
             }
         }
         public IRaster TrueRaster { get; set; }
         public IRaster FalseRaster { get; set; }
     }
}

