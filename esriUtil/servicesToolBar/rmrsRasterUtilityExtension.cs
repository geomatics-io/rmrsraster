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
                string id = ThisAddIn.AddInID;
                string arcMapVs = "10.2";//ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Version;
                string folderLoc = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArcGIS\\AddIns\\Desktop"+arcMapVs+"\\" + id;
                up.updateApp(folderLoc,arcMapVs);
                up.removeOldVersion(id, arcMapVs);//arcMapVs);
                up.updateHelp();
            }
            catch
            {
            }
            
        }
        
    }

}
