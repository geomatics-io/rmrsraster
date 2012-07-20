using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMedianAreaRectangle:neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(Dictionary<int, int[]> uniqueDic)
        {
            int cnt = (uniqueDic.Count + 1) / 2;
            List<int> areaLst = new List<int>();
            foreach (int[] cntArr in uniqueDic.Values)
            {
                areaLst.Add(cntArr[0]);
            }
            areaLst.Sort();
            return areaLst[cnt];
        }
        
    }
}
