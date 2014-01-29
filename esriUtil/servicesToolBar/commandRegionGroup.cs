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
    public class commandRegionGroup : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRegionGroup()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmRegionGroup frm = new esriUtil.Forms.RasterAnalysis.frmRegionGroup(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
