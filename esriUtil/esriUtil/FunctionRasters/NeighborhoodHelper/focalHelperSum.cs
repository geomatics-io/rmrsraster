using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperSum: focalFunctionDatasetBase
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
