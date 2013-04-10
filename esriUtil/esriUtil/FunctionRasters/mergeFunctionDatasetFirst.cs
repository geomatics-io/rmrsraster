using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class mergeFunctionDatasetFirst: mergeFunctionDatasetBase
    {
        public override object  getValue(int i, int c, int r, List<ESRI.ArcGIS.Geodatabase.IPixelBlock> inPbValueLst, int cntLst = 0)
        {
            object inVl = noDataVl;
            if (cntLst >= inPbValueLst.Count)
            {
                return inVl;
            }
            object inVlobj = inPbValueLst[cntLst].GetVal(i, c, r);
            if (inVlobj == null)
            {
                inVl = getValue(i, c, r, inPbValueLst, cntLst + 1);
                
            }
            else
            {
                inVl = inVlobj;
            }
            return inVl;
        }
    }
}
