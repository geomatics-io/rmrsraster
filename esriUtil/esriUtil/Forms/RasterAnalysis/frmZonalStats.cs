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
    public partial class frmZonalStats : Form
    {
        public frmZonalStats(IMap map)
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
        public frmZonalStats(IMap map, ref rasterUtil rasterUtility, bool AddToMap)
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
                            cmbZoneRaster.Items.Add(outName);
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
                            cmbZoneRaster.Items.Add(outName);
                        }
                        else
                        {
                            ftrDic[outName] = ftrCls;
                        }
                    }
                    gxObj = eGxObj.Next();
                }
                cmbZoneRaster.SelectedItem = outName;
            }
            return;
        }
        private void getRasterPath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Raster Dataset";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outPath = gxObj.FullName;
                    outName = gxObj.BaseName;
                    IRaster rs = rsUtil.returnRaster(outPath);
                    if (!rstDic.ContainsKey(outName))
                    {
                        rstDic.Add(outName, rs);
                        cmbValueRaster.Items.Add(outName);
                    }
                    else
                    {
                        rstDic[outName] = rs;
                    }
                    gxObj = eGxObj.Next();
                }
            }
            cmbValueRaster.SelectedItem = outName;
            return;
        }
        private IRaster outraster = null;
        public IRaster OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbZoneRaster.Items.Contains(rstName))
            {
                cmbZoneRaster.Items.Add(rstName);
                rstDic[rstName] = rst;
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbZoneRaster.Items.Contains(rstName))
            {
                cmbZoneRaster.Items.Remove(rstName);
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
                    IRaster rst = rstLyr.Raster;
                    
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbZoneRaster.Items.Add(lyrNm);
                        cmbValueRaster.Items.Add(lyrNm);
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
                        cmbZoneRaster.Items.Add(lyrNm);
                    }
                    else
                    {
                        ftrDic[lyrNm] = rst;
                    }
                    lyr = rstLyrs.Next();
                }
                foreach(string s in Enum.GetNames(typeof(rasterUtil.zoneType)))
                {
                    cmbZonalStat.Items.Add(s);
                }

            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string zNm = cmbZoneRaster.Text;
            string vNm = cmbValueRaster.Text;
            string zFld = cmbZoneField.Text;
            if (zNm == null || zNm == "")
            {
                MessageBox.Show("You must specify a zone layer","No Zone",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if(ftrDic.ContainsKey(zNm))
            {
                if(zFld==""||zFld==null)
                {
                    MessageBox.Show("You must specify a field for the feature class","No Zone",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }
            if (vNm == null || vNm == "")
            {
                MessageBox.Show("You must specify a value raster","No Value",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (lsbStats.Items.Count < 1)
            {
                MessageBox.Show("You must select at least on Zonal Statistic", "No Stats", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<rasterUtil.zoneType> rsLst = new List<rasterUtil.zoneType>();
            for (int i = 0; i < lsbStats.Items.Count; i++)
            {
                rsLst.Add((rasterUtil.zoneType)Enum.Parse(typeof(rasterUtil.zoneType),lsbStats.Items[i].ToString()));
            }
            this.Visible = false;
            IRaster vRs = rstDic[vNm];
            
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Calculating Zonal Statistics. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            rp.Refresh();
            try
            {
                ITable outTbl = null;
                if(rstDic.ContainsKey(zNm))
                {
                    IRaster zRs = rstDic[zNm];
                    outTbl = rsUtil.zonalStats(zRs,vRs,rsLst.ToArray(),rp);
                }
                else
                {
                    rp.addMessage("Converting feature class to raster...");
                    rp.Refresh();
                    IFeatureClass zFtr = ftrDic[zNm];
                    outTbl = rsUtil.zonalStats(zFtr,zFld,vRs,rsLst.ToArray(),rp);
                }
                if (mp != null&&addToMap)
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
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished Zonal Stats" + t);
                rp.enableClose();
                this.Close();
            }

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            object itVl = lsbStats.SelectedItem;
            cmbZonalStat.Items.Add(itVl);
            lsbStats.Items.Remove(itVl);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lsbStats.Items.Count; i++)
            {
                cmbZonalStat.Items.Add(lsbStats.Items[i]);
            }
            lsbStats.Items.Clear();

        }

        private void cmbZonalStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            object itVl = cmbZonalStat.SelectedItem;
            lsbStats.Items.Add(itVl);
            cmbZonalStat.Items.Remove(itVl);
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getRasterOrFeaturePath();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getRasterPath();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbZonalStat.Items.Count; i++)
            {
                lsbStats.Items.Add(cmbZonalStat.Items[i]);
            }
            cmbZonalStat.Items.Clear();
        }

        private void cmbZoneRaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            string zNm = cmbZoneRaster.Text;
            if (rstDic.ContainsKey(zNm))
            {
                cmbZoneField.Items.Clear();
                cmbZoneField.Text = "";
                cmbZoneField.Visible = false;
                lblZone.Visible = false;
            }
            else
            {
                IFeatureClass ftrCls = ftrDic[zNm];
                IFields flds = ftrCls.Fields;
                cmbZoneField.Items.Clear();
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    esriFieldType fldType = fld.Type;
                    if (fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeDouble || fldType == esriFieldType.esriFieldTypeSingle || fldType == esriFieldType.esriFieldTypeSmallInteger || fldType == esriFieldType.esriFieldTypeOID)
                    {
                        cmbZoneField.Items.Add(fld.Name);
                    }
                }
                cmbZoneField.SelectedItem = ftrCls.OIDFieldName;
                cmbZoneField.Visible = true;
                lblZone.Visible = true;
            }
        }


    }
}
