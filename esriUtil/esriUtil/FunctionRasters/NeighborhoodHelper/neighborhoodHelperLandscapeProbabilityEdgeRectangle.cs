using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeProbabilityEdgeRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            float n = uniqueDic.Count;
            Dictionary<float, int> ratioDic = new Dictionary<float, int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float ratio = cntArr[1];
                int cnt;
                if (ratioDic.TryGetValue(ratio, out cnt))
                {
                    ratioDic[ratio] = cnt + 1;
                }
                else
                {
                    ratioDic.Add(ratio, 1);
                }
            }
            float entropy = 0;
            foreach (int vl in ratioDic.Values)
            {
                float prob = vl / n;
                entropy = entropy + (prob * prob);
            }
            return entropy;
        }
    }
}
            