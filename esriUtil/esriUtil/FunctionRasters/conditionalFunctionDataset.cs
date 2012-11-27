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
            IRaster2 inRs2 = (IRaster2)pRaster;
            double inX, inY;
            int outX, outY; 
            inRs2.PixelToMap(System.Convert.ToInt32(pTlc.X),System.Convert.ToInt32(pTlc.Y),out inX, out inY);
            IPnt tpTlc = new PntClass();
            IPnt fpTlc = new PntClass();
            IRaster2 tRs2 = (IRaster2)trueRs;
            IRaster2 fRs2 = (IRaster2)falseRs;
            tRs2.MapToPixel(inX, inY, out outX, out outY);
            tpTlc.SetCoords(outX, outY);
            fRs2.MapToPixel(inX, inY, out outX, out outY);
            fpTlc.SetCoords(outX, outY);
            float pixelValue = 0f;
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                System.Array noDataValueArrT = (System.Array)((IRasterProps)trueRs).NoDataValue;
                System.Array noDataValueArrF = (System.Array)((IRasterProps)falseRs).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                #region Load log object
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                Pnt pbSize = new PntClass();
                pbSize.SetCoords(pPixelBlock.Width,pPixelBlock.Height);
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPixelBlock3 truePb = (IPixelBlock3)trueRs.CreatePixelBlock(pbSize);
                IPixelBlock3 falsePb = (IPixelBlock3)falseRs.CreatePixelBlock(pbSize);
                trueRs.Read(tpTlc, (IPixelBlock)truePb);
                falseRs.Read(fpTlc, (IPixelBlock)falsePb);
                float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(0));
                float noDataValueT = System.Convert.ToSingle(noDataValueArrT.GetValue(0));
                float noDataValueF = System.Convert.ToSingle(noDataValueArrF.GetValue(0));
                System.Array pixelValuesCon = (System.Array)ipPixelBlock.get_PixelData(0);
                int lng = pixelValuesCon.Length;
                System.Array truePixelValues = (System.Array)truePb.get_PixelData(0);
                System.Array falsePixelValues = (System.Array)falsePb.get_PixelData(0);
                for (int r = 0; r < ipPixelBlock.Height; r++)
                {
                    for (int c = 0; c < ipPixelBlock.Width; c++)
                    {
                        pixelValue = System.Convert.ToSingle(pixelValuesCon.GetValue(c, r));
                        if (rasterUtil.isNullData(pixelValue, noDataValue)) continue;
                        if (pixelValue >= 1)
                        {
                            pixelValue = System.Convert.ToSingle(truePixelValues.GetValue(c, r));
                            if (pixelValue == noDataValueT) pixelValue = noDataValue;
                        }
                        else
                        {
                            pixelValue = System.Convert.ToSingle(falsePixelValues.GetValue(c, r));
                            if (pixelValue == noDataValueT) pixelValue = noDataValue;
                        }
                    }
                    
                }
                ipPixelBlock.set_PixelData(0, pixelValuesCon);

                //unsafe
                //{
                //    fixed(float* conVl = pixelValuesCon, tVl = truePixelValues, fVl = falsePixelValues)
                //    {
                //        for (int i = 0; i < lng; i++)
                //        {
                //            pixelValue = *(conVl + i);
                //            if (rasterUtil.isNullData(pixelValue, noDataValue))
                //            {
                //                continue;
                //            }
                //            if (pixelValue >= 1)
                //            {
                //                pixelValue = *(tVl+i);
                //                if (pixelValue == noDataValueT) pixelValue = noDataValue;
                //            }
                //            else
                //            {
                //                pixelValue = *(fVl + i);
                //                if (pixelValue == noDataValueF) pixelValue = noDataValue;
                //            }
                //            *(conVl + i) = pixelValue;
                            
                //        }
                //    }
                //}
                //ipPixelBlock.set_PixelData(0, pixelValuesCon);
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
