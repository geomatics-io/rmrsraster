using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperStd : focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            //Console.WriteLine("Start CR = " + startClm.ToString()+":"+ startRw.ToString());
            double s = 0;
            double s2 = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;

                double vl = System.Convert.ToDouble(bigArr.GetValue(bWc, bRc));
                //Console.WriteLine("\t"+vl.ToString());
                if (rasterUtil.isNullData(vl, noDataValue))
                {
                    continue;
                }
                else
                {
                    s += vl;
                    s2 += vl * vl;
                }
            }
            return Math.Sqrt((s2 - ((s * s) / offsetLst.Count)) / offsetLst.Count);
        }

    }
}
