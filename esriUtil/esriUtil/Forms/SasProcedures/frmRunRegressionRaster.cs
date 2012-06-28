using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace esriUtil.Forms.SasProcedures
{
    public partial class frmRunRegressionRaster : Form
    {
        public frmRunRegressionRaster(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private SasProcedure sP = SasProcedure.REGRESSION;
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rstUtil = new rasterUtil();
        private viewUtility vUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private regressionRaster rR = new regressionRaster();
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
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                    {
                        if (!ftrDic.ContainsKey(lyrNm))
                        {
                            ftrDic.Add(lyrNm, ftrCls);
                            cmbSampleFeatureClass.Items.Add(lyrNm);
                        }
                    }
                    lyr = ftrLyrs.Next();

                }
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rstLyr.Raster;
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRasterBands.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
        }
        private void getFeaturePath(bool featureClass)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            if (featureClass)
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
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
                    cmbSampleFeatureClass.SelectedItem = outName;
                }
                else
                {
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rstUtil.returnRaster(outPath));
                        cmbRasterBands.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rstUtil.returnRaster(outPath);
                    }
                    cmbRasterBands.Text = outName;
                }

            }
            return;
        }
        private void btnOpenFeature_Click(object sender, EventArgs e)
        {
            getFeaturePath(true);
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            string txt = cmbRasterBands.Text;
            if (txt != null && txt != "")
            {
                cmbRasterBands.Items.Remove(txt);
                if (!lstRasterBands.Items.Contains(txt))
                {
                    lstRasterBands.Items.Add(txt);
                }
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            string txt = lstRasterBands.SelectedItem.ToString();
            if (txt != null && txt != "")
            {
                lstRasterBands.Items.Remove(txt);
                if (!cmbRasterBands.Items.Contains(txt))
                {
                    cmbRasterBands.Items.Add(txt);
                }
            }


        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbRasterBands.Items.Count; i++)
            {
                string st = cmbRasterBands.Items[i].ToString();
                lstRasterBands.Items.Add(st);
            }
            cmbRasterBands.Items.Clear();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstRasterBands.Items.Count; i++)
            {
                string st = lstRasterBands.Items[i].ToString();
                cmbRasterBands.Items.Add(st);
            }
            lstRasterBands.Items.Clear();

        }

        private void btnRasterBands_Click(object sender, EventArgs e)
        {
            getFeaturePath(false);
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(rR.SasOutputFile))
            {
                MessageBox.Show("You must first select a sample location that has been used to create a PLR model before you can use this button!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog frmD = new RunningProcess.frmRunningProcessDialog(false);
            frmD.stepPGBar(100);
            frmD.addMessage("Modeling parameters are as follows:");
            //frmD.addMessage("output estimates = " + rR.SasOutputFile);
            //frmD.addMessage("output categories = " + rR.Dependentfield);
            frmD.addMessage(String.Join("\n", rR.OutParameters.ToArray()));
            frmD.addMessage("Raster layers (ignoring intercept) must be specified in the same order!");
            frmD.Show();
            frmD.TopLevel = true;
            frmD.enableClose();
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = cmbSampleFeatureClass.Text;
            if (txt == null || txt == "")
            {
                return;
            }
            IFeatureClass ftrCls = ftrDic[txt];
            sasIntegration sInt = new sasIntegration(ftrCls, sP);
            rR.OutWorkspace = ((IDataset)ftrCls).Workspace;
            string estm = sInt.OutEstimatesPath;
            if (System.IO.File.Exists(estm))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(estm))
                {
                    string sF = sr.ReadLine();
                    string sV = sr.ReadLine();
                    List<string> depFlsLst = new List<string>() ;
                    while (sV != null)
                    {
                        depFlsLst.Add(sV.Split(new char[] { ',' })[2]);
                        sV = sr.ReadLine();
                    }
                    rR.Dependentfield = String.Join(" ", depFlsLst.ToArray());
                    sr.Close();

                }
                rR.SasOutputFile = estm;
            }
            else
            {
                MessageBox.Show("A Regression model has not been developed for this feature class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbSampleFeatureClass.Items.Remove(txt);
                cmbSampleFeatureClass.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ftrNm = cmbSampleFeatureClass.Text;
            if (ftrNm == "" || ftrNm == null)
            {
                MessageBox.Show("You must have a sampling layer selected");
                return;
            }
            if (!System.IO.File.Exists(rR.SasOutputFile))
            {
                MessageBox.Show("Could not find the sas estimate file. You either needed to first create the classificaiton model or select a feature dataset that has been used to create the model!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int paramLength = rR.OutParameters.Length;
            int rsBndsCnt = lstRasterBands.Items.Count;
            int bCnt = 0;
            for (int i = 0; i < rsBndsCnt; i++)
            {
                bCnt = bCnt + ((IRasterBandCollection)rstDic[lstRasterBands.Items[i].ToString()]).Count;
            }
            if (bCnt < 1 || (paramLength - 1) != bCnt)
            {
                MessageBox.Show("Param = " + (paramLength - 1).ToString() + " BandCount = " + bCnt.ToString() + "\nYou must have the same number of rasters selected in the same order as parameter estimates", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnViewOrder.PerformClick();
                return;

            }
            IRasterBandCollection rsBc = new RasterClass();
            for (int i = 0; i < rsBndsCnt; i++)
            {
                IRaster rs = rstDic[lstRasterBands.Items[i].ToString()];
                rsBc.AppendBands((IRasterBandCollection)rs);
            }
            this.Visible = false;
            rR.InRaster = (IRaster)rsBc;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new esriUtil.Forms.RunningProcess.frmRunningProcessDialog(false);
            System.DateTime dt1 = System.DateTime.Now;
            rp.addMessage("Creating Regression Raster.");
            rp.addMessage("Location of the output workspace = " + rR.OutWorkspace.PathName);
            rp.addMessage("Name of the out raster = REG. Bands are in the order of dependent variables");
            rp.addMessage("This may take some time...");
            rp.Show();
            rp.TopMost = true;
            rp.stepPGBar(20);
            rp.Refresh();
            IRaster rstArr = rR.createModelRaster();
            if (mp != null)
            {
                rp.addMessage("Adding Rasters to map");
                rp.Refresh();
                IRasterLayer rsLyr1 = new RasterLayerClass();
                rsLyr1.CreateFromRaster(rstArr);
                mp.AddLayer(rsLyr1);
            }
            rp.stepPGBar(50);
            rp.addMessage("Finished creating raster");
            rp.Refresh();
            System.DateTime dt2 = System.DateTime.Now;
            System.TimeSpan ts = dt2.Subtract(dt1);
            string prcTime = "Time to complete process:\n" + ts.Days.ToString() + " Days " + ts.Hours.ToString() + " Hours " + ts.Minutes.ToString() + " Minutes " + ts.Seconds.ToString() + " Seconds ";
            rp.addMessage(prcTime);
            rp.stepPGBar(100);
            rp.Refresh();
            rp.enableClose();
            this.Close();
            return;
        }
    }
}
