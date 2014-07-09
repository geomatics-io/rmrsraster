using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalSampleHelperMin : focalSampleDataset
    {
        public override object getTransformedValue(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 bigArr, int startClm, int startRw, int nBand)
        {
            float db = Single.MaxValue;
            foreach (int[] xy in offsetLst)
            {
                int bWc = xy[0] + startClm;
                int bRc = xy[1] + startRw;
                object objVl = bigArr.GetVal(nBand, bWc, bRc);
                
                if (objVl!=null)
                {
                    float vl = (float)objVl;
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
