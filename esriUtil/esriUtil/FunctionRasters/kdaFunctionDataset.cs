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
    public class kdaFunctionDataset: IRasterFunction
    {
        public kdaFunctionDataset()
        {
        }
        private IRasterInfo myRasterInfo; // Raster Info for the Tobit Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the Tobit Function.
        private string myName = "GLM Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using GLM transformation"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBandsCoef = null;
        private Statistics.dataPrepDiscriminantAnalysis kda = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is kdaFunctionArguments)
            {
                kdaFunctionArguments arg = (kdaFunctionArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                kda = arg.KDAModel;
                outrs = arg.OutRaster;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBandsCoef);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: regressionFunctionArguments");
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
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);
                myFunctionHelperCoef.Read(pTlc,null,myFunctionHelperCoef.Raster, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array outArr = (System.Array)ipPixelBlock.get_PixelData(0);
                double[] vArr = new double[outPb.Planes];
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        bool checkVl = true;
                        for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                        {
                            object vlObj = outPb.GetVal(0,k,i);
                            if(vlObj==null)
                            {
                                checkVl = false;
                                break;
                            }
                            vArr[coefnBand] = System.Convert.ToDouble(vlObj);
                        }
                        if(checkVl)
                        {
                            double tVl = kda.computeNew(vArr);
                            outArr.SetValue(System.Convert.ToSingle(tVl), k, i);
                        }
                        
                    }
                }
                ipPixelBlock.set_PixelData(0, outArr);
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of GLM Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }
    }
}