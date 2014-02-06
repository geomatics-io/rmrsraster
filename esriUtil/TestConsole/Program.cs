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
            
            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string ls = @"C:\Users\jshogland\Documents\JOHN\Requests\RobAhl\IMAGERY\imageTrans.gdb\pr4026";
            string wksStr = @"C:\Users\jshogland\Documents\JOHN\Requests\RobAhl\IMAGERY";
            IWorkspace wks = geoUtil.OpenRasterWorkspace(wksStr);
            string outName = "testout3";
            IRaster rs = rsUtil.returnRaster(ls);
            //IRaster nRs = rsUtil.calcArithmaticFunction(rs, 5, esriRasterArithmeticOperation.esriRasterPlus);
            //IRaster nRs2 = rsUtil.convertToDifFormatFunction(nRs, rstPixelType.PT_UCHAR);
            IRasterDataset rsD2 = rsUtil.saveRasterToDatasetM(rs, outName, wks, rasterUtil.rasterType.IMAGINE,0);

            //esriUtil.Statistics.dataPrepClusterBinary bClus = new esriUtil.Statistics.dataPrepClusterBinary(rs, 10);
            //IRaster bClusRs = rsUtil.calcClustFunctionBinary(rs, bClus);
            //IRasterProps rsProps = (IRasterProps)bClusRs;
            //int wCells= rsProps.Width;
            //int hCells = rsProps.Height;
            //Dictionary<int, List<int>> vlArrayDic = new Dictionary<int, List<int>>();
            //IRasterCursor rsCur = bClusRs.CreateCursor();
            //IPixelBlock pb = null;
            //do
            //{
            //    IPnt tlPnt = rsCur.TopLeft;
            //    int x = System.Convert.ToInt32(tlPnt.X);
            //    int y = System.Convert.ToInt32(tlPnt.Y);
            //    pb = rsCur.PixelBlock;
            //    for (int r = 0; r < pb.Height; r++)
            //    {
            //        int yjump = (y+r)*wCells;
            //        for (int c = 0; c < pb.Width; c++)
            //        {
            //            object objVl = pb.GetVal(0, c, r);
            //            if (objVl != null)
            //            {
            //                int vl = System.Convert.ToInt32(objVl);
            //                int lc = yjump+(x+c);
            //                List<int> oLst = null;
            //                if (vlArrayDic.TryGetValue(vl, out oLst))
            //                {
            //                    oLst.Add(lc);
            //                }
            //                else
            //                {
            //                    oLst = new List<int>();
            //                    oLst.Add(lc);
            //                    vlArrayDic.Add(vl, oLst);
            //                }
            //            }
            //        }
                    
            //    }
			
            //} while (rsCur.Next() == true);
            //Console.WriteLine(vlArrayDic.Keys.Count.ToString());
            //Console.WriteLine(vlArrayDic[0].Count.ToString());
            //string ls8 = @"C:\Users\jshogland\Documents\JOHN\presentation\Authoring\fy2014\MAGIP\Data\RMRS_Raster_Utility.gdb\LS8Clip";
            //IRaster inRs = rsUtil.returnRaster(ls8);
            //esriUtil.Statistics.dataPrepClusterBinary dpBinary = new esriUtil.Statistics.dataPrepClusterBinary(inRs, 10);
            //IRaster cluRs = rsUtil.calcClustFunctionBinary(inRs, dpBinary);
            //IRaster regRs = rsUtil.regionGroup(cluRs);
            ////esriUtil.segmentation seg = new segmentation(rsUtil, inRs, 406400, 40640, 10);
            ////seg.createPolygons();           
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
