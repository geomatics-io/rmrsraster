using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandAccuracyAssessment : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAccuracyAssessment()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmAccuracyAssessment frm = new esriUtil.Forms.Stats.frmAccuracyAssessment(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
