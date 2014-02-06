using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public static class blockHelperStats
    {   
        public static object getBlockSum(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                float outVlF = 0;
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlobj = inArr.GetVal(band, c, r);
                        float vl = 0;
                        if (vlobj == null)
                        {
                            vl = 0;
                        }
                        else
                        {
                            vl = System.Convert.ToSingle(vlobj);
                        }
                        outVlF = outVlF + vl;
                    }
                }
                outVl = outVlF;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockMean(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                float outVlF = 0;
                float n = 0;
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);

                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            n += 1;
                            float vl = System.Convert.ToSingle(vlObj);
                            outVlF += vl;
                        }
                    }
                }
                outVl = outVlF / n;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockVariance(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                float s = 0;
                float s2 = 0;
                float n = 0;
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            n += 1;
                            s += vl;
                            s2 += vl * vl;
                        }
                    }
                }
                outVl = (s2 - ((s * s) / n)) / n;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockStd(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object objVl = getBlockVariance(inArr, band, startColumn, startRows, numCellsInBlock, noDataVl);
            if(objVl!=noDataVl)
            {
                objVl= System.Convert.ToSingle(Math.Sqrt(System.Convert.ToDouble(objVl)));
            }
            return objVl;
        }
        public static Object getBlockMax(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                float outVlF = Single.MinValue;
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);

                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            if (vl > outVlF) outVlF = vl;
                        }
                    }
                }
                outVl = outVlF;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockMin(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {

            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                float outVlF = Single.MaxValue;
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);

                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            if (vl > outVlF) outVlF = vl;
                        }
                    }
                }
                outVl = outVlF;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockUnique(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                HashSet<float> unq = new HashSet<float>();
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {

                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            unq.Add(vl);
                        }
                    }
                }
                outVl = unq.Count;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockEntropy(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                Dictionary<float, int> unq = new Dictionary<float, int>();
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                float n = 0;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            n += 1;
                            int cnt = 0;
                            if (unq.TryGetValue(vl, out cnt))
                            {
                                unq[vl] = cnt += 1;
                            }
                            else
                            {
                                unq.Add(vl, 1);
                            }
                        }
                    }
                }
                float outvl = 0;
                foreach (int i in unq.Values)
                {
                    float prob = System.Convert.ToSingle(i) / n;
                    outvl += prob * System.Convert.ToSingle(Math.Log(prob));
                }
                outVl= -1 * outvl;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockAsm(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                Dictionary<float, int> unq = new Dictionary<float, int>();
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                float n = 0;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            n += 1;
                            int cnt = 0;
                            if (unq.TryGetValue(vl, out cnt))
                            {
                                unq[vl] = cnt += 1;
                            }
                            else
                            {
                                unq.Add(vl, 1);
                            }
                        }
                    }
                }
                float outvl = 0;
                foreach (int i in unq.Values)
                {
                    float prob = System.Convert.ToSingle(i) / n;
                    outvl += prob * prob;
                }
                outVl = outvl;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockMedian(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                Dictionary<float, int> unq = new Dictionary<float, int>();
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                float n = 0;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            n += 1;
                            int cnt = 0;
                            if (unq.TryGetValue(vl, out cnt))
                            {
                                unq[vl] = cnt += 1;
                            }
                            else
                            {
                                unq.Add(vl, 1);
                            }
                        }
                    }
                }
                float halfCnt = n / 2;
                float outvl = 0;
                List<float> sKeys = unq.Keys.ToList();
                sKeys.Sort();
                int nCnt = 0;
                foreach (int i in sKeys)
                {
                    nCnt += unq[i];
                    if (nCnt >= halfCnt)
                    {
                        outvl = i;
                        break;
                    }
                }
                outVl = outvl;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
        public static object getBlockMode(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock, object noDataVl)
        {
            object outVl = 0;
            if (((IPixelBlock4)inArr).HasData())
            {
                Dictionary<float, int> unq = new Dictionary<float, int>();
                int stC = startColumn * numCellsInBlock;
                int stR = startRows * numCellsInBlock;
                int height = stR + numCellsInBlock;
                int width = stC + numCellsInBlock;
                float n = 0;
                for (int r = stR; r < height; r++)
                {
                    for (int c = stC; c < width; c++)
                    {
                        object vlObj = inArr.GetVal(band, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            float vl = System.Convert.ToSingle(vlObj);
                            n += 1;
                            int cnt = 0;
                            if (unq.TryGetValue(vl, out cnt))
                            {
                                unq[vl] = cnt += 1;
                            }
                            else
                            {
                                unq.Add(vl, 1);
                            }
                        }
                    }
                }
                int maxCnt = unq.Values.Max();
                float outvl = 0;
                foreach (KeyValuePair<float, int> kVp in unq)
                {
                    float k = kVp.Key;
                    int v = kVp.Value;
                    if (maxCnt == v)
                    {
                        outvl = k;
                        break;
                    }
                }
                outVl = outvl;
            }
            else
            {
                outVl = noDataVl;
            }
            return outVl;
        }
    }
}
