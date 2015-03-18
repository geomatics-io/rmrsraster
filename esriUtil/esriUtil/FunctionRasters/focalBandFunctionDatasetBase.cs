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
    public abstract class focalBandFunctionDatasetBase : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Local Standard Deviation Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using local Standard Deviation value transformation"; // Description of the log Function.
        private IFunctionRasterDataset inrsBands = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public int bandsBefore = 1;
        public int bandsAfter = 1;
        public int tBands = 3;
        public void Bind(object pArgument)
        {
            if (pArgument is focalBandFunctionArguments)
            {
                focalBandFunctionArguments arg = (focalBandFunctionArguments)pArgument;
                inrsBands = arg.InRaster;
                myFunctionHelper.Bind(inrsBands);
                bandsBefore = arg.BandsBefore;
                bandsAfter = arg.BandsAfter;
                tBands = 1 + bandsBefore + bandsAfter;
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: focalBandArguments");
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
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] outArr = new System.Array[pPixelBlock.Planes];
                int pbHeight = ipPixelBlock.Height;
                int pbWidth = ipPixelBlock.Width;
                for (int p = 0; p < ipPixelBlock.Planes; p++)
                {
                    outArr[p] = (System.Array)ipPixelBlock.get_PixelData(p);
                }

                for (int r = 0; r < pbHeight; r++)
                {
                    for (int c = 0; c < pbWidth; c++)
                    {
                        object vlObj = ipPixelBlock.GetVal(0, c, r);
                        if (vlObj != null)
                        {
                            getOutPutVl(outArr, c, r);
                        }

                    }
                }
                for (int p = 0; p < ipPixelBlock.Planes; p++)
                {
                    ipPixelBlock.set_PixelData(p, outArr[p]);
                }


            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of focal band Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of focal band Function", exc);
                throw myExc;
            }
        }

        public abstract void getOutPutVl(System.Array[] outArr, int c, int r);

    }
}
