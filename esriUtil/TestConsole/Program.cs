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
//using System.Windows.Forms.DataVisualization;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing;



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

            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            string pred = @"C:\Users\jshogland\Documents\John\projects\FtStewart\models\TreeEstimates\HdwPresentPred.bch";
            string mdl = @"C:\Users\jshogland\Documents\John\projects\FtStewart\models\TreeEstimates\HdwPresent.mdl";
            double[] minArr, maxArr;
            Console.WriteLine("Getting Min Max");
            batchCalculations.getMinMaxArr(mdl,out minArr,out maxArr);
            Console.WriteLine("First Min Max: " + minArr[0].ToString() + ";" + maxArr[0].ToString());
            IFunctionRasterDataset fdset = rsUtil.extactModelDomainFunction(pred,minArr,maxArr);
            Console.WriteLine(fdset.RasterInfo.PixelType.ToString());
            IRaster rs = rsUtil.createRaster(fdset);
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(100, 100);
            IPixelBlock pb = rs.CreatePixelBlock(pntSize);
            rs.Read(pntSize, pb);
            int cnt = 0;
            for (int r = 0; r < pb.Height; r++)
            {
                for (int c = 0; c < pb.Width; c++)
                {
                    object vl = pb.GetVal(0, c, r);
                    if(vl!=null)
                    {
                        if(System.Convert.ToInt32(vl)==0)
                        {
                            cnt++;
                        }
                    }
                }
            }
            Console.WriteLine("Total zeros = " + cnt.ToString());
            //string dir = @"C:\Users\jshogland\Documents\John\projects\UMGradSchool\Classes\Mining Big Data\Assignments\NatureServe\train\ALB";
            //string[] fls = System.IO.Directory.GetFiles(dir, "*.jpg");
            //foreach (string flPath in fls)
            //{
            //    string nm = System.IO.Path.GetFileNameWithoutExtension(flPath);
            //    Console.WriteLine(nm);
            //    Bitmap bm = new Bitmap(flPath);
            //    Accord.Imaging.Moments.CentralMoments cm = new Accord.Imaging.Moments.CentralMoments(bm, 3);
            //    //Console.WriteLine(cm.Mu00.ToString());
            //    Console.WriteLine("\t"+cm.Mu01.ToString());
            //    Console.WriteLine("\t" + cm.Mu02.ToString());
            //    Console.WriteLine("\t" + cm.Mu03.ToString());
            //    Console.WriteLine("\t" + cm.Mu10.ToString());
            //    Console.WriteLine("\t" + cm.Mu20.ToString());
            //    Console.WriteLine("\t" + cm.Mu30.ToString());
            //}

            //string plots = @"C:\Users\jshogland\Documents\John\projects\UMGradSchool\Project\papers\Co-registration\Data\coReg.gdb\SampleLocations";
            //string ls= @"C:\Users\jshogland\Documents\John\projects\UMGradSchool\Project\papers\Co-registration\Data\FloridaLandsat.tif";
            //string naip= @"C:\Users\jshogland\Documents\John\projects\UMGradSchool\Project\papers\Co-registration\Data\GeorgiaNaipAgAl.tif";
            //rasterUtil rsUtil = new rasterUtil();
            
            
            
            
            
            
            
            //HashSet<string> uStr = new HashSet<string>();
            //int[] filesLines = getClasses(prjDir,ref uStr);
            //Console.WriteLine("Files = " + filesLines[0].ToString());
            //Console.WriteLine("Lines = " + filesLines[1].ToString());
            //Console.WriteLine(uStr.Count().ToString());
            



            //geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            //rasterUtil rsUtil = new rasterUtil();
            //featureUtil ftrUtil = new featureUtil(rsUtil);
            ////string origPath = @"C:\Users\jshogland\Documents\John\projects\SpatialAdjust\Shift.gdb\OriginalSample";
            ////string nwPath = @"C:\Users\jshogland\Documents\John\projects\SpatialAdjust\Shift.gdb\RandomtShiftSample";
            ////CreateNewPoints(origPath, nwPath, rsUtil, ftrUtil, geoUtil, true);
            //string fiaTestPath = @"C:\Users\jshogland\Documents\John\projects\SpatialAdjust\Shift.gdb\RandomtShiftSample";
            //string fiaOutPath = @"C:\Users\jshogland\Documents\John\projects\SpatialAdjust\Shift.gdb\adjustSample";
            //string rsPath = @"C:\Users\jshogland\Documents\John\projects\SpatialAdjust\models\predictorsOneVariable.bch";
            //IFunctionRasterDataset fDset = rsUtil.createIdentityRaster(rsPath);
            //IFeatureClass ftrCls = geoUtil.getFeatureClass(fiaTestPath);
            //int error = 15;
            //adjustCoregistrationErrors aE = new adjustCoregistrationErrors(rsUtil);
            //aE.FunctionRasterDataset = fDset;
            //aE.DependentField = "Pred15";
            //aE.GeometricErrorCells = error;
            //aE.OutFtrClassPath = fiaOutPath;
            //aE.PlotFeatureClass = ftrCls;
            //aE.adjustErrors();
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());

            m_AOLicenseInitializer.ShutdownApplication();
                
        }

        private static int[] getClasses(string prjDir,ref HashSet<string> hs)
        {
            int[] outVl=new int[2];
            string[] csPath = System.IO.Directory.GetFiles(prjDir, "*.cs");
            outVl[0] = outVl[0] + csPath.Length;
            outVl[1] = outVl[1] + getClassName(csPath,ref hs);
            foreach (string d in System.IO.Directory.GetDirectories(prjDir))
            {
                int[] tVl = getClasses(d,ref hs);
                outVl[0] = outVl[0] + tVl[0];
                outVl[1] = outVl[1] + tVl[1];
            }
            return outVl;
        }

        private static int getClassName(string[] csPath,ref HashSet<string> hs)
        {
            int cnt = 0;
            foreach(string s in csPath)
            {
                using(System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    string ln;
                    while((ln= sr.ReadLine()) != null)
                    {
                        cnt += 1;
                        if(ln.Contains("class "))
                        {
                            string[] lnArr = ln.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                            int ind = System.Array.IndexOf(lnArr, "class");
                            string clsName = lnArr[ind + 1];
                            hs.Add(clsName);
                        }
                    }
                }
            }
            return cnt;
        }
        private static void CreateNewPoints(string inPath,string outPath, rasterUtil rsUtil, featureUtil ftrUtil, geoDatabaseUtility geoUtil, bool prnd = true, int cShift = 5)
        {
            
            IFeatureClass ftrCls = geoUtil.getFeatureClass(inPath);
            IFeatureClass newFtrCls = ftrUtil.exportFeatures(ftrCls, outPath, null);
            IFeatureCursor uCur = newFtrCls.Update(null, true);
            IFeature ftr = uCur.NextFeature();
            if(prnd)
            { 
                System.Random rnd = new Random();
                while (ftr != null)
                {
                    IPoint opnt = (IPoint)ftr.ShapeCopy;
                    IPoint pnt = new PointClass();
                    double rndx = rnd.NextDouble() * 10;
                    double rndy = rnd.NextDouble() * 10;
                    pnt.PutCoords(opnt.X + rndx, opnt.Y + rndy);
                    ftr.Shape = pnt;
                    uCur.UpdateFeature(ftr);
                    ftr = uCur.NextFeature();
                }
            }
            else
            {
                while (ftr != null)
                {
                    IPoint opnt = (IPoint)ftr.ShapeCopy;
                    IPoint pnt = new PointClass();
                    pnt.PutCoords(opnt.X + cShift, opnt.Y + cShift);
                    ftr.Shape = pnt;
                    uCur.UpdateFeature(ftr);
                    ftr = uCur.NextFeature();
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);
        }
    }
}
