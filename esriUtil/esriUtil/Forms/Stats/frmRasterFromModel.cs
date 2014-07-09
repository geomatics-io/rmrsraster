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
    public partial class frmRasterFromModel : Form
    {
        public frmRasterFromModel()
        {
            InitializeComponent();
        }
        public frmRasterFromModel(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            
        }
        public frmRasterFromModel(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private bool addToMap = true;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    IRaster rs = rsUtil.returnRaster(outPath);
                    if (((IRasterBandCollection)rs).Count > 1)
                    {
                        IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                        for (int r = 0; r < rsBc.Count; r++)
                        {
                            string nNm = outName + "_Band_" + (r + 1).ToString();
                            IRaster rsB = rsUtil.returnRaster(rsUtil.getBand(rs, r));
                            if (!rstDic.ContainsKey(nNm))
                            {
                                rstDic.Add(nNm, rsB);
                                lsbRaster.Items.Add(nNm);
                                //cmbInRaster1.Items.Add(nNm);
                            }
                            else
                            {
                                rstDic[nNm] = rsB;
                            }
                        }
                    }
                    else
                    {
                        if (!rstDic.ContainsKey(outName))
                        {
                            rstDic.Add(outName, rs);
                            lsbRaster.Items.Add(outName);
                        }
                        else
                        {
                            rstDic[outName] = rs;
                        }
                    }
                    gxObj = eGxObj.Next();
                }
            }
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rsUtil.createRaster(((IRaster2)rstLyr.Raster).RasterDataset);
                    if (((IRasterBandCollection)rst).Count > 1)
                    {
                        for (int i = 0; i < ((IRasterBandCollection)rst).Count; i++)
                        {
                            IRaster rsN = rsUtil.returnRaster(rsUtil.getBand(rst, i));
                            string rsNm = lyrNm + "_Band_" + (i + 1).ToString();
                            if (!rstDic.ContainsKey(rsNm))
                            {
                                rstDic.Add(rsNm, rsN);
                                cmbInRaster1.Items.Add(rsNm);
                            }
                            else
                            {
                                rstDic[rsNm] = rsN;
                            }
                        }
                    }
                    else
                    {
                        if (!rstDic.ContainsKey(lyrNm))
                        {
                            rstDic.Add(lyrNm, rst);
                            cmbInRaster1.Items.Add(lyrNm);
                        }
                        else
                        {
                            rstDic[lyrNm] = rst;
                        }
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
            string rstNm = txtOutNm.Text;
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
            if (lsbRaster.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on Raster", "No Rasters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IRasterBandCollection rsBc = new RasterClass();
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                rsBc.AppendBands((IRasterBandCollection)rstDic[lsbRaster.Items[i].ToString()]);
            }
            IFunctionRasterDataset frDset = rsUtil.compositeBandFunction(rsBc);
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Building Model Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                Statistics.ModelHelper br = new Statistics.ModelHelper(mdPath, rsUtil.createRaster(frDset),rsUtil);
                outraster = br.getRaster();
                if (mp != null&&addToMap)
                {
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    rstLyr.CreateFromRaster(outraster);
                    rstLyr.Name = rstNm;
                    rstLyr.Visible = false;
                    mp.AddLayer(rstLyr);
                }
                outrastername = rstNm;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished Building Raster" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection s = lsbRaster.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmbInRaster1.Items.Contains(txt))
                    {
                        cmbInRaster1.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lsbRaster.Items.Remove(r);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                cmbInRaster1.Items.Add(lsbRaster.Items[i]);
            }
            lsbRaster.Items.Clear();

        }

        private void cmbInRaster1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object itVl = cmbInRaster1.SelectedItem;
            lsbRaster.Items.Add(itVl);
            cmbInRaster1.Items.Remove(itVl);
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

        private void txtOutputPath_Validated(object sender, EventArgs e)
        {
            string mPath = txtOutputPath.Text;
            if (System.IO.File.Exists(mPath))
            {
                btnVariables.Enabled = true;
            }
            else
            {
                btnVariables.Enabled = false;
            }
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            string mPath = txtOutputPath.Text;
            Statistics.ModelHelper mh = new Statistics.ModelHelper(mPath);
            mh.openModelReport(mPath, 0.05, false);
            Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.stepPGBar(100);
            rp.Text = "Model Variables";
            rp.enableClose();
            rp.addMessage("Independent variable names:");
            foreach (string s in mh.IndependentVariables)
            {
                rp.addMessage(s);
            }
            rp.Show();
        }
    }
}
