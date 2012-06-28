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
    public class commandPLRModel : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandPLRModel()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.SasProcedures.frmBuildPolytomousLogisticRegressionModel frmPlr = new esriUtil.Forms.SasProcedures.frmBuildPolytomousLogisticRegressionModel(map);
            frmPlr.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
