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
using ESRI.ArcGIS.DataSourcesNetCDF;
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
            System.DateTime dt2;
            TimeSpan ts;
            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string ftrClsPath = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\SteveBrown\ImageTest.gdb\m7001_RSA";
            string zoneValuePath = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\SteveBrown\ImageTest.gdb\m7001";
            IRaster vRs = rsUtil.returnRaster(zoneValuePath);
            IFeatureClass ftrCls = geoUtil.getFeatureClass(ftrClsPath);
            Console.WriteLine((ftrCls == null).ToString());
            esriUtil.FunctionRasters.zonalHelper zH = new esriUtil.FunctionRasters.zonalHelper();
            zH.InValueRaster = vRs;
            zH.InZoneFeatureClass = ftrCls;
            zH.InZoneField = "GRIDCODE";
            zH.reprojectInFeatureClass(ftrCls, ((IRasterProps)vRs).SpatialReference);
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
