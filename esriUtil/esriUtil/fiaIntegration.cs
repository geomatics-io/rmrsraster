using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
namespace esriUtil
{
    public class fiaIntegration
    {
        public fiaIntegration(string dbLocation)
        {
            setDbProvider();
            string ext = System.IO.Path.GetExtension(dbLocation).ToLower();
            string firstP = "Provider=Microsoft.ACE.OLEDB."+ oleAccessdbprovider +";Data Source=";
            string lastP = ";Persist Security Info=False;";
            switch (ext)
            {
                case ".xls":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + ";HDR=YES\";";
                    break;
                case ".xlsx":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + " Xml;HDR=YES\";";
                    break;
                case ".mdb":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";User Id=admin;Password=;";
                    break;
                case ".txt":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
                    break;
                case ".dbf":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
                    break;
                case ".dbc":
                    firstP = "Provider=vfpoledb;Data Source=";
                    lastP = ";Collating Sequence=machine;";
                    break;
                default:
                    break;
            }
            dbLc = firstP +dbLocation + lastP;
            //Console.WriteLine(dbLc);
        }

        private void setDbProvider()
        {
            av = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Access)+".0";
            ev = oleExceldbprovider = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Excel) + ".0";
            //if (av != "0.0")
            //{
            //    oleAccessdbprovider = av;
            //}
            //if (ev != "0.0")
            //{
            //    oleExceldbprovider = ev;
            //}
            //Console.WriteLine(av);
            //Console.WriteLine(ev);

        }
        ~fiaIntegration()
        {
        }
        string av = "0.0";
        string ev = "0.0";
        string oleAccessdbprovider = "12.0";
        string oleExceldbprovider = "12.0";
        private string dbLc = "";
        public enum biomassTypes { BAA, TPA, J_AGB, J_SAGB, J_BAGB, J_FAGB, J_TPAGB, MDBH, MHT };
        public IFeatureClass SampleFeatureClass{get;set;}
        public string PlotCnField{get;set;}
        public string SubPlotField{get;set;}
        private geoDatabaseUtility geoDbUtil = new geoDatabaseUtility();
        private biomassTypes[] fldArr = null;
        public biomassTypes[] BiomassTypes { get { return fldArr; } set { fldArr = value; } }
        private bool regen = false;
        public bool Regen { get { return regen; } set { regen = value; } }
        HashSet<string> unSp = new HashSet<string>();
        HashSet<string> sUnSp = new HashSet<string>();
        Dictionary<string,object[]> vlDic = new Dictionary<string,object[]>();
        Dictionary<string, object[]> sVlDic = new Dictionary<string, object[]>();
        //Dictionary<string, object[]> grVlDic = new Dictionary<string, object[]>();
        Dictionary<string,string> grpDic = new Dictionary<string,string>();
        bool grp = false;
        public Dictionary<string,string> GroupDic { get { return grpDic; } set { grp = true; grpDic = value; } }
        HashSet<string> unPlots = new HashSet<string>();
        Dictionary<string, object[]> bmDic = new Dictionary<string, object[]>();
        public static void summarizeBiomassPolygon(IFeatureClass pointFtr, IField[] fldsToSummarize, IFunctionRasterDataset strataRaster, IFeatureClass standsFtr, geoDatabaseUtility geoUtil = null, rasterUtil rsUtil = null)
        {
            if (geoUtil == null) geoUtil = new geoDatabaseUtility();
            if(rsUtil == null) rsUtil = new rasterUtil();
            int cnt = 0;
        //need to work on calculating N
            Dictionary<string,double[][]> vlDic = getDictionaryValues(pointFtr, fldsToSummarize, strataRaster, geoUtil, rsUtil); //Strata: SummaryFields [{sum,sum2,cnt},...]
            int[] meanFldIndex = new int[fldsToSummarize.Length];
            int[] varFldIndex = new int[fldsToSummarize.Length];
            int[] cntFldIndex = new int[fldsToSummarize.Length];
            //string cntName = geoUtil.createField(standsFtr, "n", esriFieldType.esriFieldTypeInteger, false);
            //int cntIndex = standsFtr.FindField(cntName);
            foreach (IField fld in fldsToSummarize)
            {
                string sName = geoUtil.createField(standsFtr, "v_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                varFldIndex[cnt] = standsFtr.FindField(sName);
                string mName = geoUtil.createField(standsFtr, "m_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                meanFldIndex[cnt] = standsFtr.FindField(mName);
                string cntName = geoUtil.createField(standsFtr, "n_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                cntFldIndex[cnt] = standsFtr.FindField(cntName);
                cnt++;
            }
            IFeatureCursor uCur = standsFtr.Update(null, true);
            IFeature uFtr = uCur.NextFeature();
            while (uFtr != null)
            {
                ESRI.ArcGIS.Geometry.IGeometry geo = uFtr.Shape;
                IFunctionRasterDataset cRs = rsUtil.clipRasterFunction(strataRaster, geo, esriRasterClippingType.esriRasterClippingOutside);
                //Console.WriteLine("Clipping raster");
                Dictionary<string, double> rsStrataPropDic = getStrataProportion(cRs,rsUtil); //Strata: proportion of area
                //int tn = 0;
                //double[] tn = new double[meanFldIndex.Length];
                double[][] updateValuesArr = new double[meanFldIndex.Length][];
                for (int i = 0; i < meanFldIndex.Length; i++)
			    {
                    updateValuesArr[i] = new double[3];
			    }
                foreach (KeyValuePair<string, double> kvp in rsStrataPropDic)
                {
                    string stratum = kvp.Key;
                    double proportion = kvp.Value;
                    //Console.WriteLine(stratum + " = " + proportion.ToString());
                    double[][] vlDicArr;
                    if (vlDic.TryGetValue(stratum, out vlDicArr))
                    {
                        //double n = vlDicArr[0][2];
                        //tn += System.Convert.ToInt32(n);
                        for (int i = 0; i < meanFldIndex.Length; i++)
                        {
                            double[] dArr = vlDicArr[i];
                            double n = dArr[2];
                            //tn[i] += n;
                            double s=dArr[0];
                            double s2=dArr[1];
                            updateValuesArr[i][0] += (s/n) * proportion;//mean
                            updateValuesArr[i][1] += (s2-Math.Pow(s,2)/n)/(n-1) * proportion;//variance
                            updateValuesArr[i][2] += n;
                        }

                    }
                }
                //uFtr.set_Value(cntIndex, tn);
                for (int i = 0; i < meanFldIndex.Length; i++)
                {
                    uFtr.set_Value(meanFldIndex[i], updateValuesArr[i][0]);
                    uFtr.set_Value(varFldIndex[i], updateValuesArr[i][1]);
                    uFtr.set_Value(cntFldIndex[i], updateValuesArr[i][2]);
                }
                uCur.UpdateFeature(uFtr);
                uFtr = uCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);
        }

        private static Dictionary<string, double> getStrataProportion(IFunctionRasterDataset strataRaster,rasterUtil rsUtil)
        {
            IRaster2 rs2 = (IRaster2)rsUtil.createRaster(strataRaster);
            Dictionary<string, double> outDic = new Dictionary<string, double>();
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            //Console.WriteLine(((IRasterProps)rs2).Height.ToString() + ((IRasterProps)rs2).Height.ToString());
            int n = 0;
            do
            {
                IPixelBlock pb = rsCur.PixelBlock;
                //Console.WriteLine("PixelBLock w_h = " + pb.Width.ToString() + "_" + pb.Height.ToString());
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vlObj = pb.GetVal(0, c, r);
                        if (vlObj != null)
                        {
                            string vl = vlObj.ToString();
                            double vlCnt;
                            if (outDic.TryGetValue(vl, out vlCnt))
                            {
                                outDic[vl] = vlCnt + 1;
                            }
                            else
                            {
                                outDic.Add(vl, 1);
                            }
                            n += 1;
                        }
                        else
                        {
                            //Console.WriteLine("VL Null");
                        }

                    }
                }
            } while (rsCur.Next() == true);
            //Console.WriteLine("OutDic Count = " + outDic.Count.ToString());
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rsCur);
            foreach (string s in outDic.Keys.ToArray())
            {
                double vl = outDic[s];
                outDic[s] = vl / n;
            }
            return outDic;
        }

        private static Dictionary<string, double[][]> getDictionaryValues(ESRI.ArcGIS.Geodatabase.IFeatureClass pointFtr, ESRI.ArcGIS.Geodatabase.IField[] fldsToSummarize, IFunctionRasterDataset strataRaster, geoDatabaseUtility geoUtil, rasterUtil rsUtil)
        {
            IRaster2 rs2 = (IRaster2)rsUtil.createRaster(strataRaster);
            int[] ptfldIndex = new int[fldsToSummarize.Length];
            for (int i = 0; i < ptfldIndex.Length; i++)
            {
                ptfldIndex[i] = pointFtr.FindField(fldsToSummarize[i].Name);
            }
            Dictionary<string, double[][]> outDic = new Dictionary<string, double[][]>();
            IFeatureCursor sCur = pointFtr.Search(null, true);
            IFeature sFtr = sCur.NextFeature();
            while (sFtr != null)
            {
                IGeometry geo = sFtr.Shape;
                IPoint pnt = (IPoint)geo;
                int clm, rw;
                rs2.MapToPixel(pnt.X, pnt.Y,out clm, out rw);
                object strataVlObj = rs2.GetPixelValue(0, clm, rw);
                if(strataVlObj!=null)
                {
                    string strataVl = strataVlObj.ToString();
                    double[][] vlArr;
                    if (outDic.TryGetValue(strataVl, out vlArr))
                    {
                        for (int i = 0; i < ptfldIndex.Length; i++)
                        {
                            object vlObj = sFtr.get_Value(ptfldIndex[i]);
                            if (vlObj != null)
                            {
                                double vl = System.Convert.ToDouble(vlObj);
                                vlArr[i][0] += vl;
                                vlArr[i][1] += (vl * vl);
                                vlArr[i][2] += 1;
                            }
                        }
                    }
                    else
                    {
                        vlArr = new double[fldsToSummarize.Length][];
                        for (int i = 0; i < ptfldIndex.Length; i++)
                        {
                            double[] vlSumArr = new double[3];
                            object vlObj =sFtr.get_Value(ptfldIndex[i]);
                            if (vlObj != null)
                            {
                                double vl =  System.Convert.ToDouble(vlObj);
                                vlSumArr[0] = vl;
                                vlSumArr[1] = (vl * vl);
                                vlSumArr[2] = 1;
                            }
                            vlArr[i] = vlSumArr;
                        }
                        outDic[strataVl] = vlArr;
                    }
                }
                sFtr = sCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
            return outDic;
        }
        public static void summarizeBiomassPolygon(IFeatureClass pointFtr, IField[] fldsToSummarize, IFeatureClass strataFtr, IFeatureClass standsFtr = null, geoDatabaseUtility geoUtil=null )
        {
            if(geoUtil==null) geoUtil = new geoDatabaseUtility();
            int cnt = 0;
            int[] ptFldIndex = new int[fldsToSummarize.Length];
            int[] meanFldIndex = new int[fldsToSummarize.Length];
            int[] varFldIndex = new int[fldsToSummarize.Length];
            int[] cntFldIndex = new int[fldsToSummarize.Length];
            foreach (IField fld in fldsToSummarize)
            {

                ptFldIndex[cnt] = pointFtr.FindField(fld.Name);
                string sName = geoUtil.createField(strataFtr, "v_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                varFldIndex[cnt] = strataFtr.FindField(sName);
                string mName = geoUtil.createField(strataFtr, "m_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                meanFldIndex[cnt] = strataFtr.FindField(mName);
                string cntName = geoUtil.createField(strataFtr, "n_" + fld.Name, esriFieldType.esriFieldTypeInteger, false);
                cntFldIndex[cnt] = strataFtr.FindField(cntName);
                cnt++;
            }
            IFeatureCursor uCur = strataFtr.Update(null, true);
            IFeature uFtr = uCur.NextFeature();
            while (uFtr != null)
            {
                ISpatialFilter sFilt = new SpatialFilter();
                sFilt.Geometry = uFtr.Shape;
                sFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                double[][] fldVlsArr = new double[fldsToSummarize.Length][];
                for (int i = 0; i < fldsToSummarize.Length; i++)
                {
                    fldVlsArr[i] = new double[3];
                }
                IFeatureCursor psCur = pointFtr.Search(sFilt, true);
                IFeature psFtr = psCur.NextFeature();
                
                while (psFtr != null)
                {
                    for (int i = 0; i < ptFldIndex.Length; i++)
                    {
                        int indexVl = ptFldIndex[i];
                        object objVl = psFtr.get_Value(indexVl);
                        if (objVl != null)
                        {
                            double vl = System.Convert.ToDouble(objVl);
                            double vl2 = vl*vl;
                            fldVlsArr[i][0] += vl;
                            fldVlsArr[i][1] += vl2;
                            fldVlsArr[i][2] += 1;
                        }
                    }
                    psFtr = psCur.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(psCur);
                for (int i = 0; i < ptFldIndex.Length; i++)
                {
                    double s = fldVlsArr[i][0];
                    double s2 = fldVlsArr[i][1];
                    double n = fldVlsArr[i][2];
                    double mean = s / n;//mean
                    double var = (s2 - (Math.Pow(s, 2) / n)) / (n - 1);//variance
                    int mIndex = meanFldIndex[i];
                    int vIndex = varFldIndex[i];
                    int cntIndex = cntFldIndex[i];
                    uFtr.set_Value(mIndex, mean);
                    uFtr.set_Value(vIndex, var);
                    uFtr.set_Value(cntIndex, n);
                }
                uCur.UpdateFeature(uFtr);
                uFtr = uCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);
            if (standsFtr != null)
            {
                calcStandMeans(strataFtr, standsFtr, meanFldIndex, varFldIndex, cntFldIndex, fldsToSummarize,geoUtil);
            }
        }
        private static void calcStandMeans(IFeatureClass strataFtr, IFeatureClass standsFtr, int[] meanStrataFldIndex, int[] varStrataFldIndex, int[] countFldStrataIndex, IField[] fldsToSummarize, geoDatabaseUtility geoUtil)
        {
            int cnt = 0;
            int[] ptFldIndex = new int[fldsToSummarize.Length];
            int[] meanFldIndex = new int[fldsToSummarize.Length];
            int[] varFldIndex = new int[fldsToSummarize.Length];
            int[] cntFldIndex = new int[fldsToSummarize.Length];
            foreach (IField fld in fldsToSummarize)
            {
                string sName = geoUtil.createField(standsFtr, "v_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                varFldIndex[cnt] = standsFtr.FindField(sName);
                string mName = geoUtil.createField(standsFtr, "m_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                meanFldIndex[cnt] = standsFtr.FindField(mName);
                string cName = geoUtil.createField(standsFtr, "n_" + fld.Name, esriFieldType.esriFieldTypeDouble, false);
                cntFldIndex[cnt] = standsFtr.FindField(cName);
                cnt++;
            }
            IFeatureCursor uCur = standsFtr.Update(null, true);
            IFeature uFtr = uCur.NextFeature();
            while (uFtr != null)
            {
                ESRI.ArcGIS.Geometry.IGeometry geo = uFtr.Shape;
                ISpatialFilter spFlt = new SpatialFilter();
                spFlt.Geometry = geo;
                spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                double totalArea = 0;
                IFeatureCursor sCur = strataFtr.Search(spFlt, true);
                IFeature sFtr = sCur.NextFeature();
                double[][] vlArr = new double[meanFldIndex.Length][];
                for (int i = 0; i < meanFldIndex.Length; i++)
                {
                    vlArr[i] = new double[3];
                }
                while (sFtr != null)
                {
                    ESRI.ArcGIS.Geometry.IGeometry sgeo = sFtr.Shape;
                    ESRI.ArcGIS.Geometry.ITopologicalOperator4 topo = (ESRI.ArcGIS.Geometry.ITopologicalOperator4)sgeo;
                    ESRI.ArcGIS.Geometry.IGeometry sgeo2 = topo.Intersect(geo, ESRI.ArcGIS.Geometry.esriGeometryDimension.esriGeometry2Dimension);
                    double subArea = (((ESRI.ArcGIS.Geometry.IArea)sgeo2).Area);
                    totalArea += subArea;
                    for (int i = 0; i < meanFldIndex.Length; i++)
                    {
                        vlArr[i][0] += System.Convert.ToDouble(sFtr.get_Value(meanStrataFldIndex[i])) * subArea;
                        vlArr[i][1] += System.Convert.ToDouble(sFtr.get_Value(varStrataFldIndex[i])) * subArea;
                        vlArr[i][2] += System.Convert.ToDouble(sFtr.get_Value(countFldStrataIndex[i]));
                    }
                    sFtr = sCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sCur);
                if (totalArea != 0)
                {
                    for (int i = 0; i < meanFldIndex.Length; i++)
                    {
                        uFtr.set_Value(meanFldIndex[i], vlArr[i][0]/totalArea);
                        uFtr.set_Value(varFldIndex[i], vlArr[i][1]/totalArea);
                        uFtr.set_Value(cntFldIndex[i], vlArr[i][2]);
                    }
                    uCur.UpdateFeature(uFtr);
                }
                uFtr = uCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);
        }
        private bool less5 = false;
        public bool UseLessThan5 { get { return less5; } set { less5 = value; } }
        public void summarizeBiomass()
        {
            try
            {
                if (SubPlotField == null) SubPlotField = "";
                //Console.WriteLine("Creating FIlling Plots Table");
                createAndFillPlotsTable();
                //Console.WriteLine("Creating And Filling Tree Ref Table");
                createAndFillTreesRef();
                //Console.WriteLine("Adding fields");
                //need to add in groups and update unSp
                addFields();
                if (regen)
                {
                    createAndFillRegenRef();
                    //System.Windows.Forms.MessageBox.Show(String.Join(", ", rgDic.Keys.ToArray()));
                    addRegenFields();
                }
                if (less5)
                {
                    addFields("s");
                }
                IQueryFilter qryFlt = new QueryFilterClass();
                string nFldNm = "";
                string fldNm = "";
                IFeatureCursor ftrCur = SampleFeatureClass.Update(null,true);
                IFeature ftr = ftrCur.NextFeature();
                int cnIndex = ftrCur.FindField(PlotCnField);
                int subIndex = ftrCur.FindField(SubPlotField);
                double divs = (43560 / (Math.PI * Math.Pow(24, 2)));
                double rdivs = (43560 / (Math.PI * Math.Pow(6.8, 2)));
                if (SubPlotField == "" || SubPlotField == null)
                {
                    divs = divs / 4;
                    rdivs = rdivs / 4;
                }
                List<string> allEstFld = Enum.GetNames(typeof(biomassTypes)).ToList();
                while (ftr!=null)
                {
                    string pCn = ftr.get_Value(cnIndex).ToString();
                    int sPl = 0;
                    if(subIndex>-1) sPl = System.Convert.ToInt32(ftr.get_Value(subIndex));
                    if (regen)
                    {
                        foreach (string rt in rUnSp)
                        {
                            string ky = pCn + "_" + sPl + "_" + rt;
                            int rgCnt;
                            double cnt = 0;
                            if (rgDic.TryGetValue(ky, out rgCnt))
                            {
                                cnt = rdivs * rgCnt;
                            }
                            else
                            {
                                cnt = 0;
                            }
                            int rgIndex = ftr.Fields.FindField("RTPA_" + rt);
                            ftr.set_Value(rgIndex, cnt);
                        }
                    }
                    if (less5)
                    {
                        foreach (string t in sUnSp)//(biomassTypes s in fldArr)
                        {
                            string ky = pCn + "_" + sPl + "_" + t;//CN_SubPlot_species
                            object[] vls;
                            if (sVlDic.TryGetValue(ky, out vls))
                            {
                            }
                            else
                            {
                                vls = new object[12];
                                vls[0] = pCn;
                                vls[1] = sPl;
                                vls[2] = t;
                                vls[3] = 0;//MDBH
                                vls[4] = 0;//AGB
                                vls[5] = 0;//StemAGB
                                vls[6] = 0;//ButtAGB
                                vls[7] = 0;//FoliageAGB
                                vls[8] = 0;//TopAGB
                                vls[9] = 0;//BAA
                                vls[10] = 0;//TPA
                                vls[11] = 0;//MHT
                            }
                            foreach (biomassTypes s in fldArr)
                            {
                                string btStr = s.ToString();
                                int arrayIndex = allEstFld.IndexOf(btStr);
                                double sdivs2 = rdivs;
                                if (arrayIndex <= 1)
                                {
                                    arrayIndex = 9 + arrayIndex;
                                }
                                else
                                {
                                    if (arrayIndex <= 6)
                                    {
                                        arrayIndex = 2 + arrayIndex;
                                        sdivs2 = rdivs / 2000;//tons
                                    }
                                    else if (arrayIndex == 7)
                                    {
                                        arrayIndex = 3;
                                        double d = System.Convert.ToDouble(vls[10]);
                                        if (d < 1) sdivs2 = 1;
                                        else
                                        {
                                            sdivs2 = 1 / d;
                                        }
                                    }
                                    else
                                    {
                                        arrayIndex = 11;
                                        double d = System.Convert.ToDouble(vls[10]);
                                        if (d < 1) sdivs2 = 1;
                                        else
                                        {
                                            sdivs2 = 1 / d;
                                        }
                                    }

                                }

                                string spCd = vls[2].ToString();
                                fldNm = "s" + s + "_" + spCd;
                                //Console.WriteLine(fldNm);
                                int fldIndex = ftrCur.FindField(fldNm);
                                if (fldIndex == -1)
                                {
                                    fldIndex = ftrCur.FindField(nFldNm.Substring(0, 10));
                                }
                                object vl = (System.Convert.ToDouble(vls[arrayIndex])) * sdivs2;
                                ftr.set_Value(fldIndex, vl);

                            }
                        }
                    }

                    foreach (string t in unSp)//(biomassTypes s in fldArr)
	                {
                        string ky = pCn+"_"+sPl+"_"+t;//CN_SubPlot_species
                        object[] vls;
                        if(vlDic.TryGetValue(ky,out vls))
                        {
                        }
                        else
                        {
                            vls = new object[12];
                            vls[0] = pCn;
                            vls[1] = sPl;
                            vls[2] = t;
                            vls[3] = 0;//MDBH
                            vls[4] = 0;//AGB
                            vls[5] = 0;//StemAGB
                            vls[6] = 0;//ButtAGB
                            vls[7] = 0;//FoliageAGB
                            vls[8] = 0;//TopAGB
                            vls[9] = 0;//BAA
                            vls[10] = 0;//TPA
                            vls[11] = 0;//MHT
                        }
                        foreach (biomassTypes s in fldArr)
                        {
                            string btStr = s.ToString();
                            int arrayIndex = allEstFld.IndexOf(btStr);
                            double divs2 = divs;
                            if (arrayIndex <= 1)
                            {
                                arrayIndex = 9 + arrayIndex;
                            }
                            else
                            {
                                if (arrayIndex <= 6)
                                {
                                    arrayIndex = 2 + arrayIndex;
                                    divs2 = divs / 2000;//tons
                                }
                                else if (arrayIndex == 7)
                                {
                                    arrayIndex = 3;
                                    double d = System.Convert.ToDouble(vls[10]);
                                    if (d < 1) divs2 = 1;
                                    else
                                    {
                                        divs2 = 1 / d;
                                    }
                                }
                                else
                                {
                                    arrayIndex = 11;
                                    double d = System.Convert.ToDouble(vls[10]);
                                    if (d < 1) divs2 = 1;
                                    else
                                    {
                                        divs2 = 1 / d;
                                    }
                                }
                                
                            }

                            string spCd = vls[2].ToString();
                            fldNm = s + "_" + spCd;
                            //Console.WriteLine(fldNm);
                            int fldIndex = ftrCur.FindField(fldNm);
                            if (fldIndex == -1)
                            {
                                fldIndex = ftrCur.FindField(nFldNm.Substring(0, 10));
                            }
                            object vl = (System.Convert.ToDouble(vls[arrayIndex]))*divs2;
                            ftr.set_Value(fldIndex, vl);
                            
                        }		 
	                }
                    ftrCur.UpdateFeature(ftr);
                    ftr = ftrCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                //System.Environment.OSVersion.
                if (av == "0.0" || ev == "0.0")
                {
                    System.Windows.Forms.MessageBox.Show("You do not have OleDB 12.0 installed. Please download and install Office 2007 data provider from:\nhttp://www.microsoft.com/en-us/download/confirmation.aspx?id=23734", "No data provider found", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void addRegenFields()
        {
            foreach (string s in rUnSp )
            {
                
                string fldNm = "RTPA_" + s;
                if (SampleFeatureClass.FindField(fldNm) > -1)
                {
                }
                else
                {
                    geoDbUtil.createField(SampleFeatureClass, fldNm, esriFieldType.esriFieldTypeDouble);
                }
                
            }
        }
        Dictionary<string, int> rgDic = new Dictionary<string, int>();
        HashSet<string> rUnSp = new HashSet<string>();
        private void createAndFillRegenRef()
        {
            
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                //Console.WriteLine("opened the database");
                string sql = "SELECT PLT_CN, SUBP, SPCD, TREECOUNT FROM SEEDLING";
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[4];
                    oleRd.GetValues(vls);
                    string pltCn = vls[0].ToString();
                    string subp = vls[1].ToString();
                    if (SubPlotField == "" || SubPlotField == null) subp = "0";
                    string linkVl = pltCn + "_" + subp;
                    //Console.WriteLine("Link = " + linkVl.ToString());
                    if (unPlots.Contains(linkVl))
                    {
                        string spcd = vls[2].ToString();
                        string stcd = "1";
                        string grpCd = spcd;
                        if (groupstatuscode) grpCd = spcd + "_" + stcd;
                        if (grp) spcd = findGrp(grpCd);
                        rUnSp.Add(spcd);
                        int tCnt = System.Convert.ToInt32(vls[3]);
                        string lk = linkVl + "_" + spcd.ToString();
                        int vlOut;
                        if (rgDic.TryGetValue(lk, out vlOut))
                        {
                            rgDic[lk] = vlOut+tCnt;
                        }
                        else
                        {
                            rgDic.Add(lk,tCnt);
                        }
                    }
                }
                oleRd.Close();
                con.Close();
            }
        }
        private bool groupstatuscode = false;
        public bool GroupStatusCode { get { return groupstatuscode; } set { groupstatuscode = value; } }
        private void createAndFillTreesRef()
        {
            //DataTable dtTrees = null;
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                //Console.WriteLine("opened the database");
                string sql = "SELECT SPCD, JENKINS_TOTAL_B1, JENKINS_TOTAL_B2, JENKINS_STEM_WOOD_RATIO_B1, JENKINS_STEM_WOOD_RATIO_B2, JENKINS_STEM_BARK_RATIO_B1, JENKINS_STEM_BARK_RATIO_B2, JENKINS_FOLIAGE_RATIO_B1, JENKINS_FOLIAGE_RATIO_B2 FROM REF_SPECIES";
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[9];
                    oleRd.GetValues(vls);
                    bmDic.Add(vls[0].ToString(),vls);
                }
                oleRd.Close();
                sql = "SELECT CN, PLT_CN, SUBP, TREE, SPCD, DIA, HT, STATUSCD FROM TREE WHERE DIA >= 0 and SPCD > 0";
                oleCom.CommandText = sql;
                oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[8];
                    oleRd.GetValues(vls);
                    string pltCn = vls[1].ToString();
                    string subp = vls[2].ToString();
                    if(SubPlotField==""||SubPlotField==null)subp="0";
                    string linkVl = pltCn+"_"+subp;
                    //Console.WriteLine(linkVl);
                    if(unPlots.Contains(linkVl))
                    {
                        //Console.WriteLine(linkVl);
                        string spcd = vls[4].ToString();
                        //adding in live and dead
                        string stcd = vls[7].ToString();
                        string grpCd = spcd;
                        if (groupstatuscode) grpCd = spcd + "_" + stcd;
                        object[] bmVls;
                        if (bmDic.TryGetValue(spcd, out bmVls))
                        {
                            if (grp) spcd = findGrp(grpCd);//findGrp(spcd);
                            double diam = System.Convert.ToDouble(vls[5]);//diameter
                            Dictionary<string, object[]> cDic;
                            if (diam < 5)
                            {
                                //Console.WriteLine("Adding to less than 5");
                                cDic = sVlDic;
                                sUnSp.Add(spcd);
                            }
                            else
                            {
                                //Console.WriteLine("greater than 5");
                                cDic = vlDic;
                                unSp.Add(spcd);
                            }
                            
                            string lk = linkVl + "_" + spcd.ToString();
                            object[] treeVls;
                            if (cDic.TryGetValue(lk, out treeVls))
                            {

                            }
                            else
                            {
                                treeVls = new object[12];
                                treeVls[0] = pltCn;
                                treeVls[1] = subp;
                                treeVls[2] = spcd;
                                treeVls[3] = 0;//dbh
                                treeVls[4] = 0;//AGB
                                treeVls[5] = 0;//StemAGB
                                treeVls[6] = 0;//ButtAGB
                                treeVls[7] = 0;//FoliageAGB
                                treeVls[8] = 0;//TopAGB
                                treeVls[9] = 0;//BAA
                                treeVls[10] = 0;//TPA
                                treeVls[11] = 0;//MeanHT
                                cDic.Add(lk, treeVls);
                            }
                            //Console.WriteLine(System.Convert.ToDouble(vls[5]).ToString());
                            
                            double ht = System.Convert.ToDouble(vls[6]);//height
                            treeVls[3] = System.Convert.ToDouble(treeVls[3]) + diam;
                            double j_tbm = Math.Exp(System.Convert.ToDouble(bmVls[1]) + System.Convert.ToDouble(bmVls[2]) * Math.Log(diam * 2.54)) * 2.2046;//AGB
                            double j_sbm = j_tbm * Math.Exp(System.Convert.ToDouble(bmVls[3]) + System.Convert.ToDouble(bmVls[4]) / (2.54 * diam));//SAGB
                            double j_bbm = j_tbm * Math.Exp(System.Convert.ToDouble(bmVls[5]) + System.Convert.ToDouble(bmVls[6]) / (2.54 * diam));//BAGB
                            double j_fbm = j_tbm * Math.Exp(System.Convert.ToDouble(bmVls[7]) + System.Convert.ToDouble(bmVls[8]) / (2.54 * diam));//FAGB
                            double j_tpbm = j_tbm - j_sbm - j_bbm - j_fbm;//TPAGB
                            treeVls[4] = System.Convert.ToDouble(treeVls[4]) + j_tbm;
                            treeVls[5] = System.Convert.ToDouble(treeVls[5]) + j_sbm;
                            treeVls[6] = System.Convert.ToDouble(treeVls[6]) + j_bbm;
                            treeVls[7] = System.Convert.ToDouble(treeVls[7]) + j_fbm;
                            treeVls[8] = System.Convert.ToDouble(treeVls[8]) + j_tpbm;
                            treeVls[9] = System.Convert.ToDouble(treeVls[9]) + 0.005454 * Math.Pow(diam, 2);//BA
                            treeVls[10] = System.Convert.ToDouble(treeVls[10]) + 1.0;//trees
                            treeVls[11] = System.Convert.ToDouble(treeVls[11]) + ht;
                        }
                    }
                }
                oleRd.Close();
                con.Close();
            }
        }

        private string findGrp(string spcd)
        {
            string outVl;
            if(grpDic.TryGetValue(spcd,out outVl))
            {
                return outVl;
            }
            else
            {
                return spcd;
            }
        }


        private void createAndFillPlotsTable()
        {
            //DataTable dTblPlots = dSet.Tables.Add("Table", "tblPlots");
            //dTblPlots.Columns.Add("PLT_CN",System.Type.GetType("System.String"));
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = PlotCnField;
            if (SubPlotField != "" && SubPlotField != null) qf.SubFields = PlotCnField + "," + SubPlotField;
            IFeatureCursor fCur = SampleFeatureClass.Search(qf, true);
            int cnIndex = fCur.FindField(PlotCnField);
            int subIndex = fCur.FindField(SubPlotField);
            IFeature ftr = fCur.NextFeature();
            while (ftr != null)
            {
                string plCn = ftr.get_Value(cnIndex).ToString();
                string subPlt = "0";
                if (subIndex > -1)
                {
                    object subPltObj = ftr.get_Value(subIndex);
                    if (subPltObj != null)
                    {
                        subPlt = subPltObj.ToString();
                    }
                }
                string pltID = plCn+"_"+subPlt;
                //Console.WriteLine(pltID);
                unPlots.Add(pltID);
                ftr = fCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fCur);
        }
        private void addFields(string prefix = "")
        {
            HashSet<string> un;
            if (prefix == "")
            {
                un = unSp;
            }
            else
            {
                un = sUnSp;
            }
            foreach (string s in un)
            {
                //Console.WriteLine(s.ToString());
                foreach (biomassTypes t in fldArr)
                {
                    string fldNm = prefix+t.ToString() + "_" + s;
                    //Console.WriteLine(fldNm);
                    if (SampleFeatureClass.FindField(fldNm) > -1)
                    {
                    }
                    else
                    {
                        geoDbUtil.createField(SampleFeatureClass, fldNm, esriFieldType.esriFieldTypeDouble);
                    }
                }
            }
        }
    }
}
