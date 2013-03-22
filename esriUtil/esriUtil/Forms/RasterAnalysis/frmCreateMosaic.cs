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
    public partial class frmCreateMosaic : Form
    {
        public frmCreateMosaic(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            
        }
        public frmCreateMosaic(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private bool addToMap = true;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select Rasters";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    IRaster rs = rsUtil.returnRaster(outPath);
                    
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rs);
                        lsbRaster.Items.Add(outName);
                        //cmbInRaster1.Items.Add(nNm);
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                    gxObj = eGxObj.Next();  
                }
            }
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }
        private void populateComboBox()
        {
            if (mp != null)
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
                        rstDic.Add(lyrNm, rst);
                        cmbInRaster1.Items.Add(lyrNm);
                    }
                    else
                    {
                        rstDic[lyrNm] = rst;
                    }
                    lyr = rstLyrs.Next();
                }
            }
            foreach (string s in Enum.GetNames(typeof(rstMosaicOperatorType)))
            {
                if (s != "MT_CUSTOM") cmbType.Items.Add(s);
            }
            cmbType.SelectedItem = rstMosaicOperatorType.MT_FIRST.ToString();
            foreach (string s in Enum.GetNames(typeof(esriMosaicMethod)))
            {
                cmbMethod.Items.Add(s);
            }
            cmbMethod.SelectedItem = esriMosaicMethod.esriMosaicNone.ToString();
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }
        private IWorkspace outWks = null;
        private void getWksPath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select or create a Workspace";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                outWks = geoUtil.OpenWorkSpace(outPath);
                txtWorkspace.Text = outName;

            }
            return;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = txtOutNm.Text;
            esriMosaicMethod mMethod = (esriMosaicMethod)Enum.Parse(typeof(esriMosaicMethod),cmbMethod.Text);
            rstMosaicOperatorType mType = (rstMosaicOperatorType)Enum.Parse(typeof(rstMosaicOperatorType), cmbType.Text);
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify a output name","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (outWks == null)
            {
                MessageBox.Show("You must specify an output workspace", "No Workspace", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbRaster.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on Raster", "No Rasters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<IRaster> rsLst = new List<IRaster>();
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                rsLst.Add(rstDic[lsbRaster.Items[i].ToString()]);
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Building Mosaic Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.Show();
            rp.TopMost = true;
            if (chbOverview.Checked)
            {
                rp.addMessage("Building overviews will take additional time...");
            }
            try
            {
                outraster = rsUtil.mosaicRastersFunction(outWks,rstNm,rsLst.ToArray(),mMethod,mType,chbFootprint.Checked,chbBoundary.Checked,chbSeamlines.Checked,chbOverview.Checked);
                rp.addMessage("Adding mosaic dataset to the map");
                if (mp != null&&addToMap)
                {
                    rp.Show();
                    rp.Refresh();
                    IMosaicLayer rstLyr = new MosaicLayerClass();
                    rstLyr.CreateFromMosaicDataset((IMosaicDataset)((IRaster2)outraster).RasterDataset);
                    mp.AddLayer((ILayer)rstLyr);
                }
                outrastername = rstNm;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished Mosaic Raster" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection s = lsbRaster.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmbInRaster1.Items.Contains(txt))
                    {
                        cmbInRaster1.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lsbRaster.Items.Remove(r);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                cmbInRaster1.Items.Add(lsbRaster.Items[i]);
            }
            lsbRaster.Items.Clear();

        }

        private void cmbInRaster1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object itVl = cmbInRaster1.SelectedItem;
            lsbRaster.Items.Add(itVl);
            cmbInRaster1.Items.Remove(itVl);
        }

        private void btnWorkspace_Click(object sender, EventArgs e)
        {
            getWksPath();
        }
    }
}
