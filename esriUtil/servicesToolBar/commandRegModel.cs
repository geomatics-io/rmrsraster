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
    public class commandRegModel : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRegModel()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.SasProcedures.frmBuildRegressionRaster frm = new esriUtil.Forms.SasProcedures.frmBuildRegressionRaster(map);
            frm.Show();
        }
        protected override void OnUpdate()
        {
        }
    }
}
