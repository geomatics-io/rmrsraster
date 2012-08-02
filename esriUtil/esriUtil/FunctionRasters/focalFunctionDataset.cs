﻿using System;
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
    public class focalFunctionDataset : IRasterFunction
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
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is FocalFunctionArguments)
            {
                FocalFunctionArguments args = (FocalFunctionArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                inop = args.Operation;
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
            double pixelValue = 0d;
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                if (inWindow == rasterUtil.windowType.RECTANGLE&&!(inop == rasterUtil.focalType.VARIANCE||inop==rasterUtil.focalType.STANDARD_DEVIATION))
                {
                    switch (inop)
                    {
                        case rasterUtil.focalType.MAX:
                            neighborhoodHelperMaxRectangle nHMax = new neighborhoodHelperMaxRectangle();
                            nHMax.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.MIN:
                            neighborhoodHelperMinRectangle nHMin = new neighborhoodHelperMinRectangle();
                            nHMin.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.SUM:
                            neighborhoodHelperSumRectangle nHSum = new neighborhoodHelperSumRectangle();
                            nHSum.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.MEAN:
                            neighborhoodHelperMeanRectangle nHMean = new neighborhoodHelperMeanRectangle();
                            nHMean.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.MODE:
                            neighborhoodHelperModeRectangle nHMode = new neighborhoodHelperModeRectangle();
                            nHMode.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.MEDIAN:
                            neighborhoodHelperMedianRectangle nHMed = new neighborhoodHelperMedianRectangle();
                            nHMed.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.VARIANCE:
                            neighborhoodHelperVarianceRectangle nHVar = new neighborhoodHelperVarianceRectangle();
                            nHVar.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.STANDARD_DEVIATION:
                            neighborhoodHelperStdRectangle nHSTD = new neighborhoodHelperStdRectangle();
                            nHSTD.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.UNIQUE:
                            neighborhoodHelperUniqueRectangle nHUniq = new neighborhoodHelperUniqueRectangle();
                            nHUniq.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        case rasterUtil.focalType.ENTROPY:
                            neighborhoodHelperEntropyRectangle nHEnt = new neighborhoodHelperEntropyRectangle();
                            nHEnt.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                        default:
                            neighborhoodHelperProbabilityRectangle nHProb = new neighborhoodHelperProbabilityRectangle();
                            nHProb.Read(pTlc, pRaster, pPixelBlock, clms, rws, orig);
                            break;
                    }
                }
                else
                {
                    int pBHeight = pPixelBlock.Height;
                    int pBWidth = pPixelBlock.Width;
                    IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                    IPnt pbBigSize = new PntClass();
                    IPnt pbBigLoc = new PntClass();
                    int pbBigWd = pBWidth + clms - 1;
                    int pbBigHt = pBHeight + rws - 1;
                    int r, l, t, b;
                    l = clms / 2;
                    t = rws / 2;
                    r = l;
                    b = t;
                    if (clms % 2 == 0)
                    {
                        r--;
                    }
                    if (rws % 2 == 0)
                    {
                        b--;
                    }
                    pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                    pbBigSize.SetCoords(pbBigWd, pbBigHt);
                    IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                    orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                    for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                    {
                        neighborhoodhelper nHelp = null;
                        if (inWindow == rasterUtil.windowType.CIRCLE)
                        {
                            nHelp = new neighborhoodhelper(radius);
                        }
                        else
                        {
                            nHelp = new neighborhoodhelper(clms, rws);
                        }
                        double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                        System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                        System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                        for (int i = t; i < pbBigHt - b; i++)
                        {
                            int rw = i - t;
                            for (int k = 0; k < pbBigWd; k++)
                            {
                                int cl = k - (clms - 1);
                                //Console.WriteLine("C R = " + cl.ToString() + " " + rw.ToString());
                                double[] rwsArr = new double[rws];
                                for (int s = (-1 * t); s <= b; s++)
                                {
                                    int ni = i + s;
                                    pixelValue = Convert.ToDouble(pixelValuesBig.GetValue(k, ni));
                                    if (pixelValue == noDataValue)
                                    {
                                        rwsArr[s + t] = 0;
                                    }
                                    rwsArr[s + t] = pixelValue;
                                }
                                nHelp.appendColumn(rwsArr);
                                if (cl < 0)
                                {
                                    continue;
                                }
                                //Console.WriteLine(inop);
                                switch (inop)
                                {
                                    case rasterUtil.focalType.MIN:
                                        pixelValue = nHelp.WindowMin;
                                        break;
                                    case rasterUtil.focalType.SUM:
                                        pixelValue = nHelp.WindowSum;
                                        break;
                                    case rasterUtil.focalType.MEAN:
                                        pixelValue = nHelp.WindowMean;
                                        break;
                                    case rasterUtil.focalType.MODE:
                                        pixelValue = nHelp.WindowMode;
                                        break;
                                    case rasterUtil.focalType.MEDIAN:
                                        pixelValue = nHelp.WindowMedian;
                                        break;
                                    case rasterUtil.focalType.VARIANCE:
                                        pixelValue = nHelp.WindowVariance;
                                        break;
                                    case rasterUtil.focalType.STANDARD_DEVIATION:
                                        pixelValue = nHelp.WindowStd;
                                        break;
                                    case rasterUtil.focalType.UNIQUE:
                                        pixelValue = nHelp.WindowUniqueValues;
                                        break;
                                    case rasterUtil.focalType.ENTROPY:
                                        pixelValue = nHelp.WindowEntropyValues;
                                        break;
                                    case rasterUtil.focalType.PROBABILITY:
                                        pixelValue = nHelp.WindowProbability;
                                        break;
                                    default:
                                        pixelValue = nHelp.WindowMax;
                                        break;
                                }
                                pixelValues.SetValue(pixelValue, cl, rw);
                            }
                        }
                        ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);
                    }
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of abs Function. " + exc.Message, exc);
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