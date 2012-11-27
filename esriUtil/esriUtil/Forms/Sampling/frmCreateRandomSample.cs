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

namespace esriUtil.Forms.Sampling
{
    public partial class frmCreateRandomSample : Form
    {
        /// <summary>
        /// Creates a point file with randommly assign sample locations given a raster. If statified is false the samples will be allocated randomly across the landsape and one field 
        /// is created within the point feature class called value that stores the raster values at each location.
        /// If statified is true then each stratum will recieve a specfied number of sample loctions and 2 fields will be created called class and weight identifing the the value of the raster at that locaiton and the weight that sample represents from the population of 
        /// raster cells. 
        /// </summary>
        /// <param name="map">the current map opionally this value can be null</param>
        /// <param name="stratified">boolean value that identifies whether sampling using stratification or simple random sample</param>
        public frmCreateRandomSample(IMap map,bool stratified)
        {
            InitializeComponent();
            strata = stratified;
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
                loadCombos();
            }
            if (strata)
            {
                this.Text = "Create Stratified Random Sample";
                lblNS.Text = "Samples per Strata";
            }
        }
        private bool strata = false;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private void loadCombos()
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
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRst.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
        }
        private void getPath(bool isRaster)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (isRaster)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (isRaster)
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.returnRaster(outPath));
                        cmbRst.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.returnRaster(outPath);
                    }
                    cmbRst.Text = outName;
                }
                else
                {
                    txtOutWorkspace.Text = outPath;
                }
            }
            return;
        }

        private void btnOpenWorkspace_Click(object sender, EventArgs e)
        {
            getPath(false);
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getPath(true);
        }

        private void btnCreateSamples_Click(object sender, EventArgs e)
        {
            string rst = cmbRst.Text;
            string oWks = txtOutWorkspace.Text;
            string sNm = txtSampleName.Text;
            int numSample = System.Convert.ToInt32(nudSamples.Value);
            if (rst == null || rst == "" || oWks == null || oWks == "")
            {
                MessageBox.Show("You must specify a raster, an output workspace, and a output file name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            IRaster rs = rstDic[rst];
            IRasterBandCollection rsBc = (IRasterBandCollection)rs;
            IDataset ds = (IDataset)rsBc.Item(0).RasterDataset;
            this.Visible = false;
            this.Refresh();
            RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.Show();
            rp.stepPGBar(20);
            DateTime ds1 = DateTime.Now;
            try
            {
                IFeatureClass ftrCls = null;
                if (strata)
                {
                    rp.addMessage("Creating random stratified sample. This could take a while...");
                    rp.stepPGBar(10);
                    rp.Refresh();
                    ftrCls = rsUtil.createRandomSampleLocationsByClass(geoUtil.OpenWorkSpace(oWks), rs, numSample, 1, sNm);
                    
                }
                else
                {
                    rp.addMessage("Creating random sample. This could take a while...");
                    rp.stepPGBar(10);
                    rp.Refresh();
                    ftrCls =rsUtil.createRandomSampleLocations(geoUtil.OpenWorkSpace(oWks), rs, numSample,sNm);
                }
                if (mp!=null)
                {
                    rp.addMessage("Adding Samples to the map");
                    rp.stepPGBar(20);
                    rp.Refresh();
                    IFeatureLayer fLyer = new FeatureLayerClass();
                    fLyer.FeatureClass = ftrCls;
                    fLyer.Name = sNm;
                    fLyer.Visible = true;
                    mp.AddLayer(fLyer);
                }

            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime ds2 = DateTime.Now;
                TimeSpan ts = ds2.Subtract(ds1);
                rp.stepPGBar(100);
                string tsSpan = "Process took a total of " + ts.TotalMinutes.ToString() + " minutes to finish";
                rp.addMessage("Finished creating samples");
                rp.addMessage(tsSpan);
                rp.enableClose();
                //rp.TopLevel = false;
                this.Close();
            }
        }
    }
}
