using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMode : focalSampleDataset
    {
        public override object getTransformedValue(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 bigArr, int startClm, int startRw, int nBand)
        {
            Dictionary<float, int> countDic = new Dictionary<float, int>();
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                object vlObj = bigArr.GetVal(nBand, bWc, bRc);

                if (vlObj == null)
                {
                    continue;
                }
                else
                {
                    //int vl2 = System.Convert.ToInt32(vl);
                    //Console.WriteLine(vl)
                    float vl = (float)vlObj;
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
            int vlMax = countDic.Values.Max();
            float key = noDataValue;
            foreach (KeyValuePair<float, int> kVp in countDic)
            {
                float k = kVp.Key;
                int v = kVp.Value;
                if (v == vlMax)
                {
                    key = k;
                    break;
                }
            }
            //Console.WriteLine(vlMax);
            return key;
        }
    }
}

