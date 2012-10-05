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
            //string wksStr = @"C:\Documents and Settings\jshogland\Local Settings\Temp\strConv";
            //IWorkspace wks = geoUtil.OpenRasterWorkspace(wksStr);
            //string xxx = @"C:\Documents and Settings\jshogland\Local Settings\Temp\strConv\rcz.img";
            //IRaster rs = rsUtil.calcAggregationFunction(xxx, 18, rasterUtil.focalType.SUM);
            //IRasterProps rsP = (IRasterProps)rs;
            //Console.WriteLine(rsP.MeanCellSize().X.ToString());
            //IRaster rs = rsUtil.calcAggregationFunction(xxx, 18, rasterUtil.focalType.SUM);
            //rsUtil.saveRasterToDataset(rs, "testAg", wks, rasterUtil.rasterType.IMAGINE);
            //IPnt pt = new PntClass();
            //pt.SetCoords(3, 3);
            //IPnt lc = new PntClass();
            //IRaster2 rs2 = (IRaster2)rs;
            //int sc, sr;
            //rs2.MapToPixel(-6502, 102603, out sc, out sr);
            //double mc, mr;
            //rs2.PixelToMap(sc, sr, out mc, out mr);
            //Console.WriteLine("Start C:R = " + mc.ToString() + ":" + mr.ToString());
            //lc.SetCoords(sc,sr);
            //IPixelBlock pb = rs.CreatePixelBlock(pt);
            //rs.Read(lc, pb);
            //System.Array sArr = (System.Array)pb.get_SafeArray(0);
            //for (int r = 0; r < 3; r++)
            //{
            //    for (int c = 0; c < 3; c++)
            //    {
            //        double vl = System.Convert.ToDouble(sArr.GetValue(c, r));
            //        Console.WriteLine("out value = " + vl.ToString());
            //    }
            //}
            //esriUtil.Forms.OptFuels.graphSedimentByArivalTime gSed = new esriUtil.Forms.OptFuels.graphSedimentByArivalTime(rsUtil, null);
            //string sPoly = @"C:\magfire\rcz\RCZ70_stands5d.shp";
            //IFeatureClass sPolyFtrCls = geoUtil.getFeatureClass(sPoly);
            //gSed.ResultsDir = @"C:\magfire\DBF_LT_WEST\INPUT\S4_WUI_42PERCENT\results";
            //gSed.StreamPolygon = sPolyFtrCls;

            esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime frm = new esriUtil.Forms.OptFuels.frmSummarizeGraphSedimentByArivalTime(null, rsUtil);// esriUtil.Forms.RasterAnalysis.frmBatchProcess();
            System.Windows.Forms.Application.Run(frm);    
            System.DateTime dt2 = System.DateTime.Now;
            System.TimeSpan ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
