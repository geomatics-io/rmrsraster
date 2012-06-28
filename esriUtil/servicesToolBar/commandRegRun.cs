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
    public class commandRegRun : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRegRun()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.SasProcedures.frmRunRegressionRaster frm = new esriUtil.Forms.SasProcedures.frmRunRegressionRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
