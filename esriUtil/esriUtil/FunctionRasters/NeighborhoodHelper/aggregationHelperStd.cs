using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class aggregationHelperStd : aggregationFunctionDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClms, int startRws, int cells, float noDataValue)
        {
            return blockHelperStats.getBlockStd(bigArr, startClms, startRws, cells, noDataValue);
        }

    }
}
