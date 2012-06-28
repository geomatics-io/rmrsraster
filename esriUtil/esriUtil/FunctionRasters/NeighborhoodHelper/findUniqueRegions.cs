using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class findUniqueRegions
    {
        public int[,] getUniqueRegions(double[,] windowArr, double rsNoDataValue, ref Dictionary<int, int[]> uniqueDictionary)
        {
            int uniqueCnt = 1;
            if (uniqueDictionary.Keys.Count > 0)
            {
                uniqueCnt = uniqueDictionary.Keys.Max();
            }
            int maxC = windowArr.GetUpperBound(0)+1;
            int maxR = windowArr.GetUpperBound(1) + 1;
            int[,] windowArr2 = new int[maxC, maxR];
            int noDataValue = 0;
            for (int c = 0; c < maxC; c++)
            {
                int cp = c;
                for (int r = 0; r < maxR; r++)
                {
                    int rp = r;
                    double cvd = windowArr[c, r];
                    if (cvd == rsNoDataValue||Double.IsNaN(cvd)||Double.IsInfinity(cvd))
                    {
                    }
                    else
                    {
                        int cv = System.Convert.ToInt32(cvd);
                        int pv = windowArr2[c, r];
                        if (pv != noDataValue)
                        {
                        }
                        else
                        {
                            int[] cntVls = { 1, 0 };
                            windowArr2[c, r] = uniqueCnt;
                            uniqueDictionary.Add(uniqueCnt, cntVls);
                            getUniqueRegion(cv, uniqueCnt, c, r, ref windowArr, ref windowArr2, ref uniqueDictionary);
                            uniqueCnt++;
                        }
                    }
                    
                }
            }
            return windowArr2;
        }
        private void getUniqueRegion(int inValue, int newValue, int clm, int rw, ref double[,] windowArr, ref int [,] windowArr2, ref Dictionary<int,int[]> uniqueDictionary)
        {
            int[] rtVl = uniqueDictionary[newValue];
            int maxC = windowArr.GetUpperBound(0);
            int maxR = windowArr.GetUpperBound(1);
            for (int i = 0; i < 4; i++)
            {
                int cp = clm;
                int rp = rw;
                switch (i)
                {
                    case 0:
                        cp = cp + 1;
                        break;
                    case 1:
                        rp = rp + 1;
                        break;
                    case 2:
                        cp = cp - 1;
                        break;
                    case 3:
                        rp = rp - 1;
                        break;
                    default:
                        break;
                }
                if (cp > maxC || rp > maxR || cp < 0 || rp < 0)
                {
                    rtVl[1] = rtVl[1] + 1;
                }
                else
                {
                    int cVl = windowArr2[cp,rp];
                    int pVl = System.Convert.ToInt32(windowArr[cp,rp]);
                    if (cVl == 0)
                    {

                        if (pVl == inValue)
                        {
                            rtVl[0] = rtVl[0] + 1;
                            windowArr2[cp, rp] = newValue;
                            getUniqueRegion(inValue, newValue, cp, rp, ref windowArr, ref windowArr2, ref uniqueDictionary);
                        }
                        else
                        {
                            rtVl[1] = rtVl[1] + 1;
                        }
                    }
                    else
                    {
                        if (pVl != inValue)
                        {
                            rtVl[1] = rtVl[1] + 1;
                        }
                        else
                        {
                        }
                    }
                }
                uniqueDictionary[newValue] = rtVl;
            }

        }
    }
}
