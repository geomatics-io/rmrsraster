using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class mergeFunctionDatasetMin : mergeFunctionDatasetBase
    {
        public override object getValue(int i, int c, int r, List<ESRI.ArcGIS.Geodatabase.IPixelBlock> inPbValueLst, int cntLst = 0)
        {
            object minVl = Single.MaxValue;
            foreach (ESRI.ArcGIS.Geodatabase.IPixelBlock pb in inPbValueLst)
            {
                object inVlobj = pb.GetVal(i, c, r);
                if (inVlobj == null) continue;
                else
                {
                    if (System.Convert.ToSingle(inVlobj) < System.Convert.ToSingle(minVl)) minVl = inVlobj;
                }
            }
            if (System.Convert.ToSingle(minVl) == Single.MaxValue) minVl = noDataVl;
            return minVl;
        }
    }
}
