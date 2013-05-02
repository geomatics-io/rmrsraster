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
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();
            string tblSamp = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\SampleSize.gdb\trainSample";
            string tblPop = @"C:\Documents and Settings\jshogland\My Documents\JOHN\Requests\RobAhl\samplingProcedure\SampleSize.gdb\givenSamples";
            string outMd = @"C:\Documents and Settings\jshogland\My Documents\JOHN\temp\ks.mdl";
            esriUtil.Statistics.ModelHelper mh = new esriUtil.Statistics.ModelHelper(outMd);
            mh.openModelReport(outMd, 0.05, true);
            //ITable sample1 = geoUtil.getTable(tblPop);
            //ITable sample2 = geoUtil.getTable(tblSamp);
            //string[] explanitoryVariables = { "Mean_CPCA1", "Mean_CPCA2", "Mean_CPCA3" };
            //string strataField = "Cluster";
            //esriUtil.Statistics.dataPrepCompareSamples comp = new esriUtil.Statistics.dataPrepCompareSamples(sample1, sample2, explanitoryVariables, strataField, true);
            //comp.getReport();
            //comp.writeModel(@"C:\Documents and Settings\jshogland\My Documents\JOHN\temp\ks.mdl");
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
