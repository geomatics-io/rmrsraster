using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperMean : focalFunctionDatasetBase
    {
        public override void updatePixelRectangle(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            int rs = pbInBig.Height;
            int cs = pbInBig.Width;
            int hc = clms - 1;
            int hr = rws - 1;
            int scs = pbIn.Width;
            int srs = pbIn.Height;
            Queue<float[]>[] windowQueue = new Queue<float[]>[pbIn.Planes];
            for (int b = 0; b < pbIn.Planes; b++)
			{
                System.Array upArr = (System.Array)pbIn.get_PixelData(b); ;
                Queue<float[]> queB = new Queue<float[]>();
                windowQueue[b] = queB;
                for (int r = 0; r < rws; r++)
                {
                    int nr = r - hr;
                    float[] sumNewBigArr = new float[scs];

                    float sumVl = 0;
                    for (int c = 0; c < clms; c++)
                    {
                        
                        object objBvl = pbInBig.GetVal(b, c, r);
                        float bVl = 0;
                        if (objBvl != null)
                        {
                            bVl = (float)objBvl;
                        }
                        sumVl += bVl;
                    }
                    sumNewBigArr[0] = sumVl;
                    for (int c = clms; c < cs; c++)
                    {
                        int nc = c - hc;
                        int pc = c - clms;
                        object bVlobj = pbInBig.GetVal(b, c, r);
                        object pVlobj = pbInBig.GetVal(b, pc, r);
                        float bVl = 0;
                        float pVl = 0;
                        if (bVlobj!=null)
                        {
                            bVl = (float)bVlobj;
                        }
                        if (pVlobj!=null)
                        {
                            pVl = (float)pVlobj;
                        }
                        sumVl += bVl - pVl;
                        try
                        {
                            sumNewBigArr[nc] = sumVl;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            System.Windows.Forms.MessageBox.Show(e.ToString());

                        }
                    }
                    queB.Enqueue(sumNewBigArr);
                }
                updateFirstRow(queB, ref upArr, ref pbIn);
                for (int r = rws; r < rs; r++)
                {
                    int pr = r-rws;
                    int nr = r - hr;
                    float[] sumNewBigArr = new float[scs];
                    float sumVl = 0;
                    //first 3 values 
                    for (int c = 0; c < clms; c++)
                    {
                        object bVlobj = pbInBig.GetVal(b, c, r);
                        float bVl = 0;
                        if (bVlobj!=null)
                        {
                            bVl = (float)bVlobj;
                        }
                        sumVl += bVl;
                    }
                    sumNewBigArr[0] = sumVl;
                    float oldSumVl = queB.Peek()[0];
                    float pSmallArrValue = (float)upArr.GetValue(0,pr);//pSmallArrValues[0];
                    float nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                    upArr.SetValue(nSmallArrValue, 0, nr);
                    //pSmallArrValues[0] = nSmallArrValue;
                    for (int c = clms; c < cs; c++)
                    {
                        int nc = c - hc;
                        int pc = c - clms;
                        object bVlobj = pbInBig.GetVal(b, c, r);
                        object pVlobj = pbInBig.GetVal(b, pc, r);
                        float bVl = 0;
                        float pVl = 0;
                        if (bVlobj!=null)
                        {
                            bVl = (float)bVlobj;
                        }
                        if (pVlobj!=null)
                        {
                            pVl = (float)pVlobj;
                        }
                        sumVl += bVl - pVl;
                        sumNewBigArr[nc] = sumVl;
                        oldSumVl = queB.Peek()[nc];
                        pSmallArrValue = (float)upArr.GetValue(nc, pr); ;// pSmallArrValues[nc];
                        nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
                        try
                        {
                            upArr.SetValue(nSmallArrValue, nc, nr);
                            //pSmallArrValues[nc] = nSmallArrValue;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Error in setting pSmallArrValues");
                            Console.WriteLine(e.ToString());
                            System.Windows.Forms.MessageBox.Show(e.ToString());
                        }
                    }
                    queB.Enqueue(sumNewBigArr);
                    queB.Dequeue();
                }
                pbIn.set_PixelData(b, upArr);
            }
        }

        private void updateFirstRow(Queue<float[]> queB, ref System.Array updateArr, ref ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn)
        {
            for (int c = 0; c < pbIn.Width; c++)
            {
                float sumVl = 0;
                for (int r = 0; r < rws; r++)
                {
                    sumVl += queB.ElementAt(r)[c];
                }
                updateArr.SetValue(sumVl, c, 0);
            }
            
        } 
    }
}
        //public override void getTransformedValuesCircle(System.Array bigArr, System.Array smallArr)
        //{
        //    getTransformedValuesRectangle(bigArr, smallArr);
            
        //}
        
        //public override void getTransformedValuesRectangle(System.Array bigArr, System.Array smallArr)
        //{
        //    int rs = bigArr.GetUpperBound(1) + 1;
        //    int cs = bigArr.GetUpperBound(0) + 1;
        //    int hc = clms-1;
        //    int hr = rws - 1;
        //    int scs = smallArr.GetUpperBound(0)+1;
        //    int srs = smallArr.GetUpperBound(1)+1;
        //    float[] pSmallArrValues = new float[scs];
        //    Queue<float[]> windowQueue = new Queue<float[]>();
        //    for (int r = 0; r < rws; r++)
        //    {
        //        int nr = r - hr;
        //        float[] sumNewBigArr = new float[scs];
        //        float sumVl = 0;
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            sumVl += bVl;
        //        }
        //        sumNewBigArr[0] = sumVl;
        //        for (int c = clms; c < cs; c++)
        //        {
        //            int nc = c - hc;
        //            int pc = c - clms;
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            float pVl = System.Convert.ToSingle(bigArr.GetValue(pc, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            if (rasterUtil.isNullData(pVl, noDataValue))
        //            {
        //                pVl = 0;
        //            }
        //            sumVl += bVl - pVl;
        //            try
        //            {
        //                sumNewBigArr[nc] = sumVl;
        //            }
        //            catch(Exception e)
        //            {
        //                Console.WriteLine(e.ToString());

        //            }
        //        }
        //        windowQueue.Enqueue(sumNewBigArr);
        //    }
        //    updateFirstRow(windowQueue,smallArr,ref pSmallArrValues);
        //    for (int r = rws; r < rs; r++)
        //    {
        //        int pr = r-rws;
        //        int nr = r - hr;
        //        float[] sumNewBigArr = new float[scs];
        //        float sumVl = 0;
        //        //first 3 values 
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            sumVl += bVl;
        //        }
        //        sumNewBigArr[0] = sumVl;
        //        float oldSumVl = windowQueue.Peek()[0];
        //        float pSmallArrValue = pSmallArrValues[0];
        //        float nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
        //        smallArr.SetValue(nSmallArrValue/windowN, 0, nr);
        //        pSmallArrValues[0] = nSmallArrValue;
        //        for (int c = clms; c < cs; c++)
        //        {
        //            int nc = c - hc;
        //            int pc = c - clms;
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            float pVl = System.Convert.ToSingle(bigArr.GetValue(pc, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            if (rasterUtil.isNullData(pVl, noDataValue))
        //            {
        //                pVl = 0;
        //            }
        //            sumVl += bVl - pVl;
        //            sumNewBigArr[nc] = sumVl;
        //            oldSumVl = windowQueue.Peek()[nc];
        //            pSmallArrValue = pSmallArrValues[nc];
        //            nSmallArrValue = pSmallArrValue + sumVl - oldSumVl;
        //            try
        //            {
        //                smallArr.SetValue(nSmallArrValue/windowN, nc, nr);
        //                pSmallArrValues[nc] = nSmallArrValue;
        //            }
        //            catch(Exception e)
        //            {
        //                Console.WriteLine("Error in setting pSmallArrValues");
        //                Console.WriteLine(e.ToString());
                        
        //            }
        //        }
        //        windowQueue.Enqueue(sumNewBigArr);
        //        windowQueue.Dequeue();
        //    }
        //}

        //private void updateFirstRow(Queue<float[]> windowQueue, System.Array smallArr, ref float[] pSmallArr)
        //{
        //    pSmallArr = windowQueue.ElementAt(0).ToArray();
        //    for (int i = 1; i < windowQueue.Count; i++)
        //    {
        //        float[] aRow = windowQueue.ElementAt(i);
        //        for (int k = 0; k < aRow.Length; k++)
        //        {
        //            float vl = aRow[k];
        //            pSmallArr[k] = pSmallArr[k]+vl;
        //        }
        //    }
        //    for (int k = 0; k < pSmallArr.Length; k++)
        //    {
        //        float vl = pSmallArr[k];
        //        smallArr.SetValue(vl/windowN,k,0);
        //    }
        //}
       
    //}
//}
