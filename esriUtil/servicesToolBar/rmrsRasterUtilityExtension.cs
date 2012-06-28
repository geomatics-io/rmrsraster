using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace servicesToolBar
{
    public class rmrsRasterUtilityExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        public rmrsRasterUtilityExtension()
        {
        }

        protected override void OnStartup()
        {
            esriUtil.rasterUtil.cleanupTempDirectories();
        }
        protected override void OnShutdown()
        {
            esriUtil.rasterUtil.cleanupTempDirectories();
        }
    }

}
