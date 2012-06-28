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
    public class commandSampleRasterCluster : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSampleRasterCluster()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Sampling.frmClusterSampleRaster frm = new esriUtil.Forms.Sampling.frmClusterSampleRaster(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
