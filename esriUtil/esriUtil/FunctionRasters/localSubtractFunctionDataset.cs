using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters
{
    class localSubtractFunctionDataset : localFunctionBase
    {
        public override bool getOutPutVl(IPixelBlock3 coefPb, int c, int r, out float sumVl)
        {
            sumVl = 0;
            bool checkNoData = true;
            for (int i = 0; i < coefPb.Planes; i++)
            {
                object objVl = coefPb.GetVal(i, c, r);
                if (objVl == null)
                {
                    checkNoData = false;
                    sumVl = 0;
                    break;
                }
                else
                {
                    float vl = (float)objVl;
                    sumVl -= vl;
                }

            }
            if (!checkNoData)
            {
                sumVl = 0;
            }
            return checkNoData;
        }
    }
}
