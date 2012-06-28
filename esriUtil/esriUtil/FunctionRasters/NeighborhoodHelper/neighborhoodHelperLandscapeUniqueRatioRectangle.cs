using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeUniqueRatioRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr,double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr, noDataValue, ref uniqueDic);
            List<double> uniqueLst = new List<double>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                double ratio = cntArr[1] / cntArr[0];
                if (!uniqueLst.Contains(ratio))
                {
                    uniqueLst.Add(ratio);
                }
            }
            return uniqueLst.Count;
        }
    }
}
