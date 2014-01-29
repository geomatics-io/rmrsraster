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
    public partial class frmFormatZonalData : Form
    {
        public frmFormatZonalData(IMap map)
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
        private Dictionary<string, ITable> rstDic = new Dictionary<string, ITable>();
        public Dictionary<string, ITable> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a ";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    ITable rs = geoUtil.getTable(outPath);
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rs);
                        cmbdataset.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                    gxObj = eGxObj.Next();
                }
                cmbdataset.SelectedItem = outName;
            }
            return;
        }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer rstLyr = (IFeatureLayer)lyr;
                    IFeatureClass rst = rstLyr.FeatureClass;
                    
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, (ITable)rst);
                        cmbdataset.Items.Add(lyrNm);
                    }
                    else
                    {
                        rstDic[lyrNm] = (ITable)rst;
                    }
                    
                    lyr = rstLyrs.Next();
                }
            }
        }

         private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = cmbdataset.Text;
            string mdPath = txtOutputPath.Text;
            string pfr = txtPrefix.Text;
            int t1;
            double d;
            if (Int32.TryParse(pfr, out t1)||Double.TryParse(pfr,out d)) pfr = "";
            string linkFldName = cmbField.Text;
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify an input dataset","No Input",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (mdPath == null || mdPath == "")
            {
                MessageBox.Show("You must specify a zonal dataset","No Zonal Dataset",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            ITable tbl = rstDic[rstNm];
            ITable ztbl = rstDic[mdPath];
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Transforming data. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            rp.Refresh();
            try
            {
                Statistics.ModelHelper.runProgressBar("Transposing Zonal Data");
                FunctionRasters.zonalHelper.transformData(tbl,linkFldName, ztbl, pfr);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                Statistics.ModelHelper.closeProgressBar();
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished calculations" + t);
                rp.enableClose();
                this.Close();
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            getZoneSummaryPath();
        }

        private void getZoneSummaryPath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Table";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    ITable rs = geoUtil.getTable(outPath);
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rs);
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                    gxObj = eGxObj.Next();
                }
                txtOutputPath.Text = outName;
            }
            return;
        }

        private void cmbdataset_SelectedIndexChanged(object sender, EventArgs e)
        {
            ITable tb = rstDic[cmbdataset.Text];
            IFields flds = tb.Fields;
            cmbField.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                if(fld.Type== esriFieldType.esriFieldTypeDouble||fld.Type== esriFieldType.esriFieldTypeInteger||fld.Type== esriFieldType.esriFieldTypeOID||fld.Type== esriFieldType.esriFieldTypeSingle||fld.Type== esriFieldType.esriFieldTypeSmallInteger)
                {
                    cmbField.Items.Add(fld.Name);
                }
            }
        } 
    }
}
