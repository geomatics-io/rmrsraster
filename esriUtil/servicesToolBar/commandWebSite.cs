using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class commandWebSite : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandWebSite()
        {
        }

        protected override void OnClick()
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

        protected override void OnUpdate()
        {
        }
    }
}
