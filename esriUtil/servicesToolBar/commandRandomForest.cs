using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using esriUtil;

namespace servicesToolBar
{
    public class commandRandomForest : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRandomForest()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmRandomForest frm = new esriUtil.Forms.Stats.frmRandomForest(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
