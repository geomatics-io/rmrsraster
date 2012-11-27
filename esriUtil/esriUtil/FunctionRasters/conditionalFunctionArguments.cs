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
                 if (rsProps.PixelType != rstPixelType.PT_FLOAT)
                 {
                     temp = rsUtil.convertToDifFormatFunction(temp, rstPixelType.PT_FLOAT);
                 }
                 conRs = temp;
             }
         }
         private IRaster trs = null;
         public IRaster TrueRaster
         {
             get
             {
                 return trs;
             }
             set
             {
                 IRaster temp = value;
                 IRasterProps rsProps = (IRasterProps)temp;
                 if (rsProps.PixelType != rstPixelType.PT_FLOAT)
                 {
                     temp = rsUtil.convertToDifFormatFunction(temp, rstPixelType.PT_FLOAT);
                 }
                 trs = temp;
             }
         }
         private IRaster frs = null;
         public IRaster FalseRaster
         {
             get
             {
                 return frs;
             }
             set
             {
                 IRaster temp = value;
                 IRasterProps rsProps = (IRasterProps)temp;
                 if (rsProps.PixelType != rstPixelType.PT_FLOAT)
                 {
                     temp = rsUtil.convertToDifFormatFunction(temp, rstPixelType.PT_FLOAT);
                 }
                 frs = temp;
             }
         }
     }
}

