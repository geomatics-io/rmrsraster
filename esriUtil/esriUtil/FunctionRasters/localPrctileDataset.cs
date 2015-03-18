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
    class localPrctileDataset : localRescaleFunctionBase
    {
        public override void getOutPutVl(IPixelBlock3 pb, System.Array[] inArr, float vl, int c, int r)
        {
            float[] orgArr = new float[bndCnt];
            float[] sortArr = new float[bndCnt];
            orgArr[0] = vl;
            sortArr[0] = vl;
            for (int p = 1; p < bndCnt; p++)
            {
                float vl2 = System.Convert.ToSingle(inArr[p].GetValue(c,r));
                orgArr[p] = vl2;
                sortArr[p] = vl2; 
            }
            System.Array.Sort(sortArr);
            for (int p = 0; p < bndCnt; p++)
            {
                rstPixelType ptyp = pb.get_PixelType(p);
                float orgVl = orgArr[p];
                int indexSort = System.Array.LastIndexOf(sortArr, orgVl);
                float prc = indexSort / bndCntf;
                object newVl = rasterUtil.getSafeValue(prc, ptyp);
                inArr[p].SetValue(newVl,c,r);
            }
                
            //int[] cntLst = new int[100];
            //float[] probLst = new float[100];
            //float[] orgLst = new float[bndCnt];
            //int[] indexvl = new int[bndCnt];
            //float min = float.MaxValue;
            //float max = float.MinValue;
            //float TCnt = 0;
            //for (int i = 0; i < coefPb.Planes; i++)
            //{
            //    object objVl = coefPb.GetVal(i, c, r);
            //    if (objVl != null)
            //    {
            //        float vl = (float)objVl;
            //        orgLst[i] = vl;
            //        if (vl > max) max = vl;
            //        if (vl < min) min = vl;
            //        TCnt+=1;
            //    }
            //}
            //float rng = max - min;
            //for (int i = 0; i < orgLst.Length; i++)
            //{
            //    int indexVl = (int)(99*(orgLst[i] - min) / rng);
            //    indexvl[i] = indexVl;
            //    cntLst[indexVl] = cntLst[indexVl] + 1;   
            //}
            //int sumCnt = 0;
            //for (int i = 0; i < cntLst.Length; i++)
            //{
            //    sumCnt = sumCnt + cntLst[i];
            //    probLst[i] = sumCnt/TCnt;
            //}
            //for (int i = 0; i < orgLst.Length; i++)
            //{
            //    updateArr[i].SetValue(probLst[indexvl[i]], c, r);
            //}
        }

    }
}