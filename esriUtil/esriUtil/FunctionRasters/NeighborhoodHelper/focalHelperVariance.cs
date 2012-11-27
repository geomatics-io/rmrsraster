using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperVariance : focalFunctionDatasetBase
    {
        public override void getTransformedValuesCircle(System.Array bigArr, System.Array smallArr)
        {
            getTransformedValuesRectangle(bigArr, smallArr);
        }

        public override void getTransformedValuesRectangle(System.Array bigArr, System.Array smallArr)
        {
            int rs = bigArr.GetUpperBound(1) + 1;
            int cs = bigArr.GetUpperBound(0) + 1;
            int hc = clms - 1;
            int hr = rws - 1;
            int scs = smallArr.GetUpperBound(0) + 1;
            int srs = smallArr.GetUpperBound(1) + 1;
            float[] pSmallArrValues = new float[scs];
            float[] pSmallArrValues2 = new float[scs];
            Queue<float[]> windowQueue = new Queue<float[]>();
            Queue<float[]> windowQueue2 = new Queue<float[]>();
            for (int r = 0; r < rws; r++)
            {
                int nr = r - hr;
                float[] sumNewBigArr = new float[scs];
                float[] sumNewBigArr2 = new float[scs];
                float sumVl = 0;
                float sumVl2 = 0;
                for (int c = 0; c < clms; c++)
                {
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    float bVl2 = bVl * bVl;
                    sumVl += bVl;
                    sumVl2 += bVl2;
                }
                sumNewBigArr[0] = sumVl;
                sumNewBigArr2[0] = sumVl2;
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
                    float bVl2 = bVl * bVl;
                    float pVl2 = pVl * pVl;
                    sumVl += bVl - pVl;
                    sumVl2 += bVl2 - pVl2;
                    try
                    {
                        sumNewBigArr[nc] = sumVl;
                        sumNewBigArr2[nc] = sumVl2;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());

                    }
                }
                windowQueue.Enqueue(sumNewBigArr);
                windowQueue2.Enqueue(sumNewBigArr2);
            }
            updateFirstRow(windowQueue, windowQueue2, smallArr, ref pSmallArrValues, ref pSmallArrValues2);
            for (int r = rws; r < rs; r++)
            {
                int pr = r - rws;
                int nr = r - hr;
                float[] sumNewBigArr = new float[scs];
                float[] sumNewBigArr2 = new float[scs];
                float sumVl = 0;
                float sumVl2 = 0;
                //first 3 values 
                for (int c = 0; c < clms; c++)
                {
                    float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
                    
                    if (rasterUtil.isNullData(bVl, noDataValue))
                    {
                        bVl = 0;
                    }
                    float bVl2 = bVl * bVl;
                    sumVl += bVl;
                    sumVl2 += bVl2;
                }
                sumNewBigArr[0] = sumVl;
                sumNewBigArr2[0] = sumVl2;
                float oldSumVl = windowQueue.Peek()[0];
                float oldSumVl2 = windowQueue2.Peek()[0];
                float pSmallArrValue = pSmallArrValues[0];
                float pSmallArrValue2 = pSmallArrValues2[0];
                float nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                float nSmallArrValue2 = pSmallArrValue2 + sumVl2 - oldSumVl2;
                float uVl = (nSmallArrValue2 - ((nSmallArrValue * nSmallArrValue) / windowN)) / windowN;
                smallArr.SetValue(uVl, 0, nr);
                pSmallArrValues[0] = nSmallArrValue;
                pSmallArrValues2[0] = nSmallArrValue2;
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
                    float bVl2 = bVl * bVl;
                    float pVl2 = pVl * pVl;
                    sumVl += bVl - pVl;
                    sumVl2 += bVl2 - pVl2;
                    sumNewBigArr[nc] = sumVl;
                    sumNewBigArr2[nc] = sumVl2;
                    oldSumVl = windowQueue.Peek()[nc];
                    oldSumVl2 = windowQueue2.Peek()[nc];
                    pSmallArrValue = pSmallArrValues[nc];
                    pSmallArrValue2 = pSmallArrValues2[nc];
                    nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                    nSmallArrValue2 = pSmallArrValue2 + sumVl2 - oldSumVl2;
                    try
                    {
                        uVl = (nSmallArrValue2 - ((nSmallArrValue * nSmallArrValue) / windowN)) / windowN;
                        smallArr.SetValue(nSmallArrValue, nc, nr);
                        pSmallArrValues[nc] = nSmallArrValue;
                        pSmallArrValues2[nc] = nSmallArrValue2;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in setting pSmallArrValues");
                        Console.WriteLine(e.ToString());

                    }
                }
                
                windowQueue.Enqueue(sumNewBigArr);
                windowQueue.Dequeue();
                windowQueue2.Enqueue(sumNewBigArr2);
                windowQueue2.Dequeue();
            }
        }

        private void updateFirstRow(Queue<float[]> windowQueue,Queue<float[]> windowQueue2, System.Array smallArr, ref float[] pSmallArr, ref float[] pSmallArr2)
        {
            pSmallArr = windowQueue.ElementAt(0).ToArray();
            pSmallArr2 = windowQueue2.ElementAt(0).ToArray();
            for (int i = 1; i < windowQueue.Count; i++)
            {
                float[] aRow = windowQueue.ElementAt(i);
                float[] aRow2 = windowQueue2.ElementAt(i);
                for (int k = 0; k < aRow.Length; k++)
                {
                    float vl = aRow[k];
                    float vl2 = aRow2[k];
                    pSmallArr[k] = pSmallArr[k] + vl;
                    pSmallArr2[k] = pSmallArr2[k] + vl2;
                }
            }
            for (int k = 0; k < pSmallArr.Length; k++)
            {
                float vl = pSmallArr[k];
                float vl2 = pSmallArr2[k];
                float uVl = (vl2 - ((vl * vl) / windowN)) / windowN;
                smallArr.SetValue(uVl, k, 0);
            }
        }

    }
}