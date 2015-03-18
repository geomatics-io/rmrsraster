using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class focalBandFunctionDatasetStd : focalBandFunctionDatasetBase
    {
        public override void getOutPutVl(System.Array[] updateArr, int c, int r)
        {
            int bndCnt = updateArr.Length;
            float[] origArr = new float[bndCnt];
            float[] origArr2 = new float[bndCnt];
            float sumVl = 0;
            float sumVl2 = 0;
            //need to work on this part
            for (int p = 0; p < bndCnt; p++)
            {
                float vl = System.Convert.ToSingle(updateArr[p].GetValue(c, r));
                float vl2 = System.Convert.ToSingle(Math.Pow(vl, 2));
                origArr[p] = vl;
                origArr2[p] = vl2;
                sumVl = sumVl + vl;
                sumVl2 = sumVl2 + vl2;
                if (p >= tBands)
                {
                    sumVl = sumVl - origArr[p - tBands];
                    sumVl2 = sumVl2 - origArr2[p - tBands];
                }
                float std = System.Convert.ToSingle(Math.Sqrt((sumVl2 - ((sumVl * sumVl) / tBands)) / tBands));
                updateArr[p].SetValue(std, c, r);
            }

        }

    }
}
