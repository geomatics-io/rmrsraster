using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMeanEdgeRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            double n = uniqueDic.Count;
            double sum = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double vl = cntArr[1];
                sum += vl;
            }
            return sum / n;
        }
    }
}
