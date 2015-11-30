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
            featureUtil ftrUtil = new featureUtil();
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            rasterUtil rsUtil = new rasterUtil();

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Console.WriteLine(exePath);


            //string baseUrl = @"ftp://rockyftp.cr.usgs.gov/vdelivery/Datasets/Staged/NAIP/al_2013/";
            //string outDir = @"E:\Florida\NAIP2013\Alabama";
            //string inDir = @"E:\Florida\NAIP2013";
            //List<string> fnLst = getFileNames(baseUrl);
            //moveFiles(inDir,outDir,fnLst);
            
            dt2 = System.DateTime.Now;
            ts = dt2.Subtract(dt);
            Console.WriteLine("Total Seconds = " + ts.TotalSeconds.ToString());

            m_AOLicenseInitializer.ShutdownApplication();
                
        }

        private static void moveFiles(string inDir, string outDir, List<string> fnLst)
        {
            string[] existingFiles = System.IO.Directory.GetFiles(inDir,"*.jp2");
            foreach (string s in existingFiles)
            {
                string fn = System.IO.Path.GetFileName(s);
                if (fnLst.Contains(fn))
                {
                    System.IO.File.Move(s, outDir + "\\" + fn);
                    Console.WriteLine("Moving file " + fn);
                }
                
            }

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
                outLst.Add(lnArr[lnArr.Length - 1].Trim().ToLower());
            }
            sr.Close();
            response.Close();
            return outLst;
        }
    }
}
