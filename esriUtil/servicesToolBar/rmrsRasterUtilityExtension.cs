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
        }
        protected override void OnShutdown()
        {
            try
            {
                esriUtil.rasterUtil.cleanupTempDirectories();
                esriUtil.update up = new esriUtil.update(ThisAddIn.Version);
                up.updateApp();
                up.updateHelp();
            }
            catch
            {
            }
            
        }
        
    }

}
