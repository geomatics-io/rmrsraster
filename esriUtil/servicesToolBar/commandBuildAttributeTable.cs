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
    public class commandBuildAttributeTable : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandBuildAttributeTable()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmCreateAttributeTable frm = new esriUtil.Forms.RasterAnalysis.frmCreateAttributeTable(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
