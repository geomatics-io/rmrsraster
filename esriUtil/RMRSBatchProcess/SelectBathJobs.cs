using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RMRSBatchProcess
{
    public partial class SelectBathJobs : Form
    {
        public SelectBathJobs()
        {
            InitializeComponent();
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "BatchFile|*.bch";
            ofd.AddExtension = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string s in ofd.FileNames)
                {
                    if (!lsbStats.Items.Contains(s))
                    {
                        lsbStats.Items.Add(s);
                    }
                }
            }
           
        }
        private esriUtil.batchCalculations btc = new esriUtil.batchCalculations();
        private void btnMinus_Click(object sender, EventArgs e)
        {
            lsbStats.Items.Remove(lsbStats.SelectedItem);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lsbStats.Items.Clear();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            esriUtil.Forms.RasterAnalysis.frmBatchProcess frm = new esriUtil.Forms.RasterAnalysis.frmBatchProcess();
            frm.Show();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (lsbStats.Items.Count < 1)
            {
                MessageBox.Show("You must have at least one batch file selected");
            }
            this.Visible = false;
            for (int i = 0; i < lsbStats.Items.Count; i++)
            {
                string fl = lsbStats.Items[0].ToString();
                btc.BatchPath = fl;
                btc.loadBatchFile();
                btc.runBatch();

            }
            this.Close();
        }
    }
}
