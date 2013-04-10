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
    public class commandSelectSamplesFromFeature : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSelectSamplesFromFeature()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Sampling.frmSelectSamplesFromTable frm = new esriUtil.Forms.Sampling.frmSelectSamplesFromTable(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
