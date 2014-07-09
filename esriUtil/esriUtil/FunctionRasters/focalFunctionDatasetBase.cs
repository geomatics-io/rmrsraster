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
    public abstract class focalFunctionDatasetBase:IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "focal Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using focal analysis"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset orig = null;
        public int clms, rws, radius;
        public float windowN = 9;
        public int[][] lsiter = null;
        public List<int[]> iter = null;
        public rasterUtil.windowType inWindow = rasterUtil.windowType.RECTANGLE;
        private rasterUtil.focalType inop = rasterUtil.focalType.SUM;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperOrig = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataValue = Single.MinValue; 
        public void Bind(object pArgument)
        {
            if (pArgument is FocalFunctionArguments)
            {
                FocalFunctionArguments args = (FocalFunctionArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                inop = args.Operation;
                inWindow = args.WindowType;
                lsiter = args.Fastiter;
                windowN = args.WindowCount;
                //Console.WriteLine(lsiter.Count());
                clms = args.Columns;
                rws = args.Rows;
                radius = args.Radius;
                myFunctionHelper.Bind(inrs);
                myFunctionHelperOrig.Bind(orig);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: FocalFunctonArguments");
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
            //try
            //{
                //System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPnt pbBigSize = new PntClass();
                IPnt pbBigLoc = new PntClass();
                int pbBigWd = pBWidth + clms - 1;
                int pbBigHt = pBHeight + rws - 1;
                int l, t;
                l = clms / 2;
                t = rws / 2;
                //Console.WriteLine("lt = " + (pTlc.X - l).ToString() + ":" + (pTlc.Y - t).ToString());
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                IPixelBlock3 pbBig = (IPixelBlock3)myFunctionHelperOrig.Raster.CreatePixelBlock(pbBigSize);
                myFunctionHelperOrig.Read(pbBigLoc, null, myFunctionHelperOrig.Raster, (IPixelBlock)pbBig);
                updatePixelRectangle(ipPixelBlock, pbBig);
            //}
            //catch (Exception e)
            //{
                //Console.WriteLine(e.ToString());
                //System.Windows.Forms.MessageBox.Show(e.ToString());
                //Console.WriteLine(pTlc.X.ToString() + ":" + pTlc.Y.ToString());

            //}
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
        public abstract void updatePixelRectangle(IPixelBlock3 pbIn, IPixelBlock3 pbInBig);
    }
}
