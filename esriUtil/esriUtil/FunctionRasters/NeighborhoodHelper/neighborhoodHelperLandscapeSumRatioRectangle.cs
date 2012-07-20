using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeSumRatioRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            double sumR = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = System.Convert.ToDouble(cntArr[1]) / System.Convert.ToDouble(cntArr[0]);
                sumR += ratio;
            }
            return sumR;
        }
    }
}