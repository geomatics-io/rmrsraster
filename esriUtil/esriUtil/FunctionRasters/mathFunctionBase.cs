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
    public abstract class mathFunctionBase: IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Math Function"; // Name of the log Function.
        private string myDescription = "Performs a math transformation of each pixel value"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is MathFunctionArguments)
            {
                MathFunctionArguments args = (MathFunctionArguments)pArgument;
                inrs = args.InRaster;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: MathFunctionArguments");
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
            double vl = 0;
            float pixelValue = 0f;
            try
            {
                //System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                // Call Read method of the Raster Function Helper object.
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                IPixelBlock3 pb3 = (IPixelBlock3)pPixelBlock;
                #region Load log object
                for (int nBand = 0; nBand < pPixelBlock.Planes; nBand++)
                {
                    //float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                    System.Array dArr = (System.Array)pb3.get_PixelData(nBand);
                    for (int r = 0; r < pPixelBlock.Height; r++)
                    {
                        for (int c = 0; c < pPixelBlock.Width; c++)
                        {
                            object objVl = pb3.GetVal(nBand,c, r);
                            if (objVl == null)
                            {
                                continue;
                            }
                            else
                            {
                                vl = System.Convert.ToSingle(objVl);
                                pixelValue = (float)getFunctionValue(vl);
                                dArr.SetValue(pixelValue, c, r);
                            }
                        }
                    }
                    pb3.set_PixelData(nBand, dArr);
                }
                #endregion
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method math Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of math Function", exc);
                throw myExc;
            }
        }
        public abstract double getFunctionValue(double inValue);
    }
}
