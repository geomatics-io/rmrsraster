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
    class ExtractModelRangeFunctionArguments
    {
        public ExtractModelRangeFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public ExtractModelRangeFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public ESRI.ArcGIS.DataSourcesRaster.IFunctionRasterDataset InRaster
        {
            get
            {
                return inrs;
            }
            set
            {
                inrs = rsUtil.createIdentityRaster(value, rstPixelType.PT_FLOAT);
            }
        }
        private double[] maxs = null;
        public double[] Maxs
        {
            get
            {
                return maxs;
            }
            set
            {
                maxs = value;
            }
        }
        private double[] mins = null;
        public double[] Mins
        {
            get
            {
                return mins;
            }
            set
            {
                mins = value;
            }
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset f1 = rsUtil.getBand(inrs, 0);
                IFunctionRasterDataset f2 = rsUtil.constantRasterFunction(f1,0,rstPixelType.PT_UCHAR);
                return f2;
            }
        }
    }
}
