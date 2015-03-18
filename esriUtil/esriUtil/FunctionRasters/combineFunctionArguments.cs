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
         private IFunctionRasterDataset inrs = null;
         private rasterUtil rsUtil = null;
         public IFunctionRasterDataset InRasterDataset 
         { 
             get 
             {
                 return inrs; 
             } 
             set 
             {
                 inrs = value;
                 IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                 IFunctionRasterDataset rs2 = rsUtil.createIdentityRaster(rs, rstPixelType.PT_ULONG);
                 otrs = rs2;
             } 
         }
         private IFunctionRasterDataset otrs = null;
         public IFunctionRasterDataset OutRaster { get { return otrs; } }

     }
}

