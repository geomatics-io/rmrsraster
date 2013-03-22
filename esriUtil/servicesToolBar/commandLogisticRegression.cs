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
    public class commandLogisticRegression : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLogisticRegression()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmLogisticRegression frm = new esriUtil.Forms.Stats.frmLogisticRegression(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
