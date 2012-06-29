using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace servicesToolBar
{
    public class commandWebSite : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandWebSite()
        {
           
        }

        protected override void OnClick()
        {
            if(esriUtil.mapserviceutility.connectedToInternet)
            {
                try
                {
                    System.Diagnostics.Process.Start("IExplore.exe", "http://www.fs.fed.us/rm/raster-utility/");
                }
                catch(Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Can't open website. Please check your internet connection");
                    Console.WriteLine(e.ToString());
                }
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
