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
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset orig = null;
        private int clms, rws, radius;
        public List<coordPair[]> iter = null;
        public bool horizontal = true;
        private rasterUtil.windowType inWindow = rasterUtil.windowType.RECTANGLE;
        public rasterUtil.glcmMetric glcmMetrics = rasterUtil.glcmMetric.CONTRAST; 
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperOrig = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataValue = Single.MinValue;
        private Dictionary<string, int> countDic = new Dictionary<string, int>();
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
                myFunctionHelper.Bind(inrs);
                myFunctionHelperOrig.Bind(orig);
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
                //System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
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
                IPixelBlock3 pbBig = (IPixelBlock3)myFunctionHelperOrig.Raster.CreatePixelBlock(pbBigSize);
                myFunctionHelperOrig.Read(pbBigLoc,null,myFunctionHelperOrig.Raster, (IPixelBlock)pbBig);
                noDataValue = float.MinValue;//System.Convert.ToSingle(noDataValueArr.GetValue(0));                
                for (int nBand = 0; nBand < ipPixelBlock.Planes; nBand++)
                {
                    rstPixelType pbt = ipPixelBlock.get_PixelType(nBand);
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    for (int r = 0; r < pBHeight; r++)
                    {
                        for (int c = 0; c < pBWidth; c++)
                        {
                            object inVlobj = ipPixelBlock.GetVal(nBand, c, r);
                            if (inVlobj==null)
                            {
                                continue;
                            }
                            else
                            {
                                updateGLCMDic(pbBig, c, r, nBand);
                                float outVl = System.Convert.ToSingle(getTransformedValue(countDic));
                                object newVl = rasterUtil.getSafeValue(outVl, pbt);
                                pixelValues.SetValue(newVl,c,r);
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

        private void updateGLCMDic(IPixelBlock3 pbBig, int startClm, int startRw, int nBand)
        {
            foreach (coordPair[] xy in iter)
            {
                coordPair sub = xy[0];
                coordPair add = xy[1];
                #region subtract from dictionary
                try
                {
                    
                    int c = sub.X + startClm;
                    int r = sub.Y + startRw;
                    int sc = sub.NX + startClm;
                    int sr = sub.NY + startRw;
                    object objVl1 = pbBig.GetVal(nBand, c, r);

                    object objVl2 = pbBig.GetVal(nBand, sc, sr);
                    if (objVl1 == null || objVl2 == null)
                    {
                        continue;
                    }
                    else
                    {
                        int cnt = 0;
                        string pair1 = objVl1.ToString() + ":" + objVl2.ToString();
                        string pair2 = objVl2.ToString() + ":" + objVl1.ToString();
                        if (countDic.TryGetValue(pair1, out cnt))
                        {
                            cnt = cnt - 1;
                            if (cnt == 0)
                            {
                                countDic.Remove(pair1);
                            }
                            else
                            {
                                countDic[pair1] = cnt;
                            }
                        }
                        else
                        {
                        }
                        if (countDic.TryGetValue(pair2, out cnt))
                        {
                            cnt = cnt - 1;
                            if (cnt == 0)
                            {
                                countDic.Remove(pair2);
                            }
                            else
                            {
                                countDic[pair2] = cnt;
                            }
                        }
                        else
                        {
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                #endregion
                #region add values to dicitonary
                try
                {
                    int c = add.X + startClm;
                    int r = add.Y + startRw;
                    int sc = add.NX + startClm;
                    int sr = add.NY + startRw;
                    object objVl1 = pbBig.GetVal(nBand, c, r);
                    object objVl2 = pbBig.GetVal(nBand, sc, sr);
                    if (objVl1 == null || objVl2 == null)
                    {
                        continue;
                    }
                    else
                    {
                        int cnt = 0;
                        string pair1 = objVl1.ToString() + ":" + objVl2.ToString();
                        string pair2 = objVl2.ToString() + ":" + objVl1.ToString();
                        if (countDic.TryGetValue(pair1, out cnt))
                        {
                            countDic[pair1] = cnt + 1;
                        }
                        else
                        {
                            countDic.Add(pair1, 1);
                        }
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
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                #endregion    

            }
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