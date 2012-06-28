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
    public class commandSampleGLCM : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSampleGLCM()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Sampling.frmSampleGlcm frm = new esriUtil.Forms.Sampling.frmSampleGlcm(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
