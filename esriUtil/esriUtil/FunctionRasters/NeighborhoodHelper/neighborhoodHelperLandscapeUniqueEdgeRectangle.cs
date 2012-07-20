using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeUniqueEdgeRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            List<int> uniqueCnt = new List<int>();
            foreach (int[] vlArr in uniqueDic.Values)
            {
                int vl = vlArr[1];
                if (!uniqueCnt.Contains(vl))
                {
                    uniqueCnt.Add(vlArr[1]);
                }
            }
            return uniqueCnt.Count;
        }
    }
}