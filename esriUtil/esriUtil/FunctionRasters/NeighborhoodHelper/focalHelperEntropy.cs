using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class focalHelperEntropy : focalFunctionDatasetBase
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
        //        for (int c = 0; c < clms; c++)
        //        {
        //            float bVl = System.Convert.ToSingle(bigArr.GetValue(c, r));
        //            if (rasterUtil.isNullData(bVl, noDataValue))
        //            {
        //                bVl = Single.MinValue;
        //            }
        //            int cnt = 0;
        //            if (dic.TryGetValue(bVl, out cnt))
        //            {
        //                int nCnt = cnt += 1;
        //                dic[bVl] = nCnt;
        //            }
        //            else
        //            {
        //                dic.Add(bVl, 1);
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
        //                    int nCnt = cnt += 1;
        //                    dic[pVl] = nCnt;
                           

        //                }
        //                else
        //                {
        //                    dic.Add(pVl, 1);
        //                }
        //            }
        //        }
        //        float prob = 0;
        //        float ent = 0;
        //        foreach (float dicCnt in dic.Values)
        //        {
        //            prob = dicCnt / windowN;
        //            ent += prob * System.Convert.ToSingle(Math.Log(prob));
        //        }
        //        ent = ent * -1;
        //        smallArr.SetValue(ent, 0, nr);
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
        //    int maxClmIndex = clms - 1;
        //    //remove the values from the dictionary
        //    int dicCnt = dic[pVl];
        //    if (dicCnt == 1) dic.Remove(pVl);
        //    else dic[pVl] = dicCnt - 1;
        //    for (int i = 0; i < windowQueue.Count; i++)
        //    {
        //        float pclmsVl = windowQueue.ElementAt(i)[pClm][0];
        //        dicCnt = dic[pclmsVl];
        //        if (dicCnt == 1) dic.Remove(pclmsVl);
        //        else dic[pclmsVl] = dicCnt - 1;
        //    }
        //    //add the values to the dictionary
        //    if (dic.TryGetValue(bVl, out dicCnt))
        //    {
        //        dic[bVl] = dicCnt + 1;
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

        //    float prob = 0;
        //    float ent = 0;
        //    foreach (float c in dic.Values)
        //    {
        //        prob = c / windowN;
        //        ent += prob * System.Convert.ToSingle(Math.Log(prob));
        //    }
        //    ent = ent * -1;
        //    return ent;
        //}

        //private void updateFirstRow(Queue<List<float>[]> windowQueue, System.Array smallArr)
        //{

        //    for (int k = 0; k <= smallArr.GetUpperBound(0); k++)
        //    {
        //        Dictionary<float, int> dic = new Dictionary<float, int>();
        //        for (int i = 0; i < windowQueue.Count; i++)
        //        {
        //            foreach (float d in windowQueue.ElementAt(i)[k])
        //            {
        //                int cnt = 0;
        //                if (dic.TryGetValue(d, out cnt))
        //                {
        //                    int nCnt = cnt += 1;
        //                    dic[d] = nCnt;
                            
        //                }
        //                else
        //                {
        //                    dic.Add(d, 1);
                   
        //                }
        //            }
        //        }
        //        float prob = 0;
        //        float ent = 0;
        //        foreach (float dicCnt in dic.Values)
        //        {
        //            prob = dicCnt / windowN;
        //            ent += System.Convert.ToSingle(prob*Math.Log(prob));
        //        }
        //        ent = ent * -1;
        //        smallArr.SetValue(ent, k,0);
        //    }
        //}
    }
}