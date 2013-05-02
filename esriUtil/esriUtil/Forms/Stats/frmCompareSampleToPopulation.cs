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

namespace esriUtil.Forms.Stats
{
    public partial class frmCompareSampleToPopulation : Form
    {
        public frmCompareSampleToPopulation()
        {
            InitializeComponent();
        }
        private rasterUtil rsUtil = new rasterUtil();
        private Dictionary<string, ITable> ftrDic = new Dictionary<string, ITable>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private void getFeaturePath(object sender, EventArgs e)
        {
            Control cnt = (Control)sender;
            string cntName = cnt.Name;
            TextBox txtBox = txtPop;
            if (cntName == btnSamp.Name) txtBox = txtSamp;
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature Class or Table";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                IRaster rs = rsUtil.returnRaster(outPath);
                if (rs == null)
                {
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, geoUtil.getTable(outPath));
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getTable(outPath);
                    }

                }
                else
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rs);
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                }
                txtBox.Text = outName;
            }
            updateFieldComboBox();
            return;
        }

        private void updateFieldComboBox()
        {
            string popStr = txtPop.Text;
            string sampStr = txtSamp.Text;
            cmbIndependent.Items.Clear();
            lstIndependent.Items.Clear();
            cmbStrata.Items.Clear();
            if (popStr == "" || sampStr == "")
            {
                return;
            }
            else
            {
                IFields popFlds = ftrDic[popStr].Fields;
                IFields sampFlds = ftrDic[sampStr].Fields;
                for (int i = 0; i < popFlds.FieldCount; i++)
                {
                    IField pFld = popFlds.get_Field(i);
                    string pFldN = pFld.Name;
                    if (sampFlds.FindField(pFldN) > -1)
                    {
                        cmbIndependent.Items.Add(pFldN);
                        cmbStrata.Items.Add(pFldN);
                    }
                }
            }
            return;
        }
        private void btnPlus_Click(object sender, EventArgs e)
        {
            string txt = cmbIndependent.Text;
            if (txt != null && txt != "")
            {
                cmbIndependent.Items.Remove(txt);
                if (!lstIndependent.Items.Contains(txt))
                {
                    lstIndependent.Items.Add(txt);
                }
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection s = lstIndependent.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmbIndependent.Items.Contains(txt))
                    {
                        cmbIndependent.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lstIndependent.Items.Remove(r);
            }

        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbIndependent.Items.Count; i++)
            {
                string st = cmbIndependent.Items[i].ToString();
                lstIndependent.Items.Add(st);
            }
            cmbIndependent.Items.Clear();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstIndependent.Items.Count; i++)
            {
                string st = lstIndependent.Items[i].ToString();
                cmbIndependent.Items.Add(st);

            }
            lstIndependent.Items.Clear();
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            string popStr = txtPop.Text;
            string sampStr = txtSamp.Text;
            string strataField = cmbStrata.Text;
            if (strataField == null) strataField = "";
            string outModel = txtOutput.Text;
            string[] explanitoryVariables = null;

            if (popStr == "" || popStr == null)
            {
                MessageBox.Show("You must select a population table!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (sampStr == "" || sampStr == null)
            {
                MessageBox.Show("You must select a sample table!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outModel == "" || outModel == null)
            {
                MessageBox.Show("You must select a output Model!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lstIndependent.Items.Count <= 0)
            {
                MessageBox.Show("You must select at least one explanatory variables!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                explanitoryVariables = lstIndependent.Items.Cast<string>().ToArray();
            }
            //check for variables;
            this.Visible = false;
            this.Refresh();
            try
            {
                esriUtil.Statistics.ModelHelper.runProgressBar("KS test");
                ITable sample1 = ftrDic[popStr];
                ITable sample2 = ftrDic[sampStr];
                esriUtil.Statistics.dataPrepCompareSamples ksTest = new Statistics.dataPrepCompareSamples(sample1, sample2, explanitoryVariables, strataField, chbPCA.Checked);
                ksTest.writeModel(outModel);
                ksTest.getReport();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                esriUtil.Statistics.ModelHelper.closeProgressBar();
                this.Close();
            }
           
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            txtOutput.Text = esriUtil.Statistics.ModelHelper.saveModelFileDialog();
        }

        
    }
}
