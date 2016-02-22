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
    public class commandRemoveSpaces : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRemoveSpaces()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Lidar.frmRemoveSpaces frm = new esriUtil.Forms.Lidar.frmRemoveSpaces(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
