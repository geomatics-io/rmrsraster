using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace servicesToolBar
{
    public class commandLandfireSegmentation : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandLandfireSegmentation()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.OptFuels.frmLandfireSegmentation frm = new esriUtil.Forms.OptFuels.frmLandfireSegmentation();
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
