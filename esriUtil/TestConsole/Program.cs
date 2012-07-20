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


            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            esriUtil.FunctionRasters.NeighborhoodHelper.fastArrayManipulation fsArr = new esriUtil.FunctionRasters.NeighborhoodHelper.fastArrayManipulation();
            string inWks = @"C:\Users\jshogland\Documents\John\temp\testFGD.gdb";
            string outWks = @"C:\Users\jshogland\Documents\John\temp\testGrid";
            string outRsNm = "rsEE";
            string InRsStr = inWks + "\\CCCD";
            esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime frm = new esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime();
            System.Windows.Forms.Application.Run(frm);
            //int[,] myArray = { { 1, 2, 3, 4, 5, 6 }, { 7,8,9,10,11,12 }, {13,14,15,16,17,18 }, { 19,20,21,22,23,24 }, { 25,26,27,28,29,30  } };
            
            //var outSelect = from int v in myArray select v;
            //var subArray = outSelect.Where((n, i) => (i > 0 && i < 5) || (i > 6 && i < 11) || (i > 12 && i < 17) || (i > 18 && i < 23) || (i > 24 && i < 29));
            //Console.WriteLine("Sum = " + subArray.Sum().ToString());
            //Console.WriteLine("Count = " + subArray.Count().ToString());
            //foreach (int v in subArray.Distinct())
            //{
            //    Console.WriteLine(v.ToString());
            //}
            //Console.WriteLine(outSelect.Sum());
            //Console.WriteLine(mA2.Sum());
  
            
            //int arraySum = (from int v in myArray select v).Sum();
            //int arrayLow = (from int v in myArray where v < 5 select v).Sum();
            
            //Console.WriteLine(arraySum.ToString());
            //Console.WriteLine(arrayLow.ToString());

            //double[] vlArr = { 6, 10, 11, 12 };
            //IRaster rs = rsUtil.calcFocalStatisticsFunction(InRsStr,5,5,rasterUtil.focalType.ENTROPY);
            //IPnt loc = new PntClass();
            //loc.SetCoords(7, 0);
            //IPnt sz = new PntClass();
            //sz.SetCoords(100, 100);
            //IPixelBlock pb = rs.CreatePixelBlock(sz);
            //rs.Read(loc, pb);
            //System.Array sArr = (System.Array)pb.get_SafeArray(0);
            //for (int i = 0; i < 100; i++)
            //{
            //    for (int j = 0; j < 100; j++)
            //    {
            //        double vl = System.Convert.ToDouble(sArr.GetValue(i, j));
            //        Console.WriteLine(vl.ToString());
            //    }
            //}
            //var dArr = from byte s in sArr select System.Convert.ToInt32(s);
            //var dGArr = from int i in dArr group i by i;
            //foreach (IGrouping<int, int> gp in dGArr)
            //{
                
            //    Console.WriteLine("Cell value = " + gp.Key.ToString()+ " and Count = " + gp.Count());
            //}
            //IWorkspace wks = geoUtil.OpenRasterWorkspace(outWks);
            //IRaster rs = rsUtil.returnRaster(InRsStr);
            //IRaster outrs = rsUtil.regionGroup(rs, wks, outRsNm);
            
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
