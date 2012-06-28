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
    class neighborhoodHelperMaxRectangle
    {
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock, int clms, int rws, IRaster orig)
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
                object[,] clmsValues = new object[pbBigWd, pBHeight];
                for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                {
                    double noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    //set the first clmValues row and row of output dataset
                    List<double> clmsMinArr = new List<double>();
                    for (int j = 0; j < pbBigWd; j++)
                    {
                        int pclms = j - clms;
                        int cl = pclms + 1;
                        List<double> rwsMinArr = new List<double>();
                        for (int i = 0; i < rws; i++)
                        {
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(j, i));
                            if (bVl == noDataValue) bVl = Double.MinValue;
                            rwsMinArr.Add(bVl);
                        }
                        double rwsMin = rwsMinArr.Max();
                        clmsValues[j, 0] = rwsMinArr;
                        clmsMinArr.Add(rwsMin);
                        if (cl >= 0)
                        {
                            if (cl > 0)
                            {
                                List<double> oldClmsValues = (List<double>)clmsValues[pclms, 0];
                                clmsMinArr.Remove(oldClmsValues.Max());

                            }
                            pixelValues.SetValue(clmsMinArr.Max(), cl, 0);
                        }

                    }
                    //set first clm of output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsMinArr.Clear();
                        int rwp = i - rws;
                        int rw = rwp + 1;
                        for (int k = 0; k < clms; k++)
                        {
                            List<double> pRwsMinLst = (List<double>)clmsValues[k, rwp];
                            double vlRwT = System.Convert.ToDouble(pixelValuesBig.GetValue(k, rwp));
                            if (vlRwT == noDataValue) vlRwT = Double.MinValue;
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(k, i));
                            if (bVl == noDataValue) bVl = Double.MinValue;
                            pRwsMinLst.Remove(vlRwT);
                            pRwsMinLst.Add(bVl);
                            clmsValues[k, rw] = pRwsMinLst;
                            clmsMinArr.Add(pRwsMinLst.Max());
                        }
                        pixelValues.SetValue(clmsMinArr.Max(), 0, rw);
                    }
                    //set the rest of the output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsMinArr.Clear();
                        int rwp = i - rws;
                        int rw = rwp + 1;
                        for (int k = clms; k < pbBigWd; k++)
                        {
                            int clmp = k - clms;
                            int cl = clmp + 1;
                            if (clmp == 0)
                            {
                                for (int ki = 0; ki < clms; ki++)
                                {

                                    clmsMinArr.Add(((List<double>)clmsValues[ki, rw]).Max());
                                }

                            }
                            List<double> pRwsMinLst = (List<double>)clmsValues[k, rwp];
                            double vlRwT = System.Convert.ToDouble(pixelValuesBig.GetValue(k, rwp));
                            if (vlRwT == noDataValue) vlRwT = Double.MinValue;
                            double bVl = System.Convert.ToDouble(pixelValuesBig.GetValue(k, i));
                            if (bVl == noDataValue) bVl = Double.MinValue;
                            pRwsMinLst.Remove(vlRwT);
                            pRwsMinLst.Add(bVl);
                            double rwsMin = pRwsMinLst.Max();
                            clmsMinArr.Add(rwsMin);
                            clmsValues[k, rw] = pRwsMinLst;
                            List<double> oldClmsValues = (List<double>)clmsValues[clmp, rw];
                            clmsMinArr.Remove(oldClmsValues.Max());
                            pixelValues.SetValue(clmsMinArr.Max(), cl, rw);
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
