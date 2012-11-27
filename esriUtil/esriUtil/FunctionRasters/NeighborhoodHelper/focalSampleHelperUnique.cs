using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperUnique: focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            HashSet<float> unq = new HashSet<float>();
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                float vl = System.Convert.ToSingle(bigArr.GetValue(bWc, bRc));
                if (rasterUtil.isNullData(vl, noDataValue))
                {
                    continue;
                }
                else
                {
                    unq.Add(vl);
                }
            }
            return unq.Count;
        }
    }
}
