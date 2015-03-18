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
    class localMedianFunctionDataset : localFunctionBase
    {
        public override bool getOutPutVl(System.Array[] inArr, int c, int r, out float median)
        {
            int bands = inArr.Length;
            bool checkNoData = true;
            Dictionary<float, int> probDic = new Dictionary<float, int>();
            median = 0;
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
                List<float> kysLst = probDic.Keys.ToList();
                kysLst.Sort();
                int sumCnt = 0;
                int compVl = bands / 2;
                foreach (float kyVl in kysLst)
                {
                    sumCnt += probDic[kyVl];
                    if (sumCnt >= compVl)
                    {
                        median = kyVl;
                        break;
                    }
                }
            }
            return checkNoData;
        }
    }
}