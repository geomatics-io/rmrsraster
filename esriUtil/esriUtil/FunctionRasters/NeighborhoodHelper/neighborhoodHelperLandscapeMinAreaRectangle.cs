using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMinAreaRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override float findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            int minVl = Int32.MaxValue;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                int cnt = cntArr[0];
                if (cnt < minVl)
                {
                    minVl = cnt;
                }
            }
            return minVl;
        }
      
    }
}
