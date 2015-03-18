using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class rasterData
    {
        public ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn { get; set; }
        public ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig { get; set; }
        public int p { get; set; }
        public int rws { get; set; }
        public int clms { get; set; }
        public void setPixelValues()
        {
            System.Array pbArr = (System.Array)pbIn.get_PixelData(p);
            for (int r = 0; r < pbIn.Height; r++)
            {
                for (int c = 0; c < pbIn.Width; c++)
                {
                    Dictionary<float, int> vlDic = new Dictionary<float, int>();
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
                                int vlCnt = 1;
                                //Console.WriteLine("From Thread " + p.ToString() + "; " + vl.ToString());
                                if (vlDic.TryGetValue(vl, out vlCnt))
                                {
                                    vlDic[vl] = vlCnt + 1;
                                }
                                else
                                {
                                    vlDic.Add(vl, 1);
                                }

                            }
                        }
                    }
                    int maxCnt = 0;
                    float maxVl = 0;
                    foreach (KeyValuePair<float, int> kvp in vlDic)
                    {
                        int kVl = kvp.Value;
                        //Console.WriteLine("kVl = " + kVl.ToString());
                        if (kVl > maxCnt)
                        {
                            maxVl = kvp.Key;
                            maxCnt = kVl;
                        }
                    }
                    pbArr.SetValue(maxVl, c, r);
                }

            }
            pbIn.set_PixelData(p, pbArr);
        }
    }
    class focalHelperMode : focalFunctionDatasetBase
    {
        private void setPixelData(int p, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            System.Array pbArr = (System.Array)pbIn.get_PixelData(p);
            for (int r = 0; r < pbIn.Height; r++)
            {
                for (int c = 0; c < pbIn.Width; c++)
                {
                    Dictionary<float, int> vlDic = new Dictionary<float, int>();
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
                                int vlCnt = 1;
                                //Console.WriteLine("From Thread " + p.ToString() + "; " + vl.ToString());
                                if (vlDic.TryGetValue(vl, out vlCnt))
                                {
                                    vlDic[vl] = vlCnt + 1;
                                }
                                else
                                {
                                    vlDic.Add(vl, 1);
                                }

                            }
                        }
                    }
                    int maxCnt = 0;
                    float maxVl = 0;
                    foreach (KeyValuePair<float, int> kvp in vlDic)
                    {
                        int kVl = kvp.Value;
                        //Console.WriteLine("kVl = " + kVl.ToString());
                        if (kVl > maxCnt)
                        {
                            maxVl = kvp.Key;
                            maxCnt = kVl;
                        }
                    }
                    pbArr.SetValue(maxVl, c, r);
                }

            }
            pbIn.set_PixelData(p, pbArr);
        }
        private void setPixelDataFolding(int p, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn, ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
        {
            System.Array pbArr = (System.Array)pbIn.get_PixelData(p);
            Dictionary<float, int>[][] wrDic1 = new Dictionary<float, int>[pbIn.Width][];
            //create first dictionary and set first values
            for (int w = 0; w < pbIn.Width; w++)
            {
                wrDic1[w] = new Dictionary<float, int>[rws];
                for (int r = 0; r < rws; r++)
			    {
                    wrDic1[w][r] = new Dictionary<float, int>();
                    Dictionary<float, int> fDic = wrDic1[w][r];
                    for (int c = 0; c < clms; c++)
                    {
                        object objVl = pbInBig.GetVal(p, c, r);
                        if (objVl != null)
                        {
                            float vl = System.Convert.ToSingle(objVl);
                            int vlCnt;
                            if(fDic.TryGetValue(vl,out vlCnt))
                            {
                                fDic[vl] = vlCnt + 1;
                            }
                            else
                            {
                                fDic.Add(vl,1);
                            }
                        }
                    }
			    }
                setValues(pbArr, wrDic1[w], w, 0);
            }
            //create next dictionary and set the rest of the values
            int clmsM = clms-1;
            int rwsM = rws-1;
            Dictionary<float, int>[][] wrDic2 = new Dictionary<float, int>[pbIn.Width][];
            //wrDic2[0][0] = wrDic1[0][rwsM];
            for (int r = 1; r < pbIn.Height; r++)
            {
                for (int c = 0; c < pbIn.Width; c++)
                {
                    Dictionary<float, int> clmsDic;
                    wrDic2[c] = new Dictionary<float, int>[rws];
                    for (int rb = 1; rb < rws; rb++)
                    {
                        wrDic2[c][rb-1] = new Dictionary<float, int>(wrDic1[c][rb]);
                    }
                    int nrb = r + rwsM;
                    if (c > 0)// copy previous dictionary on same row and remove/add values
                    {
                        wrDic2[c][rwsM] = new Dictionary<float, int>(wrDic2[c - 1][rwsM]);
                        clmsDic = wrDic2[c][rwsM];
                        int ncb = c + clmsM;
                        object objVln = pbInBig.GetVal(p, ncb, nrb);
                        object objVlo = pbInBig.GetVal(p, c-1, nrb);
                        if (objVlo != null)//remove old value
                        {
                            float vlo = System.Convert.ToSingle(objVlo);
                            int vloCnt = clmsDic[vlo];
                            if (vloCnt > 1)
                            {
                                clmsDic[vlo] = vloCnt - 1;
                            }
                            else
                            {
                                clmsDic.Remove(vlo);
                            }
                        }
                        if (objVln != null)//add new value
                        {
                            float vln = System.Convert.ToSingle(objVln);
                            int vlnCnt;
                            if (clmsDic.TryGetValue(vln, out vlnCnt))
                            {
                                clmsDic[vln] = vlnCnt + 1;
                            }
                            else
                            {
                                clmsDic.Add(vln, 1);
                            }

                        }
                    }
                    else //first column, need to get all numbers of new row
                    {
                        wrDic2[c][rwsM]= new Dictionary<float, int>();
                        Dictionary<float, int> fDic = wrDic2[c][rwsM];
                        for (int c2 = 0; c2 < clms; c2++)
                        {
                            object objVl = pbInBig.GetVal(p, c2, nrb);
                            if (objVl != null)
                            {
                                float vl = System.Convert.ToSingle(objVl);
                                int vlCnt;
                                if (fDic.TryGetValue(vl, out vlCnt))
                                {
                                    fDic[vl] = vlCnt + 1;
                                }
                                else
                                {
                                    fDic.Add(vl, 1);
                                }
                            }
                        }
                    }
                    setValues(pbArr, wrDic2[c], c, r);
                }   
            }
            pbIn.set_PixelData(p, pbArr);
            wrDic1 = wrDic2;

        }

        private void setValues(System.Array pbArr, Dictionary<float, int>[] wrDicClm, int c, int r)
        {
            Dictionary<float,int> clmsDic = new Dictionary<float, int>(wrDicClm[0]);
            for (int d = 1; d < rws; d++)
            {
                Dictionary<float, int> dic = wrDicClm[d];
                foreach (KeyValuePair<float, int> kvp in dic)
                {
                    float ky = kvp.Key;
                    int kyCnt = kvp.Value;
                    int tCnt;
                    if (clmsDic.TryGetValue(ky, out tCnt))
                    {
                        clmsDic[ky] = tCnt + kyCnt;
                    }
                    else
                    {
                        clmsDic.Add(ky, kyCnt);
                    }
                }
            }
            int maxCnt = 0;
            float maxVl = 0;
            foreach (KeyValuePair<float, int> kvp in clmsDic)
            {
                int kVl = kvp.Value;
                //Console.WriteLine("kVl = " + kVl.ToString());
                if (kVl > maxCnt)
                {
                    maxVl = kvp.Key;
                    maxCnt = kVl;
                }
            }
            pbArr.SetValue(maxVl, c, r);
        }
        public override void updatePixelRectangle(ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbIn,ESRI.ArcGIS.DataSourcesRaster.IPixelBlock3 pbInBig)
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
        //        Dictionary<float, int> dic = new Dictionary<float, int>();
        //        int maxCnt = -1;
        //        float maxVl = Single.MinValue;
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = Single.MinValue;
        //            }
        //            int cnt = 0;
        //            if(dic.TryGetValue(bVl,out cnt))
        //            {
        //                int nCnt = cnt += 1;
        //                dic[bVl] = nCnt;
        //                if (nCnt > maxCnt)
        //                {
        //                    maxCnt = nCnt;
        //                    maxVl = bVl;
        //                }
        //            }
        //            else
        //            {
        //                dic.Add(bVl, 1);
        //                if (1 > maxCnt)
        //                {
        //                    maxCnt = 1;
        //                    maxVl = bVl;
        //                }
        //            }
        //            vQueue.Enqueue(bVl);
        //        }
        //        valuesNewBigArr[0] = vQueue.ToList();
                
        //        windowQueue.Dequeue();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            foreach (float pVl in windowQueue.ElementAt(i)[0])
        //            {
        //                int cnt = 0;
        //                if (dic.TryGetValue(pVl, out cnt))
        //                {
        //                    int nCnt = cnt+=1;
        //                    dic[pVl] = nCnt;
        //                    if (nCnt > maxCnt)
        //                    {
        //                        maxCnt = nCnt;
        //                        maxVl = pVl;
        //                    }

        //                }
        //                else
        //                {
        //                    dic.Add(pVl, 1);
        //                }
        //            }
        //        }
        //        smallArr.SetValue(maxVl, 0, nr);
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
        //            float uVl = updateDictionary(ref dic, ref windowQueue, ref bVl, ref pVl, nc);
        //            smallArr.SetValue(uVl, nc, nr);
        //        }
        //        windowQueue.Enqueue(valuesNewBigArr);
        //    }
        //}

        //private float updateDictionary(ref Dictionary<float, int> dic, ref Queue<List<float>[]> windowQueue, ref float bVl, ref float pVl, int nc)
        //{
        //    int pClm = nc - 1;
        //    int maxClmIndex = clms-1;
        //    //remove the values from the dictionary
        //    int dicCnt = dic[pVl];
        //    if(dicCnt==1) dic.Remove(pVl);
        //    else dic[pVl] = dicCnt - 1;
        //    for(int i=0;i<windowQueue.Count;i++)
        //    {
        //        float pclmsVl = windowQueue.ElementAt(i)[pClm][0];
        //        dicCnt = dic[pclmsVl];
        //        if(dicCnt==1) dic.Remove(pclmsVl);
        //        else dic[pclmsVl] = dicCnt-1;
        //    }
        //    //add the values to the dictionary
        //    if(dic.TryGetValue(bVl,out dicCnt))
        //    {
        //        dic[bVl] = dicCnt+1;
        //    }
        //    else
        //    {
        //        dic.Add(bVl, 1);
        //    }
        //    for (int i = 0; i < windowQueue.Count; i++)
        //    {
        //        float nclmsVl = windowQueue.ElementAt(i)[nc][maxClmIndex];
        //        if (dic.TryGetValue(nclmsVl, out dicCnt))
        //        {
        //            dic[nclmsVl] = dicCnt + 1;
        //        }
        //        else
        //        {
        //            dic.Add(nclmsVl, 1);
        //        }
        //    }
        //    int maxCntVl = dic.Values.Max();
        //    float mode = 0;
        //    foreach (KeyValuePair<float, int> kvp in dic)
        //    {
        //        if (kvp.Value == maxCntVl)
        //        {
        //            mode = kvp.Key;
        //            break;
        //        }
        //    }
        //    return mode;
        //}

        //private void updateFirstRow(Queue<List<float>[]> windowQueue, System.Array smallArr)
        //{

        //    for (int k = 0; k <= smallArr.GetUpperBound(0); k++)
        //    {
        //        int maxCnt = -1;
        //        float maxVl = Single.MinValue;
        //        Dictionary<float, int> dic = new Dictionary<float, int>();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            foreach (float d in windowQueue.ElementAt(i)[k])
        //            {
        //                int cnt = 0;
        //                if (dic.TryGetValue(d, out cnt))
        //                {
        //                    int nCnt = cnt+=1;
        //                    dic[d] = nCnt;
        //                    if(nCnt>maxCnt)
        //                    {
        //                        maxCnt=nCnt;
        //                        maxVl = d;
        //                    }
        //                }
        //                else
        //                {
        //                    dic.Add(d, 1);
        //                    if(1>maxCnt)
        //                    {
        //                        maxCnt=1;
        //                        maxVl = d;
        //                    }
        //                }
        //            }
        //        }
        //        smallArr.SetValue(maxVl, k, 0);
        //    }
        //}
    }
}