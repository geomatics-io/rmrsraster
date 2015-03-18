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
        public override bool getOutPutVl(System.Array[] inArr, int c, int r, out float sProbability)
        {
            int bands = inArr.Length;
            bool checkNoData = true;
            Dictionary<float, int> probDic = new Dictionary<float, int>();
            sProbability = 0;
            int cnt = 0;
            for (int i = 0; i < bands; i++)
            {
                object objVl = inArr[i].GetValue(c, r);
                if (objVl == null)
                {
                    checkNoData = false;
                    break;
                }
                else
                {
                    float vl = System.Convert.ToSingle(objVl);
                    if (probDic.TryGetValue(vl, out cnt))
                    {
                        probDic[vl] = cnt + 1;
                    }
                    else
                    {
                        probDic.Add(vl, 1);
                    }

                }

            }
            if (checkNoData)
            {
                foreach (int prbCnt in probDic.Values)
                {
                    float prob = System.Convert.ToSingle(prbCnt) / bands;
                    sProbability += sProbability + (prob * System.Convert.ToSingle(Math.Log(prob)));
                }
                sProbability = -1 * sProbability;
            }
            return checkNoData;
        }
        
    }
}
        
