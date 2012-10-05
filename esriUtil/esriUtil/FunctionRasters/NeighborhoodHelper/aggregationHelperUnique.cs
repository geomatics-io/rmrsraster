using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class aggregationHelperUnique : aggregationFunctionDataset
    {
        public override object getTransformedValue(System.Array bigArr, int startClms, int startRws, int cells, double noDataValue)
        {
            return blockHelperStats.getBlockUnique(bigArr, startClms, startRws, cells, noDataValue);
        }

    }
}