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
    public partial class frmSummarizeRelatedTable : Form
    {
        public frmSummarizeRelatedTable(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            populateComboBox();

        }
        public frmSummarizeRelatedTable(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        private string pWhere = "";
        private string rWhere = "";
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
                            cmbParent.Items.Add(lyrNm);
                            cmbChild.Items.Add(lyrNm);
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
                        cmbParent.Items.Add(lyrNm);
                        cmbChild.Items.Add(lyrNm);
                    }
                    else
                    {
                        tblDic[lyrNm] = tbl;
                    }
                }

            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.focalType)))
            {
                cmbStats.Items.Add(s);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string ptblStr = cmbParent.Text;
            if (ptblStr == "")
            {
                MessageBox.Show("You must specify a Parent Table or Feature Class layer", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string plink = cmbPLink.Text;
            if (String.IsNullOrEmpty(plink))
            {
                MessageBox.Show("You must specify a Parent Link Field", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string rtblStr = cmbChild.Text;
            if (rtblStr == "")
            {
                MessageBox.Show("You must specify a Child Table or Feature Class layer", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string rlink = cmbCLink.Text;
            if (String.IsNullOrEmpty(rlink))
            {
                MessageBox.Show("You must specify a Child Link Field", "No Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            rasterUtil.focalType[] statLst = new rasterUtil.focalType[lsbStats.Items.Count];
            ITable ptbl = tblDic[ptblStr];
            ITable rtbl = tblDic[rtblStr];
            string[] fldsArr = lsbFields.Items.Cast<string>().ToArray();
            string[] groupFlds = lsbGroup.Items.Cast<string>().ToArray();
            for (int i = 0; i < lsbStats.Items.Count; i++)
			{
                statLst[i] = (rasterUtil.focalType)Enum.Parse(typeof(rasterUtil.focalType),lsbStats.Items[i].ToString());
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
                ftrUtil.summarizeRelatedTable(ptbl, rtbl, plink, rlink, fldsArr, groupFlds, statLst, pWhere, rWhere);
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
            if (((Button)sender).Name.ToLower().Contains("group"))
            {
                lsb = lsbGroup;
                cmb = cmbGroup;
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
            if (((Button)sender).Name.ToLower().Contains("group"))
            {
                lsb = lsbGroup;
                cmb = cmbGroup;
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
            if (((ComboBox)sender).Name.ToLower().Contains("group"))
            {
                lsb = lsbGroup;
                cmb = cmbGroup;
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
            if (((Button)sender).Name.ToLower().Contains("group"))
            {
                lsb = lsbGroup;
                cmb = cmbGroup;
            }
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                lsb.Items.Add(cmb.Items[i]);
            }
            cmb.Items.Clear();
        }

        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox pcmb = (ComboBox)sender;
            string tblStr = pcmb.Text;
            ITable tbl = tblDic[tblStr];
            IFields flds = tbl.Fields;
            ComboBox cmb = cmbPLink;
            bool rTable = false;
            if (pcmb.Name.ToLower().Contains("child"))
            {
                cmb = cmbCLink;
                rTable = true;
                cmbGroup.Items.Clear();
                cmbFields.Items.Clear();
                lsbGroup.Items.Clear();
                lsbFields.Items.Clear();
            }
            cmb.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                esriFieldType fldType = fld.Type;
                switch (fldType)
                {
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeSmallInteger:
                        cmb.Items.Add(fld.Name);
                        if (rTable)
                        {
                            cmbGroup.Items.Add(fld.Name);
                            cmbFields.Items.Add(fld.Name);
                        }
                        break;
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeGlobalID:
                    case esriFieldType.esriFieldTypeString:
                        cmb.Items.Add(fld.Name);
                        if (rTable) cmbGroup.Items.Add(fld.Name);
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnOpenTable_Click(object sender, EventArgs e)
        {
            ComboBox cmb = cmbParent;
            if (((Button)sender).Name.ToLower().Contains("child")) cmb = cmbChild;
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
                        cmb.Items.Add(outName);
                    }
                    else
                    {
                        tblDic[outName] = tbl;
                    }
                    
                    gxObj = eGxObj.Next();
                }
                cmb.SelectedItem = outName;
            }
            return;
        }

        private void btnQryParent_Click(object sender, EventArgs e)
        {
            string tblName = cmbParent.Text;
            if (!String.IsNullOrEmpty(tblName))
            {
                ITable tbl = tblDic[tblName];
                Forms.frmAttributeQuery atQry = new Forms.frmAttributeQuery(tbl);
                atQry.Query = pWhere;
                atQry.ShowDialog();
                pWhere = atQry.Query;
                atQry.Dispose();
            }
        }

        private void btnQueryChild_Click(object sender, EventArgs e)
        {
            string tblName = cmbChild.Text;
            if (!String.IsNullOrEmpty(tblName))
            {
                ITable tbl = tblDic[tblName];
                Forms.frmAttributeQuery atQry = new Forms.frmAttributeQuery(tbl);
                atQry.Query = rWhere;
                atQry.ShowDialog();
                rWhere = atQry.Query;
                atQry.Dispose();
            }
        }
    }
}