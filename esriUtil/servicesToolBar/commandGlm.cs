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
    public class commandGlm : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandGlm()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmGlm frm = new esriUtil.Forms.Stats.frmGlm(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
