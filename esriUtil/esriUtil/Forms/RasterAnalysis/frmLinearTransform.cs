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

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmLinearTransform : Form
    {
        public frmLinearTransform(IMap map)
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
        public frmLinearTransform(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
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
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IFeatureClass> FeatureDictionary { get { return ftrDic; } }
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
                            IRaster rsB = rsUtil.getBand(rs, r);
                            if (!rstDic.ContainsKey(nNm))
                            {
                                rstDic.Add(nNm, rsB);
                                //cmbInRaster1.Items.Add(nNm);
                                dgvRasterSlopes.Rows.Add(nNm, 0);
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
                            //cmbInRaster1.Items.Add(outName);
                            dgvRasterSlopes.Rows.Add(outName, 0);
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
            IRasterBandCollection rsBc = (IRasterBandCollection)rst;
            if (rsBc.Count > 1)
            {
                for (int i = 0; i < rsBc.Count; i++)
                {
                    string rstName2 = rstName + "_Band_" + (i + 1).ToString();
                    if (!cmbInRaster1.Items.Contains(rstName2))
                    {
                        cmbInRaster1.Items.Add(rstName2);
                        rstDic[rstName] = rst;
                    }
                }
            }
            else
            {
                if (!cmbInRaster1.Items.Contains(rstName))
                {
                    cmbInRaster1.Items.Add(rstName);
                    rstDic[rstName] = rst;
                }
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
                    IRaster rst = rstLyr.Raster;
                    if (((IRasterBandCollection)rst).Count > 1)
                    {
                        for (int i = 0; i < ((IRasterBandCollection)rst).Count; i++)
                        {
                            IRaster rsN = rsUtil.getBand(rst, i);
                            string rsNm = lyrNm + "_Band_" + (i+1).ToString();
                            if (!rstDic.ContainsKey(rsNm))
                            {
                                rstDic.Add(rsNm,rsN);
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
            double intercept = System.Convert.ToDouble(nudIntercept.Value);
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify a output name","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (dgvRasterSlopes.Rows.Count < 1)
            {
                MessageBox.Show("You must select at least one Raster", "No Rasters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            IRasterBandCollection rsBc = new RasterClass();
            List<double> slopes = new List<double>();
            for (int i = 0; i < dgvRasterSlopes.Rows.Count; i++)
            {
                rsBc.AppendBands((IRasterBandCollection)rstDic[dgvRasterSlopes[0,i].Value.ToString()]);
                object vl = dgvRasterSlopes[1, i].Value;
                if (Convert.IsDBNull(vl))
                {
                    vl = 0;
                }
                slopes.Add(System.Convert.ToDouble(vl));
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Transforming Rasters. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                outraster = rsUtil.calcRegressFunction((IRaster)rsBc,intercept,slopes.ToArray());
                if (mp != null&&addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
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
                rp.addMessage("Finished Transforming Rasters" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            DataGridViewCell dgvC = dgvRasterSlopes.SelectedCells[0];
            int rIndex = dgvC.RowIndex;
            string itVl = dgvRasterSlopes.Rows[rIndex].Cells[0].Value.ToString();
            cmbInRaster1.Items.Add(itVl);
            dgvRasterSlopes.Rows.RemoveAt(dgvC.RowIndex);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvRasterSlopes.Rows.Count; i++)
            {
                cmbInRaster1.Items.Add(dgvRasterSlopes.Rows[i].Cells[0].Value.ToString());
            }
            dgvRasterSlopes.Rows.Clear();
        }

        private void cmbInRaster1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object itVl = cmbInRaster1.SelectedItem;
            dgvRasterSlopes.Rows.Add(new object[] { itVl, 0 });
            cmbInRaster1.Items.Remove(itVl);
        }

    }
}