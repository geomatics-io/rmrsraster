using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using esriUtil;
using System.Windows.Forms;



namespace servicesToolBar
{
    public class commandServiceSetup : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandServiceSetup()
        {
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            ArcMap.Application.CurrentTool = null;
            esriUtil.Forms.MapServices.frmMapServices frmMapService = new esriUtil.Forms.MapServices.frmMapServices();
            frmMapService.TopLevel = true;
            frmMapService.Show();

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
