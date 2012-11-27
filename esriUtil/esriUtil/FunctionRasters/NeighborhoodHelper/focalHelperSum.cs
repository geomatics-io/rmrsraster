using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperSum: focalFunctionDatasetBase
    {

        public override void getTransformedValuesCircle(System.Array bigArr, System.Array smallArr)
        {
            getTransformedValuesRectangle(bigArr, smallArr);
        }
        
        public override void getTransformedValuesRectangle(System.Array bigArr, System.Array smallArr)
        {
            int rs = bigArr.GetUpperBound(1) + 1;
            int cs = bigArr.GetUpperBound(0) + 1;
            int hc = clms-1;
            int hr = rws - 1;
            int scs = smallArr.GetUpperBound(0)+1;
            int srs = smallArr.GetUpperBound(1)+1;
            float[] pSmallArrValues = new float[scs];
            Queue<float[]> windowQueue = new Queue<float[]>();
            for (int r = 0; r < rws; r++)
            {
                int nr = r - hr;
                float[] sumNewBigArr = new float[scs];
                float sumVl = 0;
                for (int c = 0; c < clms; c++)
                {
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    sumVl += bVl;
                }
                sumNewBigArr[0] = sumVl;
                for (int c = clms; c < cs; c++)
                {
                    int nc = c - hc;
                    int pc = c - clms;
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    float pVl = System.Convert.ToSingle(bigArr.GetValue(pc, r));
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    if (rasterUtil.isNullData(pVl, noDataValue))
                    {
                        pVl = 0;
                    }
                    sumVl += bVl - pVl;
                    try
                    {
                        sumNewBigArr[nc] = sumVl;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());

                    }
                }
                windowQueue.Enqueue(sumNewBigArr);
            }
            updateFirstRow(windowQueue,smallArr,ref pSmallArrValues);
            for (int r = rws; r < rs; r++)
            {
                int pr = r-rws;
                int nr = r - hr;
                float[] sumNewBigArr = new float[scs];
                float sumVl = 0;
                //first 3 values 
                for (int c = 0; c < clms; c++)
                {
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    sumVl += bVl;
                }
                sumNewBigArr[0] = sumVl;
                float oldSumVl = windowQueue.Peek()[0];
                float pSmallArrValue = pSmallArrValues[0];
                float nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                smallArr.SetValue(nSmallArrValue, 0, nr);
                pSmallArrValues[0] = nSmallArrValue;
                for (int c = clms; c < cs; c++)
                {
                    int nc = c - hc;
                    int pc = c - clms;
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    float pVl = System.Convert.ToSingle(bigArr.GetValue(pc, r));
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    if (rasterUtil.isNullData(pVl, noDataValue))
                    {
                        pVl = 0;
                    }
                    sumVl += bVl - pVl;
                    sumNewBigArr[nc] = sumVl;
                    oldSumVl = windowQueue.Peek()[nc];
                    pSmallArrValue = pSmallArrValues[nc];
                    nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                    try
                    {
                        smallArr.SetValue(nSmallArrValue, nc, nr);
                        pSmallArrValues[nc] = nSmallArrValue;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error in setting pSmallArrValues");
                        Console.WriteLine(e.ToString());
                        
                    }
                }
                windowQueue.Enqueue(sumNewBigArr);
                windowQueue.Dequeue();
            }
        }

        private void updateFirstRow(Queue<float[]> windowQueue, System.Array smallArr, ref float[] pSmallArr)
        {
            pSmallArr = windowQueue.ElementAt(0).ToArray();
            for (int i = 1; i < windowQueue.Count; i++)
            {
                float[] aRow = windowQueue.ElementAt(i);
                for (int k = 0; k < aRow.Length; k++)
                {
                    float vl = aRow[k];
                    pSmallArr[k] = pSmallArr[k]+vl;
                }
            }
            for (int k = 0; k < pSmallArr.Length; k++)
            {
                float vl = pSmallArr[k];
                smallArr.SetValue(vl,k,0);
            }
        }
       
    }
}
