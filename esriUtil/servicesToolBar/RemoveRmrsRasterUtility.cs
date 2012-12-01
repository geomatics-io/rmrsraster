using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace servicesToolBar
{
    public class RemoveRmrsRasterUtility : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public RemoveRmrsRasterUtility()
        {
        }

        protected override void OnClick()
        {
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("Are you sure you want to uninstall RMRS Raster Utility?", "Uninstall", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                string id = ThisAddIn.AddInID;
                string folderLoc = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArcGIS\\AddIns\\Desktop10.0\\" + id;
                esriUtil.update upd = new esriUtil.update();
                upd.removeRMRSRasterUtility(folderLoc);
                System.Windows.Forms.MessageBox.Show("RMRS Raster Utility has be uninstalled. You will need to restart ArcMap for this change to take effect.");
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
