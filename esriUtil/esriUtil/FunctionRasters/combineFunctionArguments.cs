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
                 IFunctionRasterDataset rs = rsUtil.getBand(inrs[0], 0);
                 otrs = rs;
             } 
         }
         private IFunctionRasterDataset otrs = null;
         public IFunctionRasterDataset InRasterDataset
         {
             get
             {
                 return rsUtil.compositeBandFunction(inrs);
             }
         }
         public IFunctionRasterDataset OutRaster { get { return otrs; } }

     }
}

