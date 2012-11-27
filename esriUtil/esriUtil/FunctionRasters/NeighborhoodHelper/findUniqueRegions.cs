using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class findUniqueRegions
    {
        public static Dictionary<int, int[]> getUniqueRegions(System.Array windowArr, int eC, int eR, int windowClms, int windowRows,int sc,int sr, float rsNoDataValue)
        {
            Dictionary<int, int[]> outDic = new Dictionary<int, int[]>();
            int uniqueCnt = 1;
            int[,] windowArr2 = new int[windowClms, windowRows];
            int noDataValue = 0;
            
            for (int c = sc; c < eC; c++)
            {
                
                int cSmall = c - sc;
                for (int r = sr; r < eR; r++)
                {
                    int rSmall = r - sr;
                    
                    float cvd = System.Convert.ToSingle(windowArr.GetValue(c, r));
                    
                    if (rasterUtil.isNullData(cvd,rsNoDataValue))
                    {
                    }
                    else
                    {
                        int cv = System.Convert.ToInt32(cvd);
                        int pv = windowArr2[cSmall, rSmall];
                        //Console.WriteLine("cv =  " + cv.ToString() + "pv= " + pv.ToString());
                        if (pv != noDataValue)
                        {
                        }
                        else
                        {
                            int[] cntVls = { 1, 0 };
                            windowArr2[cSmall, rSmall] = uniqueCnt;
                            outDic.Add(uniqueCnt, cntVls);
                            getUniqueRegion(cv, uniqueCnt, c, r, cSmall, rSmall, windowArr, windowArr2, outDic);
                            uniqueCnt++;
                        }
                    }

                }
            }
            return outDic;
        }
        public static Dictionary<int, int[]> getUniqueRegions(System.Array windowArr, int eC, int eR, int windowClms, int windowRows,int sc,int sr, float rsNoDataValue, int[,] circleWindow)
        {
            Dictionary<int, int[]> outDic = new Dictionary<int, int[]>();
            int uniqueCnt = 1;
            int[,] windowArr2 = new int[windowClms, windowRows];
            int noDataValue = 0;
            for (int c = sc; c < eC; c++)
            {
                int cSmall = c - sc;
                for (int r = sr; r < eR; r++)
                {
                    int rSmall = r - sr;
                    float cvd = System.Convert.ToSingle(windowArr.GetValue(c, r));
                    if (rasterUtil.isNullData(cvd, rsNoDataValue))
                    {
                    }
                    else
                    {
                        
                        int circleVl = circleWindow[cSmall, rSmall];
                        if (circleVl == 0)
                        {
                        }
                        else
                        {
                            int cv = System.Convert.ToInt32(cvd);
                            int pv = windowArr2[cSmall, rSmall];
                            if (pv != noDataValue)
                            {
                            }
                            else
                            {
                                int[] cntVls = { 1, 0 };
                                windowArr2[cSmall, rSmall] = uniqueCnt;
                                outDic.Add(uniqueCnt, cntVls);
                                getUniqueRegion(cv, uniqueCnt, c, r, cSmall, rSmall, windowArr, windowArr2, outDic, circleWindow);
                                uniqueCnt++;
                            }
                        }
                    }
                }
            }
            return outDic;
        }
        private static void getUniqueRegion(int inValue, int newValue, int clm, int rw, int cSmall,int rSmall, System.Array windowArr, int[,] windowArr2, Dictionary<int, int[]> uniqueDictionary)
        {
            int[] rtVl = uniqueDictionary[newValue];
            int maxC = windowArr2.GetUpperBound(0);
            int maxR = windowArr2.GetUpperBound(1);
            for (int i = 0; i < 4; i++)
            {
                int cp = cSmall;
                int rp = rSmall;
                int cp2 = clm;
                int rp2 = rw;
                switch (i)
                {
                    case 0:
                        cp = cp + 1;
                        cp2 = cp2 + 1;
                        break;
                    case 1:
                        rp = rp + 1;
                        rp2 = rp2 + 1;
                        break;
                    case 2:
                        cp = cp - 1;
                        cp2 = cp2 - 1;
                        break;
                    case 3:
                        rp = rp - 1;
                        rp2 = rp2 - 1;
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
                int cVl = windowArr2[cp, rp];
                    
                    //Console.WriteLine("cp:rp = " + cp.ToString() + ":" + rp.ToString());
                    //Console.WriteLine("cp2:rp2 = " + cp2.ToString() + ":" + rp2.ToString());
                    int pVl = Int32.MinValue;
                    try
                    {
                        pVl = System.Convert.ToInt32(windowArr.GetValue(cp2, rp2));
                    }
                    catch
                    {

                    }
                    if (cVl == 0)
                    {

                        if (pVl == inValue)
                        {
                            rtVl[0] = rtVl[0] + 1;
                            windowArr2[cp, rp] = newValue;
                            getUniqueRegion(inValue, newValue, cp2,rp2,cp, rp, windowArr, windowArr2, uniqueDictionary);
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
        private static void getUniqueRegion(int inValue, int newValue, int clm, int rw, int cSmall, int rSmall, System.Array windowArr, int[,] windowArr2, Dictionary<int, int[]> uniqueDictionary, int[,] CircleWindow)
        {
            int[] rtVl = uniqueDictionary[newValue];
            int maxC = windowArr2.GetUpperBound(0);
            int maxR = windowArr2.GetUpperBound(1);
            for (int i = 0; i < 4; i++)
            {
                int cp = cSmall;
                int rp = rSmall;
                int cp2 = clm;
                int rp2 = rw;
                switch (i)
                {
                    case 0:
                        cp = cp + 1;
                        cp2 = cp2 + 1;
                        break;
                    case 1:
                        rp = rp + 1;
                        rp2 = rp2 + 1;
                        break;
                    case 2:
                        cp = cp - 1;
                        cp2 = cp2 - 1;
                        break;
                    case 3:
                        rp = rp - 1;
                        rp2 = rp2 - 1;
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
                    int circleCV = CircleWindow[cp, rp];
                    if (circleCV == 0)
                    {
                        rtVl[1] = rtVl[1] + 1;
                    }
                    else
                    {
                        int cVl = windowArr2[cp, rp];

                        //Console.WriteLine("cp:rp = " + cp.ToString() + ":" + rp.ToString());
                        //Console.WriteLine("cp2:rp2 = " + cp2.ToString() + ":" + rp2.ToString());
                        int pVl = Int32.MinValue;
                        try
                        {
                            pVl = System.Convert.ToInt32(windowArr.GetValue(cp2, rp2));
                        }
                        catch
                        {

                        }
                        if (cVl == 0)
                        {

                            if (pVl == inValue)
                            {
                                rtVl[0] = rtVl[0] + 1;
                                windowArr2[cp, rp] = newValue;
                                getUniqueRegion(inValue, newValue, cp2, rp2, cp, rp, windowArr, windowArr2, uniqueDictionary,CircleWindow);
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
                }
                uniqueDictionary[newValue] = rtVl;
            }

        }
    
    }
}
