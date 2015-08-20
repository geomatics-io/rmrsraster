using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandSummarizeRelated : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSummarizeRelated()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmSummarizeRelatedTable frm = new esriUtil.Forms.Stats.frmSummarizeRelatedTable(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
