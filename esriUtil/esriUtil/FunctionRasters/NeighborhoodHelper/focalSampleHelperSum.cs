using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperSum : focalSampleDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClm, int startRw)
        {
            //Console.WriteLine("Start CR = " + startClm.ToString()+":"+ startRw.ToString());
            float db = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                //Console.WriteLine("\tOffset CR = " + bWc.ToString() + " : " + bRc.ToString());
                float vl = System.Convert.ToSingle(bigArr.GetValue(bWc, bRc));
                //Console.WriteLine("\t"+vl.ToString());
                if (rasterUtil.isNullData(vl, noDataValue))
                {
                    continue;
                }
                else
                {
                    db += vl;
                }
            }
            return db;
        }

    }
}
