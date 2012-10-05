using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperMin: focalFunctionDatasetBase
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            double db = Double.MaxValue;
            foreach (int[] xy in iter)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                if (!rasterUtil.isNullData(vl, noDataValue))
                {

                    if (vl < db)
                    {
                        db = vl;
                    }
                }
                else
                {
                    continue;
                }
            }
            return db;
        }
    }
}
