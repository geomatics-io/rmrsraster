using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmRemapRaster : Form
    {
        public frmRemapRaster(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        public frmRemapRaster(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        private IMap mp = null;
        private bool addToMap = true;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
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
        private void getFeaturePath(Control cmb)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            gxDialog.ObjectFilter = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!rstDic.ContainsKey(outName))
                {
                    rstDic.Add(outName, rsUtil.returnRaster(outPath));
                    cmbInRaster1.Items.Add(outName);
                }
                else
                {
                    rstDic[outName] = rsUtil.returnRaster(outPath);
                }
                cmb.Text = outName;

            }
            return;
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
                    lyr = rstLyrs.Next();

                }
            }
            dgvRanges.RowCount = 1;
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath((Control)cmbInRaster1);
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            dgvRanges.RowCount = dgvRanges.RowCount + 1;

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            dgvRanges.RowCount = dgvRanges.RowCount - 1;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvRanges.Rows.Clear();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = txtOutNm.Text;
            string rstIn = cmbInRaster1.Text;
            if (rstNm == null || rstNm == ""||rstIn==null||rstIn=="")
            {
                MessageBox.Show("You must specify a output name", "No Output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dgvRanges.Rows.Count < 1)
            {
                MessageBox.Show("You must have at least on range of values selected", "No Ranges", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IRemapFilter rfilt = new RemapFilterClass();
            for (int i = 0; i < dgvRanges.Rows.Count; i++)
            {
                object obj1 = dgvRanges[0, i].Value;
                object obj2 = dgvRanges[1, i].Value;
                object obj3 = dgvRanges[2, i].Value;
                if ( Convert.IsDBNull(obj1)) obj1 = 0;
                if (Convert.IsDBNull(obj2)) obj2 = 0;
                if (Convert.IsDBNull(obj3)) obj3 = 0;
                double min = System.Convert.ToDouble(obj1);
                double max = System.Convert.ToDouble(obj2);
                double vl = System.Convert.ToDouble(obj3);
                rfilt.AddClass(min,max,vl);
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Remapping Rasters. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                outraster = rsUtil.calcRemapFunction(rstDic[rstIn],rfilt);
                if (mp != null && addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromRaster(outraster);
                    rstLyr.Name = rstNm;
                    rstLyr.Visible = false;
                    mp.AddLayer(rstLyr);
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
                rp.addMessage("Finished Remapping Rasters" + t);
                rp.enableClose();
                this.Close();
            }

        }

    }
}