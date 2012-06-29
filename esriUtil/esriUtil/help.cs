using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using esriUtil;
using System.Net;

namespace esriUtil
{
    public class help
    {
        public void start()
        {
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new Forms.RunningProcess.frmRunningProcessDialog(false);
            bool closeAuto = true;
            try
            {
                System.Diagnostics.Process pr = new System.Diagnostics.Process();
                geoUtil.check_dir(rmrsDir);
                string hFl = rmrsDir + "\\" + HelpFileName;
                pr.StartInfo.FileName = hFl;
                rp.addMessage("Checking for latest version of Help...");
                rp.stepPGBar(15);
                rp.TopMost = true;
                rp.Show();
                rp.Refresh();
                checkHelpVersion();
                if (System.IO.File.Exists(hFl))
                {
                    rp.addMessage("Updating help...");
                    pr.Start();
                }
                else
                {
                    rp.addMessage("Can't find help files. Either the server is down or you are not connected to the internet. Please try again later.");
                    esriUtil.Properties.Settings.Default.HelpVersion = "unknown";
                    esriUtil.Properties.Settings.Default.Save();
                    closeAuto = false;
                }

            }
            catch (Exception e)
            {
                closeAuto = false;
                rp.addMessage("Can't find help files. Either the server is down or you are not connected to the internet. Please try again later.\n" + e.ToString());
            }
            finally
            {
                rp.addMessage("Finished Checking Help");
                rp.stepPGBar(100);
                rp.enableClose();
                if (closeAuto)
                {
                    rp.Close();
                }
                
            }
            
            
        }
        private string HelpFileName = "RmrsRasterUtilityToolbarHelp.chm";
        private string HelpFileVersion = "helpVersion.txt";
        private bool checkHelpVersion()
        {
            bool sameV = true;
            string currentHelpVersion = esriUtil.Properties.Settings.Default.HelpVersion;
            string serverHelpVersion = getServerHelpVersion();
            //Console.WriteLine( serverHelpVersion);
            if (serverHelpVersion == null)
            {
                System.Windows.Forms.MessageBox.Show("Can't access the server try again later server help == null");
            }
            else if (currentHelpVersion != serverHelpVersion)
            {
                if(System.Windows.Forms.MessageBox.Show("Found a new version of help on the server. Do you want to download the help?","Download",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Question)== System.Windows.Forms.DialogResult.Yes)
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
            System.Net.NetworkInformation.PingReply rp = png.Send(rmrsDir,120);
            System.Net.NetworkInformation.IPStatus ipStat = rp.Status;
            if (ipStat != System.Net.NetworkInformation.IPStatus.Success)
            {
                up = false;
            }
            return up;


        }

        private string getServerHelpVersion()
        {
            string localcopyofHelpFile = copyFileFromServer(HelpFileVersion);
            string ln = null;
            if (localcopyofHelpFile != null)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(localcopyofHelpFile))
                {
                    ln = sr.ReadLine();
                    if(ln==null||ln.Length==0)
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
