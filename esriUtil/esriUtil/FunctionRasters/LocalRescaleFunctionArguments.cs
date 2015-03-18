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
    public class LocalRescaleFunctionArguments
    {
        public LocalRescaleFunctionArguments()
             {
                 rsUtil = new rasterUtil();
             }
        public LocalRescaleFunctionArguments(rasterUtil rasterUtility)
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

                     inrs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
                 } 
             }
             
     }
}