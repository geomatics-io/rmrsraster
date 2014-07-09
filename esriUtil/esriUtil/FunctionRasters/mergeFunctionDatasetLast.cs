using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class mergeFunctionDatasetLast:mergeFunctionDatasetBase
    {
        public override object getValue(int i, int c, int r, List<ESRI.ArcGIS.Geodatabase.IPixelBlock> inPbValueLst, int cntLst = 0)
        {
            int rV = (inPbValueLst.Count-1) - cntLst;
            object inVl = 0;
            if (rV < 0)
            {
                return inVl;
            }
            object inVlobj = inPbValueLst[rV].GetVal(i, c, r);
            if (inVlobj == null)
            {
                inVl = getValue(i, c, r, inPbValueLst, cntLst +1);

            }
            else
            {
                inVl = inVlobj;
            }
            return inVl;
        }
    }
}
