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
    public partial class frmSummarizeByField : Form
    {
        public frmSummarizeByField(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            populateComboBox();

        }
        public frmSummarizeByField(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        private Dictionary<string, ITable> tblDic = new Dictionary<string, ITable>();
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
                        ITable tbl = (ITable)ftrLyr.FeatureClass;
                        if (!tblDic.ContainsKey(lyrNm))
                        {
                            tblDic.Add(lyrNm, tbl);
                            cmbTable.Items.Add(lyrNm);
                        }
                        else
                        {
                            tblDic[lyrNm] = tbl;
                        }
                    }
                    lyr = mLyr.Next();
                }
                IStandaloneTableCollection tblCol = (IStandaloneTableCollection)mp;
                for (int i = 0; i < tblCol.StandaloneTableCount; i++)
                {
                    
                    IStandaloneTable StTbl = tblCol.StandaloneTable[i];
                    string lyrNm = StTbl.Name;
                    ITable tbl = StTbl.Table;
                    if (!tblDic.ContainsKey(lyrNm))
                    {
                        tblDic.Add(lyrNm, tbl);
                        cmbTable.Items.Add(lyrNm);
                    }
                    else
                    {
                        tblDic[lyrNm] = tbl;
                    }
                }
                foreach (string s in Enum.GetNames(typeof(rasterUtil.localType)))
                {
                    if(s!="POWER") cmbStats.Items.Add(s);
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string tblStr = cmbTable.Text;
            if (tblStr == "")
            {
                MessageBox.Show("You must specify a Table or Feature Class layer", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbFields.Items.Count < 1 )
            {
                MessageBox.Show("You must select at least on Field to summarize", "No Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbStats.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on summary statistic", "No Stats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            rasterUtil.localType[] statLst = new rasterUtil.localType[lsbStats.Items.Count];
            ITable tbl = tblDic[tblStr];
            string[] fldsArr = lsbFields.Items.Cast<string>().ToArray();
            for (int i = 0; i < lsbStats.Items.Count; i++)
			{
                statLst[i] = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),lsbStats.Items[i].ToString());
			}
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
                ftrUtil.summarizeAcrossFields(tbl, fldsArr, statLst, qWhere);
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
            if (((Button)sender).Name.ToLower().Contains("stats"))
            {
                lsb = lsbStats;
                cmb = cmbStats;
            }
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
            if (((Button)sender).Name.ToLower().Contains("stats"))
            {
                lsb = lsbStats;
                cmb = cmbStats;
            }
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
            if (((ComboBox)sender).Name.ToLower().Contains("stats"))
            {
                lsb = lsbStats;
                cmb = cmbStats;
            }
            object itVl = cmb.SelectedItem;
            lsb.Items.Add(itVl);
            cmb.Items.Remove(itVl);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            ListBox lsb = lsbFields;
            ComboBox cmb = cmbFields;
            if(((Button)sender).Name.ToLower().Contains("stats"))
            {
                lsb = lsbStats;
                cmb = cmbStats;
            }
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                lsb.Items.Add(cmb.Items[i]);
            }
            cmb.Items.Clear();
        }

        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tblStr = cmbTable.Text;
            ITable tbl = tblDic[tblStr];
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
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            fltColl.AddFilter(flt2, false);
            gxDialog.Title = "Select a Table or Feature Class";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;

                    ITable tbl = geoUtil.getTable(outPath);//need to check if this works for feature classes
                    if (!tblDic.ContainsKey(outName))
                    {
                        tblDic.Add(outName, tbl);
                        cmbTable.Items.Add(outName);
                    }
                    else
                    {
                        tblDic[outName] = tbl;
                    }
                    
                    gxObj = eGxObj.Next();
                }
                cmbTable.SelectedItem = outName;
            }
            return;
        }

        private void btnQry_Click(object sender, EventArgs e)
        {
            string tblName = cmbTable.Text;
            if(!String.IsNullOrEmpty(tblName))
            {
                ITable tbl = tblDic[tblName];
                Forms.frmAttributeQuery atQry = new Forms.frmAttributeQuery(tbl);
                atQry.Query = qWhere;
                atQry.ShowDialog();
                qWhere = atQry.Query;
                atQry.Dispose();
            }
        }
    }
}