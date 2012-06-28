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
    public class commandRemap : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRemap()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmRemapRaster frm = new esriUtil.Forms.RasterAnalysis.frmRemapRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
