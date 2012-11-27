using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMax : focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            float db = Single.MinValue;
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
                    if (vl > db)
                    {
                        db = vl;
                    }
                }
            }
            return db;
        }
    }
}