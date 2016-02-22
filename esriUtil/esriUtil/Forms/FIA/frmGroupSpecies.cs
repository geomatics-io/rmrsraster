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

namespace esriUtil.Forms.FIA
{
    public partial class frmGroupSpecies : Form
    {
        public frmGroupSpecies(string dbPath)
        {
            InitializeComponent();
            dbStr = dbPath;
            setDbProvider();
            string ext = System.IO.Path.GetExtension(dbStr).ToLower();
            string firstP = "Provider=Microsoft.ACE.OLEDB."+ oleAccessdbprovider +";Data Source=";
            string lastP = ";Persist Security Info=False;";
            switch (ext)
            {
                case ".xls":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + ";HDR=YES\";";
                    break;
                case ".xlsx":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + " Xml;HDR=YES\";";
                    break;
                case ".mdb":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";User Id=admin;Password=;";
                    break;
                case ".txt":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
                    break;
                case ".dbf":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
                    break;
                case ".dbc":
                    firstP = "Provider=vfpoledb;Data Source=";
                    lastP = ";Collating Sequence=machine;";
                    break;
                default:
                    break;
            }
            dbLc = firstP +dbStr + lastP;
            populateSpeciesDictionary();
            //btnAddGroup_Click(null,null);
            //Console.WriteLine(dbLc);
        }
        private HashSet<string> fieldPltCn = new HashSet<string>();
        public HashSet<string> UniqueFieldPlotsCN { get { return fieldPltCn; } set { fieldPltCn = value; } }
        private Dictionary<string,string> spDic = new Dictionary<string,string>();
        private void populateSpeciesDictionary()
        {
            int pltCnt = fieldPltCn.Count;
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                string sql = "SELECT SPCD, COMMON_NAME FROM REF_SPECIES ORDER BY SPCD";
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[2];
                    oleRd.GetValues(vls);
                    string jVl = vls[0].ToString() + " | " + vls[1].ToString();
                    spDic.Add(vls[0].ToString(), vls[1].ToString());
                    //lstSpecies.Items.Add(jVl);
                }
                oleRd.Close();
                con.Close();
            }

        }
        private void populateSpeciesList()
        {
            lstSpecies.Items.Clear();
            bool checkSpHash = true;
            if (fieldSpcd.Count < 1) checkSpHash = false;
            foreach (KeyValuePair<string,string> kvp in spDic)
            {
                string spcd = kvp.Key;
                if (checkSpHash)
                {
                    if(fieldSpcd.Contains(spcd))
                    {
                        string cmn = kvp.Value;
                        string jvl = spcd + " | " + cmn;
                        lstSpecies.Items.Add(jvl);
                    }
                }
                else
                {
                    string cmn = kvp.Value;
                    string jvl = spcd + " | " + cmn;
                    lstSpecies.Items.Add(jvl);
                }
            }
        }
        private void populateDataTable()
        {
            dgvSp.Rows.Clear();
            Dictionary<string, string> tblDic = new Dictionary<string, string>();
            bool chbox = false;
            foreach (KeyValuePair<string,string> kvp in grpDic)
            {
                string[] spcArr = kvp.Key.Split(new char[]{'_'});
                string spc = spcArr[0];
                string vlStr = spc + " | " + spDic[spc];
                lstSpecies.Items.Remove(vlStr);
                string[] grpArr = kvp.Value.Split(new char[]{'_'});
                string grp = grpArr[0];
                if (grpArr.Length > 1) chbox = true;
                string outVl;
                if (tblDic.TryGetValue(grp, out outVl))
                {
                    tblDic[grp] = outVl + ", " + spc;
                }
                else
                {
                    tblDic.Add(grp, spc);
                }
            }
            chbStatusCode.Checked = chbox;
            foreach (KeyValuePair<string,string> kvp in tblDic)
            {
                string[] rVl = {kvp.Key,kvp.Value};
                dgvSp.Rows.Add(rVl);
            }
        }
        private Dictionary<string, string> grpDic = new Dictionary<string, string>();
        public Dictionary<string, string> GroupDictionary { get { updateGroupDictionary(); return grpDic; } set { grpDic = value; populateDataTable(); } }
        private IFeatureClass pltFtc = null;
        public IFeatureClass PlotFeatureClass { get { return pltFtc; } set { pltFtc = value; updateFldPlt(); } }
        private string cnFldName = null;
        public string PLT_CN_FieldName { get { return cnFldName; } set { cnFldName = value; if (cnFldName != "PLT_CN" && pltFtc != null) updateFldPlt(); } }
        private void updateFldPlt()
        {
            if (pltFtc == null||cnFldName==null)
            {
                return;
            }
            else
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.SubFields = cnFldName;
                IFeatureCursor ftrCur = pltFtc.Search(qf, true);
                int fldIndex = ftrCur.FindField(cnFldName);
                if (fldIndex > -1)
                {
                    fieldPltCn.Clear();
                    IFeature ftr = ftrCur.NextFeature();
                    while (ftr != null)
                    {
                        fieldPltCn.Add(ftr.get_Value(fldIndex).ToString().ToUpper());
                        ftr = ftrCur.NextFeature();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
                    updateSpeciesFromTreeTable();
                }
                
            }

        }
        bool lt5 = false;
        public bool LessThan5 { get { return lt5; } set { lt5 = value; } }
        bool seedlings = false;
        public bool Seedlings { get { return seedlings; } set { seedlings = value; } }
        HashSet<string> fieldSpcd = new HashSet<string>();
        private void updateSpeciesFromTreeTable()
        {
            fieldSpcd.Clear();
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                string sql = "SELECT DISTINCT PLT_CN, SPCD FROM TREE WHERE DIA >= 5 and SPCD > 0";
                if (lt5)
                {
                    sql = "SELECT DISTINCT PLT_CN, SPCD FROM TREE WHERE SPCD > 0";
                }
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[2];
                    oleRd.GetValues(vls);
                    string pcn = vls[0].ToString().ToUpper();
                    if (fieldPltCn.Contains(pcn))
                    {
                        string spcd = vls[1].ToString();
                        fieldSpcd.Add(spcd);
                    }
                }
                oleRd.Close();
                if (seedlings)
                {
                    sql = "SELECT DISTINCT PLT_CN, SPCD FROM SEEDLING";
                    oleCom.CommandText = sql;
                    oleRd = oleCom.ExecuteReader();
                    while (oleRd.Read())
                    {
                        object[] vls = new object[2];
                        oleRd.GetValues(vls);
                        string pcn = vls[0].ToString().ToUpper();
                        if (fieldPltCn.Contains(pcn))
                        {
                            string spcd = vls[1].ToString();
                            fieldSpcd.Add(spcd);
                        }
                    }
                }
                con.Close();
            }
            populateSpeciesList();
            populateDataTable();

        }
        string oleAccessdbprovider = "12.0";
        string oleExceldbprovider = "12.0";
        string dbStr = null;
        private string dbLc = "";
        string av = "0.0";
        string ev = "0.0";
        private void btnAdd_Click(object sender, EventArgs e)
        {
            int spCnt = lstSpecies.SelectedItems.Count;
            int rCnt = dgvSp.SelectedRows.Count;
            if (spCnt < 1)
            {
                System.Windows.Forms.MessageBox.Show("You must have at least 1 species selected to add to a group!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (rCnt < 1)
            {
                btnAddGroup_Click(null, null);
            }
            ListBox.SelectedObjectCollection sObj = lstSpecies.SelectedItems;
            string[] vlArr = new string[sObj.Count];
            string[] rVlArr = new string[sObj.Count];
            for (int i = 0; i < sObj.Count; i++)
			{
                string vl = sObj[i].ToString();
                string vlnum = vl.Split(new string[]{" | "},StringSplitOptions.RemoveEmptyEntries)[0];
                vlArr[i] = vlnum;
                rVlArr[i] = vl;
			}
            foreach (string s in rVlArr)
            {
                lstSpecies.Items.Remove(s);
            }
            string psVl = dgvSp.SelectedRows[0].Cells[1].Value.ToString();
            if (psVl == null || psVl == "")
            {
                dgvSp.SelectedRows[0].Cells[1].Value = String.Join(", ", vlArr);
            }
            else
            {
                dgvSp.SelectedRows[0].Cells[1].Value = psVl + ", " + String.Join(", ", vlArr);
            }
            
        }
        private void setDbProvider()
        {
            av = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Access)+".0";
            ev = oleExceldbprovider = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Excel) + ".0";
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            object[] nRwArr = {(dgvSp.Rows.Count).ToString(),""}; 
            dgvSp.Rows.Add(nRwArr);
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            DataGridViewRow dgR = dgvSp.SelectedRows[0];
            string rVl = dgR.Cells[1].Value.ToString();
            string[] sArr = rVl.Split(new string[] {", "},StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in sArr)
            {
                string spName = spDic[s];
                lstSpecies.Items.Add(s + " | " + spName);
            }
            dgvSp.Rows.Remove(dgR);
        }
        string[] stCodeArr = null;
        private void updateGroupDictionary()
        {
            if (chbStatusCode.Checked)
            {
                if (stCodeArr == null)
                {
                    stCodeArr = getStatusCodes();
                }
            }
            grpDic.Clear();
            for (int i = 0; i < dgvSp.Rows.Count; i++)
            {
                DataGridViewCellCollection cls = dgvSp.Rows[i].Cells;
                string grpVl = cls[0].Value.ToString();
                string[] spcArr = cls[1].Value.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in spcArr)
                {
                    if (chbStatusCode.Checked)
                    {
                        foreach (string str in stCodeArr)
                        {
                            grpDic[s + "_" + str] = grpVl + "_" + str;
                        }
                        
                    }
                    else
                    {
                        grpDic[s] = grpVl;
                    }
                }
                
            } 
        }

        private string[] getStatusCodes()
        {
            HashSet<string> statusCodeHash = new HashSet<string>();
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                string sql = "SELECT DISTINCT STATUSCD FROM TREE";
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[1];
                    oleRd.GetValues(vls);
                    string spcd = vls[0].ToString();
                    statusCodeHash.Add(spcd);
                }
                oleRd.Close();
                con.Close();
            }
            return statusCodeHash.ToArray();
        }

        private void frmGroupSpecies_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void frmGroupSpecies_Shown(object sender, EventArgs e)
        {
            if (lstSpecies.Items.Count < 1) populateSpeciesList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Grp File|*.grp";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                string fNm = sd.FileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fNm, false))
                {
                    int rwCnt = dgvSp.Rows.Count;
                    for (int i = 0; i < rwCnt; i++)
                    {
                        DataGridViewCellCollection cl = dgvSp.Rows[i].Cells;
                        string gp = cl[0].Value.ToString();
                        string cd = cl[1].Value.ToString();
                        string wVl = gp + "|" + cd;
                        sw.WriteLine(wVl);
                    }
                    sw.Flush();
                    sw.Close();
                }
            }
            sd.Dispose();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "Grp File|*.grp";
            sd.Multiselect = false;
            if (sd.ShowDialog() == DialogResult.OK)
            {
                populateSpeciesList();
                dgvSp.Rows.Clear();
                string fNm = sd.FileName;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fNm))
                {
                    string ln = "";
                    while ((ln = sr.ReadLine())!=null)
                    {
                        string[] lnArr = ln.Split(new char[]{'|'});
                        
                        dgvSp.Rows.Add(lnArr);
                        string[] spArr = lnArr[1].Split(new string[]{", "},StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in spArr)
                        {
                            string cmn = spDic[s];
                            string nvl = s + " | " + cmn;
                            lstSpecies.Items.Remove(nvl);
                        }
                    }
                    sr.Close();
                }
            }
            sd.Dispose();
            

        }

        
    }
}
