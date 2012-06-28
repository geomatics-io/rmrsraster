using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class neighborhoodHelperLandscapeMaxAreaRectangle: neighborhoodHelperLandscapeRectangleBase
    {
        public override double findUniqueRegionsValue(double[,] windowArr, double rsNoDataValue)
        {
            //Console.WriteLine("got to the overide");
            Dictionary<int, int[]> uniqueDic = new Dictionary<int, int[]>();
            findUniqueRegions fUnq = new findUniqueRegions();
            fUnq.getUniqueRegions(windowArr,rsNoDataValue,ref uniqueDic);
            int maxVl = 0;
            foreach (int[] cntArr in uniqueDic.Values)
            {
                int cnt = cntArr[0];
                if (cnt > maxVl)
                {
                    maxVl = cnt;
                }
            }
            return maxVl;
        }
    }
}
