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
    class pairedttestFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "TTest Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using TTest transformation"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBandsCoef = null;
        private Dictionary<string, double[]> tDic = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is pairedttestFunctionArguments)
            {
                pairedttestFunctionArguments arg = (pairedttestFunctionArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                tDic = arg.TTestDictionary;
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBandsCoef);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: plrFunctionArguments");
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
                // Call Read method of the Raster Function Helper object.

                //System.Array noDataValueArr = (System.Array)((IRasterProps)inrsBandsCoef).NoDataValue;
                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                //Console.WriteLine("After Read");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);//independent variables  
                myFunctionHelperCoef.Read(pTlc, null,myFunctionHelperCoef.Raster,(IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] pArr = new System.Array[ipPixelBlock.Planes];
                for (int coefnBand = 0; coefnBand < ipPixelBlock.Planes; coefnBand++)
                {
                    System.Array pixelValues = (System.Array)(pPixelBlock.get_SafeArray(coefnBand));
                    pArr[coefnBand] = pixelValues;
                }
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        object pObj = outPb.GetVal(0, k, i);
                        
                        if (pObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            string pixelValue = pObj.ToString();
                            double[] c;
                            if (tDic.TryGetValue(pixelValue, out c))
                            {
                                for (int v = 0; v < pArr.Length; v++)
                                {
                                    rstPixelType pty = pPixelBlock.get_PixelType(v);
                                    float vl = System.Convert.ToSingle(c[v]);
                                    object newVl = rasterUtil.getSafeValue(vl, pty);
                                    pArr[v].SetValue(newVl, k, i);
                                }
                            }
                            else
                            {
                                for (int v = 0; v < pArr.Length; v++)
                                {
                                    pArr.SetValue(0, k, i);
                                }
                            }
                        }

                    }
                }
                for (int i = 0; i < pArr.Length; i++)
                {
                    ipPixelBlock.set_PixelData(i, pArr[i]);
                }


            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of TTest Function. " + exc.Message, exc);
                Console.WriteLine(exc.ToString());
            }
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of TTest Function", exc);
                throw myExc;
            }
        }
    }
}
