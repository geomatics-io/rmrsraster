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
using System.Windows.Forms.DataVisualization;



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
            System.DateTime dt2;
            TimeSpan ts;
            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string zoneFtr = @"C:\Documents and Settings\jshogland\My Documents\JOHN\presentation\Authoring\fy2013\MAGIP\MAGIP_Presentation.gdb\ForestClip";
            string valueRst = @"C:\Documents and Settings\jshogland\My Documents\JOHN\presentation\Authoring\fy2013\MAGIP\MAGIP_Presentation.gdb\LandSat";
            IFeatureClass ftrCls = geoUtil.getFeatureClass(zoneFtr);
            rasterUtil.zoneType[] zt = {rasterUtil.zoneType.SUM};
            rsUtil.zonalStats(ftrCls, "OBJECTID", rsUtil.returnRaster(valueRst), "testPr1", zt, null);
            //esriUtil.Forms.Texture.frmCreateGlcmSurface frm = new esriUtil.Forms.Texture.frmCreateGlcmSurface(null, ref rsUtil, false);// esriUtil.Forms.SasProcedures.frmRunPolytomousLogisticRegression(null);// esriUtil.Forms.RasterAnalysis.frmBatchProcess();
            //System.Windows.Forms.Application.Run(frm); 
            
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
