using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandSaveRaster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        
        private IApplication ap = ArcMap.Application;
        public commandSaveRaster()
        {
        }
        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmSaveRaster frm = new esriUtil.Forms.RasterAnalysis.frmSaveRaster(map);
            System.Windows.Forms.Application.Run(frm);
        }

        protected override void OnUpdate()
        {
        }
        
    }
}
