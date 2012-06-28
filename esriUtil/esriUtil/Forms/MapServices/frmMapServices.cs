using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Forms.MapServices
{
    public partial class frmMapServices : Form
    {
        public frmMapServices()
        {
            InitializeComponent();
            string prjDb = msUtil.LcCacheDb;
            if (prjDb == "")
            {
                msUtil.changeLocalDatabase();
            }
            loadConnection();
        }

        private void loadConnection()
        {
            cmbCon.Items.Clear();
            cmbCon.Items.AddRange(msUtil.getConnectionStrings().ToArray());
        }
        private void loadService()
        {
            cmbSrv.Items.Clear();
            List<string> svLst = msUtil.getServiceStrings(cmbCon.Text);
            if (svLst.Count < 1)
            {
                this.UseWaitCursor = true;
                msUtil.updateServiceTable(cmbCon.Text);
                svLst = msUtil.getServiceStrings(cmbCon.Text);
                this.UseWaitCursor = false;
            }
            cmbSrv.Items.AddRange(svLst.ToArray());
        }
        private void loadLayers()
        {
            chbLayers.Items.Clear();
            Dictionary<string,bool> lyrDic = msUtil.getLayerDic(cmbCon.Text,cmbSrv.Text);
            if (lyrDic.Count < 1)
            {
                this.UseWaitCursor = true;
                IAGSServerConnection2 sConn = msUtil.getServiceConnection(cmbCon.Text);
                Dictionary<string, int> lyrIdDic = new Dictionary<string, int>(); 
                string sType = msUtil.getServiceType(cmbCon.Text, cmbSrv.Text);
                IAGSServerObjectName3 sObj = msUtil.getServiceObject(sConn, cmbSrv.Text, sType);
                if (sType.ToLower() == "mapserver")
                {
                    IMapServer2 ms2 = msUtil.getMapService(sObj);
                    lyrIdDic = msUtil.getLayers(ms2);
                }
                else if (sType.ToLower() == "imageserver")
                {
                    IImageServer ims = msUtil.getIMageService(sObj);
                    lyrIdDic = msUtil.getImages(ims);
                }              
                msUtil.updateLayerTable(lyrIdDic,cmbCon.Text, cmbSrv.Text);
                lyrDic = msUtil.getLayerDic(cmbCon.Text, cmbSrv.Text);
                this.UseWaitCursor = false;
            }
            if (lyrDic.Count < 1)
            {
                MessageBox.Show("Can't find any layers that will allow users to download information");

            }
            else
            {
                foreach (KeyValuePair<string, bool> kvp in lyrDic)
                {
                    chbLayers.Items.Add(kvp.Key, kvp.Value);
                }
            }
        }
        private mapserviceutility msUtil = new mapserviceutility();

        private void cmbSrv_SelectedValueChanged(object sender, EventArgs e)
        {
            string svl = cmbSrv.Text;
            string cvl = cmbCon.Text;
            chbLayers.Items.Clear();
            chbLayers.Enabled = false;
            if (!(svl == "" || svl == null || cvl == "" || cvl == null))
            {
                
                
                loadLayers();
                
                chbLayers.Enabled = true;
            }
        }

        

        private void cmbCon_SelectedValueChanged(object sender, EventArgs e)
        {
            string cvl = cmbCon.Text;
            cmbSrv.Text = "";
            cmbSrv.Items.Clear();
            cmbSrv.Enabled = false;
            chbLayers.Items.Clear();
            chbLayers.Enabled = false;
            if (!(cvl == "" || cvl == null))
            {
                loadService();
                cmbSrv.Enabled = true;
            }

        }

        private void chbLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            
            CheckState chS = e.NewValue;
            string uV = "NO";
            if (chS == CheckState.Checked)
            {
                uV = "YES";
            }
            string sOid = msUtil.getServiceOID(cmbCon.Text,cmbSrv.Text);
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = "FKID = " + sOid + " and LAYERS = '" + chbLayers.Items[e.Index].ToString() + "'";
            msUtil.updateLayerRow(qf,uV);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmInputBox input = new frmInputBox();
            DialogResult dRslt = input.ShowDialog();
            if (dRslt != DialogResult.OK)
            {
                return;
            }
            string con = input.ArcGISConnection;
            input.Dispose();
            if (msUtil.getServiceConnection(con) == null)
            {
                MessageBox.Show("Can't connect to ArcGIS server " + con);
                return;
            }
            msUtil.updateConnectionTable(con);
            msUtil.updateServiceTable(con);
            loadConnection();
            cmbCon.SelectedText = con;
            

            


            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connStr = cmbCon.Text;
            if (connStr != "" && connStr != null)
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "CONNECTION = '" + cmbCon.Text + "'";
                msUtil.removeExistingRecords(msUtil.ConnectionsTable, qf);
                cmbCon.Items.Remove(connStr);
                cmbCon.Text = "";
                cmbSrv.Items.Clear();
                cmbSrv.Text = "";
                chbLayers.Items.Clear();
            }
        }

        private void btnChangeOutput_Click(object sender, EventArgs e)
        {
            msUtil.changeLocalDatabase();
        }
        
    }
}
