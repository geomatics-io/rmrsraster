using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using esriUtil;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace servicesToolBar
{
    public class commandAddFunctionModelToMap : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandAddFunctionModelToMap()
        {
        }

        protected override void OnClick()
        {
            functionModel funcMd = new functionModel();
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            funcMd.addFunctionRasterToMap(map);
        }

        protected override void OnUpdate()
        {
        }
    }
}
