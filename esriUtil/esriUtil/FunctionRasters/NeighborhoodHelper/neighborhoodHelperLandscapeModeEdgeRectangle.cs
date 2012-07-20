using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeModeEdgeRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            double mode = 0;
            List<int> areaLst = new List<int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                areaLst.Add(cntArr[1]);
            }
            int maxArea = areaLst.Max();
            foreach (int cntVl in areaLst)
            {
                if (cntVl == maxArea)
                {
                    mode = cntVl;
                }
            }
            return mode;

        }
   }
}
