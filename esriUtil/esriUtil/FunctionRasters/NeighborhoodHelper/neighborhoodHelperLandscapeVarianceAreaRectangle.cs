using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeVarianceAreaRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr, noDataValue, ref uniqueDic);
            double n = uniqueDic.Count;
            double sum = 0;
            double sum2 = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double vl = cntArr[0];
                sum += vl;
                sum2 += (vl * vl);
            }
            sum = (sum * sum) / n;
            double var = (sum2 - sum) / n;
            return var;
        }
    }
}
