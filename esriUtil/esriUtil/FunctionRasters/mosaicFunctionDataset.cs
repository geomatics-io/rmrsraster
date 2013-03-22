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
    class mosaicFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "mosaic Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using mosaic transformation"; // Description of the log Function.
        private IRaster outrs = null;
        private IRaster[] inrs = null;
        private IMosaicRaster mos = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is mosaicFunctionArguments)
            {
                mosaicFunctionArguments arg = (mosaicFunctionArguments)pArgument;
                inrs = arg.InRasterArr;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                mos = arg.Mosaic;
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: mergeFunctionArguments");
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


                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                IRaster mosRs = (IRaster)mos;
                IPnt pntSize = new PntClass();
                pntSize.SetCoords(pPixelBlock.Width, pPixelBlock.Height);
                IPixelBlock pb = mosRs.CreatePixelBlock(pntSize);
                mosRs.Read(pTlc, pb);
                for (int i = 0; i < pb.Planes; i++)
                {
                    pPixelBlock.set_SafeArray(i, pb.get_SafeArray(i));
                }


            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of mosaic Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of merge Function", exc);
                throw myExc;
            }
        }
    }
}