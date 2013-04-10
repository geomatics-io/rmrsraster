using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace servicesToolBar
{
    public class commandAdjustedAccuracyAssessment : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAdjustedAccuracyAssessment()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmAdjustAccuracyAssessment frm = new esriUtil.Forms.Stats.frmAdjustAccuracyAssessment(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
