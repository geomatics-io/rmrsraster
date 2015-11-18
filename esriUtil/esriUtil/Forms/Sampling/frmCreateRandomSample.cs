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
            frmHlp = new frmHelper(map);
            geoUtil = frmHlp.GeoUtility;
            rstDic = frmHlp.FunctionRasterDictionary;
            rsUtil = frmHlp.RasterUtility;
            ftrDic = frmHlp.FeatureDictionary;
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
                btnOpenModel.Visible = true;
            }
        }
        private frmHelper frmHlp = null;
        private bool strata = false;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = null;
        private rasterUtil rsUtil = null;
        private Dictionary<string, IFunctionRasterDataset> rstDic = null;
        private Dictionary<string, IFeatureClass> ftrDic = null;
        private void loadCombos()
        {
            if (mp != null)
            {
                foreach (KeyValuePair<string, IFeatureClass> kvp in ftrDic)
                {
                    string nm = kvp.Key;
                    IFeatureClass ftCls = kvp.Value;
                    if (ftCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                    {
                        cmbRst.Items.Add(nm);
                    }
                }
                foreach (KeyValuePair<string, IFunctionRasterDataset> kvp in rstDic)
                {
                    string nm = kvp.Key;
                    IFunctionRasterDataset ftCls = kvp.Value;
                    cmbRst.Items.Add(nm);
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
                flt = new ESRI.ArcGIS.Catalog.GxFilterDatasetsClass();
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
                    string wksPath = geoUtil.getDatabasePath(outPath);
                    IWorkspace wks = geoUtil.OpenWorkSpace(wksPath);
                    IEnumDatasetName rsDsetName = wks.get_DatasetNames(esriDatasetType.esriDTRasterDataset);
                    bool rsCheck = false;
                    IDatasetName dsName = rsDsetName.Next();
                    while (dsName != null)
                    {
                        if (outName.ToLower() == dsName.Name.ToLower())
                        {
                            rsCheck=true;
                            break;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(dsName);
                        dsName = rsDsetName.Next();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rsDsetName);
                    if (rsCheck)
                    {

                        if (!rstDic.ContainsKey(outName))
                        {
                            rstDic.Add(outName, rsUtil.createIdentityRaster(outPath));
                            cmbRst.Items.Add(outName);
                        }
                        else
                        {
                            rstDic[outName] = rsUtil.createIdentityRaster(outPath);
                        }
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getFeatureClass(outPath);
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
            string mPath = txtSampleSize.Text;
            int[] numSample = null;
            double prop = System.Convert.ToDouble(nudProp.Value);
            double alpha = System.Convert.ToDouble(nudAlpha.Value);
            if (rsUtil.isNumeric(mPath))
            {
                numSample = new int[]{System.Convert.ToInt32(mPath)};
            }
            else
            {
                numSample = esriUtil.Statistics.dataPrepSampleSize.sampleSizeMaxCluster(mPath, prop, alpha);
            }
            if (rst == null || rst == "" || oWks == null || oWks == "")
            {
                MessageBox.Show("You must specify a raster, an output workspace, and a output file name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            bool rsCheck = false;
            if (rstDic.Keys.Contains(rst)) rsCheck = true;
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
                    if (rsCheck)
                    {
                        IFunctionRasterDataset rs = rstDic[rst];
                        rp.addMessage("Creating random stratified sample. This could take a while...");
                        rp.stepPGBar(10);
                        rp.Refresh();
                        ftrCls = rsUtil.createRandomSampleLocationsByClass(geoUtil.OpenWorkSpace(oWks), rs, numSample, 1, sNm);
                    }
                    else
                    {
                        rp.addMessage("Cannot create stratified random sample for feature class. Use raster instead!");
                    }
                    
                }
                else
                {
                    rp.addMessage("Creating random sample. This could take a while...");
                    rp.stepPGBar(10);
                    rp.Refresh();
                    if (rsCheck)
                    {
                        IFunctionRasterDataset rs = rstDic[rst];
                        ftrCls = rsUtil.createRandomSampleLocations(geoUtil.OpenWorkSpace(oWks), rs, numSample[0], sNm);
                    }
                    else
                    {
                        IFeatureClass inFtrCls = ftrDic[rst];
                        ftrCls = frmHlp.FeatureUtility.createRandomSample(inFtrCls, numSample[0], oWks + "\\" + sNm);
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {
            txtSampleSize.Text = esriUtil.Statistics.ModelHelper.openModelFileDialog();
        }

        private void txtSampleSize_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtSampleSize.Text))
            {
                nudProp.Visible = true;
                nudAlpha.Visible = true;
                lblAlpha.Visible = true;
                lblProp.Visible = true;
            }
            else
            {
                nudProp.Visible = false;
                nudAlpha.Visible = false;
                lblAlpha.Visible = false;
                lblProp.Visible = false;
            }
        }
    }
}
