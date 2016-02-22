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
    public partial class frmCalcGridMetricsFusion : Form
    {
        public frmCalcGridMetricsFusion(ESRI.ArcGIS.Carto.IMap mp, rasterUtil rasterUtility = null)
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
            frmHlp = new frmHelper(mp,rsUtil);
        }

        
        private fusionIntegration fsInt;
        private rasterUtil rsUtil;
        private frmHelper frmHlp;
        
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

        private void btnOutDir_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] flNames;
            string[] flPath = frmHlp.getPath(flt, out flNames, false);
            if (flPath.Length > 0)
            {
                txtOutDir.Text = flPath[0];
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string lasDirPath = txtLasDir.Text;
            string outDirPath = txtOutDir.Text;
            int cellSize = System.Convert.ToInt32(nudCellSize.Value);
            double cutHeight = 0;
            string dtmDirPath = txtDTM.Text;
            bool dtmExists = true;
            if (lasDirPath == "" || lasDirPath == null)
            {
                System.Windows.Forms.MessageBox.Show("No las directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outDirPath == "" || outDirPath == null)
            {
                System.Windows.Forms.MessageBox.Show("No output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dtmDirPath == "" || dtmDirPath == null)
            {
                dtmExists=false;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Running Fusion's Grid Metrics. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                if (dtmExists)
                {
                    fsInt.RunGridMetrics(lasDirPath, outDirPath, cellSize, cutHeight,dtmDirPath,System.Convert.ToDouble(nudCuttOffBelow.Value),System.Convert.ToDouble(nudCuttOffAbove.Value));
                }
                else
                {
                    fsInt.RunGridMetrics(lasDirPath, outDirPath, cellSize, cutHeight);
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
                rp.addMessage("Finished creating metrics in" + t);
                rp.enableClose();
                this.Close();
            }
            
        }

        private void btnDtm_Click(object sender, EventArgs e)
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
