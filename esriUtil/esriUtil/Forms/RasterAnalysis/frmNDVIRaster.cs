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
    public partial class frmNDVIRaster : Form
    {
        public frmNDVIRaster(IMap map)
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
        public frmNDVIRaster(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Raster";
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
                cmbInRaster1.Text = outName;


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
                    lyr = rstLyrs.Next();

                }
            }
        }
        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            string rstNm = cmbInRaster1.Text;
            string outNm = txtOutName.Text;
            string visStr = cmbVis.Text;
            string irStr = cmbIr.Text;
            if (visStr == "" || visStr == null || rstNm == "" || rstNm == null||irStr==""||irStr==null)
            {
                MessageBox.Show("You must have a raster layer selected and a valid visual and ir band index value", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outNm == "" || outNm == null)
            {
                MessageBox.Show("You must specify an output raster name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            int visVl = System.Convert.ToInt32(visStr)-1;
            int irVl = System.Convert.ToInt32(irStr)-1;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Making Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                IRaster rst = rstDic[rstNm];
                outraster = rsUtil.calcNDVIFunction(rst, visVl, irVl);
                if (mp != null && addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromRaster(outraster);
                    rstLyr.Name = outNm;
                    rstLyr.Visible = false;
                    mp.AddLayer(rstLyr);
                }
                outrastername = outNm;
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
                rp.addMessage("Finished Making Raster" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void cmbInRaster1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rsStr = cmbInRaster1.Text;
            if(rstDic.ContainsKey(rsStr))
            {
                IRasterBandCollection rsBc = (IRasterBandCollection)rstDic[rsStr];
                cmbVis.Items.Clear();
                cmbIr.Items.Clear();
                object[] itm = Enumerable.Range(1, rsBc.Count).Cast<object>().ToArray();
                cmbVis.Items.AddRange(itm);
                cmbIr.Items.AddRange(itm);
                if (itm.Length > 3)
                {
                    cmbVis.SelectedItem = itm[2];
                    cmbIr.SelectedItem = itm[3];
                }
                else
                {
                    cmbVis.SelectedItem = itm[0];
                    cmbIr.SelectedItem = itm[0];
                }
            }
        }

        
    }
}

