using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace servicesToolBar
{
    public class commandAggregateRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAggregateRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmAggregationRaster frm = new esriUtil.Forms.RasterAnalysis.frmAggregationRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
