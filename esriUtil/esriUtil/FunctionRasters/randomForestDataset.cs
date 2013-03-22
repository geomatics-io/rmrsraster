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
        private IRaster outrs = null;
        private IRaster inrsBandsCoef = null;
        private Statistics.dataPrepRandomForest df = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is randomForestArguments)
            {
                randomForestArguments arg = (randomForestArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                df = arg.RandomForestModel;
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
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
                // Call Read method of the Raster Function Helper object.

                System.Array noDataValueArr = (System.Array)((IRasterProps)inrsBandsCoef).NoDataValue;
                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                //Console.WriteLine("After Read");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)inrsBandsCoef.CreatePixelBlock(pbSize);//independent variables  
                inrsBandsCoef.Read(pTlc, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] pArr = new System.Array[outPb.Planes];
                System.Array[] cArr = new System.Array[ipPixelBlock.Planes];
                for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                {
                    System.Array pixelValues = (System.Array)(outPb.get_PixelData(coefnBand));
                    pArr[coefnBand] = pixelValues;
                }

                for (int outBand = 0; outBand < ipPixelBlock.Planes; outBand++)
                {
                    //float[,] td = new float[ipPixelBlock.Width, ipPixelBlock.Height];
                    System.Array pixelValues = (System.Array)ipPixelBlock.get_PixelData(outBand);//(System.Array)(td);
                    cArr[outBand] = pixelValues;
                }

                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        double[] xVls = new double[outPb.Planes];
                        bool ndT = true;
                        for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                        {
                            float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(coefnBand));
                            float pixelValue = Convert.ToSingle(pArr[coefnBand].GetValue(k, i));
                            if (rasterUtil.isNullData(pixelValue, noDataValue))
                            {
                                ndT = false;
                                break;
                            }
                            xVls[coefnBand] = pixelValue;
                        }
                        if (ndT)
                        {
                            double[] pp = df.computNew(xVls);
                            for (int p = 0; p < pp.Length; p++)
                            {
                                double pVl = pp[p];
                                cArr[p].SetValue(System.Convert.ToSingle(pVl), k, i);
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