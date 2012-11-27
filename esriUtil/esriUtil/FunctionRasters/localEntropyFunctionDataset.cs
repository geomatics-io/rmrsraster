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
    class localEntropyFunctionDataset : localFunctionBase
    {
        public override void updateOutArr(ref System.Array outArr, ref List<System.Array> pArr)
        {
            int pBWidth = outArr.GetUpperBound(0) + 1;
            int pBHeight = outArr.GetUpperBound(1) + 1;
            for (int i = 0; i < pBHeight; i++)
            {
                for (int k = 0; k < pBWidth; k++)
                {
                    Dictionary<double,int> entDic = new Dictionary<double,int>();
                    float vl = System.Convert.ToSingle(pArr[0].GetValue(k, i));
                    float entropy = 0;
                    int cnt = 0;
                    if (rasterUtil.isNullData(vl, System.Convert.ToSingle(noDataValueArr.GetValue(0))))
                    {
                        continue;
                    }
                    entDic[vl]=1;
                    for (int nBand = 1; nBand < pArr.Count; nBand++)
                    {
                        float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                        vl = System.Convert.ToSingle(pArr[nBand].GetValue(k, i));
                        if (rasterUtil.isNullData(vl, noDataValue))
                        {
                            entropy = noDataVl;
                            break;
                        }
                        if (entDic.TryGetValue(vl,out cnt))
                        {
                            entDic[vl]=cnt+1;
                        }
                        else
                        {
                            entDic.Add(vl,1);
                        }
                    }
                    foreach (int entCnt in entDic.Values)
                        {
                            float prob = System.Convert.ToSingle(entCnt)/pArr.Count;
                            entropy = entropy + (prob * System.Convert.ToSingle(Math.Log(prob)));
                        }
                        entropy = -1 * entropy;

                    outArr.SetValue(entropy,k,i);
                }
            }
        }
    }
}
        
