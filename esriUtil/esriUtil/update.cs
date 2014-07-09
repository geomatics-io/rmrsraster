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
        private string baseVersion = "10.0";
        private string HelpFileName = "RmrsRasterUtilityToolbarHelp.chm";
        private string HelpFileVersion = "helpVersion.txt";
        private string toolbarVersion = "";
        private string appFileName = "servicesToolBar.esriAddIn";//servicesToolBarx.esriAddIn for anything later than 10.0
        private string appFileVersion = "toolbarVersion.txt";//toolbarVersionx.txt for anything later than 10.0
        public void updateApp(string curDir, string curVer)
        {
            bool cTb = true;
            if (baseVersion.ToLower() != curVer.ToLower())
            {
                string ust = "x";
                appFileName = "servicesToolBar" + ust + ".esriAddIn";
                if (!System.IO.File.Exists(curDir + "\\" + appFileName))
                {
                    if (System.Windows.Forms.MessageBox.Show("Found a new version of RMRS Raster Utility on the server. Do you want to download?", "Download", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        copyFileFromServer(appFileName);
                        cTb = false;
                    }
                }
                else
                {
                    cTb = checkToolbarVersion();
                }
            }
            else
            {
                cTb = checkToolbarVersion();
            }
            if (!cTb)
            {
                try
                {
                    removeRMRSRasterUtility(curDir,curVer);
                    System.Diagnostics.Process pc = new System.Diagnostics.Process();
                    
                    string lcInstall = rmrsDir + "\\" + appFileName;
                    pc.StartInfo.FileName = lcInstall;
                    pc.Start();
                }
                catch(Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Could not update addin. You will need to delete the addin folder and manually install!\n\n" + e.ToString() ); 
                }
                
            }


        }
        public bool updateHelp()
        {
            return checkHelpVersion();
        }
        private bool checkHelpVersion()
        {
            bool sameV = true;
            
            if ((UpdateCheck.ToLower()=="yes"))
            {
                
                string currentHelpVersion = esriUtil.Properties.Settings.Default.HelpVersion;
                string serverHelpVersion = getServerVersion(true);
                //Console.WriteLine(serverHelpVersion);
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
            }
            return sameV;

        }
        private bool checkToolbarVersion()
        {
            bool sameV = true;
            if ((UpdateCheck.ToLower() == "yes"))
            {
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
                string cvStr = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Version;
                if(cvStr!=baseVersion)
                {
                    string upStr = "x";
                    appFileVersion = "toolbarVersion" + upStr + ".txt";
                }
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
        private string upDateCheck = esriUtil.Properties.Settings.Default.AutoUpdate;
        public string UpdateCheck
        {
            get
            {
                //System.Windows.Forms.MessageBox.Show(upDateCheck);
                return upDateCheck;
            }
            set
            {
                string vl = value;
                if (vl.ToLower() == "yes")
                {
                    upDateCheck = "yes";
                    
                }
                else
                {
                    upDateCheck = "no";
                }
                esriUtil.Properties.Settings.Default.AutoUpdate = upDateCheck;
                esriUtil.Properties.Settings.Default.Save();
            }
        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        public void removeRMRSRasterUtility(string curDir,string curVer)
        {
            if (System.IO.Directory.Exists(curDir)) System.IO.Directory.Delete(curDir, true);
            


        }
        public void removeOldVersion(string folderId,string curVer)
        {
            try
            {
                string addinDirStr = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ArcGIS\\AddIns";//\\Desktop"
                foreach (string s in System.IO.Directory.GetDirectories(addinDirStr))
                {
                    string lsPart = System.IO.Path.GetFileName(s);
                    if (lsPart.ToLower().Contains(curVer.ToLower()))
                    {
                    }
                    else
                    {
                        foreach (string s2 in System.IO.Directory.GetDirectories(s))
                        {
                            string lsPart2 = System.IO.Path.GetFileName(s2);
                            if (lsPart2.ToLower() == folderId.ToLower())
                            {
                                System.IO.Directory.Delete(s2, true);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
