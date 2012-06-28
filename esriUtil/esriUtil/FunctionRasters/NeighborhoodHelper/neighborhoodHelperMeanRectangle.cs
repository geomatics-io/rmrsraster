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
    class neighborhoodHelperMeanRectangle
    {
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock, int clms, int rws, IRaster orig)
        {
            double clmsSum = 0;
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
                int n = clms * rws;
                pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                double[,] clmsValues = new double[pbBigWd, pBHeight];
                for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                {
                    double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    //set the first clmValues row and row of output dataset
                    clmsSum = 0;
                    for (int j = 0; j < pbBigWd; j++)
                    {
                        int pclms = j - clms;
                        int cl = pclms + 1;
                        double rwsSum = 0;
                        for (int i = 0; i < rws; i++)
                        {
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(j, i));
                            if (bVl == noDataValue) bVl = 0;
                            rwsSum += bVl;
                        }
                        clmsValues.SetValue(rwsSum, j, 0);
                        clmsSum += rwsSum;
                        if (cl >= 0)
                        {
                            if (cl > 0)
                            {
                                double oldClmsValues = clmsValues[pclms, 0];
                                clmsSum = clmsSum - oldClmsValues;
                            }
                            double mean = clmsSum / n;
                            pixelValues.SetValue(mean, cl, 0);
                        }
                    }
                    //set first clm of output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsSum = 0;
                        int rwp = i - rws;
                        int rw = rwp + 1;
                        for (int k = 0; k < clms; k++)
                        {
                            double pRwsSum = clmsValues[k, rwp];
                            double vlRwT = System.Convert.ToDouble(pixelValuesBig.GetValue(k, rwp));
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(k, i));
                            if (vlRwT == noDataValue) vlRwT = 0;
                            if (bVl == noDataValue) bVl = 0;
                            double rwsSum = pRwsSum - vlRwT + bVl;
                            clmsValues.SetValue(rwsSum, k, rw);
                            clmsSum += rwsSum;
                        }
                        double mean = clmsSum / n;
                        pixelValues.SetValue(mean, 0, rw);
                    }
                    //set the rest of the output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsSum = 0;
                        int rwp = i - rws;
                        int rw = rwp + 1;
                        for (int k = clms; k < pbBigWd; k++)
                        {
                            double pRwsSum = clmsValues[k, rwp];
                            double vlRwT = System.Convert.ToDouble(pixelValuesBig.GetValue(k, rwp));
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(k, i));
                            if (vlRwT == noDataValue) vlRwT = 0;
                            if (bVl == noDataValue) bVl = 0;
                            double rwsSum = pRwsSum - vlRwT + bVl;
                            int clmp = k - clms;
                            int cl = clmp + 1;
                            clmsValues.SetValue(rwsSum, k, rw);
                            clmsSum += rwsSum;
                            double oldClmsValues = clmsValues[clmp, rw];
                            clmsSum = clmsSum - oldClmsValues;
                            double mean = clmsSum / n;
                            pixelValues.SetValue(mean, cl, rw);
                        }
                    }
                    ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in neighborhood sum rectangle helper Function. " + exc.Message, exc);
                throw myExc;
            }
        }
    }
}
