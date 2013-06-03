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
    public class commandEasting : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandEasting()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmFlipRaster frm = new esriUtil.Forms.RasterAnalysis.frmFlipRaster(map, esriUtil.rasterUtil.surfaceType.EASTING);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
