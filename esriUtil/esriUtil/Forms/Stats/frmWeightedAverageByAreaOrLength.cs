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
using ESRI.ArcGIS.Geometry;

namespace esriUtil.Forms.Stats
{
    public partial class frmWeightedAverageByAreaOrLength : Form
    {
        public frmWeightedAverageByAreaOrLength(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            populateComboBox();

        }
        public frmWeightedAverageByAreaOrLength(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
            mp = map;
            populateComboBox();
        }
        private bool addToMap = true;
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private string qWhere = "";
        
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer mLyr = mp.Layers;
                ILayer lyr = mLyr.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    if (lyr is FeatureLayer)
                    {
                        IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                        IFeatureClass ftrCls = ftrLyr.FeatureClass;
                        if(ftrCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                        {
                            if (!ftrDic.ContainsKey(lyrNm))
                            {
                                ftrDic.Add(lyrNm, ftrCls);
                                cmbStrata.Items.Add(lyrNm);
                                cmbStands.Items.Add(lyrNm);
                            }
                            else
                            {
                                ftrDic[lyrNm] = ftrCls;
                            }
                        }
                    }
                    lyr = mLyr.Next();
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string strataStr = cmbStrata.Text;
            string standsStr = cmbStands.Text;
            if (strataStr == "")
            {
                MessageBox.Show("You must specify a Stratification Feature Class layer", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (standsStr == "")
            {
                MessageBox.Show("You must specify a Stands Feature Class layer", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbFields.Items.Count < 1 )
            {
                MessageBox.Show("You must select at least on Field to summarize", "No Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            IFeatureClass strataCls = ftrDic[strataStr];
            IFeatureClass standsCls = ftrDic[standsStr];
            string[] fldsArr = lsbFields.Items.Cast<string>().ToArray();
            this.Visible = false;
            

            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Summarizing Fields. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                featureUtil ftrUtil = new featureUtil();
                ftrUtil.weightFieldValuesByAreaLength(strataCls,fldsArr,standsCls, chbLength.Checked);
                if (mp != null && addToMap)
                {

                    rp.Refresh();

                }

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
                rp.addMessage("Finished summarizing" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox lsb = lsbFields;
            ComboBox cmb = cmbFields;
            ListBox.SelectedObjectCollection s = lsb.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmb.Items.Contains(txt))
                    {
                        cmb.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lsb.Items.Remove(r);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ListBox lsb = lsbFields;
            ComboBox cmb = cmbFields;
            for (int i = 0; i < lsb.Items.Count; i++)
            {
                cmb.Items.Add(lsb.Items[i]);
            }
            lsb.Items.Clear();

        }

        private void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lsb = lsbFields;
            ComboBox cmb = cmbFields;
            object itVl = cmb.SelectedItem;
            lsb.Items.Add(itVl);
            cmb.Items.Remove(itVl);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            ListBox lsb = lsbFields;
            ComboBox cmb = cmbFields;
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                lsb.Items.Add(cmb.Items[i]);
            }
            cmb.Items.Clear();
        }

        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tblStr = cmbStrata.Text;
            IFeatureClass tbl = ftrDic[tblStr];
            IFields flds = tbl.Fields;
            cmbFields.Items.Clear();
            lsbFields.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                esriFieldType fldType = fld.Type;
                if (fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeDouble || fldType == esriFieldType.esriFieldTypeSingle || fldType == esriFieldType.esriFieldTypeSmallInteger)
                {
                    cmbFields.Items.Add(fld.Name);
                }
            }
        }

        private void btnOpenTable_Click(object sender, EventArgs e)
        {
            ComboBox cmb = cmbStrata;
            if (((Button)sender).Name.ToLower().Contains("stands")) cmb = cmbStands;
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            fltColl.AddFilter(flt2, false);
            gxDialog.Title = "Select a Feature Class";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;

                    
                    IFeatureClass ftrCls = geoUtil.getFeatureClass(outPath);//need to check if this works for feature classes
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, ftrCls);
                        cmb.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = ftrCls;
                    }
                    
                    gxObj = eGxObj.Next();
                }
                cmb.SelectedItem = outName;
            }
            return;
        }
    }
}