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
    public class commandExtractRasterBands : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandExtractRasterBands()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmExtractBand frm = new esriUtil.Forms.RasterAnalysis.frmExtractBand(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
