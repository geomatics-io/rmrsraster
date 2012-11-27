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
        public override void updateOutArr(ref System.Array outArr, ref List<System.Array> pArr)
        {
            int pBWidth = outArr.GetUpperBound(0) + 1;
            int pBHeight = outArr.GetUpperBound(1) + 1;
            for (int i = 0; i < pBHeight; i++)
            {
                for (int k = 0; k < pBWidth; k++)
                {
                    float sumVl = System.Convert.ToSingle(pArr[0].GetValue(k, i));
                    if (rasterUtil.isNullData(sumVl, System.Convert.ToSingle(noDataValueArr.GetValue(0))))
                    {
                        continue;
                    }
                    for (int nBand = 0; nBand < pArr.Count; nBand++)
                    {
                        float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                        float pixelValue = System.Convert.ToSingle(pArr[nBand].GetValue(k, i));
                        if (rasterUtil.isNullData(pixelValue, noDataValue))
                        {
                            sumVl = noDataVl;
                            break;
                        }
                        sumVl = System.Convert.ToSingle(Math.Pow(sumVl,pixelValue));
                    }
                    outArr.SetValue(sumVl, k, i);
                }
            }
        }
    }
}
