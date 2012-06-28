using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMaxRatioRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr,noDataValue, ref uniqueDic);
            double maxVl = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = cntArr[1]/System.Convert.ToDouble(cntArr[0]);
                if (ratio > maxVl)
                {
                    maxVl = ratio;
                }
            }
            return maxVl;
        }
    }
}
    