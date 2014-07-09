using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class commandSeperateProcess : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandSeperateProcess()
        {
        }

        protected override void OnClick()
        {
            string fLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string exe = "\\RMRSBatchProcess.exe";
            System.Diagnostics.Process.Start(fLoc+exe);
        }

        protected override void OnUpdate()
        {
        }
    }
}
