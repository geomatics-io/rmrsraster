using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandComparClassifications : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandComparClassifications()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmCompareClassifications frm = new esriUtil.Forms.Stats.frmCompareClassifications(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
