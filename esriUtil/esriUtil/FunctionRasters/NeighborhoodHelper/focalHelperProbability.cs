using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperProbability : focalFunctionDatasetBase
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            Dictionary<double, int> countDic = new Dictionary<double, int>();
            int cntSub = 0;
            foreach (int[] xy in iter)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                if (vl == noDataValue)
                {
                    cntSub+=1;
                }
                else
                {
                    int cnt = 0;
                    if (countDic.TryGetValue(vl, out cnt))
                    {
                        countDic[vl] = cnt+1;
                    }
                    else
                    {
                        countDic.Add(vl, 1);
                    }
                }
            }
            double n = System.Convert.ToDouble(iter.Count - cntSub);
            double prob = 0;
            foreach (int v in countDic.Values)
            {
                double vD = System.Convert.ToDouble(v);
                prob += Math.Pow(( vD / n),2);
            }
            //Console.WriteLine(vlMax);
            return prob;
        }
    }
}