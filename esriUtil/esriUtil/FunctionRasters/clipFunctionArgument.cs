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
    class clipFunctionArgument
    {
        public clipFunctionArgument()
        {
            rsUtil = new rasterUtil();
        }
        public clipFunctionArgument(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OutRaster
        {
            get
            {
                IRasterProps rsProps = (IRasterProps)inrs;
                IPnt pntSize = rsProps.MeanCellSize();
                double cWidth = pntSize.X;
                double cHeight = pntSize.Y;
                double hCellW = cWidth / 2;
                double hCellH = cHeight / 2;
                IEnvelope env = geo.Envelope;
                double maxY = env.YMax;
                double minX = env.XMin;
                int mC, mR;
                IRaster2 inrs2 = (IRaster2)inrs;
                inrs2.MapToPixel(minX, maxY, out mC, out mR);
                inrs2.PixelToMap(mC, mR, out minX, out maxY);
                env.YMax = maxY+hCellH;
                env.XMin = minX-hCellW;
                double w = env.Width;
                double h = env.Height;
                int wC = (int)((w / pntSize.X) + 1);
                int wH = (int)((h / pntSize.Y) + 1);
                env.YMin = env.YMax - wH * cHeight;
                env.XMax = env.XMin + wC * cWidth;
                IRaster oRs = rsUtil.constantRasterFunction(inrs, env, 1, pntSize);
                return oRs;
            } 
        }
        private IPixelOperation pOp = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster 
        { 
            get 
            {
                if (pOp == null)
                {
                    IClipFilter2 cFilt2 = new ClipFilterClass();
                    cFilt2.Add(Geometry);
                    cFilt2.ClippingType = ClipType;
                    pOp = (IPixelOperation)inrs;
                    pOp.PixelFilter = (IPixelFilter)cFilt2;
                }
                return inrs;
            } 
            set 
            {
                inrs = rsUtil.returnRaster(value);
                pOp = null;
            } 
        }
        private IGeometry geo = null;
        public IGeometry Geometry { get { return geo; } set { geo = value; } }

        private esriRasterClippingType ctype = esriRasterClippingType.esriRasterClippingOutside;
        public esriRasterClippingType ClipType { get { return ctype; } set { ctype = value; } }
    }
}