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
         private IFunctionRasterDataset conRs = null;
         public IFunctionRasterDataset CoefRaster
         {
             get
             {
                 IRasterBandCollection rsBc = new RasterClass();
                 rsBc.AppendBands((IRasterBandCollection)rsUtil.getBand(conRs, 0));
                 rsBc.AppendBands((IRasterBandCollection)rsUtil.getBand(trs, 0));
                 rsBc.AppendBands((IRasterBandCollection)rsUtil.getBand(frs, 0));
                 return rsUtil.compositeBandFunction(rsBc);
             }
         }
         public IFunctionRasterDataset ConditionalRaster 
         {
             get
             {
                 return conRs;
             }
             set
             {
                 conRs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
             }
         }
         private IFunctionRasterDataset trs = null;
         public IFunctionRasterDataset TrueRaster
         {
             get
             {
                 return trs;
             }
             set
             {
                 trs = rsUtil.createIdentityRaster(value, rstPixelType.PT_FLOAT);
             }
         }
         private IFunctionRasterDataset frs = null;
         public IFunctionRasterDataset FalseRaster
         {
             get
             {
                 return frs;
             }
             set
             {
                 
                 frs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
             }
         }
         public IFunctionRasterDataset OutRaster
         {
             get
             {
                 return rsUtil.getBand(conRs, 0);
             }
         }
     }
}

