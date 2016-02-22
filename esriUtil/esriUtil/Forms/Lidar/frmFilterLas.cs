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
    public partial class frmFilterLas : Form
    {
        public frmFilterLas(ESRI.ArcGIS.Carto.IMap mp, rasterUtil rasterUtility = null)
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

        private void btnDtm_Click(object sender, EventArgs e)
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
            string lasDir = txtLasDir.Text;
            string gLasDir = txtOutDir.Text;
            int cellSize = System.Convert.ToInt32(nudCellSize.Value);
            if (!System.IO.Directory.Exists(lasDir))
            {
                MessageBox.Show("Could not find the LAS directory!", "Error", MessageBoxButtons.OK);
                return;
            }
            if (!System.IO.Directory.Exists(gLasDir))
            {
                MessageBox.Show("Could not find the Ground LAS directory!", "Error", MessageBoxButtons.OK);
                return;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Running ground Filter. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {

                fsInt.RunGroundFilter(lasDir,gLasDir,cellSize);
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
                rp.addMessage("Finished creating filtered las files in" + t);
                rp.enableClose();
                this.Close();
            }
        }
    }
}
