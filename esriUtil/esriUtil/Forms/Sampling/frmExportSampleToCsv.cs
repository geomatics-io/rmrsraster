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

namespace esriUtil.Forms.Sampling
{
    public partial class frmExportSampleToCsv : Form
    {
        public frmExportSampleToCsv(IMap map)
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
                    if (!ftrDic.ContainsKey(lyrNm))
                    {
                        ftrDic.Add(lyrNm, ftrCls);
                        cmbSampleFeatureClass.Items.Add(lyrNm);
                    }
                    lyr = ftrLyrs.Next();

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
                cmbSampleFeatureClass.Text = outName;
            }
            return;
        }
        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sFd = new SaveFileDialog();
            sFd.DefaultExt = ".csv";
            sFd.Filter = "Text|*.csv";
            sFd.Title = "Save CSV to";
            sFd.AddExtension = true;
            if (sFd.ShowDialog(this) == DialogResult.OK)
            {
                txtCSV.Text = sFd.FileName;
            }
            sFd.Dispose();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string inFtrNm = cmbSampleFeatureClass.Text;
            string outCsvNm = txtCSV.Text;
            if (inFtrNm == "" || inFtrNm == null || outCsvNm == "" || outCsvNm == null)
            {
                MessageBox.Show("You must specify both a feature class and a output CSV file name");
                return;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            System.DateTime dt1 = DateTime.Now;
            rp.addMessage("Exporting samples to the following location:\n" + outCsvNm);
            rp.stepPGBar(50);
            rp.Show();
            rp.Refresh();
            try
            {
                IFeatureClass ftrCls = ftrDic[inFtrNm];
                geoUtil.exportTableToTxt((ITable)ftrCls,System.IO.Path.GetDirectoryName(outCsvNm),System.IO.Path.GetFileName(outCsvNm));
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                string dur = "Export took " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds to complete.";
                rp.addMessage(dur);
                rp.stepPGBar(100);
                rp.addMessage("Finished exporting samples");
                rp.enableClose();
                rp.Refresh();
                this.Close();
            }
        }
    }
}
