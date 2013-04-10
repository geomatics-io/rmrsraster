using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandTTest : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandTTest()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmTTest frm = new esriUtil.Forms.Stats.frmTTest(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
