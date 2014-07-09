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
    class localModeFunctionDataset : localFunctionBase
    {
        public override bool getOutPutVl(IPixelBlock3 coefPb, int c, int r, out float mode)
        {
            bool checkNoData = true;
            Dictionary<float, int> probDic = new Dictionary<float, int>();
            mode = 0;
            int cnt = 0;
            for (int i = 0; i < coefPb.Planes; i++)
            {
                object objVl = coefPb.GetVal(i, c, r);
                if (objVl == null)
                {
                    checkNoData = false;
                    break;
                }
                else
                {
                    float vl = (float)objVl;
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
                int maxCnt = probDic.Values.Max();
                foreach (KeyValuePair<float, int> kVp in probDic)
                {
                    int dicVl = kVp.Value;
                    if (dicVl == maxCnt)
                    {
                        mode = kVp.Key;
                        break;
                    }
                }
            }
            return checkNoData;
        }
    }
}