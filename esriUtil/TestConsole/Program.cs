using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using System.Windows.Forms;
using esriUtil;
using System.Threading;



namespace TestConsole
{
    class Program
    {

        private static LicenseInitializer m_AOLicenseInitializer = new TestConsole.LicenseInitializer();
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcInfo }, new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst });
            System.DateTime dt = System.DateTime.Now;
            Console.WriteLine();

            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string inRsStr = @"C:\Users\jshogland\Documents\John\temp\bpssusecn_z\bpssubscnmaj2";
            string vRsStr = @"C:\Users\jshogland\Documents\John\temp\modis_rc\ndviMAX_2000_rng.img";
            IRaster zRs = rsUtil.returnRaster(inRsStr);
            IRaster vRs = rsUtil.returnRaster(vRsStr);
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new esriUtil.Forms.RunningProcess.frmRunningProcessDialog(false);
            rp.Show();
            rasterUtil.zoneType[] zT = {rasterUtil.zoneType.MEAN,rasterUtil.zoneType.SUM,rasterUtil.zoneType.STD};
            ITable tbl = rsUtil.zonalStats(zRs, vRs, zT, rp);
            //esriUtil.Forms.RasterAnalysis.frmZonalStats frm = new esriUtil.Forms.RasterAnalysis.frmZonalStats(null);
            //System.Windows.Forms.Application.Run(frm);    
            System.DateTime dt2 = System.DateTime.Now;
            System.TimeSpan ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
