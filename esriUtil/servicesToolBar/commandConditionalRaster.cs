using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandConditionalRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandConditionalRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmConditionalRaster frm = new esriUtil.Forms.RasterAnalysis.frmConditionalRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
