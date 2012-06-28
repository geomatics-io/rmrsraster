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
    class localMinBandFunction : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Local MinBand Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using local Band value transformation"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster inrsBands = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is LocalFunctionArguments)
            {
                LocalFunctionArguments arg = (LocalFunctionArguments)pArgument;
                inrsBands = arg.InRaster;
                inrs = arg.outRaster;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: LocalFunctionArguments");
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
                double noDataVl = System.Convert.ToDouble(noDataValueArr.GetValue(0));
                noDataValueArr = (System.Array)((IRasterProps)inrsBands).NoDataValue;
                // Call Read method of the Raster Function Helper object.
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)inrsBands.CreatePixelBlock(pbSize);
                inrsBands.Read(pTlc, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] pArr = new System.Array[outPb.Planes];
                System.Array outArr = (System.Array)ipPixelBlock.get_PixelData(0);
                for (int nBand = 0; nBand < outPb.Planes; nBand++)
                {
                    System.Array pixelValues = (System.Array)(outPb.get_PixelData(nBand));
                    pArr[nBand] = pixelValues;
                }
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        double min = Double.MaxValue;
                        int outBand = 0;
                        for (int nBand = 0; nBand < outPb.Planes; nBand++)
                        {
                            double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                            double pixelValue = Convert.ToDouble(pArr[nBand].GetValue(k, i));
                            if (pixelValue == noDataValue)
                            {
                                min = noDataVl;
                                break;
                            }
                            if (pixelValue < min)
                            {
                                min = pixelValue;
                                outBand = nBand+1;
                            }
                        }
                        outArr.SetValue(outBand, k, i);
                    }
                }
                ipPixelBlock.set_PixelData(0, outArr);

            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of localMean Function. " + exc.Message, exc);
                throw myExc;
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
