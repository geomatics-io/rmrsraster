using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.Net;

namespace esriUtil
{
    public class update
    {
        public update()
        {
        }
        public update(string ToolBarVersion)
        {
            toolbarVersion = ToolBarVersion;
        }
        private string HelpFileName = "RmrsRasterUtilityToolbarHelp.chm";
        private string HelpFileVersion = "helpVersion.txt";
        private string toolbarVersion = "";
        private string appFileName = "serviceToolBar.esriaddin";
        private string appFileVersion = "toolbarVersion.txt";
        public void updateApp()
        {
            if (!checkToolbarVersion())
            {

                System.Diagnostics.Process pc = new System.Diagnostics.Process();
                string lcInstall = rmrsDir + "\\" + appFileName;
                pc.StartInfo.FileName = lcInstall;
                pc.Start();
            }


        }
        public bool updateHelp()
        {
            return checkHelpVersion();
        }
        private bool checkHelpVersion()
        {
            bool sameV = true;
            string currentHelpVersion = esriUtil.Properties.Settings.Default.HelpVersion;
            string serverHelpVersion = getServerVersion(true);
            //Console.WriteLine( serverHelpVersion);
            if (serverHelpVersion == null)
            {
                //System.Windows.Forms.MessageBox.Show("Can't access the server try again later server help == null");
            }
            else if (currentHelpVersion != serverHelpVersion)
            {
                if (System.Windows.Forms.MessageBox.Show("Found a new version of RMRS Raster Utility help on the server. Do you want to download the help?", "Download", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    copyFileFromServer(HelpFileName);
                    if (HelpFileName != null)
                    {
                        esriUtil.Properties.Settings.Default.HelpVersion = serverHelpVersion;
                        esriUtil.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        sameV = false;
                    }
                }

            }
            else
            {
            }
            return sameV;

        }
        private bool checkToolbarVersion()
        {
            bool sameV = true;
            string currentAppVersion = toolbarVersion;
            string serverAppVersion = getServerVersion(false);
            //Console.WriteLine( serverHelpVersion);
            if (serverAppVersion == null)
            {
                //System.Windows.Forms.MessageBox.Show("Can't access the server try again later");
            }
            else if (currentAppVersion != serverAppVersion)
            {
                if (System.Windows.Forms.MessageBox.Show("Found a new version of RMRS Raster Utility on the server. Do you want to download?", "Download", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    copyFileFromServer(appFileName);
                    sameV = false;
                }

            }
            else
            {
            }
            return sameV;

        }

        private string copyFileFromServer(string nm)
        {
            string svDir = rmrsDownload;
            string website = parseWebsite(svDir);
            string outName = null;
            bool serverUp = pingServer(website);
            if (serverUp)
            {
                try
                {
                    WebClient wbClient = new WebClient();
                    wbClient.BaseAddress = svDir;
                    wbClient.DownloadFile(svDir + nm, rmrsDir + "\\" + nm);
                    outName = rmrsDir + "\\" + nm;
                }
                catch
                {

                }
            }
            else
            {
                if (System.IO.File.Exists(rmrsDir + "\\" + nm))
                {
                    outName = rmrsDir + "\\" + nm;
                }
            }
            return outName;

        }

        private string parseWebsite(string svDir)
        {
            string outWebsit = "www.fs.fed.us";
            int www = svDir.IndexOf("www.");
            string sub = svDir.Substring(www);
            outWebsit = sub.Split(new char[] { '/' })[0];
            return outWebsit;
        }

        private bool pingServer(string rmrsDir)
        {
            bool up = true;
            System.Net.NetworkInformation.Ping png = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply rp = png.Send(rmrsDir, 120);
            System.Net.NetworkInformation.IPStatus ipStat = rp.Status;
            if (ipStat != System.Net.NetworkInformation.IPStatus.Success)
            {
                up = false;
            }
            return up;


        }

        private string getServerVersion(bool help)
        {
            string localcopyofHelpFile = null;
            if (help)
            {
                localcopyofHelpFile = copyFileFromServer(HelpFileVersion);
            }
            else
            {
                localcopyofHelpFile = copyFileFromServer(appFileVersion);
            }
            string ln = null;
            if (localcopyofHelpFile != null)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(localcopyofHelpFile))
                {
                    ln = sr.ReadLine();
                    if (ln == null || ln.Length == 0)
                    {
                        ln = "";
                    }
                    else
                    {
                        ln = ln.Trim();
                    }
                    sr.Close();
                }
            }
            return ln;
        }
        private string rmrsDownload = esriUtil.Properties.Settings.Default.AppDownLoad;
        private string rmrsDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RmrsRasterUtilityHelp";
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
    }
}
