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
        private IFunctionRasterDataset inrs = null;
        public IFunctionRasterDataset InRaster 
        { 
            get 
            {
                return inrs;
            } 
            set 
            {
                inrs = rsUtil.createIdentityRaster(value);
                origRs = rsUtil.createIdentityRaster(value,rstPixelType.PT_FLOAT);
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
        private List<coordPair[]> genericiterator = new List<coordPair[]>();
        private bool horz = true;
        public bool Horizontal { get { return horz; } set { horz = value; } }
        public List<coordPair[]> GenericIterator //list of pair of pixels to be subtracted and added to dictionary
        { 
            get 
            {
                genericiterator.Clear();
                if (windowtype == rasterUtil.windowType.RECTANGLE)
                {
                    if (Horizontal)
                    {
                        for (int i = 0; i < rows; i++)
                        {
                            coordPair sub = new coordPair();
                            sub.X = 0;
                            sub.Y = i;
                            sub.NX = -1;
                            sub.NY = i;
                            coordPair add = new coordPair();
                            add.X = columns - 1;
                            add.Y = i;
                            add.NX = columns - 2;
                            add.NY = i;
                            genericiterator.Add(new coordPair[2]{sub,add});
                        }
                    }
                    else
                    {
                        for (int i = 0; i < columns; i++)
                        {
                            coordPair sub = new coordPair();
                            sub.X = i;
                            sub.Y = 0;
                            sub.NX = i;
                            sub.NY = -1;
                            coordPair add = new coordPair();
                            add.X = i;
                            add.Y = rows - 1;
                            add.NX = i;
                            add.NY = rows - 2;
                            genericiterator.Add(new coordPair[2] { sub, add });
                        }
                    }
                    
			

                }
                else
                {
                    if(Horizontal)
                    {
                        for (int y = 0; y < rows; y++)
                        {
                            for (int x = 0; x < columns; x++)
                            {
                                double cD = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow((y - radius), 2));
                                if (cD <= radius)
                                {
                                    coordPair sub = new coordPair();
                                    sub.X = x;
                                    sub.Y = y;
                                    sub.NX = x-1;
                                    sub.NY = y;
                                    coordPair add = new coordPair();
                                    add.X = x;
                                    add.Y = y;
                                    add.NX = columns-x;
                                    add.NY = y;
                                    genericiterator.Add(new coordPair[]{sub,add}); 
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int x = 0; x <columns; x++)
                        {
                            for (int y = 0; y < rows; y++)
                            {
                                double cD = Math.Sqrt(Math.Pow((x - radius), 2) + Math.Pow((y - radius), 2));
                                if (cD <= radius)
                                {
                                    coordPair sub = new coordPair();
                                    sub.X = x;
                                    sub.Y = y;
                                    sub.NX = x;
                                    sub.NY = y-1;
                                    coordPair add = new coordPair();
                                    add.X = x;
                                    add.Y = y;
                                    add.NX = x;
                                    add.NY = rows-y;
                                    genericiterator.Add(new coordPair[]{sub,add}); 
                                    break;
                                }
                            }
                        }
                    }
                      
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
        private IFunctionRasterDataset origRs = null;
        public IFunctionRasterDataset OriginalRaster { get { return origRs; } }
    }
    public class coordPair
    {
        public int NX { get; set; }
        public int NY { get; set; }
        public int X{get;set;}
        public int Y{get;set;}
    }
}

