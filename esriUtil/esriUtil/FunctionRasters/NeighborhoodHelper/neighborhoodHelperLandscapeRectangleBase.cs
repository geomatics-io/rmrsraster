using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public abstract class neighborhoodHelperLandscapeRectangleBase
    {
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock, int clms, int rws, IRaster orig, rasterUtil.windowType wd)
        {
            try
            {
                if (wd == rasterUtil.windowType.RECTANGLE)
                {
                    try
                    {
                        System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
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
                        pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                        pbBigSize.SetCoords(pbBigWd, pbBigHt);
                        IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                        orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                        object[,] clmsValues = new object[pBWidth, pBHeight];
                        for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                        {
                            double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                            System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                            System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                            for (int r = rws; r < pbBigHt; r++)
                            {
                                int nr = r - rws;
                                for (int c = clms; c < pbBigWd; c++)
                                {
                                    int nc = c - clms;
                                    double[,] windowArr = new double[clms, rws];
                                    for (int c2 = 0; c2 < clms; c2++)
                                    {
                                        int c2p = nc + c2;
                                        for (int r2 = 0; r2 < rws; r2++)
                                        {
                                            int r2p = nr + r2;
                                            double inVl = System.Convert.ToDouble(pixelValuesBig.GetValue(c2p, r2p));
                                            if (Double.IsInfinity(inVl) || Double.IsNaN(inVl) || inVl == noDataValue)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                windowArr[c2, r2] = inVl;
                                            }
                                        }
                                    }
                                    double uniqueMax = findUniqueRegionsValue(windowArr, noDataValue);
                                    pixelValues.SetValue(uniqueMax, nc, nr);
                                }
                            }
                            try
                            {
                                ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);
                            }
                            catch
                            {
                                ((IPixelBlock3)pPixelBlock).set_PixelData(nBand,noDataValue);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Exception myExc = new System.Exception("Exception caught in neighborhood landscape base rectangle helper Function. " + e.Message, e);
                        throw myExc;
                    }
                }
                else
                {
                    try
                    {
                        int[,] circleWindow = rasterUtil.createFocalWidow(clms, clms, wd);
                        System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
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
                        pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                        pbBigSize.SetCoords(pbBigWd, pbBigHt);
                        IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                        orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                        object[,] clmsValues = new object[pBWidth, pBHeight];
                        for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                        {
                            double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                            System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                            System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                            for (int r = rws; r < pbBigHt; r++)
                            {
                                int nr = r - rws;
                                for (int c = clms; c < pbBigWd; c++)
                                {
                                    int nc = c - clms;
                                    double[,] windowArr = new double[clms, rws];
                                    for (int c2 = 0; c2 < clms; c2++)
                                    {
                                        int c2p = nc + c2;
                                        for (int r2 = 0; r2 < rws; r2++)
                                        {
                                            int r2p = nr + r2;
                                            int cVl = circleWindow[c2, r2];
                                            if (cVl == 0)
                                            {
                                                windowArr[c2, r2] = noDataValue;
                                            }
                                            else
                                            {
                                                double inVl = System.Convert.ToDouble(pixelValuesBig.GetValue(c2p, r2p));
                                                if (Double.IsInfinity(inVl) || Double.IsNaN(inVl)||inVl==noDataValue)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    windowArr[c2, r2] = inVl;
                                                }
                                                windowArr[c2, r2] = inVl;
                                            }
                                        }
                                    }
                                    double uniqueMax = findUniqueRegionsValue(windowArr, noDataValue);
                                    try
                                    {
                                        pixelValues.SetValue(uniqueMax, nc, nr);
                                    }
                                    catch
                                    {
                                        pixelValues.SetValue(noDataValue, nc, nr);
                                    }
                                }
                            }
                            ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Exception myExc = new System.Exception("Exception caught in neighborhood landscape base circle helper Function. " + e.Message, e);
                        throw myExc;
                    }
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in neighborhood landscape base helper Function. " + exc.Message, exc);
                throw myExc;
            }
        }

        public abstract double findUniqueRegionsValue(double[,] windowArr, double noDataValue);
    }
}
