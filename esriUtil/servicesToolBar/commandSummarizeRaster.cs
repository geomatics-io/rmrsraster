using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandSummarizeRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSummarizeRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmSummarizeRaster frm = new esriUtil.Forms.RasterAnalysis.frmSummarizeRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
