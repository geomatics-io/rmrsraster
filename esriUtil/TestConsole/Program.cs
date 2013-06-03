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
using Accord.Statistics.Testing;



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
            double a = 5.5;
            //Console.WriteLine(System.Convert.ToInt32(a));
            //Console.WriteLine((int)a);
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();
            featureUtil ftUtil = new featureUtil();
            string org1 = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\ANAL_DB\CLWNEZ_DATA.gdb\Original_CCV_TRAIN";
            string mdl = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\ANAL_DB\testKS2.mdl";
            string st = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\ANAL_DB\CLWNEZ_DATA.gdb\RANDOM_CLWNEZ_CCV_TRAIN_4000";
            ITable sTbl = geoUtil.getTable(org1);
            ITable s2Tbl = geoUtil.getTable(st);
            ftUtil.selectKSFeaturesToSample(sTbl, s2Tbl, mdl, "CCV_SAMP_mlc");
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
