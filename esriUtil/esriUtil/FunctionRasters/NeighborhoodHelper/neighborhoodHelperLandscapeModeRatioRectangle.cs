using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeModeRatioRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            Dictionary<float,int> ratioDic = new Dictionary<float,int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float ratio = System.Convert.ToSingle(cntArr[1]) / System.Convert.ToSingle(cntArr[0]);
                int cnt;
                if (ratioDic.TryGetValue(ratio, out cnt))
                {
                    ratioDic[ratio] = cnt++;
                }
                else
                {
                    ratioDic.Add(ratio, 1);
                }
            }
            float mode = 0;
            int cntMax = ratioDic.Values.Max();
            foreach (KeyValuePair<float, int> kvp in ratioDic)
            {
                float k = kvp.Key;
                int v = kvp.Value;
                if (v == cntMax)
                {
                    mode = k;
                }
            }
            return mode;
        }
    }
}
       