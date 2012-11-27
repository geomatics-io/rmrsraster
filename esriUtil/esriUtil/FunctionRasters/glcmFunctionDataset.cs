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
    public abstract class glcmFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "focal Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using focal analysis"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster orig = null;
        private int clms, rws, radius;
        public List<int[]> iter = null;
        public bool horizontal = true;
        private rasterUtil.windowType inWindow = rasterUtil.windowType.RECTANGLE;
        public rasterUtil.glcmMetric glcmMetrics = rasterUtil.glcmMetric.CONTRAST; 
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataValue = Single.MinValue;
        public void Bind(object pArgument)
        {
            if (pArgument is glcmFunctionArguments)
            {
                glcmFunctionArguments args = (glcmFunctionArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                inWindow = args.WindowType;
                iter = args.GenericIterator;
                clms = args.Columns;
                rws = args.Rows;
                radius = args.Radius;
                glcmMetrics = args.GLCMMETRICS;
                horizontal = args.Horizontal;
                IRasterProps rsProp = (IRasterProps)inrs;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: glcmFunctonArguments");
            }
        }
        /// <summary>
        /// Read pixels from the input Raster and fill the PixelBlock provided with processed pixels.
        /// The RasterFunctionHelper object is used to handle pixel type conversion and resampling.
        /// The log raster is the natural log of the raster. 
        /// </summary>
        /// <param name="pTlc">Point to start the reading from in the Raster</param>
        /// <param name="pRaster">Reference Raster for the PixelBlock</param>
        /// <param name="pPixelBlock">PixelBlock to be filled in</param>
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock)
        {
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPnt pbBigSize = new PntClass();
                IPnt pbBigLoc = new PntClass();
                int pbBigWd = pBWidth + clms;
                int pbBigHt = pBHeight + rws;
                int l, t;
                l = clms / 2;
                t = rws / 2;
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(0));                
                for (int nBand = 0; nBand < ipPixelBlock.Planes; nBand++)
                {
                    System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    for (int r = 0; r < pBHeight; r++)
                    {
                        for (int c = 0; c < pBWidth; c++)
                        {
                            float inVl = System.Convert.ToSingle(pixelValues.GetValue(c, r));
                            if (rasterUtil.isNullData(inVl, noDataValue))
                            {
                                continue;
                            }
                            else
                            {
                                Dictionary<string,int> glcmDic = getGLCMDic(pixelValuesBig, c, r);
                                float outVl = System.Convert.ToSingle(getTransformedValue(glcmDic));
                                //Console.WriteLine("GLCM Value = " + outVl.ToString());
                                pixelValues.SetValue(outVl,c,r);
                            }
                        }

                    }
                    ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private Dictionary<string, int> getGLCMDic(System.Array bigArr, int startClm, int startRw)
        {
            Dictionary<string, int> countDic = new Dictionary<string, int>();
            foreach (int[] xy in iter)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                int bWc2 = bWc;
                int bRc2 = bRc;
                bool getNeighbor = (xy[2] == 1);
                if (getNeighbor)
                {
                    float vl = System.Convert.ToSingle(bigArr.GetValue(bWc, bRc));
                    int cnt = 0;
                    if (horizontal)
                    {
                        bWc2 = bWc + 1;
                    }
                    else
                    {
                        bRc2 = bRc - 1;
                    }
                    float vl2 = System.Convert.ToSingle(bigArr.GetValue(bWc2, bRc2));
                    string pair = vl.ToString() + ":" + vl2.ToString();
                    if (countDic.TryGetValue(pair, out cnt))
                    {
                        countDic[pair] = cnt + 1;
                    }
                    else
                    {
                        countDic.Add(pair, 1);
                    }
                    string pair2 = vl2.ToString() + ":" + vl.ToString();
                    if (countDic.TryGetValue(pair2, out cnt))
                    {
                        countDic[pair2] = cnt + 1;
                    }
                    else
                    {
                        countDic.Add(pair2, 1);
                    }
                }
            }
            return countDic;
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }

        public abstract object getTransformedValue(Dictionary<string,int> glcmDic);
    }
}