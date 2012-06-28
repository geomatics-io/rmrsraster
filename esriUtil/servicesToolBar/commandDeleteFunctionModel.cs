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
    public class commandDeleteFunctionModel : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandDeleteFunctionModel()
        {
        }

        protected override void OnClick()
        {
            functionModel funcMd = new functionModel();
            IMxDocument mxDoc = ArcMap.Document;
            IMap map = mxDoc.FocusMap;
            funcMd.deleteFunctionModel();
        }

        protected override void OnUpdate()
        {
        }
    }
}
