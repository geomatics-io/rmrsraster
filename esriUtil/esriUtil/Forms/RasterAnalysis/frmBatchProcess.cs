using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmBatchProcess : Form
    {
        private viewUtility vUtil = null;
        private batchCalculations btCalc = null;
        private rasterUtil rsUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, ITable> tblDic = new Dictionary<string, ITable>();
        private Dictionary<string, string> lyrDic = new Dictionary<string, string>();
        private void populateCmb()
        {
            if(mp!=null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rstLyr.Raster;
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        IDataset dSet = (IDataset)((IRaster2)rst).RasterDataset;
                        rstDic.Add(lyrNm, rst);
                        lyrDic.Add(lyrNm, dSet.Workspace.PathName + "\\" + dSet.BrowseName);
                        lsbLayers.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
                IEnumLayer ftrLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                lyr = ftrLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    if (!ftrDic.ContainsKey(lyrNm))
                    {
                        ftrDic.Add(lyrNm, ftrCls);
                        IDataset dSet = (IDataset)ftrCls;
                        lyrDic.Add(lyrNm, dSet.Workspace.PathName + "\\" + dSet.BrowseName);
                        lsbLayers.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
            lsbFunctions.Items.AddRange(Enum.GetNames(typeof(batchCalculations.batchGroups)));
            lsbFunctions.SelectedItem = lsbFunctions.Items[0];

        }
        public frmBatchProcess()
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            btCalc = new batchCalculations(rsUtil,rp);
            populateCmb();
        }
        public frmBatchProcess(IMap map, rasterUtil rasterUtility)
        {
            InitializeComponent();
            mp = map;
            vUtil = new viewUtility((IActiveView)mp);
            rsUtil = rasterUtility;
            btCalc = new batchCalculations(rsUtil,rp);
            populateCmb();
        }
        private esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new esriUtil.Forms.RunningProcess.frmRunningProcessDialog(false);
        private IMap mp = null;
        private void btnEqual_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void addToBatch(object sender)
        {
            string vl = ((Button)sender).Text;
            if (vl.ToLower() == "return") vl = Environment.NewLine;
            int sS = rtbBatch.SelectionStart;
            rtbBatch.Text = rtbBatch.Text.Insert(sS,vl);
            rtbBatch.SelectionStart = sS + vl.Length;
        }

        private void btnComma_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void btnSep_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void btnPl_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void btnPR_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void btnOpenLayer_Click(object sender, EventArgs e)
        {
            getLayer();

        }
        private void getLayer()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt3 = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt4 = new ESRI.ArcGIS.Catalog.GxFilterTablesClass();
            fltColl.AddFilter(flt, true);
            fltColl.AddFilter(flt2, false);
            fltColl.AddFilter(flt3, false);
            fltColl.AddFilter(flt4, false);
            gxDialog.Title = "Select a Raster, Polygon, Point, or Table";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    if (gxDialog.ObjectFilter is ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass)
                    {
                        IRaster rs = rsUtil.returnRaster(outPath);
                        if (!rstDic.ContainsKey(outName))
                        {
                            rstDic.Add(outName, rs);
                        }
                        else
                        {
                            rstDic[outName] = rs;
                        }
                    }
                    else if (gxDialog.ObjectFilter is ESRI.ArcGIS.Catalog.GxFilterTablesClass)
                    {
                        ITable tbl = geoUtil.getTable(outPath);
                        if (!tblDic.ContainsKey(outName))
                        {
                            tblDic.Add(outName, tbl);
                        }
                        else
                        {
                            tblDic[outName] = tbl;
                        }
                    }
                    else
                    {
                        IFeatureClass ftrCls = geoUtil.getFeatureClass(outPath);
                        if (!ftrDic.ContainsKey(outName))
                        {
                            ftrDic.Add(outName, ftrCls);
                        }
                        else
                        {
                            ftrDic[outName] = ftrCls;
                        }
                    }
                    if (!lyrDic.ContainsKey(outName))
                    {
                        lyrDic.Add(outName, outPath);
                        lsbLayers.Items.Add(outName);
                    }
                    else
                    {
                        lyrDic[outName] = outPath;
                    }
                    gxObj = eGxObj.Next();
                }
            }
            return;
        }

        private void lsbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string vl = lsbLayers.SelectedItem.ToString();
            int sStart = rtbBatch.SelectionStart;
            rtbBatch.Text = rtbBatch.Text.Insert(sStart, vl);
            rtbBatch.SelectionStart = sStart + vl.Length;
        }

        private void lsbFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string vl = lsbFunctions.SelectedItem.ToString();
            lsbOptions.Items.Clear();
            updateOptions(vl);
        }

        private void updateOptions(string vl)
        {
            batchCalculations.batchGroups g = (batchCalculations.batchGroups)Enum.Parse(typeof(batchCalculations.batchGroups), vl);
            string[] nm = null;
            List<string> l = new List<string>();
            switch (g)
            {
                case batchCalculations.batchGroups.ARITHMETIC:
                    nm = Enum.GetNames(typeof(esriRasterArithmeticOperation));
                    break;
                case batchCalculations.batchGroups.MATH:
                    nm = Enum.GetNames(typeof(rasterUtil.transType));
                    break;
                case batchCalculations.batchGroups.LOGICAL:
                    nm = Enum.GetNames(typeof(rasterUtil.logicalType));
                    break;
                case batchCalculations.batchGroups.FOCAL:
                    nm = Enum.GetNames(typeof(rasterUtil.focalType));
                    break;
                case batchCalculations.batchGroups.LOCALSTATISTICS:
                    nm = Enum.GetNames(typeof(rasterUtil.localType));
                    break;
                case batchCalculations.batchGroups.RESCALE:
                    nm = Enum.GetNames(typeof(rstPixelType));
                    break;
                case batchCalculations.batchGroups.GLCM:
                    nm = Enum.GetNames(typeof(rasterUtil.glcmMetric));
                    break;
                case batchCalculations.batchGroups.LANDSCAPE:
                    l.Clear();
                    l.AddRange(Enum.GetNames(typeof(rasterUtil.landscapeType)));
                    l.AddRange(Enum.GetNames(typeof(rasterUtil.focalType)));
                    nm = l.ToArray();
                    break;
                case batchCalculations.batchGroups.AGGREGATION:
                    nm = Enum.GetNames(typeof(rasterUtil.focalType));
                    break;
                case batchCalculations.batchGroups.ZONALSTATS:
                    nm = Enum.GetNames(typeof(rasterUtil.zoneType));
                    break;
                case batchCalculations.batchGroups.SAVEFUNCTIONRASTER:
                    nm = Enum.GetNames(typeof(rasterUtil.rasterType));
                    break;
                case batchCalculations.batchGroups.MOSAIC:
                    l.Clear();
                    l.AddRange(Enum.GetNames(typeof(esriMosaicMethod)));
                    l.AddRange(Enum.GetNames(typeof(rstMosaicOperatorType)));
                    nm = l.ToArray();
                    break;
                case batchCalculations.batchGroups.MERGE:
                    l.Clear();
                    l.AddRange(Enum.GetNames(typeof(rasterUtil.mergeType)));
                    nm = l.ToArray();
                    break;
                case batchCalculations.batchGroups.CONVERTPIXELTYPE:
                    nm = Enum.GetNames(typeof(rstPixelType));
                    break;
                case batchCalculations.batchGroups.FOCALSAMPLE:
                    nm = Enum.GetNames(typeof(rasterUtil.focalType));
                    break;
                case batchCalculations.batchGroups.SURFACE:
                    nm = Enum.GetNames(typeof(rasterUtil.surfaceType));
                    break;
                default:
                    break;
            }
            if (nm != null)
            {
                lsbOptions.Items.AddRange(nm);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            rtbBatch.Text = rtbBatch.Text.Insert(rtbBatch.SelectionStart,btCalc.openBatchFile());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btCalc.saveBatchFile(getBatchLines(rtbBatch.Lines));
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            rp.Show();
            btCalc.FeatureClassDictionary = ftrDic;
            btCalc.RasterDictionary = rstDic;
            btCalc.TableDictionary = tblDic;
            string[] mbtLn = getBatchLines(rtbBatch.Lines);
            btCalc.manuallyAddBatchLines(mbtLn);
            System.Threading.Thread td = new System.Threading.Thread(() => btCalc.runBatch());
            td.Start();
            this.Close();
        }

        private string[] getBatchLines(string[] p)
        {
            List<string> outLst = new List<string>();
            foreach (string s in p)
            {
                string o, f, pr;
                btCalc.getFunctionAndParameters(s, out f, out pr, out o);
                if (o == "" || f == "" || pr == "")
                {
                    continue;
                }
                else
                {
                    string fp = parseParam(pr);
                    outLst.Add(o + "=" + f + "(" + fp + ")");
                }
            }
            return outLst.ToArray();
        }

        private string parseParam(string param)
        {
            string[] pArr = param.Split(new char[] { ';' });
            foreach (string s in pArr)
            {
                foreach( string s2 in s.Split(new char[]{'-'}))
                {
                    string vl = s2.Trim();
                    //Console.WriteLine(vl);
                    if(lyrDic.ContainsKey(vl)&&!rsUtil.isNumeric(vl))
                    {
                        param = param.Replace(vl, lyrDic[vl]);
                    }
                }
            }
            return param;
        }

        private void lsbFunctions_DoubleClick(object sender, EventArgs e)
        {
            string vl = lsbFunctions.SelectedItem.ToString();
            int sStart = rtbBatch.SelectionStart;
            rtbBatch.Text = rtbBatch.Text.Insert(sStart, vl);
            rtbBatch.SelectionStart = sStart + vl.Length;
        }

        private void lsbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string vl = lsbOptions.SelectedItem.ToString();
            int sStart = rtbBatch.SelectionStart;
            rtbBatch.Text = rtbBatch.Text.Insert(sStart, vl);
            rtbBatch.SelectionStart = sStart + vl.Length;
        }

        private void btnsemicolon_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addToBatch(sender);
        }

        private void btnSyntax_Click(object sender, EventArgs e)
        {
            btCalc.syntaxExample((batchCalculations.batchGroups)Enum.Parse(typeof(batchCalculations.batchGroups),lsbFunctions.SelectedItem.ToString()));
        }

    }
}
