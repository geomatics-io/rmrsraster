using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesNetCDF;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using System.Data.SqlClient;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace esriUtil
{
    public class featureUtil
    {
        public featureUtil()
        {
            rsUtil = new rasterUtil();
        }
        public featureUtil(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        public void weightFieldValuesByAreaLength(IFeatureClass strataFtr, string[] fldNames, IFeatureClass standsFtr, bool length = false)
        {
            int[] meanFldIndex = new int[fldNames.Length];
            int[] fldNamesIndex = new int[fldNames.Length];
            for (int i = 0; i < fldNames.Length; i++)
            {
                string mName = geoUtil.createField(standsFtr, "m_" + fldNames[i], ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeDouble, false);
                meanFldIndex[i] = standsFtr.FindField(mName);
                fldNamesIndex[i] = strataFtr.FindField(fldNames[i]);
            }
            IFeatureCursor uCur = standsFtr.Update(null, true);
            IFeature uFtr = uCur.NextFeature();
            while (uFtr != null)
            {
                ESRI.ArcGIS.Geometry.IGeometry geo = uFtr.Shape;
                ISpatialFilter spFlt = new SpatialFilter();
                spFlt.Geometry = geo;
                spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                double totalVl = 0;
                IFeatureCursor sCur = strataFtr.Search(spFlt, true);
                IFeature sFtr = sCur.NextFeature();
                double[] vlArr = new double[meanFldIndex.Length];
                while (sFtr != null)
                {
                    ESRI.ArcGIS.Geometry.IGeometry sgeo = sFtr.Shape;
                    ESRI.ArcGIS.Geometry.ITopologicalOperator4 topo = (ESRI.ArcGIS.Geometry.ITopologicalOperator4)sgeo;
                    ESRI.ArcGIS.Geometry.IGeometry sgeo2 = topo.Intersect(geo, ESRI.ArcGIS.Geometry.esriGeometryDimension.esriGeometry2Dimension);
                    double subArea = 0;
                    if (length)
                    {
                        subArea = (((ESRI.ArcGIS.Geometry.IPolygon)sgeo2).Length);
                        totalVl += subArea;
                    }
                    else
                    {
                        subArea = ((ESRI.ArcGIS.Geometry.IArea)sgeo2).Area;
                        totalVl += subArea;
                    }

                    for (int i = 0; i < meanFldIndex.Length; i++)
                    {
                        vlArr[i] += System.Convert.ToDouble(sFtr.get_Value(fldNamesIndex[i])) * subArea;
                    }
                    sFtr = sCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
                if (totalVl != 0)
                {
                    for (int i = 0; i < meanFldIndex.Length; i++)
                    {
                        uFtr.set_Value(meanFldIndex[i], vlArr[i] / totalVl);
                    }
                    uCur.UpdateFeature(uFtr);
                }
                uFtr = uCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);

        }
        public void summarizeAcrossFields(ITable ftrCls, string[] fieldNames, esriUtil.rasterUtil.localType[] stats, string qWhere="", string prefix="")
        {
            int[] fldIndex = new int[fieldNames.Length];
            int[] updateFldsIndex =  new int[stats.Length];
            string[] updateNames = new string[stats.Length];
            for (int i = 0; i < fieldNames.Length; i++)
			{
                fldIndex[i]=ftrCls.FindField(fieldNames[i]);
			}
            for (int i = 0; i < stats.Length; i++)
			{
                string nm = stats[i].ToString();
                if(prefix!="" && prefix!=null) nm = prefix+"_"+nm;
                updateNames[i] = geoUtil.createField(ftrCls,nm,esriFieldType.esriFieldTypeDouble,false);
                updateFldsIndex[i] = ftrCls.FindField(updateNames[i]);
			}
            bool catStat = false;
            if(stats.Contains(rasterUtil.localType.ASM)||stats.Contains(rasterUtil.localType.ENTROPY)||stats.Contains(rasterUtil.localType.UNIQUE)||stats.Contains(rasterUtil.localType.MEDIAN)||stats.Contains(rasterUtil.localType.MODE)) catStat = true;
            //Console.WriteLine("Updating Categories = " + catStat.ToString());
            IQueryFilter qf = new QueryFilterClass();
            if (!String.IsNullOrEmpty(qWhere)) qf.WhereClause = qWhere;
            //qf.SubFields = String.Join(",", fieldNames)+","+String.Join(",",updateNames);
            ICursor uCur = ftrCls.Update(qf, true);
            IRow ftr = uCur.NextRow();
            while (ftr != null)
            {
                double[] vlArr = new double[10];//cnt, min, max, sum, sum2, minfield, maxfield, subtract, multiply, divide
                Dictionary<string, int> dic = new Dictionary<string,int>();
                vlArr[1]=double.MaxValue;
                vlArr[2]=double.MinValue;
                for (int i = 0; i < fldIndex.Length; i++)
                {
                    object vlObj = ftr.get_Value(fldIndex[i]);
                    if(vlObj!=null)
                    {
                        double vl = System.Convert.ToDouble(vlObj);
                        vlArr[0] = vlArr[0] + 1;
                        if (vl < vlArr[1])
                        {
                            vlArr[1] = vl;
                            vlArr[5]=i;
                        }
                        if (vl > vlArr[2])
                        {
                            vlArr[2] = vl;
                            vlArr[6] = i;
                        }
                        vlArr[3] = vlArr[3] + vl;
                        vlArr[4] = vlArr[4] + (vl * vl);
                        vlArr[7] = vlArr[7] - vl;
                        vlArr[8] = vlArr[8] * vl;
                        vlArr[9] = vlArr[9] / vl;
                        if(catStat)
                        {
                            int cntVl;
                            string vlStr = vl.ToString();
                            if(dic.TryGetValue(vlStr,out cntVl))
                            {
                                dic[vlStr] = cntVl+1;
                            }
                            else
                            {
                                dic.Add(vlStr,1);
                            }
                        }
                    }
                }
                for (int i = 0; i < stats.Length; i++)
			    {
                    rasterUtil.localType st = stats[i];
                    double sVl = 0;
                    switch (st)
	                {
		                case rasterUtil.localType.MAX:
                            sVl = vlArr[2];
                         break;
                        case rasterUtil.localType.MIN:
                            sVl = vlArr[1];
                         break;
                        case rasterUtil.localType.MAXBAND:
                            sVl = vlArr[6];
                         break;
                        case rasterUtil.localType.MINBAND:
                            sVl = vlArr[5];
                         break;
                        case rasterUtil.localType.SUM:
                            sVl = vlArr[3];
                         break;
                        case rasterUtil.localType.MULTIPLY:
                            sVl = vlArr[8];
                         break;
                        case rasterUtil.localType.DIVIDE:
                            sVl = vlArr[9];
                         break;
                        case rasterUtil.localType.SUBTRACT:
                            sVl = vlArr[7];
                         break;
                        case rasterUtil.localType.MEAN:
                            sVl = vlArr[3]/vlArr[0];
                         break;
                        case rasterUtil.localType.VARIANCE:
                            sVl = (vlArr[4] - (Math.Pow(vlArr[3], 2) / vlArr[0])) / (vlArr[0] - 1);
                         break;
                        case rasterUtil.localType.STD:
                            sVl = Math.Sqrt((vlArr[4] - (Math.Pow(vlArr[3], 2) / vlArr[0])) / (vlArr[0] - 1));
                         break;
                        case rasterUtil.localType.MODE:
                            sVl = getMode(dic);
                         break;
                        case rasterUtil.localType.MEDIAN:
                            sVl = getMedian(dic);
                         break;
                        case rasterUtil.localType.UNIQUE:
                            sVl = dic.Keys.Count;
                         break;
                        case rasterUtil.localType.ENTROPY:
                            sVl = getEntropy(dic);
                         break;
                        case rasterUtil.localType.ASM:
                            sVl = getASM(dic);
                         break;
                        default:
                         break;
	                }
                    //Console.WriteLine("Setting value " + updateNames[i] + " to " + sVl.ToString());
                    ftr.set_Value(updateFldsIndex[i],sVl);

			    }
                uCur.UpdateRow(ftr);
                ftr = uCur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);

        }
        public void summarizeRelatedTable(ITable pTable, ITable rTable, string plinkField, string rlinkField, string[] summaryFlds, string[] groupFlds, rasterUtil.focalType[] stats,string pWhere="", string rWhere="")
        {
            bool needCatDic = false;
            if(stats.Contains(rasterUtil.focalType.ASM)||stats.Contains(rasterUtil.focalType.ENTROPY)||stats.Contains(rasterUtil.focalType.MEDIAN)||stats.Contains(rasterUtil.focalType.MODE)||stats.Contains(rasterUtil.focalType.UNIQUE))needCatDic=true;
            HashSet<string> uGroups;
            //Console.WriteLine("Summarizing values");
            Dictionary<string,Dictionary<string, object[][]>> sumDic = getRelatedSummary(pTable, rTable, plinkField, rlinkField, summaryFlds, groupFlds, needCatDic, pWhere, rWhere, out uGroups);//<link,<group,[6][number of fields to summarize]>
            //foreach (string s in sumDic.Keys)
            //{
            //    Console.WriteLine("ID key:" + s);
            //}
            //foreach (string s in uGroups)
            //{
            //    Console.WriteLine("Group:" + s);
            //}
            //Console.WriteLine("Updating parent");
            int[] newFldNameIndex = new int[summaryFlds.Length * uGroups.Count * (stats.Length+1)];
            string[] newFldNameString = new string[summaryFlds.Length * uGroups.Count * (stats.Length+1)];
            int IndexCnt = 0;
            //int nIndex = pTable.FindField(geoUtil.createField(pTable,"n",esriFieldType.esriFieldTypeDouble,false));
            int linkIndex = pTable.FindField(plinkField);
            for (int i = 0; i < summaryFlds.Length; i++)
            {
                string fldNm = summaryFlds[i];
                foreach(string k in uGroups)
                {
                    string fldNm2 = fldNm + "_" + k;
                    string newFldName = geoUtil.createField(pTable, fldNm2 + "_N", esriFieldType.esriFieldTypeInteger, false);
                    newFldNameString[IndexCnt] = newFldName;
                    newFldNameIndex[IndexCnt] = pTable.FindField(newFldName);
                    IndexCnt += 1;
                    for (int j = 0; j < stats.Length; j++)
                    {
                        string fldNm3 = fldNm2 + "_" + stats[j];
                        newFldName = geoUtil.createField(pTable, fldNm3, esriFieldType.esriFieldTypeDouble, false);
                        newFldNameString[IndexCnt]=newFldName;
                        newFldNameIndex[IndexCnt] = pTable.FindField(newFldName);
                        IndexCnt += 1;
                    }
                } 
            }
            IQueryFilter pQf = new QueryFilterClass();
            if (!String.IsNullOrEmpty(pWhere)) pQf.WhereClause = pWhere;
            //Console.WriteLine(pWhere);
            ICursor cur = pTable.Update(pQf, true);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                string lnk = rw.get_Value(linkIndex).ToString();
                Dictionary<string,object[][]> d;
                if (sumDic.TryGetValue(lnk, out d))
                {
                    //Console.WriteLine("Found dictionary value " + lnk);
                    object n = 0;
                    for (int i = 0; i < newFldNameString.Length; i++)
                    {
                        string fldNameC = newFldNameString[i];
                        //Console.WriteLine(fldNameC);
                        string[] fldnameArr = fldNameC.Split(new char[] { '_' });
                        string fld = fldnameArr[0];
                        string grp = fldnameArr[1];
                        for (int k = 1; k < groupFlds.Length; k++)
                        {
                            grp = grp + "_" + fldnameArr[1 + k];
                        }
                        string st = fldnameArr[fldnameArr.Length-1];
                        object[][] vlFldArr;
                        if (d.TryGetValue(grp, out vlFldArr))
                        {
                            //Console.WriteLine("Found Group value " + grp);
                            object uVl = 0;
                            int clIndex = System.Array.IndexOf(summaryFlds, fld);
                            object[] vlArr = vlFldArr[clIndex];
                            n = vlArr[0];
                            if (st != "N")
                            {
                                rasterUtil.focalType sType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), st);
                                //Console.WriteLine(sType.ToString());
                                switch (sType)
                                {
                                    case rasterUtil.focalType.SUM:
                                        uVl = vlArr[1];
                                        break;
                                    case rasterUtil.focalType.MIN:
                                        uVl = vlArr[3];
                                        break;
                                    case rasterUtil.focalType.MAX:
                                        uVl = vlArr[4];
                                        break;
                                    case rasterUtil.focalType.MEAN:
                                        uVl = System.Convert.ToDouble(vlArr[1]) / System.Convert.ToDouble(n);
                                        break;
                                    case rasterUtil.focalType.STD:
                                        uVl = Math.Sqrt((System.Convert.ToDouble(vlArr[2]) - (System.Convert.ToDouble(vlArr[1]) / System.Convert.ToDouble(n))) / (System.Convert.ToDouble(n) - 1));
                                        break;
                                    case rasterUtil.focalType.MODE:
                                        uVl = getMode((Dictionary<string, int>)vlArr[5]);
                                        break;
                                    case rasterUtil.focalType.MEDIAN:
                                        uVl = getMedian((Dictionary<string, int>)vlArr[5]);
                                        break;
                                    case rasterUtil.focalType.VARIANCE:
                                        uVl = (System.Convert.ToDouble(vlArr[2]) - (System.Convert.ToDouble(vlArr[1]) / System.Convert.ToDouble(n))) / (System.Convert.ToDouble(n) - 1);
                                        break;
                                    case rasterUtil.focalType.UNIQUE:
                                        uVl = ((Dictionary<string, int>)vlArr[5]).Keys.Count;
                                        break;
                                    case rasterUtil.focalType.ENTROPY:
                                        uVl = getEntropy((Dictionary<string, int>)vlArr[5]);
                                        break;
                                    case rasterUtil.focalType.ASM:
                                        uVl = getASM((Dictionary<string, int>)vlArr[5]);
                                        break;
                                    default:
                                        break;
                                }
                                
                            }
                            else
                            {
                                uVl = n;
                            }
                            rw.set_Value(newFldNameIndex[i], uVl);
                        }
                        else
                        {
                            //Console.WriteLine("Could not find Group " + grp);
                            rw.set_Value(newFldNameIndex[i], 0);
                        }                      
                    }
                }
                else
                {
                    for (int i = 0; i < newFldNameString.Length; i++)
                    {
                        rw.set_Value(newFldNameIndex[i], 0);
                    }
                }
                cur.UpdateRow(rw);
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);  
        }

        private Dictionary<string, Dictionary<string, object[][]>> getRelatedSummary(ITable pTable, ITable rTable, string plinkField, string rlinkField, string[] summaryFlds, string[] groupFlds, bool needCatDic, string pWhere, string rWhere, out HashSet<string> uGroups)
        {
            HashSet<string> uLink = new HashSet<string>();
            uGroups = new HashSet<string>();
            Dictionary<string, Dictionary<string, object[][]>> outDic = new Dictionary<string, Dictionary<string, object[][]>>();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = plinkField;
            if (!String.IsNullOrEmpty(pWhere)) qf.WhereClause = pWhere;
            ICursor cur = pTable.Search(qf, true);
            int linkIndex = cur.FindField(plinkField);
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                object vlObj = rw.get_Value(linkIndex);
                if(vlObj!=null)
                {
                    string vl = vlObj.ToString();
                    if (uLink.Add(vl))
                    {
                        outDic.Add(vl, new Dictionary<string, object[][]>());
                    }
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            qf.SubFields = rlinkField + "," + String.Join(",", summaryFlds);
            if (groupFlds != null && groupFlds.Length > 0) qf.SubFields = qf.SubFields + "," + String.Join(",", groupFlds);
            if (!String.IsNullOrEmpty(rWhere)) qf.WhereClause = rWhere;
            else qf.WhereClause = "";
            //Console.WriteLine(qf.SubFields);
            ICursor curR = rTable.Search(qf, true);
            linkIndex = curR.FindField(rlinkField);
            int[] grpIndex = null;
            if (groupFlds != null && groupFlds.Length > 0)
            {
                grpIndex = new int[groupFlds.Length];
                for (int i = 0; i < grpIndex.Length; i++)
                {
                    grpIndex[i] = curR.FindField(groupFlds[i]);
                }
            }
            int[] sumIndex = new int[summaryFlds.Length];
            for (int i = 0; i < summaryFlds.Length; i++)
            {
                sumIndex[i] = curR.FindField(summaryFlds[i]);
            }
            IRow rwR = curR.NextRow();
            while (rwR != null)
            {
                object rlObj = rwR.get_Value(linkIndex);
                if (rlObj != null)
                {
                    string rl = rlObj.ToString();
                    Dictionary<string, object[][]> dic;
                    if(outDic.TryGetValue(rl,out dic))
                    {
                        string grpVl = "All";
                        if (grpIndex != null)
                        {
                            string[] grpVlArr = new string[grpIndex.Length];
                            for (int i = 0; i < grpIndex.Length; i++)
                            {
                                object vlObj = rwR.get_Value(grpIndex[i]);
                                if (vlObj != null)
                                {
                                    grpVlArr[i] = vlObj.ToString();
                                }
                                else
                                {
                                    grpVlArr[i] = "_";
                                }
                            }

                            grpVl = String.Join("_", grpVlArr);
                        }
                        uGroups.Add(grpVl);
                        object[][] statObj;
                        if (dic.TryGetValue(grpVl, out statObj))
                        {
                           
                        }
                        else
                        {
                            statObj = new object[summaryFlds.Length][];
                            for (int i = 0; i < summaryFlds.Length; i++)
                            {
                                statObj[i] = new object[6];
                                statObj[i][0] = 0;
                                statObj[i][1] = 0;
                                statObj[i][2] = 0;
                                statObj[i][3] = double.MaxValue;
                                statObj[i][4] = double.MinValue;
                                statObj[i][5] = new Dictionary<string,int>();
                            }
                        }
                        for (int i = 0; i < summaryFlds.Length; i++)
                        {
                            object fVlObj = rwR.get_Value(sumIndex[i]);
                            if (fVlObj != null && !System.Convert.IsDBNull(fVlObj))
                            {
                                double fvl = System.Convert.ToDouble(fVlObj);
                                statObj[i][0] = System.Convert.ToInt32(statObj[i][0]) + 1;
                                statObj[i][1] = System.Convert.ToDouble(statObj[i][1]) + fvl;
                                statObj[i][2] = System.Convert.ToDouble(statObj[i][2]) + fvl*fvl;
                                double min = System.Convert.ToDouble(statObj[i][3]);
                                if (fvl < min) statObj[i][3] = fvl;
                                double max = System.Convert.ToDouble(statObj[i][4]);
                                if (fvl < max) statObj[i][4] = fvl;
                                if (needCatDic)
                                {
                                    int dCnt = 0;
                                    string vlStr = fVlObj.ToString();
                                    Dictionary<string,int> vlDic = (Dictionary<string,int>)statObj[i][5];
                                    if (vlDic.TryGetValue(vlStr, out dCnt))
                                    {
                                        vlDic[vlStr] = dCnt + 1;
                                    }
                                    else
                                    {
                                        vlDic.Add(vlStr, 1);
                                    }
                                }
                            }

                        }
                        dic[grpVl] = statObj;

                    }
                }
                rwR = curR.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(curR);
            return outDic;
        }
        private double getASM(Dictionary<string, int> dic)
        {
            double outVl = 0;
            double cnt = dic.Values.Sum();
            double pSum = 0;
            foreach (KeyValuePair<string, int> kvp in dic)
            {
                double p = kvp.Value / cnt;
                pSum += (p * p);
            }
            outVl = pSum;
            return outVl;
        }

        private double getEntropy(Dictionary<string, int> dic)
        {
            double outVl = 0;
            double cnt = dic.Values.Sum();
            double pSum = 0;
            foreach (KeyValuePair<string,int> kvp in dic)
            {
                double p = kvp.Value / cnt;
                pSum += (p * Math.Log(p));
            }
            outVl = -1 * pSum;
            return outVl;
        }

        private double getMedian(Dictionary<string, int> dic)
        {

            double outVl = 0;
            int mCnt = dic.Values.Sum()/2;
            string[] sKey = dic.Keys.ToArray();
            System.Array.Sort(sKey);
            int cnt = 0;
            foreach (string k in sKey)
            {
                cnt += dic[k];
                if (cnt > mCnt)
                {
                    outVl = System.Convert.ToDouble(k);
                    break;
                }
            }
            return outVl;
        }

        private double getMode(Dictionary<string, int> dic)
        {
            double outVl = 0;
            int mCnt = 0;
            foreach (KeyValuePair<string, int> kvp in dic)
            {
                int cnt = kvp.Value;
                if (cnt > mCnt)
                {
                    mCnt = cnt;
                    outVl = System.Convert.ToDouble(kvp.Key);
                }
            }
            return outVl;
        }
        /// <summary>
        /// creates a new field called sample and populates yes or no depending on whether that feature should be sampled based on a previously ran cluster analysis
        /// </summary>
        /// <param name="inputTable"></param>
        /// <param name="clusterModelPath"></param>
        /// <param name="proportionOfMean"></param>
        /// <param name="alpha"></param>
        public void selectClusterFeaturesToSample(ITable inputTable, string clusterModelPath, string clusterFieldName="Cluster", double proportionOfMean=0.1, double alpha=0.05, bool weightsEqual=false)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            esriUtil.Statistics.dataPrepClusterKmean dpC = new Statistics.dataPrepClusterKmean();
            dpC.buildModel(clusterModelPath);
            List<string> labels = dpC.Labels;
            HashSet<string> unqVls = geoUtil.getUniqueValues(inputTable, clusterFieldName);
            System.Random rd = new Random();
            int[] samplesPerCluster = esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxCluster(clusterModelPath, proportionOfMean, alpha);
            double[] propPerCluster = esriUtil.Statistics.dataPrepSampleSize.clusterProportions(clusterModelPath);
            double[] weightsPerCluster = new double[propPerCluster.Length];
            double sSamp = System.Convert.ToDouble(samplesPerCluster.Sum());
            for (int i = 0; i < weightsPerCluster.Length; i++)
            {
                weightsPerCluster[i] = propPerCluster[i] / (samplesPerCluster[i] / sSamp);
            }
            if (weightsEqual)
            {
                double minProp = weightsPerCluster.Min();
                for (int i = 0; i < samplesPerCluster.Length; i++)
                {
                    samplesPerCluster[i] = System.Convert.ToInt32(samplesPerCluster[i] * (weightsPerCluster[i] / minProp));
                    weightsPerCluster[i] = 1;
                }
            }
            int[] tsPerCluster = new int[propPerCluster.Length];
            double[] randomRatioPerClust = new double[propPerCluster.Length];
            if (samplesPerCluster.Length != unqVls.Count)
            {
                System.Windows.Forms.MessageBox.Show("Unique Values in cluster field do not match the number of cluster models!");
                return;
            }
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger,false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble,false);
            IQueryFilter qf0 = new QueryFilterClass();
            qf0.SubFields = clusterFieldName;
            string h = "";
            IField fld = inputTable.Fields.get_Field(inputTable.FindField(clusterFieldName));
            if (fld.Type == esriFieldType.esriFieldTypeString) h = "'";
            for (int i = 0; i < samplesPerCluster.Length; i++)
            {

                qf0.WhereClause = clusterFieldName + " = " + h+labels[i]+h;
                int tCnt = inputTable.RowCount(qf0);
                tsPerCluster[i] = tCnt;
                randomRatioPerClust[i] = System.Convert.ToDouble(samplesPerCluster[i]) / tCnt;
            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = clusterFieldName + "," + sampleFldName + "," + weightFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(clusterFieldName);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = rw.get_Value(cIndex).ToString();
                    int clust = labels.IndexOf(clustStr);
                    double w = weightsPerCluster[clust];
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = randomRatioPerClust[clust];
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, w);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            
        }

        public void selectStrataFeaturesToSample(ITable inputTable, string strataModelPath, string strataFieldName = "Cluster", double proportionOfMean = 0.1, double alpha = 0.05, bool weightsEqual = false)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            esriUtil.Statistics.dataPrepStrata dpC = new Statistics.dataPrepStrata();
            dpC.buildModel(strataModelPath);
            List<string> labels = dpC.Labels;
            HashSet<string> unqVls = geoUtil.getUniqueValues(inputTable, strataFieldName);
            System.Random rd = new Random();
            int[] samplesPerCluster = esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxCluster(strataModelPath, proportionOfMean, alpha);
            double[] propPerCluster = esriUtil.Statistics.dataPrepSampleSize.clusterProportions(strataModelPath);
            double[] weightsPerCluster = new double[propPerCluster.Length];
            double sSamp = System.Convert.ToDouble(samplesPerCluster.Sum());
            for (int i = 0; i < weightsPerCluster.Length; i++)
            {
                weightsPerCluster[i] = propPerCluster[i] / (samplesPerCluster[i] / sSamp);
            }
            if (weightsEqual)
            {
                double minProp = weightsPerCluster.Min();
                for (int i = 0; i < samplesPerCluster.Length; i++)
                {
                    samplesPerCluster[i] = System.Convert.ToInt32(samplesPerCluster[i] * (weightsPerCluster[i] / minProp));
                    weightsPerCluster[i] = 1;
                }
            }
            int[] tsPerCluster = new int[propPerCluster.Length];
            double[] randomRatioPerClust = new double[propPerCluster.Length];
            if (samplesPerCluster.Length != unqVls.Count)
            {
                System.Windows.Forms.MessageBox.Show("Unique Values in cluster field do not match the number of cluster models!");
                return;
            }
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble, false);
            IQueryFilter qf0 = new QueryFilterClass();
            qf0.SubFields = strataFieldName;
            string h = "";
            IField fld = inputTable.Fields.get_Field(inputTable.FindField(strataFieldName));
            if (fld.Type == esriFieldType.esriFieldTypeString) h = "'";
            for (int i = 0; i < samplesPerCluster.Length; i++)
            {

                qf0.WhereClause = strataFieldName + " = " + h + labels[i] + h;
                int tCnt = inputTable.RowCount(qf0);
                tsPerCluster[i] = tCnt;
                randomRatioPerClust[i] = System.Convert.ToDouble(samplesPerCluster[i]) / tCnt;
            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = strataFieldName + "," + sampleFldName + "," + weightFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(strataFieldName);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = rw.get_Value(cIndex).ToString();
                    int clust = labels.IndexOf(clustStr);
                    double w = weightsPerCluster[clust];
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = randomRatioPerClust[clust];
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, w);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }

        }
        public void selectAccuracyFeaturesToSample(ITable inputTable, string AccuracyAssessmentModelPath, string mapField, double proportionOfMean, double alpha, bool weightsEqual=false)
        {
            esriUtil.Statistics.dataGeneralConfusionMatirx dGc = new Statistics.dataGeneralConfusionMatirx();
            dGc.getXTable(AccuracyAssessmentModelPath);
            List<string> labels = dGc.Labels.ToList();
            int samplesPerClass = esriUtil.Statistics.dataPrepSampleSize.sampleSizeKappa(AccuracyAssessmentModelPath, proportionOfMean, alpha)/labels.Count + 1;
            selectEqualFeaturesToSample(inputTable, mapField, samplesPerClass, weightsEqual);
        }
        public void selectRandomFeaturesToSample(ITable inputTable, int totalSamples)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            System.Random rd = new Random();
            double tRec = inputTable.RowCount(null);
            double gR = totalSamples / System.Convert.ToDouble(tRec);
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble, false);
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(null, false);
                int sIndex = cur.FindField(sampleFldName);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    if (rNum <= gR)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, 1);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }
        public void selectEqualFeaturesToSample(ITable inputTable, string mapField, int SamplesPerClass, bool weightsEqual=false )
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            HashSet<string> unqVls = geoUtil.getUniqueValues(inputTable, mapField);
            System.Random rd = new Random();
            int samplesPerClass = SamplesPerClass;
            double tSamples = System.Convert.ToDouble(samplesPerClass * unqVls.Count);
            double gR = samplesPerClass / tSamples;
            double[] weightsPerClass = new double[unqVls.Count];
            int[] tsPerClass = new int[unqVls.Count];
            double[] ratioPerClass = new double[unqVls.Count];
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            string weightFldName = geoUtil.createField(inputTable, "weight", esriFieldType.esriFieldTypeDouble, false);
            IQueryFilter qf0 = new QueryFilterClass();
            qf0.SubFields = mapField;
            string h = "";
            IField fld = inputTable.Fields.get_Field(inputTable.FindField(mapField));
            if (fld.Type == esriFieldType.esriFieldTypeString) h = "'";
            for (int i = 0; i < unqVls.Count; i++)
            {

                qf0.WhereClause = mapField + " = " + h + unqVls.ElementAt(i) + h;
                int tCnt = inputTable.RowCount(qf0);
                tsPerClass[i] = tCnt;
                ratioPerClass[i] = System.Convert.ToDouble(samplesPerClass) / tCnt;
            }
            double tsSamp = System.Convert.ToDouble(tsPerClass.Sum());
            for (int i = 0; i < weightsPerClass.Length; i++)
            {
                weightsPerClass[i] = (tsPerClass[i]/tsSamp) / (gR);
            }
            if (weightsEqual)
            {
                double minW = weightsPerClass.Min();
                for (int i = 0; i < weightsPerClass.Length; i++)
                {
                    double aSamp = samplesPerClass*(weightsPerClass[i]/minW);
                    ratioPerClass[i] = aSamp / tsPerClass[i];
                    weightsPerClass[i] = 1;
                }

            }
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = mapField + "," + sampleFldName + "," + weightFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(mapField);
                int wIndex = cur.FindField(weightFldName);
                IRow rw = cur.NextRow();
                List<string> unqLst = unqVls.ToList();
                while (rw != null)
                {
                    string classStr = rw.get_Value(cIndex).ToString();
                    int cls = unqLst.IndexOf(classStr);
                    double w = weightsPerClass[cls];
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = ratioPerClass[cls];
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    rw.set_Value(wIndex, w);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public void selectKSFeaturesToSample(ITable sampledTable,ITable samplesToDrawFromTable, string ksModelPath, string groupFieldName = "")
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)sampledTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Sampled Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            if (samplesToDrawFromTable != null)
            {
                objInfo2 = (IObjectClassInfo2)samplesToDrawFromTable;
                if (!objInfo2.CanBypassEditSession())
                {
                    System.Windows.Forms.MessageBox.Show("Samples to draw from table participates in a composite relationship. Please export this table as a new table and try again!");
                    return;
                }
            }
            if (groupFieldName == null) groupFieldName = "";
            try
            {
                esriUtil.Statistics.dataPrepCompareSamples dpComp = new Statistics.dataPrepCompareSamples(ksModelPath);
                Dictionary<string,object[]> sampledBinPropDic = calcBinValues(dpComp, sampledTable); //key = stratfield_bin, values = [0] ratios {10} for random selection [1] counts {10} from sample
                //bins and ratios calculated next use ratios to select from class and bin

                IWorkspace wks = ((IDataset)sampledTable).Workspace;
                IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
                if (wksE.IsBeingEdited())
                {
                    wksE.StopEditing(true);
                }
           
                System.Random rd = new Random();
                string sampleFldName = geoUtil.createField(sampledTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
                List<string> labels = dpComp.Labels.ToList();
            
                ICursor cur = sampledTable.Update(null, false);
                int sIndex = cur.FindField(sampleFldName);
                int cIndex = cur.FindField(groupFieldName);
                int bIndex = cur.FindField("BIN");
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    string clustStr = labels[0];
                    if (cIndex > -1)
                    {
                        clustStr = rw.get_Value(cIndex).ToString();
                    }
                    int b = System.Convert.ToInt32(rw.get_Value(bIndex));
                    double rNum = rd.NextDouble();
                    double r = ((double[])sampledBinPropDic[clustStr][0])[b];

                    int ss = 0;
                    if (rNum <= r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
                if (samplesToDrawFromTable != null)
                {
                    appendSamples(sampledTable, samplesToDrawFromTable, sampledBinPropDic,dpComp);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        private void appendSamples(ITable sampledTable, ITable samplesToDrawFromTable, Dictionary<string, object[]> sampledBinPropDic,Statistics.dataPrepCompareSamples dpComp)
        {
            Dictionary<string,object[]> bigSampleDic = calcBinValues(dpComp,samplesToDrawFromTable);
            Dictionary<string, double[]> ratioDic = new Dictionary<string, double[]>();
            bool check = false;
            foreach (KeyValuePair<string, object[]> kvp in sampledBinPropDic)
            {
                string ky = kvp.Key;
                
                int[] cntArr = (int[])kvp.Value[1];
                int totalCnt = cntArr.Sum();
                //double[] ratioArr = (double[])kvp.Value[0];
                int[] cntArr2 = (int[])bigSampleDic[ky][1];
                double[] ratioArr2 = dpComp.ClusterProportions[ky];//.binPropDic1[ky][0];// (double[])bigSampleDic[ky][0];
                double[] nrArr = new double[ratioArr2.Length];
                for (int i = 0; i < cntArr.Length; i++)
                {
                    double nr = 0;
                    double r = ratioArr2[i];
                    int tCntS = cntArr[i];
                    int sN = (int)((totalCnt*r) - tCntS);
                    if (sN > 0)
                    {
                        check = true;
                        nr = System.Convert.ToDouble(sN) / cntArr2[i];
                    }
                    nrArr[i] = nr;
                }
                ratioDic.Add(ky, nrArr);
            }
            if (!check) return;
            Random rd = new Random();
            string[] labels = dpComp.Labels;
            List<int> bFldsIndex = new List<int>();
            List<int> sFldsIndex = new List<int>();
            for (int i = 0; i < sampledTable.Fields.FieldCount; i++)
            {
                IField sfld = sampledTable.Fields.get_Field(i);
                string sfldName = sfld.Name;
                int bfldIndex = samplesToDrawFromTable.FindField(sfldName);
                if (bfldIndex > -1 && sfld.Editable)
                {
                    bFldsIndex.Add(bfldIndex);
                    sFldsIndex.Add(i);
                }
            }
            ICursor cur = samplesToDrawFromTable.Search(null, false);
            int cIndex = cur.FindField(dpComp.StrataField);
            int bIndex = cur.FindField("BIN");
            IRow rw = cur.NextRow();
            int sIndex = sampledTable.FindField("Sample");
            int wIndex = sampledTable.FindField("Weight");
            while (rw != null)
            {
                string clustStr = labels[0];
                if (cIndex > -1)
                {
                    clustStr = rw.get_Value(cIndex).ToString();
                }
                int b = System.Convert.ToInt32(rw.get_Value(bIndex));
                double rNum = rd.NextDouble();
                double r = ratioDic[clustStr][b];
                if (rNum <= r)
                {
                    IRow srw = sampledTable.CreateRow();
                    for (int i = 0; i < sFldsIndex.Count; i++)
                    {
                        try
                        {
                            srw.set_Value(sFldsIndex[i], rw.get_Value(bFldsIndex[i]));
                        }
                        catch
                        {
                            
                        }
                    }
                    if (sIndex > -1) srw.set_Value(sIndex, 1);
                    if (wIndex > -1) srw.set_Value(wIndex, sampledBinPropDic[clustStr][2]);
                    srw.Store();
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
        }

        private Dictionary<string, object[]> calcBinValues(Statistics.dataPrepCompareSamples dpComp, ITable inTable)
        {
            Statistics.dataPrepClusterKmean clus = dpComp.Cluster;

            int nBins = clus.Classes ;
            Dictionary<string, object[]> outDic = new Dictionary<string, object[]>();//{double[10],int[10],double} ratios, counts, weight
            //Dictionary<string, double> minDic = new Dictionary<string, double>();//the min value of each bin
            //Dictionary<string, double> spanDic = new Dictionary<string, double>();//the span of each bin
            Dictionary<string, int> cntDic = new Dictionary<string,int>();//counts by class
            //Statistics.dataPrepPrincipleComponents pca = dpComp.PCA;
            string[] labels = dpComp.Labels;
            for (int i = 0; i < labels.Length; i++)
            {
                string lbl = labels[i];
                //double[][] minmax1 = dpComp.minMaxDic1[lbl];
                //double[][] minmax2 = dpComp.minMaxDic2[lbl];
                //nBins = dpComp.binPropDic1[lbl][0].Length;
                //double min = minmax1[0][0];
                //if (minmax2[0][0] < min) min = minmax2[0][0];
                //double max = minmax1[1][0];
                //if (minmax2[1][0] > max) max = minmax2[1][0];
                //double span = (max - min) / nBins;
                //minDic.Add(lbl, min);
                //spanDic.Add(lbl, span);
                cntDic.Add(lbl,0);
                double[] ratios = new double[nBins];
                int[] cnts = new int[nBins];
                object[] outObjectValues = new object[3];
                outObjectValues[0] = ratios;
                outObjectValues[1] = cnts;
                outObjectValues[2] = 1;
                outDic.Add(lbl, outObjectValues);
            }
            int[] vArrIndex = new int[dpComp.Variables.Length];
            for (int i = 0; i < vArrIndex.Length; i++)
            {
                vArrIndex[i]=inTable.FindField(dpComp.Variables[i]);
            }
            string binFldName = geoUtil.createField(inTable, "BIN", esriFieldType.esriFieldTypeInteger, false);
            string strataFldName = dpComp.StrataField;
            string weightFldName = "Weight";
            int binFldNameIndex = inTable.FindField(binFldName);
            int strataFldNameIndex = inTable.FindField(strataFldName);
            int weightFldNameINdex = inTable.FindField(weightFldName);
            ICursor cur = inTable.Update(null, false);
            IRow rw = cur.NextRow();
            double[] varr = new double[vArrIndex.Length];
            int totalCnt = 0;
            while (rw != null)
            {
                bool check = true;
                for (int i = 0; i < varr.Length; i++)
			    {
                    object vlObj = rw.get_Value(vArrIndex[i]);
                    if(vlObj==null)
                    {
                        check = false;
                        break;
                    }
                    varr[i]=System.Convert.ToDouble(vlObj);
			    }
                if(check)
                {
                    int vl = clus.computNew(varr);
                    //double min;
                    //double span;
                    string g = labels[0];
                    object w = 1;
                    if(strataFldNameIndex>-1)
                    {
                        g = rw.get_Value(strataFldNameIndex).ToString();
                    }
                    if (weightFldNameINdex > -1)
                    {
                        w = rw.get_Value(weightFldNameINdex);
                    }
                    object[] oOut = outDic[g];
                    int[] cntArr = (int[])oOut[1];
                    oOut[2] = w;
                    //min = minDic[g];
                    //span = spanDic[g];
                    //weightDic[g] = w;
                    cntDic[g] += 1;
                    //int b = (int)((vl-min)/span);
                    //if (b >= cntArr.Length) b = cntArr.Length - 1;
                    //if (b < 0) b = 0;
                    //Console.WriteLine("\nValue = " + vl.ToString() + "\nmin = " + min.ToString() + "\nSpan = " + span.ToString() + "\nbin = " + b.ToString());
                    cntArr[vl] = cntArr[vl]+1;
                    rw.set_Value(binFldNameIndex,vl);
                    cur.UpdateRow(rw);
                    totalCnt+=1;
                }
                rw = cur.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            Dictionary<string, double[]> clusterProp = dpComp.ClusterProportions;
            foreach(KeyValuePair<string,object[]> kvp in outDic)
            {
                string ky = kvp.Key;
                int gCnt = cntDic[ky];
                double[] prop = clusterProp[ky];// dpComp.binPropDic1[ky][0];
                object[] outObj = kvp.Value;
                int[] cntArr = (int[])outObj[1];
                double[] rArr = (double[])outObj[0];
                for (int i = 0; i < cntArr.Length; i++)
			    {
                    double p = System.Convert.ToDouble(cntArr[i])/gCnt;
                    double po = prop[i];
                    rArr[i] = po/p;
			    }
            }

            return outDic;
        }

        public void selectCovCorrFeaturesToSample(ITable inputTable, string covCorrModelPath, double proptionOfMean=0.1, double alpha = 0.05)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            Statistics.dataPrepVarCovCorr covCor = new Statistics.dataPrepVarCovCorr();
            covCor.buildModel(covCorrModelPath);
            System.Random rd = new Random();
            double tSamples = System.Convert.ToDouble(esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxMean(covCor.MeanVector,covCor.StdVector,proptionOfMean,alpha));
            int tRecords = inputTable.RowCount(null);
            double gR = tSamples / tRecords;
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            IQueryFilter qf0 = new QueryFilterClass();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = sampleFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = gR;
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public void selectPcaFeaturesToSample(ITable inputTable, string pcaModelPath, double proportionOfMean, double alpha)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inputTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            Statistics.dataPrepPrincipleComponents pca = new Statistics.dataPrepPrincipleComponents();
            pca.buildModel(pcaModelPath);
            System.Random rd = new Random();
            double tSamples = System.Convert.ToDouble(esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxMean(pca.MeanVector, pca.StdVector, proportionOfMean, alpha));
            int tRecords = inputTable.RowCount(null);
            double gR = tSamples / tRecords;
            string sampleFldName = geoUtil.createField(inputTable, "sample", esriFieldType.esriFieldTypeSmallInteger, false);
            IQueryFilter qf0 = new QueryFilterClass();
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = sampleFldName;
            IWorkspace wks = ((IDataset)inputTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                ICursor cur = inputTable.Update(qf, false);
                int sIndex = cur.FindField(sampleFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    double rNum = rd.NextDouble();
                    int ss = 0;
                    double r = gR;
                    if (rNum < r)
                    {
                        ss = 1;
                    }
                    rw.set_Value(sIndex, ss);
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        public IFeatureClass exportFeatures(IFeatureClass inputFeatureClass, string outPath, ISpatialFilter filter)
        {
            
            // Create a name object for the source (shapefile) workspace and open it.
            IDataset inDset = (IDataset)inputFeatureClass;
            IWorkspace sourceWorkspace = (inDset).Workspace;

            // Create a name object for the target (file GDB) workspace and open it.
            string outDbStr = geoUtil.parseDbStr(outPath);
            string outName = System.IO.Path.GetFileName(outPath);
            IWorkspace targetWorkspace = geoUtil.OpenWorkSpace(outDbStr);
            outName = geoUtil.getSafeOutputNameNonRaster(targetWorkspace, outName);
            
            // Create a name object for the source dataset.
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.Name = inDset.Name;
            sourceDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)sourceWorkspace).FullName;

            // Create a name object for the target dataset.
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.Name = outName;
            targetDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)targetWorkspace).FullName; ;

            // Open source feature class to get field definitions.
            //IName sourceName = (IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = inputFeatureClass;

            // Create the objects and references necessary for field validation.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields sourceFields = sourceFeatureClass.Fields;
            IFields targetFields = null;
            IEnumFieldError enumFieldError = null;

            // Set the required properties for the IFieldChecker interface.
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;

            // Validate the fields and check for errors.
            fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
            if (enumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }

            // Find the shape field.
            String shapeFieldName = sourceFeatureClass.ShapeFieldName;
            int shapeFieldIndex = sourceFeatureClass.FindField(shapeFieldName);
            IField shapeField = sourceFields.get_Field(shapeFieldIndex);

            // Get the geometry definition from the shape field and clone it.
            IGeometryDef geometryDef = shapeField.GeometryDef;
            IClone geometryDefClone = (IClone)geometryDef;
            IClone targetGeometryDefClone = geometryDefClone.Clone();
            IGeometryDef targetGeometryDef = (IGeometryDef)targetGeometryDefClone;

            // Cast the IGeometryDef to the IGeometryDefEdit interface.
            IGeometryDefEdit targetGeometryDefEdit = (IGeometryDefEdit)targetGeometryDef;

            // Set the IGeometryDefEdit properties.
            targetGeometryDefEdit.GridCount_2 = 1;
            targetGeometryDefEdit.set_GridSize(0, 0.75);

            // Create the converter and run the conversion.
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertFeatureClass(sourceFeatureClassName, filter, null, targetFeatureClassName, targetGeometryDef, targetFields, "", 1000, 0);

            // Check for errors.
            IInvalidObjectInfo invalidObjectInfo = null;
            enumInvalidObject.Reset();
            while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}", invalidObjectInfo.InvalidObjectID);
            }
            return (IFeatureClass)((IName)targetFeatureClassName).Open();

        }
        public ITable exportTable(ITable inputTable, string outPath, IQueryFilter filter)
        {
            // Create a name object for the source workspace and open it.
            IDataset inDset = (IDataset)inputTable;
            IWorkspace sourceWorkspace = (inDset).Workspace;

            // Create a name object for the target (file GDB) workspace and open it.
            string outDbStr = geoUtil.parseDbStr(outPath);
            string outName = System.IO.Path.GetFileName(outPath);
            IWorkspace targetWorkspace = geoUtil.OpenWorkSpace(outDbStr);
            outName = geoUtil.getSafeOutputNameNonRaster(targetWorkspace, outName);

            // Create a name object for the source dataset.
            ITableName sourceTableName = new TableNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceTableName;
            sourceDatasetName.Name = inDset.Name;
            sourceDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)sourceWorkspace).FullName;

            // Create a name object for the target dataset.
            ITableName targetTableName = new TableNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetTableName;
            targetDatasetName.Name = outName;
            targetDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)targetWorkspace).FullName; ;

            // Open source feature class to get field definitions.
            //IName sourceName = (IName)sourceFeatureClassName;
            ITable sourceTable = inputTable;

            // Create the objects and references necessary for field validation.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields sourceFields = sourceTable.Fields;
            IFields targetFields = null;
            IEnumFieldError enumFieldError = null;

            // Set the required properties for the IFieldChecker interface.
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;

            // Validate the fields and check for errors.
            fieldChecker.Validate(sourceFields, out enumFieldError, out targetFields);
            if (enumFieldError != null)
            {
                // Handle the errors in a way appropriate to your application.
                Console.WriteLine("Errors were encountered during field validation.");
            }

            // Create the converter and run the conversion.
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            IEnumInvalidObject enumInvalidObject = featureDataConverter.ConvertTable(sourceDatasetName, filter, targetDatasetName, targetFields,"",1000,0);

            // Check for errors.
            IInvalidObjectInfo invalidObjectInfo = null;
            enumInvalidObject.Reset();
            while ((invalidObjectInfo = enumInvalidObject.Next()) != null)
            {
                // Handle the errors in a way appropriate to the application.
                Console.WriteLine("Errors occurred for the following feature: {0}", invalidObjectInfo.InvalidObjectID);
            }
            return (Table)((IName)targetTableName).Open();
        }
        public void renameField(ITable inTable, string oldFldName, string newFldName)
        {
            IObjectClassInfo2 objInfo2 = (IObjectClassInfo2)inTable;
            if (!objInfo2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Input Table participates in a composite relationship. Please export this table as a new table and try again!");
                return;
            }
            
            IWorkspace wks = ((IDataset)inTable).Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            try
            {
                int inFldIndex = inTable.Fields.FindField(oldFldName);
                IField inFld = inTable.Fields.get_Field(inFldIndex);
                esriFieldType fType = inFld.Type;
                string outFldName = geoUtil.createField(inTable, newFldName, fType, false);
                IQueryFilter qf = new QueryFilterClass();
                ICursor cur = inTable.Update(qf, false);
                int outFldIndex = cur.FindField(outFldName);
                inFldIndex = cur.FindField(oldFldName);
                IRow rw = cur.NextRow();
                while (rw != null)
                {
                    rw.set_Value(outFldIndex, rw.get_Value(inFldIndex));
                    cur.UpdateRow(rw);
                    rw = cur.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(cur);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }
        public ITable subSetRasterTallyCounts(IFeatureClass inFeatureClass, IRasterDataset[] inRasterDatasets, string outTbl)
        {
            string wksPath = geoUtil.getDatabasePath(outTbl);
            IWorkspace wks = geoUtil.OpenWorkSpace(wksPath);
            string outTblNm = System.IO.Path.GetFileNameWithoutExtension(outTbl);
            outTblNm = geoUtil.getSafeOutputNameNonRaster(wks, outTblNm);
            IFields flds = new FieldsClass();
            IFieldsEdit fldsEdit = (IFieldsEdit)flds;
            ITable outTable = geoUtil.createTable(wks, outTblNm, null);
            string ftrIdNm = geoUtil.createField(outTable,"link",esriFieldType.esriFieldTypeInteger,true);
            string rsFldNm = geoUtil.createField(outTable, "raster", esriFieldType.esriFieldTypeString, true);
            string bndFldNm = geoUtil.createField(outTable, "band", esriFieldType.esriFieldTypeInteger, true);
            string vlFldNm = geoUtil.createField(outTable, "value", esriFieldType.esriFieldTypeString, true);
            string cntFldNm = geoUtil.createField(outTable, "count", esriFieldType.esriFieldTypeInteger, true);
            int idIndex = outTable.FindField(ftrIdNm);
            int rsIndex = outTable.Fields.FindField(rsFldNm);
            int bndIndex = outTable.Fields.FindField(bndFldNm);
            int vlIndex = outTable.Fields.FindField(vlFldNm);
            int cntIndex = outTable.Fields.FindField(cntFldNm);
            int[] fldIndexArr = {idIndex,rsIndex,bndIndex,vlIndex,cntIndex};
            for (int rsDset = 0; rsDset < inRasterDatasets.Length; rsDset++)
            {
                IFunctionRasterDataset inRasterDataset = rsUtil.createIdentityRaster(inRasterDatasets[rsDset]);
                string rsName = ((IDataset)inRasterDatasets[rsDset]).BrowseName;
                Console.WriteLine("Working on image " + rsName);
                int bndCnt = inRasterDataset.RasterInfo.BandCount;
                IEnvelope ext = inRasterDataset.RasterInfo.Extent;
                ISpatialFilter sp = new SpatialFilter();
                sp.GeometryField = inFeatureClass.ShapeFieldName;
                sp.Geometry = ext;
                sp.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor ftrCur = inFeatureClass.Search(sp, true);
                IFeature ftr = ftrCur.NextFeature();
                while (ftr != null)
                {
                    Dictionary<string, int>[] outDic = new Dictionary<string, int>[bndCnt];
                    for (int b = 0; b < bndCnt; b++)
                    {
                        outDic[b] = new Dictionary<string, int>();
                    }
                    IGeometry geo = ftr.Shape;
                    int ftrId = ftr.OID;
                    ITopologicalOperator4 tp = (ITopologicalOperator4)geo;
                    IGeometry geo2 = tp.Intersect(ext, esriGeometryDimension.esriGeometry2Dimension);
                    IFunctionRasterDataset rsDsetClip = rsUtil.clipRasterFunction(inRasterDataset, geo2, esriRasterClippingType.esriRasterClippingOutside);
                    IRaster rs = rsUtil.createRaster(rsDsetClip);
                    IRasterCursor rsCur = ((IRaster2)rs).CreateCursorEx(null);
                    IPixelBlock pb = null;
                    do
                    {
                        pb = rsCur.PixelBlock;
                        for (int b = 0; b < bndCnt; b++)
                        {
                            for (int r = 0; r < pb.Height; r++)
                            {
                                for (int c = 0; c < pb.Width; c++)
                                {
                                    object vlObj = pb.GetVal(b, c, r);
                                    if (vlObj != null)
                                    {
                                        string vl = vlObj.ToString();
                                        int rCnt = 1;
                                        if (outDic[b].TryGetValue(vl, out rCnt))
                                        {
                                            outDic[b][vl] = rCnt + 1;
                                        }
                                        else
                                        {
                                            outDic[b][vl] = 1;
                                        }
                                    }
                                }
                            }
                        }

                    } while (rsCur.Next() == true);
                    updateTable(outTable, outDic,rsName, ftrId, fldIndexArr);
                    ftr = ftrCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            }
            return outTable;
        }

        private void updateTable(ITable outTable, Dictionary<string, int>[] outDic, string rsName, int ftrId, int[] fldIndexArr)
        {
            ICursor iCur = outTable.Insert(true);
            IRowBuffer rw = outTable.CreateRowBuffer();
            object[] vlObj = { ftrId, rsName, 0, 0, 0 };
            for (int b = 0; b < outDic.Length; b++)
            {
                Dictionary<string, int> dic = outDic[b];
                vlObj[2] = b+1;
                foreach (KeyValuePair<string,int> kvp in dic)
                {
                    vlObj[3] = System.Convert.ToInt32(kvp.Key);
                    vlObj[4] = kvp.Value;
                    for (int i = 0; i < fldIndexArr.Length; i++)
                    {
                        rw.set_Value(fldIndexArr[i], vlObj[i]);
                    }
                    iCur.InsertRow(rw);
                }
            }
            iCur.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(iCur);
        }
        public IFeatureClass createRandomSample(IFeatureClass boundaryFtrCls, int numPlots, string outPath)
        {
            ISpatialReference sp = ((IGeoDataset)boundaryFtrCls).SpatialReference;
            IFeatureClass pntCls = geoUtil.createFeatureClass(outPath, null, esriGeometryType.esriGeometryPoint,sp);   
            List<IGeometry> geoLst = new List<IGeometry>();
            List<double> geoArea = new List<double>();
            double totalArea = 0;
            IFeatureCursor ftrCurBoundary = boundaryFtrCls.Search(null, true);
            IFeature ftrBnd = ftrCurBoundary.NextFeature();
            while (ftrBnd != null)
            {
                IGeometry geo = ftrBnd.ShapeCopy;
                double area = ((IArea)geo).Area;
                geoLst.Add(geo);
                geoArea.Add(area);
                totalArea += area;
                ftrBnd = ftrCurBoundary.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCurBoundary);
            int[] sampleSize = new int[geoArea.Count];
            for (int i = 0; i < sampleSize.Length; i++)
			{
                double dArea = geoArea[i];
                int ss = System.Convert.ToInt32(dArea/totalArea*numPlots);
                if(ss>0) sampleSize[i] = ss;
                else sampleSize[i] = 1;
	        }
            IFeatureCursor ftrCur = pntCls.Insert(true);
            IFeatureBuffer ftrBuff = pntCls.CreateFeatureBuffer();
            Random rnd = new Random();
            for(int i = 0;i<geoLst.Count;i++)
	        {
                //Console.WriteLine("working on geo " + i.ToString());
                HashSet<string> xyHash = new HashSet<string>();
                IGeometry geo = geoLst[i];
                int np = sampleSize[i];
                double pntCnt = 0;
                IEnvelope env = geo.Envelope;
                double xMin = env.XMin;
                double xMax = env.XMax;
                double xRange = xMax - xMin;
                double yMin = env.YMin;
                double yMax = env.YMax;
                double yRange = yMax - yMin;
                while (pntCnt < np)
                {
                    double nx = (rnd.NextDouble() * xRange) + xMin;
                    double ny = (rnd.NextDouble() * yRange) + yMin;
                    //string nxny = nx.ToString() + ":" + ny.ToString();
                    //if (xyHash.Add(nxny))
                    IPoint pnt = new ESRI.ArcGIS.Geometry.PointClass();
                    pnt.X = nx;
                    pnt.Y = ny;
                    pnt.SpatialReference = sp;
                    IRelationalOperator rOp = (IRelationalOperator)pnt;
                    if (rOp.Within(geo))
                    {
                        ftrBuff.Shape = pnt;
                        ftrCur.InsertFeature(ftrBuff);
                        pntCnt++;
                    }
                
                }
            }
            ftrCur.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            return pntCls;
        }
        /// <summary>
        /// samples a featureclass given a sample location point file a featureclass to sample, and the sample field to get info from 
        /// </summary>
        /// <param name="sampleLocations">the sample locations</param>
        /// <param name="featureClassToSample">the feature class to sample</param>
        /// <param name="fld">the field to sample</param>
        public void sampleFeatureClass(IFeatureClass sampleLocations, IFeatureClass featureClassToSample, IField[] flds)
        {

            IDataset dSet = (IDataset)featureClassToSample;
            int[] fldIndexArr = new int[flds.Length];
            int[] uFldIndexArr = new int[flds.Length];
            for (int f = 0; f < flds.Length; f++)
            {
                IField fld = flds[f];
                string als = fld.Name;
                if (als.Length > 12)
                {
                    als = als.Substring(12);
                }
                geoUtil.createField(sampleLocations, als, fld.Type);
                fldIndexArr[f] = featureClassToSample.FindField(fld.Name);
                uFldIndexArr[f] = sampleLocations.FindField(als);
            }

            IFeatureCursor sCur = featureClassToSample.Search(null, true);
            IFeature sRow = sCur.NextFeature();
            while (sRow != null)
            {
                IGeometry geo = sRow.Shape;
                ISpatialFilter spFlt = new SpatialFilterClass();
                spFlt.Geometry = geo;
                spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spFlt.GeometryField = sampleLocations.ShapeFieldName;
                IFeatureCursor sCur2 = sampleLocations.Update(spFlt, true);
                IFeature sRow2 = sCur2.NextFeature();
                while (sRow2 != null)
                {
                    for (int f = 0; f < flds.Length; f++)
                    {
                        int fldIndex = fldIndexArr[f];
                        int uFldIndex = uFldIndexArr[f];
                        object sFldV = sRow.get_Value(fldIndex);
                        sRow2.set_Value(uFldIndex, sFldV);

                    }
                    sCur2.UpdateFeature(sRow2);
                    sRow2 = sCur2.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur2);
                sRow = sCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
        }
        public IFeatureClass splitPolyFeatures(IFeatureClass inputFtrCls, int numberOfSplits, string outFtrClsPath, int buffer = 0)
        {
            IFeatureClass outFtrCls = geoUtil.createFeatureClass(outFtrClsPath, null, inputFtrCls.ShapeType, ((IGeoDataset)inputFtrCls).SpatialReference);
            IFeatureCursor inCur = outFtrCls.Insert(true);
            IFeatureBuffer ftrBuff = outFtrCls.CreateFeatureBuffer();
            IFeatureCursor ftrCur = inputFtrCls.Search(null, true);
            IFeature ftr = ftrCur.NextFeature();
            while (ftr != null)
            {
                IGeometry geo = ftr.ShapeCopy;
                double xMin = geo.Envelope.XMin;
                double xMax = geo.Envelope.XMax;
                double yMin = geo.Envelope.YMin;
                double yMax = geo.Envelope.YMax;
                double xRange = xMax - xMin;
                double yRange = yMax - yMin;
                double tArea = xRange * yRange;
                double subArea = tArea / numberOfSplits;
                double step = System.Math.Sqrt(subArea);
                for (double x = xMin; x < xMax; x = x + step)
                {
                    double nx = x - buffer;
                    for (double y = yMin; y < yMax; y = y + step)
                    {
                        double ny = y - buffer;
                        IPolygon poly = new PolygonClass();
                        IPointCollection ptCol = (IPointCollection)poly;
                        IPoint pnt = new PointClass();
                        pnt.PutCoords(nx, ny);
                        IPoint pnt2 = new PointClass();
                        pnt2.PutCoords(nx, ny + step + buffer);
                        IPoint pnt3 = new PointClass();
                        pnt3.PutCoords(nx + step+ buffer, ny + step + buffer);
                        IPoint pnt4 = new PointClass();
                        pnt4.PutCoords(nx + step + buffer, ny);
                        ptCol.AddPoint(pnt);
                        ptCol.AddPoint(pnt2);
                        ptCol.AddPoint(pnt3);
                        ptCol.AddPoint(pnt4);
                        poly.Close();
                        ftrBuff.Shape = poly;
                        inCur.InsertFeature(ftrBuff);
                    }
                }
                ftr = ftrCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(inCur);
            return outFtrCls;
        }
        /// <summary>
        /// Creates mosaic datasets based on the images within a specified directory and the extents each geometry 
        /// </summary>
        /// <param name="geoArray">The array of geometries used to define the extent of each mosaic</param>
        /// <param name="imageWorkspace">the image workspace used to search from images</param>
        /// <param name="outWorkspace">the output workspace</param>
        /// <returns>an array of FunctionDatasets containing each mosaic</returns>
        public IFunctionRasterDataset[] createMosaicDatasetFromGeometry(IGeometry[] geoArray, string outPrefix, IWorkspace imageWorkspace, IWorkspace outWorkspace)
        {
            List<IFunctionRasterDataset> dSetLst = new List<IFunctionRasterDataset>();
            try
            {
                int cnt = 1;
                object[][] rsExt = getRasterExtents(imageWorkspace);
                foreach (IGeometry geo in geoArray)
                {
                    Console.WriteLine("Working on geo " + cnt.ToString());
                    IEnvelope env = geo.Envelope;
                    IEnvelope cmbEnv;
                    string[] rstLst = getRasterList(env, rsExt, out cmbEnv);
                    if (rstLst.Length > 0)
                    {
                        dSetLst.Add((IFunctionRasterDataset)rsUtil.createIdentityRaster(rsUtil.mosaicRastersFunction(outWorkspace, outPrefix + "_" + cnt.ToString(), rstLst, cmbEnv, esriMosaicMethod.esriMosaicNone, rstMosaicOperatorType.MT_FIRST, true, true, false, false)));
                        cnt++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return dSetLst.ToArray();
        }
        /// <summary>
        /// Creates mosaic datasets based on the images within a specified directory and the extents each feature within a specified featureclass 
        /// </summary>
        /// <param name="ftrCls">The feature class used to define the extent of each mosaic</param>
        /// <param name="imageWorkspace">the image workspace used to search from images</param>
        /// <param name="outWorkspace">the output workspace</param>
        /// <returns>an array of FunctionDatasets containing each mosaic</returns>
        public IFunctionRasterDataset[] tiledMosaics(IFeatureClass ftrCls, IWorkspace imageWorkspace, IWorkspace outWorkspace)
        {
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = ftrCls.OIDFieldName+","+ftrCls.ShapeFieldName;
            IFeatureCursor ftrCur = ftrCls.Search(qf, true);
            IFeature ftr = ftrCur.NextFeature();
            List<IGeometry> geoLst = new List<IGeometry>();
            while(ftr!=null)
            {
                IGeometry geo = ftr.ShapeCopy;
                geoLst.Add(geo);
                ftr = ftrCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            return createMosaicDatasetFromGeometry(geoLst.ToArray(), "Tile_", imageWorkspace, outWorkspace);
        }

        private object[][] getRasterExtents(IWorkspace imageWorkspace)
        {
            
            List<object[]> outLst = new List<object[]>();
            IEnumDataset eDset = imageWorkspace.get_Datasets(esriDatasetType.esriDTRasterDataset);
            eDset.Reset();
            List<IEnvelope> envLst = new List<IEnvelope>();
            IDataset dSet = eDset.Next();
            while (dSet != null)
            {
                //Console.WriteLine("\tadding raster " + dSet.Name);
                IEnvelope env = ((IGeoDataset)dSet).Extent;
                string rsName = imageWorkspace.PathName + "\\" + dSet.BrowseName;
                object[] lsObj = { env, rsName };
                outLst.Add(lsObj);
                envLst.Add(env);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(dSet);
                dSet = eDset.Next();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(eDset);
            //Console.WriteLine("Converting to Array");
            return outLst.ToArray();
        }

        private string[] getRasterList(IEnvelope env, object[][] rsArra, out IEnvelope cmbEnv)
        {
            cmbEnv = new EnvelopeClass();
            cmbEnv.SpatialReference = ((IEnvelope)rsArra[0][0]).SpatialReference;
            List<string> rsLst = new List<string>();
            IRelationalOperator rOp = (IRelationalOperator)env;
            for (int i = 0; i < rsArra.Length; i++)
            {
                IEnvelope rsExt = (IEnvelope)rsArra[i][0];
                if (!rOp.Disjoint(rsExt))
                {
                    cmbEnv.Union(rsExt);
                    rsLst.Add(rsArra[i][1].ToString());
                }
            }
            return rsLst.ToArray();

        }
    }
}
