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
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;

namespace esriUtil.Forms.Sampling
{
    public partial class frmExplodeSample : Form
    {
        public frmExplodeSample(IMap map)
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
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
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
            }
            dgvAzDs.RowCount = 1;
        }

        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnExplode_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string linkFld = cmbLinkedFld.Text;
            if (smpFtrNm == null || smpFtrNm == ""||linkFld==null||linkFld=="" )
            {
                MessageBox.Show("sample location not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Dictionary<double, double> azDsDic = new Dictionary<double, double>();
            for (int i = 0; i < dgvAzDs.RowCount; i++)
            {
                double az = System.Convert.ToDouble(dgvAzDs[0, i].Value);
                double ds = System.Convert.ToDouble(dgvAzDs[1, i].Value);
                if (!azDsDic.ContainsKey(az))
                {
                    azDsDic.Add(az, ds);
                }
            }
            if (azDsDic.Count < 1)
            {
                MessageBox.Show("Nothing specified for offset!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Exploding Samples");
            rp.addMessage("This may take a while...");
            rp.stepPGBar(20);
            rp.Show();
            rp.Refresh();
            DateTime dt1 = DateTime.Now;
            try
            {
                IFeatureClass ftrCls = ftrDic[smpFtrNm];
                string outNm = smpFtrNm + "_explode";
                IFeatureClass outFtr = geoUtil.explodePoints(ftrDic[smpFtrNm],linkFld,azDsDic,outNm);
                IDataset dSet = (IDataset)outFtr;
                string outPath = dSet.Workspace.PathName + "\\" + outNm;
                rp.addMessage("New exploded feature class is stored at the following location:\n\t" + outPath);
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                string prcTime = "Time to complete process:\n" + ts.Days.ToString() + " Days " + ts.Hours.ToString() + " Hours " + ts.Minutes.ToString() + " Minutes " + ts.Seconds.ToString() + " Seconds ";
                rp.addMessage(prcTime);
                if (MessageBox.Show("Do you want to add layer to map?", "Add Layer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    IFeatureLayer ftrLyr = new FeatureLayerClass();
                    ftrLyr.FeatureClass = outFtr;
                    ftrLyr.Name = outNm;
                    mp.AddLayer((ILayer)ftrLyr);
                }

            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.enableClose();
                this.Close();
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            dgvAzDs.RowCount = dgvAzDs.RowCount + 1;
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            dgvAzDs.RowCount = dgvAzDs.RowCount - 1;
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ftrNm = cmbSampleFeatureClass.Text;
            cmbLinkedFld.Items.Clear();
            cmbLinkedFld.Text = "";
            if (ftrNm != null && ftrNm != "")
            {
                IFields flds = ftrDic[ftrNm].Fields;
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    esriFieldType flType = fld.Type;
                    if (flType != esriFieldType.esriFieldTypeRaster && flType != esriFieldType.esriFieldTypeOID && flType != esriFieldType.esriFieldTypeGeometry && flType != esriFieldType.esriFieldTypeBlob && flType != esriFieldType.esriFieldTypeDate && flType != esriFieldType.esriFieldTypeGlobalID && flType != esriFieldType.esriFieldTypeXML)
                    {
                        cmbLinkedFld.Items.Add(fld.Name);
                    }
                }
            }
        }

        
    }
}
