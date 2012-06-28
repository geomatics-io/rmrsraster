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
    class neighborhoodHelperModeRectangle
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
                int dicCnt = 0;
                int n = clms * rws;
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
                    Dictionary<int, int> clmsDic = new Dictionary<int, int>();
                    for (int j = 0; j < pbBigWd; j++)
                    {
                        int pclms = j - clms;
                        int cl = pclms + 1;
                        List<int> rwsMinArr = new List<int>();
                        for (int i = 0; i < rws; i++)
                        {
                            int bVl = System.Convert.ToInt32(pixelValuesBig.GetValue(j, i));
                            rwsMinArr.Add(bVl);
                            if (clmsDic.TryGetValue(bVl, out dicCnt))
                            {
                                clmsDic[bVl] = dicCnt += 1;
                            }
                            else
                            {
                                clmsDic.Add(bVl, 1);
                            }
                        }
                        clmsValues[j, 0] = rwsMinArr;
                        if (cl >= 0)
                        {
                            if (cl > 0)
                            {
                                List<int> oldClmsValues = (List<int>)clmsValues[pclms, 0];
                                foreach (int clInt in oldClmsValues)
                                {
                                    dicCnt = clmsDic[clInt];
                                    if (dicCnt == 1)
                                    {
                                        clmsDic.Remove(clInt);
                                    }
                                    else
                                    {
                                        clmsDic[clInt] = dicCnt - 1;
                                    }
                                }

                            }
                            double entropy = calcProb(clmsDic, n);
                            pixelValues.SetValue(entropy, cl, 0);
                        }

                    }
                    //set first clm of output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsDic.Clear();
                        int rwp = i - rws;
                        int rw = rwp + 1;
                        for (int k = 0; k < clms; k++)
                        {
                            List<int> pRwsMinLst = (List<int>)clmsValues[k, rwp];
                            int vlRwT = System.Convert.ToInt32(pixelValuesBig.GetValue(k, rwp));
                            int bVl = System.Convert.ToInt32(pixelValuesBig.GetValue(k, i));
                            pRwsMinLst.Remove(vlRwT);
                            pRwsMinLst.Add(bVl);
                            clmsValues[k, rw] = pRwsMinLst;
                            foreach (int clInt in pRwsMinLst)
                            {
                                if (clmsDic.TryGetValue(clInt, out dicCnt))
                                {
                                    clmsDic[clInt] = dicCnt + 1;
                                }
                                else
                                {
                                    clmsDic.Add(clInt, 1);
                                }
                            }
                        }
                        double entropy = calcProb(clmsDic, n);
                        pixelValues.SetValue(entropy, 0, rw);
                    }
                    //set the rest of the output data
                    for (int i = rws; i < pbBigHt; i++)
                    {
                        clmsDic.Clear();
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
                                    List<int> pClmValues = (List<int>)clmsValues[ki, rw];
                                    foreach (int clInt in pClmValues)
                                    {
                                        if (clmsDic.TryGetValue(clInt, out dicCnt))
                                        {
                                            clmsDic[clInt] = dicCnt + 1;
                                        }
                                        else
                                        {
                                            clmsDic.Add(clInt, 1);
                                        }
                                    }
                                }
                            }
                            List<int> pRwsMinLst = (List<int>)clmsValues[k, rwp];
                            int vlRwT = System.Convert.ToInt32(pixelValuesBig.GetValue(k, rwp));
                            int bVl = System.Convert.ToInt32(pixelValuesBig.GetValue(k, i));
                            pRwsMinLst.Remove(vlRwT);
                            pRwsMinLst.Add(bVl);
                            foreach (int clInt in pRwsMinLst)
                            {
                                if (clmsDic.TryGetValue(clInt, out dicCnt))
                                {
                                    clmsDic[clInt] = dicCnt + 1;
                                }
                                else
                                {
                                    clmsDic.Add(clInt, 1);
                                }
                            }
                            clmsValues[k, rw] = pRwsMinLst;
                            List<int> oldClmsValues = (List<int>)clmsValues[clmp, rw];
                            foreach (int clInt in oldClmsValues)
                            {
                                dicCnt = clmsDic[clInt];
                                if (dicCnt <= 1)
                                {
                                    clmsDic.Remove(clInt);
                                }
                                else
                                {
                                    clmsDic[clInt] = dicCnt - 1;
                                }
                            }
                            double entropy = calcProb(clmsDic, n);
                            pixelValues.SetValue(entropy, cl, rw);
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

        private double calcProb(Dictionary<int, int> clmsDic, int n)
        {
            int tProb = 0;
            int maxVl = clmsDic.Values.Max();
            foreach (KeyValuePair<int,int> kvp in clmsDic)
            {
                int k = kvp.Key;
                int v = kvp.Value;
                if (v == maxVl)
                {
                    tProb = v;
                    break;
                }
            }
            return tProb;
        }
    }
}
