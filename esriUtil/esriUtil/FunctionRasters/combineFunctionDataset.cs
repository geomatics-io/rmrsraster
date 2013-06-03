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
        private IRaster inrs = null;
        private IRaster outRs = null;
        private Dictionary<string,int> uniqueDic = new Dictionary<string,int>();
        private int uniqueCounter = 0;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is combineFunctionArguments)
            {
                combineFunctionArguments args = (combineFunctionArguments)pArgument;
                IRaster[] rsArr = args.InRaster;
                inrs = new RasterClass();
                IRasterBandCollection rsBc = (IRasterBandCollection)inrs;
                foreach (IRaster rs in rsArr)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                outRs = args.outRaster;
                myFunctionHelper.Bind(outRs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
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
                IPixelBlock3 inVlsPb = (IPixelBlock3)inrs.CreatePixelBlock(pbSize);
                inrs.Read(pTlc, (IPixelBlock)inVlsPb);
                IPixelBlock3 outPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array outArr = (System.Array)outPixelBlock.get_PixelData(0);
                updateOutArr(ref outArr, ref inVlsPb);
                outPixelBlock.set_PixelData(0, outArr);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void updateOutArr(ref System.Array outArr, ref IPixelBlock3 inVlsPb)
        {
            string[] sArr = new string[inVlsPb.Planes];
            for (int r = 0; r < inVlsPb.Height; r++)
            {
                for (int c = 0; c < inVlsPb.Width; c++)
                {
                    for (int p = 0; p < inVlsPb.Planes; p++)
                    {
                        object inObj = inVlsPb.GetVal(p, c, r);
                        if (inObj == null)
                        {
                            sArr[p] = "null";
                        }
                        else
                        {
                            sArr[p] = inObj.ToString();
                        }
                    }
                    
                    int vl = uniqueCounter;
                    string sVls = String.Join(",",sArr);
                    if(!uniqueDic.TryGetValue(sVls,out vl))
                    {
                        uniqueCounter+=1;
                        vl = uniqueCounter;
                        uniqueDic.Add(sVls,vl);
                    }
                    outArr.SetValue(vl,c,r);
                    

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

