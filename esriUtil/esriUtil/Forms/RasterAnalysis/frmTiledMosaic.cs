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
    public partial class frmTiledMosaic : Form
    {
        public frmTiledMosaic(IMap map)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            rsUtil = frmHlp.RasterUtility;
            geoUtil = frmHlp.GeoUtility;
            ftrUtil = frmHlp.FeatureUtility;
            ftrDic = frmHlp.FeatureDictionary;
            fillCombo();
        }

        private void fillCombo()
        {
            foreach (string s in ftrDic.Keys)
            {
                cmbFtrCls.Items.Add(s);
            }
        }
        private frmHelper frmHlp;
        private rasterUtil rsUtil;
        private geoDatabaseUtility geoUtil;
        private featureUtil ftrUtil;
        private Dictionary<string, IFeatureClass> ftrDic;

        private void btnFtrCls_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            string[] nm;
            string ftrPath = frmHlp.getPath(flt,out nm, false)[0];
            ftrDic[nm[0]] = geoUtil.getFeatureClass(ftrPath);
            cmbFtrCls.Items.Add(nm);
            cmbFtrCls.SelectedItem = nm;
        }

        private void btnImg_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterBasicTypesClass();
            string[] nm;
            string wksPath = frmHlp.getPath(flt, out nm, false)[0];
            txtImgWks.Text = wksPath;
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            string[] nm;
            string wksPath = frmHlp.getPath(flt, out nm, false)[0];
            txtOutWks.Text = wksPath;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string ftrPath = cmbFtrCls.Text;
            string imgPath = txtImgWks.Text;
            string outPath = txtOutWks.Text;
            if (ftrPath == "" || ftrPath == null)
            {
                MessageBox.Show("Missing Feature Class");
                return;
            }
            if (imgPath == "" || imgPath == null)
            {
                MessageBox.Show("Missing image directory\\workspace");
                return;
            }
            if (outPath == "" || outPath == null)
            {
                MessageBox.Show("Missing output workspace");
                return;
            }
            RunningProcess.frmRunningProcessDialog rd = new RunningProcess.frmRunningProcessDialog(false);
            this.Visible = false;
            rd.addMessage("Making Tiles. This may take a while....");
            rd.stepPGBar(10);
            rd.TopMost = true;
            DateTime dt = DateTime.Now;
            rd.Show();
            try
            {
                IFeatureClass ftrCls= ftrDic[ftrPath];
                IWorkspace imgWks = geoUtil.OpenRasterWorkspace(imgPath);
                IWorkspace outWks = geoUtil.OpenWorkSpace(outPath);
                IFunctionRasterDataset[] outDset = ftrUtil.tiledMosaics(ftrCls,imgWks,outWks);
                for (int i = 0; i < outDset.Length; i++)
			    {
                    IRasterLayer lyr = new RasterLayerClass();
                    lyr.Name = "Tile_" + (i+1).ToString();
                    lyr.CreateFromDataset((IRasterDataset)outDset[i]);
                    frmHlp.TheMap.AddLayer(lyr);
			    }
            }
            catch(Exception ex)
            {
                rd.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rd.stepPGBar(100);
                rd.addMessage("Finished Making Tiles" + t);
                rd.enableClose();
                this.Close();
                rd.stepPGBar(100);
                
            }


        }
    }
}
