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
    public partial class frmFiaSummarizePoly : Form
    {
        public frmFiaSummarizePoly(IMap map)
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
        public frmFiaSummarizePoly(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private Dictionary<string, IFeatureClass> ftrDic = new Dictionary<string, IFeatureClass>();
        private void getRasterOrFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            fltColl.AddFilter(flt, true);
            fltColl.AddFilter(flt2, false);
            gxDialog.Title = "Select a Raster Polygon Dataset";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    if (gxDialog.ObjectFilter is ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass)
                    {
                        IRaster rs = rsUtil.returnRaster(outPath);
                        if (!rstDic.ContainsKey(outName))
                        {
                            rstDic.Add(outName, rs);
                            cmbStrata.Items.Add(outName);
                        }
                        else
                        {
                            rstDic[outName] = rs;
                        }
                    }
                    else
                    {
                        IFeatureClass ftrCls = geoUtil.getFeatureClass(outPath);
                        if (!ftrDic.ContainsKey(outName))
                        {
                            ftrDic.Add(outName, ftrCls);
                            cmbStrata.Items.Add(outName);
                        }
                        else
                        {
                            ftrDic[outName] = ftrCls;
                        }
                    }
                    gxObj = eGxObj.Next();
                }
                cmbStrata.SelectedItem = outName;
            }
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbStrata.Items.Contains(rstName))
            {
                cmbStrata.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbStrata.Items.Contains(rstName))
            {
                cmbStrata.Items.Remove(rstName);
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
                    IRaster rst = rsUtil.createRaster(((IRaster2)rstLyr.Raster).RasterDataset);

                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbStrata.Items.Add(lyrNm);
                    }
                    else
                    {
                        rstDic[lyrNm] = rst;
                    }

                    lyr = rstLyrs.Next();
                }
                rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer rstLyr = (IFeatureLayer)lyr;
                    IFeatureClass rst = rstLyr.FeatureClass;

                    if (!ftrDic.ContainsKey(lyrNm))
                    {
                        ftrDic.Add(lyrNm, rst);
                        if (rst.ShapeType == esriGeometryType.esriGeometryPolygon)
                        {
                            cmbStrata.Items.Add(lyrNm);
                            cmbStands.Items.Add(lyrNm);
                        }
                        if (rst.ShapeType == esriGeometryType.esriGeometryPoint)
                        {
                            cmbPlots.Items.Add(lyrNm);
                        }
                    }
                    else
                    {
                        ftrDic[lyrNm] = rst;
                    }
                    lyr = rstLyrs.Next();
                }

            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string strataStr = cmbStrata.Text;
            string plotsStr = cmbPlots.Text;
            string standsStr = cmbStands.Text;
            if (strataStr == null || strataStr == "")
            {
                MessageBox.Show("You must specify a Stratification layer", "No Zone", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (plotsStr == null || plotsStr == "")
            {
                MessageBox.Show("You must specify a plots layer", "No Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (lsbFields.Items.Count < 1 )
            {
                MessageBox.Show("You must select at least on Field to summarize", "No Fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<IField> fldLst = new List<IField>();
            IFeatureClass pltFtrCls = ftrDic[plotsStr];
            for (int i = 0; i < lsbFields.Items.Count; i++)
            {
                IField fld  = pltFtrCls.Fields.get_Field(pltFtrCls.FindField(lsbFields.Items[i].ToString()));
                fldLst.Add(fld);
            }
            this.Visible = false;
            

            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Summarizing plots. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            //Statistics.ModelHelper.runProgressBar("Summarizing");
            try
            {
                IFeatureClass standFtrCls = null;
                if(ftrDic.ContainsKey(standsStr))
                {
                    standFtrCls = ftrDic[standsStr];
                }
                if (rstDic.ContainsKey(strataStr))
                {
                    rp.addMessage("Summarizing FIA plots using raster...");
                    IRaster strataRs = rstDic[strataStr];
                    IFunctionRasterDataset stratafDset = rsUtil.createIdentityRaster(strataRs);
                    //outTbl = rsUtil.zonalStats(zRs, vRs, oTbl, rsLst.ToArray(), rp, chbClassCounts.Checked);
                    fiaIntegration.summarizeBiomassPolygon(pltFtrCls, fldLst.ToArray(), stratafDset, standFtrCls, geoUtil, rsUtil);
                }
                else
                {
                    rp.addMessage("Summarizing FIA plots using polygon");
                    rp.Refresh();
                    IFeatureClass strataFtr = ftrDic[strataStr];
                    //outTbl = rsUtil.zonalStats(zFtr, zFld, vRs, oTbl, rsLst.ToArray(), rp, chbClassCounts.Checked);
                    fiaIntegration.summarizeBiomassPolygon(pltFtrCls, fldLst.ToArray(),strataFtr, standFtrCls, geoUtil);
                }
                if (mp != null && addToMap)
                {

                    rp.Refresh();

                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                Statistics.ModelHelper.closeProgressBar();
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished summarizing" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection s = lsbFields.SelectedItems;
            int cnt = s.Count;
            List<string> rLst = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                string txt = s[i].ToString();
                rLst.Add(txt);
                if (txt != null && txt != "")
                {
                    if (!cmbFields.Items.Contains(txt))
                    {
                        cmbFields.Items.Add(txt);
                    }
                }
            }
            foreach (string r in rLst)
            {
                lsbFields.Items.Remove(r);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsbFields.Items.Count; i++)
            {
                cmbFields.Items.Add(lsbFields.Items[i]);
            }
            lsbFields.Items.Clear();

        }

        private void cmbZonalStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            object itVl = cmbFields.SelectedItem;
            lsbFields.Items.Add(itVl);
            cmbFields.Items.Remove(itVl);
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getRasterOrFeaturePath();
        }

        

        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbFields.Items.Count; i++)
            {
                lsbFields.Items.Add(cmbFields.Items[i]);
            }
            cmbFields.Items.Clear();
        }

        private void cmbZoneRaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            string plts = cmbPlots.Text;
            IFeatureClass ftrCls = ftrDic[plts];
            IFields flds = ftrCls.Fields;
            cmbFields.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                esriFieldType fldType = fld.Type;
                if (fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeDouble || fldType == esriFieldType.esriFieldTypeSingle || fldType == esriFieldType.esriFieldTypeSmallInteger)
                {
                    cmbFields.Items.Add(fld.Name);
                }
            }
            //cmbFields.SelectedItem = cmbFields.Items[0];
        }

        private void btnOpenPlots_Click(object sender, EventArgs e)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterPointFeatureClassesClass();
            fltColl.AddFilter(flt2, false);
            gxDialog.Title = "Select a Point Feature Class";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    
                    IFeatureClass ftrCls = geoUtil.getFeatureClass(outPath);
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, ftrCls);
                        cmbPlots.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = ftrCls;
                    }
                    
                    gxObj = eGxObj.Next();
                }
                cmbPlots.SelectedItem = outName;
            }
            return;
        }

        private void btnOpenStands_Click(object sender, EventArgs e)
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            ESRI.ArcGIS.Catalog.IGxObjectFilterCollection fltColl = (ESRI.ArcGIS.Catalog.IGxObjectFilterCollection)gxDialog;
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt2 = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            fltColl.AddFilter(flt2, false);
            gxDialog.Title = "Select a Stands Feature Class";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    
                    IFeatureClass ftrCls = geoUtil.getFeatureClass(outPath);
                    if (!ftrDic.ContainsKey(outName))
                    {
                        ftrDic.Add(outName, ftrCls);
                        cmbStands.Items.Add(outName);
                    }
                    else
                    {
                        ftrDic[outName] = ftrCls;
                    }
                    
                    gxObj = eGxObj.Next();
                }
                cmbStands.SelectedItem = outName;
            }
            return;
        }
    }
}
