using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class commandDeleteModel : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandDeleteModel()
        {
        }

        protected override void OnClick()
        {
            esriUtil.Statistics.ModelHelper.deleteModelFile();
        }

        protected override void OnUpdate()
        {
        }
    }
}
