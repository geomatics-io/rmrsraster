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
    class regionGroupFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Region Group Function"; // Name of the log Function.
        private string myDescription = "defines regions"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrs = null;
        private IRasterProps rsProp = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperInput = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        private int width = 0;
        private int height = 0;

        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is regionGroupFunctionArguments)
            {
                regionGroupFunctionArguments arg = (regionGroupFunctionArguments)pArgument;
                inrs = arg.InRaster;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                rsProp = (IRasterProps)outrs;
                width = rsProp.Width;
                height = rsProp.Height;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperInput.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: randomForestArguments");
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
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 inputPb = (IPixelBlock3)myFunctionHelperInput.Raster.CreatePixelBlock(pbSize);//independent variables  
                myFunctionHelperInput.Read(pTlc, null, myFunctionHelperInput.Raster, (IPixelBlock)inputPb);
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array pixelValues = (System.Array)ipPixelBlock.get_PixelData(0);
                createRegions(inputPb, pixelValues);
                ipPixelBlock.set_PixelData(0,pixelValues);
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of the Region Group function Function. " + exc.Message, exc);
                Console.WriteLine(exc.ToString());
            }
        }
        int regionCounter = 1;
        private void createRegions(IPixelBlock3 inputPb, System.Array outputPixelValues,int sClms = 0, int sRws = 0)
        {
            int clms = inputPb.Width;
            int rws = inputPb.Height;
            for (int r = sRws; r < rws; r++)
            {
                for (int c = sClms; c < clms; c++)
                {
                    object vlObj = inputPb.GetVal(0, c, r);
                    if (vlObj == null) outputPixelValues.SetValue(vlObj,c,r);
                    else
                    {
                        int vlObj2 = System.Convert.ToInt32(outputPixelValues.GetValue(c,r));
                        if (vlObj2 == 0)
                        {
                            outputPixelValues.SetValue(regionCounter, c, r);
                            int vl = System.Convert.ToInt32(vlObj);
                            List<int[]> rwClmCheckLst = new List<int[]>();
                            checkNeighbors(vl, inputPb, outputPixelValues, c, r, rwClmCheckLst);
                            while (rwClmCheckLst.Count > 0)
                            {
                                int[] clmRw = rwClmCheckLst[0];
                                checkNeighbors(vl, inputPb, outputPixelValues, clmRw[0], clmRw[1], rwClmCheckLst);
                                rwClmCheckLst.RemoveAt(0);
                            }
                            regionCounter++;

                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private void checkNeighbors(int vl, IPixelBlock3 inputPb, System.Array outputPixelValues, int c, int r, List<int[]> rwClmCheckLst)
        {
            int lastWidthcell = inputPb.Width;
            int lastHeightcell = inputPb.Height;
            int clmPlus = c+1;
            int clmMinus = c-1;
            int rwPlus = r+1;
            int rwMinus = r-1;
            if (clmPlus < lastWidthcell)
            {
                lookAtNeighbor(vl, inputPb, outputPixelValues, clmPlus, r, rwClmCheckLst);
            }
            else
            {
            }
            if (clmMinus >= 0)
            {
                lookAtNeighbor(vl, inputPb, outputPixelValues, clmMinus, r, rwClmCheckLst);
            }
            else
            {
                
            }
            if (rwPlus < lastHeightcell)
            {
                lookAtNeighbor(vl, inputPb, outputPixelValues, c, rwPlus, rwClmCheckLst);
            }
            else
            {
            }
            if (rwMinus >= 0)
            {
                lookAtNeighbor(vl, inputPb, outputPixelValues, c, rwMinus, rwClmCheckLst);
            }
            else
            {
            }

            
        }

        private void lookAtNeighbor(int vl, IPixelBlock3 inputPb, System.Array outputPixelValues, int c, int r, List<int[]> rwClmCheckLst)
        {
            int nVl = System.Convert.ToInt32(outputPixelValues.GetValue(c, r));
            if (nVl == 0)
            {
                object nClusVlobj = inputPb.GetVal(0, c, r);
                if (nClusVlobj == null) outputPixelValues.SetValue(nClusVlobj, c, r);
                else
                {
                    int nClusVl = System.Convert.ToInt32(nClusVlobj);
                    if (nClusVl == vl)
                    {
                        outputPixelValues.SetValue(regionCounter, c, r);
                        rwClmCheckLst.Add(new int[] { c, r });
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of RegionGroup Function", exc);
                throw myExc;
            }
        }
    }
}