using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandAddData : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAddData()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.MapServices.frmAddProjectLayers frmAdd = new esriUtil.Forms.MapServices.frmAddProjectLayers(map, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass);
            frmAdd.TopLevel = true;
            frmAdd.Show();
            
        }

        protected override void OnUpdate()
        {
        }
    }
}
