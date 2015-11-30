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

namespace esriUtil.Forms.MapServices
{
    public partial class frmDownloadMapServiceLayer : Form
    {
        public frmDownloadMapServiceLayer(IMap map)
        {
            InitializeComponent();
            frmHlp = new frmHelper(map);
            geoUtil = frmHlp.GeoUtility;
            rsUtil = frmHlp.RasterUtility;
            fillCmb();
        }

        
        frmHelper frmHlp = null;
        geoDatabaseUtility geoUtil;
        rasterUtil rsUtil;
        Dictionary<string, IMapServerLayer> mpSrvDic = new Dictionary<string, IMapServerLayer>();
        Dictionary<string, int> mpLayerDic = new Dictionary<string, int>();
        private void fillCmb()
        {
            for (int i = 0; i < frmHlp.TheMap.LayerCount; i++)
            {
                ILayer lyr = frmHlp.TheMap.Layer[i];
                if (lyr is MapServerLayer)
                {
                    mpSrvDic[lyr.Name] = (IMapServerLayer)lyr;
                    cmbService.Items.Add(lyr.Name);
                }

            }
        }
        private IMapServer mpSvr = null;
        private string mpName = "";
        private void cmbService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(mpSrvDic.ContainsKey(cmbService.Text))
            {
                IMapServerLayer mpSvrLyr = mpSrvDic[cmbService.Text];
                ESRI.ArcGIS.GISClient.IAGSServerObjectName svrObjName;
                string docLocation;
                mpSvrLyr.GetConnectionInfo(out svrObjName,out docLocation, out mpName);
                //MessageBox.Show("MapName = " + mpName);
                mpSvr = mapserviceutility.getMapServer(svrObjName);
                cmbLayer.Items.Clear();
                mpLayerDic = mapserviceutility.getLayerIds(mpSvr, mpName, true);
                foreach (string s in mpLayerDic.Keys)
                {
                    cmbLayer.Items.Add(s);
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string svrStr = cmbService.Text;
            string lyrStr = cmbLayer.Text;
            string outPath = txtOutFtrCls.Text;
            bool clip = chbDisplayExtent.Checked;
            if(mpSvr==null)
            {
                MessageBox.Show("No Map server selected");
                return;
            }
            if(!mpLayerDic.ContainsKey(lyrStr))
            {
                MessageBox.Show("No Layer selected");
            }
            RunningProcess.frmRunningProcessDialog rd = new RunningProcess.frmRunningProcessDialog(false);
            this.Visible = false;
            rd.addMessage("Downloading data. This may take a while....");
            rd.stepPGBar(10);
            rd.TopMost = true;
            DateTime dt = DateTime.Now;
            rd.Show();
            try
            {
                IFeatureClass ftrCls = mapserviceutility.createFeatureClassFromMapService(outPath,mpSvr,mpName,mpLayerDic[lyrStr]);
                IFeatureLayer lyr = new FeatureLayerClass();
                lyr.FeatureClass = ftrCls;
                lyr.Name = ((IDataset)ftrCls).Name;
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

        private void btnOutFtrCls_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterPolygonFeatureClassesClass();
            string outName;
            txtOutFtrCls.Text = frmHlp.getPathSave(flt, out outName);
        }

        

    }
}
