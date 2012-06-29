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

namespace esriUtil.Forms.MapServices
{
    public partial class frmTileImageServiceLayer : Form
    {
        public frmTileImageServiceLayer(IActiveView acV)
        {
            InitializeComponent();
            if (acV == null)
            {
                OpenFileDialog oFd = new OpenFileDialog();
                oFd.Filter = "Map Document|*.mxd";
                oFd.Multiselect = false;
                DialogResult dR = oFd.ShowDialog();
                if (dR == System.Windows.Forms.DialogResult.OK)
                {
                    mapDoc = new MapDocumentClass();
                    mapDoc.Open(oFd.FileName);
                    acV = mapDoc.ActiveView;

                }
            }
            av = acV;
        }
        private IMapDocument mapDoc = null;

        private IActiveView av = null;
        private IEnumLayer getFeatureLayers()
        {
            string lyrGuid = "{" + typeof(IFeatureLayer).GUID.ToString() + "}";
            IMap map = (IMap)av;
            UID uid = new UIDClass();
            uid.Value = lyrGuid;
            return map.get_Layers(uid, true);

        }
        private IEnumLayer getImageServiceLayers()
        {
            string lyrGuid = "{"+ typeof(IImageServerLayer).GUID.ToString()+"}";
            IMap map = (IMap)av;
            UID uid = new UIDClass();
            uid.Value = lyrGuid;
            return map.get_Layers(uid, true);

        }
        private Dictionary<string, IImageServerLayer> lyrDic = new Dictionary<string, IImageServerLayer>();
        private void FillftrLyrDic()
        {
            ftrLyrDic.Clear();
            IEnumLayer eLyr = getFeatureLayers();
            ILayer lyr = eLyr.Next();
            while (lyr != null)
            {
                ftrLyrDic.Add(lyr.Name, (IFeatureLayer)lyr);
                lyr = eLyr.Next();
            }
        }
        private Dictionary<string, IFeatureLayer> ftrLyrDic = new Dictionary<string, IFeatureLayer>();
        private void FillLyrDic()
        {
            lyrDic.Clear();
            IEnumLayer eLyr = getImageServiceLayers();
            ILayer lyr = eLyr.Next();
            while (lyr != null)
            {
                lyrDic.Add(lyr.Name, (IImageServerLayer)lyr);
                lyr = eLyr.Next();
            }
        }

        private void frmTileImageServiceLayer_Load(object sender, EventArgs e)
        {
            FillLyrDic();
            FillftrLyrDic();
            cmbImageServiceLayer.Items.Clear();
            cmbImageServiceLayer.Items.AddRange(lyrDic.Keys.ToArray());
            cmbExtent.Items.Clear();
            cmbExtent.Items.Add("Display");
            cmbExtent.Items.AddRange(ftrLyrDic.Keys.ToArray());
            cmbExtent.SelectedItem = cmbExtent.Items[0];


        }

        private void frmTileImageServiceLayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mapDoc != null)
            {
                mapDoc.Close();
            }
        }

        private bool checkInputs()
        {
            bool x = true;
            try
            {
                string geoDb = txtGeoDb.Text;
                string ext = cmbExtent.Text;
                string svr = cmbImageServiceLayer.Text;
                if (geoDb == "" || ext == "" || svr == "")
                {
                    x = false;
                }
            }
            catch (Exception e)
            {
                x = false;
                Console.WriteLine(e.ToString());
            }
            return x;
        }
        private void btnGeoDb_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a File geodatabase to store tiles";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                txtGeoDb.Text = gxObj.FullName;
            }
        }
        private IFeatureLayer getFeatureLayer()
        {
            IFeatureLayer svLyr = null;
            ftrLyrDic.TryGetValue(cmbExtent.Text , out svLyr);
            return svLyr;
        }
        private IImageServerLayer getServerLayer()
        {
            IImageServerLayer svLyr = null;
            lyrDic.TryGetValue(cmbImageServiceLayer.Text, out svLyr);
            return svLyr;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (!checkInputs())
            {
                MessageBox.Show("You must have a value for all inputs");
                return;
            }
            mapserviceutility msUtil = new mapserviceutility();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            IImageServerLayer svLyr = getServerLayer();
            ESRI.ArcGIS.Geometry.IEnvelope ext = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            if(cmbExtent.Text.ToLower()=="display")
            {
                ext = av.Extent;
            }
            else
            {
                IFeatureLayer ftrLyr = getFeatureLayer();
                ext = ((ESRI.ArcGIS.Geodatabase.IGeoDataset)ftrLyr).Extent;
            }
            if(svLyr==null)
            {
                MessageBox.Show("You must select a Image server layer");
                return;
            }
            this.Visible = false;
            ESRI.ArcGIS.Geodatabase.IWorkspace wks = geoUtil.OpenWorkSpace(txtGeoDb.Text);
            ESRI.ArcGIS.Geodatabase.IRaster rs = null;
            string msg = msUtil.fillDbRaster(svLyr,wks,ext,svLyr.ServiceInfo.SpatialReference,out rs);
            IMap mp = (IMap)av;
            if (rs != null)
            {
                IRasterLayer rsLyr = new RasterLayerClass();
                rsLyr.CreateFromRaster(rs);
                rsLyr.Name = svLyr.ServiceInfo.Name;
                rsLyr.Visible = false;
                mp.AddLayer((ILayer)rsLyr);
            }
            this.Close();
            //MessageBox.Show(msg);
        }

    }
}
