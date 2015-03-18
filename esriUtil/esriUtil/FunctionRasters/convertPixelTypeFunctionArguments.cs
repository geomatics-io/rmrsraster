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
    public class convertPixelTypeFunctionArguments
    {
        public convertPixelTypeFunctionArguments()
         {
             rsUtil = new rasterUtil();
         }
        public convertPixelTypeFunctionArguments(rasterUtil rasterUtility)
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
                 //outrs = rsUtil.createIdentityRaster(value);
             } 
         }
         //private IFunctionRasterDataset outrs = null;
         //public IFunctionRasterDataset OutRaster{ get { outrs.Function.PixelType = rspixeltype; return outrs; } }
         private rstPixelType rspixeltype = rstPixelType.PT_UNKNOWN;
         public rstPixelType RasterPixelType { get { return rspixeltype; } set { rspixeltype = value; } }
     }
}