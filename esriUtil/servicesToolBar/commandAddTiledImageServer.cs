using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;


namespace servicesToolBar
{
    public class commandAddTiledImageServer : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAddTiledImageServer()
        {

        }

        protected override void OnClick()
        {
            if (esriUtil.mapserviceutility.connectedToInternet)
            {
                IMxDocument doc = ArcMap.Document;
                IMap map = doc.FocusMap;
                esriUtil.Forms.MapServices.frmTileImageServiceLayer frm = new esriUtil.Forms.MapServices.frmTileImageServiceLayer((IActiveView)map);
                frm.TopLevel = true;
                frm.Show();
            }
            else
            {
                MessageBox.Show("You are not connected to the internet. To use this tool you must be connected to the internet!", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            
        }

        protected override void OnUpdate()
        {
        }
    }
}
