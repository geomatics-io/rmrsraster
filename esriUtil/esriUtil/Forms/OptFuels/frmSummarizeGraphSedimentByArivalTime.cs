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

namespace esriUtil.Forms.OptFuels
{
    public partial class frmSummarizeGraphSedimentByArivalTime : Form
    {
        public frmSummarizeGraphSedimentByArivalTime()
        {
            InitializeComponent();
            mp = null;
            rsUtil = new rasterUtil();
            populateComboBox();
        }
        public frmSummarizeGraphSedimentByArivalTime(IMap map, rasterUtil rasterUtility)
        {
            InitializeComponent();
            mp = map;
            vUtil = new viewUtility((IActiveView)map);
            rsUtil = rasterUtility;
            populateComboBox();
        }
        private graphSedimentByArivalTime gSed = null;
        private IMap mp = null;
        private rasterUtil rsUtil = null;
        private viewUtility vUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private Dictionary<string, string> prjDic = new Dictionary<string, string>();
        private Dictionary<string, string> scnDic = new Dictionary<string, string>();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a RCZ Layer";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                string nm = gxObj.Name;
                IFeatureClass ftrCls = geoUtil.getFeatureClass(gxObj.FullName);
                ftrDic.Add(nm,ftrCls);
                cmbInFeatureClass.Items.Add(nm);
                cmbInFeatureClass.SelectedItem = nm;
            }
        }
        private void populateComboBox()
        {
            gSed = new graphSedimentByArivalTime(rsUtil, mp);
            if (mp != null)
            {
                IEnumLayer ftrLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = ftrLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                    {
                        if (!ftrDic.ContainsKey(lyrNm))
                        {
                            ftrDic.Add(lyrNm, ftrCls);
                            cmbInFeatureClass.Items.Add(lyrNm);
                        }
                    }
                    lyr = ftrLyrs.Next();
                }
            }
            string magfireDir = @"c:\magfire";
            if(System.IO.Directory.Exists(magfireDir))
            {
                System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(magfireDir);
                foreach(System.IO.DirectoryInfo d in dInfo.GetDirectories())
                {
                    string dirPath = d.FullName + "\\Input";
                    System.IO.DirectoryInfo d2 = new System.IO.DirectoryInfo(dirPath);
                    if (d2.Exists)
                    {
                        foreach (System.IO.DirectoryInfo d3 in d2.GetDirectories())
                        {
                            string dirPath2 = d3.FullName + "\\RESULTS";
                            System.IO.DirectoryInfo d4 = new System.IO.DirectoryInfo(dirPath2);
                            if (d4.Exists)
                            {
                                cmbProject.Items.Add(d.Name);
                                prjDic.Add(d.Name, d.FullName);
                                break;
                            }
                        }
                    }
                }
            }
            string defaultRcz = magfireDir + "\\rcz\\RCZ70_stands5d.shp";
            IFeatureClass defRcz = geoUtil.getFeatureClass(defaultRcz);
            defaultRcz = ((IDataset)defRcz).BrowseName;
            if (defRcz != null)
            {
                ftrDic.Add(defaultRcz, defRcz);
                cmbInFeatureClass.Items.Add(defaultRcz);
                cmbInFeatureClass.SelectedItem = defaultRcz;
            }
            //foreach (string s in Enum.GetNames(typeof(graphSedimentByArivalTime.ArrivalClasses)))
            //{
            //    cmbArrival.Items.Add(s);
            //}
            //cmbArrival.SelectedItem = cmbArrival.Items[0];

        }

        private void cmbInFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ftrName = cmbInFeatureClass.Text;
            if (ftrName == "" || ftrName == null)
            {
                return;
            }
            IFeatureClass ftrCls = ftrDic[cmbInFeatureClass.Text];
            IFields flds = ftrCls.Fields;
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                esriFieldType fldType = fld.Type;
                if (fldType == esriFieldType.esriFieldTypeDouble || fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeSmallInteger || fldType == esriFieldType.esriFieldTypeSingle)
                {
                    string fldNm = fld.Name;
                    cmbF10.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "NA10") cmbF10.SelectedItem = fldNm;
                    cmbF50.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "NA50") cmbF50.SelectedItem = fldNm;
                    cmbHSF10.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "HS10") cmbHSF10.SelectedItem = fldNm;
                    cmbHSF50.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "HS50") cmbHSF50.SelectedItem = fldNm;
                    cmbT10.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "TR10") cmbT10.SelectedItem = fldNm;
                    cmbT50.Items.Add(fldNm);
                    if (fldNm.ToUpper() == "TR50") cmbT50.SelectedItem = fldNm;
                }
            }


        }
        private string resultsDir = "";
        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            scnDic.Clear();
            cmbScenerio.Items.Clear();
            cmbScenerio.Text = "";
            resultsDir = "";
            string inputDirectory = prjDic[cmbProject.Text]+"\\Input";
            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(inputDirectory);
            foreach (System.IO.DirectoryInfo d in dInfo.GetDirectories())
            {
                string dirPath = d.FullName + "\\RESULTS";
                if (System.IO.Directory.Exists(dirPath))
                {
                    scnDic.Add(d.Name, d.FullName);
                    cmbScenerio.Items.Add(d.Name);
                }
            }
            

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(resultsDir))
            {
                MessageBox.Show("You must select both a OpFuels Project and Scenerio before you can execute.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //gSed.CellSize = System.Convert.ToDouble(nudCellSize.Value);
            //gSed.ArrivalClass = (graphSedimentByArivalTime.ArrivalClasses)Enum.Parse(typeof(graphSedimentByArivalTime.ArrivalClasses),cmbArrival.Text);
            gSed.CreateIntermediateRasters = chbIntermidiate.Checked;
            gSed.CreateCoreRasters = chbCore.Checked;
            gSed.FlameLength = System.Convert.ToDouble(nudFlameLength.Value);
            if (cmbF10.Text != "")
            {
                gSed.Fine10Field = cmbF10.Text;
            }
            else
            {
                MessageBox.Show("You must specify a NA10 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbF50.Text != "")
            {
                gSed.Fine50Field = cmbF50.Text;
            }
            else
            {
                MessageBox.Show("You must specify a NA50 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbT10.Text != "")
            {
                gSed.T1_Fine_10Field = cmbT10.Text;
            }
            else
            {
                MessageBox.Show("You must specify a TR10 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbT50.Text != "")
            {
                gSed.T1_Fine_50Field = cmbT50.Text;
            }
            else
            {
                MessageBox.Show("You must specify a TR50 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbHSF10.Text != "")
            {
                gSed.HSF10Field = cmbHSF10.Text;
            }
            else
            {
                MessageBox.Show("You must specify a HS10 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbHSF50.Text != "")
            {
                gSed.HSF50Field = cmbHSF50.Text;
            }
            else
            {
                MessageBox.Show("You must specify a HS50 field value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbInFeatureClass.Text == "" || cmbInFeatureClass.Text == null)
            {
                MessageBox.Show("You must specify a sediment polygon.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Need to add the progress form
            this.Visible = false;
            this.Refresh();
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rd = new RunningProcess.frmRunningProcessDialog(false);
            rd.addMessage("Creating sediment by arrival time table");
            rd.stepPGBar(10);
            rd.Refresh();
            rd.Show();
            DateTime dt1 = DateTime.Now;
            try
            {
                rd.addMessage("Setting up directories...");
                rd.Refresh();
                gSed.ResultsDir = resultsDir;
                rd.stepPGBar(10);
                rd.addMessage("Output directroy = " + gSed.GraphDir+ "\nCreating Sediment Rasters...");
                rd.Refresh();
                gSed.RunningProcessForm = rd;
                gSed.StreamPolygon = ftrDic[cmbInFeatureClass.Text];
                rd.stepPGBar(10);
                rd.addMessage("Summarizing sediment by arrival time. This may take a while...");
                rd.Refresh();
                gSed.sumSedimentValues();
            }
            catch (Exception ex)
            {
                rd.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                string totalTime = "Processing took " + ts.TotalMinutes + " minutes to complete";
                rd.addMessage(totalTime);
                rd.addMessage("Finished making table");
                rd.stepPGBar(100);
                rd.enableClose();
                this.Close();
            }


        }

        private void cmbScenerio_SelectedIndexChanged(object sender, EventArgs e)
        {
            resultsDir = scnDic[cmbScenerio.Text] + "\\RESULTS"; 
        }

        private void chbAO_CheckedChanged(object sender, EventArgs e)
        {
            if (chbAO.Checked)
            {
                groupBox2.Visible = true;
            }
            else
            {
                groupBox2.Visible = false;
            }
        }



    }
}
