using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.Forms.MapServices
{
    public partial class frmAddProjectLayers : Form
    {
        public frmAddProjectLayers(IMap map2, esriDatasetType dType)
        {
            InitializeComponent();
            if (map2 == null)
            {
                OpenFileDialog ofdMxd = new OpenFileDialog();
                ofdMxd.Filter = "Map Doc|*.mxd";
                ofdMxd.Multiselect = false;
                DialogResult rslt = ofdMxd.ShowDialog();
                if (rslt == DialogResult.OK)
                {
                    mapDoc = new MapDocumentClass();
                    mapDoc.Open(ofdMxd.FileName, "");
                    map = mapDoc.get_Map(0);
                }
                else
                {
                    MessageBox.Show("A map document must be selected to work. Shutting down");
                    this.Close();
                }
            }
            else
            {
                map = map2;
            }
            if (dType == esriDatasetType.esriDTAny)
            {
                dType = esriDatasetType.esriDTFeatureClass;
            }
            switch (dType)
            {
                case esriDatasetType.esriDTTable:
                    this.Text = "Add Table";
                    label1.Text = "Select Table";
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    this.Text = "Add Raster";
                    label1.Text = "Select Raster";
                    break;
                default:
                    this.Text = "Add Feature";
                    label1.Text = "Select Feature";
                    break;
            }
            mapserviceutility msUtil = new mapserviceutility();

            prjDatabase = msUtil.LcCacheDb;
            if (prjDatabase == "")
            {
                msUtil.changeLocalDatabase();
                prjDatabase = msUtil.LcCacheDb;
            }
            this.cmbLayers.Items.Clear();
            foreach (string s in getNames(dType))
            {
                lyrDic.Add(s, prjDatabase + "\\" + s);
                cmbLayers.Items.Add(s);
            }
            this.Refresh();
        }
        Dictionary<string, string> lyrDic = new Dictionary<string, string>();
        private string prjDatabase = null;
        private IMapDocument mapDoc = null;
        private IMap map = null;
        private List<string> getNames(esriDatasetType dType)
        {
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            List<string> lyrLst = new List<string>();
            switch (dType)
            {
                case esriDatasetType.esriDTFeatureClass:
                    lyrLst = geoUtil.getAllFeatureNames(prjDatabase);
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    lyrLst = geoUtil.getAllRasterNames(prjDatabase);
                    break;
                case esriDatasetType.esriDTTable:
                    lyrLst = geoUtil.getAllTableNames(prjDatabase);
                    break;
                default:
                    break;
            }
            return lyrLst;
        }

        private void frmAddProjectLayers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mapDoc != null)
            {
                mapDoc.Close();
            }
        }

        private void cmbLayers_SelectedValueChanged(object sender, EventArgs e)
        {
            lstLayers.Items.Add(cmbLayers.Text);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            lstLayers.Items.Clear();
            foreach (string s in lyrDic.Keys)
            {
                lstLayers.Items.Add(s);
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            lstLayers.Items.Remove(lstLayers.SelectedItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            viewUtility vUtil = new viewUtility((IActiveView)map);
            foreach (string s in lstLayers.Items)
            {
                string lyrPath = String.Empty;
                lyrDic.TryGetValue(s, out lyrPath);
                if (lyrPath != String.Empty)
                {
                    vUtil.addLayer(lyrPath);
                }
            }
            this.Close();
        }

    }
}
