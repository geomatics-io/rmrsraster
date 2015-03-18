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
    class PixelBlockToRasterFunctionArguments
    {
        public PixelBlockToRasterFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public PixelBlockToRasterFunctionArguments(rasterUtil rasterUtility)
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
            IEnvelope env = new EnvelopeClass();
            double mx,my;
            IRaster2 rs = (IRaster2)rsUtil.createRaster(valueraster);
            IPnt cellSize = valueraster.RasterInfo.CellSize;
            rs.PixelToMap((int)topleft.X,(int)topleft.Y,out mx, out my);
            env.PutCoords(mx,my-(cellSize.Y*vpixelBlock.Height),mx + (cellSize.X*vpixelBlock.Width),my);
            inrs = rsUtil.constantRasterFunction((IRaster)rs, env, 0, cellSize);
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
        private IPnt topleft = null;
        public IPnt TopLeft
        {
            get
            {
                return topleft;
            }
            set
            {
                topleft = value;
            }
        }
        private IPixelBlock vpixelBlock = null;
        public IPixelBlock ValuePixelBlock
        {
            get
            {
                return vpixelBlock;
            }
            set
            {
                vpixelBlock = value;
            }
        }


    }
}
