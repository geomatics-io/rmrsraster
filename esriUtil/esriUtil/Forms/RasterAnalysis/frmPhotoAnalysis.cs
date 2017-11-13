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
    public partial class frmPhotoAnalysis : Form
    {
        public frmPhotoAnalysis(IMap mp, rasterUtil rasterUtility = null)
        {
            InitializeComponent();
            frmHelper = new frmHelper(mp, rasterUtility);
            rsUtil = frmHelper.RasterUtility;
            geoUtil = frmHelper.GeoUtility;
            fillCombo();
        }
        frmHelper frmHelper = null;
        rasterUtil rsUtil = null;
        geoDatabaseUtility geoUtil = null;
        string outCsvPath = "";
        string ftrClsPath = null;
        string[] inDirPath;
        string[] ftrClsName;
        string[] inDirName;
        photoAnalysis.extensionType ext = photoAnalysis.extensionType.jpg;

        private void fillCombo()
        {
            cmbROI.Items.Clear();
            foreach (string s in frmHelper.FeatureDictionary.Keys)
            {
                IFeatureClass ftrCls = frmHelper.FeatureDictionary[s];
                if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    cmbROI.Items.Add(s);
                }
            }
            cmbExt.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(photoAnalysis.extensionType)))
            {
                cmbExt.Items.Add(s);
            }
            cmbExt.SelectedItem = "jpg";
        }
        private void btnDir_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            inDirPath = frmHelper.getPath(flt, out inDirName);
            txtInDir.Text = inDirPath[0];
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            SaveFileDialog sFd = new SaveFileDialog();
            sFd.Filter = "CSV | *.csv";
            if (sFd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outCsvPath = sFd.FileName;
                txtOutDir.Text = outCsvPath;
            }
        }

        private void btnROI_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            ftrClsPath = frmHelper.getPath(flt, out ftrClsName)[0];
            frmHelper.FeatureDictionary[ftrClsName[0]] = geoUtil.getFeatureClass(ftrClsPath);
            cmbROI.Items.Add(ftrClsName[0]);
            cmbROI.SelectedItem = ftrClsName[0];
            
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string inp = txtInDir.Text;
            string outfl = txtOutDir.Text;
            string polyPath = cmbROI.Text;
            string extStr = cmbExt.Text;
            if (inp == null || inp == "")
            {
                MessageBox.Show("No Image Directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outfl == null || outfl == "")
            {
                MessageBox.Show("No Output CSV file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (polyPath == null || polyPath == "")
            {
                MessageBox.Show("No ROI Selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (extStr == null || extStr == "")
            {
                MessageBox.Show("No image extension selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ext = (photoAnalysis.extensionType)Enum.Parse(typeof(photoAnalysis.extensionType), extStr);
            IFeatureClass ftrCls = frmHelper.FeatureDictionary[polyPath];
            IDataset dSet = (IDataset)ftrCls;
            string fPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rdp = new RunningProcess.frmRunningProcessDialog(false);
            rdp.addMessage("Running photo analysis...");
            rdp.TopMost = true;
            rdp.Show();
            DateTime dt1 = DateTime.Now;
            try
            {
                photoAnalysis photoA = new photoAnalysis(inDirPath[0], fPath, outCsvPath, ext);
                photoA.runAnalysis();
            }
            catch (Exception exc)
            {
                rdp.addMessage(exc.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                rdp.addMessage("Finished photo analysis in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds");
                rdp.stepPGBar(100);
                rdp.enableClose();
                this.Close();
            }

            
        }

    }
}
