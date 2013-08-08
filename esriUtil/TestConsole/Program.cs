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
            //Console.WriteLine(System.Convert.ToInt32(a));
            //Console.WriteLine((int)a);
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();
            //featureUtil ftUtil = new featureUtil();
            string dem10Str = @"C:\Documents and Settings\jshogland\My Documents\JOHN\presentation\Authoring\fy2013\COFE\GISData\COFE.gdb\dem";
            string subFtr = @"C:\Documents and Settings\jshogland\My Documents\JOHN\presentation\Authoring\fy2013\COFE\GISData\COFE.gdb\subset";
            IRaster dem10 = rsUtil.returnRaster(dem10Str);
            IFeatureClass ftrCls = geoUtil.getFeatureClass(subFtr);
            IFeatureCursor cur = ftrCls.Search(null, false);
            IFeature ftr = cur.NextFeature();
            IGeometry geo = ftr.Shape;
            IRaster oRs = rsUtil.clipRasterFunction(dem10, geo, esriRasterClippingType.esriRasterClippingOutside);
            //IRaster asp = rsUtil.calcAspectFunction(dem10);
            //string mdPath = @"C:\Documents and Settings\jshogland\My Documents\JOHN\presentation\Authoring\fy2013\COFE\GISData\rfBiomass.mdl";
            //IRaster CoefficentRaster = rsUtil.compositeBandFunction(new IRaster[] { asp, dem10 });
            //esriUtil.Statistics.ModelHelper mh = new esriUtil.Statistics.ModelHelper(mdPath, CoefficentRaster, rsUtil);
            //IRaster oRs = mh.getRaster();
            IPnt pnt = new PntClass();
            pnt.SetCoords(100,100);
            IPnt pntLoc = new PntClass();
            pntLoc.SetCoords(0, 0);
            IPixelBlock pb = oRs.CreatePixelBlock(pnt);
            oRs.Read(pntLoc, pb);
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 10; c++)
                {
                    object vl = pb.GetVal(0, c, r);
                    if (vl == null) Console.WriteLine("Null");
                    else Console.WriteLine(vl.ToString());
                }
            }
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Pointer Total Seconds = " + ts.TotalSeconds.ToString());
            m_AOLicenseInitializer.ShutdownApplication();
                
        }
    }
}
