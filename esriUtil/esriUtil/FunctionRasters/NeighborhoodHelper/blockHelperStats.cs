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
        public static float getBlockSum(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {
            
            float outVl = 0;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR+numCellsInBlock;
            int width = stC+numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    object vlobj = inArr.GetVal(band,c, r);
                    float vl = 0;
                    if (vlobj == null)
                    {
                        vl = 0;
                    }
                    else
                    {
                        vl = System.Convert.ToSingle(vlobj);
                    }
                    outVl = outVl + vl;
                }
            }
            return outVl;
        }
        public static float getBlockMean(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {

            float outVl = 0;
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
                    
                    if (vlObj==null)
                    {
                        continue;
                    }
                    else
                    {
                        n += 1;
                        float vl = System.Convert.ToSingle(vlObj);
                        outVl += vl;
                    }
                }
            }
            return outVl/n;
        }
        public static float getBlockVariance(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
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
                    if (vlObj==null)
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
            return (s2 - ((s * s) / n)) / n;
        }
        public static float getBlockStd(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {
            return System.Convert.ToSingle(Math.Sqrt(getBlockVariance(inArr, band, startColumn, startRows, numCellsInBlock)));
        }
        public static float getBlockMax(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {

            float outVl = Single.MinValue;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    object vlObj = inArr.GetVal(band, c, r);
                    
                    if (vlObj==null)
                    {
                        continue;
                    }
                    else
                    {
                        float vl = System.Convert.ToSingle(vlObj);
                        if (vl > outVl) outVl = vl;
                    }
                }
            }
            return outVl;
        }
        public static float getBlockMin(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {

            float outVl = Single.MaxValue;
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
                        if (vl < outVl) outVl = vl;
                    }
                }
            }
            return outVl;
        }
        public static int getBlockUnique(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
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
            return unq.Count;
        }
        public static float getBlockEntropy(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
        {
            Dictionary<float,int> unq = new Dictionary<float,int>();
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
                        n+=1;
                        int cnt = 0;
                        if (unq.TryGetValue(vl, out cnt))
                        {
                            unq[vl] = cnt += 1;
                        }
                        else
                        {
                            unq.Add(vl,1);
                        }
                    }
                }
            }
            float outvl = 0;
            foreach (int i in unq.Values)
            {
                float prob = System.Convert.ToSingle(i)/n;
                outvl += prob * System.Convert.ToSingle(Math.Log(prob));
            }
            return -1*outvl;
        }
        public static float getBlockAsm(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
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
            return outvl;
        }
        public static float getBlockMedian(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
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
            return outvl;
        }
        public static float getBlockMode(IPixelBlock3 inArr, int band, int startColumn, int startRows, int numCellsInBlock)
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
            foreach (KeyValuePair<float,int> kVp in unq)
            {
                float k = kVp.Key;
                int v = kVp.Value;
                if(maxCnt==v)
                {
                    outvl = k;
                    break;
                }
            }
            return outvl;
        }
    }
}
