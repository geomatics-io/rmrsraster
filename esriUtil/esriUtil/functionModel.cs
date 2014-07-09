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
    public class functionModel
    {
        public functionModel()
        {
            rsUtil = new rasterUtil();
        }
        public functionModel(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        ~functionModel()
        {
        }
        public enum dem { NorthSouth, EastWest, Slope, Aspect }
        public enum functionGroups { Arithmetic, Math, SetNull, Logical, Clip, Conditional, Convolution, Focal, LocalStatistics, LinearTransform, Rescale, Remap, Composite, ExtractBand, GLCM, Landscape, FocalSample, Aggregation };
        private rasterUtil rsUtil = null;
        private System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string path = "";
        public string FunctionDatasetPath { get { return path; } set { path = value; } }
        public static Dictionary<string, string> fdsDic = new Dictionary<string, string>();
        public static Dictionary<string, string> FunctionDatasetPathDic { get { return fdsDic; } }
        public static Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        //private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public static Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        public void createFunctionRaster(out string nm, out IRaster rs, out string desc)
        {
            nm = null;
            rs = null;
            desc = null;
            if (path == "" || !System.IO.File.Exists(path))
            {
                return;
            }
            try
            {
                string otNm = "";
                IRaster otRs = null;
                string ln = "";
                using(System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                    while ((ln = sr.ReadLine()) != null)
                    {
                        if (ln.Length > 0)
                        {
                            parseFunctionModel(ln, out otNm, out otRs);
                            if (rstDic.ContainsKey(otNm))
                            {
                                rstDic[otNm] = otRs;
                            }
                            else
                            {
                                rstDic.Add(otNm, otRs);
                            }
                            desc = ln;
                        }

                    }
                    sr.Close();
                }
                rs = updateStats(otRs);
                nm = otNm;
                fdsDic[otNm] = path;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
            }
            
        }

        public void addFunctionRasterToMap(IMap map)
        {
            try
            {
                ofd.Title = "Add Function Model to map";
                ofd.Filter = "Function Dataset|*.fds|Batch Datasets|*.bch";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FunctionDatasetPath = ofd.FileName;
                    int fIndex = ofd.FilterIndex;
                    string outName = "";
                    object outobj = null;
                    string outDesc = "";
                    if (fIndex == 1)
                    {
                        IRaster outRs;
                        createFunctionRaster(out outName, out outRs, out outDesc);
                        outobj = outRs;
                    }
                    else
                    {
                        createBatchRaster(out outName, out outobj, out outDesc);
                    }
                    if (outobj is IFunctionRasterDataset)
                    {
                        IRasterLayer rsLyr = new RasterLayerClass();
                        rsLyr.CreateFromDataset((IRasterDataset)outobj);
                        rsLyr.Name = outName;
                        rsLyr.Visible = false;
                        map.AddLayer((ILayer)rsLyr);
                    }
                    else if (outobj is IRaster)
                    {
                        IRasterLayer rsLyr = new RasterLayerClass();
                        rsLyr.CreateFromRaster((IRaster)outobj);
                        rsLyr.Name = outName;
                        rsLyr.Visible = false;
                        map.AddLayer((ILayer)rsLyr);
                    }
                    else if (outobj is FeatureClass)
                    {
                        IFeatureLayer ftrLyr = new FeatureLayerClass();
                        ftrLyr.Name = outName;
                        ftrLyr.Visible = false;
                        ftrLyr.FeatureClass = (IFeatureClass)outobj;
                        map.AddLayer((ILayer)ftrLyr);
                    }
                    else if (outobj is Table)
                    {
                        IStandaloneTableCollection stCol = (IStandaloneTableCollection)map;
                        IStandaloneTable stTbl = new StandaloneTableClass();
                        stTbl.Table = (ITable)outobj;
                        stTbl.Name = outName;
                        stCol.AddStandaloneTable(stTbl);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("There was a problem parsing the model. Please check your model file");
                    }

                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Could not add output to the map:" + e.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            

        }

        private void createBatchRaster(out string nm, out object rs, out string desc)
        {
            nm = null;
            rs = null;
            desc = null;
            if (path == "" || !System.IO.File.Exists(path))
            {
                return;
            }
            try
            {
                batchCalculations btch = new batchCalculations(rsUtil, new Forms.RunningProcess.frmRunningProcessDialog(true));
                btch.BatchPath = path;
                btch.loadBatchFile();
                btch.runBatch();
                btch.getFinalObject(out nm, out rs, out desc);
            }
            catch
            {
            }
            finally
            {
            }
        }

        public void deleteFunctionModel()
        {
            ofd.Title = "Delete Function Model";
            ofd.Filter = "Function Dataset|*.fds";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] paths = ofd.FileNames;
                foreach (string s in paths)
                {
                    try
                    {
                        System.IO.File.Delete(s);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void parseFunctionModel(string ln, out string otNm, out IRaster otRs)
        {
            otNm = "";
            otRs = null;
            string func = ln.Substring(0, ln.IndexOf('('));
            string prm = ln.Substring(ln.IndexOf('('));
            prm = prm.Substring(0, prm.IndexOf(')'));
            string[] prmArr = prm.Split(new char[] { ';' });
            switch ((functionGroups)Enum.Parse(typeof(functionGroups),func))
            {
                case functionGroups.Arithmetic:
                    calcArithmetic(prmArr,out otNm, out otRs);
                    break;
                case functionGroups.Logical:
                    calcLogical(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Clip:
                    calcClip(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Conditional:
                    calcConditional(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Convolution:
                    calcConvolution(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Focal:
                    calcFocal(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.LocalStatistics:
                    calcSummarize(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.LinearTransform:
                    calcLinearTransform(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Rescale:
                    calcRescale(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Remap:
                    calcRemap(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Composite:
                    calcComposite(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.ExtractBand:
                    calcExtractBand(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.GLCM:
                    calcGLCM(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Math:
                    calcMath(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Landscape:
                    calcLandscapeMetrics(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.SetNull:
                    calcSetNull(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.FocalSample:
                    calcFocalSample(prmArr, out otNm, out otRs);
                    break;
                case functionGroups.Aggregation:
                    calcAggregation(prmArr, out otNm, out otRs);
                    break;
                default:
                    break;
            }
            

        }

        private void calcAggregation(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[1].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[3].Split(new char[] { '@' })[1];
            int clm = System.Convert.ToInt32(prmArr[2].Split(new char[] { '@' })[1]);
            string fTyp = prmArr[0].Split(new char[] { '@' })[1];
            object rs1 = null;
            raster = null;
            if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            rasterUtil.focalType fcType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), fTyp);
            
            raster = rsUtil.createRaster(rsUtil.calcAggregationFunction(rs1,clm,fcType));
        }

        private void calcFocalSample(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[2].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[3].Split(new char[] { '@' })[1];
            string ranges = prmArr[0].Split(new char[] { '@' })[1];
            rasterUtil.focalType statType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), prmArr[1].Split(new char[] { '@' })[1]);
            HashSet<string> rngLst = new HashSet<string>();
            foreach (string s in ranges.Split(new char[] { ',' }))
            {
                string vl = s.Replace("`", ";");
                //Console.WriteLine(vl);
                rngLst.Add(vl);
            }
            object rs1 = null;
            if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.createRaster(rsUtil.calcFocalSampleFunction(rs1,rngLst,statType));
        }

        private void calcSetNull(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            string ranges = prmArr[1].Split(new char[]{'@'})[1];
            IStringArray strArr = new StrArrayClass();
            foreach(string s in ranges.Split(new char[]{','}))
            {
                string[] vlArr = s.Split(new char[]{'`'});
                int min = System.Convert.ToInt32(vlArr[0]);
                int max = System.Convert.ToInt32(vlArr[1]);
                List<string> ndVlsLst = new List<string>();
                for (int i = min; i < max; i++)
                {
                    ndVlsLst.Add(i.ToString());
                }
                strArr.Add(String.Join(" ",ndVlsLst.ToArray()));
            }
            object rs1 = null;
            if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.returnRaster(rsUtil.setValueRangeToNodata(rs1, strArr));
        }

        private void calcLandscapeMetrics(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[2].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[6].Split(new char[] { '@' })[1];
            int clm = System.Convert.ToInt32(prmArr[4].Split(new char[] { '@' })[1]);
            int rws = System.Convert.ToInt32(prmArr[5].Split(new char[] { '@' })[1]);
            string wTyp = prmArr[3].Split(new char[] { '@' })[1];
            string fTyp = prmArr[1].Split(new char[] { '@' })[1];
            string lTyp = prmArr[0].Split(new char[] { '@' })[1];
            object rs1 = null;
            raster = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            rasterUtil.focalType fcType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), fTyp);
            rasterUtil.windowType wdType = (rasterUtil.windowType)Enum.Parse(typeof(rasterUtil.windowType), wTyp);
            rasterUtil.landscapeType lsType = (rasterUtil.landscapeType)Enum.Parse(typeof(rasterUtil.landscapeType), lTyp);
            if (wdType == rasterUtil.windowType.CIRCLE)
            {
                raster = rsUtil.createRaster(rsUtil.calcLandscapeFunction(rs1, clm, fcType,lsType));
            }
            else
            {
                raster = rsUtil.createRaster(rsUtil.calcLandscapeFunction(rs1, clm, rws, fcType,lsType));
            }
        }

        private void calcMath(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[1].Split(new char[] { '@' })[1];
            string tTypeStr = prmArr[2].Split(new char[] { '@' })[1];
            rasterUtil.transType tType = (rasterUtil.transType)Enum.Parse(typeof(rasterUtil.transType), tTypeStr);
            object rs1 = null;
            if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.returnRaster(rsUtil.calcMathRasterFunction(rs1,tType));
        }

        private string getFeaturePath(string ftNm, bool featureClass)
        {
            string outPath = null;
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (featureClass)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Can't Find Feature " + ftNm;
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
            }
            return outPath;
        }

        private void calcGLCM(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[1].Split(new char[] { '@' })[1];
            string glcmTypes = prmArr[2].Split(new char[] { '@' })[1];
            string wDir = prmArr[3].Split(new char[] { '@' })[1];
            string wTyp = prmArr[4].Split(new char[] { '@' })[1];
            int rws = System.Convert.ToInt32(prmArr[5].Split(new char[] { '@' })[1]);
            int clm = System.Convert.ToInt32(prmArr[6 ].Split(new char[] { '@' })[1]);
            IRaster rs = null;
            if (rstDic.ContainsKey(inRaster1)) rs = rstDic[inRaster1];
            else rs = rsUtil.returnRaster(inRaster1);
            bool horz = true;
            rasterUtil.glcmMetric rm = (rasterUtil.glcmMetric)Enum.Parse(typeof(rasterUtil.glcmMetric),glcmTypes);
            if (wDir.ToLower() != "horizontal")
            {
                horz = false;
            }
            if (wTyp == rasterUtil.windowType.CIRCLE.ToString())
            {
                raster = rsUtil.createRaster(rsUtil.fastGLCMFunction(rs, clm, horz, rm));
            }
            else
            {
                raster = rsUtil.createRaster(rsUtil.fastGLCMFunction(rs, clm, rws, horz, rm));
            }
        }

        private void calcExtractBand(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[1].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            string bands = prmArr[0].Split(new char[] { '@' })[1];
            IRaster rs = null;
            ILongArray lArr = new LongArrayClass();
            IRasterBandCollection rsBc = new RasterClass();
            if (rstDic.ContainsKey(inRaster1)) rs = rstDic[inRaster1];
            else rs = rsUtil.returnRaster(inRaster1);
            foreach (string s in bands.Split(new char[] { ',' }))
            {
                int bndIndex = System.Convert.ToInt32(s.Split(new char[] { '_' })[1]) - 1;
                lArr.Add(bndIndex);
            }
            raster = rsUtil.createRaster(rsUtil.getBands(rs, lArr));
        }

        private void calcComposite(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRasters = prmArr[1].Split(new char[] { '@' })[1];
            List<IRaster> rsLst = new List<IRaster>();
            foreach (string s in inRasters.Split(new char[] { ',' }))
            {
                IRaster rs = null;
                if (rstDic.ContainsKey(s))
                {
                    rs = rstDic[s];
                }
                else
                {
                    rs = rsUtil.returnRaster(s);
                }
                rsLst.Add(rs);
                
            }
            raster = rsUtil.createRaster(rsUtil.compositeBandFunction(rsLst.ToArray()));
        }

        private void calcRemap(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            string remapVls = prmArr[1].Split(new char[] { '@' })[1];
            IRemapFilter flt = new RemapFilterClass();
            foreach(string s in remapVls.Split(new char[]{','}))
            {
                string[] sArr = s.Split(new char[] { '`' });
                double min = System.Convert.ToDouble(sArr[0]);
                double max = System.Convert.ToDouble(sArr[1]);
                double vl = System.Convert.ToDouble(sArr[2]);
                flt.AddClass(min, max, vl);
            }
            object rs1 = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.createRaster(rsUtil.calcRemapFunction(rs1, flt));
        }

        private void calcRescale(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[1].Split(new char[] { '@' })[1];
            string pType = prmArr[2].Split(new char[] { '@' })[1];
            rstPixelType rP = (rstPixelType)Enum.Parse(typeof(rstPixelType),pType);
            object rs1 = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.createRaster(rsUtil.reScaleRasterFunction(rs1, rP));
        }

        private void calcLinearTransform(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            float intercept = System.Convert.ToSingle(prmArr[1].Split(new char[] { '@' })[1]);
            string rasterSlopes = prmArr[2].Split(new char[] { '@' })[1];
            List<float> slpLst = new List<float>();
            slpLst.Add(intercept);
            IRasterBandCollection rsBc = new RasterClass();
            foreach (string s in rasterSlopes.Split(new char[]{','}))
            {
                string[] sArr = s.Split(new char[] { '`' });
                string rsNm = sArr[0];
                string slVl = sArr[1];
                IRaster rs = null;
                if (rstDic.ContainsKey(rsNm))
                {
                    rs = rstDic[rsNm];
                }
                else
                {
                    rs = rsUtil.returnRaster(rsNm);
                }
                rsBc.AppendBands((IRasterBandCollection)rs);
                slpLst.Add(System.Convert.ToSingle(s.Split(new char[]{'`'})[1]));
            }
            List<float[]> fLst = new List<float[]>();
            fLst.Add(slpLst.ToArray());
            raster = rsUtil.createRaster(rsUtil.calcRegressFunction((IRaster)rsBc, fLst));
        }

        private void calcSummarize(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[1].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            string sumType = prmArr[0].Split(new char[]{'@'})[1];
            rasterUtil.localType op = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),sumType);
            IRasterBandCollection rsBc = new RasterClass();
            foreach(string s in inRaster1.Split(new char[]{','}))
            {
                IRaster rs = null;
                if (rstDic.ContainsKey(s))
                {
                    rs = rstDic[s];
                }
                else
                {
                    rs = rsUtil.returnRaster(s);
                }
                rsBc.AppendBands((IRasterBandCollection)rs);
            }
            raster = rsUtil.createRaster(rsUtil.localStatisticsfunction((IRaster)rsBc,op));
        }

        private void calcFocal(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[1].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[5].Split(new char[] { '@' })[1];
            int clm = System.Convert.ToInt32(prmArr[3].Split(new char[] { '@' })[1]);
            int rws = System.Convert.ToInt32(prmArr[4].Split(new char[] { '@' })[1]);
            string wTyp = prmArr[2].Split(new char[] { '@' })[1];
            string fTyp = prmArr[0].Split(new char[] { '@' })[1];
            object rs1 = null;
            raster = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            rasterUtil.focalType fcType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), fTyp);
            rasterUtil.windowType wdType = (rasterUtil.windowType)Enum.Parse(typeof(rasterUtil.windowType),wTyp);
            if(wdType== rasterUtil.windowType.CIRCLE)
            {
                raster = rsUtil.createRaster(rsUtil.calcFocalStatisticsFunction(rs1,clm,fcType));
            }
            else
            {
                raster = rsUtil.createRaster(rsUtil.calcFocalStatisticsFunction(rs1,clm,rws,fcType));
            }
        }

        private void calcConvolution(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[3].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[4].Split(new char[] { '@' })[1];
            object rs1 = null;
            int wd,ht;
            wd = System.Convert.ToInt32(prmArr[1].Split(new char[] { '@' })[1]);
            ht = System.Convert.ToInt32(prmArr[2].Split(new char[] { '@' })[1]);
            string[] kns = prmArr[0].Split(new char[] { '@' })[1].Split(new char[] { ',' });
            double[] kn = new double[kns.Length];
            for (int i=0;i<kns.Length;i++)
            {
                kn[i] = System.Convert.ToDouble(kns[i]);
            }

            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster=rsUtil.createRaster(rsUtil.convolutionRasterFunction(rs1, wd, ht, kn));
        }

        private void calcConditional(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[1].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[3].Split(new char[] { '@' })[1];
            object rs1 = null;
            object rs2 = null;
            object rs3 = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            string inRaster2 = prmArr[2].Split(new char[] { '@' })[1];
            if (rsUtil.isNumeric(inRaster2))
            {
                rs2 = System.Convert.ToDouble(inRaster2);
            }
            else if (rstDic.ContainsKey(inRaster2))
            {
                rs2 = rstDic[inRaster2];
            }
            else
            {
                rs2 = inRaster2;
            }
            string inRaster3 = prmArr[0].Split(new char[] { '@' })[1];
            if (rsUtil.isNumeric(inRaster3))
            {
                rs3 = System.Convert.ToDouble(rs3);
            }
            else if (rstDic.ContainsKey(inRaster3))
            {
                rs3 = rstDic[inRaster3];
            }
            else
            {
                rs3 = inRaster3;
            }
            raster = rsUtil.createRaster(rsUtil.conditionalRasterFunction(rs1, rs2, rs3));
        }

        private void calcClip(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            string inFtr = prmArr[3].Split(new char[] { '@' })[1];
            string chbVL = prmArr[1].Split(new char[] { '@' })[1];
            IGeometry geo = geoUtil.createGeometry(geoUtil.getFeatureClass(inFtr));
            esriRasterClippingType cTy = esriRasterClippingType.esriRasterClippingInside;
            if(chbVL.ToLower()=="false")
            {
                cTy = esriRasterClippingType.esriRasterClippingOutside;
            }
            object rs1 = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = inRaster1;
            }
            raster = rsUtil.createRaster(rsUtil.clipRasterFunction(rs1, geo, cTy));
        }

        private void calcLogical(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[] { '@' })[1];
            string inRaster1 = prmArr[2].Split(new char[] { '@' })[1];
            object rs1 = null;
            object rs2 = null;
            if (rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if (rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = rsUtil.returnRaster(inRaster1);
                if (rs1 == null)
                {
                    string rsNm = getFeaturePath(name, false);
                    if (rsNm != null)
                    {
                        rs1 = rsUtil.returnRaster(rsNm);
                    }
                }
            }
            string inRaster2 = prmArr[1].Split(new char[] { '@' })[1];
            if (rsUtil.isNumeric(inRaster2))
            {
                rs2 = System.Convert.ToDouble(inRaster2);
            }
            else if (rstDic.ContainsKey(inRaster2))
            {
                rs2 = rstDic[inRaster2];
            }
            else
            {
                rs2 = rsUtil.returnRaster(inRaster2);
                if (rs2 == null)
                {
                    string rsNm = getFeaturePath(name, false);
                    if (rsNm != null)
                    {
                        rs2 = rsUtil.returnRaster(rsNm);
                    }
                }
            }
            string op = prmArr[3].Split(new char[] { '@' })[1];
            switch (op)
            {
                case ">":
                    raster = rsUtil.returnRaster(rsUtil.calcGreaterFunction(rs1, rs2));
                    break;
                case "<":
                    raster = rsUtil.returnRaster(rsUtil.calcLessFunction(rs1, rs2));
                    break;
                case "<=":
                    raster = rsUtil.returnRaster(rsUtil.calcLessEqualFunction(rs1, rs2));
                    break;
                case "=":
                    raster = rsUtil.returnRaster(rsUtil.calcEqualFunction(rs1, rs2));
                    break;
                case "and":
                    raster = rsUtil.returnRaster(rsUtil.calcAndFunction(rs1, rs2));
                    break;
                case "or":
                    raster = rsUtil.returnRaster(rsUtil.calcOrFunction(rs1, rs2));
                    break;
                default:
                    raster = rsUtil.returnRaster(rsUtil.calcGreaterEqualFunction(rs1, rs2));
                    break;
            }

        }

        private void calcArithmetic(string[] prmArr, out string name, out IRaster raster)
        {
            name = prmArr[0].Split(new char[]{'@'})[1];
            string inRaster1 = prmArr[2].Split(new char[]{'@'})[1];
            object rs1 = null;
            object rs2 = null;
            if(rsUtil.isNumeric(inRaster1))
            {
                rs1 = System.Convert.ToDouble(inRaster1);
            }
            else if(rstDic.ContainsKey(inRaster1))
            {
                rs1 = rstDic[inRaster1];
            }
            else
            {
                rs1 = rsUtil.returnRaster(inRaster1);
                if (rs1 == null)
                {
                    string rsNm = getFeaturePath(name, false);
                    if (rsNm != null)
                    {
                        rs1 = rsUtil.returnRaster(rsNm);
                    }
                }
            }
            string inRaster2 = prmArr[1].Split(new char[]{'@'})[1];
            if(rsUtil.isNumeric(inRaster2))
            {
                rs2 = System.Convert.ToDouble(inRaster2);
            }
            else if(rstDic.ContainsKey(inRaster2))
            {
                rs2 = rstDic[inRaster2];
            }
            else
            {
                rs2 = rsUtil.returnRaster(inRaster2);
                if (rs2 == null)
                {
                    string rsNm = getFeaturePath(name, false);
                    if (rsNm != null)
                    {
                        rs2 = rsUtil.returnRaster(rsNm);
                    }
                }
            }
            string op = prmArr[3].Split(new char[]{'@'})[1];
            esriRasterArithmeticOperation rsOp = esriRasterArithmeticOperation.esriRasterPlus;
            switch (op)
	        {
                case "*":
                    rsOp = esriRasterArithmeticOperation.esriRasterMultiply;
                    break;
                case "POW":
                    rsOp = esriRasterArithmeticOperation.esriRasterPower;
                    break;
                case "-":
                    rsOp = esriRasterArithmeticOperation.esriRasterMinus;
                    break;
                case "/":
                    rsOp = esriRasterArithmeticOperation.esriRasterDivide;
                    break;
                case "MODE":
                    rsOp = esriRasterArithmeticOperation.esriRasterMode;
                    break;
		        default:
                    rsOp = esriRasterArithmeticOperation.esriRasterPlus;
                    break;
	        }
            raster = rsUtil.returnRaster(rsUtil.calcArithmaticFunction(rs1, rs2, rsOp));
        }


        public static void estimateStatistics(IRaster inRaster, IRaster outRaster, rasterUtil.transType transType)
        {
            IRasterBandCollection rsbci = (IRasterBandCollection)inRaster;
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbci.Count; i++)
            {
                IRasterBand rsBi = rsbci.Item(i);
                IRasterBand rsBo = rsbco.Item(i);
                IRasterStatistics rsBStatsi = rsBi.Statistics;
                IRasterStatistics rsBStatso = rsBo.Statistics;
                if (rsBStatsi == null)
                {
                    continue;
                }
                if (rsBStatso == null)
                {
                    rsBStatso = new RasterStatistics();
                }
                switch (transType)
                {
                    case rasterUtil.transType.LOG10:
                        rsBStatso.Maximum = Math.Log10(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Log10(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Log10(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Log10(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.LN:
                        rsBStatso.Maximum = Math.Log(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Log(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Log(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Log(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.EXP:
                        rsBStatso.Maximum = Math.Exp(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Exp(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Exp(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Exp(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.EXP10:
                        rsBStatso.Maximum = Math.Pow(10,rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Pow(10, rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Pow(10, rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Pow(10, rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.ABS:
                        rsBStatso.Maximum = Math.Abs(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Abs(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Abs(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Abs(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.SIN:
                        rsBStatso.Maximum = Math.Sin(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Sin(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Sin(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Sin(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.COS:
                        rsBStatso.Maximum = 1;
                        rsBStatso.Minimum = -1;
                        rsBStatso.StandardDeviation = Math.Cos(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Cos(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.TAN:
                        rsBStatso.Maximum = Math.Tan(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Tan(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Tan(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Tan(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.ASIN:
                        rsBStatso.Maximum = Math.Asin(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Asin(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Asin(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Asin(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.ACOS:
                        rsBStatso.Maximum = Math.Acos(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Acos(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Acos(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Acos(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.ATAN:
                        rsBStatso.Maximum = Math.Atan(rsBStatsi.Maximum);
                        rsBStatso.Minimum = Math.Atan(rsBStatsi.Minimum);
                        rsBStatso.StandardDeviation = Math.Atan(rsBStatsi.StandardDeviation);
                        rsBStatso.Mean = Math.Atan(rsBStatsi.Mean);
                        break;
                    case rasterUtil.transType.RADIANS:
                        rsBStatso.Maximum = rsBStatsi.Maximum*Math.PI/180;
                        rsBStatso.Minimum = rsBStatsi.Minimum * Math.PI / 180;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation * Math.PI / 180;
                        rsBStatso.Mean = rsBStatsi.Mean * Math.PI / 180;
                        break;
                    default:
                        break;
                }
            }
        }
        public static void estimateStatistics(object inRaster1, object inRaster2, IRaster outRaster, esriRasterArithmeticOperation arithmeticType)
        {
            if (inRaster1 is Raster&&inRaster2 is IScalar)
            {
                IRasterBandCollection rsbci = (IRasterBandCollection)inRaster1;
                IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
                double[] sc = (double[])((IScalar)inRaster2).Value;
                for (int i = 0; i < rsbci.Count; i++)
                {
                    IRasterBand rsBi = rsbci.Item(i);
                    IRasterBand rsBo = rsbco.Item(i);
                    IRasterStatistics rsBStatsi = rsBi.Statistics;
                    IRasterStatistics rsBStatso = rsBo.Statistics;
                    if (rsBStatsi == null)
                    {
                        continue;
                    }
                    if (rsBStatso == null)
                    {
                        rsBStatso = new RasterStatistics();
                    }
                    double vl = sc[i];
                    switch (arithmeticType)
	                {
		                case esriRasterArithmeticOperation.esriRasterDivide:
                            rsBStatso.Minimum = rsBStatsi.Minimum / vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum / vl;
                            rsBStatso.Mean = rsBStatsi.Mean / vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation / vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMinus:
                            rsBStatso.Minimum = rsBStatsi.Minimum - vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum - vl;
                            rsBStatso.Mean = rsBStatsi.Mean - vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation - vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMode:
                            rsBStatso.Minimum = rsBStatsi.Minimum % vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum % vl;
                            rsBStatso.Mean = rsBStatsi.Mean % vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation % vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMultiply:
                            rsBStatso.Minimum = rsBStatsi.Minimum * vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum * vl;
                            rsBStatso.Mean = rsBStatsi.Mean * vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation * vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPlus:
                            rsBStatso.Minimum = rsBStatsi.Minimum + vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum + vl;
                            rsBStatso.Mean = rsBStatsi.Mean + vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation + vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPower:
                            rsBStatso.Minimum = Math.Pow(rsBStatsi.Minimum,vl);
                            rsBStatso.Maximum = Math.Pow(rsBStatsi.Maximum,vl);
                            rsBStatso.Mean = Math.Pow(rsBStatsi.Mean,vl);
                            rsBStatso.StandardDeviation = Math.Pow(rsBStatsi.StandardDeviation, vl);
                            break;
                         default:
                            break;
	                }
                }
            }
            else if(inRaster1 is Scalar&&inRaster2 is Raster)
            {
                IRasterBandCollection rsbci = (IRasterBandCollection)inRaster2;
                IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
                double[] sc = (double[])((IScalar)inRaster1).Value;
                for (int i = 0; i < rsbci.Count; i++)
                {
                    IRasterBand rsBi = rsbci.Item(i);
                    IRasterBand rsBo = rsbco.Item(i);
                    IRasterStatistics rsBStatsi = rsBi.Statistics;
                    IRasterStatistics rsBStatso = rsBo.Statistics;
                    if (rsBStatsi == null)
                    {
                        continue;
                    }
                    if (rsBStatso == null)
                    {
                        rsBStatso = new RasterStatistics();
                    }
                    double vl = sc[i];
                    switch (arithmeticType)
                    {
                        case esriRasterArithmeticOperation.esriRasterDivide:
                            rsBStatso.Minimum = rsBStatsi.Minimum / vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum / vl;
                            rsBStatso.Mean = rsBStatsi.Mean / vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation / vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMinus:
                            rsBStatso.Minimum = rsBStatsi.Minimum - vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum - vl;
                            rsBStatso.Mean = rsBStatsi.Mean - vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation - vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMode:
                            rsBStatso.Minimum = rsBStatsi.Minimum % vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum % vl;
                            rsBStatso.Mean = rsBStatsi.Mean % vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation % vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMultiply:
                            rsBStatso.Minimum = rsBStatsi.Minimum * vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum * vl;
                            rsBStatso.Mean = rsBStatsi.Mean * vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation * vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPlus:
                            rsBStatso.Minimum = rsBStatsi.Minimum + vl;
                            rsBStatso.Maximum = rsBStatsi.Maximum + vl;
                            rsBStatso.Mean = rsBStatsi.Mean + vl;
                            rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation + vl;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPower:
                            rsBStatso.Minimum = Math.Pow(rsBStatsi.Minimum, vl);
                            rsBStatso.Maximum = Math.Pow(rsBStatsi.Maximum, vl);
                            rsBStatso.Mean = Math.Pow(rsBStatsi.Mean, vl);
                            rsBStatso.StandardDeviation = Math.Pow(rsBStatsi.StandardDeviation, vl);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                IRasterBandCollection rsbci1 = (IRasterBandCollection)inRaster1;
                IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
                IRasterBandCollection rsbci2 = (IRasterBandCollection)inRaster1;
                for (int i = 0; i < rsbci1.Count; i++)
                {
                    IRasterBand rsBi1 = rsbci1.Item(i);
                    IRasterBand rsBi2 = rsbci2.Item(i);
                    IRasterBand rsBo = rsbco.Item(i);
                    IRasterStatistics rsBStatsi1 = rsBi1.Statistics;
                    IRasterStatistics rsBStatsi2 = rsBi2.Statistics;
                    IRasterStatistics rsBStatso = rsBo.Statistics;
                    if (rsBStatsi1 == null||rsBStatsi2==null)
                    {
                        continue;
                    }
                    if (rsBStatso == null)
                    {
                        rsBStatso = new RasterStatistics();
                    }
                    switch (arithmeticType)
                    {
                        case esriRasterArithmeticOperation.esriRasterDivide:
                            rsBStatso.Minimum = rsBStatsi1.Minimum / rsBStatsi2.Minimum;
                            rsBStatso.Maximum = rsBStatsi1.Maximum / rsBStatsi2.Maximum;
                            rsBStatso.Mean = rsBStatsi1.Mean / rsBStatsi2.Mean;
                            rsBStatso.StandardDeviation = rsBStatsi1.StandardDeviation / rsBStatsi2.StandardDeviation;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMinus:
                            rsBStatso.Minimum = rsBStatsi1.Minimum - rsBStatsi2.Maximum;
                            rsBStatso.Maximum = rsBStatsi1.Maximum - rsBStatsi2.Minimum;
                            rsBStatso.Mean = (rsBStatsi1.Mean + rsBStatsi2.Mean)/2;
                            rsBStatso.StandardDeviation = (rsBStatsi1.StandardDeviation + rsBStatsi2.Minimum)/2;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMode:
                            rsBStatso.Minimum = rsBStatsi1.Minimum % rsBStatsi2.Minimum;
                            rsBStatso.Maximum = rsBStatsi1.Maximum % rsBStatsi2.Maximum;
                            rsBStatso.Mean = rsBStatsi1.Mean % rsBStatsi2.Mean;
                            rsBStatso.StandardDeviation = rsBStatsi1.StandardDeviation % rsBStatsi2.StandardDeviation;
                            break;
                        case esriRasterArithmeticOperation.esriRasterMultiply:
                            rsBStatso.Minimum = rsBStatsi1.Minimum / rsBStatsi2.Minimum;
                            rsBStatso.Maximum = rsBStatsi1.Maximum / rsBStatsi2.Maximum;
                            rsBStatso.Mean = rsBStatsi1.Mean / rsBStatsi2.Mean;
                            rsBStatso.StandardDeviation = rsBStatsi1.StandardDeviation / rsBStatsi2.StandardDeviation;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPlus:
                            rsBStatso.Minimum = rsBStatsi1.Minimum + rsBStatsi2.Maximum;
                            rsBStatso.Maximum = rsBStatsi1.Maximum + rsBStatsi2.Minimum;
                            rsBStatso.Mean = (rsBStatsi1.Mean + rsBStatsi2.Mean)/2;
                            rsBStatso.StandardDeviation = (rsBStatsi1.StandardDeviation + rsBStatsi2.Minimum)/2;
                            break;
                        case esriRasterArithmeticOperation.esriRasterPower:
                            rsBStatso.Minimum = Math.Pow(rsBStatsi1.Minimum, rsBStatsi2.Minimum);
                            rsBStatso.Maximum = Math.Pow(rsBStatsi1.Maximum, rsBStatsi2.Maximum);
                            rsBStatso.Mean = Math.Pow(rsBStatsi1.Mean, rsBStatsi2.Mean);
                            rsBStatso.StandardDeviation = Math.Pow(rsBStatsi1.StandardDeviation, rsBStatsi2.StandardDeviation);
                            break;
                        default:
                            break;
                    }
                }
            }
            
        }

        public static void estimateStatistics(IRaster outRaster)
        {
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbco.Count; i++)
            {
                IRasterBand rsBo = rsbco.Item(i);
                IRasterStatistics rsBStatso = rsBo.Statistics;
                if (rsBStatso == null)
                {
                    rsBStatso = new RasterStatisticsClass();
                }
                rsBStatso.Minimum = 0;
                rsBStatso.Maximum = 1;
                rsBStatso.Mean = 0.5;
                rsBStatso.StandardDeviation = 0.08;
            }
        }

        public static void estimateStatistics(IRaster outRaster, dem dem)
        {
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbco.Count; i++)
            {
                
                IRasterBand rsBo = rsbco.Item(i);
                IRasterStatistics rsBStatso = rsBo.Statistics;
                if (rsBStatso == null)
                {
                    rsBStatso = new RasterStatisticsClass();
                }
                switch (dem)
                {
                    case dem.NorthSouth:
                        rsBStatso.Minimum = 0;
                        rsBStatso.Maximum = 180;
                        rsBStatso.Mean = 90;
                        rsBStatso.StandardDeviation = 57.6;
                        break;
                    case dem.EastWest:
                       rsBStatso.Minimum = 0;
                        rsBStatso.Maximum = 180;
                        rsBStatso.Mean = 90;
                        rsBStatso.StandardDeviation = 57.6;
                        break;
                    case dem.Slope:
                        rsBStatso.Minimum = 0;
                        rsBStatso.Maximum = 90;
                        rsBStatso.Mean = 45;
                        rsBStatso.StandardDeviation = 26.3;
                        break;
                    case dem.Aspect:
                        rsBStatso.Minimum = 0;
                        rsBStatso.Maximum = 360;
                        rsBStatso.Mean = 180;
                        rsBStatso.StandardDeviation = 115.2;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void estimateStatistics(IRaster inRaster, IRaster outRaster, rasterUtil.localType localType)
        {
            IRasterBandCollection rsbci = (IRasterBandCollection)inRaster;
            double[] bMaxArr = new double[rsbci.Count];
            double[] bMinArr = new double[rsbci.Count];
            double[] bMeanArr = new double[rsbci.Count];
            double[] bStdArr = new double[rsbci.Count];
            double[] bModeArr = new double[rsbci.Count];
            double[] bMedianArr = new double[rsbci.Count];
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            IRasterStatistics rsStatso = rsbco.Item(0).Statistics;
            
            if (rsStatso == null)
            {
                rsStatso = new RasterStatistics();
            }
            for (int i = 0; i < rsbci.Count; i++)
            {
                IRasterBand rsBi = rsbci.Item(i);
                IRasterStatistics rsBStatsi = rsBi.Statistics;
                if (rsBStatsi == null)
                {
                    continue;
                }
                bMaxArr[i] = rsBStatsi.Maximum;
                bMinArr[i] = rsBStatsi.Minimum;
                bMeanArr[i] = rsBStatsi.Mean;
                bStdArr[i] = rsBStatsi.StandardDeviation;
            }
            switch (localType)
            {
                case rasterUtil.localType.MAX:
                    rsStatso.Maximum = bMaxArr.Max();
                    rsStatso.Minimum = bMinArr.Max();
                    rsStatso.Mean = bMeanArr.Max();
                    rsStatso.StandardDeviation = bStdArr.Max();
                    break;
                case rasterUtil.localType.MIN:
                    rsStatso.Maximum = bMaxArr.Min();
                    rsStatso.Minimum = bMinArr.Min();
                    rsStatso.Mean = bMeanArr.Min();
                    rsStatso.StandardDeviation = bStdArr.Min();
                    break;
                case rasterUtil.localType.MAXBAND:
                case rasterUtil.localType.MINBAND:
                    rsStatso.Maximum = rsbci.Count;
                    rsStatso.Minimum = 1;
                    rsStatso.Mean = rsbci.Count/2;
                    rsStatso.StandardDeviation = (rsStatso.Mean*0.48)/3;
                    break;
                case rasterUtil.localType.SUM:
                    rsStatso.Mean = bMeanArr.Sum();
                    rsStatso.Maximum = bMaxArr.Sum();
                    rsStatso.Minimum = bMinArr.Sum();
                    rsStatso.StandardDeviation = bStdArr.Sum();
                    break;
                case rasterUtil.localType.MULTIPLY:
                    rsStatso.Mean = iterValues(bMeanArr,localType);
                    rsStatso.Maximum = iterValues(bMeanArr, localType);
                    rsStatso.Minimum = iterValues(bMeanArr, localType);
                    rsStatso.StandardDeviation = iterValues(bMeanArr, localType);
                    break;
                case rasterUtil.localType.DIVIDE:
                    rsStatso.Mean = iterValues(bMeanArr, localType);
                    rsStatso.Maximum = iterValues(bMeanArr, localType);
                    rsStatso.Minimum = iterValues(bMeanArr, localType);
                    rsStatso.StandardDeviation = iterValues(bMeanArr, localType);
                    break;
                case rasterUtil.localType.SUBTRACT:
                    rsStatso.Mean = iterValues(bMeanArr, localType);
                    rsStatso.Maximum = iterValues(bMeanArr, localType);
                    rsStatso.Minimum = iterValues(bMeanArr, localType);
                    rsStatso.StandardDeviation = iterValues(bMeanArr, localType);
                    break;
                case rasterUtil.localType.POWER:
                    rsStatso.Mean = iterValues(bMeanArr, localType);
                    rsStatso.Maximum = iterValues(bMeanArr, localType);
                    rsStatso.Minimum = iterValues(bMeanArr, localType);
                    rsStatso.StandardDeviation = iterValues(bMeanArr, localType);
                    break;
                case rasterUtil.localType.MEAN:
                    rsStatso.Mean = bMeanArr.Average();
                    rsStatso.Maximum = bMaxArr.Average();
                    rsStatso.Minimum = bMinArr.Average();
                    rsStatso.StandardDeviation = bStdArr.Average();
                    break;
                case rasterUtil.localType.VARIANCE:
                    double stdMean = bStdArr.Average();
                    rsStatso.Mean = Math.Pow(stdMean,2);
                    rsStatso.Maximum = Math.Pow(stdMean*1.65,2);
                    rsStatso.Minimum = Math.Pow(stdMean-(stdMean*0.65),2);
                    rsStatso.StandardDeviation = Math.Pow(stdMean*.21666666,2);
                    break;
                case rasterUtil.localType.STD:
                    rsStatso.Mean = bStdArr.Average();
                    rsStatso.Maximum = rsStatso.Mean*1.65;
                    rsStatso.Minimum = rsStatso.Mean - (rsStatso.Mean*0.65);
                    rsStatso.StandardDeviation = rsStatso.Mean*.21666666;
                    break;
                case rasterUtil.localType.MODE:
                    rsStatso.Mean = bModeArr.Average();
                    rsStatso.Maximum = bModeArr.Max();
                    rsStatso.Minimum = bModeArr.Min();
                    rsStatso.StandardDeviation = rsStatso.Mean * 0.2166666;
                    break;
                case rasterUtil.localType.MEDIAN:
                    rsStatso.Mean = bMedianArr.Average();
                    rsStatso.Maximum = bMedianArr.Max();
                    rsStatso.Minimum = bMedianArr.Min();
                    rsStatso.StandardDeviation = rsStatso.Mean * 0.2166666;
                    break;
                case rasterUtil.localType.UNIQUE:
                    rsStatso.Maximum = rsbci.Count;
                    rsStatso.Minimum = 1;
                    rsStatso.Mean = rsbci.Count/2;
                    rsStatso.StandardDeviation = (rsStatso.Mean*0.48)/3;
                    break;
                case rasterUtil.localType.ENTROPY:
                    rsStatso.Maximum = (-1*0.4*Math.Log(0.4))*(rsbci.Count*.4);
                    rsStatso.Minimum = 0;
                    rsStatso.Mean = (rsStatso.Maximum-rsStatso.Minimum)/2;
                    rsStatso.StandardDeviation = (rsStatso.Mean*0.48)/3;
                    break;
                default:
                    break;
            }
        }

        private static double iterValues(double[] doubleArr, rasterUtil.localType localType)
        {
            double outVl = 0;
            foreach (double d in doubleArr)
            {
                switch (localType)
                {
                    case rasterUtil.localType.MULTIPLY:
                        outVl = outVl * d;
                        break;
                    case rasterUtil.localType.DIVIDE:
                        outVl = outVl / d;
                        break;
                    case rasterUtil.localType.SUBTRACT:
                        outVl = outVl - d;
                        break;
                    case rasterUtil.localType.POWER:
                        outVl = Math.Pow(outVl, d);
                        break;
                    default:
                        break;
                }
            }
            return outVl;
        }

        public static void estimateStatistics(double rasterValue, IRaster outRaster)
        {
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbco.Count; i++)
            {
                IRasterBand rsB = rsbco.Item(i);
                IRasterStatistics rStat = rsB.Statistics;
                if (rStat == null)
                {
                    rStat = new RasterStatistics();
                }
                rStat.Minimum = rasterValue;
                rStat.Maximum = rasterValue;
                rStat.Mean = rasterValue;
                rStat.StandardDeviation = 0;
            }
        }

        public static void estimateStatistics(IRaster inRaster, IRaster outRaster, rasterUtil.focalType statType,double cells)
        {
            IRasterBandCollection rsbci = (IRasterBandCollection)inRaster;
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbci.Count; i++)
            {
                IRasterBand rsBi = rsbci.Item(i);
                IRasterBand rsBo = rsbco.Item(i);
                IRasterStatistics rsBStatsi = rsBi.Statistics;
                IRasterStatistics rsBStatso = rsBo.Statistics;
                if (rsBStatsi == null)
                {
                    continue;
                }
                if (rsBStatso == null)
                {
                    rsBStatso = new RasterStatistics();
                }
                switch (statType)
                {
                    case rasterUtil.focalType.MAX:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.MIN:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.SUM:
                        rsBStatso.Maximum = rsBStatsi.Maximum*cells;
                        rsBStatso.Minimum = rsBStatsi.Minimum*cells;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation*cells;
                        rsBStatso.Mean = rsBStatsi.Mean*cells;
                        break;
                    case rasterUtil.focalType.MEAN:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.MODE:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.MEDIAN:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.VARIANCE:
                        rsBStatso.Maximum = rsBStatsi.Maximum;
                        rsBStatso.Minimum = rsBStatsi.Minimum;
                        rsBStatso.StandardDeviation = rsBStatsi.StandardDeviation;
                        rsBStatso.Mean = rsBStatsi.Mean;
                        break;
                    case rasterUtil.focalType.STD:
                        rsBStatso.Mean = rsBStatsi.StandardDeviation;
                        rsBStatso.Maximum = rsBStatso.Mean*1.96;
                        rsBStatso.Minimum = rsBStatso.Mean-(rsBStatso.Maximum-rsBStatso.Mean);
                        rsBStatso.StandardDeviation = (rsBStatso.Maximum-rsBStatso.Mean)/3;
                        break;
                    case rasterUtil.focalType.UNIQUE:
                        rsBStatso.Maximum = cells;
                        rsBStatso.Minimum = 1;
                        rsBStatso.Mean = cells / 2;
                        rsBStatso.StandardDeviation = (rsBStatso.Maximum - rsBStatso.Mean) / 3;
                        break;
                    case rasterUtil.focalType.ENTROPY:
                        rsBStatso.Maximum = cells*0.4*(-1*0.4*Math.Log(0.4));
                        rsBStatso.Minimum = 0;
                        rsBStatso.Mean = rsBStatso.Maximum / 2;
                        rsBStatso.StandardDeviation = (rsBStatso.Maximum - rsBStatso.Mean) / 3;
                        break;
                    case rasterUtil.focalType.ASM:
                        rsBStatso.Maximum = 1;
                        rsBStatso.Minimum = 0;
                        rsBStatso.Mean = rsBStatso.Maximum / 2;
                        rsBStatso.StandardDeviation = (rsBStatso.Maximum - rsBStatso.Mean) / 3;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void estimateStatistics(IRaster outRaster, rstPixelType pType)
        {
            IRasterBandCollection rsbco = (IRasterBandCollection)outRaster;
            for (int i = 0; i < rsbco.Count; i++)
            {
                IRasterBand rsBo = rsbco.Item(i);
                IRasterStatistics rsBStatso = rsBo.Statistics;
                if (rsBStatso == null)
                {
                    rsBStatso = new RasterStatistics();
                }
                switch (pType)
                {
                    case rstPixelType.PT_CHAR:
                        rsBStatso.Maximum = Char.MaxValue;
                        rsBStatso.Minimum = Char.MinValue + 1;
                        break;
                    case rstPixelType.PT_FLOAT:
                        rsBStatso.Maximum = Single.MaxValue;
                        rsBStatso.Minimum = Single.MinValue + 1;
                        break;
                    case rstPixelType.PT_LONG:
                        rsBStatso.Maximum = long.MaxValue;
                        rsBStatso.Minimum = long.MinValue + 1;
                        break;
                    case rstPixelType.PT_SHORT:
                        rsBStatso.Maximum = short.MaxValue;
                        rsBStatso.Minimum = short.MinValue + 1;
                        break;
                    case rstPixelType.PT_U1:
                        rsBStatso.Maximum = 1;
                        rsBStatso.Minimum = 0;
                        break;
                    case rstPixelType.PT_U2:
                        rsBStatso.Maximum = 3;
                        rsBStatso.Minimum = 0;
                        break;
                    case rstPixelType.PT_U4:
                        rsBStatso.Maximum = 15;
                        rsBStatso.Minimum = 0;
                        break;
                    case rstPixelType.PT_UCHAR:
                        rsBStatso.Maximum = 255;
                        rsBStatso.Minimum = 0;
                        break;
                    case rstPixelType.PT_ULONG:
                        rsBStatso.Maximum = ulong.MaxValue-1;
                        rsBStatso.Minimum = 0;
                        break;
                    case rstPixelType.PT_USHORT:
                        rsBStatso.Maximum = ushort.MaxValue-1;
                        rsBStatso.Minimum = 0;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void buildStats(IRaster inputRaster, string functionModelPath)
        {
            IRasterProps rsProps = (IRasterProps)inputRaster;
            double[] noDataArr = (double[])rsProps.NoDataValue;
            IRasterBandCollection rsBc = (IRasterBandCollection)inputRaster;
            string statPath = functionModelPath.Replace(".fds",".sta");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(statPath))
            {
                for (int i = 0; i < rsBc.Count; i++)
                {
                    double ndVl = noDataArr[i];
                    IRasterBand rsBand = rsBc.Item(i);
                    IRasterStatistics rsStats = rsBand.Statistics;
                    double max = checkStatNumber(rsStats.Maximum,ndVl);
                    double min = checkStatNumber(rsStats.Minimum,ndVl);
                    double mean = checkStatNumber(rsStats.Mean,ndVl);
                    double std = checkStatNumber(rsStats.StandardDeviation,ndVl);
                    string bndStatsLn = max.ToString() + ":" + mean.ToString() + ":" + min.ToString() + ":" + std.ToString()+ ":"+rsStats.SkipFactorX.ToString()+":"+rsStats.SkipFactorY;
                    List<string> igVlLst = new List<string>();
                    if (!(rsStats.IgnoredValues == null))
                    {
                        System.Array ignoredVl = (System.Array)rsStats.IgnoredValues;

                        for (int j = 0; j < ignoredVl.Length; j++)
                        {
                            string vl = checkStatNumber((double)ignoredVl.GetValue(j),ndVl).ToString();
                            igVlLst.Add(vl);
                        }
                    }
                    bndStatsLn = bndStatsLn + ":" + String.Join(",", igVlLst.ToArray());
                    sw.WriteLine(bndStatsLn);
                }
                sw.Close();
            }
        }

        private static double checkStatNumber(double p,double noDataVl)
        {
            double vl = p;
            if (Double.IsNaN(p))
            {
                vl = noDataVl;
            }
            else if (Double.IsNegativeInfinity(p))
            {
                vl = -1.79E+308;
            }
            else if (Double.IsPositiveInfinity(p))
            {
                vl = 1.79E+308;
            }
            else if (p==Double.MinValue)
            {
                vl = -1.79E+308;
            }
            else if (p == Double.MaxValue)
            {
                vl = 1.79e+308;
            }
            return vl;
        }
        private IRaster updateStats(IRaster outRs)
        {
            IRaster2 rs2 = (IRaster2)outRs;
            //rs2.AttributeTable = null;
            IRasterProps rsP = (IRasterProps)rs2;
            int sk = rsP.Width/10;
            int sk2 = rsP.Width/10;
            if (sk2<sk)
            {
                sk=sk2;
            }
            string statPath =  FunctionDatasetPath.Replace(".fds", ".sta");
            IRasterBandCollection rsBc = (IRasterBandCollection)outRs;
            bool t = true;
            rsBc.Item(0).HasStatistics(out t);
            IRaster outRs2 = null;
            if (!t)
            {
                outRs2 = rsUtil.calcStatsAndHist(outRs, sk);
                rsBc = (IRasterBandCollection)outRs2;
            }
            else
            {
                outRs2 = outRs;
            }

            if (System.IO.File.Exists(statPath))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(statPath))
                {
                    int lnCnt = 0;
                    string bandLn = "";
                    while ((bandLn = sr.ReadLine()) != null)
                    {
                        string[] statArr = bandLn.Split(new char[] { ':' });
                        if (statArr.Length > 0)
                        {
                            IRasterBand rsB = rsBc.Item(lnCnt);
                            IRasterStatistics rsStats = rsB.Statistics;
                            rsStats.Maximum = System.Convert.ToDouble(statArr[0]);
                            rsStats.Mean = System.Convert.ToDouble(statArr[1]);
                            rsStats.Minimum = System.Convert.ToDouble(statArr[2]);
                            rsStats.StandardDeviation = System.Convert.ToDouble(statArr[3]);
                            rsStats.SkipFactorX = System.Convert.ToInt32(statArr[4]);
                            rsStats.SkipFactorY = System.Convert.ToInt32(statArr[5]);
                            string[] igArr = statArr[6].Split(new char[] { ',' });
                            double[] igDarr = new double[igArr.Length];
                            for (int j = 0; j < igArr.Length; j++)
                            {
                                string vl = igArr[j];
                                if (vl != "")
                                {
                                    igDarr[j] = System.Convert.ToDouble(igArr[j]);
                                }
                            }
                            rsStats.IgnoredValues = igDarr;
                        }
                        lnCnt++;
                    }
                    sr.Close();
                }
                
            }
            else
            {
                
            }
            return outRs2;
        }
    }
}
