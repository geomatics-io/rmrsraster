using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMinRatioRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            float maxVl = Single.MaxValue;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float ratio = System.Convert.ToSingle(cntArr[1]) / System.Convert.ToSingle(cntArr[0]);
                if (ratio < maxVl)
                {
                    maxVl = ratio;
                }
            }
            return maxVl;
        }
    }
}
