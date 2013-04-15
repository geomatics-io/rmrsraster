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
            string tblStr = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\SampleSize.gdb\valdationSample";
            string mf = "F_DTB_mlc";
            string rf = "F_DTB";
            string wf = "weight";
            esriUtil.Statistics.dataGeneralConfusionMatirx aa = new esriUtil.Statistics.dataGeneralConfusionMatirx(geoUtil.getTable(tblStr), rf, mf);
            aa.WeightFeild = wf;
            Console.WriteLine(aa.Overall.ToString());
            Console.WriteLine(aa.Kappa.ToString());
            

           
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
