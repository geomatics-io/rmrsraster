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
        private IFunctionRasterDataset inrs = null;
        public IFunctionRasterDataset InRaster
        { 
            get 
            { 
                return inrs;
            } 
            set 
            { 
                inrs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
                origRs = rsUtil.createIdentityRaster(value, rstPixelType.PT_FLOAT);
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
        private int[][] fastiter = null;
        public int[][] Fastiter
        {
            get
            {
                fastiter = new int[rows][];
                Dictionary<int,int[]> dic  = new Dictionary<int,int[]>();
                foreach (int[] xyPair in GenericIterator)
                {
                    int c = xyPair[0];
                    int r = xyPair[1];
                    int[] clmsVls = new int[2];
                    if(dic.TryGetValue(r,out clmsVls))
                    {
                        int min = clmsVls[0];
                        int max = clmsVls[1];
                        if (c < min) clmsVls[0] = c;
                        if (c > max) clmsVls[1] = c;  
                    }
                    else
                    {
                        clmsVls = new int[2];
                        clmsVls[0]=c;
                        clmsVls[1]=c;
                    }
                    dic[r] = clmsVls;
                }
                for (int i=0;i<rows;i++)
                {
                    fastiter[i] = dic[i];
                }
                return fastiter;
                

            }

        }
        private float windowN = 9;
        public float WindowCount { get { return windowN; } set { windowN = value; } }
        private List<int[]> genericiterator = new List<int[]>();
        public List<int[]> GenericIterator 
        { 
            get 
            {
                if (windowtype == rasterUtil.windowType.RECTANGLE)
                {
                    rsUtil.createFocalWindowRectangle(columns, rows, out genericiterator);
                    windowN = rows * columns;
                }
                else
                {
                    windowN = (from int v in (rsUtil.createFocalWindowCircle(radius, out genericiterator)) select v).Sum();
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
        private IFunctionRasterDataset origRs = null;
        public IFunctionRasterDataset OriginalRaster { get { return origRs; } }
    }
}
