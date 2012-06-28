using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandRescaleRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRescaleRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmRescaleRaster frm = new esriUtil.Forms.RasterAnalysis.frmRescaleRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
