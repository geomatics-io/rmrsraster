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
    class PixelBlockToRasterFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "PixelBlock to Raster Function"; // Name of the log Function.
        private string myDescription = "Converts a pixel block to a Raster"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        IPnt tlLoc = null;
        IPixelBlock3 vPb = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is PixelBlockToRasterFunctionArguments)
            {
                PixelBlockToRasterFunctionArguments args = (PixelBlockToRasterFunctionArguments)pArgument;
                inrs = args.InRaster;
                vPb = (IPixelBlock3)args.ValuePixelBlock;
                tlLoc = args.TopLeft;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: PixelBlockToRasterArguments");
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
                IPnt pntSize = new PntClass();
                pntSize.SetCoords(pPixelBlock.Width,pPixelBlock.Height);
                IPixelBlock3 pb3 = (IPixelBlock3)pPixelBlock;
                for (int nBand = 0; nBand < pPixelBlock.Planes; nBand++)
                {
                    //float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                    object dArr = vPb.get_PixelData(nBand);
                    pb3.set_PixelData(nBand,dArr);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method PixelBlock To Raster Function. " + exc.Message, exc);
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
    }
}