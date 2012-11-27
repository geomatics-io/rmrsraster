using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeVarianceEdgeRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            float n = uniqueDic.Count;
            float sum = 0;
            float sum2 = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float vl = cntArr[1];
                sum += vl;
                sum2 += (vl * vl);
            }
            sum = (sum * sum) / n;
            float var = (sum2 - sum) / n;
            return var;
        }
    }
}