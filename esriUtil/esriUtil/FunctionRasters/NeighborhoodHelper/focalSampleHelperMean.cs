using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMean : focalSampleDataset
    {
        public override object getTransformedValue( ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 bigArr, int startClm, int startRw, int nBand)
        {
            //Console.WriteLine("Start CR = " + startClm.ToString()+":"+ startRw.ToString());
            float db = 0;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                object objVl = bigArr.GetVal(nBand,bWc, bRc);
                
                //Console.WriteLine("\t"+vl.ToString());
                if (objVl==null)
                {
                    continue;
                }
                else
                {
                    db += (float)objVl;
                }
            }
            return db/offsetLst.Count;
        }

    }
}
