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
    public class commandNdviRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandNdviRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmNDVIRaster frm = new esriUtil.Forms.RasterAnalysis.frmNDVIRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
