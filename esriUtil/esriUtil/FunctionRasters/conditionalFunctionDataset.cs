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
    class conditionalFunctionDataset : IRasterFunction
    {
        public conditionalFunctionDataset()
        {
        }
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Conditional Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using Conditonal transformation"; // Description of the log Function.
        private IFunctionRasterDataset outRs, coefRs;
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
            if (pArgument is conditionalFunctionArguments)
            {
                conditionalFunctionArguments args = (conditionalFunctionArguments)pArgument;
                coefRs = args.CoefRaster;
                outRs = args.OutRaster;
                myFunctionHelper.Bind(outRs);
                myFunctionHelperCoef.Bind(coefRs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: Conditional arguments");
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
                #region Load log object
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                Pnt pbSize = new PntClass();
                pbSize.SetCoords(pPixelBlock.Width,pPixelBlock.Height);
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array outPutArr = (System.Array)ipPixelBlock.get_PixelData(0);
                IPixelBlock3 CoefPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);
                myFunctionHelperCoef.Read(pTlc, null, myFunctionHelperCoef.Raster, (IPixelBlock)CoefPb);
                for (int r = 0; r < ipPixelBlock.Height; r++)
                {
                    for (int c = 0; c < ipPixelBlock.Width; c++)
                    {
                        object vlObj = CoefPb.GetVal(0, c, r);
                        if (vlObj != null)
                        {
                            if ((float)vlObj > 0)
                            {
                                vlObj = CoefPb.GetVal(1, c, r);
                            }
                            else
                            {
                                vlObj = CoefPb.GetVal(2, c, r);
                            }
                            if (vlObj != null)
                            {
                                outPutArr.SetValue(vlObj, c, r);
                            }
                        }
                    }
                }
                ipPixelBlock.set_PixelData(0,outPutArr);
                #endregion
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of con Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of con Function", exc);
                throw myExc;
            }
        }
    }
}
