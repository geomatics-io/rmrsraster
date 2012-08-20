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
    class landscapeFunctionDataset: IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "focal Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using focal analysis"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster orig = null;
        private int clms, rws, radius;
        private rasterUtil.windowType inWindow = rasterUtil.windowType.RECTANGLE;
        private rasterUtil.focalType inop = rasterUtil.focalType.SUM;
        private rasterUtil.landscapeType landType = rasterUtil.landscapeType.AREA;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is LandscapeFunctionArguments)
            {
                LandscapeFunctionArguments args = (LandscapeFunctionArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                inop = args.Operation;
                landType = args.LandscapeType;
                inWindow = args.WindowType;
                clms = args.Columns;
                rws = args.Rows;
                radius = args.Radius;
                IRasterProps rsProp = (IRasterProps)inrs;
                myFunctionHelper.Bind(inrs);
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
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                switch (landType)
                {
                    #region Area
                    case rasterUtil.landscapeType.AREA:
                        switch (inop)
                        {
                            case rasterUtil.focalType.MAX:
                                neighborhoodHelperLandscapeMaxAreaRectangle nHMax = new neighborhoodHelperLandscapeMaxAreaRectangle();
                                nHMax.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig,inWindow);
                                break;
                            case rasterUtil.focalType.MIN:
                                neighborhoodHelperLandscapeMinAreaRectangle nHMin = new neighborhoodHelperLandscapeMinAreaRectangle();
                                nHMin.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.SUM:
                                neighborhoodHelperLandscapeSumAreaRectangle nHSum = new neighborhoodHelperLandscapeSumAreaRectangle();
                                nHSum.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEAN:
                                neighborhoodHelperLandscapeMeanAreaRectangle nHMean = new neighborhoodHelperLandscapeMeanAreaRectangle();
                                nHMean.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MODE:
                                neighborhoodHelperLandscapeModeAreaRectangle nHMode = new neighborhoodHelperLandscapeModeAreaRectangle();
                                nHMode.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEDIAN:
                                neighborhoodHelperLandscapeMedianAreaRectangle nHMed = new neighborhoodHelperLandscapeMedianAreaRectangle();
                                nHMed.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.VARIANCE:
                                neighborhoodHelperLandscapeVarianceAreaRectangle nHVar = new neighborhoodHelperLandscapeVarianceAreaRectangle();
                                nHVar.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.STD:
                                neighborhoodHelperLandscapeStdAreaRectangle nHSTD = new neighborhoodHelperLandscapeStdAreaRectangle();
                                nHSTD.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.UNIQUE:
                                neighborhoodHelperLandscapeUniqueAreaRectangle nHUniq = new neighborhoodHelperLandscapeUniqueAreaRectangle();
                                nHUniq.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.ENTROPY:
                                neighborhoodHelperLandscapeEntropyAreaRectangle nHEnt = new neighborhoodHelperLandscapeEntropyAreaRectangle();
                                nHEnt.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            default:
                                neighborhoodHelperLandscapeProbabilityAreaRectangle nHProb = new neighborhoodHelperLandscapeProbabilityAreaRectangle();
                                nHProb.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                        }
                        break;
                    #endregion
                    #region edge
                    case rasterUtil.landscapeType.EDGE:
                        switch (inop)
                        {
                            case rasterUtil.focalType.MAX:
                                neighborhoodHelperLandscapeMaxEdgeRectangle nHMax = new neighborhoodHelperLandscapeMaxEdgeRectangle();
                                nHMax.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MIN:
                                neighborhoodHelperLandscapeMinEdgeRectangle nHMin = new neighborhoodHelperLandscapeMinEdgeRectangle();
                                nHMin.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.SUM:
                                neighborhoodHelperLandscapeSumEdgeRectangle nHSum = new neighborhoodHelperLandscapeSumEdgeRectangle();
                                nHSum.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEAN:
                                neighborhoodHelperLandscapeMeanEdgeRectangle nHMean = new neighborhoodHelperLandscapeMeanEdgeRectangle();
                                nHMean.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MODE:
                                neighborhoodHelperLandscapeModeEdgeRectangle nHMode = new neighborhoodHelperLandscapeModeEdgeRectangle();
                                nHMode.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEDIAN:
                                neighborhoodHelperLandscapeMedianEdgeRectangle nHMed = new neighborhoodHelperLandscapeMedianEdgeRectangle();
                                nHMed.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.VARIANCE:
                                neighborhoodHelperLandscapeVarianceEdgeRectangle nHVar = new neighborhoodHelperLandscapeVarianceEdgeRectangle();
                                nHVar.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.STD:
                                neighborhoodHelperLandscapeStdEdgeRectangle nHSTD = new neighborhoodHelperLandscapeStdEdgeRectangle();
                                nHSTD.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.UNIQUE:
                                neighborhoodHelperLandscapeUniqueEdgeRectangle nHUniq = new neighborhoodHelperLandscapeUniqueEdgeRectangle();
                                nHUniq.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.ENTROPY:
                                neighborhoodHelperLandscapeEntropyEdgeRectangle nHEnt = new neighborhoodHelperLandscapeEntropyEdgeRectangle();
                                nHEnt.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            default:
                                neighborhoodHelperLandscapeProbabilityEdgeRectangle nHProb = new neighborhoodHelperLandscapeProbabilityEdgeRectangle();
                                nHProb.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                        }
                        break;
                    #endregion
                    #region ratio
                    case rasterUtil.landscapeType.RATIO:
                        switch (inop)
                        {
                            case rasterUtil.focalType.MAX:
                                neighborhoodHelperLandscapeMaxRatioRectangle nHMax = new neighborhoodHelperLandscapeMaxRatioRectangle();
                                nHMax.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MIN:
                                neighborhoodHelperLandscapeMinRatioRectangle nHMin = new neighborhoodHelperLandscapeMinRatioRectangle();
                                nHMin.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.SUM:
                                 
                                neighborhoodHelperLandscapeSumRatioRectangle nHSum = new neighborhoodHelperLandscapeSumRatioRectangle();
                                nHSum.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEAN:
                                neighborhoodHelperLandscapeMeanRatioRectangle nHMean = new neighborhoodHelperLandscapeMeanRatioRectangle();
                                nHMean.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MODE:
                                neighborhoodHelperLandscapeModeRatioRectangle nHMode = new neighborhoodHelperLandscapeModeRatioRectangle();
                                nHMode.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.MEDIAN:
                                neighborhoodHelperLandscapeMedianRatioRectangle nHMed = new neighborhoodHelperLandscapeMedianRatioRectangle();
                                nHMed.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.VARIANCE:
                                neighborhoodHelperLandscapeVarianceRatioRectangle nHVar = new neighborhoodHelperLandscapeVarianceRatioRectangle();
                                nHVar.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.STD:
                                neighborhoodHelperLandscapeStdRatioRectangle nHSTD = new neighborhoodHelperLandscapeStdRatioRectangle();
                                nHSTD.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.UNIQUE:
                                neighborhoodHelperLandscapeUniqueRatioRectangle nHUniq = new neighborhoodHelperLandscapeUniqueRatioRectangle();
                                nHUniq.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            case rasterUtil.focalType.ENTROPY:
                                neighborhoodHelperLandscapeEntropyRatioRectangle nHEnt = new neighborhoodHelperLandscapeEntropyRatioRectangle();
                                nHEnt.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                            default:
                                neighborhoodHelperLandscapeProbabilityRatioRectangle nHProb = new neighborhoodHelperLandscapeProbabilityRatioRectangle();
                                nHProb.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                                break;
                        }
                        break;
                    #endregion
                    default:
                        neighborhoodHelperLandscapeUniqueRegionsRectangle nHReg = new neighborhoodHelperLandscapeUniqueRegionsRectangle();
                        nHReg.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig, inWindow);
                        break;
                        
                }
                        

                
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of landscape Function. " + exc.Message, exc);
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }
    }
}

