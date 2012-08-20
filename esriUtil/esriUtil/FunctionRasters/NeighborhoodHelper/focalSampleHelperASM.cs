using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperASM:focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            Dictionary<double, int> countDic = new Dictionary<double, int>();
            int cntSub = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                if (vl == noDataValue)
                {
                    cntSub += 1;
                }
                else
                {
                    int cnt = 0;
                    if (countDic.TryGetValue(vl, out cnt))
                    {
                        countDic[vl] = cnt + 1;
                    }
                    else
                    {
                        countDic.Add(vl, 1);
                    }
                }
            }
            int n = offsetLst.Count - cntSub;
            double prob = 0;
            double ent = 0;
            foreach (int v in countDic.Values)
            {
                prob = (System.Convert.ToDouble(v) / n);
                ent += (prob * prob);
            }
            //Console.WriteLine(vlMax);
            return ent;
        }
    }
}
