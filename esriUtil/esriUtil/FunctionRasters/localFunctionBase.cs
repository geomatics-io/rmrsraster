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
    public abstract class localFunctionBase: IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Local Standard Deviation Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using local Standard Deviation value transformation"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster inrsBands = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataVl = 0;
        public System.Array noDataValueArr; 
        public void Bind(object pArgument)
        {
            if (pArgument is LocalFunctionArguments)
            {
                LocalFunctionArguments arg = (LocalFunctionArguments)pArgument;
                inrsBands = arg.InRaster;
                inrs = arg.outRaster;
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
                noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                noDataVl = System.Convert.ToSingle(noDataValueArr.GetValue(0));
                noDataValueArr = (System.Array)((IRasterProps)inrsBands).NoDataValue;
                // Call Read method of the Raster Function Helper object.
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)inrsBands.CreatePixelBlock(pbSize);
                inrsBands.Read(pTlc, (IPixelBlock)outPb);
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                List<System.Array> pArr = new List<System.Array>();
                System.Array outArr = (System.Array)ipPixelBlock.get_PixelData(0);
                for (int nBand = 0; nBand < outPb.Planes; nBand++)
                {
                    System.Array pixelValues = (System.Array)(outPb.get_PixelData(nBand));
                    pArr.Add(pixelValues);
                }
                updateOutArr(ref outArr, ref pArr);
                ipPixelBlock.set_PixelData(0, outArr);

            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of localMean Function. " + exc.Message, exc);
                throw myExc;
            }
        }

        public abstract void updateOutArr(ref System.Array outArr, ref List<System.Array> pArr);
        
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
       
    }
}