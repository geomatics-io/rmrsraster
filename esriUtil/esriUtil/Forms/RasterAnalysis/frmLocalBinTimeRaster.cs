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
    public partial class frmLocalBinTimeRaster : Form
    {
        public frmLocalBinTimeRaster(IMap map)
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
        public frmLocalBinTimeRaster(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        private Dictionary<string, IFunctionRasterDataset> rstDic = new Dictionary<string, IFunctionRasterDataset>();
        public Dictionary<string, IFeatureClass> FeatureDictionary { get { return ftrDic; } }
        public Dictionary<string, IFunctionRasterDataset> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = true;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltCol = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            fltCol.AddFilter(new ESRI.ArcGIS.Catalog.GxFilterFilesClass(), false);
            fltCol.AddFilter(new ESRI.ArcGIS.Catalog.GxFilterTextFilesClass(), false);
            gxDialog.Title = "Select a Raster or net_Cdf";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.createIdentityRaster(outPath));
                        //cmbInRaster1.Items.Add(outName);
                        lsbRaster.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.createIdentityRaster(outPath);
                    }
                    gxObj = eGxObj.Next();
                }
                //cmbInRaster1.SelectedItem = outName;
            }
            return;
        }
        private IFunctionRasterDataset outraster = null;
        public IFunctionRasterDataset OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, IFunctionRasterDataset rst)
        {
            if (!cmbInRaster1.Items.Contains(rstName))
            {
                cmbInRaster1.Items.Add(rstName);
                rstDic[rstName]= rst;
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
                    IFunctionRasterDataset rst = rsUtil.createIdentityRaster(((IRaster2)rstLyr.Raster).RasterDataset);
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbInRaster1.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
                
            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.localType)))
            {
                cmbFunction.Items.Add(s);
            }
            cmbFunction.SelectedItem = rasterUtil.localType.MEAN.ToString();
        }

         private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = txtOutNm.Text;
            string funNm = cmbFunction.Text;
            int before = System.Convert.ToInt32(nudBefore.Value);
            int after = System.Convert.ToInt32(nudAfter.Value);
            if (rstNm == null || rstNm == "")
            {
                MessageBox.Show("You must specify a output name","No Output",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (lsbRaster.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on Raster", "No Rasters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ( funNm==null||funNm=="")
            {
                MessageBox.Show("You must select at least on function", "No Function Selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IRasterBandCollection rsBc = new RasterClass();
            for (int i = 0; i < lsbRaster.Items.Count; i++)
            {
                rsBc.AppendBands((IRasterBandCollection)rstDic[lsbRaster.Items[i].ToString()]);
            }
            IFunctionRasterDataset fdset = rsUtil.compositeBandFunction(rsBc);
            rasterUtil.localType op = (rasterUtil.localType)Enum.Parse(typeof(rasterUtil.localType),funNm);
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Calculating raster values. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                outraster = rsUtil.focalBandfunction(fdset,op,before,after);
                //rp.addMessage("Number of bands = " + ((IRasterBandCollection)outraster).Count);
                if (mp != null&&addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromDataset((IRasterDataset)outraster);
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
                rp.addMessage("Finished Creating Rasters" + t);
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
            
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Net CDF|*.nc";
            od.Multiselect = true;
            if (od.ShowDialog()==DialogResult.OK)
            {
                FrmNecCdfAttributeSelection frmAt = new FrmNecCdfAttributeSelection(od.FileNames[0],rsUtil);
                frmAt.ShowDialog();
                foreach (string fn in od.FileNames)
                {
                    //System.Windows.Forms.MessageBox.Show(fn);
                    string outName = System.IO.Path.GetFileNameWithoutExtension(fn);
                    if (!rstDic.ContainsKey(outName))
                    {
                        //string vls = frmAt.variable + ", " + frmAt.xdim + ", " + frmAt.ydim + ", " + frmAt.bands;
                        IFunctionRasterDataset rsA = rsUtil.returnFunctionRasterDatasetNetCDF(fn,frmAt.variable, frmAt.xdim, frmAt.ydim, frmAt.bands);
                        //System.Windows.Forms.MessageBox.Show(((IRasterBandCollection)rsA).Count.ToString() + "; " + vls);
                        rstDic.Add(outName, rsA);
                        //cmbInRaster1.Items.Add(outName);
                        lsbRaster.Items.Add(outName);
                    }
                    else
                    {
                        IFunctionRasterDataset rsA = rsUtil.returnFunctionRasterDatasetNetCDF(fn, frmAt.variable, frmAt.xdim, frmAt.ydim, frmAt.bands);
                        rstDic[outName] = rsA;
                        //System.Windows.Forms.MessageBox.Show(((IRasterBandCollection)rsA).Count.ToString());
                    }
                }
                frmAt.Close();
                frmAt.Dispose();
            }
        }
    }
}

