using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class mergeFunctionDatasetMax : mergeFunctionDatasetBase
    {
        public override object getValue(int i, int c, int r, List<ESRI.ArcGIS.Geodatabase.IPixelBlock> inPbValueLst, int cntLst = 0)
        {
            object maxVl = Single.MinValue;
            foreach (ESRI.ArcGIS.Geodatabase.IPixelBlock pb in inPbValueLst)
            {
                object inVlobj = pb.GetVal(i, c, r);
                if (inVlobj == null) continue;
                else
                {
                    if (System.Convert.ToSingle(inVlobj) > System.Convert.ToSingle(maxVl)) maxVl = inVlobj;
                }
            }
            if (System.Convert.ToSingle(maxVl) == Single.MinValue) maxVl = 0;
            return maxVl;
        }
    }
}
