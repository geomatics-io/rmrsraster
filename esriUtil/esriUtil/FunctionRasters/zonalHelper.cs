using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters
{
    public class zonalHelper
    {
        public zonalHelper()
        {
            rsUtil = new rasterUtil();
            tempWksStr = rsUtil.TempConvDir;
            rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rd.TopMost = true;
            rd.Show();
        }
        public zonalHelper(esriUtil.Forms.RunningProcess.frmRunningProcessDialog runningdialog)
        {
            rsUtil = new rasterUtil();
            tempWksStr = rsUtil.TempConvDir;
            rd = runningdialog;
        }
        public zonalHelper(rasterUtil rasterUtility,esriUtil.Forms.RunningProcess.frmRunningProcessDialog runningdialog)
        {
            rsUtil = rasterUtility;
            tempWksStr = rsUtil.TempConvDir;
            rd = runningdialog;
        }

        private esriUtil.Forms.RunningProcess.frmRunningProcessDialog rd = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private IRasterProps zProps = null;
        private IRasterProps vProps = null;
        public rasterUtil.zoneType[] ZoneTypes { get; set; }
        private IWorkspace wks = null;
        public IWorkspace OutWorkspace { get { return wks; } }
        private ITable oTbl = null;
        public ITable OutTable { get { return oTbl; } }
        private string linkfieldname = "Value";
        public string LinkFieldName { get { return linkfieldname; } }
        private IRaster zRs = null;
        private IRaster vRs = null;
        private string oName = "_VAT";
        public string OutTableName { get { return oName; } set { oName = value; } }
        public IRaster InZoneRaster 
        {
            get
            {
                return zRs;
            }
            set
            {
                zRs = value;
                zProps = (IRasterProps)zRs;
                IDataset ds = (IDataset)((IRaster2)zRs).RasterDataset;
                if(wks==null) wks = geoUtil.OpenWorkSpace(ds.Workspace.PathName);
                //zName = ds.BrowseName;
                //Console.WriteLine(tblName);
            }
        }
        public IRaster InValueRaster
        {
            get
            {
                return vRs;
            }
            set
            {
                
                vRs = value;
                vProps = (IRasterProps)vRs;
                zoneValueDicArr = new Dictionary<double, object[]>[((IRasterBandCollection)vRs).Count];
                for (int i = 0; i < zoneValueDicArr.Length; i++)
                {
                    zoneValueDicArr[i] = new Dictionary<double, object[]>();
                }
                IDataset ds = (IDataset)((IRaster2)vRs).RasterDataset;
                //vName = ds.BrowseName;
            }
        }
        public void setZoneValues()
        {
            tblName = geoUtil.getSafeOutputNameNonRaster(wks, oName + "_VAT");
            if (vRs == null)
            {
                if (rd != null) rd.addMessage("Value raster has not been set! Cannot proceed!");
                return;
            }
            if (zRs == null)
            {
                rd.addMessage("Feature class method");
                if (ftrCls == null||ftrField==null)
                {
                    if (rd != null) rd.addMessage("Zone Feature Class has not been set! Cannot proceed!");
                    return;
                }
                bool cP = checkProjectionsFtr();
                bool cE = checkExtentsFtr();
                
                if (!cE)
                {
                    if (rd != null) rd.addMessage("Zone and value dataset extents do not overlap! Cannot proceed!");
                    return;
                }
                if (!cP)
                {
                    if (rd != null) rd.addMessage("Zone and value dataset projections are different! Project on the fly to value raster projection!");
                    ftrCls = reprojectInFeatureClass(ftrCls, vProps.SpatialReference);
                }
                //need to re-project before clipping
                //vRs = rsUtil.clipRasterFunction(vRs, ((IGeoDataset)ftrCls).Extent, esriRasterClippingType.esriRasterClippingOutside);
                //vProps = (IRasterProps)vRs;
                calcZoneValuesFtr();
            }
            else
            {

                rd.addMessage("Raster method");
                bool cP = checkProjections();
                bool cC = checkCellSize();
                bool cE = checkExtents();
                if (!cE)
                {
                    if (rd != null) rd.addMessage("Zone and value dataset extents do not overlap! Cannot proceed!");
                    return;
                }
                if (!cP)
                {
                    if (rd != null) rd.addMessage("Zone and value datasets are not in the same projection! Projecting value raster!");
                }
                calcZoneValues();
            }
            if (ZoneTypes==null||ZoneTypes.Length>0)
            {
                fillFields();
            }
            if (zoneClassCount)
            {
                buildZoneClassCount();
            }

        }

        private void fillFields()
        {
            if (rd != null) rd.addMessage("Output table name = " + wks.PathName + "\\" + tblName);
            bool weCreate = true;
            if (!geoUtil.ftrExists(wks, tblName))
            {
                IFields nflds = new FieldsClass();
                IFieldsEdit nfldsE = (IFieldsEdit)nflds;
                IField nfld = new FieldClass();
                IFieldEdit nfldE = (IFieldEdit)nfld;
                nfldE.Name_2 = "Band";
                nfldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfldE);
                IField nfld2 = new FieldClass();
                IFieldEdit nfld2E = (IFieldEdit)nfld2;
                nfld2E.Name_2 = "Zone";
                nfld2E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld2E);
                IField nfld3 = new FieldClass();
                IFieldEdit nfld3E = (IFieldEdit)nfld3;
                nfld3E.Name_2 = "Count";
                nfld3E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld3E);
                oTbl = geoUtil.createTable(wks, tblName, nflds);
            }
            else
            {
                weCreate = false;
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                oTbl = ftrWks.OpenTable(tblName);
            }
            foreach (rasterUtil.zoneType zT in ZoneTypes)
            {
                string fldNm = zT.ToString();
                if (oTbl.FindField(fldNm) == -1)
                {
                    geoUtil.createField(oTbl, fldNm, esriFieldType.esriFieldTypeDouble);
                }
            }
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            //ITransactions trs = (ITransactions)wks;
            //trs.StartTransaction();
            try
            {
                int bdIndex = oTbl.FindField("Band");
                int vlIndex = oTbl.FindField("Zone");
                int cntIndex = oTbl.FindField("Count");
                int bndCnt = 1;
                foreach (Dictionary<double, object[]> zoneValueDicOut in zoneValueDicArr)
                {
                    foreach (KeyValuePair<double, object[]> kVp in zoneValueDicOut)
                    {

                        double key = kVp.Key;
                        object[] vl = kVp.Value;
                        Dictionary<rasterUtil.zoneType, double> vDic = getValueDic(vl);
                        IRow rw = null;
                        if (!weCreate)
                        {
                            string qry = "Band = " + bndCnt.ToString() + " and Zone = " + key;
                            IQueryFilter qf = new QueryFilterClass();
                            qf.WhereClause = qry;
                            ISelectionSet tblSelectionSet = oTbl.Select(qf, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionOnlyOne, wks);
                            if (tblSelectionSet.Count > 0)
                            {
                                int id = tblSelectionSet.IDs.Next();
                                rw = oTbl.GetRow(id);
                            }
                            else
                            {
                                rw = oTbl.CreateRow();
                            }

                        }
                        else
                        {
                            rw = oTbl.CreateRow();
                        }
                        //Console.WriteLine(key.ToString());
                        rw.set_Value(bdIndex, bndCnt);
                        rw.set_Value(vlIndex, key);
                        rw.set_Value(cntIndex, vl[0]);
                        foreach (rasterUtil.zoneType zT in ZoneTypes)
                        {
                            string fldNm = zT.ToString();
                            int fldIndex = oTbl.FindField(fldNm);
                            double zVl = vDic[zT];
                            //Console.WriteLine("\t"+fldNm+ ": " + zVl.ToString());
                            if (fldIndex > -1)
                            {
                                rw.set_Value(fldIndex, zVl);
                            }
                            else
                            {
                                Console.WriteLine(fldNm);
                            }
                        }
                        rw.Store();
                    }
                    bndCnt += 1;
                }
                //trs.CommitTransaction();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //trs.AbortTransaction();
            }
            finally
            {
            }
        }

        private void calcZoneValuesFtr()
        {
            //Console.WriteLine("made it to the feature calculations");
            bool makeDic = (ZoneClassCount || ZoneTypes.Contains(rasterUtil.zoneType.VARIETY) || ZoneTypes.Contains(rasterUtil.zoneType.ENTROPY) || ZoneTypes.Contains(rasterUtil.zoneType.ASM) || ZoneTypes.Contains(rasterUtil.zoneType.MINORITY) || ZoneTypes.Contains(rasterUtil.zoneType.MODE) || ZoneTypes.Contains(rasterUtil.zoneType.MEDIAN));
            //
            //HashSet<byte> hByt = new HashSet<byte>();
            //
            ISpatialReference sr = vProps.SpatialReference;
            IEnvelope vrsEnv = vProps.Extent;
            ISpatialFilter spFilt = new SpatialFilterClass();
            spFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            spFilt.Geometry = vrsEnv;
            spFilt.GeometryField = ftrCls.ShapeFieldName;
            IFeatureCursor fCur = ftrCls.Search(spFilt, false);
            int zoneIndex = fCur.FindField(InZoneField);
            IFeature ftr = fCur.NextFeature();
            while (ftr != null)
            {
                IGeometry geo = ftr.Shape;
                double z = System.Convert.ToDouble(ftr.get_Value(zoneIndex));
                IPolygon4 poly = (IPolygon4)geo;
                if (needToProject)
                {
                    poly.Project(sr);
                }
                IGeometryBag geoBag = poly.ExteriorRingBag;
                IGeometryCollection geoColl = (IGeometryCollection)geoBag;
                for (int g = 0; g < geoColl.GeometryCount; g++)
                {
                    IGeometry geo2 = geoColl.Geometry[g];
                    IRaster rs = rsUtil.clipRasterFunction(vRs, geo2, esriRasterClippingType.esriRasterClippingOutside);
                    IEnvelope rsEnv = ((IRasterProps)rs).Extent;
                    //Console.WriteLine((rsEnv.Width / 30).ToString() + ", " + (rsEnv.Height / 30).ToString());
                    IRasterCursor rsCur = ((IRaster2)rs).CreateCursorEx(null);
                    do
                    {
                        IPixelBlock pb = rsCur.PixelBlock;
                        for (int p = 0; p < pb.Planes; p++)
                        {
                            zoneValueDic = zoneValueDicArr[p];
                            object[] zoneValue;
                            double cnt = 0;
                            double maxVl = Double.MinValue;
                            double minVl = Double.MaxValue;
                            double s = 0;
                            double s2 = 0;
                            Dictionary<double, int> uDic = null;
                            if (zoneValueDic.TryGetValue(z, out zoneValue))
                            {
                                cnt = System.Convert.ToDouble(zoneValue[0]);
                                maxVl = System.Convert.ToDouble(zoneValue[1]);
                                minVl = System.Convert.ToDouble(zoneValue[2]);
                                s = System.Convert.ToDouble(zoneValue[3]);
                                s2 = System.Convert.ToDouble(zoneValue[4]);
                                uDic = (Dictionary<double, int>)zoneValue[5];
                            }
                            else
                            {
                                zoneValue = new object[6];
                                zoneValue[0] = cnt;
                                zoneValue[1] = maxVl;
                                zoneValue[2] = minVl;
                                zoneValue[3] = s;
                                zoneValue[4] = s2;
                                uDic = null;
                                if (makeDic)
                                {
                                    uDic = new Dictionary<double, int>();
                                }
                                zoneValue[5] = uDic;
                            }
                            for (int r = 0; r < pb.Height; r++)
                            {
                                for (int c = 0; c < pb.Width; c++)
                                {
                                    object vlo = pb.GetVal(p, c, r);
                                    if (vlo == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        double vl = System.Convert.ToDouble(vlo);
                                        cnt++;
                                        if (vl > maxVl) maxVl = vl;
                                        if (vl < minVl) minVl = vl;
                                        s += vl;
                                        s2 += vl * vl;
                                        if (makeDic)
                                        {
                                            int cntVl = 0;
                                            if (uDic.TryGetValue(vl, out cntVl))
                                            {
                                                uDic[vl] = cntVl += 1;
                                            }
                                            else
                                            {
                                                uDic.Add(vl, 1);
                                            }

                                        }
                                    }


                                }


                            }
                            zoneValue[0] = cnt;
                            zoneValue[1] = maxVl;
                            zoneValue[2] = minVl;
                            zoneValue[3] = s;
                            zoneValue[4] = s2;
                            zoneValue[5] = uDic;
                            zoneValueDic[z] = zoneValue;

                        }
                    } while (rsCur.Next());
                }
                ftr = fCur.NextFeature();
            }
            
        }

        public bool checkProjectionsFtr()
        {
            bool x = (((IGeoDataset)ftrCls).SpatialReference.FactoryCode == vProps.SpatialReference.FactoryCode);
            if (!x)
            {
                needToProject = true;
            }
            else
            {
                needToProject = false;
            }
            return x;
        }

        private bool checkExtentsFtr()
        {
            IEnvelope envFtr = ((IGeoDataset)ftrCls).Extent;
            if (needToProject)
            {
                envFtr.Project(vProps.SpatialReference);
            }
            IEnvelope envRst = vProps.Extent;
            IRelationalOperator rO = (IRelationalOperator)envRst;
            bool dis = rO.Disjoint(envFtr);
            if (dis) return false;
            else return true;
        }
        private string tblName = "";
        private void buildZoneClassCount()
        {
            string cTblName = geoUtil.getSafeOutputNameNonRaster(wks,tblName.Replace("_VAT","_CLASS"));
            if (rd != null) rd.addMessage("Output table name = " + wks.PathName + "\\" + cTblName);
            bool weCreate = true;
            if(!geoUtil.ftrExists(wks,cTblName))
            {
                IFields nflds = new FieldsClass();
                IFieldsEdit nfldsE = (IFieldsEdit)nflds;
                IField nfld = new FieldClass();
                IFieldEdit nfldE = (IFieldEdit)nfld;
                nfldE.Name_2 = "Band";
                nfldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfldE);
                IField nfld2 = new FieldClass();
                IFieldEdit nfld2E = (IFieldEdit)nfld2;
                nfld2E.Name_2 = "Zone";
                nfld2E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld2E);
                IField nfld4 = new FieldClass();
                IFieldEdit nfld4E = (IFieldEdit)nfld4;
                nfld4E.Name_2 = "Class";
                nfld4E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld4E);
                IField nfld3 = new FieldClass();
                IFieldEdit nfld3E = (IFieldEdit)nfld3;
                nfld3E.Name_2 = "Count";
                nfld3E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld3E);
                oTbl = geoUtil.createTable(wks,cTblName,nflds);
            }
            else
            {
                weCreate = false;
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                oTbl = ftrWks.OpenTable(tblName);
            }
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                wksE.StopEditing(true);
            }
            //ITransactions trs = (ITransactions)wks;
            //trs.StartTransaction();
            try
            {
                int bdIndex = oTbl.FindField("Band");
                int vlIndex = oTbl.FindField("Zone");
                int cntIndex = oTbl.FindField("Count");
                int clsIndex = oTbl.FindField("Class");
                int bndCnt = 1;
                foreach (Dictionary<double, object[]> zoneValueDicOut in zoneValueDicArr)
                {
                    foreach (KeyValuePair<double, object[]> kVp in zoneValueDicOut)
                    {

                        double key = kVp.Key;
                        object[] vl = kVp.Value;
                        Dictionary<double, int> uDic = (Dictionary<double, int>)vl[5];
                        IRow rw = null;
                        foreach(KeyValuePair<double,int> uKvp in uDic)
                        {
                            double uDicKey = uKvp.Key;
                            int uDicVl = uKvp.Value;
                            if (!weCreate)
                            {
                            
                                string qry = "Band = " + bndCnt.ToString() + " and Zone = " + key + " and Class = " + uDicKey;
                                IQueryFilter qf = new QueryFilterClass();
                                qf.WhereClause = qry;
                                ISelectionSet tblSelectionSet = oTbl.Select(qf, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionOnlyOne, wks);
                                if (tblSelectionSet.Count > 0)
                                {
                                    int id = tblSelectionSet.IDs.Next();
                                    rw = oTbl.GetRow(id);
                                }
                                else
                                {
                                    rw = oTbl.CreateRow();
                                }
                            

                            }
                            else
                            {
                                rw = oTbl.CreateRow();
                            }

                            //Console.WriteLine(key.ToString());
                            rw.set_Value(bdIndex, bndCnt);
                            rw.set_Value(vlIndex, key);
                            rw.set_Value(cntIndex, uDicVl);
                            rw.set_Value(clsIndex, uDicKey);
                            rw.Store();
                        }
                    }
                    bndCnt += 1;
                }
                //trs.CommitTransaction();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                //trs.AbortTransaction();
            }
            finally
            {
            }

        }

        private Dictionary<rasterUtil.zoneType, double> getValueDic(object[] zoneValues)
        {
            double cellCnt = System.Convert.ToDouble(zoneValues[0]);
            double min = System.Convert.ToDouble(zoneValues[2]);
            double max = System.Convert.ToDouble(zoneValues[1]);
            double s = System.Convert.ToDouble(zoneValues[3]);
            double s2 = System.Convert.ToDouble(zoneValues[4]);
            Dictionary<double,int> uDic = (Dictionary<double,int>)zoneValues[5];
            double uniq = 0;
            double ent = 0;
            double asm = 0;
            double median = 0;
            double mode = 0;
            double minority = 0;
            if (uDic != null)
            {
                int halfCellCnt = System.Convert.ToInt32(cellCnt / 2);
                List<double> sKeyLst = uDic.Keys.ToList();
                sKeyLst.Sort();
                int vlCntMax = 0;
                int vlCntMin = Int32.MaxValue;
                int rCellCnt = 0;
                bool lMed = true;
                foreach (double key in sKeyLst)
                {
                    uniq += 1;
                    int vlCnt = uDic[key];
                    //Console.WriteLine(key.ToString()+":"+vlCnt);
                    rCellCnt += vlCnt;
                    double p = System.Convert.ToDouble(vlCnt) / cellCnt;
                    if (vlCnt > vlCntMax)
                    {
                        mode = key;
                        vlCntMax = vlCnt;
                    }
                    if (vlCnt < vlCntMin)
                    {
                        minority = key;
                        vlCntMin = vlCnt;
                    }
                    if (lMed && rCellCnt >= halfCellCnt)
                    {
                        median = key;
                        lMed = false;
                    }
                    ent += (p * Math.Log(p));
                    asm += p * p;
                } 
                ent = ent * -1; 
            }
            double range = max - min;
            double var = (s2 - ((s * s) / cellCnt)) / cellCnt;
            double std = Math.Sqrt(var);
            Dictionary<rasterUtil.zoneType, double> vDic = new Dictionary<rasterUtil.zoneType, double>();
            
            foreach (rasterUtil.zoneType t in ZoneTypes)
            {
                
                switch (t)
                {
                    case rasterUtil.zoneType.MIN:
                        vDic.Add(t, min);
                        break;
                    case rasterUtil.zoneType.SUM:
                        vDic.Add(t, s);
                        break;
                    case rasterUtil.zoneType.MEAN:
                        vDic.Add(t, s / cellCnt);
                        break;
                    case rasterUtil.zoneType.VAR:
                        vDic.Add(t,var);
                        break;
                    case rasterUtil.zoneType.STD:
                        vDic.Add(t, std);
                        break;
                    case rasterUtil.zoneType.VARIETY:
                        vDic.Add(t, uniq);
                        break;
                    case rasterUtil.zoneType.ENTROPY:
                        vDic.Add(t, ent);
                        break;
                    case rasterUtil.zoneType.ASM:
                        vDic.Add(t, asm);
                        break;
                    case rasterUtil.zoneType.RANGE:
                        vDic.Add(t, range);
                        break;
                    case rasterUtil.zoneType.MEDIAN:
                        vDic.Add(t, median);
                        break;
                    case rasterUtil.zoneType.MODE:
                        vDic.Add(t, mode);
                        break;
                    case rasterUtil.zoneType.MINORITY:
                        vDic.Add(t, minority);
                        break;
                    default:
                        vDic.Add(t, max);
                        break;
                }

            }
            return vDic;
        }
        private Dictionary<double, object[]>[] zoneValueDicArr = null;
        private Dictionary<double, object[]> zoneValueDic = null;//value = [count,max,min,sum,sum2,dictionary<int,int>]->dictionary is for unique, entropy, and ASM
        private void calcZoneValues()
        {
            bool makeDic = (ZoneClassCount||ZoneTypes.Contains(rasterUtil.zoneType.VARIETY)||ZoneTypes.Contains(rasterUtil.zoneType.ENTROPY)||ZoneTypes.Contains(rasterUtil.zoneType.ASM)||ZoneTypes.Contains(rasterUtil.zoneType.MINORITY)||ZoneTypes.Contains(rasterUtil.zoneType.MODE)||ZoneTypes.Contains(rasterUtil.zoneType.MEDIAN));
            double zNoDataVl = System.Convert.ToDouble(((System.Array)zProps.NoDataValue).GetValue(0));
            IPnt zMeanCellSize = zProps.MeanCellSize();
            IPnt vMeanCellSize = vProps.MeanCellSize();
            int intersectWidthCells = System.Convert.ToInt32(intEnv.Width / zMeanCellSize.X);
            int intersectHeightCells = System.Convert.ToInt32(intEnv.Height / zMeanCellSize.Y);
            IPoint tl = intEnv.UpperLeft;
            int bH = 512;
            int bW = 512;
            int wCellsMax = intersectWidthCells;
            int hCellsMax = intersectHeightCells;
            IPnt zPntLoc = new PntClass();
            IPnt vPntLoc = new PntClass();
            IPnt zPntSize = new PntClass();
            IRaster2 zr = (IRaster2)InZoneRaster;
            IRaster2 vr = (IRaster2)InValueRaster;
            int zclm, zrw, vclm, vrw;
            zr.MapToPixel(tl.X+(zMeanCellSize.X/2), tl.Y-(zMeanCellSize.Y/2), out zclm, out zrw);
            vr.MapToPixel(tl.X + (zMeanCellSize.X / 2), tl.Y - (zMeanCellSize.Y / 2), out vclm, out vrw);
            int ozclm = zclm;
            int ozrw = zrw;
            int ovclm = vclm;
            int ovrw = vrw;
            zPntLoc.SetCoords(zclm, zrw);
            vPntLoc.SetCoords(vclm, vrw);
            for (int brw = 0; brw < hCellsMax; brw += bH)
            {
                int rH = hCellsMax-brw;//Height of block
                if (rH > bH) rH = bH;
                for (int bclm = 0; bclm < wCellsMax; bclm += bW)
                {
                    int cW = wCellsMax - bclm;//Width of block
                    if (cW > bW) cW = bW;
                    zPntSize.SetCoords(cW, rH);
                    IPixelBlock zPb = InZoneRaster.CreatePixelBlock(zPntSize);
                    IPixelBlock vPb = InValueRaster.CreatePixelBlock(zPntSize);
                    InZoneRaster.Read(zPntLoc, zPb);
                    InValueRaster.Read(vPntLoc, vPb);
                    //System.Array zPix = (System.Array)zPb.get_SafeArray(0);
                    for (int i = 0; i < vPb.Planes; i++)
                    {
                        zoneValueDic = zoneValueDicArr[i];
                        //double vNoDataVl = System.Convert.ToDouble(((System.Array)vProps.NoDataValue).GetValue(i));
                        //System.Array vPix = (System.Array)vPb.get_SafeArray(i);
                        for (int r = 0; r < rH; r++)
                        {
                            for (int c = 0; c < cW; c++)
                            {
                                object zObj = zPb.GetVal(0, c, r);
                                if (zObj==null)
                                {
                                    continue;
                                }
                                else
                                {
                                    double z = System.Convert.ToDouble(zObj);
                                    object vObj = vPb.GetVal(i, c, r);
                                    
                                    if (vObj==null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        double v = System.Convert.ToDouble(vObj);
                                        object[] zoneValue;
                                        if (zoneValueDic.TryGetValue(z, out zoneValue))
                                        {
                                            double cnt = System.Convert.ToDouble(zoneValue[0]);
                                            zoneValue[0] = cnt += 1;
                                            double maxVl = System.Convert.ToDouble(zoneValue[1]);
                                            if (v > maxVl)
                                            {
                                                maxVl = v;
                                                zoneValue[1] = maxVl;
                                            }
                                            double minVl = System.Convert.ToDouble(zoneValue[2]);
                                            if (v < minVl)
                                            {
                                                minVl = v;
                                                zoneValue[2] = minVl;
                                            }
                                            double s = System.Convert.ToDouble(zoneValue[3]);
                                            zoneValue[3] = s + v;
                                            double s2 = System.Convert.ToDouble(zoneValue[4]);
                                            zoneValue[4] = s2 + v * v;
                                            if (makeDic)
                                            {
                                                Dictionary<double, int> uDic = (Dictionary<double, int>)zoneValue[5];
                                                int cntVl = 0;
                                                if (uDic.TryGetValue(v, out cntVl))
                                                {
                                                    uDic[v] = cntVl += 1;
                                                }
                                                else
                                                {
                                                    uDic.Add(v, 1);
                                                }
                                                zoneValue[5] = uDic;
                                            }
                                            zoneValueDic[z] = zoneValue;
                                        }
                                        else
                                        {
                                            zoneValue = new object[6];
                                            zoneValue[0] = 1d;
                                            zoneValue[1] = v;
                                            zoneValue[2] = v;
                                            zoneValue[3] = v;
                                            zoneValue[4] = v * v;
                                            if (makeDic)
                                            {
                                                Dictionary<double, int> uDic = new Dictionary<double, int>();
                                                uDic.Add(v, 1);
                                                zoneValue[5] = uDic;
                                            }
                                            zoneValueDic.Add(z, zoneValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    //do this at the end
                    zclm = zclm + cW;
                    vclm = vclm + cW;
                    zPntLoc.SetCoords(zclm, zrw);
                    vPntLoc.SetCoords(vclm, vrw);
                }
                zrw = zrw + rH;
                vrw = vrw + rH;
                //reset PixelBlock columns
                zclm = ozclm;
                vclm = ovclm;
                zPntLoc.SetCoords(zclm, zrw);
                vPntLoc.SetCoords(vclm, vrw);
            }

        }
        IEnvelope intEnv = null;
        public bool checkExtents()
        {
            IEnvelope zenv = zProps.Extent;
            IEnvelope venv = vProps.Extent;
            IRelationalOperator rsOp = (IRelationalOperator)zenv;
            if (rsOp.Disjoint(venv))
            {
                return false;
            }
            else
            {
                if (!rsOp.Equals(venv))
                {
                    zenv.Intersect(venv);
                }
                intEnv = zenv;
                return true;
            }

        }
        private bool needToProject = false;
        private bool checkProjections()
        {
            bool x = (zProps.SpatialReference.FactoryCode == vProps.SpatialReference.FactoryCode);
            if (!x)
            {
                InZoneRaster = rsUtil.reprojectRasterFunction(InZoneRaster, vProps.SpatialReference);
            }
            return x;
        }
        
        public bool checkCellSize()
        {
            bool cS = true;
            IPnt zPnt = zProps.MeanCellSize();
            IPnt vPnt = vProps.MeanCellSize();
            double zX = zPnt.X;
            double zY = zPnt.Y;
            double vX = vPnt.X;
            double vY = vPnt.Y;
            double ns = vX;
            if (Math.Abs(vX - vY)>0.0001)
            {
                
                if(vY<vX) ns = vY;
                InValueRaster = rsUtil.reSampleRasterFunction(vRs, ns, rstResamplingTypes.RSP_NearestNeighbor);
                if (rd != null) rd.addMessage("Value dataset cells Width and Height are not the same! Resizing the valueRaster cell width and height to " + ns.ToString()+"!");
            }
            if (Math.Abs(zX-ns)>0.0001)
            {
                InZoneRaster = rsUtil.reSampleRasterFunction(zRs, ns,rstResamplingTypes.RSP_NearestNeighbor);
                if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size! Resizing the zoneRaster cell width and height to match the value raster ("+vProps.MeanCellSize().X.ToString()+":"+zProps.MeanCellSize().X.ToString()+")!");
            }
            return cS;
        }
        string tempWksStr = "";
        private IFeatureClass ftrCls = null;
        public IFeatureClass InZoneFeatureClass 
        { 
            get 
            { 
                return ftrCls;
            }
            set
            { 
                ftrCls = value;
                IDataset ds = (IDataset)ftrCls;
                wks = ds.Workspace;
                
            } 
        }
        string ftrField = null;
        private bool zoneClassCount = false;
        public bool ZoneClassCount { get { return zoneClassCount; } set { zoneClassCount = value; } }
        public string InZoneField { get { return ftrField; } set { ftrField = value; } }
        public void convertFeatureToRaster(IFeatureClass InFeatureClass, string fldName)
        {
            
            ftrCls = InFeatureClass;
            ftrField = fldName;
            IDataset dSet = (IDataset)InFeatureClass;
            string outRsNm = dSet.BrowseName;
            wks = dSet.Workspace;
            if (vRs != null)
            {
                if (!checkProjectionsFtr())
                {
                    if (rd != null) rd.addMessage("Re-projecting feature class to match value raster's projection");
                    InFeatureClass = reprojectInFeatureClass(InFeatureClass, vProps.SpatialReference);
                    
                }
            }
            IWorkspace wksTemp = geoUtil.OpenRasterWorkspace(tempWksStr);
            IRaster rs = rsUtil.convertFeatureClassToRaster(InFeatureClass, rasterUtil.rasterType.IMAGINE, wksTemp, outRsNm, vProps.MeanCellSize().X, ((IRaster2)vRs).RasterDataset);
            rs = rsUtil.returnRaster(rs, rstPixelType.PT_FLOAT);
            int fieldIndex = InFeatureClass.FindField(fldName);
            if(fieldIndex == -1)
            {
                fieldIndex = InFeatureClass.FindField(fldName + "_1");
            }
            if (fldName.ToLower() == InFeatureClass.OIDFieldName.ToLower()||fieldIndex == -1)
            {
                zRs = rs;
            }
            else
            {
                IRemapFilter rFilt = new RemapFilterClass();
                IFeatureCursor ftrCur = InFeatureClass.Search(null, false);
                IFeature ftr = ftrCur.NextFeature();
                
                while (ftr != null)
                {
                    double id = ftr.OID;
                    double nVl = System.Convert.ToDouble(ftr.get_Value(fieldIndex));
                    if (Double.IsNaN(nVl) || Double.IsInfinity(nVl))
                    {
                        ftr = ftrCur.NextFeature();
                    }
                    else
                    {
                        //Console.WriteLine("adding oid = " + id.ToString() + " and value = " + nVl.ToString());
                        rFilt.AddClass(id, id + 1, nVl);
                        ftr = ftrCur.NextFeature();
                    }
                }
                zRs = rsUtil.calcRemapFunction(rs, rFilt);
            }
            zProps = (IRasterProps)zRs;
        }

        public IFeatureClass reprojectInFeatureClass(IFeatureClass InFeatureClass, ISpatialReference SpatialReference)
        {
            IWorkspace tempWorkspace = geoUtil.OpenWorkSpace(tempWksStr);
            string outNm = ((IDataset)InFeatureClass).BrowseName+"_PR";
            outNm = geoUtil.getSafeOutputNameNonRaster(wks,outNm);
            IFields outFlds = new FieldsClass();
            IFieldsEdit outFldsE = (IFieldsEdit)outFlds;
            IField inFld = InFeatureClass.Fields.get_Field(InFeatureClass.FindField(ftrField));
            IField outFld = new FieldClass();
            if (inFld.Type == esriFieldType.esriFieldTypeOID)
            {
                IFieldEdit outFldE = (IFieldEdit)outFld;
                outFldE.Type_2 = esriFieldType.esriFieldTypeInteger;
                outFldE.Name_2 = inFld.Name;
            }
            else
            {
                IClone cl = (IClone)inFld;
                outFld = (IField)cl.Clone();
            }
            outFldsE.AddField(outFld);
            IFeatureClass outFtrCls = geoUtil.createFeatureClass((IWorkspace2)tempWorkspace,outNm,outFldsE,InFeatureClass.ShapeType,SpatialReference);
            string ozName = ftrField;
            int ozIndex = outFtrCls.FindField(ozName);            
            if (ozIndex == -1)
            {
                ozName = ftrField+"_1";
                ozIndex = outFtrCls.FindField(ozName);
                
            }
            int izIndex = InFeatureClass.FindField(ftrField);
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = InFeatureClass.ShapeFieldName + "," + ftrField;
            IFeatureCursor fCur = InFeatureClass.Search(qf, false);
            IFeature ftr = fCur.NextFeature();
            IWorkspaceEdit wksE = (IWorkspaceEdit)tempWorkspace;
            bool weStart = true;
            if(wksE.IsBeingEdited())
            {
                weStart=false;
            }
            else
            {
                wksE.StartEditing(false);
            }
            wksE.StartEditOperation();
            try
            {
                while (ftr != null)
                {
                    object vl = ftr.get_Value(izIndex);
                    IFeatureProject ftrP = (IFeatureProject)ftr;
                    ftrP.Project(SpatialReference);
                    IFeature oFtr = outFtrCls.CreateFeature();
                    oFtr.Shape = ftr.Shape;
                    if(ozIndex>-1) oFtr.set_Value(ozIndex, vl);
                    oFtr.Store();
                    ftr = fCur.NextFeature();
                }
                ftrField = ozName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                wksE.StopEditOperation();
                if (weStart) wksE.StopEditing(true);
            }
            return outFtrCls;

        }


        public static void transformData(ITable zoneTable, string linkFieldName, ITable zonalSummaryTable)
        {
            IObjectClassInfo2 oi2 = (IObjectClassInfo2)zoneTable;
            if (!oi2.CanBypassEditSession())
            {
                System.Windows.Forms.MessageBox.Show("Table has a composite relationship. Please export data to a simple object and try again.");
                return;
            }
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            IFields zsFlds = zonalSummaryTable.Fields;
            IFields zFlds = zoneTable.Fields;
            foreach (string s in new string[] { "Band", "Zone", "Count" })
            {
                if (zsFlds.FindField(s) == -1)
                {
                    System.Windows.Forms.MessageBox.Show("Not a valid Zonal Summary table!!!", "Format", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }
            if (zFlds.FindField(linkFieldName) == -1)
            {
                System.Windows.Forms.MessageBox.Show("Not a valid Zone table!!!", "Format", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            IDataStatistics dStat = new DataStatisticsClass();
            dStat.Cursor = zonalSummaryTable.Search(null, false);
            dStat.Field = "Band";
            int unqCnt = 0;
            System.Collections.IEnumerator en = dStat.UniqueValues;
            en.MoveNext();
            do
            {
                //Console.WriteLine(en.Current.ToString());
                unqCnt++;
            } while (en.MoveNext());
            int exRwCnt = zoneTable.RowCount(null) * unqCnt;
            int sumRwCnt = zonalSummaryTable.RowCount(null);
            //Console.WriteLine("zonal*bands = " + exRwCnt.ToString() + "zoneSumCnt = " + sumRwCnt.ToString());
            if (exRwCnt != sumRwCnt)
            {
                
                System.Windows.Forms.MessageBox.Show("Zone and Zonal Summary tables row counts do not match! You must update your zonal statistics before running this tool!", "Format", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
                
            }
            List<string> newFldNames = new List<string>();
            List<string> zsFldNames = new List<string>();
            for (int i = 0; i < zsFlds.FieldCount; i++)
            {
                IField fld = zsFlds.get_Field(i);
                if (fld.Type == esriFieldType.esriFieldTypeDouble)
                {
                    string nm = fld.Name;
                    if (nm.ToLower() != "zone" && nm.ToLower() != "band")
                    {
                        zsFldNames.Add(nm);
                        for (int j = 0; j < unqCnt; j++)
                        {
                            string nnm = nm + "_" + (j + 1).ToString();
                            newFldNames.Add(geoUtil.createField(zoneTable, nnm, esriFieldType.esriFieldTypeDouble,false));
                        }
                    }

                }
            }
            int[] zsFldNamesIndex = new int[zsFldNames.Count];
            for (int i = 0; i < zsFldNames.Count; i++)
            {
                string vl = zsFldNames[i];
                zsFldNamesIndex[i] = zonalSummaryTable.FindField(vl);
            }
            int[] newFldNamesIndex = new int[newFldNames.Count];
            for (int i = 0; i < newFldNames.Count; i++)
            {
                string vl = newFldNames[i];
                newFldNamesIndex[i] = zoneTable.FindField(vl);
            }            
            //IQueryFilter qfz = new QueryFilterClass();
            //IQueryFilterDefinition qfzD = (IQueryFilterDefinition)qfz;
            //qfzD.PostfixClause = "ORDER BY " + linkFieldName;
            //IQueryFilter qfzs = new QueryFilterClass();
            //IQueryFilterDefinition qfzsD = (IQueryFilterDefinition)qfzs;
            //qfzsD.PostfixClause = "ORDER BY Zone, Band";
            //ICursor curZ = zoneTable.Update(qfz, false);
            //ICursor curZs = zonalSummaryTable.Search(qfzs, false);
            ITableSort tblSortZ = new TableSortClass();
            tblSortZ.Table = zoneTable;
            tblSortZ.Fields = linkFieldName;
            ITableSort tblSortZs = new TableSortClass();
            tblSortZs.Table = zonalSummaryTable;
            tblSortZs.Fields = "Zone, Band";
            tblSortZs.Sort(null);
            tblSortZ.Sort(null);
            ICursor curZ = tblSortZ.Rows;
            ICursor curZs = tblSortZs.Rows;
            IRow rwZ = curZ.NextRow();
            while (rwZ != null)
            {
                for (int i = 0; i < unqCnt; i++)
                {
                    IRow rwZs = curZs.NextRow();
                    for (int j = 0; j < zsFldNames.Count; j++)
                    {
                        string zsN = zsFldNames[j];  
                        int zsNIndex = zsFldNamesIndex[j];
                        double zsVl = System.Convert.ToDouble(rwZs.get_Value(zsNIndex));
                        string newZName = zsN + "_" + (i + 1).ToString();
                        int newZNameIndex = newFldNamesIndex[newFldNames.IndexOf(newZName)];
                        rwZ.set_Value(newZNameIndex, zsVl);
                    }
                }
                rwZ.Store();
                //curZ.UpdateRow(rwZ);
                rwZ = curZ.NextRow();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(curZ);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(curZs);
        }
    }
}
