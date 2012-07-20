using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeModeRatioRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            Dictionary<double,int> ratioDic = new Dictionary<double,int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = System.Convert.ToDouble(cntArr[1]) / System.Convert.ToDouble(cntArr[0]);
                int cnt;
                if (ratioDic.TryGetValue(ratio, out cnt))
                {
                    ratioDic[ratio] = cnt++;
                }
                else
                {
                    ratioDic.Add(ratio, 1);
                }
            }
            double mode = 0;
            int cntMax = ratioDic.Values.Max();
            foreach (KeyValuePair<double, int> kvp in ratioDic)
            {
                double k = kvp.Key;
                int v = kvp.Value;
                if (v == cntMax)
                {
                    mode = k;
                }
            }
            return mode;
        }
    }
}
       