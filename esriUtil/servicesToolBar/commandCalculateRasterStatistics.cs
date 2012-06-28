using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandCalculateRasterStatistics : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCalculateRasterStatistics()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmCalculateStatistics frm = new esriUtil.Forms.RasterAnalysis.frmCalculateStatistics(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
