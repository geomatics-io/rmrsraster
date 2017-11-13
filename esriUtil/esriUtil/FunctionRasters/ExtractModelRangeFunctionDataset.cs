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
    class ExtractModelRangeFunctionDataset : IRasterFunction
    {
        public ExtractModelRangeFunctionDataset()
        {
        }
        private IRasterInfo myRasterInfo; // Raster Info for the Tobit Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the Tobit Function.
        private string myName = "Model Domain Function"; // Name of the log Function.
        private string myDescription = "Defines the range of values of a model"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBands = null;
        private double[] mins = null;
        private double[] maxs = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass();
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();// Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is ExtractModelRangeFunctionArguments)
            {
                ExtractModelRangeFunctionArguments arg = (ExtractModelRangeFunctionArguments)pArgument;
                inrsBands = arg.InRaster;
                mins = arg.Mins;
                maxs = arg.Maxs;
                outrs = arg.OutRaster;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBands);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: ExtractModelRangeFunctionArguments");
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
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);
                myFunctionHelperCoef.Read(pTlc, null, myFunctionHelperCoef.Raster, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                for (int nBand = 0; nBand < pPixelBlock.Planes; nBand++)
                {
                    System.Array outArr = (System.Array)ipPixelBlock.get_PixelData(nBand);
                    rstPixelType pty = ipPixelBlock.get_PixelType(nBand);
                    for (int i = pBRowIndex; i < pBHeight; i++)
                    {
                        for (int k = pBColIndex; k < pBWidth; k++)
                        {
                            bool checkNoData = true;
                            int ov = 1;
                            for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                            {
                                double min = mins[coefnBand];
                                double max = maxs[coefnBand];
                                object objPvl = outPb.GetVal(coefnBand, k, i);
                                if (objPvl == null)
                                {
                                    checkNoData = false;
                                    ov = 0;
                                    break;
                                }
                                else
                                {
                                    double pixelValue = Convert.ToDouble(objPvl);
                                    if(pixelValue<min || pixelValue>max)
                                    {
                                        ov = 0;
                                        break;
                                    }
                                }
                            }
                            if (checkNoData)
                            {
                                object nVl = rasterUtil.getSafeValue(ov, pty);
                                outArr.SetValue(nVl, k, i);
                            }
                        }
                    }
                    ipPixelBlock.set_PixelData(nBand, outArr);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of Domain Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of Domain Function", exc);
                throw myExc;
            }
        }
    }
}
