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
    class localUniqueValuesFunctionDataset : localFunctionBase
    {
        public override void updateOutArr(ref System.Array outArr, ref List<System.Array> pArr)
        {
            int pBWidth = outArr.GetUpperBound(0) + 1;
            int pBHeight = outArr.GetUpperBound(1) + 1;
            for (int i = 0; i < pBHeight; i++)
            {
                for (int k = 0; k < pBWidth; k++)
                {
                    HashSet<double> hash = new HashSet<double>();
                    float vl = System.Convert.ToSingle(pArr[0].GetValue(k, i));
                    if (rasterUtil.isNullData(vl, System.Convert.ToSingle(noDataValueArr.GetValue(0))))
                    {
                        continue;
                    }
                    hash.Add(vl);
                    double unique = 0;
                    for (int nBand = 1; nBand < pArr.Count; nBand++)
                    {
                        double noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                        vl = System.Convert.ToSingle(pArr[nBand].GetValue(k, i));
                        if (rasterUtil.isNullData(vl, noDataValue))
                        {
                            unique = noDataValue;
                            break;
                        }
                        hash.Add(vl);
                    }
                    outArr.SetValue(hash.Count,k, i);
                }
            }
        }
    }
}

