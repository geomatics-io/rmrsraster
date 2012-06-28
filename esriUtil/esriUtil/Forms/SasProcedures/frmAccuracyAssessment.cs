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
    public partial class frmAccuracyAssessment : Form
    {
        public frmAccuracyAssessment(IMap map)
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
        private Dictionary<string, object> rstDic = new Dictionary<string, object>();
        private IFields flds = null;
        private string weight = null;
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
                        ftrDic.Add(lyrNm, ftrCls);
                        rstDic.Add(lyrNm, ftrCls);
                        cmbSampleFeatureClass.Items.Add(lyrNm);
                        cmbRst.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
                rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rstLyr.Raster;
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRst.Items.Add(lyrNm);
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
            flt = new ESRI.ArcGIS.Catalog.GxFilterFeatureClassesClass();
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
        private void getRasterPath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature Class or Raster Dataset";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!ftrDic.ContainsKey(outName))
                {
                    rstDic.Add(outName, rstUtil.returnRaster(outPath));
                    cmbRst.Items.Add(outName);
                }
                else
                {
                    ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                }
                cmbRst.SelectedItem = outName;
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
            IFeatureClass ftrCls = ftrDic[cmbTxt];
            flds = ftrCls.Fields;
            cmbMap.Text = "";
            cmbMap.Items.Clear();
            cmbRef.Items.Clear();
            cmbWeight.Items.Clear();
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
                    if (fldType == esriFieldType.esriFieldTypeString)
                    {
                        lstCategoricalFlds.Add(fldNm);
                    }
                    else
                    {
                        cmbWeight.Items.Add(fldNm);
                    }
                }
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string mapFld = cmbMap.Text;
            string refFld = cmbRef.Text;
            string wght = cmbWeight.Text;
            string rstNm = cmbRst.Text;
            bool edSas = chbEditSas.Checked;
            bool exact = chbExact.Checked;
            double alpha = System.Convert.ToDouble(nudAlpha.Value);
            if (wght != null && wght != "")
            {
                WEIGHT = wght;
            }
            if (smpFtrNm == null || smpFtrNm == "")
            {
                MessageBox.Show("You must select a feature Class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (mapFld == null || mapFld == "")
            {
                MessageBox.Show("You must select a map field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (refFld == null || refFld == "")
            {
                MessageBox.Show("You must select a Reference field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            this.Visible = false;
            IFeatureClass ftrCls = ftrDic[smpFtrNm];
            accuracyAssessment aa = new accuracyAssessment();
            aa.SampleLocations = ftrCls;
            aa.MapField = mapFld;
            aa.ReferenceField = refFld;
            aa.WeightField = WEIGHT;
            aa.Alpha = alpha;
            aa.Exact = exact;
            if(rstNm!=null&&rstNm!="")
            {
                //can be either a raster or feature class looking for the ITABLE
                object mapObj = rstDic[rstNm];
                ITable mapTbl = null;
                if (mapObj is IRaster)
                {
                    IRaster rs = (IRaster)mapObj;
                    IRaster2 rs2 = (IRaster2)rs;
                    mapTbl = rs2.AttributeTable;

                }
                else
                {
                    IFeatureClass mFtrCls = (IFeatureClass)mapObj;
                    mapTbl = (ITable)mFtrCls;
                }
                aa.InTable = mapTbl;
            }
            aa.runSasProcedure();
            aa.showModelOutput();
            if (edSas)
            {
                aa.editSasFile();
            }
            this.Close();
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getRasterPath();
        }
    }
}