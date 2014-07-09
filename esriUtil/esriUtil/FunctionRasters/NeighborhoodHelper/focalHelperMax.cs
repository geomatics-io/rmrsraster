using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperMax : focalFunctionDatasetBase
    {
        public override void updatePixelRectangle(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            throw new NotImplementedException();
        }
        //public override void getTransformedValuesCircle(System.Array bigArr, System.Array updateArr)
        //{
        //    getTransformedValuesRectangle(bigArr, updateArr);
        //}
        //public override void getTransformedValuesRectangle(System.Array bigArr, System.Array smallArr)
        //{
        //    int rs = bigArr.GetUpperBound(1) + 1;
        //    int cs = bigArr.GetUpperBound(0) + 1;
        //    int hc = clms - 1;
        //    int hr = rws - 1;
        //    int scs = smallArr.GetUpperBound(0) + 1;
        //    int srs = smallArr.GetUpperBound(1) + 1;
        //    Queue<float[]> windowQueue = new Queue<float[]>();
            
        //    for (int r = 0; r < rws; r++)
        //    {
        //        int nr = r - hr;
        //        float[] sumNewBigArr = new float[scs];
        //        Queue<float> vQueue = new Queue<float>();
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = Single.MinValue;
        //            }
        //            vQueue.Enqueue(bVl);
        //        }
        //        sumNewBigArr[0] = vQueue.Max();
        //        for (int c = clms; c < cs; c++)
        //        {
        //            int nc = c - hc;
        //            int pc = c - clms;
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            vQueue.Enqueue(bVl);
        //            vQueue.Dequeue();
        //            sumNewBigArr[nc] = vQueue.Max();
        //        }
        //        windowQueue.Enqueue(sumNewBigArr);
        //    }
        //    updateFirstRow(windowQueue, smallArr);
        //    for (int r = rws; r < rs; r++)
        //    {
        //        int pr = r - rws;
        //        int nr = r - hr;
        //        float[] sumNewBigArr = new float[scs];
        //        Queue<float> vQueue = new Queue<float>();
        //        //first 3 values 
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = Single.MinValue;
        //            }
        //            vQueue.Enqueue(bVl);
        //        }
        //        float nMax = vQueue.Max();
        //        sumNewBigArr[0] = nMax;
        //        windowQueue.Dequeue();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            float oMax = windowQueue.ElementAt(i)[0];
        //            if (oMax > nMax) nMax = oMax;
        //        }
        //        smallArr.SetValue(nMax, 0, nr);
        //        for (int c = clms; c < cs; c++)
        //        {
        //            int nc = c - hc;
        //            int pc = c - clms;
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            vQueue.Enqueue(bVl);
        //            vQueue.Dequeue();
        //            nMax = vQueue.Max();
        //            sumNewBigArr[nc] = nMax;
        //            for (int i = 0; i < windowQueue.Count; i++)
        //            {
        //                float oMax = windowQueue.ElementAt(i)[nc];
        //                if (oMax > nMax) nMax = oMax;
        //            }
        //            smallArr.SetValue(nMax,nc,nr);
        //        }
        //        windowQueue.Enqueue(sumNewBigArr);
        //    }
        //}

        //private void updateFirstRow(Queue<float[]> windowQueue, System.Array smallArr)
        //{

        //    for(int k = 0; k<=smallArr.GetUpperBound(0);k++)
        //    {
        //        float maxVL = windowQueue.ElementAt(0)[k];
        //        for (int i=1;i<windowQueue.Count;i++)
        //        {
        //            float vl = windowQueue.ElementAt(i)[k];
        //            if (vl > maxVL) maxVL = vl;
        //        }
        //        smallArr.SetValue(maxVL, k, 0);
        //    }
        //}
    }
}

