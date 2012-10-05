using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperMean : focalFunctionDatasetBase
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            double db = 0;
            int cntSub = 0;
            foreach (int[] xy in iter)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                if (rasterUtil.isNullData(vl, noDataValue))
                {
                    cntSub++;
                }
                else
                {
                    db += vl;
                }
            }
            return db/(iter.Count-cntSub);
        }
    }
}
