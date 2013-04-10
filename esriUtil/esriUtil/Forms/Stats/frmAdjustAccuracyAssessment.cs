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
    public partial class frmAdjustAccuracyAssessment : Form
    {
        public frmAdjustAccuracyAssessment(IMap map)
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
        private rasterUtil rsUtil = new rasterUtil();
        private viewUtility vUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
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
                        cmbProjectBoundary.Items.Add(lyrNm);
                        cmbMap.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
        }
        private void getFeaturePathBoundary()
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
                    cmbProjectBoundary.Items.Add(outName);
                }
                else
                {
                    ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                }
                cmbProjectBoundary.SelectedItem = outName;
            }
            return;
        }
        private void getFeaturePathMap()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterGeoDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature or Raster Class";
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
                        ftrDic.Add(outName, geoUtil.getFeatureClass(outPath));
                        cmbMap.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                    }
                }
                else
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.returnRaster(outPath));
                        cmbMap.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.returnRaster(outPath);
                    }

                }
                cmbMap.SelectedItem = outName;
            }
            return;
        }


        private void btnExecute_Click(object sender, EventArgs e)
        {
            string projectFtrName = cmbProjectBoundary.Text;
            string MapStr = cmbMap.Text;
            double alpha = System.Convert.ToDouble(nudAlpha.Value);
            string oPath = txtOAA.Text;
            string aPath = txtOutModel.Text;
            if (aPath == null || aPath == "")
            {
                MessageBox.Show("You must select an accuracy assessment  model", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (oPath == null || oPath == "")
            {
                MessageBox.Show("You must select an output model", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (projectFtrName == null || projectFtrName == "")
            {
                MessageBox.Show("You must select a feature Class that represent a boundary", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MapStr == null || MapStr == "")
            {
                MessageBox.Show("You must select a Map featureClass or Raster", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
            this.Visible = false;
            IFeatureClass pj = ftrDic[projectFtrName];
            Statistics.dataPrepAdjustAccuracyAssessment aa = null;
            if (ftrDic.ContainsKey(MapStr))
            {
                IFeatureClass mp = ftrDic[MapStr];
                aa = new Statistics.dataPrepAdjustAccuracyAssessment(pj, mp, oPath, aPath);
            }
            else
            {
                IRaster mp = rstDic[MapStr];
                aa = new Statistics.dataPrepAdjustAccuracyAssessment(pj, mp, oPath, aPath);
            }
            aa.buildModel();
            Statistics.ModelHelper mH = new Statistics.ModelHelper(aPath);
            mH.openModelReport(aPath, alpha);
            this.Close();
        }

        private void btnOpenModel_Click(object sender, EventArgs e)
        {
            txtOAA.Text = Statistics.ModelHelper.openModelFileDialog();
        }
        private void btnSaveModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Model|*.mdl";
            sd.DefaultExt = "mdl";
            sd.AddExtension = true;
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutModel.Text = sd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getFeaturePathBoundary();
        }

        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePathMap();
        }
    }
}
