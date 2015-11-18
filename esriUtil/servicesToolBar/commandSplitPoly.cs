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
    public class commandSplitPoly : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSplitPoly()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmSplitPoly frm = new esriUtil.Forms.RasterAnalysis.frmSplitPoly(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
