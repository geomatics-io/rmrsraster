using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandAddRasterData : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAddRasterData()
        {
        }

        protected override void OnClick()
        {
            IMxDocument doc = ArcMap.Document;
            IMap map = doc.FocusMap;
            esriUtil.Forms.MapServices.frmAddProjectLayers frmAdd = new esriUtil.Forms.MapServices.frmAddProjectLayers(map, ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRasterDataset);
            frmAdd.TopLevel = true;
            frmAdd.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
