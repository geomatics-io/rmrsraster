using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandSedByArival : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSedByArival()
        {
        }

        protected override void OnClick()
        {
            esriUtil.rasterUtil rsUtil = new esriUtil.rasterUtil();
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime frm = new esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime(map, rsUtil);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}