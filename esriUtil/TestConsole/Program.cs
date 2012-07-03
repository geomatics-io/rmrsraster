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

            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string outWks = @"C:\Users\jshogland\Documents\John\temp\testFGD.gdb";
            string outRsNm = "rsEE";
            string InRsStr = outWks + "\\CCCD";
            IWorkspace wks = geoUtil.OpenRasterWorkspace(outWks);
            Console.WriteLine(wks.Type.ToString());
            IRaster rs = rsUtil.returnRaster(InRsStr);
            IRaster outrs = rsUtil.regionGroup(rs, wks, outRsNm);
            //esriUtil.Forms.RasterAnalysis.frmRegionGroup frm = new esriUtil.Forms.RasterAnalysis.frmRegionGroup(null, ref rsUtil, false);
            //System.Windows.Forms.Application.Run(frm);

            
            //rsUtil.calcStatsAndHist(rs, 5);
            //IRasterBandCollection rsBc = (IRasterBandCollection)rs2;
            //for (int i = 0; i < rsBc.Count; i++)
            //{

            //    IRasterBand rsB = rsBc.Item(i);
            //    Console.WriteLine("Max = " + rsB.Statistics.Maximum);
            //    Console.WriteLine("Min = " + rsB.Statistics.Minimum);
            //    Console.WriteLine("Mean = " + rsB.Statistics.Mean);
            //    Console.WriteLine("std = " + rsB.Statistics.StandardDeviation);
            //}
            //IPnt pntSize = new PntClass();
            //pntSize.SetCoords(512, 512);
            //IPnt pntLoc = new PntClass();
            //pntLoc.SetCoords(0, 0);

            //IPixelBlock pbOld = rs.CreatePixelBlock(pntSize);
            //IPixelBlock pbNew = rs2.CreatePixelBlock(pntSize);
            //rs.Read(pntLoc, pbOld);
            //rs2.Read(pntLoc, pbNew);
            //System.Array sArrOld = (System.Array)pbOld.get_SafeArray(0);
            //System.Array sArrNew = (System.Array)pbNew.get_SafeArray(0);
            //for (int c = 7; c < 100; c++)
            //{
            //    for (int r = 7; r < 100; r++)
            //    {
            //        double oVl = System.Convert.ToDouble(sArrOld.GetValue(c, r));
            //        double nVl = System.Convert.ToDouble(sArrNew.GetValue(c, r));
            //        Console.WriteLine("Old value = " + oVl.ToString() + " New value = " + nVl.ToString());
            //    }
            //}
                     
            System.DateTime dt2 = System.DateTime.Now;
            System.TimeSpan ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
