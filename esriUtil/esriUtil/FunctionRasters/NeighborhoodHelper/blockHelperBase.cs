using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public static class blockHelperValue
    {   
        public static double getBlockSum(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {
            
            double outVl = 0;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR+numCellsInBlock;
            int width = stC+numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
                    {
                        continue;
                    }
                    else
                    {
                        outVl += vl;
                    }
                }
            }
            return outVl;
        }
        public static double getBlockMean(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {

            double outVl = 0;
            double n = 0;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
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
        public static double getBlockVariance(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {

            double s = 0;
            double s2 = 0;
            double n = 0;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
                    {
                        continue;
                    }
                    else
                    {
                        n += 1;
                        s += vl;
                        s += vl * vl;
                    }
                }
            }
            return (s2 - ((s * s) / n)) / n; ;
        }
        public static double getBlockStd(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {
            return Math.Sqrt(getBlockVariance(inArr, startColumn, startRows, numCellsInBlock, noDataVl));
        }
        public static double getBlockMax(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {

            double outVl = Double.MinValue;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
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
        public static double getBlockMin(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {

            double outVl = Double.MaxValue;
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
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
        public static int getBlockUnique(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {

            HashSet<double> unq = new HashSet<double>();
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
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
        public static double getBlockEntropy(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {
            Dictionary<double,int> unq = new Dictionary<double,int>();
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            double n = 0;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    if (vl == noDataVl)
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
            double outvl = 0;
            foreach (int i in unq.Values)
            {
                double prob = System.Convert.ToDouble(i)/n;
                outvl += prob * Math.Log(prob);
            }
            return -1*outvl;
        }
        public static double getBlockAsm(System.Array inArr, int startColumn, int startRows, int numCellsInBlock, double noDataVl)
        {
            Dictionary<double, int> unq = new Dictionary<double, int>();
            int stC = startColumn * numCellsInBlock;
            int stR = startRows * numCellsInBlock;
            int height = stR + numCellsInBlock;
            int width = stC + numCellsInBlock;
            double n = 0;
            for (int r = stR; r < height; r++)
            {
                for (int c = stC; c < width; c++)
                {
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
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
            double outvl = 0;
            foreach (int i in unq.Values)
            {
                double prob = System.Convert.ToDouble(i) / n;
                outvl += prob * prob;
            }
            return outvl;
        }
    }
}
