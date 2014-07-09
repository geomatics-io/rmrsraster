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
    public partial class frmConvolutionRaster : Form
    {
        public frmConvolutionRaster(IMap map)
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
        public frmConvolutionRaster(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
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
        ~frmConvolutionRaster()
        {
            sfd.Dispose();
            ofd.Dispose();
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
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!rstDic.ContainsKey(outName))
                {
                    rstDic.Add(outName, rsUtil.returnRaster(outPath));
                    cmbInRaster1.Items.Add(outName);
                }
                else
                {
                    rstDic[outName] = rsUtil.returnRaster(outPath);
                }
                cmbInRaster1.Text = outName;
                

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
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbInRaster1.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
            dgvKernal.ColumnCount = 3;
            dgvKernal.RowCount = 3;
        }
        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            string rstNm = cmbInRaster1.Text;
            string outNm = txtOutName.Text;
            int rws = System.Convert.ToInt32(nudRows.Value);
            int clms = System.Convert.ToInt32(nudColumns.Value);
            if (rstNm == "" || rstNm == null)
            {
                MessageBox.Show("You must have a raster layer selected and a pixel type selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outNm == "" || outNm == null)
            {
                MessageBox.Show("You must specify an output raster name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<double> kernalLst = new List<double>();
            for (int i = 0; i < dgvKernal.ColumnCount; i++)
            {
                for (int j = 0; j < dgvKernal.RowCount; j++)
                {
                    object vlo = dgvKernal[i, j].Value;
                    double vl = 0;
                    if (DBNull.Value.Equals(vlo))
                    {
                        vl = 0;
                    }
                    else
                    {
                        vl = System.Convert.ToDouble(dgvKernal[i, j].Value);
                    }
                    kernalLst.Add(vl);
                    //Console.WriteLine(vl.ToString());
                }
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Creating Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            try
            {
                IRaster rst = rstDic[rstNm];
                outraster = rsUtil.returnRaster(rsUtil.convolutionRasterFunction(rst,clms,rws,kernalLst.ToArray()));
                if (mp != null && addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Show();
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromRaster(outraster);
                    rstLyr.Name = outNm;
                    rstLyr.Visible = false;
                    mp.AddLayer(rstLyr);
                }
                outrastername = outNm;
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
                rp.addMessage("Finished Convolution Analysis" + t);
                rp.enableClose();
                this.Close();
            }
        }

        private void dgvKernal_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if(!rsUtil.isNumeric(e.FormattedValue.ToString()))
            {
                e.Cancel = true;
                MessageBox.Show("You must use a numeric value!", "Numeric value error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvKernal[e.ColumnIndex, e.RowIndex].Value = 0;
            }
            
        }

        private void nudColumns_ValueChanged(object sender, EventArgs e)
        {
            int clms = System.Convert.ToInt32(nudColumns.Value);
            dgvKernal.ColumnCount = clms;
        }

        private void nudRows_ValueChanged(object sender, EventArgs e)
        {
            int rws = System.Convert.ToInt32(nudRows.Value);
            dgvKernal.RowCount = rws;
        }
        private OpenFileDialog ofd = new OpenFileDialog();
        private SaveFileDialog sfd = new SaveFileDialog();
        private void btnSave_Click(object sender, EventArgs e)
        {
            List<string> vlLst = new List<string>();
            sfd.Filter = "Kernel File|*.krn";
            sfd.AddExtension = true;
            sfd.Title = "Save Kernel";
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                string outfl = sfd.FileName;
                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(outfl))
                {
                    int clmCnt = dgvKernal.ColumnCount;
                    int rwsCnt = dgvKernal.RowCount;
                    for (int c = 0; c < clmCnt; c++)
                    {
                        for (int r = 0; r < rwsCnt; r++)
                        {
                            object vl = dgvKernal[c,r].Value;
                            if (Convert.IsDBNull(vl))
                            {
                                vl = 0;
                            }
                            try
                            {
                                vlLst.Add(vl.ToString());
                            }
                            catch (Exception ex)
                            {
                                vlLst.Add("0");
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                    sr.WriteLine(clmCnt.ToString() + "," + rwsCnt.ToString());
                    sr.WriteLine(String.Join(",",vlLst.ToArray()));
                    sr.Close();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            ofd.Multiselect = false;
            ofd.Filter = "Kernal File|*.krn";
            List<string> vlLst = new List<string>();
            if (ofd.ShowDialog(this)==DialogResult.OK)
            {
                string infl = ofd.FileName;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(infl))
                {
                    string ln = "";
                    while ((ln=sr.ReadLine())!=null)
                    {
                        vlLst.Add(ln);
                    }
                    sr.Close();
                }
                string clrw = vlLst[0];
                string cellVl = vlLst[1];
                string[] clrwArr = clrw.Split(new char[] { ',' });
                string[] cellVlArr = cellVl.Split(new char[] { ',' });
                int clm = System.Convert.ToInt32(clrwArr[0]);
                int rws = System.Convert.ToInt32(clrwArr[1]);
                nudColumns.Value = System.Convert.ToDecimal(clrwArr[0]);
                nudRows.Value = System.Convert.ToDecimal(clrwArr[1]);
                int cnt = 0;
                for (int c = 0; c < clm; c++)
                {
                    for (int r = 0; r < rws; r++)
                    {
                        dgvKernal[c, r].Value = System.Convert.ToInt32(cellVlArr[cnt]);
                        cnt++;
                    }
                }
            }
        }
    }
}
