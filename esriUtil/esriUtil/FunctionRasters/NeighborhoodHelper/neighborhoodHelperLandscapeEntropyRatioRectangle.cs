using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeEntropyRatioRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            double n = uniqueDic.Count;
            Dictionary<double,int> ratioDic = new Dictionary<double,int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = System.Convert.ToDouble(cntArr[1])/System.Convert.ToDouble(cntArr[0]);
                int cnt;
                if (ratioDic.TryGetValue(ratio, out cnt))
                {
                    ratioDic[ratio] = cnt + 1;
                }
                else
                {
                    ratioDic.Add(ratio, 1);
                }
            }
            double entropy = 0;
            foreach(int vl in ratioDic.Values)
            {
                double prob = vl / n;
                entropy = entropy + prob * Math.Log(prob);
            }
            entropy = -1 * entropy;
            return entropy;
        }
    }
}