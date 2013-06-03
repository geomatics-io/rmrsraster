using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandCombineRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCombineRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmCompositeRaster frm = new esriUtil.Forms.RasterAnalysis.frmCompositeRaster(map,false);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
