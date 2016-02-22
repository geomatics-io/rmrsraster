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
    public class commandCalcCloudMetrics : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCalcCloudMetrics()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Lidar.frmCalcCloudMetrics frm = new esriUtil.Forms.Lidar.frmCalcCloudMetrics(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
