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
    public class commandCreateDtmFiles : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCreateDtmFiles()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Lidar.frmCreateDtmFiles frm = new esriUtil.Forms.Lidar.frmCreateDtmFiles(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
