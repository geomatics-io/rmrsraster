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
    public partial class frmCreateMosaic : Form
    {
        public frmCreateMosaic(IMap map)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            mp = frmHlp.TheMap;
            geoUtil = frmHlp.GeoUtility;
            populateComboBox();
            
        }
        public frmCreateMosaic(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            mp = frmHlp.TheMap;
            geoUtil = frmHlp.GeoUtility;
            addToMap = AddToMap;
            populateComboBox();
        }
        frmHelper frmHlp = null;
        private bool addToMap = true;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, string> rstDic = new Dictionary<string, string>();
        public Dictionary<string, string> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath()
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            string[] rsNames;
            string[] rsPaths = frmHlp.getPath(flt, out rsNames, true);
            for (int i = 0; i < rsPaths.Length; i++)
            {
                string pt = rsPaths[i];
                string nm = rsNames[i];
                rstDic[nm] = pt;
                lsbRaster.Items.Add(nm);
            }
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, string rst)
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
            if (frmHlp.TheMap != null)
            {
                for (int i = 0; i < frmHlp.TheMap.LayerCount; i++)
                {
                    ILayer lyr = frmHlp.TheMap.Layer[i];
                    if (lyr is IRasterLayer)
                    {
                        IRasterLayer rsLyr = (IRasterLayer)lyr;
                        ITemporaryLayer tmp = (ITemporaryLayer)rsLyr;
                        if (tmp.Temporary)
                        {
                            //MessageBox.Show("Temp layer = " + rsLyr.Name);
                        }
                        else
                        {
                            rstDic[rsLyr.Name] = rsLyr.FilePath;
                        }
                    }
                }
            }
            foreach (string s in Enum.GetNames(typeof(rstMosaicOperatorType)))
            {
                if (s != "MT_CUSTOM") cmbType.Items.Add(s);
            }
            cmbType.SelectedItem = rstMosaicOperatorType.MT_FIRST.ToString();
            foreach (string s in Enum.GetNames(typeof(esriMosaicMethod)))
            {
                cmbMethod.Items.Add(s);
            }
            cmbMethod.SelectedItem = esriMosaicMethod.esriMosaicNone.ToString();
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }
        private IWorkspace outWks = null;
        private void getWksPath()
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            string[] namesArr;
            string[] pathArr = frmHlp.getPath(flt, out namesArr, false);
            string outPath = pathArr[0];
            outWks = geoUtil.OpenWorkSpace(outPath);
            txtWorkspace.Text = namesArr[0];
            return;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = txtOutNm.Text;
            esriMosaicMethod mMethod = (esriMosaicMethod)Enum.Parse(typeof(esriMosaicMethod),cmbMethod.Text);
            rstMosaicOperatorType mType = (rstMosaicOperatorType)Enum.Parse(typeof(rstMosaicOperatorType), cmbType.Text);
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify a output name","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (outWks == null)
            {
                MessageBox.Show("You must specify an output workspace", "No Workspace", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbRaster.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on Raster", "No Rasters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<string> rsLst = new List<string>();
            IEnvelope cmbEnv = new EnvelopeClass();
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                string nm = rstDic[lsbRaster.Items[i].ToString()];
                string bnd;
                IDataset dSet = (IDataset)rsUtil.openRasterDataset(nm,out bnd);
                IEnvelope rsEnv = ((IGeoDataset)dSet).Extent;
                cmbEnv.Union(rsEnv);
                string fPath = dSet.Workspace.PathName + dSet.BrowseName;
                //MessageBox.Show("Raster name = " + fPath);
                rsLst.Add(fPath);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(dSet);
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Building Mosaic Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.Show();
            rp.TopMost = true;
            if (chbOverview.Checked)
            {
                rp.addMessage("Building overviews will take additional time...");
            }
            try
            {
                outraster = rsUtil.mosaicRastersFunction(outWks,rstNm,rsLst.ToArray(),cmbEnv,mMethod,mType,chbFootprint.Checked,chbBoundary.Checked,chbSeamlines.Checked,chbOverview.Checked);
                rp.addMessage("Adding mosaic dataset to the map");
                if (mp != null&&addToMap)
                {
                    rp.Show();
                    rp.Refresh();
                    IMosaicLayer rstLyr = new MosaicLayerClass();
                    rstLyr.CreateFromMosaicDataset((IMosaicDataset)((IRaster2)outraster).RasterDataset);
                    mp.AddLayer((ILayer)rstLyr);
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
                rp.addMessage("Finished Mosaic Raster" + t);
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

        private void btnWorkspace_Click(object sender, EventArgs e)
        {
            getWksPath();
        }
    }
}
