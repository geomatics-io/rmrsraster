using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandPCA : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandPCA()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmPcaAnalysis frm = new esriUtil.Forms.Stats.frmPcaAnalysis(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
