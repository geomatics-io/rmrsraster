using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;

namespace servicesToolBar
{
    public class rmrsRasterUtilityExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        public rmrsRasterUtilityExtension()
        {

        }

        protected override void OnStartup()
        {
            //esriUtil.rasterUtil.cleanupTempDirectories();
        }
        protected override void OnShutdown()
        {
            esriUtil.update up = new esriUtil.update(ThisAddIn.Version);
            up.updateApp();
            esriUtil.rasterUtil.cleanupTempDirectories();
        }
        
    }

}
