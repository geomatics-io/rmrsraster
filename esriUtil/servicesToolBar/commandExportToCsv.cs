using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;


namespace servicesToolBar
{
    public class commandExportToCsv : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandExportToCsv()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mpDoc = ArcMap.Document;
            IMap map = mpDoc.FocusMap;
            esriUtil.Forms.Sampling.frmExportSampleToCsv frm = new esriUtil.Forms.Sampling.frmExportSampleToCsv(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
