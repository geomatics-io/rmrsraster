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
    public class commandMathRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandMathRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmMathRaster frm = new esriUtil.Forms.RasterAnalysis.frmMathRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
