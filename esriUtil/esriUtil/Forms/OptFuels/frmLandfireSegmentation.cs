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

namespace esriUtil.Forms.OptFuels
{
    public partial class frmLandfireSegmentation : Form
    {
        public frmLandfireSegmentation()
        {
            InitializeComponent();
            lf = new landFire(rsUtil);
        }

        private rasterUtil rsUtil = new rasterUtil();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private landFire lf = null;
        private void btnOpenWorkspace_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterWorkspacesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Workspace";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                txtLandFireDir.Text = gxObj.FullName;
            }
        }

        private void btnSegment_Click(object sender, EventArgs e)
        {
            int CovVl = trbCover.Value;
            int HtVl = trbHeight.Value;
            bool Asp = chbAspect.Checked;
            double minArea = System.Convert.ToDouble(nudMin.Value);
            double maxArea = System.Convert.ToDouble(nudMax.Value);
            string wksPath = txtLandFireDir.Text;
            if (wksPath == "" || wksPath == null)
            {
                MessageBox.Show("You must select a workspace that has LandFire Data");
                return;
            }
            IWorkspace wks = geoUtil.OpenWorkSpace(wksPath);
            RunningProcess.frmRunningProcessDialog rd = new RunningProcess.frmRunningProcessDialog(false);
            rd.Show(this);
            DateTime dt1 = DateTime.Now;
            try
            {
                rd.addMessage("Segmenting LandFireData...");
                rd.stepPGBar(25);
                rd.Refresh();
                lf.LandFireWorkspace = wks;
                lf.CoverLevels = CovVl;
                lf.HeightLevels = HtVl;
                lf.UseAspect = Asp;
                lf.MaxArea = maxArea;
                lf.MinArea = minArea;
                rd.addMessage("Performing Raster Analyses");
                rd.Refresh();
                lf.segmentLandFireData();
                rd.addMessage("Converting to Polygons");
                rd.Refresh();
                lf.convertToPolygon();
                rd.addMessage("Splitting Polygons");
                lf.splitPolygons(false);
            }
            catch (Exception ex)
            {

                rd.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                rd.addMessage("Finished Segmenting LandFire Data in " + ts.TotalMinutes.ToString() + " minutes.");
                rd.Refresh();
                rd.stepPGBar(100);
                rd.enableClose();
                
            }
            
        }
    }
}
