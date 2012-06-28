using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class commandHelp : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandHelp()
        {
        }

        protected override void OnClick()
        {
            esriUtil.help hp = new esriUtil.help();
            hp.start();
        }

        protected override void OnUpdate()
        {
        }
    }
}
