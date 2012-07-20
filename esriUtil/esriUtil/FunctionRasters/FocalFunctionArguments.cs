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
    public class FocalFunctionArguments
    {
        public FocalFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public FocalFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster { 
            get 
            { 
                return inrs;
            } 
            set 
            { 
                IRaster inrsT = value;
                IRasterProps rsProps = (IRasterProps)inrsT;
                if(rsProps.PixelType!=rstPixelType.PT_DOUBLE)
                {
                    inrsT = rsUtil.convertToDifFormatFunction(inrsT, rstPixelType.PT_DOUBLE);
                }
                inrs = inrsT;
                origRs = rsUtil.getBand(inrs, 0);
            } 
        }
        public rasterUtil.focalType Operation { get; set; }
        private rasterUtil.windowType windowtype = rasterUtil.windowType.RECTANGLE;
        private int radius = 2;
        public int Radius
        {
            get 
            { 
                return radius;
            }
            set
            {
                radius=value;
                columns = ((radius-1)*2)+1;
                rows = columns;
                windowtype = rasterUtil.windowType.CIRCLE;
            }
        }
        private List<int[]> genericiterator = new List<int[]>();
        public List<int[]> GenericIterator 
        { 
            get 
            {
                if (windowtype == rasterUtil.windowType.RECTANGLE)
                {
                    rsUtil.createFocalWindowRectangle(columns, rows, out genericiterator);
                }
                else
                {
                    rsUtil.createFocalWindowCircle(radius, out genericiterator);
                }
                return genericiterator;
            }
        }
        public rasterUtil.windowType WindowType
        {
            get { return windowtype; }
            //set
            //{
            //    windowtype = value;
            //    if (windowtype == rasterUtil.windowType.CIRCLE)
            //    {
            //        if (Columns % 2 != 0)
            //        {
            //            Radius = (Columns + 1) / 2;
            //        }
            //        else
            //        {
            //            Radius = Columns / 2;
            //        }

            //        rsUtil.createFocalWindowCircle(Radius,out genericiterator);
            //    }
            //    else
            //    {
            //        rsUtil.createFocalWindowRectangle(Columns, Rows, out genericiterator);
            //    }
            //}
        }
        private int columns = 3;
        private int rows = 3;
        public int Columns { get { return columns; } set { columns = value; windowtype = rasterUtil.windowType.RECTANGLE; } }
        public int Rows { get { return rows; } set { rows = value; windowtype = rasterUtil.windowType.RECTANGLE; } }
        private ESRI.ArcGIS.Geodatabase.IRaster origRs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OriginalRaster { get { return origRs; } }
    }
}
