using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace servicesToolBar
{
    public class commandCompareSampleSize : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandCompareSampleSize()
        {
        }

        protected override void OnClick()
        {
            esriUtil.Forms.Stats.frmCompareSampleToPopulation frm = new esriUtil.Forms.Stats.frmCompareSampleToPopulation();
            frm.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
