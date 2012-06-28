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
    public class commandCreateMosaic : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCreateMosaic()
        {
            
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmCreateMosaic frm = new esriUtil.Forms.RasterAnalysis.frmCreateMosaic(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
