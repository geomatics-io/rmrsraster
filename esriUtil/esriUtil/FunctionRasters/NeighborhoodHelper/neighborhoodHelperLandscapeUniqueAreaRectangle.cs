using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeUniqueAreaRectangle : neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double rsNoDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr, rsNoDataValue, ref uniqueDic);
            List<int> uniqueCnt = new List<int>();
            foreach (int[] vlArr in uniqueDic.Values)
            {
                int vl = vlArr[0];
                if (!uniqueCnt.Contains(vl))
                {
                    uniqueCnt.Add(vl);
                }
            }
            return uniqueCnt.Count;
        }
    }
}
