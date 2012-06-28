using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMedianEdgeRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double noDataValue)
        {
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr,noDataValue, ref uniqueDic);
            int cnt = (uniqueDic.Count+1)/2;
            List<int> areaLst = new List<int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                areaLst.Add(cntArr[1]);
            }
            areaLst.Sort();
            return areaLst[cnt];
        }
    }
}
