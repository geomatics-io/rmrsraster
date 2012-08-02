using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace esriUtil
{
    class batchCalculations
    {
        public batchCalculations()
        {
            rsUtil = new rasterUtil();
        }
        public batchCalculations(rasterUtil rasterUtility, esriUtil.Forms.RunningProcess.frmRunningProcessDialog runningDialog)
        {
            rsUtil = rasterUtility;
            if(rp!=null)rp = runningDialog;
        }
        public enum batchGroups { ARITHMETIC, MATH, SETNULL, LOGICAL, CLIP, CONDITIONAL, CONVOLUTION, FOCAL, LOCALSTATISTICS, LINEARTRANSFORM, RESCALE, REMAP, COMPOSITE, EXTRACTBAND, GLCM, LANDSCAPE, ZONALSTATS, SAVEFUNCTIONRASTER, BUILDRASTERSTATS, BUILDRASTERVAT, MOSAIC, MERGE, SAMPLERASTER, CLUSTERSAMPLERASTER, CREATERANDOMSAMPLE, CREATESTRATIFIEDRANDOMSAMPLE };
        private rasterUtil rsUtil = null;
        private System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        private System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string path = "";
        public string BatchPath 
        {
            get 
            { 
                return path; 
            } 
            set 
            { 
                path = value; 
            } 
        }
        private esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new Forms.RunningProcess.frmRunningProcessDialog(false);
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, ITable> tblDic = new Dictionary<string, ITable>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } set { rstDic = value; } }
        public Dictionary<string, IFeatureClass> FeatureClassDictionary { get { return ftrDic; } set { ftrDic = value; } }
        public Dictionary<string, ITable> TableDictionary { get { return tblDic; } set { tblDic = value; } }
        private List<string> lnLst = new List<string>();
        public void manuallyAddBatchLines(string[] lines)
        {
            lnLst.Clear();
            foreach (string s in lines)
            {
                lnLst.Add(s);
            }
        }
        public void saveBatchFile(string[] lines)
        {
            sfd.AddExtension = true;
            sfd.Filter = "Batch File|*.bch";
            sfd.DefaultExt = "bch";
            System.Windows.Forms.DialogResult dr = sfd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                string outFileName = sfd.FileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outFileName,false))
                {
                    foreach (string s in lines)
                    {
                        if (s.Length > 0)
                        {
                            sw.WriteLine(s);
                        }
                    }
                    sw.Close();
                }
                path = outFileName;
                loadBatchFile();
            }

        }
        public string openBatchFile()
        {
            string ostr = "";
            ofd.AddExtension = true;
            ofd.DefaultExt = "bch";
            ofd.Multiselect = false;
            ofd.Title = "Open Batch File";
            ofd.Filter = "Batch File|*.bch";
            System.Windows.Forms.DialogResult ds = ofd.ShowDialog();
            if (ds == System.Windows.Forms.DialogResult.OK)
            {
                path = ofd.FileName;
                ostr = loadBatchFile();
            }
            return ostr;
        }
        public string loadBatchFile()
        {
            //Console.WriteLine(path);
            StringBuilder sb = new StringBuilder();
            lnLst.Clear();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                string ln = sr.ReadLine();
                while(ln!=null)
                {
                    Console.WriteLine(ln);
                    if (ln.Length > 0)
                    {
                        sb.AppendLine(ln);
                        lnLst.Add(ln);
                    }
                    ln = sr.ReadLine();
                }
                sr.Close();
            }
            return sb.ToString();
        }
        public void runBatch()
        {
            rp.addMessage("Running in batch mode...");
            DateTime dt = DateTime.Now;
            try
            {
                string func = "";
                string param = "";
                string outName = "";
                foreach (string ln in lnLst)
                {
                    rp.stepPGBar(5);
                    rp.Refresh();
                    if (ln.Length > 0)
                    {
                        getFunctionAndParameters(ln, out func, out param, out outName);
                        if (func == "" || param == "" || outName == "")
                        {
                            continue;
                        }
                        else
                        {
                            try
                            {
                                rp.addMessage("Running process: " + ln);
                                rp.Refresh();
                                batchGroups bg = (batchGroups)Enum.Parse(typeof(batchGroups), func);
                                string[] paramArr = param.Split(new char[] { ';' });
                                switch (bg)
                                {
                                    case batchGroups.ARITHMETIC:
                                        rstDic[outName] = createArithmeticFunction(paramArr);
                                        break;
                                    case batchGroups.MATH:
                                        rstDic[outName] = createMathFunction(paramArr);
                                        break;
                                    case batchGroups.SETNULL:
                                        rstDic[outName] = createSetNullFunction(paramArr);
                                        break;
                                    case batchGroups.LOGICAL:
                                        rstDic[outName] = createLogicalFunction(paramArr);
                                        break;
                                    case batchGroups.CLIP:
                                        rstDic[outName] = createClipFunction(paramArr);
                                        break;
                                    case batchGroups.CONDITIONAL:
                                        rstDic[outName] = createConditionalFunction(paramArr);
                                        break;
                                    case batchGroups.CONVOLUTION:
                                        rstDic[outName] = createConvolutionFunction(paramArr);
                                        break;
                                    case batchGroups.FOCAL:
                                        rstDic[outName] = createFocalFunction(paramArr);//pickback up here
                                        break;
                                    case batchGroups.LOCALSTATISTICS:
                                        rstDic[outName] = createLocalFunction(paramArr);
                                        break;
                                    case batchGroups.LINEARTRANSFORM:
                                        rstDic[outName] = createLinearTransformFunction(paramArr);
                                        break;
                                    case batchGroups.RESCALE:
                                        rstDic[outName] = createRescaleFunction(paramArr);
                                        break;
                                    case batchGroups.REMAP:
                                        rstDic[outName] = createRemapFunction(paramArr);
                                        break;
                                    case batchGroups.COMPOSITE:
                                        rstDic[outName] = createCompositeFunction(paramArr);
                                        break;
                                    case batchGroups.EXTRACTBAND:
                                        rstDic[outName] = createExtractFunction(paramArr);
                                        break;
                                    case batchGroups.GLCM:
                                        rstDic[outName] = createGLCMFunction(paramArr);
                                        break;
                                    case batchGroups.LANDSCAPE:
                                        rstDic[outName] = createLandscapeFunction(paramArr);
                                        break;
                                    case batchGroups.ZONALSTATS:
                                        tblDic[outName] = createZonalStats(paramArr);
                                        break;
                                    case batchGroups.SAVEFUNCTIONRASTER:
                                        rstDic[outName] = saveRaster(paramArr);
                                        break;
                                    case batchGroups.BUILDRASTERSTATS:
                                        rstDic[outName] = buildRasterStats(paramArr);
                                        break;
                                    case batchGroups.BUILDRASTERVAT:
                                        tblDic[outName] = buildRasterVat(paramArr);
                                        break;
                                    case batchGroups.MOSAIC:
                                        rstDic[outName] = createMosaic(paramArr);
                                        break;
                                    case batchGroups.MERGE:
                                        rstDic[outName] = createMerge(paramArr);
                                        break;
                                    case batchGroups.SAMPLERASTER:
                                        break;
                                    case batchGroups.CREATERANDOMSAMPLE:
                                        break;
                                    case batchGroups.CREATESTRATIFIEDRANDOMSAMPLE:
                                        break;
                                    case batchGroups.CLUSTERSAMPLERASTER:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                rp.addMessage(e.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string tsStr = ts.Days.ToString() + " days, " + ts.Minutes.ToString() + " minutes, and " + ts.Seconds.ToString() + " seconds";
                rp.stepPGBar(100);
                rp.addMessage("Fished Batch Process in " + tsStr);
                rp.enableClose();
            }
        }

        private IRaster createMerge(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createMosaic(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private ITable buildRasterVat(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            return rsUtil.buildVat(rs);
        }

        private IRaster buildRasterStats(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            int skipFactor = System.Convert.ToInt32(paramArr[1]);
            return rsUtil.calcStatsAndHist(rs, skipFactor);
            
        }

        private IRaster saveRaster(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            string outName = paramArr[1];
            IWorkspace wks = geoUtil.OpenRasterWorkspace(paramArr[2]);
            rasterUtil.rasterType rastertype = (rasterUtil.rasterType)Enum.Parse(typeof(rasterUtil.rasterType),paramArr[3].ToUpper());
            return rsUtil.returnRaster(rsUtil.saveRasterToDataset(rs, outName, wks, rastertype));
        }

        private IRaster createLandscapeFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createGLCMFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createExtractFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createCompositeFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createRemapFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createRescaleFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createLinearTransformFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createLocalFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createFocalFunction(string[] paramArr)
        {
            throw new NotImplementedException();
        }

        private IRaster createConvolutionFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            int wd = System.Convert.ToInt32(paramArr[1]);
            int ht = System.Convert.ToInt32(paramArr[2]);
            string krn = paramArr[3];
            List<double> dblLst = new List<double>();
            foreach (string s in krn.Split(new char[] { ',' }))
            {
                dblLst.Add(System.Convert.ToDouble(s));
            }
            return rsUtil.convolutionRasterFunction(rs, wd, ht, dblLst.ToArray());
        }

        private IRaster createConditionalFunction(string[] paramArr)
        {
            IRaster conRs = getRaster(paramArr[0]);
            IRaster tRs = getRaster(paramArr[1]);
            IRaster fRs = getRaster(paramArr[2]);
            return rsUtil.conditionalRasterFunction(conRs, tRs, fRs);
        }

        private IRaster createClipFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            IFeatureClass ftrCls = getFeatureClass(paramArr[1]);
            IGeometry geo = ((IGeoDataset)ftrCls).Extent;
            return rsUtil.clipRasterFunction(rs, geo, esriRasterClippingType.esriRasterClippingOutside);
        }

        private IRaster createLogicalFunction(string[] paramArr)
        {
            rasterUtil.logicalType opType = (rasterUtil.logicalType)Enum.Parse(typeof(rasterUtil.logicalType),paramArr[2].ToUpper());
            string p1 = paramArr[0];
            string p2 = paramArr[1];
            IRaster rs = null;
            switch (opType)
            {
                case rasterUtil.logicalType.GT:
                    rs =  rsUtil.calcGreaterFunction(p1, p2);
                    break;
                case rasterUtil.logicalType.LT:
                    rs = rsUtil.calcLessFunction(p1, p2);
                    break;
                case rasterUtil.logicalType.GE:
                    rs = rsUtil.calcGreaterEqualFunction(p1, p2);
                    break;
                case rasterUtil.logicalType.LE:
                    rs = rsUtil.calcLessEqualFunction(p1, p2);
                    break;
                case rasterUtil.logicalType.EQ:
                    rs = rsUtil.calcEqualFunction(p1, p2);
                    break;
                case rasterUtil.logicalType.AND:
                    rs = rsUtil.calcAndFunction(p1, p2);
                    break;
                default:
                    rs = rsUtil.calcOrFunction(p1, p2);
                    break;
            }
            return rs;
        }

        private IRaster createSetNullFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            List<double[]> minMaxLst = new List<double[]>();
            string vlStr = paramArr[1];
            string[] vlArr = vlStr.Split(new char[] { ',' });
            foreach (string s in vlArr)
            {
                string[] rngArr = s.Split(new char[] { '-' });
                double[] mm = { System.Convert.ToDouble(rngArr[0]), System.Convert.ToDouble(rngArr[1]) };
                minMaxLst.Add(mm);
            }
            return rsUtil.setValueRangeToNodata(rs,minMaxLst);
        }

        private IRaster createMathFunction(string[] paramArr)
        {
            IRaster rs = getRaster(paramArr[0]);
            rasterUtil.transType tType = (rasterUtil.transType)Enum.Parse(typeof(rasterUtil.transType), paramArr[1]);
            return rsUtil.calcMathRasterFunction(rs, tType);
            
        }
        private ITable createZonalStats(string[] paramArr)
        {
            string zTStr = paramArr[paramArr.Length-1];
            string[] zTStrArr = zTStr.Split(new char[]{','});
            List<rasterUtil.zoneType> zLst = new List<rasterUtil.zoneType>();
            foreach(string s in zTStrArr)
            {
                rasterUtil.zoneType zT = (rasterUtil.zoneType)Enum.Parse(typeof(rasterUtil.zoneType),s.ToUpper());
                zLst.Add(zT);
            }
            if(paramArr.Length<4)
            {
                IRaster rs1 = getRaster(paramArr[0]);
                IRaster rs2 = getRaster(paramArr[1]);
                return rsUtil.zonalStats(rs1, rs2, zLst.ToArray(),rp);
            }
            else
            {
                IFeatureClass inFtrCls = getFeatureClass(paramArr[0]);
                string inFtrFld = paramArr[1];
                IRaster vRs = getRaster(paramArr[2]);
                return rsUtil.zonalStats(inFtrCls, inFtrFld, vRs, zLst.ToArray(),rp);
            }
        }

        private IFeatureClass getFeatureClass(string p)
        {
            string tp = p.Trim();
            if (ftrDic.ContainsKey(tp))
            {
                return ftrDic[tp];
            }
            else
            {
                IFeatureClass outFtrCls = geoUtil.getFeatureClass(tp);
                return outFtrCls;
            }
        }

        private IRaster createArithmeticFunction(string[] paramArr)
        {
            IRaster inRs1 = getRaster(paramArr[0]);
            IRaster inRs2 = getRaster(paramArr[1]);
            esriRasterArithmeticOperation op = (esriRasterArithmeticOperation)Enum.Parse(typeof(esriRasterArithmeticOperation), paramArr[2]);
            return rsUtil.calcArithmaticFunction(inRs1, inRs2, op);
        }

        private IRaster getRaster(string p)
        {
            string tp = p.Trim();
            if (rstDic.ContainsKey(tp))
            {
                return rstDic[tp];
            }
            else
            {
                IRaster outRs = rsUtil.returnRaster(tp);
                return outRs;
            }
        }

        public void getFunctionAndParameters(string ln, out string func, out string param, out string outName)
        {
            func = "";
            param = "";
            outName = "";
            try
            {
                string[] lnArr = ln.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                outName = lnArr[0].Trim();
                string lnS = lnArr[1].Trim();
                int lP = lnS.IndexOf("(");
                int rP = lnS.IndexOf(")");
                func = lnS.Substring(0, lP).ToUpper();
                param = lnS.Substring(lP+1, lnS.Length - (lP+2));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void syntaxExample(batchCalculations.batchGroups batchFunction)
        {
            string msg = "working on this";
            switch (batchFunction)
            {
                case batchGroups.ARITHMETIC:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;arithmeticFunction)";
                    break;
                case batchGroups.MATH:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;mathFunction)";
                    break;
                case batchGroups.SETNULL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;0-4,8-9,13-15)";
                    break;
                case batchGroups.LOGICAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;logicalOperator)";
                    break;
                case batchGroups.CLIP:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;inFeatureClass;logicalOperator)";
                    break;
                case batchGroups.CONDITIONAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;in_Raster2;in_Raster3)";
                    break;
                case batchGroups.CONVOLUTION:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;0,1,0,1,0,1,0,1,0)";
                    break;
                case batchGroups.FOCAL:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster1;width;height;focalStat)\noutRS = " + batchFunction.ToString() + "(in_Raster1;radius;focalStat)";
                    break;
                case batchGroups.LOCALSTATISTICS:
                    break;
                case batchGroups.LINEARTRANSFORM:
                    break;
                case batchGroups.RESCALE:
                    break;
                case batchGroups.REMAP:
                    break;
                case batchGroups.COMPOSITE:
                    break;
                case batchGroups.EXTRACTBAND:
                    break;
                case batchGroups.GLCM:
                    break;
                case batchGroups.LANDSCAPE:
                    break;
                case batchGroups.ZONALSTATS:
                    msg = "outTbl = " + batchFunction.ToString() + "(ZoneRaster;ValueRaster;MAX,MIN,SUM)\noutTbl = " + batchFunction.ToString() + "(ZoneFeatureClass;ZoneField;ValueRaster2;MAX,MIN,SUM)";
                    break;
                case batchGroups.SAVEFUNCTIONRASTER:
                    msg = "outRs = " + batchFunction.ToString() + "(inRaster;outName;outWorkspace;rasterType)";
                    break;
                case batchGroups.BUILDRASTERSTATS:
                    msg = "outRS = " + batchFunction.ToString() + "(in_Raster;skipFactor)";
                    break;
                case batchGroups.BUILDRASTERVAT:
                    msg = "outTable = " + batchFunction.ToString() + "(in_Raster)";
                    break;
                case batchGroups.MOSAIC:
                    break;
                case batchGroups.MERGE:
                    break;
                case batchGroups.SAMPLERASTER:
                    break;
                case batchGroups.CLUSTERSAMPLERASTER:
                    break;
                case batchGroups.CREATERANDOMSAMPLE:
                    break;
                case batchGroups.CREATESTRATIFIEDRANDOMSAMPLE:
                    break;
                default:
                    break;
            }
            System.Windows.Forms.MessageBox.Show(msg);
        }
    }
}
