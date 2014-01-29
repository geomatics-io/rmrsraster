using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace servicesToolBar
{
    public class commandNormalize : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandNormalize()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmNormalize frm = new esriUtil.Forms.RasterAnalysis.frmNormalize(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
