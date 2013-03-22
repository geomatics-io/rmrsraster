using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;

namespace servicesToolBar
{
    public class commandMultivariateRegression : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandMultivariateRegression()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            esriUtil.Forms.Stats.frmMultivariateLinearRegression frm = new esriUtil.Forms.Stats.frmMultivariateLinearRegression(map);
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
