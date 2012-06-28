using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace esriUtil.Forms.MapServices
{
    public partial class frmInputBox : Form
    {
        public frmInputBox()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        public string ArcGISConnection { get { return txtCon.Text; } }
        public string Label { get { return lbl1.Text; } set { lbl1.Text = value; } }

        
    }
}
