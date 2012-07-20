using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMedianRatioRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            List<double> ratioLst = new List<double>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = System.Convert.ToDouble(cntArr[1]) / System.Convert.ToDouble(cntArr[0]);
                ratioLst.Add(ratio);
            }
            ratioLst.Sort();
            int cnt = (uniqueDic.Count + 1) / 2;
            return ratioLst[cnt];
        }
    }
}