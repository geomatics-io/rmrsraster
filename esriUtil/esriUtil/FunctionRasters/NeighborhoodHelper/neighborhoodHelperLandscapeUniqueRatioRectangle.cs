using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeUniqueRatioRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            List<float> uniqueLst = new List<float>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                float ratio = System.Convert.ToSingle(cntArr[1]) / System.Convert.ToSingle(cntArr[0]);
                if (!uniqueLst.Contains(ratio))
                {
                    uniqueLst.Add(ratio);
                }
            }
            return uniqueLst.Count;
        }
    }
}
