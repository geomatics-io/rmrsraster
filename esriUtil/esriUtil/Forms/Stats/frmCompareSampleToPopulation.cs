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
    public partial class frmCompareSampleToPopulation : Form
    {
        public frmCompareSampleToPopulation()
        {
            InitializeComponent();
        }

        private void btnPop_Click(object sender, EventArgs e)
        {
            txtPop.Text = Statistics.ModelHelper.openModelFileDialog();
        }

        private void btnSamp_Click(object sender, EventArgs e)
        {
            txtSamp.Text = Statistics.ModelHelper.openModelFileDialog();
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            string popStr = txtPop.Text;
            string sampStr = txtSamp.Text;
            if (popStr == "" || popStr == null)
            {
                MessageBox.Show("You must select a population model!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (sampStr == "" || sampStr == null)
            {
                MessageBox.Show("You must select a sample model!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double[] means, var, meanDiff, varDiff;
            List<string> labels;
            
            this.Visible = false;
            RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.Text = "Comparing strata and cluster means";
            rp.stepPGBar(10);
            rp.addMessage("Comparing means and variance using paired t-test (p-values)...\n");
            rp.Show();
            rp.Refresh();
            try
            {
                Statistics.dataPrepCompareVarCov.CompareStratMeansVar(popStr, sampStr, out labels,out meanDiff,out varDiff, out means, out var);
                rp.stepPGBar(75);
                rp.addMessage("Label     | Mean dif  | p-value         | Var dif   | p-value        ");
                rp.addMessage("-".PadRight(69,'-'));
                for (int i = 0; i < means.Length; i++)
                {
                    string lbl = getValue(labels[i],9);
                    string md = getValue(meanDiff[i].ToString(),9);
                    string m = getValue(means[i].ToString(),15);
                    string vd = getValue(varDiff[i].ToString(),9);
                    string v = getValue(var[i].ToString(),15);
                    string ln = lbl + " | " + md + " | " + m+" | "+vd+" | "+v;
                    rp.addMessage(ln);

                }
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.enableClose();
                rp.addMessage("-".PadRight(69, '-'));
                rp.addMessage("\nFinished Comparing means and variance");
            }
        }
        private string getValue(string vl, int leng)
        {
            string outVl = vl;
            if (vl.Length > leng) outVl = vl.Substring(0, leng);
            else outVl = vl.PadRight(leng, ' ');
            return outVl;
        }
    }
}
