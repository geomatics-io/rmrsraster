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

namespace esriUtil.Forms.Stats
{
    public partial class frmRunPolytomousLogisticRegression : Form
    {
        public frmRunPolytomousLogisticRegression(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rstUtil = new rasterUtil();
        private viewUtility vUtil = null;
        private Dictionary<string, IWorkspace> ftrDic = new Dictionary<string, IWorkspace>();
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        private polytomousLogisticRaster plrR = new polytomousLogisticRaster();
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
                flt = new ESRI.ArcGIS.Catalog.GxFilterWorkspacesClass();
                gxDialog.Title = "Select a Workspace";
            }
            else
            {
                flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
                gxDialog.Title = "Select a Raster";
            }
            gxDialog.ObjectFilter = flt;
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
                        ftrDic.Add(outName, geoUtil.OpenWorkSpace(outPath));
                        cmbSampleFeatureClass.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.OpenWorkSpace(outPath);
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
            if (!System.IO.File.Exists(plrR.SasOutputFile))
            {
                MessageBox.Show("You must first select a sample location that has been used to create a PLR model before you can use this button!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog frmD = new RunningProcess.frmRunningProcessDialog(false);
            frmD.stepPGBar(100);
            frmD.addMessage("Modeling parameters are as follows:");
            frmD.addMessage(String.Join("\n",plrR.OutParameters.ToArray()));
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
            IWorkspace wks = ftrDic[txt];
            string wksPath = wks.PathName;
            plrR.OutWorkspace = geoUtil.OpenRasterWorkspace(wksPath);
            cmbModelDir.Items.Clear();
            System.IO.DirectoryInfo dinfo = new System.IO.DirectoryInfo(wksPath + "\\SASOUTPUT");
            foreach (System.IO.DirectoryInfo dInfo2 in dinfo.GetDirectories())
            {
                cmbModelDir.Items.Add(dInfo2.Name);
            }
            cmbModelDir.SelectedItem = "GENERLIZED_LOGISTIC";
            
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ftrNm = cmbSampleFeatureClass.Text;
            if (ftrNm == "" || ftrNm == null)
            {
                MessageBox.Show("You must have a sampling layer selected");
                return;
            }
            if (!System.IO.File.Exists(plrR.SasOutputFile))
            {
                MessageBox.Show("Could not find the sas estimate file. You either needed to first create the classificaiton model or select a feature dataset that has been used to create the model!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int paramLength = plrR.OutParameters.Length;
            int rsBndsCnt = lstRasterBands.Items.Count;
            int bCnt = 0;
            for (int i = 0; i < rsBndsCnt; i++)
            {
                bCnt = bCnt + ((IRasterBandCollection)rstDic[lstRasterBands.Items[i].ToString()]).Count;
            }
            if (bCnt < 1||(paramLength-1)!=bCnt)
            {
                MessageBox.Show("Param = " + (paramLength-1).ToString() + " BandCount = " + bCnt.ToString() + "\nYou must have the same number of rasters selected in the same order as parameter estimates", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            plrR.InRaster = (IRaster)rsBc;
            
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new esriUtil.Forms.RunningProcess.frmRunningProcessDialog(false);
            System.DateTime dt1 = System.DateTime.Now;
            rp.addMessage("Creating PLR Classification Raster.");
            rp.addMessage("Bands are organized as follows:\n\tBand1=MLC\n\tBand2=BaseCategory");
            int bcCnt = 3;
            foreach (string c in plrR.Categories)
            {
                rp.addMessage("\tBand" + bcCnt.ToString() + "=" + c);
                bcCnt += 1;
            }
            rp.addMessage("This may take some time...");
            rp.Show();
            rp.TopMost = true;
            rp.stepPGBar(20);
            rp.Refresh();
            IRaster rst = plrR.createModelRaster(seed);
            if (mp != null)
            {
                rp.addMessage("Adding Raster to map");
                rp.Refresh();
                IRasterLayer rsLyr1 = new RasterLayerClass();
                rsLyr1.CreateFromRaster(rst);
                rsLyr1.Name = "PLR_OUT";
                rsLyr1.Visible = false;
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

        IRaster seed = null;
        private void btnSeed_Click(object sender, EventArgs e)
        {
            seed = getSeedRaster();

        }

        private IRaster getSeedRaster()
        {
            IRaster rsOut = null;
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Seed Raster";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                rsOut = rstUtil.returnRaster(outPath);
            }
            return rsOut;
        }

        private void cmbModelDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            string txt = cmbModelDir.Text;
            string estm = plrR.OutWorkspace.PathName + "\\SASOUTPUT\\" + txt + "\\outest.csv";
            if (System.IO.File.Exists(estm))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(estm))
                {
                    string sF = sr.ReadLine();
                    string sV = sr.ReadLine();
                    List<string> depFlsLst = new List<string>();
                    while (sV != null)
                    {
                        depFlsLst.Add(sV.Split(new char[] { ',' })[3]);
                        sV = sr.ReadLine();
                    }
                    plrR.Dependentfield = String.Join(" ", depFlsLst.ToArray());
                    sr.Close();

                }
                plrR.SasOutputFile = estm;
            }
            else
            {
                MessageBox.Show("A PLR Regression model has not been developed for this SAS run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbSampleFeatureClass.Items.Remove(txt);
                cmbSampleFeatureClass.Text = "";
            }
        }


    }
}
