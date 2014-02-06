using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;


namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class aggregationHelperSum : aggregationFunctionDataset
    {
        public override object getTransformedValue(IPixelBlock3 bigArr, int band, int startClms, int startRws, int cells, object noDataVl)
        {
            return blockHelperStats.getBlockSum(bigArr, band, startClms, startRws, cells, noDataVl);
        }

    }
}