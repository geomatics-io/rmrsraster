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
            try
            {
                geoDatabaseUtility geoUtil = new geoDatabaseUtility();
                System.Diagnostics.Process pr = new System.Diagnostics.Process();
                geoUtil.check_dir(rmrsDir);
                string hFl = rmrsDir + "\\" + HelpFileName;
                pr.StartInfo.FileName = hFl;
               
                if (System.IO.File.Exists(hFl))
                {
                    pr.Start();
                }
                else
                {
                    update up = new update();
                    try
                    {
                        System.Windows.Forms.MessageBox.Show("Can't find help files. Trying to download from the internet.");
                        if (up.updateHelp())
                        {
                            pr.Start();
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Can't find help files on the internet. Try again later.");
                        }
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Can't find help files on the internet. Try again later.");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                
            }
            
            
        }
        private string HelpFileName = "RmrsRasterUtilityToolbarHelp.chm";
        private string rmrsDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RmrsRasterUtilityHelp";
    }
}
