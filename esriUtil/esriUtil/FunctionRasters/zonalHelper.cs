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
                wks = geoUtil.OpenWorkSpace(ds.Workspace.PathName);
                tblName = geoUtil.getSafeOutputNameNonRaster(wks,ds.BrowseName + "_VAT");
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
            }
        }
        public void setZoneValues()
        {
            if (vRs == null)
            {
                if (rd != null) rd.addMessage("Value raster has not been set! Cannot proceed!");
                return;
            }
            if (zRs == null)
            {
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
                }
                vRs = rsUtil.clipRasterFunction(vRs, ((IGeoDataset)ftrCls).Extent, esriRasterClippingType.esriRasterClippingOutside);
                vProps = (IRasterProps)vRs;
                calcZoneValuesFtr();
            }
            else
            {
                bool cP = checkProjections();
                bool cE = checkExtents();
                if (!cE)
                {
                    if (rd != null) rd.addMessage("Zone and value dataset extents do not overlap! Cannot proceed!");
                    return;
                }
                bool cS = checkSnapped();
                if (!cP)
                {
                    if (rd != null) rd.addMessage("Zone and value datasets are not in the same projection! Projecting value raster!");
                }
                if (!cS)
                {
                    if (rd != null) rd.addMessage("Zone and value datasets are not snapped! Using the closest pixel!");
                }
                bool cC = checkCellSize();
                if (!cC)
                {
                    return;
                }
                calcZoneValues();
            }
            fillFields();
        }

        private void calcZoneValuesFtr()
        {
            bool makeDic = (ZoneTypes.Contains(rasterUtil.zoneType.VARIETY) || ZoneTypes.Contains(rasterUtil.zoneType.ENTROPY) || ZoneTypes.Contains(rasterUtil.zoneType.ASM) || ZoneTypes.Contains(rasterUtil.zoneType.MINORITY) || ZoneTypes.Contains(rasterUtil.zoneType.MODE) || ZoneTypes.Contains(rasterUtil.zoneType.MEDIAN));
            IPnt vPntLoc = new PntClass();
            IPnt vPntSize = new PntClass();
            IPnt meanCellSize = vProps.MeanCellSize();
            double cSizeX = meanCellSize.X;
            double cSizeY = meanCellSize.Y;
            int zoneIndex = ftrCls.FindField(InZoneField);
            IFeatureCursor ftrCur = ftrCls.Search(null, false);
            IFeature ftr = ftrCur.NextFeature();
            while (ftr != null)
            {
                int z = System.Convert.ToInt32(ftr.get_Value(zoneIndex));
                IGeometry geo = ftr.ShapeCopy;
                if (needToProject) geo.Project(vProps.SpatialReference);
                IRaster cutRs = rsUtil.clipRasterFunction(vRs, geo, esriRasterClippingType.esriRasterClippingOutside);
                IRasterProps cProps = (IRasterProps)cutRs;
                double cNoDataVl = System.Convert.ToDouble(((System.Array)cProps.NoDataValue).GetValue(0));
                vPntSize.SetCoords(cProps.Width,cProps.Height);
                vPntLoc.SetCoords(0,0);
                IPixelBlock cPb = cutRs.CreatePixelBlock(vPntSize);
                cutRs.Read(vPntLoc, cPb);
                System.Array vPix = (System.Array)cPb.get_SafeArray(0);
                for (int r = 0; r < cProps.Height; r++)
                {
                    for (int c = 0; c < cProps.Width; c++)
                    {
                        double v = System.Convert.ToDouble(vPix.GetValue(c, r));
                        if (v == cNoDataVl)
                        {
                            continue;
                        }
                        else
                        {
                            int vi = System.Convert.ToInt32(v);
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
                                    Dictionary<int, int> uDic = (Dictionary<int, int>)zoneValue[5];
                                    int cntVl = 0;
                                    if (uDic.TryGetValue(vi, out cntVl))
                                    {
                                        uDic[vi] = cntVl += 1;
                                    }
                                    else
                                    {
                                        uDic.Add(vi, 1);
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
                                    Dictionary<int, int> uDic = new Dictionary<int, int>();
                                    uDic.Add(vi, 1);
                                    zoneValue[5] = uDic;
                                }
                                zoneValueDic.Add(z, zoneValue);
                            }
                        }
                    }
                }
                ftr = ftrCur.NextFeature();
            }
        }

        private bool checkProjectionsFtr()
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
        private void fillFields()
        {
            if (rd != null) rd.addMessage("Output table name = " + wks.PathName + "\\" + tblName);
            bool weCreate = true;
            if(!geoUtil.ftrExists(wks,tblName))
            {
                IFields nflds = new FieldsClass();
                IFieldsEdit nfldsE = (IFieldsEdit)nflds;
                IField nfld = new FieldClass();
                IFieldEdit nfldE = (IFieldEdit)nfld;
                nfldE.Name_2 = "Value";
                nfldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld);
                IField nfld2 = new FieldClass();
                IFieldEdit nfld2E = (IFieldEdit)nfld2;
                nfld2E.Name_2 = "Count";
                nfld2E.Type_2 = esriFieldType.esriFieldTypeDouble;
                nfldsE.AddField(nfld2E);
                oTbl = geoUtil.createTable(wks,tblName,nflds);
            }
            else
            {
                weCreate = false;
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                oTbl = ftrWks.OpenTable(tblName);
            }
            foreach(rasterUtil.zoneType zT in ZoneTypes)
            {
                string fldNm = zT.ToString();
                if (oTbl.FindField(fldNm) == -1)
                {
                    geoUtil.createField(oTbl, fldNm, esriFieldType.esriFieldTypeDouble);
                }
            }
            bool weStartEdit = true;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            if (wksE.IsBeingEdited())
            {
                weStartEdit = false;
            }
            else
            {
                wksE.StartEditing(false);
            }
            wksE.StartEditOperation();
            try
            {
                int vlIndex = oTbl.FindField("Value");
                int cntIndex = oTbl.FindField("Count");
                foreach (KeyValuePair<int, object[]> kVp in zoneValueDic)
                {
                    
                    int key = kVp.Key;
                    object[] vl = kVp.Value;
                    Dictionary<rasterUtil.zoneType, double> vDic = getValueDic(vl);
                    IRow rw = null;
                    if (!weCreate)
                    {
                        string qry = "Value = " + key;
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
                    rw.set_Value(vlIndex, key);
                    rw.set_Value(cntIndex, vl[0]);
                    foreach (rasterUtil.zoneType zT in ZoneTypes)
                    {
                        string fldNm = zT.ToString();
                        int fldIndex = oTbl.FindField(fldNm);
                        double zVl = vDic[zT];
                        //Console.WriteLine("\t"+fldNm+ ": " + zVl.ToString());
                        rw.set_Value(fldIndex, zVl);
                    }
                    rw.Store();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                wksE.StopEditOperation();
                if (weStartEdit)
                {
                    wksE.StopEditing(true);
                }
            }

        }

        private Dictionary<rasterUtil.zoneType, double> getValueDic(object[] zoneValues)
        {
            double cellCnt = System.Convert.ToDouble(zoneValues[0]);
            double min = System.Convert.ToDouble(zoneValues[2]);
            double max = System.Convert.ToDouble(zoneValues[1]);
            double s = System.Convert.ToDouble(zoneValues[3]);
            double s2 = System.Convert.ToDouble(zoneValues[4]);
            Dictionary<int,int> uDic = (Dictionary<int,int>)zoneValues[5];
            double uniq = 0;
            double ent = 0;
            double asm = 0;
            double median = 0;
            double mode = 0;
            double minority = 0;
            if (uDic != null)
            {
                int halfCellCnt = System.Convert.ToInt32(cellCnt / 2);
                List<int> sKeyLst = uDic.Keys.ToList();
                sKeyLst.Sort();
                int vlCntMax = 0;
                int vlCntMin = Int32.MaxValue;
                int rCellCnt = 0;
                bool lMed = true;
                foreach (int key in sKeyLst)
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
                    case rasterUtil.zoneType.VARIANCE:
                        vDic.Add(t,var);
                        break;
                    case rasterUtil.zoneType.STANDARD_DEVIATION:
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
        private Dictionary<int, object[]> zoneValueDic = new Dictionary<int, object[]>();//value = [count,max,min,sum,sum2,dictionary<int,int>]->dictionary is for unique, entropy, and ASM
        private void calcZoneValues()
        {
            bool makeDic = (ZoneTypes.Contains(rasterUtil.zoneType.VARIETY)||ZoneTypes.Contains(rasterUtil.zoneType.ENTROPY)||ZoneTypes.Contains(rasterUtil.zoneType.ASM)||ZoneTypes.Contains(rasterUtil.zoneType.MINORITY)||ZoneTypes.Contains(rasterUtil.zoneType.MODE)||ZoneTypes.Contains(rasterUtil.zoneType.MEDIAN));
            double zNoDataVl = System.Convert.ToDouble(((System.Array)zProps.NoDataValue).GetValue(0));
            double vNoDataVl = System.Convert.ToDouble(((System.Array)vProps.NoDataValue).GetValue(0));
            IPnt vPntLoc = new PntClass();
            IPnt zPntSize = new PntClass();
            IPnt vPntSize = new PntClass();
            IRaster2 zr = (IRaster2)InZoneRaster;
            IRaster2 vr = (IRaster2)InValueRaster;
            zPntSize.SetCoords(512, 512);
            IRasterCursor zCur = zr.CreateCursorEx(zPntSize);
            do
            {
                IPixelBlock zPb = zCur.PixelBlock;
                IPnt zPnt = zCur.TopLeft;
                double mX, mY;
                zr.PixelToMap(System.Convert.ToInt32(zPnt.X),System.Convert.ToInt32(zPnt.Y),out mX,out mY);
                if (needToProject)
                {
                    IPoint newPnt = new PointClass();
                    newPnt.PutCoords(mX, mY);
                    newPnt.SpatialReference = zProps.SpatialReference;
                    newPnt.Project(vProps.SpatialReference);
                    mX = newPnt.X;
                    mY = newPnt.Y;
                }
                int x,y;
                vr.MapToPixel(mX,mY,out x,out y);
                vPntLoc.SetCoords(x,y);
                int wd = zPb.Width;
                int ht = zPb.Height;
                vPntSize.SetCoords(wd,ht);
                IPixelBlock vPb = InValueRaster.CreatePixelBlock(vPntSize);
                InValueRaster.Read(vPntLoc, vPb);
                System.Array zPix = (System.Array)zPb.get_SafeArray(0);
                System.Array vPix = (System.Array)vPb.get_SafeArray(0);
                for (int r = 0; r < ht; r++)
                {
                    for (int c = 0; c < wd; c++)
                    {
                        int z = System.Convert.ToInt32(zPix.GetValue(c, r));
                        if (z == zNoDataVl)
                        {
                            continue;
                        }
                        else
                        {
                            double v = System.Convert.ToDouble(vPix.GetValue(c, r));
                            if (v == vNoDataVl)
                            {
                                continue;
                            }
                            else
                            {
                                //Console.WriteLine(z.ToString());
                                int vi = System.Convert.ToInt32(v);
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
                                        Dictionary<int, int> uDic = (Dictionary<int, int>)zoneValue[5];
                                        int cntVl = 0;
                                        if (uDic.TryGetValue(vi, out cntVl))
                                        {
                                            uDic[vi] = cntVl += 1;
                                        }
                                        else
                                        {
                                            uDic.Add(vi, 1);
                                        }
                                        zoneValue[5] = uDic;
                                    }
                                    zoneValueDic[z] = zoneValue;
                                }
                                else
                                {
                                    zoneValue = new object[6];
                                    zoneValue[0]= 1d;
                                    zoneValue[1]= v;
                                    zoneValue[2]= v;
                                    zoneValue[3]= v;
                                    zoneValue[4] = v * v;
                                    if (makeDic)
                                    {
                                        Dictionary<int, int> uDic = new Dictionary<int, int>();
                                        uDic.Add(vi, 1);
                                        zoneValue[5] = uDic;
                                    }
                                    zoneValueDic.Add(z, zoneValue);
                                }
                                
                            }
                        }
                    }
                }


            }
            while (zCur.Next()==true);
            

        }
        public bool checkExtents()
        {
            IEnvelope zenv = zProps.Extent;
            IEnvelope venv = vProps.Extent;
            if (needToProject)
            {
                venv.Project(zProps.SpatialReference);
            }
            IRelationalOperator rsOp = (IRelationalOperator)zenv;
            if (rsOp.Disjoint(venv))
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private bool needToProject = false;
        public bool checkProjections()
        {
            bool x = (zProps.SpatialReference.FactoryCode == vProps.SpatialReference.FactoryCode);
            if (!x) needToProject = true;
            else needToProject = false;
            return x;
        }
        public bool checkSnapped()
        {
            bool sn = true;
            IEnvelope zenv = zProps.Extent;
            IEnvelope venv = vProps.Extent;
            IPoint zPoint = zenv.UpperLeft;
            IPoint vPoint = venv.UpperLeft;
            double bX = vPoint.X;
            double bY = vPoint.Y;
            double sX = zPoint.X;
            double sY = zPoint.Y;
            double xR = 0;
            double yR = 0;
            

            if (bX < sX)
            {
                xR = (sX - bX);
                
            }
            else
            {
                xR = (bX - sX);

            }
            if (bY < sY)
            {
                yR = (sY - bY);
            }
            else
            {
                yR = (bY - sY);
            }
            xR = xR % vProps.MeanCellSize().X;
            yR = yR % vProps.MeanCellSize().Y;
            if (xR != 0)
            {
                sn = false;
            }
            if (yR != 0 )
            {
                sn = false;
            }
            return sn;
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
            double divX = 0;
            double divY = 0;
            if (zX != vX)
            {
                if (zX > vX)
                {
                    divX = zX % vX;
                }
                else
                {
                    divX = vX % zX;
                }
                if (divX == 0)
                {
                    if (zX > vX)
                    {
                        zProps.Width = System.Convert.ToInt32(zProps.Width * (zX / vX));
                        if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size! Resizing the zoneRaster cell width to match the value raster ("+zProps.MeanCellSize().X.ToString()+")!");
                    }
                    else
                    {
                        vProps.Width = System.Convert.ToInt32(vProps.Width * (vX / zX));
                        if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size! Resizing the valueRaster cell width to match the zone raster(" + vProps.MeanCellSize().X.ToString() + ")!");
                    }
                }
                else
                {
                    if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size (width) and are not a multiple of each other! Please resample your zone raster to match your value raster!");
                    cS = false;
                    return cS;
                }
                
            }

            if (zY != vY)
            {
                if (zY > vY)
                {
                    divY = zY % vY;
                }
                else
                {
                    divY = vY % zY;
                }
                if (divY == 0)
                {
                    if (zY > vY)
                    {
                        zProps.Height = System.Convert.ToInt32(zProps.Height * (zY / vY));
                        if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size! Resizing the zoneRaster cell height to match the value raster(" + zProps.MeanCellSize().Y.ToString() + ")!");
                        
                    }
                    else
                    {
                        vProps.Height = System.Convert.ToInt32(vProps.Height * (vY / zY));
                        if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size! Resizing the valueRaster cell height to match the zone raster(" + vProps.MeanCellSize().Y.ToString() + ")!");
                        
                    }
                }
                else
                {
                    if (rd != null) rd.addMessage("Zone and value dataset cells are not the same size (height) and are not a multiple of each other! Please resample your zone raster to match your value raster!");
                    cS = false;
                    return cS;
                }
                
                
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
                tblName = geoUtil.getSafeOutputNameNonRaster(wks, ds.BrowseName + "_VAT");
            } 
        }
        string ftrField = null;
        public string InZoneField { get { return ftrField; } set { ftrField = value; } }
        public void convertFeatureToRaster(IFeatureClass InFeatureClass, string fldName)
        {
            
            ftrCls = InFeatureClass;
            ftrField = fldName;
            IDataset dSet = (IDataset)InFeatureClass;
            string outRsNm = dSet.BrowseName;
            wks = dSet.Workspace;
            tblName = geoUtil.getSafeOutputNameNonRaster(wks,outRsNm + "_VAT");
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
            if (fldName.ToLower() == InFeatureClass.OIDFieldName.ToLower())
            {
                zRs = rs;
            }
            else
            {
                IRemapFilter rFilt = new RemapFilterClass();
                IFeatureCursor ftrCur = InFeatureClass.Search(null, false);
                IFeature ftr = ftrCur.NextFeature();
                int fieldIndex = InFeatureClass.FindField(fldName);
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
                        rFilt.AddClass(id, id + 1, nVl);
                        ftr = ftrCur.NextFeature();
                    }
                }
                zRs = rsUtil.calcRemapFunction(rs, rFilt);
            }
            zProps = (IRasterProps)zRs;
        }

        private IFeatureClass reprojectInFeatureClass(IFeatureClass InFeatureClass, ISpatialReference SpatialReference)
        {
            string outNm = ((IDataset)InFeatureClass).BrowseName+"_PR";
            outNm = geoUtil.getSafeOutputNameNonRaster(wks,outNm);
            IFields outFlds = new FieldsClass();
            IFieldsEdit outFldsE = (IFieldsEdit)outFlds;
            IField inFld = InFeatureClass.Fields.get_Field(InFeatureClass.FindField(ftrField)); 
            IClone cl = (IClone)inFld;
            IField outFld = (IField)cl.Clone();
            outFldsE.AddField(outFld);
            IFeatureClass outFtrCls = geoUtil.createFeatureClass((IWorkspace2)wks,outNm,outFlds,InFeatureClass.ShapeType,SpatialReference);
            int ozIndex = outFtrCls.FindField(ftrField);
            int izIndex = InFeatureClass.FindField(ftrField);
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = InFeatureClass.ShapeFieldName + "," + ftrField;
            IFeatureCursor fCur = InFeatureClass.Search(qf, false);
            IFeature ftr = fCur.NextFeature();
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
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
                    oFtr.set_Value(ozIndex, vl);
                    oFtr.Store();
                    ftr = fCur.NextFeature();
                }
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

    }
}
