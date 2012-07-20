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
        private IRaster conRs, trueRs, falseRs;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
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
                conRs = args.ConditionalRaster;
                trueRs = args.TrueRaster;
                falseRs = args.FalseRaster;
                myFunctionHelper.Bind(conRs);
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
            double pixelValue = 0d;
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                System.Array noDataValueArrT = (System.Array)((IRasterProps)trueRs).NoDataValue;
                System.Array noDataValueArrF = (System.Array)((IRasterProps)falseRs).NoDataValue;
                // Call Read method of the Raster Function Helper object.
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                #region Load log object
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                Pnt pbSize = new PntClass();
                pbSize.SetCoords(pPixelBlock.Width,pPixelBlock.Height);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPixelBlock3 truePb = (IPixelBlock3)trueRs.CreatePixelBlock(pbSize);
                IPixelBlock3 falsePb = (IPixelBlock3)falseRs.CreatePixelBlock(pbSize);
                trueRs.Read(pTlc, (IPixelBlock)truePb);
                falseRs.Read(pTlc, (IPixelBlock)falsePb);
                double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(0));
                double noDataValueT = System.Convert.ToDouble(noDataValueArrT.GetValue(0));
                double noDataValueF = System.Convert.ToDouble(noDataValueArrF.GetValue(0));
                System.Array pixelValuesCon = (System.Array)ipPixelBlock.get_PixelData(0);
                System.Array truePixelValues = (System.Array)truePb.get_PixelData(0);
                System.Array falsePixelValues = (System.Array)falsePb.get_PixelData(0);
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        pixelValue = Convert.ToDouble(pixelValuesCon.GetValue(k, i));
                        if (pixelValue == noDataValue)
                        {
                            continue;
                        }
                        if(pixelValue>=1)
                        {
                            pixelValue = System.Convert.ToDouble(truePixelValues.GetValue(k,i));
                            if (pixelValue == noDataValueT) pixelValue = noDataValue;
                        }
                        else
                        {
                            pixelValue = System.Convert.ToDouble(falsePixelValues.GetValue(k,i));
                            if (pixelValue == noDataValueF) pixelValue = noDataValue;
                        }

                        pixelValuesCon.SetValue(pixelValue, k, i);
                    }
                }
                pPixelBlock.set_SafeArray(0, pixelValuesCon);
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
