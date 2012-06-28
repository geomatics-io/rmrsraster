using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeVarianceRatioRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr, noDataValue, ref uniqueDic);
            double sumR=0;
            double sumR2=0;
            double n = uniqueDic.Count;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = cntArr[1] / cntArr[0];
                sumR += ratio;
                sumR2 += ratio * ratio;
            }
            double var = (sumR2 - (Math.Pow(sumR, 2) / n)) / n;
            return var;
        }
    }
}