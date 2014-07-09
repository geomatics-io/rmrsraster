using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperStd : focalSampleDataset
    {
        public override object getTransformedValue(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 bigArr, int startClm, int startRw, int nBand)
        {
            //Console.WriteLine("Start CR = " + startClm.ToString()+":"+ startRw.ToString());
            float s = 0;
            float s2 = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                object vlObj = bigArr.GetVal(nBand, bWc, bRc);
                
                if (vlObj==null)
                {
                    continue;
                }
                else
                {
                    float vl = (float)vlObj;
                    s += vl;
                    s2 += vl * vl;
                }
            }
            return Math.Sqrt((s2 - ((s * s) / offsetLst.Count)) / offsetLst.Count);
        }

    }
}
