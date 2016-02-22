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
    public class commandConvertGridMetricsToRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandConvertGridMetricsToRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Lidar.frmConvertGridMetricsToRaster frm = new esriUtil.Forms.Lidar.frmConvertGridMetricsToRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
