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
     public class LocalFunctionArguments 
     {
         public LocalFunctionArguments()
         {
             rsUtil = new rasterUtil();
         }
         public LocalFunctionArguments(rasterUtil rasterUtility)
         {
             rsUtil = rasterUtility;
         }
         private IFunctionRasterDataset inrs = null;
         private rasterUtil rsUtil = null;
         public IFunctionRasterDataset InRaster 
         { 
             get 
             { 
                 return inrs; 
             } 
             set 
             {

                 inrs = value;
                 IFunctionRasterDataset tr = rsUtil.createIdentityRaster(inrs, rstPixelType.PT_FLOAT);
                 otrs = rsUtil.getBand(tr, 0);
             } 
         }
         private IFunctionRasterDataset otrs = null;
         public IFunctionRasterDataset outRaster { get { return otrs; }}
     }
}

