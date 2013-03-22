using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmSetupToolbar : Form
    {
        public frmSetupToolbar()
        {
            InitializeComponent();
            setupDefaults();
        }
        esriUtil.update upRmrs = new update();
        private void setupDefaults()
        {
            if (upRmrs.UpdateCheck.ToLower() == "yes")
            {
                chbAutoUpdate.Checked = true;
            }
            else
            {
                chbAutoUpdate.Checked = false;
            }          
        }

        private void chbAutoUpdate_Click(object sender, EventArgs e)
        {
            if (chbAutoUpdate.Checked)
            {
                upRmrs.UpdateCheck = "yes";
            }
            else
            {
                upRmrs.UpdateCheck = "no";
            }
        }
    }
}
