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
    public partial class frmClipRaster : Form
    {
        public frmClipRaster(IMap map)
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
        public frmClipRaster(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        public Dictionary<string, IFeatureClass> FeatureDictionary { get { return ftrDic; } }
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private void getFeaturePath(bool featureClass)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (featureClass)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterGeoDatasetsClass();//.GxFilterPolygonFeatureClassesClass();
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (featureClass)
                {
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, geoUtil.getFeatureClass(outPath));
                        cmbSampleFeatureClass.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                    }
                    cmbSampleFeatureClass.Text = outName;
                }
                else
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rsUtil.returnRaster(outPath));
                        cmbRaster.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rsUtil.returnRaster(outPath);
                    }
                    cmbRaster.Text = outName;
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
            if (!cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer ftrLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = ftrLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    try
                    {
                        if (ftrCls != null)
                        {
                            if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                            {
                                if (!ftrDic.ContainsKey(lyrNm))
                                {
                                    ftrDic.Add(lyrNm, ftrCls);
                                    cmbSampleFeatureClass.Items.Add(lyrNm);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    lyr = ftrLyrs.Next();

                }
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rsUtil.createRaster(((IRaster2)rstLyr.Raster).RasterDataset);
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRaster.Items.Add(lyrNm);
                        cmbSampleFeatureClass.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
        }

        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePath(true);
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath(false);
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            string ftrNm = cmbSampleFeatureClass.Text;
            string rstNm = cmbRaster.Text;
            string outNm = txtOutRasterName.Text;
            bool chErase = chbErase.Checked;
            esriRasterClippingType clType = esriRasterClippingType.esriRasterClippingOutside;
            if (ftrNm == "" || ftrNm == null || rstNm == "" || rstNm == null)
            {
                MessageBox.Show("You must have a polygon and raster layer selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(outNm==""||outNm==null)
            {
                MessageBox.Show("You must specify an output raster name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(chErase)
            {
                clType= esriRasterClippingType.esriRasterClippingInside;
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Clipping Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            try
            {
                IRaster rst = rstDic[rstNm];
                IFeatureClass ftrCls = ftrDic[ftrNm];
                IGeometry geo = geoUtil.createGeometry(ftrCls);
                IFunctionRasterDataset outraster = rsUtil.clipRasterFunction(rst, geo, clType);
                if (mp != null&&addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromDataset((IRasterDataset)outraster);
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
                rp.addMessage("Finished Clipping Raster" + t);
                rp.enableClose();
                this.Close();
            }

        }
    }
}
