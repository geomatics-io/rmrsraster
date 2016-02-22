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
    public partial class frmConvertGridMetricsToRaster : Form
    {
        public frmConvertGridMetricsToRaster(ESRI.ArcGIS.Carto.IMap mp, rasterUtil rasterUtility = null)
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
            cmbMetrics.Items.AddRange(fsInt.MetricsArr);
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
            string MetricsDirPath = txtMetricsDir.Text;
            string outDirPath = txtOutDir.Text;
            
            if (MetricsDirPath == "" || MetricsDirPath == null)
            {
                System.Windows.Forms.MessageBox.Show("No metrics directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outDirPath == "" || outDirPath == null)
            {
                System.Windows.Forms.MessageBox.Show("No output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lstMetrics.Items.Count < 1)
            {
                MessageBox.Show("No Metrics selected to convert to raster", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] metrics = lstMetrics.Items.Cast<string>().ToArray();

            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Converting Grid Metrics to Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                fsInt.ConvertGridMetricsToRaster(MetricsDirPath, outDirPath, metrics);
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
                rp.addMessage("Finished creating rasters in" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnOpenMetricsDir_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] flNames;
            string[] flPath = frmHlp.getPath(flt, out flNames, false);
            if (flPath.Length > 0)
            {
                txtMetricsDir.Text = flPath[0];
            }
        }
        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection s = lstMetrics.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmbMetrics.Items.Contains(txt))
                    {
                        cmbMetrics.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lstMetrics.Items.Remove(r);
            }

        }
        private void btnAddAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbMetrics.Items.Count; i++)
            {
                string st = cmbMetrics.Items[i].ToString();
                lstMetrics.Items.Add(st);
            }
            cmbMetrics.Items.Clear();
        }
        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstMetrics.Items.Count; i++)
            {
                string st = lstMetrics.Items[i].ToString();
                cmbMetrics.Items.Add(st);

            }
            lstMetrics.Items.Clear();
        }
        private void btnPlus_Click(object sender, EventArgs e)
        {
            string txt = cmbMetrics.Text;
            if (txt != null && txt != "")
            {
                cmbMetrics.Items.Remove(txt);
                if (!lstMetrics.Items.Contains(txt))
                {
                    lstMetrics.Items.Add(txt);
                }
            }
        }
    }
}