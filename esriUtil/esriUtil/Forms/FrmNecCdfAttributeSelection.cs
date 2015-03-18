using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.Forms
{
    public partial class FrmNecCdfAttributeSelection : Form
    {
        public FrmNecCdfAttributeSelection(string netCdfFilePath, rasterUtil rasterUtility = null)
        {
            InitializeComponent();
            netCdf = netCdfFilePath;
            if (rasterUtility == null)
            {
                rsUtil = new rasterUtil();
            }
            else
            {
                rsUtil = rasterUtility;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            variable = cmbVariable.SelectedItem.ToString();
            xdim = cmbX.SelectedItem.ToString();
            ydim = cmbY.SelectedItem.ToString();
            bands = cmbBands.SelectedItem.ToString();
            this.Visible = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public string variable, xdim, ydim, bands, netCdf;

        public rasterUtil rsUtil = null;

        private void FrmNecCdfAttributeSelection_Load(object sender, EventArgs e)
        {
            IStringArray atArray = rsUtil.getNetCdfVariables(netCdf);
            for (int i = 0; i < atArray.Count; i++)
			{
                string s = atArray.get_Element(i);
                cmbVariable.Items.Add(s);
                cmbX.Items.Add(s);
                cmbY.Items.Add(s);
                cmbBands.Items.Add(s);
            }
        }
    }
}
