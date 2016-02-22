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
using System.Net;
using System.Collections;
using ESRI.ArcGIS.GeoDatabaseExtensions;



namespace TestConsole
{
    class Program
    {

        private static LicenseInitializer m_AOLicenseInitializer = new TestConsole.LicenseInitializer();
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced}, new esriLicenseExtensionCode[] {esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst});//{esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst});
            System.DateTime dt = System.DateTime.Now;
            
            System.DateTime dt2;
            TimeSpan ts;
            featureUtil ftrUtil = new featureUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();
            string sampStr = @"C:\Users\jshogland\Documents\JOHN\projects\RimLidar\OriginalData.gdb\TestPlots";
            IFeatureClass ftrCls = geoUtil.getFeatureClass(sampStr);
            string lasDir = @"D:\LidarProject\LAS";
            string olasDir = @"C:\Users\jshogland\Documents\JOHN\projects\RimLidar\DG_LAS";
            string metricsDir = @"D:\LidarProject\metrics";
            string metricsAboveDir = @"D:\LidarProject\metricsAbove";
            string outDir = @"D:\LidarProject\rasterTest";
            string groundLas = @"D:\LidarProject\GroundLAS";
            string dtm = @"D:\LidarProject\DTM";
            fusionIntegration fInt = new fusionIntegration();
            //fInt.checkAndRenameAllFiles(groundLas,".las");
            fInt.RunCloudMetrics(ftrCls, 11.5f, olasDir,dtm);
            //fInt.RunGroundFilter(olasDir, groundLas, 1);
            //fInt.RunGridSurfaceCreate(groundLas, dtm, 1);
            //fInt.RunGridMetrics(olasDir,metricsDir,23,0,dtm);
            //string[] mArr = fInt.MetricsArr;
            //fInt.ConvertGridMetricsToRaster(metricsDir, outDir, mArr);
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());

            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
