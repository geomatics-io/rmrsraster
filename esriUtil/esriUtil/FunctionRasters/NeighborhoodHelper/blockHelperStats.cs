using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public static class blockHelperStats
    {   
        public static float getBlockSum(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        vl = 0;
                    }
                    outVl = outVl + vl;
                }
            }
            return outVl;
        }
        public static float getBlockMean(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
                        n += 1;
                        outVl += vl;
                    }
                }
            }
            return outVl/n;
        }
        public static float getBlockVariance(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
                        n += 1;
                        s += vl;
                        s2 += vl * vl;
                    }
                }
            }
            return (s2 - ((s * s) / n)) / n;
        }
        public static float getBlockStd(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
        {
            return System.Convert.ToSingle(Math.Sqrt(getBlockVariance(inArr, startColumn, startRows, numCellsInBlock, noDataVl)));
        }
        public static float getBlockMax(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
                        if (vl > outVl) outVl = vl;
                    }
                }
            }
            return outVl;
        }
        public static float getBlockMin(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
                        if (vl < outVl) outVl = vl;
                    }
                }
            }
            return outVl;
        }
        public static int getBlockUnique(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
                        unq.Add(vl);
                    }
                }
            }
            return unq.Count;
        }
        public static float getBlockEntropy(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl, noDataVl))
                    {
                        continue;
                    }
                    else
                    {
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
        public static float getBlockAsm(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (rasterUtil.isNullData(vl,noDataVl))
                    {
                        continue;
                    }
                    else
                    {
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
        public static float getBlockMedian(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (vl == noDataVl)
                    {
                        continue;
                    }
                    else
                    {
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
        public static float getBlockMode(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, float noDataVl)
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
                    float vl = System.Convert.ToSingle(inArr.GetValue(c, r));
                    if (vl == noDataVl)
                    {
                        continue;
                    }
                    else
                    {
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
