using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace servicesToolBar
{
    public class commandVarCov : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandVarCov()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.Stats.frmVarCovCorr frm = new esriUtil.Forms.Stats.frmVarCovCorr(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
