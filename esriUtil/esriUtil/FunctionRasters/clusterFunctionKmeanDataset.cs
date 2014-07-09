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
    class clusterFunctionKmeanDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Cluster Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using Cluster transformation"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBandsCoef = null;
        private Statistics.dataPrepClusterKmean cluster = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is clusterFunctionArguments)
            {
                clusterFunctionArguments arg = (clusterFunctionArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                cluster = (Statistics.dataPrepClusterKmean)arg.ClusterModel;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBandsCoef);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: plrFunctionArguments");
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

                //System.Array noDataValueArr = (System.Array)((IRasterProps)inrsBandsCoef).NoDataValue;
                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                //Console.WriteLine("After Read");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);//independent variables  
                myFunctionHelperCoef.Read(pTlc,null,myFunctionHelperCoef.Raster, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                //System.Array[] pArr = new System.Array[outPb.Planes];
                //for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                //{
                //    System.Array pixelValues = (System.Array)(outPb.get_PixelData(coefnBand));
                //    pArr[coefnBand] = pixelValues;
                //}
                System.Array pValues = (System.Array)ipPixelBlock.get_PixelData(0);//(System.Array)(td);
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        double[] xVls = new double[outPb.Planes];
                        bool ndT = true;
                        for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                        {
                            //float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(coefnBand));
                            object pObj = outPb.GetVal(coefnBand, k, i);
                            
                            if (pObj==null)
                            {
                                ndT = false;
                                break;
                            }
                            float pixelValue = Convert.ToSingle(pObj);
                            xVls[coefnBand] = pixelValue;
                        }
                        if (ndT)
                        {
                            int c = cluster.computNew(xVls);
                            pValues.SetValue(c, k, i);
                        }

                    }
                }
                ipPixelBlock.set_PixelData(0, pValues);
                
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of Cluster Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of Cluster Function", exc);
                throw myExc;
            }
        }
    }
}
