using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMedian : focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            List<double> dbLst = new List<double>();
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                if (vl == noDataValue)
                {
                    continue;
                }
                else
                {
                    dbLst.Add(vl);
                }
            }
            int middleVlIndex = dbLst.Count / 2;
            dbLst.Sort();
            return dbLst[middleVlIndex];
        }
    }
}