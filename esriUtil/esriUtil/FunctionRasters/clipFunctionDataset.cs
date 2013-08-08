using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil.FunctionRasters.NeighborhoodHelper;

namespace esriUtil.FunctionRasters
{
    class clipFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "clip Function"; // Name of the log Function.
        private string myDescription = "clips a raster to a given geometry"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster outrs = null;
        //private IEnvelope ext = null;
        private IGeometry geo = null;
        private esriRasterClippingType cType = esriRasterClippingType.esriRasterClippingOutside;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        private object noDataValue;
        public void Bind(object pArgument)
        {
            if (pArgument is clipFunctionArgument)
            {
                clipFunctionArgument args = (clipFunctionArgument)pArgument;
                inrs = args.InRaster;
                outrs = args.OutRaster;
                noDataValue = ((System.Array)((IRasterProps)outrs).NoDataValue).GetValue(0);
                geo = args.Geometry;
                cType = args.ClipType;
                myFunctionHelper.Bind(outrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: clipFunctonArguments");
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
                IPixelBlock3 pPixelBlock3 = (IPixelBlock3)pPixelBlock;
                IRaster2 inRs2 = (IRaster2)inrs;
                IRaster2 outRs2 = (IRaster2)outrs;
                double mx,my;
                outRs2.PixelToMap((int)pTlc.X,(int)pTlc.Y,out mx, out my);
                int clm,rw;
                inRs2.MapToPixel(mx,my,out clm,out rw);
                IPnt pntLoc = new PntClass();
                pntLoc.SetCoords(clm,rw);
                IPnt pntSize = new PntClass();
                pntSize.SetCoords(pPixelBlock.Width,pPixelBlock.Height);
                IPixelBlock3 pBIn = (IPixelBlock3)inrs.CreatePixelBlock(pntSize);
                inrs.Read(pntLoc,(IPixelBlock)pBIn);
                for (int p = 0; p < pPixelBlock.Planes; p++)
                {
                    pPixelBlock3.set_PixelData(p, pBIn.get_PixelData(p));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in clip Function", exc);
                throw myExc;
            }
        }
    }
}
