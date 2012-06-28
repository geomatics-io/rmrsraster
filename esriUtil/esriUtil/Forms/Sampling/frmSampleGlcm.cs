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
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Forms.Sampling
{
    public partial class frmSampleGlcm : Form
    {
        /// <summary>
        /// Form used to sample GLCM texture values given a point feature class. Texture values will be added to new fields within the sample feature class
        /// </summary>
        /// <param name="map">current map that has the focus. Optionally a null value can be passed</param>
        public frmSampleGlcm(IMap map)
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
        private glcm glcmTexture = new glcm();
        private viewUtility vUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private void getFeaturePath(bool featureClass)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (featureClass)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (featureClass)
                {
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, geoUtil.getFeatureClass(outPath));
                        cmbSampleFeatureClass.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                    }
                    cmbSampleFeatureClass.Text = outName;
                }
                else
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.returnRaster(outPath));
                        cmbRaster.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.returnRaster(outPath);
                    }
                    cmbRaster.Text = outName;
                }

            }
            return;
        }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer ftrLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = ftrLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                    {
                        if (!ftrDic.ContainsKey(lyrNm))
                        {
                            ftrDic.Add(lyrNm, ftrCls);
                            cmbSampleFeatureClass.Items.Add(lyrNm);
                        }
                    }
                    lyr = ftrLyrs.Next();

                }
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rstLyr.Raster;
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRaster.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
            foreach (string s in Enum.GetNames(typeof(glcm.windowType)))
            {
                cmbWindowType.Items.Add(s);
            }
            foreach(string s in Enum.GetNames(typeof(glcm.glcmMetric)))
            {
                cmbGlcmTypes.Items.Add(s);
            }
        }

        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePath(true);
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath(false);
        }

        private void cmbWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWindowType.Text.ToLower() == "circle")
            {
                nudRows.Visible = false;
                lblRows.Visible = false;
                lblColumns.Text = "Radius";
            }
            else
            {
                nudRows.Visible = true;
                lblRows.Visible = true;
                lblColumns.Text = "Columns";
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            string txt = cmbGlcmTypes.Text;
            if (txt != null && txt != "")
            {
                cmbGlcmTypes.Items.Remove(txt);
                if (!lsbGlcmType.Items.Contains(txt))
                {
                    lsbGlcmType.Items.Add(txt);
                }
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            string txt = lsbGlcmType.SelectedItem.ToString();
            if (txt != null && txt != "")
            {
                lsbGlcmType.Items.Remove(txt);
                if (!cmbGlcmTypes.Items.Contains(txt))
                {
                    cmbGlcmTypes.Items.Add(txt);
                }
            }


        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbGlcmTypes.Items.Count; i++)
            {
                string st = cmbGlcmTypes.Items[i].ToString();
                lsbGlcmType.Items.Add(st);
                
            }
            cmbGlcmTypes.Items.Clear();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsbGlcmType.Items.Count; i++)
            {
                string st = lsbGlcmType.Items[i].ToString();
                cmbGlcmTypes.Items.Add(st);
                
            }
            lsbGlcmType.Items.Clear();

        }

        private void btnSample_Click(object sender, EventArgs e)
        {
            string sampFtrClsNm = cmbSampleFeatureClass.Text;
            string rstNm = cmbRaster.Text;
            string windType = cmbWindowType.Text;
            string windDir = cmbDirections.Text;
            int clms = System.Convert.ToInt32(nudColumns.Value);
            int rows = System.Convert.ToInt32(nudRows.Value);
            int lsbCnt = lsbGlcmType.Items.Count;
            glcm.glcmMetric[] glcmArr = new glcm.glcmMetric[lsbCnt];
            for (int i = 0; i < lsbCnt; i++)
            {
                string vl = lsbGlcmType.Items[i].ToString();
                glcmArr[i]=(glcm.glcmMetric)Enum.Parse(typeof(glcm.glcmMetric), vl);
            }
            if (sampFtrClsNm == null || sampFtrClsNm == ""||rstNm==""||rstNm==null)
            {
                MessageBox.Show("You must have both a point feature class and a raster selected to use this tool!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (windType == null || windType == "" || windDir == "" || windDir == null)
            {
                MessageBox.Show("Window Type and Direction are not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (glcmArr.Length <1)
            {
                MessageBox.Show("You do not have any GLCM metrics selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (windType == "RADIUS")
            {
                glcmTexture.RADIUS = clms;
            }
            else
            {
                glcmTexture.COLUMNS = clms;
                glcmTexture.ROWS = rows;
            }
            if (windDir.ToUpper() == "HORIZONTAL")
            {
                glcmTexture.HORIZONTAL = true;
            }
            else
            {
                glcmTexture.HORIZONTAL = false;
            }
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            try
            {
                rp.addMessage("Sampling GLCM metrics for the first band of the specified raster layer.\nThis may take a few minutes...");
                rp.stepPGBar(15);
                rp.Show();
                rp.TopMost = true;
                rp.Refresh();
                glcmTexture.GLCM_METRIC = glcmArr;
                glcmTexture.InRaster = rstDic[rstNm];
                glcmTexture.sampleTexture(ftrDic[sampFtrClsNm]);
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.addMessage("Finished sampling GLCM metric...");
                rp.enableClose();
                this.Close();
            }
        }
    }
}
