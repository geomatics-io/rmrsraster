using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeSumRatioRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double rsNoDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr,rsNoDataValue, ref uniqueDic);
            double sumR = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = cntArr[1] / cntArr[0];
                sumR += ratio;
            }
            return sumR;
        }
    }
}