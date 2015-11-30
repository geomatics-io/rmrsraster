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
    public partial class frmSaveParallel : Form
    {
        public frmSaveParallel(IMap map)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            geoUtil = frmHlp.GeoUtility;
            ftrUtil = frmHlp.FeatureUtility;
            fillCmb();
        }
        private frmHelper frmHlp = null;
        private rasterUtil rsUtil = null;
        private featureUtil ftrUtil = null;
        private geoDatabaseUtility geoUtil = null;
        private void fillCmb()
        {
            cmbType.Items.Add(rasterUtil.rasterType.IMAGINE.ToString());
            cmbType.Items.Add(rasterUtil.rasterType.TIFF.ToString());
            cmbType.Items.Add(rasterUtil.rasterType.GRID.ToString());
            cmbType.Items.Add(rasterUtil.rasterType.BMP.ToString());
            cmbType.Items.Add(rasterUtil.rasterType.RST.ToString());
            cmbType.SelectedItem = rasterUtil.rasterType.IMAGINE.ToString();
            foreach (string s in frmHlp.FeatureDictionary.Keys)
            {
                IFeatureClass ftrCls = frmHlp.FeatureDictionary[s];
                if (ftrCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    cmbFtrCls.Items.Add(s);
                }
            }

        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog oFd = new OpenFileDialog();
            oFd.Filter = "Batch|*.bch";
            oFd.Multiselect = false;
            if (oFd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtRasterPath.Text = oFd.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            string[] outName;
            string outPath = frmHlp.getPath(flt, out outName, false)[0];
            frmHlp.FeatureDictionary[outName[0]] = geoUtil.getFeatureClass(outPath);
            cmbFtrCls.Items.Add(outName[0]);
            cmbFtrCls.SelectedItem = outName[0];
        }

        private void btnOutWks_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] outName;
            string outPath = frmHlp.getPath(flt, out outName, false)[0];
            txtOutWks.Text = outPath;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string rsStr = txtRasterPath.Text;
            string ftrStr = cmbFtrCls.Text;
            string wksStr = txtOutWks.Text;
            string prefixStr = txtOutName.Text;
            string noDataVlStr = txtNoDataVl.Text;
            int blockSize = System.Convert.ToInt32(nudBS.Value);
            string typeStr = cmbType.Text;
            if (rsStr == null || rsStr == "")
            {
                MessageBox.Show("No raster selected");
                return;
            }
            if (ftrStr == null || ftrStr == "")
            {
                MessageBox.Show("No tiled feature class selected");
                return;
            }
            if (wksStr == null || wksStr == "")
            {
                MessageBox.Show("No workspace selected");
                return;
            }
            IFeatureClass ftrCls = frmHlp.FeatureDictionary[ftrStr];
            IDataset dSet = (IDataset)ftrCls;
            string ftrPath = dSet.Workspace.PathName +"\\"+ dSet.BrowseName;
            string fLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string exe = "\\saveSubset.exe";
            string exePath = fLoc + exe;
            if (noDataVlStr == "") noDataVlStr = "null";
            try
            {
                string cmd = rsStr + " " + prefixStr + " " + wksStr + " " + typeStr + " " + ftrPath + " " + noDataVlStr + " " + blockSize.ToString() + " " + blockSize.ToString();
                System.Diagnostics.Process pr = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo stInfo = pr.StartInfo;
                stInfo.FileName = exePath;
                stInfo.Arguments = cmd;
                pr.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Close();
            }
        }
    }
}
