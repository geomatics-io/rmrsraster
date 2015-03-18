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
    class convertPixelTypeFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private rstPixelType convPType = rstPixelType.PT_UNKNOWN;
        private string myName = "ConvertPixelType"; // Name of the log Function.
        private string myDescription = "Converts the values of a pixel type"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is convertPixelTypeFunctionArguments)
            {
                convertPixelTypeFunctionArguments arg = (convertPixelTypeFunctionArguments)pArgument;
                inrs = arg.InRaster;
                convPType = arg.RasterPixelType;
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
                // Call Read method of the Raster Function Helper object.
                //IRasterProps rsP = (IRasterProps)pRaster;
                //rsP.PixelType = convPType;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                for (int p = 0; p < pPixelBlock.Planes; p++)
			    {
                    System.Array outArr = (System.Array)pPixelBlock.get_SafeArray(p);
                    for (int r = 0; r < pPixelBlock.Height; r++)
                    {
                        for (int c = 0; c < pPixelBlock.Width; c++)
                        {
                            object vlObj = pPixelBlock.GetVal(p, c, r);
                            if (vlObj != null)
                            {
                                object newVl = convertVl(vlObj);
                                outArr.SetValue(newVl, c, r);
                            }
                            else
                            {
                                outArr.SetValue(null, c, r);
                            }
                        }
                    }
                    pPixelBlock.set_SafeArray(p, outArr);
			    }
                //int pBHeight = pPixelBlock.Height;
                //int pBWidth = pPixelBlock.Width;
                //IPnt pbSize = new PntClass();
                //pbSize.SetCoords(pBWidth, pBHeight);
                //IRasterBandCollection rsBc = (IRasterBandCollection)inrs;
                //for (int p = 0; p < pPixelBlock.Planes; p++)
                //{
                //    IRasterBand rsB = rsBc.Item(p);
                //    IRawPixels rP = (IRawPixels)rsB;
                //    IPixelBlock pb = rP.CreatePixelBlock(pbSize);
                //    rP.Read(pTlc, pb);
                //    System.Array outArr = (System.Array)((IPixelBlock3)pPixelBlock).get_PixelData(p);
                //    for (int r = 0; r < pBHeight; r++)
                //    {
                //        for (int c = 0; c < pBWidth; c++)
                //        {
                //            object outVlObj = pb.GetVal(0, c, r);
                //            if (outVlObj != null)
                //            {
                //                object adVl = convertVl(outVlObj);
                //                outArr.SetValue(adVl, c, r); 
                //            }
                //            else
                //            {
                //                //Console.WriteLine("outVlObj = null for r,c " + r.ToString() + ", " + c.ToString());
                //            }



                //        }
                //    }
                //    ((IPixelBlock3)pPixelBlock).set_PixelData(p, outArr);
                //}

            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of localMean Function. " + exc.Message, exc);
                Console.Write(exc.ToString());
                throw myExc;
            }
        }

        private object convertVl(object outVl)
        {
            //Console.WriteLine(myFunctionHelper.RasterInfo.PixelType.ToString());
            object newVl = 0;
            switch (convPType)
            {
                case rstPixelType.PT_CHAR:
                    newVl = System.Convert.ToSByte(outVl);
                    break;
                case rstPixelType.PT_LONG:
                    newVl = System.Convert.ToInt32(outVl);
                    break;
                case rstPixelType.PT_SHORT:
                    newVl = System.Convert.ToInt16(outVl);
                    break;
                case rstPixelType.PT_U1:
                    newVl = System.Convert.ToBoolean(outVl);
                    break;
                case rstPixelType.PT_U2:
                case rstPixelType.PT_U4:
                case rstPixelType.PT_UCHAR:
                    //Console.WriteLine("Converting Values");
                    newVl = System.Convert.ToByte(outVl);
                    break;
                case rstPixelType.PT_ULONG:
                    newVl = System.Convert.ToUInt32(outVl);
                    break;
                case rstPixelType.PT_USHORT:
                    newVl = System.Convert.ToUInt16(outVl);
                    break;
                default:
                    newVl = outVl;
                    break;
            }
            return newVl;
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of local Function", exc);
                throw myExc;
            }
        }
    }
}
