using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperEntropy : focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            Dictionary<float, int> countDic = new Dictionary<float, int>();
            int cntSub = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                float vl = System.Convert.ToSingle(bigArr.GetValue(bWc, bRc));
                if (rasterUtil.isNullData(vl, noDataValue))
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
            int n = offsetLst.Count - cntSub;
            float prob = 0;
            float ent = 0;
            foreach (int v in countDic.Values)
            {
                prob = (System.Convert.ToSingle(v) / n);
                ent += (prob * System.Convert.ToSingle(Math.Log(prob)));
            }
            //Console.WriteLine(vlMax);
            return (ent * -1);
        }
    }
}