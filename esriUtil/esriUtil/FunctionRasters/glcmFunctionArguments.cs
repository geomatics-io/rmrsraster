using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil.FunctionRasters.NeighborhoodHelper;

namespace esriUtil.FunctionRasters
{
    class glcmFunctionArguments
    {
        public glcmFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public glcmFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster 
        { 
            get 
            {
                return inrs;
            } 
            set 
            { 
                IRaster inrsT = value;
                inrs = rsUtil.constantRasterFunction(inrsT, 0);
                origRs = rsUtil.returnRaster(value,rstPixelType.PT_FLOAT);
            } 
        }
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
        private bool horz = true;
        public bool Horizontal { get { return horz; } set { horz = value; } }
        public List<int[]> GenericIterator 
        { 
            get 
            {
                if (windowtype == rasterUtil.windowType.RECTANGLE)
                {
                    rsUtil.createFocalWindowRectangleGLCM(columns, rows, horz, out genericiterator);
                }
                else
                {
                    rsUtil.createFocalWindowCircleGLCM(radius,horz, out genericiterator);
                }
                return genericiterator;
            }
        }
        public rasterUtil.windowType WindowType
        {
            get { return windowtype; }
            
        }
        private int columns = 3;
        private int rows = 3;
        private rasterUtil.glcmMetric glcmmetrics = rasterUtil.glcmMetric.CONTRAST;
        public rasterUtil.glcmMetric GLCMMETRICS
        { 
            get 
            { 
                return glcmmetrics; 
            } 
            set 
            { 
                glcmmetrics = value; 
            } 
        }
        public int Columns { get { return columns; } set { columns = value; windowtype = rasterUtil.windowType.RECTANGLE; } }
        public int Rows { get { return rows; } set { rows = value; windowtype = rasterUtil.windowType.RECTANGLE; } }
        private ESRI.ArcGIS.Geodatabase.IRaster origRs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OriginalRaster { get { return origRs; } }
    }
}

