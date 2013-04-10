using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class commandViewSampleSizeFromModel : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandViewSampleSizeFromModel()
        {
        }

        protected override void OnClick()
        {
            esriUtil.Forms.Stats.frmViewModelSampleSize frm = new esriUtil.Forms.Stats.frmViewModelSampleSize();
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
