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



namespace TestConsole
{
    class Program
    {

        private static LicenseInitializer m_AOLicenseInitializer = new TestConsole.LicenseInitializer();
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced}, new esriLicenseExtensionCode[] {});
            System.DateTime dt = System.DateTime.Now;
            
            System.DateTime dt2;
            TimeSpan ts;
            featureUtil ftrUtil = new featureUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string baseUrl = @"http://gis.apfo.usda.gov/arcgis/services";
            string outPath = @"C:\Users\jshogland\Documents\JOHN\projects\R8_Longleaf\LongleafProject.gdb\gaFlightLines";
            IAGSServerConnection con = mapserviceutility.GetMapServerConnection(baseUrl);
            Dictionary<string, IAGSServerObjectName> conDic = mapserviceutility.getServerObjects(con);
            //foreach (string s in conDic.Keys)
            //{
            //    Console.WriteLine(s);
            //}
            IAGSServerObjectName svrObjName = conDic["NAIP/NAIP_Image_Dates"];
            IMapServer mpSvr = mapserviceutility.getMapServer(svrObjName);
            IFeatureClass ftrCls = mapserviceutility.createFeatureClassFromMapService(outPath, mpSvr, 40, 0);
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());

            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
