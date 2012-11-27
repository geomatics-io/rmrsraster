using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeVarianceRatioRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            float sumR = 0;
            float sumR2 = 0;
            float n = uniqueDic.Count;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float ratio = System.Convert.ToSingle(cntArr[1]) / System.Convert.ToSingle(cntArr[0]);
                sumR += ratio;
                sumR2 += ratio * ratio;
            }
            float var = System.Convert.ToSingle((sumR2 - (Math.Pow(sumR, 2) / n)) / n);
            return var;
        }
    }
}