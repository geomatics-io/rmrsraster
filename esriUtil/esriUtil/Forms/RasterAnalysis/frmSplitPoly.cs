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
    public partial class frmSplitPoly : Form
    {
        public frmSplitPoly(IMap map)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            geoUtil = frmHlp.GeoUtility;
            ftrUtil = frmHlp.FeatureUtility;
            fillCmb();
        }

        

        private rasterUtil rsUtil;
        private geoDatabaseUtility geoUtil;
        private frmHelper frmHlp;
        private featureUtil ftrUtil;
        private void btnInFtrCls_Click(object sender, EventArgs e)
        {
            string[] nm;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClasses();
            string outPath = frmHlp.getPath(flt, out nm)[0];
            cmbInFtrCls.Items.Add(nm[0]);
            frmHlp.FeatureDictionary[nm[0]] = geoUtil.getFeatureClass(outPath);
            cmbInFtrCls.Text = nm[0];

        }
        private void fillCmb()
        {
            foreach (string s in frmHlp.FeatureDictionary.Keys)
            {
                cmbInFtrCls.Items.Add(s);
            }
        }

        private void btnOutFtrCls_Click(object sender, EventArgs e)
        {
            string nm;
            ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClasses flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClasses();
            string outPath = frmHlp.getPathSave(flt, out nm);
            txtOutFtrCls.Text = outPath;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string inftrStr = cmbInFtrCls.Text;
            int splits = System.Convert.ToInt32(nudSplits.Value);
            string outFtrStr = txtOutFtrCls.Text;
            if (inftrStr == "" || inftrStr == null)
            {
                MessageBox.Show("Missing input Feature Class");
                return;
            }
            if (outFtrStr == "" || outFtrStr == null)
            {
                MessageBox.Show("Missing output Feature Class path");
                return;
            }
            RunningProcess.frmRunningProcessDialog rd = new RunningProcess.frmRunningProcessDialog(false);
            this.Visible = false;
            rd.addMessage("Splitting Feature Class. This may take a while....");
            rd.stepPGBar(10);
            rd.TopMost = true;
            DateTime dt = DateTime.Now;
            rd.Show();
            try
            {
                IFeatureClass ftrCls = frmHlp.FeatureDictionary[inftrStr];
                IFeatureClass outFtrCls = ftrUtil.splitPolyFeatures(ftrCls, splits, outFtrStr);
                IFeatureLayer lyr = new FeatureLayerClass();
                lyr.FeatureClass = outFtrCls;
                lyr.Name = ((IDataset)outFtrCls).Name;
                frmHlp.TheMap.AddLayer(lyr);
                
            }
            catch (Exception ex)
            {
                rd.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rd.stepPGBar(100);
                rd.addMessage("Finished splitting" + t);
                rd.enableClose();
                this.Close();
                rd.stepPGBar(100);

            }

        }
    }
}
