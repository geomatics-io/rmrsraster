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
    public class commandFIABiomassSummary : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandFIABiomassSummary()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.FIA.frmFiaBiomass frm = new esriUtil.Forms.FIA.frmFiaBiomass(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
