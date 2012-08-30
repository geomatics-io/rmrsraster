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


namespace esriUtil.Forms.SasProcedures
{
    public partial class frmBuildRegressionRaster : Form
    {
        public frmBuildRegressionRaster(IMap map)
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
        private rasterUtil rstUtil = new rasterUtil();
        private viewUtility vUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private IFields flds = null;
        private string weight = null;
        private string WEIGHT { get { return weight; } set { weight = value; } }
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
                        ftrDic.Add(lyrNm, ftrCls);
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
            flt = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
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
                    ftrDic.Add(outName, geoUtil.getFeatureClass(outPath));
                    cmbSampleFeatureClass.Items.Add(outName);
                }
                else
                {
                    ftrDic[outName] = geoUtil.getFeatureClass(outPath);
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
            string txt = lstIndependent.SelectedItem.ToString();
            if (txt != null && txt != "")
            {
                lstIndependent.Items.Remove(txt);
                if (!cmbIndependent.Items.Contains(txt))
                {
                    cmbIndependent.Items.Add(txt);
                }
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

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmbTxt = cmbSampleFeatureClass.Text;
            if (cmbTxt == null || cmbTxt == "")
            {
                return;
            }
            IFeatureClass ftrCls = ftrDic[cmbTxt];
            flds = ftrCls.Fields;
            cmbDepedent.Text = "";
            cmbDepedent.Items.Clear();
            cmbIndependent.Items.Clear();
            cmbWeight.Items.Clear();
            lstIndependent.Items.Clear();
            lstDependent.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                string fldNm = fld.Name;
                esriFieldType fldType = fld.Type;
                if (fldType != esriFieldType.esriFieldTypeString && fldType != esriFieldType.esriFieldTypeBlob && fldType != esriFieldType.esriFieldTypeDate && fldType != esriFieldType.esriFieldTypeGeometry && fldType != esriFieldType.esriFieldTypeGlobalID && fldType != esriFieldType.esriFieldTypeXML && fldType != esriFieldType.esriFieldTypeGUID && fldType != esriFieldType.esriFieldTypeOID && fldType != esriFieldType.esriFieldTypeRaster)
                {
                    cmbIndependent.Items.Add(fldNm);
                    cmbDepedent.Items.Add(fldNm);
                    cmbWeight.Items.Add(fldNm);
                }
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            
            int ent = System.Convert.ToInt32(nudEnter.Value);
            int sty = System.Convert.ToInt32(nudStay.Value);
            string wght = cmbWeight.Text;
            if(wght!=null&&wght!="")
            {
                WEIGHT = wght;
            }
            bool rslt = chbResults.Checked;
            bool val = chbValidate.Checked;
            bool edSas = chbSas.Checked;
            bool stepSelect = chbStepWise.Checked;
            if (smpFtrNm == null || smpFtrNm == "")
            {
                MessageBox.Show("You must select a feature Class","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (lstDependent.Items.Count<1)
            {
                MessageBox.Show("You must select at least 1 dependent field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lstDependent.Items.Count > 1)
            {
                stepSelect = false;
            }
            List<string> lstDep = new List<string>();
            for (int i = 0; i < lstDependent.Items.Count; i++)
            {
                string s = lstDependent.Items[i].ToString();
                lstDep.Add(s);
            }
            List<string> lstInd = new List<string>();
            for (int i = 0; i < lstIndependent.Items.Count; i++)
            {
                string s = lstIndependent.Items[i].ToString();
                lstInd.Add(s);
            }
            if (lstInd.Count < 1)
            {
                MessageBox.Show("You must select at least one independent field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            IFeatureClass ftrCls = ftrDic[smpFtrNm];
            regressionRaster rR = new regressionRaster();
            rR.Dependentfield = String.Join(" ",lstDep.ToArray());
            rR.IndependentFields = lstInd.ToArray();
            rR.OutWorkspace = ((IDataset)ftrCls).Workspace;
            rR.SampleLocations = ftrCls;
            rR.StepWiseSelection = stepSelect;
            rR.Validate = val;
            rR.WeightField = WEIGHT;
            rR.SlEnter = ent;
            rR.SlStay = sty;
            rR.runSasProcedure();
            if (rslt==true)
            {
                rR.showModelOutput();
            }
            if (edSas==true)
            {
                rR.editSasFile();
            }
            this.Close();
        }

        private void cmbDepedent_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = cmbDepedent.Text;
            if (txt != null && txt != "")
            {
                cmbDepedent.Items.Remove(txt);
                if (!lstDependent.Items.Contains(txt))
                {
                    lstDependent.Items.Add(txt);
                }
            }
        }

        private void btnMinDep_Click(object sender, EventArgs e)
        {
            string txt = lstDependent.SelectedItem.ToString();
            if (txt != null && txt != "")
            {
                lstDependent.Items.Remove(txt);
                if (!cmbDepedent.Items.Contains(txt))
                {
                    cmbDepedent.Items.Add(txt);
                }
            }

        }
    }
}
