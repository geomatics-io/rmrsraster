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
            try
            {
                string mPath = esriUtil.Statistics.ModelHelper.openModelFileDialog();
                if (mPath != "")
                {
                    esriUtil.Statistics.ModelHelper br = new esriUtil.Statistics.ModelHelper(mPath);
                    br.openModelReport(mPath, 0.05);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
