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
    class combineFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "combine Function"; // Name of the log Function.
        private string myDescription = "creates a unique value raster given a multiband raster"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset outRs = null;
        private Dictionary<string,int> uniqueDic = new Dictionary<string,int>();
        private int uniqueCounter = 0;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        //private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        object noDataVl = null;
        public void Bind(object pArgument)
        {
            if (pArgument is combineFunctionArguments)
            {
                combineFunctionArguments args = (combineFunctionArguments)pArgument;

                inrs = args.InRasterDataset;
                outRs = args.OutRaster;
                myFunctionHelper.Bind(outRs);
                //myFunctionHelperCoef.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
                object tnd = myRasterInfo.NoData;
                if(tnd==null)
                {
                }
                else
                {
                    noDataVl = ((System.Array)myRasterInfo.NoData).GetValue(0);
                }
                
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: combineFunctonArguments");
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
                //IPixelBlock3 inVlsPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);
                //myFunctionHelperCoef.Read(pTlc,null,myFunctionHelperCoef.Raster, (IPixelBlock)inVlsPb);
                IPixelBlock3 outPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array outArr = (System.Array)outPixelBlock.get_PixelData(0);
                System.Array[] inArr = new System.Array[inrs.RasterInfo.BandCount];
                IRasterBandCollection rsBc = (IRasterBandCollection)inrs;
                for (int b = 0; b < inrs.RasterInfo.BandCount; b++)
			    {
                    IRasterBand rsB = rsBc.Item(b);
                    IRawPixels rPix = (IRawPixels)rsB;
                    IPixelBlock pb = rPix.CreatePixelBlock(pbSize);
                    rPix.Read(pTlc,pb);
                    inArr[b] = (System.Array)pb.get_SafeArray(b);
			    }
                updateOutArr(outPixelBlock, inArr, outArr);
                outPixelBlock.set_PixelData(0, outArr);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void updateOutArr(IPixelBlock3 outPb, System.Array[] inArr, System.Array outArr)
        {
            
            int bands = inArr.Length;
            string[] sArr = new string[bands];
            System.Array[] vlArr = new System.Array[bands];
            rstPixelType pTy = outPb.get_PixelType(0);
            for (int r = 0; r < outPb.Height; r++)
            {
                for (int c = 0; c < outPb.Width; c++)
                {
                    bool ch = true;
                    for (int p = 0; p < bands; p++)
                    {
                        object inObj = inArr[p].GetValue(c, r);
                        if (inObj == null)
                        {
                            ch = false;
                            break;
                        }
                        else
                        {
                            sArr[p] = inObj.ToString();
                        }
                    }
                    if (ch)
                    {
                        int vl = uniqueCounter;
                        string sVls = String.Join(",", sArr);
                        if (!uniqueDic.TryGetValue(sVls, out vl))
                        {
                            uniqueCounter += 1;
                            vl = uniqueCounter;
                            uniqueDic.Add(sVls, vl);
                        }
                        object newVl = rasterUtil.getSafeValue(vl, pTy);
                        outArr.SetValue(newVl, c, r);
                    }
                }
            }
        }

        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of combine Function", exc);
                throw myExc;
            }
        }
    }
}

