using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace servicesToolBar
{
    public class commandFiaPolygonSummary : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandFiaPolygonSummary()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.FIA.frmFiaSummarizePoly frm = new esriUtil.Forms.FIA.frmFiaSummarizePoly(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
