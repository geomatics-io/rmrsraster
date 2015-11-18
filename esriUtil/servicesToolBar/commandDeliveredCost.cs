using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandDeliveredCost : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandDeliveredCost()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmTransportationCost frm = new esriUtil.Forms.RasterAnalysis.frmTransportationCost(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
