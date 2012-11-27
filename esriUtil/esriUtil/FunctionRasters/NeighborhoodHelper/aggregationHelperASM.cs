using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class aggregationHelperASM : aggregationFunctionDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClms, int startRws, int cells, float noDataValue)
        {
            return blockHelperStats.getBlockAsm(bigArr, startClms, startRws, cells, noDataValue);
        }

    }
}
