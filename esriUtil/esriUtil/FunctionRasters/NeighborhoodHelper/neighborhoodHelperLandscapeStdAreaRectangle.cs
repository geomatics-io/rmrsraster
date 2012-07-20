using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeStdAreaRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
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
            double std = Math.Sqrt(var);
            return std;
        }
    }
}
