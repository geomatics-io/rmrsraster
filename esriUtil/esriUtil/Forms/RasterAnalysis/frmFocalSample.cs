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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmFocalSample : Form
    {
        public frmFocalSample(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            rsUtil = new rasterUtil();
        }
        public frmFocalSample(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
        }
        private IMap mp = null;
        private bool addToMap = true;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private void getRasterPath(ComboBox cntr)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
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
                    cntr.Items.Add(outName);
                }
                else
                {
                    rstDic[outName] = rsUtil.returnRaster(outPath);
                }
                cntr.Text = outName;
            }
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster{get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
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
                        cmbRaster.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.focalType)))
            {
                cmbFocalStat.Items.Add(s);
            }
            cmbFocalStat.SelectedItem = rasterUtil.focalType.SUM.ToString();

        }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }
        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getRasterPath(cmbRaster);
        }
        private void btnExecute_Click(object sender, EventArgs e)
        {
            string inRst1Nm = cmbRaster.Text;
            string inFocalStat = cmbFocalStat.Text;
            string outNmRst = txtOutName.Text;
            if (inRst1Nm == "" || inRst1Nm == null)
            {
                MessageBox.Show("You must specify an input raster for In Rasterr","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (inFocalStat == "" || inFocalStat == null)
            {
                MessageBox.Show("You must specify an Focal Statistic", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (outNmRst == "" || outNmRst == null)
            {
                MessageBox.Show("You must specify an output raster name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            HashSet<string> offsets = new HashSet<string>();
            for (int i = 0; i < dgvAzDs.RowCount; i++)
            {
                string az = dgvAzDs[0, i].Value.ToString();
                string ds = dgvAzDs[1, i].Value.ToString();
                offsets.Add(az + ";" + ds);
                
            }
            if (offsets.Count < 1)
            {
                MessageBox.Show("Nothing specified for offset!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IRaster rs1 = rstDic[inRst1Nm];
            IRaster outRs = null;
            rasterUtil.focalType fcType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), inFocalStat);
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Creating Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                outRs = rsUtil.calcFocalSampleFunction(rs1, offsets, fcType);
                outraster = outRs;
                outrastername = outNmRst;
                if (mp != null && addToMap)
                {
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rsLyr = new RasterLayerClass();
                    rsLyr.CreateFromRaster(outraster);
                    rsLyr.Name = OutRasterName;
                    rsLyr.Visible = false;
                    mp.AddLayer(rsLyr);
                }
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
                rp.addMessage("Finished Focal Sample Analysis" + t);
                rp.enableClose();
                this.Close();
            }
        }
        private void btnPlus_Click(object sender, EventArgs e)
        {
            dgvAzDs.RowCount = dgvAzDs.RowCount + 1;
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            dgvAzDs.RowCount = dgvAzDs.RowCount - 1;
        }
    }
}
