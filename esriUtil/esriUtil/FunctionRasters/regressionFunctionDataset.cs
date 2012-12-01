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
    class regressionFunctionDataset : IRasterFunction
    {
        public regressionFunctionDataset()
        {
        }
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Regression Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using regression transformation"; // Description of the log Function.
        private IRaster outrs = null;
        private IRaster inrsBandsCoef = null;
        private List<float[]> slopes = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is regressionFunctionArguments)
            {
                regressionFunctionArguments arg = (regressionFunctionArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                slopes = arg.Slopes;
                outrs = arg.OutRaster;
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: regressionFunctionArguments");
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

                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                float noDataVl = System.Convert.ToSingle(noDataValueArr.GetValue(0));
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                noDataValueArr = (System.Array)((IRasterProps)inrsBandsCoef).NoDataValue;//inrsBandsCoef
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)inrsBandsCoef.CreatePixelBlock(pbSize);
                inrsBandsCoef.Read(pTlc, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] pArr = new System.Array[outPb.Planes];
                for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                {
                    System.Array pixelValues = (System.Array)(outPb.get_PixelData(coefnBand));
                    pArr[coefnBand] = pixelValues;
                }
                for (int nBand = 0; nBand < pPixelBlock.Planes; nBand++)
                {
                    System.Array outArr = (System.Array)ipPixelBlock.get_PixelData(nBand);
                    for (int i = pBRowIndex; i < pBHeight; i++)
                    {
                        for (int k = pBColIndex; k < pBWidth; k++)
                        {

                            float[] IntSlpArr = slopes[nBand];
                            double sumVls = IntSlpArr[0];
                            for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                            {
                                double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(coefnBand));
                                double pixelValue = Convert.ToDouble(pArr[coefnBand].GetValue(k, i));
                                if (rasterUtil.isNullData(pixelValue, noDataValue))
                                {
                                    sumVls = noDataVl;
                                    break;
                                }
                                
                                double slp = System.Convert.ToDouble(IntSlpArr[coefnBand + 1]);
                                sumVls += pixelValue * slp;
                            }
                            outArr.SetValue(System.Convert.ToSingle(sumVls), k, i);
                        }
                    }
                    ipPixelBlock.set_PixelData(nBand, outArr);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of Regression Function. " + exc.Message, exc);
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