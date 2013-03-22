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
    public class commandSoftMaxNnet : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSoftMaxNnet()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmSoftMaxNnet frm = new esriUtil.Forms.Stats.frmSoftMaxNnet(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
