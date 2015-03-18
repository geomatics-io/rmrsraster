using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Forms
{
    public partial class frmAttributeQuery : Form
    {
        public frmAttributeQuery(ITable inputTable)
        {
            InitializeComponent();
            tbl = inputTable;
            flds = tbl.Fields;
            IDataset dset = (IDataset)tbl;
            IWorkspace wks = (dset).Workspace;
            string wksp = wks.PathName.ToLower();
            if(wksp.EndsWith(".mdb")||wksp.EndsWith(".accbd"))
            {
                btn1.Text = "?";
                btn2.Text = "*";
                pr = '[';
                sf = ']';
            }
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                lstFields.Items.Add(pr+fld.Name+sf);
            }
            lstFields.SelectedItem = lstFields.Items[0];
            lblSelect.Text = "SELECT * FROM " + dset.BrowseName + " WHERE ";
        }
        public string qry = "";
        private ITable tbl = null;
        private IFields flds = null;
        private char pr = '"';
        private char sf = '"';
        geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private void btnEqual_Click(object sender, EventArgs e)
        {
            string vl = ((Button)sender).Text;
            int sS = rctQry.SelectionStart;
            rctQry.Text = rctQry.Text.Insert(sS, vl);
            rctQry.SelectionStart = sS + vl.Length;
        }

        private void btnUniq_Click(object sender, EventArgs e)
        {
            lstUniq.Items.Clear();
            string fldNm = lstFields.SelectedItem.ToString();
            fldNm = fldNm.TrimStart(new char[]{pr});
            fldNm = fldNm.TrimEnd(new char[]{sf});
            int fldIndex = flds.FindField(fldNm);
            if (fldIndex > -1)
            {
                IField fld = flds.get_Field(fldIndex);
                string p = "";
                string s = "";
                if (fld.Type == esriFieldType.esriFieldTypeString)
                {
                    p = "'";
                    s = "'";
                }
                HashSet<string> hash = geoUtil.getUniqueValues(tbl, fldNm);
                foreach (string hs in hash)
                {
                    lstUniq.Items.Add(p + hs + s);
                }
            }

        }

        private void lstUniq_DoubleClick(object sender, EventArgs e)
        {
            string vl = lstUniq.SelectedItem.ToString();
            int sS = rctQry.SelectionStart;
            rctQry.Text = rctQry.Text.Insert(sS, vl);
            rctQry.SelectionStart = sS + vl.Length;
        }

        private void lstFields_DoubleClick(object sender, EventArgs e)
        {
            string vl =  lstFields.SelectedItem.ToString();
            int sS = rctQry.SelectionStart;
            rctQry.Text = rctQry.Text.Insert(sS, vl);
            rctQry.SelectionStart = sS + vl.Length;
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            qry = rctQry.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Visible = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog svD = new SaveFileDialog();
            svD.AddExtension = true;
            svD.DefaultExt = ".qry";
            svD.Filter = "(Query)|*.qry";
            if (svD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(svD.FileName))
                {
                    sw.WriteLine(rctQry.Text);
                    sw.Close();
                }
            }
            svD.Dispose();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofD = new OpenFileDialog();
            ofD.AddExtension = true;
            ofD.DefaultExt = ".qry";
            ofD.Filter = "(Query)|*.qry";
            if (ofD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ofD.FileName))
                {
                    rctQry.Text = sr.ReadLine();
                    sr.Close();
                }
            }
            ofD.Dispose();
        }

    }
}
