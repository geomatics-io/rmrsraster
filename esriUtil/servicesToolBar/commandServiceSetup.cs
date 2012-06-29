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
            if (esriUtil.mapserviceutility.connectedToInternet)
            {
                ArcMap.Application.CurrentTool = null;
                esriUtil.Forms.MapServices.frmMapServices frmMapService = new esriUtil.Forms.MapServices.frmMapServices();
                frmMapService.TopLevel = true;
                frmMapService.Show();
            }
            else
            {
                MessageBox.Show("You are not connected to the internet. To use this tool you must be connected to the internet!", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        protected override void OnUpdate()
        {
            
        }
    }

}
