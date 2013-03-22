using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Forms.Stats
{
    public partial class frmMultivariateLinearRegression : Form
    {
        public frmMultivariateLinearRegression(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private viewUtility vUtil = null;
        private Dictionary<string, ITable> ftrDic = new Dictionary<string, ITable>();
        private IFields flds = null;
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    if (!ftrDic.ContainsKey(lyrNm))
                    {
                        ftrDic.Add(lyrNm, (ITable)ftrCls);
                        cmbSampleFeatureClass.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
        }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!ftrDic.ContainsKey(outName))
                {
                    ftrDic.Add(outName, geoUtil.getTable(outPath));
                    cmbSampleFeatureClass.Items.Add(outName);
                }
                else
                {
                    ftrDic[outName] = geoUtil.getTable(outPath);
                }
                cmbSampleFeatureClass.SelectedItem = outName;
            }
            return;
        }


        private void btnOpenFeture_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }


        private void btnPlus_Click(object sender, EventArgs e)
        {
            string btnName = ((Control)sender).Name;
            if (btnName == cmbDependent.Name)
            {
                string txt = cmbDependent.Text;
                if (txt != null && txt != "")
                {
                    cmbDependent.Items.Remove(txt);
                    if (!lstDependent.Items.Contains(txt))
                    {
                        lstDependent.Items.Add(txt);
                    }
                }
            }
            else
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
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            string btnName = ((Button)sender).Name;
            if (btnName == btnMinus2.Name)
            {
                ListBox.SelectedObjectCollection s = lstDependent.SelectedItems;
                int cnt = s.Count;
                List<string> rLst = new List<string>();
                for (int i = 0; i < cnt; i++)
                {
                    string txt = s[i].ToString();
                    rLst.Add(txt);
                    if (txt != null && txt != "")
                    {
                        if (!cmbDependent.Items.Contains(txt))
                        {
                            cmbDependent.Items.Add(txt);
                        }
                    }
                }
                foreach (string r in rLst)
                {
                    lstDependent.Items.Remove(r);
                }
            }
            else
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
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            string btnName = ((Button)sender).Name;
            if (btnName == btnAddAll2.Name)
            {
                for (int i = 0; i < cmbDependent.Items.Count; i++)
                {
                    string st = cmbDependent.Items[i].ToString();
                    lstDependent.Items.Add(st);
                }
                cmbDependent.Items.Clear();
            }
            else
            {
                for (int i = 0; i < cmbIndependent.Items.Count; i++)
                {
                    string st = cmbIndependent.Items[i].ToString();
                    lstIndependent.Items.Add(st);
                }
                cmbIndependent.Items.Clear();
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            string btnName = ((Button)sender).Name;
            if (btnName == btnRemoveall2.Name)
            {
                for (int i = 0; i < lstDependent.Items.Count; i++)
                {
                    string st = lstDependent.Items[i].ToString();
                    cmbDependent.Items.Add(st);

                }
                lstDependent.Items.Clear();
            }
            else
            {
                for (int i = 0; i < lstIndependent.Items.Count; i++)
                {
                    string st = lstIndependent.Items[i].ToString();
                    cmbIndependent.Items.Add(st);

                }
                lstIndependent.Items.Clear();
            }
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {

            string cmbTxt = cmbSampleFeatureClass.Text;
            if (cmbTxt == null || cmbTxt == "")
            {
                return;
            }
            ITable ftrCls = ftrDic[cmbTxt];
            flds = ftrCls.Fields;
            cmbDependent.Text = "";
            cmbIndependent.Text = "";
            cmbDependent.Items.Clear();
            lstDependent.Items.Clear();
            cmbIndependent.Items.Clear();
            lstIndependent.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                string fldNm = fld.Name;
                esriFieldType fldType = fld.Type;
                if (fldType != esriFieldType.esriFieldTypeBlob && fldType != esriFieldType.esriFieldTypeDate && fldType != esriFieldType.esriFieldTypeGeometry && fldType != esriFieldType.esriFieldTypeGlobalID && fldType != esriFieldType.esriFieldTypeXML && fldType != esriFieldType.esriFieldTypeGUID && fldType != esriFieldType.esriFieldTypeOID && fldType != esriFieldType.esriFieldTypeRaster)
                {
                    cmbIndependent.Items.Add(fldNm);
                    if (fldType != esriFieldType.esriFieldTypeString)
                    {
                        cmbDependent.Items.Add(fldNm);
                    }
                }
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string[] depStrArr = lstDependent.Items.Cast<string>().ToArray();
            string[] indStrArr = lstIndependent.Items.Cast<string>().ToArray();
            string outPath = txtOutputPath.Text;
            double alpha = System.Convert.ToDouble(nudAlpha.Value);
            if (smpFtrNm == null || smpFtrNm == "")
            {
                MessageBox.Show("You must select a feature Class or table","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (depStrArr.Length<2)
            {
                MessageBox.Show("You must select at least 2 dependent field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (indStrArr.Length < 1)
            {
                MessageBox.Show("You must select at least 1 independent field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outPath==""||outPath==null)
            {
                MessageBox.Show("You must select an output Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<string> lstCat = new List<string>();
            for (int i = 0; i < lstIndependent.Items.Count; i++)
            {
                string s = lstIndependent.Items[i].ToString();
                IField fld = flds.get_Field(flds.FindField(s));
                if(fld.Type== esriFieldType.esriFieldTypeString)
                {
                    lstCat.Add(s);
                }
            }
            this.Visible = false;
            ITable ftrCls = ftrDic[smpFtrNm];
            Statistics.dataPrepMultivariateLinearRegression mvlr = new Statistics.dataPrepMultivariateLinearRegression(ftrCls, depStrArr, indStrArr, lstCat.ToArray(), chbIntOrigin.Checked);
            mvlr.writeModel(outPath);
            mvlr.getReport(alpha);
            this.Close();
        }

        private void btnOutputModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Model|*.mdl";
            sd.DefaultExt = "mdl";
            sd.AddExtension = true;
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputPath.Text = sd.FileName;
            }
        }
    }
}
