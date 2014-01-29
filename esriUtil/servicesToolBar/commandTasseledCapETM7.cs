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
    public class commandTasseledCapETM7 : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandTasseledCapETM7()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmTasseledCap frm = new esriUtil.Forms.RasterAnalysis.frmTasseledCap(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
