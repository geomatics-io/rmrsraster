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
            rasterUtil rsUtil = new rasterUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            //string rsStr1 = @"C:\Users\jshogland\Documents\JOHN\Requests\JasonDrake\LongleafProbabilityGrids.gdb\CoastalLL";
            //string rsStr2 = @"C:\Users\jshogland\Documents\JOHN\Requests\JasonDrake\LongleafProbabilityGrids.gdb\Deciduous";
            //string rsStr3 = @"C:\Users\jshogland\Documents\JOHN\Requests\JasonDrake\LongleafProbabilityGrids.gdb\Evergreen";
            //IRasterBandCollection rsBC = new RasterClass();
            //IFunctionRasterDataset rDset1 = rsUtil.createIdentityRaster(rsStr1);
            //IFunctionRasterDataset rDset2 = rsUtil.createIdentityRaster(rsStr2);
            //IFunctionRasterDataset rDset3 = rsUtil.createIdentityRaster(rsStr3);
            //rsBC.AppendBands((IRasterBandCollection)rDset1);
            //rsBC.AppendBands((IRasterBandCollection)rDset2);
            //rsBC.AppendBands((IRasterBandCollection)rDset3);
            //IFunctionRasterDataset comp = rsUtil.compositeBandFunction(rsBC);


            ////IFunctionRasterDataset fDset = rsUtil.createIdentityRaster(rsStr);
            //IFunctionRasterDataset fDset2 = rsUtil.localStatisticsfunction(comp, rasterUtil.localType.MAXBAND);
            ////IFunctionRasterDataset fDset3 = rsUtil.constantRasterFunction(fDset2, 0, rstPixelType.PT_UCHAR);
            //IFunctionRasterDataset fDset1 = rsUtil.convertToDifFormatFunction(fDset2, rstPixelType.PT_UCHAR);
            //Console.WriteLine("Pixel Type of fdset 1" + fDset1.RasterInfo.PixelType.ToString());
            //IPnt pntSize = new PntClass();
            //IPnt pLoc = new PntClass();
            //pntSize.SetCoords(6000, 6000);
            //pLoc.SetCoords(10000, 10000);
            ////Console.WriteLine("Running dataset 1");
            ////runRasterPb(fDset, pntSize, pLoc, rsUtil);
            //Console.WriteLine("Running dataset 2");
            //runRasterPb(fDset1, pntSize, pLoc, rsUtil);


            string ftpSite = @"ftp://rockyftp.cr.usgs.gov/vdelivery/Datasets/Staged/NAIP/mt_2013/";
            List<string> fNames = getFileNames(ftpSite);
            string outDir = @"E:\Helena\NAIP\Bulk Order 421750\NAIP JPG2000";
            List<string> exNames = getExistingNames(outDir);
            List<string> tiles = getTiles();
            for (int i = 0; i < fNames.Count; i++)
            {
                string fName = fNames[i];
                string lfn = fName.Substring(0, 7);
                if (tiles.Contains(lfn.ToLower()))
                {
                    if (!exNames.Contains(fName))
                    {
                        //Console.WriteLine("Total number of files = " + fNames.Count.ToString());
                        Console.WriteLine("Downloading " + fName);
                        bool gotFile = Download(ftpSite, outDir, fName);
                        Console.WriteLine("Got File " + fName + " " + gotFile.ToString());
                    }
                }
            }
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds RawBlock= " + ts.TotalSeconds.ToString());

            m_AOLicenseInitializer.ShutdownApplication();
                
        }

        private static List<string> getTiles()
        {
            List<string> outLSt = new List<string>();
            string[] lng = { "113", "112", "111", "110" };
            string[] lt = { "47", "46", "45" };
            for (int i = 0; i < lng.Length; i++)
            {
                for (int j = 0; j < lt.Length; j++)
                {
                    string sStr = "m_" + lt[j] + lng[i];
                    outLSt.Add(sStr);
                }
            }
            return outLSt;
        }

        private static List<string> getExistingNames(string outDir)
        {
            List<string> outLst = new List<string>();
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(outDir);
            System.IO.FileInfo[] fInfoArr = d.GetFiles("*.jp2");
            for (int i = 0; i < fInfoArr.Length; i++)
            {
                System.IO.FileInfo f = fInfoArr[i];
                outLst.Add(f.Name);
            }
            return outLst;

        }
        private static bool Download(string ftpPath, string filePath, string fileName)
        {
            bool ch = true;
            FtpWebRequest reqFTP;
            try
            {
                
                System.IO.FileStream outputStream = new System.IO.FileStream(filePath +"\\" + fileName, System.IO.FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential("anonymous","anonymous");
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                System.IO.Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ch = false;
            }
            return ch;
        }

        private static bool downloadFile(string ftpSite, string p, string outDir, string p_2)
        {
            bool ch = true;

            try
            {
                WebClient wbc = new WebClient();
                wbc.BaseAddress = ftpSite;
                wbc.DownloadFile(ftpSite + p, outDir + p_2);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                ch = false;
            }
            return ch;
        }
        private static List<string> getFileNames(string ftpPath)
        {
            List<string> outLst = new List<string>();

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", "anonymous");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(responseStream);
            string ln;
            while ((ln = sr.ReadLine()) != null)
            {
                string[] lnArr = ln.Split(new char[] { ' ' });
                outLst.Add(lnArr[lnArr.Length - 1]);
            }
            sr.Close();
            response.Close();
            return outLst;

            

        }
        private static void runRawBlock(IFunctionRasterDataset fDset, IPnt pSize, IPnt pLoc, rasterUtil rsUtil)
        {
            IRaster2 rs2 = (IRaster2)rsUtil.createRaster(fDset);
            for (int b = 0; b < 1; b++)
            {
                object vl = rs2.GetPixelValue(b, 100, 100);
            }
            //IRawBlocks rPb = (IRawBlocks)fDset;
            //IPixelBlock pb = rPb.CreatePixelBlock();
            //int bEndc = (int)Math.Ceiling(pSize.X / pb.Width);
            //int bEndr = (int)Math.Ceiling(pSize.Y / pb.Height);
            //for (int rb = 0; rb < bEndr; rb += (int)pSize.Y)
            //{
            //    for (int cb = 0; cb < bEndc; cb += (int)pSize.X)
            //    {
            //        for (int b = 0; b < rPb.RasterInfo.BandCount; b++)
            //        {
            //            Console.WriteLine("Band " + b.ToString());
            //            rPb.ReadBlock(100, 100, b, pb);
            //            for (int r = 0; r < pb.Height; r++)
            //            {
            //                for (int c = 0; c < pb.Width; c++)
            //                {
            //                    object vl = pb.GetVal(0, c, r);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private static void runRawPixel(IFunctionRasterDataset fDset, IPnt pSize, IPnt pLoc, rasterUtil rsUtil)
        {
            IRasterBandCollection rsBc = (IRasterBandCollection)fDset;
            for (int b = 0; b < 1; b++)
            {
                IRasterBand rsb = rsBc.Item(b);
                IRawPixels rP = (IRawPixels)rsb;
                IPixelBlock pb = rP.CreatePixelBlock(pSize);
                rP.Read(pLoc, pb);
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vl = pb.GetVal(0, c, r);
                    }
                }
            }
        }

        private static void runRasterPb(IFunctionRasterDataset fDset, IPnt pSize, IPnt pLoc, rasterUtil rsUtil)
        {
            IRaster rs = rsUtil.createRaster(fDset);
            IPixelBlock pb = rs.CreatePixelBlock(pSize);
            rs.Read(pLoc, pb);
            for (int b = 0; b < 1; b++)
            {
                Console.WriteLine("PixelBlock type = " + pb.PixelType[b].ToString());
                Console.WriteLine("fDset type = " + fDset.RasterInfo.PixelType.ToString());
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vl = pb.GetVal(b, c, r);
                        //if (vl == null) Console.WriteLine("VL = null for r,c " + r.ToString() + ", " + c.ToString());
                        //else Console.WriteLine("VL = " + vl.ToString());
                    }
                }
                
            }
            
        }
    }
}
