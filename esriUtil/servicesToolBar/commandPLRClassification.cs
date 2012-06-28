using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using esriUtil;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;


namespace servicesToolBar
{
    public class commandPlrClassification : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandPlrClassification()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.SasProcedures.frmRunPolytomousLogisticRegression frmPly = new esriUtil.Forms.SasProcedures.frmRunPolytomousLogisticRegression(map);
            frmPly.Show();

        }

        protected override void OnUpdate()
        {
        }
    }
}
