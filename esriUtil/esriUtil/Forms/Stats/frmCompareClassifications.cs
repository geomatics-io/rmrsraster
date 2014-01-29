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
    public partial class frmCompareClassifications : Form
    {
        public frmCompareClassifications(IMap map)
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
        private string weight = "";
        private string WEIGHT { get { return weight; } set { weight = value; } }
        private List<string> lstCategoricalFlds = new List<string>();
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
            gxDialog.Title = "Select a Feature Class";
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

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmbTxt = cmbSampleFeatureClass.Text;
            if (cmbTxt == "" || cmbTxt == null)
            {
                return;
            }
            ITable ftrCls = ftrDic[cmbTxt];
            flds = ftrCls.Fields;
            cmbMap.Text = "";
            cmbMap.Items.Clear();
            cmbRef.Items.Clear();
            cmbMap2.Items.Clear();
            lstCategoricalFlds.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                string fldNm = fld.Name;
                esriFieldType fldType = fld.Type;
                if (fldType != esriFieldType.esriFieldTypeBlob && fldType != esriFieldType.esriFieldTypeDate && fldType != esriFieldType.esriFieldTypeGeometry && fldType != esriFieldType.esriFieldTypeGlobalID && fldType != esriFieldType.esriFieldTypeXML && fldType != esriFieldType.esriFieldTypeGUID && fldType != esriFieldType.esriFieldTypeOID && fldType != esriFieldType.esriFieldTypeRaster)
                {
                    cmbMap.Items.Add(fldNm);
                    cmbRef.Items.Add(fldNm);
                    cmbMap2.Items.Add(fldNm);
                    if (fldType == esriFieldType.esriFieldTypeString)
                    {
                        lstCategoricalFlds.Add(fldNm);
                    }
                }
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string mapFld = cmbMap.Text;
            string refFld = cmbRef.Text;
            string mapFld2 = cmbMap2.Text;
            string outModelPath = txtOutputPath.Text;
            if (outModelPath == null || outModelPath == "")
            {
                MessageBox.Show("You must select an output model", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (smpFtrNm == null || smpFtrNm == "")
            {
                MessageBox.Show("You must select a feature Class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (mapFld == null || mapFld == "")
            {
                MessageBox.Show("You must select a map field 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (mapFld2 == null || mapFld2 == "")
            {
                MessageBox.Show("You must select a map field 2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (refFld == null || refFld == "")
            {
                MessageBox.Show("You must select a Reference field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Visible = false;
            ITable ftrCls = ftrDic[smpFtrNm];
            Statistics.dataPrepCompareClassifications cc = new Statistics.dataPrepCompareClassifications(ftrCls,refFld,mapFld,mapFld2);
            
            cc.writeModel(outModelPath);
            cc.getReport();
            this.Close();
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            txtOutputPath.Text = Statistics.ModelHelper.saveModelFileDialog();
        }
    }
}
