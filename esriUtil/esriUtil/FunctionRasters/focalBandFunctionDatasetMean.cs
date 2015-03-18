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
    class focalBandFunctionDatasetMean : focalBandFunctionDatasetBase
    {
        public override void getOutPutVl(System.Array[] updateArr, int c, int r)
        {
            int bndCnt = updateArr.Length;
            float[] origArr = new float[bndCnt];
            float sumVl = 0;
            //need to work on this part
            for (int p = 0; p < bndCnt; p++)
            {
                float vl2 = System.Convert.ToSingle(updateArr[p].GetValue(c, r));
                origArr[p] = vl2;
                sumVl = sumVl + vl2;
                float m = sumVl;
                if (p >= tBands)
                {
                    sumVl = sumVl - origArr[p - tBands];
                }
                m = sumVl / tBands;
                updateArr[p].SetValue(m, c, r);
                
            }
            
        }

    }
}