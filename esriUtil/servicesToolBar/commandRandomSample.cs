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
    public class commandRandomSample : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandRandomSample()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Sampling.frmCreateRandomSample frm = new esriUtil.Forms.Sampling.frmCreateRandomSample(map, false);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
