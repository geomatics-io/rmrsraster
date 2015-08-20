using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandSpatiallyWeightedAverage : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSpatiallyWeightedAverage()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmWeightedAverageByAreaOrLength frm = new esriUtil.Forms.Stats.frmWeightedAverageByAreaOrLength(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
