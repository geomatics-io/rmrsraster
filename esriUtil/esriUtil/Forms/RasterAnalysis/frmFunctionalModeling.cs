using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using esriUtil.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase; 

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmFunctionalModeling : Form
    {
        public frmFunctionalModeling(IMap map)
        {
            InitializeComponent();
            mp = map;
            rsUtil = new rasterUtil();
            funcModel = new functionModel(rsUtil);
            populateLstBox();
        }
        public frmFunctionalModeling(IMap map,ref rasterUtil rasterUtility)
        {
            InitializeComponent();
            mp = map;
            rsUtil = rasterUtility;
            funcModel = new functionModel(rsUtil);
            populateLstBox();
        }
        ~frmFunctionalModeling()
        {
            if (frmA != null) frmA.Dispose();
            if(frmL!=null) frmL.Dispose();           
            if(frmC != null) frmC.Dispose();
            if(frmCon != null) frmCon.Dispose();
            if(frmConv != null) frmConv.Dispose();
            if(frmF != null) frmF.Dispose();
            if(frmR != null) frmR.Dispose();
            if(frmComp != null) frmComp.Dispose();
            if(frmLt != null) frmLt.Dispose();
            if(frmRm != null) frmRm.Dispose();
            if(frmSm != null) frmSm.Dispose();
            if(frmE != null) frmE.Dispose();
            if(frmT != null) frmT.Dispose();
            if (frmM != null) frmM.Dispose();
            if (frmLand != null) frmLand.Dispose();
        }
        private void populateLstBox()
        {
            foreach (string s in Enum.GetNames(typeof(functionModel.functionGroups)))
            {
                lsbFunctions.Items.Add(s);
            }
        }
        private IMap mp = null;
        private rasterUtil rsUtil = null;
        private Dictionary<string, IRaster> rsultDic = new Dictionary<string, IRaster>();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, IRaster> mOutDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> ModeledOutputs { get { return mOutDic; } }
        private functionModel funcModel = null;
        private string[] getFormInputs(Form frm)
        {
            List<string> inputLst = new List<string>();
            foreach (Control c in frm.Controls)
            {
                if (c is GroupBox)
                {
                    foreach (Control cn in ((GroupBox)c).Controls)
                    {
                        inputLst.AddRange(getFormInputs(cn));
                    }
                }
                else
                {
                    inputLst.AddRange(getFormInputs(c));
                }
            }
            return inputLst.ToArray();

        }
        private string[] getFormInputs(Control c)
        {
            //Console.WriteLine("getting form inputs");
            List<string> inputLst = new List<string>();
            
            //Console.WriteLine(c.Name);
            if (c is ComboBox)
            {
                inputLst.Add(c.Name + "@" + c.Text);
            }
            else if (c is TextBox)
            {
                inputLst.Add(c.Name + "@" + c.Text);
            }
            else if (c is CheckedListBox)
            {
                CheckedListBox clsB = (CheckedListBox)c;
                List<string> clsbLst = new List<string>();
                foreach (string s in clsB.CheckedItems)
                {
                    clsbLst.Add(s);
                }
                inputLst.Add(c.Name + "@" + String.Join(",", clsbLst.ToArray()));
            }
            else if (c is ListBox)
            {
                ListBox lsB = (ListBox)c;
                List<string> lsbLst = new List<string>();
                foreach (string s in lsB.Items)
                {
                    lsbLst.Add(s);
                }
                inputLst.Add(c.Name + "@" + String.Join(",",lsbLst.ToArray()));
            }
            else if (c is CheckBox)
            {
                inputLst.Add(c.Name + "@" + ((CheckBox)c).Checked.ToString());
            }
            else if (c is NumericUpDown)
            {
                inputLst.Add(c.Name + "@" + ((NumericUpDown)c).Value.ToString());
            }
            else if (c is DataGridView)
            {
                DataGridView dg = (DataGridView)c;
                List<string> dgLst = new List<string>();
                for (int rw = 0; rw < dg.RowCount; rw++)
                {
                    List<string> vlLst = new List<string>();
                    for (int col = 0; col < dg.ColumnCount; col++)
                    {
                        object vl = dg[col, rw].Value;
                        if (Convert.IsDBNull(vl))
                        {
                            
                            vl = 0;
                        }
                        try
                        {
                            vlLst.Add(vl.ToString());
                        }
                        catch
                        {
                            vlLst.Add("0");
                        }
                    }
                    string apStr = String.Join("`",vlLst.ToArray());
                    dgLst.Add(apStr);
                }
                inputLst.Add(c.Name + "@" + String.Join(",", dgLst.ToArray()));
            }
            else if (c is GroupBox)
            {

            }
            else
            {
            }
            return inputLst.ToArray();
        }
        private frmArithmeticRaster frmA = null;
        private frmLogicalRaster frmL = null;
        private frmClipRaster frmC = null;
        private frmConditionalRaster frmCon = null;
        private frmConvolutionRaster frmConv = null;
        private frmFocalAnalysis frmF = null;
        private frmRescaleRaster frmR = null;
        private frmCompositeRaster frmComp = null;
        private frmLinearTransform frmLt = null;
        private frmRemapRaster frmRm = null;
        private frmSummarizeRaster frmSm = null;
        private frmExtractBand frmE = null;
        private frmMathRaster frmM = null;
        private frmSetNull frmSN = null;
        private Texture.frmLandscapeMetrics frmLand = null;
        private Texture.frmCreateGlcmSurface frmT = null;
        private string funcDirNm = "functionDatasets";
        private string funcExt = ".fds";
        private string outFunctionDatasetDir = null;

        private void lsbFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string vl = lsbFunctions.SelectedItem.ToString();
            functionModel.functionGroups fg = (functionModel.functionGroups)Enum.Parse(typeof(functionModel.functionGroups), vl);
            IRaster mo = null;
            string nm = "";
            string desc = "";
            DialogResult dRslt;
            switch (fg)
            {
                case functionModel.functionGroups.Arithmetic:
                    if (frmA == null)
                    {
                        frmA = new frmArithmeticRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmA.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmA.ShowDialog(this);
                    mo = frmA.OutRaster;
                    nm = frmA.OutRasterName;
                    if (mo != null && nm != null && dRslt== System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmA).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmA.Dispose();
                    break;
                case functionModel.functionGroups.Logical:
                    if (frmL == null)
                    {
                        frmL = new frmLogicalRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmL.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmL.ShowDialog(this);
                    mo = frmL.OutRaster;
                    nm = frmL.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmL).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }                    
                    //frmL.Dispose();
                    break;
                case functionModel.functionGroups.Clip:
                    if (frmC == null)
                    {
                        frmC = new frmClipRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmC.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmC.ShowDialog(this);
                    mo = frmC.OutRaster;
                    nm = frmC.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmC).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmC.Dispose();
                    break;
                case functionModel.functionGroups.Conditional:
                    if (frmCon == null)
                    {
                        frmCon = new frmConditionalRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmCon.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmCon.ShowDialog(this);               
                    mo = frmCon.OutRaster;
                    nm = frmCon.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmCon).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmCon.Dispose();
                    break;
                case functionModel.functionGroups.Convolution:
                    if (frmConv == null)
                    {
                        frmConv = new frmConvolutionRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmConv.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmConv.ShowDialog(this);
                    
                    mo = frmConv.OutRaster;
                    nm = frmConv.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmConv).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmConv.Dispose();
                    break;
                case functionModel.functionGroups.Focal:
                    if (frmF == null)
                    {
                        frmF = new frmFocalAnalysis(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmF.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmF.ShowDialog(this);
                    mo = frmF.OutRaster;
                    nm = frmF.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmF).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmF.Dispose();
                    break;
                case functionModel.functionGroups.Rescale:
                    if (frmR == null)
                    {
                        frmR = new frmRescaleRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmR.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmR.ShowDialog(this);
                    mo = frmR.OutRaster;
                    nm = frmR.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmR).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmR.Dispose();
                    break;
                case functionModel.functionGroups.Composite:
                    if (frmComp == null)
                    {
                        frmComp = new frmCompositeRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmComp.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmComp.ShowDialog(this);
                    
                    mo = frmComp.OutRaster;
                    nm = frmComp.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmComp).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmComp.Dispose();
                    break;
                case functionModel.functionGroups.LinearTransform:
                    if (frmLt == null)
                    {
                        frmLt = new frmLinearTransform(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmLt.addRasterToComboBox(kVp.Key, kVp.Value);
                    }
                    dRslt = frmLt.ShowDialog(this);
                    mo = frmLt.OutRaster;
                    nm = frmLt.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmLt).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmLt.Dispose();
                    break;
                case functionModel.functionGroups.Remap:
                    if (frmRm == null)
                    {
                        frmRm = new frmRemapRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmRm.addRasterToComboBox(kVp.Key, kVp.Value);
                    }
                    dRslt = frmRm.ShowDialog(this);
                    mo = frmRm.OutRaster;
                    nm = frmRm.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmRm).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmRm.Dispose();
                    break;
                case functionModel.functionGroups.LocalStatistics:
                    if (frmSm == null)
                    {
                        frmSm = new frmSummarizeRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmSm.addRasterToComboBox(kVp.Key, kVp.Value);
                    }
                    dRslt = frmSm.ShowDialog(this);
                    mo = frmSm.OutRaster;
                    nm = frmSm.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmSm).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmSm.Dispose();
                    break;
                case functionModel.functionGroups.ExtractBand:
                    if (frmE == null)
                    {
                        frmE = new frmExtractBand(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmE.addRasterToComboBox(kVp.Key, kVp.Value);
                    }
                    dRslt = frmE.ShowDialog(this);
                    mo = frmE.OutRaster;
                    nm = frmE.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmE).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmE.Dispose();
                    break;
                case functionModel.functionGroups.Math:
                    if (frmM == null)
                    {
                        frmM = new frmMathRaster(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmM.addRasterToComboBox(kVp.Key, kVp.Value);
                    }
                    dRslt = frmM.ShowDialog(this);
                    mo = frmM.OutRaster;
                    nm = frmM.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmM).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm, false, desc };
                        addRowsToDataGrid(ob);
                    }
                    break;
                case functionModel.functionGroups.Landscape:
                    if (frmLand == null)
                    {
                        frmLand = new Texture.frmLandscapeMetrics(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmLand.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmLand.ShowDialog(this);
                    mo = frmLand.OutRaster;
                    nm = frmLand.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmLand).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm,false, desc };
                        addRowsToDataGrid(ob);
                    }
                    break;
                case functionModel.functionGroups.SetNull:
                    if (frmSN == null)
                    {
                        frmSN = new frmSetNull(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmSN.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmSN.ShowDialog(this);
                    mo = frmSN.OutRaster;
                    nm = frmSN.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmSN).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm,false, desc };
                        addRowsToDataGrid(ob);
                    }
                    break;
                default:
                    if (frmT == null)
                    {
                        frmT = new Texture.frmCreateGlcmSurface(mp, ref rsUtil, false);
                    }
                    foreach (KeyValuePair<string, IRaster> kVp in rsultDic)
                    {
                        frmT.addRasterToComboBox(kVp.Key,kVp.Value);
                    }
                    dRslt = frmT.ShowDialog(this);
                    mo = frmT.OutRaster;
                    nm = frmT.OutRasterName;
                    if (mo != null && nm != null && nm != "" && dRslt == System.Windows.Forms.DialogResult.OK)
                    {
                        desc = vl + "(" + String.Join(";", getFormInputs((Form)frmT).ToArray()) + ")";
                        rsultDic[nm] = mo;
                        object[] ob = { nm,false, desc };
                        addRowsToDataGrid(ob);
                    }
                    //frmT.Dispose();
                    break;
            }
        }

        private void addRowsToDataGrid(object[] ob)
        {
            string rsName = ob[0].ToString();
            bool chNm = false;

            for (int i = 0; i < dgvOutputs.Rows.Count; i++)
            {
                DataGridViewCellCollection cells = dgvOutputs.Rows[i].Cells;
                string vl = cells[0].Value.ToString();
                if (rsName == vl)
                {
                    cells[2].Value = ob[2];
                    chNm = true;
                    break;
                }
            }
            if (!chNm)
            {
                dgvOutputs.Rows.Add(ob);
            }
        }

        private string getFeaturePath(string ftNm, bool featureClass)
        {
            string outPath = null;
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (featureClass)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Can't Find Feature " + ftNm;
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
            }
            return outPath;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int rwCnt = dgvOutputs.Rows.Count;
                bool chCheck = false;
                for (int i = 0; i < rwCnt; i++)
                {
                    DataGridViewCellCollection cells = dgvOutputs.Rows[i].Cells;
                    bool ch = System.Convert.ToBoolean(cells[1].Value);
                    if (ch)
                    {
                        chCheck = true;
                        break;
                    }
                }
                if (chCheck)
                {
                    ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
                    gxDialog.AllowMultiSelect = false;
                    ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
                    flt = new ESRI.ArcGIS.Catalog.GxFilterWorkspacesClass();
                    gxDialog.ObjectFilter = flt;
                    gxDialog.Title = "Select a Workspace";
                    ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
                    if (gxDialog.DoModalOpen(0, out eGxObj))
                    {
                        ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                        outFunctionDatasetDir = gxObj.FullName + "\\" + funcDirNm;
                        geoUtil.check_dir(outFunctionDatasetDir);
                        for (int i = 0; i < rwCnt; i++)
                        {
                            DataGridViewCellCollection cells = dgvOutputs.Rows[i].Cells;
                            bool ch = System.Convert.ToBoolean(cells[1].Value);
                            if (ch)
                            {
                                string nm = cells[0].Value.ToString();
                                string newNm = nm;
                                char[] bChr = System.IO.Path.GetInvalidFileNameChars();
                                foreach (char c in bChr)
                                {
                                    newNm = newNm.Replace(c, '_');
                                }
                                foreach (string s in new string[] { ":", ";", "?", ">", "<", "`", "~", "!", ".", ",", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "-" })
                                {
                                    newNm = newNm.Replace(s, "_");
                                }
                                string outFilePath = outFunctionDatasetDir + "\\" + newNm + funcExt;
                                //Console.WriteLine(outFilePath);
                                string dsc = cells[2].Value.ToString();
                                using (System.IO.StreamWriter sWr = new System.IO.StreamWriter(outFilePath))
                                {
                                    string lns = getSubFunctions(nm, dsc,null);
                                    if (lns.Length > 0)
                                    {
                                        sWr.WriteLine(lns);
                                    }
                                    sWr.Close();
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("To save a model you must have at least 1 model checked", "No models checked!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
            }
        }

        private string getSubFunctions(string outNm,string dsc, List<string> pLst)
        {
            //semicolon between variables and @ seperator for control name and value
            int rwCnt = dgvOutputs.Rows.Count;
            StringBuilder sb = new StringBuilder();
            List<string> fLst;
            if (pLst == null)
            {
                fLst = new List<string>();
            }
            else
            {
                fLst = pLst;
            }
            if (fdsDic.ContainsKey(outNm)) 
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fdsDic[outNm]))
                {
                    string dLn = null;
                    while ((dLn = sr.ReadLine()) != null)
                    {
                        if (dLn != System.Environment.NewLine&& dLn!="\n")
                        {
                            fLst.Add(dLn.Replace(System.Environment.NewLine, ""));
                        }
                    }
                    sr.Close();
                }
            }
            else
            {
                string func = dsc.Substring(0, dsc.IndexOf('('));
                string d = dsc;
                d = d.Substring(d.IndexOf('('));
                d = d.Substring(0, d.LastIndexOf(')'));
                string nwDsc = "";
                string[] sArray = d.Split(new char[] { ';' });
                string txtReplace = "";
                foreach (string s in sArray)
                {
                    txtReplace = s;
                    string[] sArr = s.Split(new char[] { '@' });
                    string vls = sArr[1];
                    foreach (string vls2 in vls.Split(new char[] { ',' }))
                    {
                        string vl = vls2.Split(new char[] { '`' })[0];
                        string contrl = sArr[0];
                        if (vl == outNm || !(contrl.ToLower().IndexOf("raster") > -1 || contrl.ToLower().IndexOf("feature") > -1))
                        {
                            continue;
                        }
                        if (rsultDic.ContainsKey(vl))
                        {
                            for (int i = 0; i < rwCnt; i++)
                            {
                                DataGridViewCellCollection cCol = dgvOutputs.Rows[i].Cells;
                                string outName = cCol[0].Value.ToString();
                                if (outName == vl)
                                {
                                    nwDsc = cCol[2].Value.ToString();
                                    nwDsc = getSubFunctions(outName, nwDsc, fLst);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            IRaster2 rs2 = null;
                            IRasterDataset rsDset = null;
                            IDataset dSet = null;
                            IFeatureClass ftrCls = null;
                            string fullPath = "";
                            bool checkReplace = true;
                            #region function switch
                            switch ((functionModel.functionGroups)Enum.Parse(typeof(functionModel.functionGroups), func))
                            {
                                case functionModel.functionGroups.Arithmetic:
                                    //Console.WriteLine("arithmetic function switch");
                                    if (frmA.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmA.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Logical:
                                    if (frmL.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmL.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Clip:
                                    if (frmC.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmC.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else if (frmC.FeatureDictionary.ContainsKey(vl))
                                    {
                                        ftrCls = frmC.FeatureDictionary[vl];
                                        dSet = (IDataset)ftrCls;
                                        fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                    }
                                    else
                                    {
                                        bool testVl = false;
                                        if (contrl.ToLower().IndexOf("raster") > -1)
                                        {
                                            testVl = true;
                                        }
                                        fullPath = getFeaturePath(vl, testVl);
                                    }
                                    break;
                                case functionModel.functionGroups.Conditional:
                                    if (frmCon.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmCon.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Convolution:
                                    if (frmConv.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmConv.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Focal:
                                    if (frmF.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmF.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.LocalStatistics:
                                    if (frmSm.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmSm.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.LinearTransform:
                                    if (frmLt.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmLt.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Rescale:
                                    if (frmR.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmR.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Remap:
                                    if (frmRm.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmRm.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Composite:
                                    if (frmComp.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmComp.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.ExtractBand:
                                    if (frmE.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmE.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.GLCM:
                                    if (frmT.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmT.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Math:
                                    if (frmM.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmM.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.Landscape:
                                    if (frmLand.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmLand.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                case functionModel.functionGroups.SetNull:
                                    if (frmSN.RasterDictionary.ContainsKey(vl))
                                    {
                                        rs2 = (IRaster2)frmSN.RasterDictionary[vl];
                                        rsDset = rs2.RasterDataset;
                                        dSet = (IDataset)rsDset;
                                        int vlIndex = vl.IndexOf("Band_");
                                        if ((((IRasterBandCollection)rsDset).Count > 1) && vlIndex > -1)
                                        {
                                            if (vlIndex > -1)
                                            {
                                                string bandStr = vl.Substring(vlIndex, vl.Length - vlIndex);
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\" + bandStr;
                                            }
                                            else
                                            {
                                                fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName + "\\xxx";
                                            }
                                        }
                                        else
                                        {
                                            fullPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                                        }
                                    }
                                    else if (rsUtil.isNumeric(vl) || vl == "")
                                    {
                                        checkReplace = false;
                                    }
                                    else
                                    {
                                        fullPath = getFeaturePath(vl, false);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            if (checkReplace)
                            {
                                string pTxtReplace = txtReplace.Replace(vl, fullPath);
                                nwDsc = nwDsc.Replace(txtReplace, pTxtReplace);
                                dsc = dsc.Replace(txtReplace, pTxtReplace);
                                txtReplace = pTxtReplace;
                            }
                        }
                    }

                    string[] nwDscArr = nwDsc.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in nwDscArr)
                    {
                        if (str != "" && !fLst.Contains(str) && str != "\n" && str!=null)
                        {
                            Console.WriteLine(str);
                            fLst.Add(str);
                        }
                    }
                }
            }
            string[] dscArr = dsc.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in dscArr)
            {
                if (str != "" && !fLst.Contains(str) && str != "\n" && str != null)
                {
                    fLst.Add(str);
                }
            }
            foreach (string s in fLst)
            {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }
        
        private void btnMinus_Click(object sender, EventArgs e)
        {

            string rsNm = dgvOutputs.SelectedRows[0].Cells[0].Value.ToString();
            rsultDic.Remove(rsNm);
            dgvOutputs.Rows.Remove(dgvOutputs.SelectedRows[0]);
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvOutputs.Rows.Clear();
            rsultDic.Clear();
        }

        private void frmFunctionalModeling_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < dgvOutputs.RowCount; i++)
            {
                DataGridViewCellCollection cl = dgvOutputs.Rows[i].Cells;
                string rsNm = cl[0].Value.ToString();
                bool tst = System.Convert.ToBoolean(cl[1].Value);
                if (tst == true)
                {
                    IRaster rs = rsultDic[rsNm];
                    IRasterLayer rsLyr = new RasterLayerClass();
                    rsLyr.CreateFromRaster(rs);
                    rsLyr.Name = rsNm;
                    rsLyr.Visible = false;
                    mp.AddLayer((ILayer)rsLyr);
                }
            }
        }

        private void dgvOutputs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            bool vl = System.Convert.ToBoolean(dgvOutputs.SelectedRows[0].Cells[1].Value);
            bool nvl = true;
            if (vl)
            {
                nvl = false;
            }
            else
            {
                nvl = true;
            }
            dgvOutputs.SelectedRows[0].Cells[1].Value = nvl;
        }
        private OpenFileDialog ofd = new OpenFileDialog();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Function Model|*.fds";
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                string fileNm = ofd.FileName;
                if (System.IO.File.Exists(fileNm))
                {
                    funcModel.FunctionDatasetPath = fileNm;
                    string nm = null;
                    IRaster rs = null;
                    string desc = null;
                    funcModel.createFunctionRaster(out nm, out rs, out desc);
                    rsultDic[nm] = rs;
                    Dictionary<string, IRaster> fDic = functionModel.RasterDictionary;
                    //foreach (KeyValuePair<string, IRaster> kvp in fDic)
                    //{
                    //    string k = kvp.Key;
                    //    IRaster r = kvp.Value;
                    //    rsultDic[k] = r;
                    //}
                    fdsDic = functionModel.FunctionDatasetPathDic;
                    object[] obj = {nm,false,desc};
                    addRowsToDataGrid(obj);
                }
            }       
        }
        private Dictionary<string, string> fdsDic = new Dictionary<string, string>();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
