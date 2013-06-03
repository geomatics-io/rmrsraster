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
    class combineFunctionArguments
    {
         public combineFunctionArguments()
         {
             rsUtil = new rasterUtil();
         }
         public combineFunctionArguments(rasterUtil rasterUtility)
         {
             rsUtil = rasterUtility;
         }
         private IRaster[] inrs = null;
         private rasterUtil rsUtil = null;
         public ESRI.ArcGIS.Geodatabase.IRaster[] InRaster 
         { 
             get 
             {
                 return inrs; 
             } 
             set 
             {
                 inrs = value;
                 IRaster  tRs = rsUtil.returnRaster(inrs[0],rstPixelType.PT_FLOAT);
                 IRaster rs = rsUtil.getBand(tRs, 0);
                 otrs = rsUtil.constantRasterFunction(rs, 0);
             } 
         }
         private IRaster otrs = null;
         public ESRI.ArcGIS.Geodatabase.IRaster outRaster { get { return otrs; }}
     }
}

