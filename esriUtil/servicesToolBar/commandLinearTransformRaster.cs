using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandLinearTransformRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLinearTransformRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmLinearTransform frm = new esriUtil.Forms.RasterAnalysis.frmLinearTransform(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
