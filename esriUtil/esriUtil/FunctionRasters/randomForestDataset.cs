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
    class randomForestDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Random Forest Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using Random Forest"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBandsCoef = null;
        private Statistics.dataPrepRandomForest df = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass();
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();// Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        private double[] xVls = null;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is randomForestArguments)
            {
                randomForestArguments arg = (randomForestArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                outrs = arg.OutRaster;
                xVls = new double[((IRasterBandCollection)inrsBandsCoef).Count];
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                df = arg.RandomForestModel;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBandsCoef);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: randomForestArguments");
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
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);//independent variables  
                myFunctionHelperCoef.Read(pTlc,null,myFunctionHelper.Raster, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] cArr = new System.Array[ipPixelBlock.Planes];
                for (int outBand = 0; outBand < ipPixelBlock.Planes; outBand++)
                {
                    System.Array pixelValues = (System.Array)ipPixelBlock.get_PixelData(outBand);
                    cArr[outBand] = pixelValues;
                }

                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        bool ndT = true;
                        for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                        {
                            object pObj = outPb.GetVal(coefnBand, k, i);
                            if (pObj==null)
                            {
                                ndT = false;
                                //Console.WriteLine("Pixel Value = null for band " + coefnBand.ToString());
                                break;
                            }
                            double pixelValue = Convert.ToDouble(pObj);
                            //Console.WriteLine("Pixel Value = " + pixelValue.ToString());
                            xVls[coefnBand] = pixelValue;
                        }
                        if (ndT)
                        {
                            try
                            {
                                double[] pp = df.computNew(xVls);
                                for (int p = 0; p < pp.Length; p++)
                                {
                                    double pVl = pp[p];
                                    float spVl = System.Convert.ToSingle(pVl);
                                    //Console.WriteLine("Computed Value = " + spVl.ToString());
                                    cArr[p].SetValue(spVl, k, i);
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }

                    }
                }
                for (int i = 0; i < ipPixelBlock.Planes; i++)
                {
                    ipPixelBlock.set_PixelData(i, cArr[i]);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of the forest function Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }
    }
}