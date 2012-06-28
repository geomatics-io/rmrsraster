using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandAddTiledImageServer : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAddTiledImageServer()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.MapServices.frmTileImageServiceLayer frm = new esriUtil.Forms.MapServices.frmTileImageServiceLayer((IActiveView)map);
            frm.TopLevel = true;
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
