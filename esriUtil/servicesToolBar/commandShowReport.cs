using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using esriUtil;

namespace servicesToolBar
{
    public class commandShowReport : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public commandShowReport()
        {
        }

        protected override void OnClick()
        {
            string mPath = esriUtil.Statistics.ModelHelper.openModelFileDialog();
            if(mPath!="")
            {
                esriUtil.Statistics.ModelHelper br = new esriUtil.Statistics.ModelHelper(mPath);
                br.openModelReport(mPath, 0.05);    
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
