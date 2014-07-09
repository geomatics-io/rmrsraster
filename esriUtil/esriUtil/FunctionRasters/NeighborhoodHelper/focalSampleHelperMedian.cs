using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMedian : focalSampleDataset
    {
        public override object getTransformedValue(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 bigArr, int startClm, int startRw, int nBand)
        {
            List<float> dbLst = new List<float>();
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                object vlObj = bigArr.GetVal(nBand, bWc, bRc);

                if (vlObj == null)
                {
                    continue;
                }
                else
                {
                    float vl = (float)vlObj;
                    dbLst.Add(vl);
                }
            }
            int middleVlIndex = dbLst.Count / 2;
            dbLst.Sort();
            return dbLst[middleVlIndex];
        }
    }
}