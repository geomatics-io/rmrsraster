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
    class localPowFunctionDataset : localFunctionBase
    {
        public override bool getOutPutVl(System.Array[] inArr, int c, int r, out float sumVl)
        {
            int bands = inArr.Length;
            bool checkNoData = true;
            sumVl = 0;
            for (int i = 0; i < bands; i++)
            {
                object objVl = inArr[i].GetValue(c, r);
                if (objVl == null)
                {
                    checkNoData = false;
                    sumVl = 0;
                    break;
                }
                else
                {
                    float vl = System.Convert.ToSingle(objVl);
                    if (i == 0)
                    {
                        sumVl = vl;
                    }
                    else
                    {
                        sumVl = System.Convert.ToSingle(Math.Pow(sumVl,vl));
                    }
                }

            }
            return checkNoData;
        }
    }
}
