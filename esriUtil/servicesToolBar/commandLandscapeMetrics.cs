using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandLandscapeMetrics : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLandscapeMetrics()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Texture.frmLandscapeMetrics frm = new esriUtil.Forms.Texture.frmLandscapeMetrics(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
