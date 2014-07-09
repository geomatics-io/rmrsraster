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
    class localVarianceFunctionDataset :localFunctionBase
    {
        public override bool getOutPutVl(IPixelBlock3 coefPb, int c, int r, out float var)
        {
            bool checkNoData = true;
            var = 0;
            double sumVl = 0;
            double sumVl2 = 0;
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
                    sumVl += vl;
                    sumVl2 += vl * vl;
                }

            }
            if (checkNoData)
            {
                var = System.Convert.ToSingle((sumVl2 - ((sumVl * sumVl) / coefPb.Planes)) / coefPb.Planes);
            }
            return checkNoData;
        }
    }
}
