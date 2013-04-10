using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.Forms.Stats
{
    public partial class frmViewModelSampleSize : Form
    {
        public frmViewModelSampleSize()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "Model|*.mdl";
            sd.DefaultExt = "mdl";
            sd.AddExtension = true;
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputPath.Text = sd.FileName;
            }
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            string modelPath = txtOutputPath.Text;
            if (modelPath == null || modelPath == "")
            {
                MessageBox.Show("You must specify a model path");
                return;
            }
            else
            {
                double prop = System.Convert.ToDouble(nudProp.Value);
                double alpha = System.Convert.ToDouble(nudAlpha.Value);
                esriUtil.Statistics.dataPrepSampleSize.getReport(modelPath, prop, alpha);
                this.Close();
            }

        }
    }
}
