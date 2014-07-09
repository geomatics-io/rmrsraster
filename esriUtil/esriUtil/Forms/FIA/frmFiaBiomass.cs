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

namespace esriUtil.Forms.FIA
{
    public partial class frmFiaBiomass : Form
    {
        //private enum biomassTypes { BAA, TPA, Total, Stem, Stump, Foliage, Top };
        public frmFiaBiomass(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
            
        }
        ~frmFiaBiomass()
        {
            ofd.Dispose();
        }
        
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClasses();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
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
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer ftrLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = ftrLyrs.Next();
                while (lyr != null)
                {
                    try
                    {
                        string lyrNm = lyr.Name;
                        IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                        IFeatureClass ftrCls = ftrLyr.FeatureClass;
                        if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                        {
                            Console.WriteLine(lyrNm);
                            Console.WriteLine(((IDataset)ftrLyr).Workspace.PathName);
                            if (!ftrDic.ContainsKey(lyrNm))
                            {
                                ftrDic.Add(lyrNm, ftrCls);
                                cmbSampleFeatureClass.Items.Add(lyrNm);
                                
                            }
                        }
                    }
                    catch (Exception e)
                    {
 
                        Console.WriteLine(e.ToString());
                        break;
                    }
                    lyr = ftrLyrs.Next();

                }
            }
            foreach (string s in Enum.GetNames(typeof(fiaIntegration.biomassTypes)))
            {
                try
                {
                    clbBiomassTypes.Items.Add(s, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
        }
        private OpenFileDialog ofd = new OpenFileDialog();
        private void btnOpenFeatureClass_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string ftrNm = cmbSampleFeatureClass.Text;
            string plotFldNm = cmbPlot.Text;
            string subPlotFldNm = cmbSubPlot.Text;
            string fiaDbNm = txtAccessDb.Text;
            if (ftrNm == "" || ftrNm == null || fiaDbNm == "" || fiaDbNm == null)
            {
                MessageBox.Show("You must have a sample file and FIA database selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (plotFldNm == "" || plotFldNm == null)
            {
                MessageBox.Show("You must have a plot field selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (clbBiomassTypes.CheckedItems.Count < 1)
            {
                MessageBox.Show("You must check at least 1 biomass type", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            List<fiaIntegration.biomassTypes> bTypes = new List<fiaIntegration.biomassTypes>();
            foreach (string s in clbBiomassTypes.CheckedItems)
            {
                //int vl = System.Convert.ToInt32(Enum.Parse(typeof(biomassTypes),s));
                fiaIntegration.biomassTypes bT = (fiaIntegration.biomassTypes)Enum.Parse(typeof(fiaIntegration.biomassTypes),s);
                bTypes.Add(bT);
            }
            this.Visible = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Calculating Biomass. This may take a while...");
            rp.stepPGBar(30);
            rp.TopMost = true;
            rp.Show();
            rp.Refresh();
            try
            {
                fiaIntegration fiaInt = new fiaIntegration(fiaDbNm);
                fiaInt.PlotCnField = plotFldNm;
                fiaInt.SubPlotField = subPlotFldNm;
                IFeatureClass ftrCls = ftrDic[ftrNm];
                fiaInt.BiomassTypes = bTypes.ToArray();
                fiaInt.SampleFeatureClass = ftrCls;
                fiaInt.summarizeBiomass();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished Estimating Biomass" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnAccessDb_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Access db|*.mdb;*.accdb";
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtAccessDb.Text = ofd.FileName;
            }
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbSampleFeatureClass.Text!=""&&cmbSampleFeatureClass.Text!=null)
            cmbPlot.Items.Clear();
            cmbSubPlot.Items.Clear();
            IFeatureClass ftrCls = ftrDic[cmbSampleFeatureClass.Text];
            IFields flds = ftrCls.Fields;
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                esriFieldType fldType = fld.Type;
                string fldNm = fld.Name;
                if (fldType == esriFieldType.esriFieldTypeString)
                {
                    cmbPlot.Items.Add(fldNm);
                }
                else if (fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeSingle || fldType == esriFieldType.esriFieldTypeSmallInteger || fldType == esriFieldType.esriFieldTypeDouble)
                {
                    cmbSubPlot.Items.Add(fldNm);
                }
                else
                {
                }
            }
        }
    }
}
