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
    public class commandSetNullValue : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSetNullValue()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmSetNullValue frm = new esriUtil.Forms.RasterAnalysis.frmSetNullValue(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
