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

namespace esriUtil.Forms.Texture
{
    public partial class frmCreateGlcmSurface : Form
    {
        public frmCreateGlcmSurface(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            rstUtil = new rasterUtil();
            populateComboBox();
        }
        public frmCreateGlcmSurface(IMap map, ref rasterUtil rasterUtility, bool addToMap)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            rstUtil = rasterUtility;
            populateComboBox();
            perm = addToMap;
        }
        private IRaster rst = null;
        private string oNm = null;
        public IRaster OutRaster { get { return rst; } }
        public string OutRasterName { get { return oNm; } }
        private bool perm = true; 
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rstUtil = null;
        //private glcm glcmTexture = null;
        private viewUtility vUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
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
            foreach (string s in Enum.GetNames(typeof(rasterUtil.windowType)))
            {
                cmbWindowType.Items.Add(s);
            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.glcmMetric)))
            {
                cmbGlcmTypes.Items.Add(s);
            }
        }
        private void getRasterPath()
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
                    rstDic.Add(outName, rstUtil.returnRaster(outPath));
                    cmbRaster.Items.Add(outName);
                }
                else
                {
                    rstDic[outName] = rstUtil.returnRaster(outPath);
                }
                cmbRaster.Text = outName;
            }
            return;
        }

        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Add(rstName);
                rstDic.Add(rstName, rst);
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
            getRasterPath();
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

        

        

        
        private void btnCreate_Click(object sender, EventArgs e)
        {
            createRaster();
        }
        private void createRaster()
        {
            string rstNm = cmbRaster.Text;
            string rstOut = mtbOutName.Text.Trim();
            string windType = cmbWindowType.Text;
            string windDir = cmbDirections.Text;
            string glcmType = cmbGlcmTypes.Text;
            int clms = System.Convert.ToInt32(nudColumns.Value);
            int rows = System.Convert.ToInt32(nudRows.Value);
            List<rasterUtil.glcmMetric> glcmArr = new List<rasterUtil.glcmMetric>();
            if (rstNm == "" || rstNm == null||rstOut==""||rstOut==null)
            {
                MessageBox.Show("You must have both a raster and output name specified to use this tool!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (windType == null || windType == "" || windDir == "" || windDir == null)
            {
                MessageBox.Show("Window Type and Direction are not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (glcmType == null || glcmType == "" || glcmType == "" || glcmType == null)
            {
                MessageBox.Show("Glcm Type is not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool horz = true;
            rasterUtil.windowType wt = (rasterUtil.windowType)Enum.Parse(typeof(rasterUtil.windowType),windType);
            rasterUtil.glcmMetric gm = (rasterUtil.glcmMetric)Enum.Parse(typeof(rasterUtil.glcmMetric),glcmType);
            if (windDir.ToUpper() != "HORIZONTAL")
            {
                horz = false;
            }
            bool radius = false;
            if(wt!= rasterUtil.windowType.RECTANGLE)
            {
                radius = true;

            }

            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog((perm==false));
            System.DateTime t1 = System.DateTime.Now;
            rp.addMessage("Start date and time = " + t1.Month.ToString() + "\\" + t1.Day.ToString() + "\\" + t1.Year.ToString() + " " + t1.Hour.ToString() + ":" + t1.Minute.ToString() + ":" + t1.Second.ToString());
            rp.addMessage("Creating the following " + windDir + " GLCM surfaces for the specified Raster:");
            rp.stepPGBar(15);
            if (perm)
            {
                rp.Show();
            }
            rp.Refresh();
            rp.TopMost = true;
            foreach (rasterUtil.glcmMetric g in glcmArr)
            {
                rp.addMessage("\t" + g.ToString());
            }
            rp.Refresh();
            IRaster inRs = rstDic[rstNm];
            try
            {
                if(radius)
                {
                    rst = rstUtil.calcGLCMFunction(inRs,clms,horz,gm);
                }
                else
                {

                    rst = rstUtil.calcGLCMFunction(inRs, clms, rows, horz, gm);
                }
                if (mp != null&&perm)
                {
                    IRasterLayer rsLyr = new RasterLayerClass();
                    //rstUtil.calcStatsAndHist(rst);
                    rsLyr.CreateFromRaster(rst);
                    rsLyr.Name = rstOut;
                    rsLyr.Visible = false;
                    mp.AddLayer((ILayer)rsLyr);
                }
                System.DateTime t2 = System.DateTime.Now;
                rp.addMessage("End date and time = " + t2.Month.ToString() + "\\" + t2.Day.ToString() + "\\" + t2.Year.ToString() + " " + t2.Hour.ToString() + ":" + t2.Minute.ToString() + ":" + t2.Second.ToString());
                rp.stepPGBar(100);
                rp.Refresh();
                oNm = rstOut;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.enableClose();
                this.Close();
            }
            return;
        }
    }
}
