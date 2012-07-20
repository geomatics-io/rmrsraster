﻿using System;
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
                        int l, t;
                        l = clms / 2;
                        t = rws / 2;
                        int pbBigWd = pBWidth + clms;// -1;
                        int pbBigHt = pBHeight + rws;// -1;
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
                            for (int r = 0; r < pBHeight; r++)//coordinates in terms of the small pixel block
                            {
                                int er = r + rws;
                                for (int c = 0; c < pBWidth; c++)
                                {
                                    int ec = c + clms;
                                    
                                    Dictionary<int, int[]> uDic = findUniqueRegions.getUniqueRegions(pixelValuesBig, ec,er,clms,rws,c,r, noDataValue); //key(int) = cell value value(int[2] = number of cells and number of edges)  
                                   
                                    double uniqueMax = findUniqueRegionsValue(uDic);
                                    
                                    pixelValues.SetValue(uniqueMax, c, r);
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
                        List<int[]> iter = new List<int[]>();
                        int[,] circleWindow = rasterUtil.createFocalWidow(clms, clms, wd,out iter);
                        System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                        int pBHeight = pPixelBlock.Height;
                        int pBWidth = pPixelBlock.Width;
                        IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                        IPnt pbBigSize = new PntClass();
                        IPnt pbBigLoc = new PntClass();
                        int pbBigWd = pBWidth + clms;
                        int pbBigHt = pBHeight + rws;
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
                            for (int r = 0; r < pBHeight; r++)
                            {
                                int er = r + rws;
                                for (int c = 0; c < pBWidth; c++)
                                {
                                    int ec = c + clms;
                                    
                                    Dictionary<int, int[]> uDic = findUniqueRegions.getUniqueRegions(pixelValuesBig, ec, er,clms,rws,c,r, noDataValue, circleWindow); //key(int) = cell value value(int[2] = number of cells and number of edges)  
                                    double uniqueMax = findUniqueRegionsValue(uDic);

                                    
                                    try
                                    {
                                        pixelValues.SetValue(uniqueMax, c, r);
                                    }
                                    catch
                                    {
                                        pixelValues.SetValue(noDataValue, c, r);
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
        public abstract double findUniqueRegionsValue(Dictionary<int,int[]> uniqueDic);
    }
}
