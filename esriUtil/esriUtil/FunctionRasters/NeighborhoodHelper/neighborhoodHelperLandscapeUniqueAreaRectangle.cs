using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeUniqueAreaRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            List<int> uniqueCnt = new List<int>();
            foreach (int[] vlArr in uniqueDic.Values)
            {
                int vl = vlArr[0];
                if (!uniqueCnt.Contains(vl))
                {
                    uniqueCnt.Add(vl);
                }
            }
            return uniqueCnt.Count;
        }
    }
}
