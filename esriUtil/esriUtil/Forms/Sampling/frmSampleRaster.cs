using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;

namespace esriUtil.Forms.Sampling
{
    public partial class frmSampleRaster : Form
    {
        /// <summary>
        /// fills in raster value by band for each point location. Appends those values to a field named by the raster/band combination  
        /// </summary>
        /// <param name="map">The current focus map optionally it can = null</param>
        public frmSampleRaster(IMap map)
        {
            InitializeComponent();
            mp = map;
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            geoUtil = frmHlp.GeoUtility;
            ftrDic = frmHlp.FeatureDictionary;
            rstDic = frmHlp.FunctionRasterDictionary;
            ftrUtil = frmHlp.FeatureUtility;
            populateComboBox();
        }
        private frmHelper frmHlp = null;
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private featureUtil ftrUtil = null;
        private Dictionary<string, IFeatureClass> ftrDic = null;
        private Dictionary<string, IFunctionRasterDataset> rstDic = null;
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
                flt = new ESRI.ArcGIS.Catalog.GxFilterDatasets();
            }
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (featureClass)
                {
                    ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                    cmbSampleFeatureClass.Items.Add(outName);
                    cmbSampleFeatureClass.Text = outName;
                }
                else
                {
                    string wksPath = geoUtil.getDatabasePath(outPath);
                    IWorkspace wks = geoUtil.OpenWorkSpace(wksPath);
                    IEnumDataset rsDset = wks.get_Datasets(esriDatasetType.esriDTAny);
                    bool rsCheck = false;
                    IDataset ds = rsDset.Next();
                    while (ds != null)
                    {
                        if (outName.ToLower() == ds.Name.ToLower()&&(ds.Type== esriDatasetType.esriDTMosaicDataset||ds.Type==esriDatasetType.esriDTRasterDataset||ds.Type== esriDatasetType.esriDTRasterCatalog))
                        {
                            rsCheck=true;
                            break;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(ds);
                        ds = rsDset.Next();
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rsDset);
                    if (rsCheck)
                    {
                        rstDic[outName] = rsUtil.createIdentityRaster(outPath);
                        cmbRaster.Items.Add(outName);
                        cmbRaster.Text = outName;
                    }
                    else
                    {
                        ftrDic[outName] = geoUtil.getFeatureClass(outPath);
                        cmbRaster.Items.Add(outName);
                        cmbRaster.Text = outName;
                    }
                }

            }
            return;
        }
        private void populateComboBox()
        {
            if (mp != null)
            {
                foreach (KeyValuePair<string,IFeatureClass> kvp in ftrDic)
                {
                    IFeatureClass ftrCls = kvp.Value;
                    string nm = kvp.Key;
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint || ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryMultipoint)
                    {
                        cmbSampleFeatureClass.Items.Add(nm);
                    }
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                    {
                        cmbRaster.Items.Add(nm);
                    }
                }
                foreach (KeyValuePair<string, IFunctionRasterDataset> kvp in rstDic)
                {
                    IFunctionRasterDataset rsDet = kvp.Value;
                    string nm = kvp.Key;
                    cmbRaster.Items.Add(nm);
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

        private void btnSample_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string rstNm = cmbRaster.Text;
            
            if (smpFtrNm == null || smpFtrNm == "" || rstNm == "" || rstNm == null)
            {
                MessageBox.Show("sample location or raster are not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IFeatureClass ftrCls = ftrDic[smpFtrNm];
            string fldNm = cmbBandField.Text;
            this.Visible=false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Sampling layers");
            rp.addMessage("This may take a while...");
            rp.stepPGBar(20);
            rp.Show();
            rp.Refresh();
            DateTime dt1 = DateTime.Now;
            try
            {
                if (rstDic.Keys.Contains(rstNm))
                {
                    
                    if (ftrCls.Fields.FindField(fldNm) == -1)
                    {
                        fldNm = null;
                    }
                    rsUtil.sampleRaster(ftrCls, rstDic[rstNm], rstNm, fldNm);
                }
                else
                {
                    List<IField> flds = new List<IField>();
                    IFeatureClass ftrClsSamp = ftrDic[rstNm];
                    int fldIndex = ftrClsSamp.Fields.FindField(fldNm);
                    if (fldIndex == -1)
                    {
                        fldNm = null;
                    }
                    else
                    {
                        flds.Add(ftrClsSamp.Fields.get_Field(fldIndex));
                    }
                    if(fldNm==null)
                    {
                        for (int f = 0; f < ftrClsSamp.Fields.FieldCount; f++)
			            {
                            IField fld = ftrClsSamp.Fields.get_Field(f);
                            if(fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                            {
                                flds.Add(fld);
                            }
                            else
                            {
                                
                            }
			            }
                    }
                    ftrUtil.sampleFeatureClass(ftrCls, ftrDic[rstNm], flds.ToArray());
                }
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                string prcTime = "Time to complete process:\n" + ts.Days.ToString() + " Days " + ts.Hours.ToString() + " Hours " + ts.Minutes.ToString() + " Minutes " + ts.Seconds.ToString() + " Seconds ";
                rp.addMessage(prcTime);
                
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.enableClose();
                this.Close();
            }
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBandField.Items.Clear();
            if (rstDic.ContainsKey(cmbRaster.Text)&&ftrDic.ContainsKey(cmbSampleFeatureClass.Text))
            {
                string ftr = cmbSampleFeatureClass.Text;
                IFeatureClass ftrCls = ftrDic[ftr];
                IFields flds = ftrCls.Fields;
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                    {
                        cmbBandField.Items.Add(fld.Name);
                    }
                }
            }
            else
            {
                if (ftrDic.ContainsKey(cmbRaster.Text))
                {
                    IFeatureClass ftrCls = ftrDic[cmbRaster.Text];
                    IFields flds = ftrCls.Fields;
                    for (int i = 0; i < flds.FieldCount; i++)
                    {
                        IField fld = flds.get_Field(i);
                        if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                        {
                            cmbBandField.Items.Add(fld.Name);
                        }
                    }
                }
                else
                {
                    if (ftrDic.ContainsKey(cmbSampleFeatureClass.Text))
                    {
                        IFeatureClass ftrCls = ftrDic[cmbSampleFeatureClass.Text];
                        IFields flds = ftrCls.Fields;
                        for (int i = 0; i < flds.FieldCount; i++)
                        {
                            IField fld = flds.get_Field(i);
                            if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                            {
                                cmbBandField.Items.Add(fld.Name);
                            }
                        }
                    }
                }
            }
        }

        private void cmbRaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (rstDic.ContainsKey(cmbRaster.Text)&&ftrDic.ContainsKey(cmbSampleFeatureClass.Text))
            {
                cmbBandField.Items.Clear();
                IFeatureClass ftrCls = ftrDic[cmbSampleFeatureClass.Text];
                IFields flds = ftrCls.Fields;
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                    {
                        cmbBandField.Items.Add(fld.Name);
                    }
                }
            }
            else if (ftrDic.ContainsKey(cmbRaster.Text))
            {
                cmbBandField.Items.Clear();
                IFeatureClass ftrCls = ftrDic[cmbRaster.Text];
                IFields flds = ftrCls.Fields;
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                    {
                        cmbBandField.Items.Add(fld.Name);
                    }
                }
                
            }
            else
            {
            }
        }
    }
}
