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
    public partial class frmConditionalRaster : Form
    {
        public frmConditionalRaster(IMap map)
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
        public frmConditionalRaster(IMap map, ref rasterUtil rasterUtility, bool addToMap)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            rsUtil = rasterUtility;
            aM = addToMap;
        }
        private IMap mp = null;
        private bool aM = true;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private Dictionary<string, rasterUtil.logicalType> oprDic = new Dictionary<string, rasterUtil.logicalType>();
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
                        cmbInRaster1.Items.Add(lyrNm);
                        cmbInRaster2.Items.Add(lyrNm);
                        cmbInRaster3.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
         }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (! cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Add(rstName);
                cmbInRaster2.Items.Add(rstName);
                cmbInRaster3.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Remove(rstName);
                cmbInRaster2.Items.Remove(rstName);
                cmbInRaster3.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }
        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            ComboBox cmb = null;
            switch (((Button)sender).Name.ToLower())
            {
                case "btninraster1":
                    cmb = cmbInRaster1;
                    break;
                case "btninraster2":
                    cmb = cmbInRaster2;
                    break;
                default:
                    cmb = cmbInRaster3;
                    break;
            }
            getRasterPath(cmb);
        }
        private void btnExecute_Click(object sender, EventArgs e)
        {
            string inRst1Nm = cmbInRaster1.Text;
            string inRst2Nm = cmbInRaster2.Text;
            string inRst3Nm = cmbInRaster3.Text;
            string outNmRst = txtOutName.Text;
            if (inRst1Nm == "" || inRst1Nm == null)
            {
                MessageBox.Show("You must specify an input raster for In Raster 1","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (inRst2Nm == "" || inRst2Nm == null)
            {
                MessageBox.Show("You must specify an input raster for In Raster 2 or type in a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (inRst3Nm == "" || inRst3Nm == null)
            {
                MessageBox.Show("You must specify an input raster for In Raster 3 or type in a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outNmRst == "" || outNmRst == null)
            {
                MessageBox.Show("You must specify an output raster name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            object rs1,rs2,rs3;
            if(rstDic.ContainsKey(inRst1Nm))
            {
                rs1 = rstDic[inRst1Nm];
            }
            else
            {
                MessageBox.Show("You must specify an input raster for In Raster 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(rstDic.ContainsKey(inRst2Nm))
            {
                rs2 = rstDic[inRst2Nm];
            }
            else if(rsUtil.isNumeric(inRst2Nm))
            {
                rs2 = inRst2Nm;
                
            }
            else
            {
                MessageBox.Show("You must specify an input raster for In Raster 2 or type in a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (rstDic.ContainsKey(inRst3Nm))
            {
                rs3 = rstDic[inRst3Nm];
            }
            else if (rsUtil.isNumeric(inRst3Nm))
            {
                rs3 = inRst3Nm;

            }
            else
            {
                MessageBox.Show("You must specify an input raster for In Raster 3 or type in a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Creating Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                outraster = rsUtil.conditionalRasterFunction(rs1, rs2, rs3);
                outrastername = outNmRst;
                if (mp != null && aM)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    //rsUtil.calcStatsAndHist(outraster);
                    IRasterLayer rsLyr = new RasterLayerClass();
                    rsLyr.CreateFromRaster(outraster);
                    rsLyr.Name = outrastername;
                    rsLyr.Visible = false;
                    mp.AddLayer((ILayer)rsLyr);
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
                rp.addMessage("Finished Conditional Analysis" + t);
                rp.enableClose();
                this.Close();
            }
        }
    }
}
