using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace esriUtil
{
    public static class msofficeHelper
    {
        public enum OfficeComponent { Word, Excel, PowerPoint, Outlook, Access }
        private static string GetComponentPath(OfficeComponent officeComponent)
        {
            const string RegKey = @"Software\Microsoft\Windows\CurrentVersion\App Paths";
            string vl = string.Empty;
            string key = string.Empty;

            switch (officeComponent)
            {
                case OfficeComponent.Word:
                    key = "winword.exe";
                    break;
                case OfficeComponent.Excel:
                    key = "excel.exe";
                    break;
                case OfficeComponent.PowerPoint:
                    key = "powerpnt.exe";
                    break;
                case OfficeComponent.Outlook:
                    key = "outlook.exe";
                    break;
                case OfficeComponent.Access:
                    key = "msaccess.exe";
                    break;
                default:
                    break;

            }

            //looks in CURRENT_USER:
            RegistryKey mainKey = Registry.CurrentUser;
            try
            {
                mainKey = mainKey.OpenSubKey(RegKey + "\\" + key, false);
                if (mainKey != null)
                {
                    vl = mainKey.GetValue(string.Empty).ToString();
                }
            }
            catch
            { }

            //if not found, looks inside LOCAL_MACHINE:
            mainKey = Registry.LocalMachine;
            if (string.IsNullOrEmpty(vl))
            {
                try
                {
                    mainKey = mainKey.OpenSubKey(RegKey + "\\" + key, false);
                    if (mainKey != null)
                    {
                        vl = mainKey.GetValue(string.Empty).ToString();
                    }
                }
                catch
                { }
            }

            //closing the handle:
            if (mainKey != null)
                mainKey.Close();

            return vl;
        }
        private static int GetMajorVersion(string path)
        {
            int vl = 0;
            if (File.Exists(path))
            {
                try
                {
                    FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(path);
                    vl = fileVersion.FileMajorPart;
                }
                catch
                { }
            }
            return vl;
        }
        private static int GetMinorVersion(string path)
        {
            int vl = 0;
            if (File.Exists(path))
            {
                try
                {
                    FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(path);
                    vl = fileVersion.FileMinorPart;
                }
                catch
                { }
            }
            return vl;
        }
        public static string returnMajorVersion(OfficeComponent comp)
        {
            string p = GetComponentPath(comp);
            string v = GetMajorVersion(p).ToString();
            return v;
        }
        public static string returnMinorVersion(OfficeComponent comp)
        {
            string p = GetComponentPath(comp);
            string v = GetMinorVersion(p).ToString();
            return v;
        }
        
    }
}
