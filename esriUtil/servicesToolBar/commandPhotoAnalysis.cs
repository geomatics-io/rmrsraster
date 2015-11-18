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
    public class commandPhotoAnalysis : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandPhotoAnalysis()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.RasterAnalysis.frmPhotoAnalysis frm = new esriUtil.Forms.RasterAnalysis.frmPhotoAnalysis(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
