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
    class localMinBandFunction : localFunctionBase
    {
        public override bool getOutPutVl(System.Array[] inArr, int c, int r, out float minB)
        {
            int bands = inArr.Length;
            bool checkNoData = true;
            float minVl = float.MaxValue;
            minB = 0;
            for (int i = 0; i < bands; i++)
            {
                object objVl = inArr[i].GetValue(c, r);
                if (objVl == null)
                {
                    checkNoData = false;
                    minB = 0;
                    break;
                }
                else
                {
                    float vl = System.Convert.ToSingle(objVl);
                    if (vl < minVl)
                    {
                        minVl = vl;
                        minB = i;
                    }
                }
                

            }
            return checkNoData;
        }
    }
}