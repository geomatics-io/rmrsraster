using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperUnique : focalFunctionDatasetBase
    {
        private void setPixelData(int p, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            System.Array pbArr = (System.Array)pbIn.get_PixelData(p);
            for (int r = 0; r < pbIn.Height; r++)
            {
                for (int c = 0; c < pbIn.Width; c++)
                {
                    HashSet<float> hash = new HashSet<float>();
                    for (int rb = 0; rb < rws; rb++)
                    {
                        int nrb = r + rb;
                        for (int cb = 0; cb < clms; cb++)
                        {
                            int ncb = c + cb;
                            object objVl = pbInBig.GetVal(p, ncb, nrb);
                            if (objVl != null)
                            {
                                float vl = System.Convert.ToSingle(objVl);
                                hash.Add(vl);
                            }
                        }
                    }
                    
                    pbArr.SetValue(hash.Count, c, r);
                }

            }
            pbIn.set_PixelData(p, pbArr);
        }
        private void setPixelDataFolding(int p, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            System.Array pbArr = (System.Array)pbIn.get_PixelData(p);
            HashSet<float>[][] wrDic1 = new HashSet<float>[pbIn.Width][];
            //create first dictionary and set first values
            for (int w = 0; w < pbIn.Width; w++)
            {
                wrDic1[w] = new HashSet<float>[rws];
                for (int r = 0; r < rws; r++)
                {
                    wrDic1[w][r] = new HashSet<float>();
                    HashSet<float> fDic = wrDic1[w][r];
                    for (int c = 0; c < clms; c++)
                    {
                        object objVl = pbInBig.GetVal(p, c, r);
                        if (objVl != null)
                        {
                            float vl = System.Convert.ToSingle(objVl);
                            fDic.Add(vl);
                        }
                    }
                }
                setValues(pbArr, wrDic1[w], w, 0);
            }
            //create next dictionary and set the rest of the values
            int clmsM = clms - 1;
            int rwsM = rws - 1;
            HashSet<float>[][] wrDic2 = new HashSet<float>[pbIn.Width][];
            //wrDic2[0][0] = wrDic1[0][rwsM];
            for (int r = 1; r < pbIn.Height; r++)
            {
                for (int c = 0; c < pbIn.Width; c++)
                {
                    HashSet<float> clmsDic;
                    wrDic2[c] = new HashSet<float>[rws];
                    for (int rb = 1; rb < rws; rb++)
                    {
                        wrDic2[c][rb - 1] = new HashSet<float>(wrDic1[c][rb]);
                    }
                    int nrb = r + rwsM;
                    if (c > 0)// copy previous dictionary on same row and remove/add values
                    {
                        wrDic2[c][rwsM] = new HashSet<float>(wrDic2[c - 1][rwsM]);
                        clmsDic = wrDic2[c][rwsM];
                        int ncb = c + clmsM;
                        object objVln = pbInBig.GetVal(p, ncb, nrb);
                        object objVlo = pbInBig.GetVal(p, c - 1, nrb);
                        if (objVlo != null)//remove old value
                        {
                            float vlo = System.Convert.ToSingle(objVlo);
                            clmsDic.Remove(vlo);
                        }
                        if (objVln != null)//add new value
                        {
                            float vln = System.Convert.ToSingle(objVln);
                            clmsDic.Add(vln);

                        }
                    }
                    else //first column, need to get all numbers of new row
                    {
                        wrDic2[c][rwsM] = new HashSet<float>();
                        HashSet<float> fDic = wrDic2[c][rwsM];
                        for (int c2 = 0; c2 < clms; c2++)
                        {
                            object objVl = pbInBig.GetVal(p, c2, nrb);
                            if (objVl != null)
                            {
                                float vl = System.Convert.ToSingle(objVl);
                                fDic.Add(vl);
                            }
                        }
                    }
                    setValues(pbArr, wrDic2[c], c, r);
                }
            }
            pbIn.set_PixelData(p, pbArr);
            wrDic1 = wrDic2;

        }

        private void setValues(System.Array pbArr, HashSet<float>[] wrDicClm, int c, int r)
        {
            HashSet<float> hash = new HashSet<float>(wrDicClm[0]);
            for (int d = 1; d < rws; d++)
            {
                foreach (float ky in wrDicClm[d])
                {
                    hash.Add(ky);
                }
            }
            pbArr.SetValue(hash.Count, c, r);
        }
        public override void updatePixelRectangle(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {

            for (int p = 0; p < pbIn.Planes; p++)
            {
                //setPixelDataFolding(p, pbIn, pbInBig);
                setPixelData(p, pbIn, pbInBig);
            }
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
        //    Queue<List<float>[]> windowQueue = new Queue<List<float>[]>();

        //    for (int r = 0; r < rws; r++)
        //    {
        //        int nr = r - hr;
        //        List<float>[] valuesNewBigArr = new List<float>[scs];
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
        //        valuesNewBigArr[0] = vQueue.ToList();
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
        //            valuesNewBigArr[nc] = vQueue.ToList();
        //        }
        //        windowQueue.Enqueue(valuesNewBigArr);
        //    }
        //    updateFirstRow(windowQueue, smallArr);
        //    for (int r = rws; r < rs; r++)
        //    {
        //        int pr = r - rws;
        //        int nr = r - hr;
        //        List<float>[] valuesNewBigArr = new List<float>[scs];
        //        Queue<float> vQueue = new Queue<float>();
        //        //first 3 values 
        //        HashSet<float> hash = new HashSet<float>();
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = Single.MinValue;
        //            }
        //            hash.Add(bVl);
        //            vQueue.Enqueue(bVl);
        //        }
        //        valuesNewBigArr[0] = vQueue.ToList();
                
        //        windowQueue.Dequeue();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            foreach (float pVl in windowQueue.ElementAt(i)[0])
        //            {
        //                hash.Add(pVl);
        //            }
        //        }
        //        smallArr.SetValue(hash.Count, 0, nr);
        //        //dic.Clear();
        //        for (int c = clms; c < cs; c++)
        //        {
        //            int nc = c - hc;
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = 0;
        //            }
        //            vQueue.Enqueue(bVl);
        //            float pVl = vQueue.Dequeue();
        //            valuesNewBigArr[nc] = vQueue.ToList();
        //            //remove previous clms and add new columns and value to dic
        //            float uVl = updateHash(ref hash, ref windowQueue, ref bVl, ref pVl, nc);
        //            smallArr.SetValue(uVl, nc, nr);
        //        }
        //        windowQueue.Enqueue(valuesNewBigArr);
        //    }
        //}

        //private float updateHash(ref HashSet<float> hash, ref Queue<List<float>[]> windowQueue, ref float bVl, ref float pVl, int nc)
        //{
        //    int pClm = nc - 1;
        //    int maxClmIndex = clms-1;
        //    //remove the values from the dictionary
        //    hash.Remove(pVl);
        //    for(int i=0;i<windowQueue.Count;i++)
        //    {
        //        float pclmsVl = windowQueue.ElementAt(i)[pClm][0];
        //        hash.Remove(pclmsVl);
        //    }
        //    //add the values to the dictionary
        //    hash.Add(bVl);
        //    for (int i = 0; i < windowQueue.Count; i++)
        //    {
        //        float nclmsVl = windowQueue.ElementAt(i)[nc][maxClmIndex];
        //        hash.Add(nclmsVl);
        //    }
        //    return hash.Count;
        //}

        //private void updateFirstRow(Queue<List<float>[]> windowQueue, System.Array smallArr)
        //{

        //    for (int k = 0; k <= smallArr.GetUpperBound(0); k++)
        //    {
        //        HashSet<float> hash = new HashSet<float>();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            foreach (float d in windowQueue.ElementAt(i)[k])
        //            {
        //                hash.Add(d);
        //            }
        //        }
        //        smallArr.SetValue(hash.Count, k, 0);
        //    }
        //}
    }
}