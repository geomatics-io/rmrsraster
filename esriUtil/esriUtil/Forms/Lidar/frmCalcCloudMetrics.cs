using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.Forms.Lidar
{
    public partial class frmCalcCloudMetrics : Form
    {
        public frmCalcCloudMetrics(ESRI.ArcGIS.Carto.IMap mp, rasterUtil rasterUtility = null)
        {
            InitializeComponent();
            if (rasterUtility == null)
            {
                rsUtil = new rasterUtil();
            }
            else
            {
                rsUtil = rasterUtility;
            }
            fsInt = new fusionIntegration(rsUtil);
            frmHlp = new frmHelper(mp, rsUtil);
            populateCombo();
        }

        private fusionIntegration fsInt;
        private rasterUtil rsUtil;
        private frmHelper frmHlp;

        private void populateCombo()
        {
            foreach (KeyValuePair<string,ESRI.ArcGIS.Geodatabase.IFeatureClass> kvp in frmHlp.FeatureDictionary)
            {
                string nm = kvp.Key;
                ESRI.ArcGIS.Geodatabase.IFeatureClass ftrCls = kvp.Value;
                if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                {
                    cmbSample.Items.Add(nm);
                }
            }
        }
        private void btnOpenLasDir_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] flNames;
            string[] flPath = frmHlp.getPath(flt, out flNames, false);
            if (flPath.Length > 0)
            {
                txtLasDir.Text = flPath[0];
            }
        }


        private void btnExecute_Click(object sender, EventArgs e)
        {
            string lasDirPath = txtLasDir.Text;
            string ftrName = cmbSample.Text;
            string dtmDirPath = txtDTM.Text;
            float radius = System.Convert.ToSingle(nudRadius.Value);
            bool hasDtmDir = true;
            string shapeStr  = cmbShape.Text;
            int shapeVl = 0;
            if(shapeStr=="Circle") shapeVl=1;
            if (lasDirPath == "" || lasDirPath == null)
            {
                System.Windows.Forms.MessageBox.Show("No las directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (ftrName == "" || ftrName == null)
            {
                System.Windows.Forms.MessageBox.Show("No output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dtmDirPath == "" || dtmDirPath == null)
            {
                hasDtmDir = false;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Running Fusion's Cloud Metrics. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                ESRI.ArcGIS.Geodatabase.IFeatureClass ftrCls = frmHlp.FeatureDictionary[ftrName];
                if (hasDtmDir)
                {
                    fsInt.RunCloudMetrics(ftrCls, radius, lasDirPath, dtmDirPath, System.Convert.ToDouble(nudCuttOffBelow.Value), System.Convert.ToDouble(nudCuttOffAbove.Value));
                }
                else
                {
                    fsInt.RunCloudMetrics(ftrCls, radius,lasDirPath,shape:shapeVl);
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
                rp.addMessage("Finished sampling in" + t);
                rp.enableClose();
                this.Close();
            }

        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private void btnSample_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
            string[] flNames;
            string[] flPath = frmHlp.getPath(flt, out flNames, false);
            if (flPath.Length > 0)
            {
                cmbSample.Items.Add(flNames[0]);
                ESRI.ArcGIS.Geodatabase.IFeatureClass ftrCls = geoUtil.getFeatureClass(flPath[0]);
                frmHlp.FeatureDictionary.Add(flNames[0], ftrCls);
                cmbSample.SelectedItem = flNames[0];
            }
        }

        private void dtm_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] flNames;
            string[] flPath = frmHlp.getPath(flt, out flNames, false);
            if (flPath.Length > 0)
            {
                txtDTM.Text = flPath[0];
            }
        }
    }
}
