using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandLocalRescaleRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLocalRescaleRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmLocalRescaleAnalysis frm = new esriUtil.Forms.RasterAnalysis.frmLocalRescaleAnalysis(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
