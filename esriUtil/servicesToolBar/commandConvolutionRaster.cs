using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandConvolutionRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandConvolutionRaster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmConvolutionRaster frm = new esriUtil.Forms.RasterAnalysis.frmConvolutionRaster(map);
            frm.Show();
        }
        protected override void OnUpdate()
        {
        }
    }
}
