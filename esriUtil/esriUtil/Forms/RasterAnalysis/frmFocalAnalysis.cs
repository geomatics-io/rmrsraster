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
    public partial class frmFocalAnalysis : Form
    {
        public frmFocalAnalysis(IMap map)
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
        public frmFocalAnalysis(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
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
            foreach (string s in Enum.GetNames(typeof(rasterUtil.windowType)))
            {
                cmbWindowType.Items.Add(s);
            }
            cmbWindowType.SelectedItem = rasterUtil.windowType.RECTANGLE.ToString();
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
        private void cmbWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWindowType.Text.ToUpper() == "CIRCLE")
            {
                nudRows.Visible = false;
                lblRows.Visible = false;
                lblColumns.Text = "Radius";
            }
            else
            {
                nudRows.Visible = true;
                lblRows.Visible = true;
                lblColumns.Text = "Columns";
            }
        }
        private void btnExecute_Click(object sender, EventArgs e)
        {
            string inRst1Nm = cmbRaster.Text;
            string inFocalStat = cmbFocalStat.Text;
            string windowType = cmbWindowType.Text;
            string outNmRst = txtOutName.Text;
            int clms = System.Convert.ToInt32(nudColumns.Value);
            int rws = System.Convert.ToInt32(nudRows.Value);
            if (inRst1Nm == "" || inRst1Nm == null)
            {
                MessageBox.Show("You must specify an input raster for In Raster 1 or type in a number","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (inFocalStat == "" || inFocalStat == null)
            {
                MessageBox.Show("You must specify an Focal Statistic", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (windowType == "" || windowType == null)
            {
                MessageBox.Show("You must select a widow type from dropdown menu", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outNmRst == "" || outNmRst == null)
            {
                MessageBox.Show("You must specify an output raster name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IRaster rs1 = rstDic[inRst1Nm];
            IRaster outRs = null;
            rasterUtil.windowType wdType = (rasterUtil.windowType)Enum.Parse(typeof(rasterUtil.windowType), windowType);
            rasterUtil.focalType fcType = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType), inFocalStat);
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Creating Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                if (wdType == rasterUtil.windowType.CIRCLE)
                {
                    outRs = rsUtil.calcFocalStatisticsFunction(rs1, clms, fcType);
                }
                else
                {
                    outRs = rsUtil.calcFocalStatisticsFunction(rs1, clms, rws, fcType);
                }
                outraster = outRs;
                outrastername = outNmRst;
                if (mp != null && addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rsLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
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
                rp.addMessage("Finished Focal Analysis" + t);
                rp.enableClose();
                this.Close();
            }
        }
    }
}
