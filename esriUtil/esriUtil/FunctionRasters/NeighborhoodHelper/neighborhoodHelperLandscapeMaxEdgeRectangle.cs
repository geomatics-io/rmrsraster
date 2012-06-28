using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMaxEdgeRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr, noDataValue, ref uniqueDic);
            int maxVl = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                int cnt = cntArr[1];
                if (cnt > maxVl)
                {
                    maxVl = cnt;
                }
            }
            return maxVl;
        }
    }
}
