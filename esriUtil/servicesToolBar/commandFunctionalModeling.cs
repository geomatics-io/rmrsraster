using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop;
using esriUtil;


namespace servicesToolBar
{
    public class commandFunctionalModeling : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandFunctionalModeling()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmFunctionalModeling frm = new esriUtil.Forms.RasterAnalysis.frmFunctionalModeling(map);
            frm.ShowDialog();
            frm.Dispose();
        }

        protected override void OnUpdate()
        {
            
        }
        

    }
}
