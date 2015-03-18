using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandResampleRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandResampleRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmResampleRaster frm = new esriUtil.Forms.RasterAnalysis.frmResampleRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
