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
    public abstract class glcmFunctionDataset2 : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "focal Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using focal analysis"; // Description of the log Function.
        private IRaster inrs = null;
        private int clms, rws, radius;
        public int Columns { get { return clms; } }
        public int Rows { get { return rws; } }
        public int Radius { get { return radius; } }
        public List<int[]> iter = null;
        private bool horizontal = true;
        public bool Horizontal { get { return horizontal; } }
        private rasterUtil.windowType inWindow = rasterUtil.windowType.RECTANGLE;
        public rasterUtil.windowType WindowType { get { return inWindow; } }
        public rasterUtil.glcmMetric glcmMetrics = rasterUtil.glcmMetric.CONTRAST;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataValue = Single.MinValue;
        public void Bind(object pArgument)
        {
            if (pArgument is glcmFunctionArguments)
            {
                glcmFunctionArguments args = (glcmFunctionArguments)pArgument;
                inrs = args.OriginalRaster;
                inWindow = args.WindowType;
                clms = args.Columns;
                rws = args.Rows;
                radius = args.Radius;
                glcmMetrics = args.GLCMMETRICS;
                horizontal = args.Horizontal;
                IRasterProps rsProp = (IRasterProps)inrs;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: glcmFunctonArguments");
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
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                getTransformedValue(ref pPixelBlock);
                
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }
        public abstract void getTransformedValue(ref IPixelBlock pb);
    }
}
