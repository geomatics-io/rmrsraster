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
    class meanShiftFunctionArguments
    {
        public meanShiftFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public meanShiftFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public IFunctionRasterDataset InRaster
        {
            get
            {
                if(inrs==null)
                {
                    setInRs();
                }
                return inrs;
            }
        }

        private void setInRs()
        {
            IFunctionRasterDataset dset = rsUtil.getBand(valueraster, 0);
            inrs = rsUtil.createIdentityRaster(dset, rstPixelType.PT_FLOAT);
            
        }
        private IFunctionRasterDataset valueraster = null;
        public IFunctionRasterDataset ValueRaster
        {
            get
            {
                return valueraster;
            }
            set
            {
                valueraster = value;
            }
        }
        private int minCells = 200;
        private int maxCells = 2000;
        public int MinCells
        {
            get;
            set;
        }
        public double Radius 
        { 
            get 
            {
                double radius = 3;//Math.Sqrt(minCells / Math.PI);
                return radius; 
            } 
        }

    }
}
