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
    public partial class frmSaveRaster : Form
    {
        public frmSaveRaster(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }

        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private IWorkspace outWks = null;
        private void getFeaturePath(Control cmb, ESRI.ArcGIS.Catalog.IGxObjectFilter flt)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (cmb.Name.ToLower() == "cmbraster")
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.returnRaster(outPath));
                        cmbRaster.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.returnRaster(outPath);
                    }
                }
                else
                {
                    outWks = geoUtil.OpenWorkSpace(outPath);
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
                        cmbRaster.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
        }

        private void fillCmbType()
        {
            string wksTxt = txtWorkspace.Text;
            cmbType.Items.Clear();
            if (wksTxt == null || wksTxt == ""||outWks==null)
            {
                return;
            }
            
            esriWorkspaceType tp = outWks.Type;
            if (tp == esriWorkspaceType.esriFileSystemWorkspace)
            {
                foreach (string s in Enum.GetNames(typeof(rasterUtil.rasterType)))
                {
                    if (s != rasterUtil.rasterType.GDB.ToString())
                    {
                        cmbType.Items.Add(s);
                    }
                }
                cmbType.SelectedItem = rasterUtil.rasterType.GRID.ToString();
            }
            else
            {
                cmbType.Items.Add(rasterUtil.rasterType.GDB.ToString());
                cmbType.SelectedItem = rasterUtil.rasterType.GDB.ToString();
            }
            
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath((Control)cmbRaster,new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string rstNm = cmbRaster.Text;
            string wksNm = txtWorkspace.Text;
            string outNm = txtOutName.Text;
            string outType = cmbType.Text;
            if (outWks == null || wksNm == "" || wksNm == null || rstNm == "" || rstNm == null || outNm==""||outNm==null)
            {
                MessageBox.Show("Raster, Workspace, or Output Name are not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            rasterUtil.rasterType rType = (rasterUtil.rasterType)Enum.Parse(typeof(rasterUtil.rasterType), outType);
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Saving raster");
            rp.addMessage("This may take a while...");
            rp.stepPGBar(20);
            rp.Show();
            rp.Refresh();
            
            DateTime dt1 = DateTime.Now;
            try
            {
                IRaster rs = rstDic[rstNm];
                IRasterDataset rsDset = rsUtil.saveRasterToDataset(rs, outNm, outWks, rType);
                DateTime dt2 = DateTime.Now;
                if (MessageBox.Show("Do you want to add raster to current map?", "Add Raster", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    IRasterLayer rsLyr = new RasterLayerClass();
                    rsLyr.CreateFromDataset(rsDset);
                    rsLyr.Name = rstNm;
                    rsLyr.Visible = false;
                    mp.AddLayer((ILayer)rsLyr);
                }

                TimeSpan ts = dt2.Subtract(dt1);
                string prcTime = "Time to complete process:\n" + ts.Days.ToString() + " Days " + ts.Hours.ToString() + " Hours " + ts.Minutes.ToString() + " Minutes " + ts.Seconds.ToString() + " Seconds ";
                rp.addMessage(prcTime);
                this.DialogResult = DialogResult.OK;


            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.enableClose();
                this.Close();
            }
                

        }

        private void btnWorkspace_Click(object sender, EventArgs e)
        {
            getFeaturePath((Control)txtWorkspace, new ESRI.ArcGIS.Catalog.GxFilterWorkspacesClass());
        }

        private void txtWorkspace_TextChanged(object sender, EventArgs e)
        {
            fillCmbType();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        
    }
}
