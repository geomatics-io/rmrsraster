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
    public partial class frmPredictNewData : Form
    {
        public frmPredictNewData(IMap map)
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
                        cmbSampleFeatureClass.Items.Add(outName);
                        cmbSampleFeatureClass.SelectedItem = outName;
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                    gxObj = eGxObj.Next();
                }
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
                        cmbSampleFeatureClass.Items.Add(lyrNm);
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
            string rstNm = cmbSampleFeatureClass.Text;
            string mdPath = txtOutputPath.Text;
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify a output name","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (mdPath == null || mdPath == "")
            {
                MessageBox.Show("You must specify a model path","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            ITable tbl = rstDic[rstNm];
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Predicting new data. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                Statistics.ModelHelper.runProgressBar("Predicting data");
                Statistics.ModelHelper br = new Statistics.ModelHelper(mdPath);
                rp.Refresh();
                br.predictNewData(tbl);
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
            OpenFileDialog sd = new OpenFileDialog();
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
