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
using Accord.Statistics.Testing.Power;



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
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();
            string zoneRsStr = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\Robb\TTest\remap.img";
            string vlRsStr = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\Robb\TTest\arivalTime.img";
            string outModel = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\Robb\TTest\ArivialTimeTTest8.mdl";
            esriUtil.Statistics.dataPrepPairedTTest pTT = new esriUtil.Statistics.dataPrepPairedTTest();
            pTT.buildModel(outModel);
            Console.WriteLine("Labels = " + String.Join(", ",pTT.Labels.ToArray()));
            Console.WriteLine("Total N = " + pTT.N);
            Console.WriteLine("Variables = " + String.Join(", ", pTT.VariableFieldNames));
            pTT.getReport();
            //esriUtil.Statistics.ModelHelper mh = new esriUtil.Statistics.ModelHelper(outModel);
            //mh.openModelReport(outModel, 0.05);
            
            
           
            //featureUtil ftrUtil = new featureUtil();
            //string inputTable = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\ClusterSampleSize\NPC_TRAINING_DATA.gdb\CLWNEZ_IMSTAT_BASE";
            //string clusterPath = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\ClusterSampleSize\CLWNEZ_IMSTAT_BASE.mdl";
            //ITable tbl = geoUtil.getTable(inputTable);
            //ftrUtil.selectStratifiedFeaturesToSample(tbl, clusterPath, "Cluster");
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
