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
    public class commandBatchProcess : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandBatchProcess()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmBatchProcess frm = new esriUtil.Forms.RasterAnalysis.frmBatchProcess(map, new esriUtil.rasterUtil());
            frm.Show();
                
        }

        protected override void OnUpdate()
        {
        }
    }
}
