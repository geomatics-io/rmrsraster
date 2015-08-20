using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;



namespace servicesToolBar
{
    public class commandSummarizeByField : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSummarizeByField()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmSummarizeByField frm = new esriUtil.Forms.Stats.frmSummarizeByField(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
