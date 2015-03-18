using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace RMRSBatchProcess
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new RMRSBatchProcess.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[]{ esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced},new esriLicenseExtensionCode[] { });
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            if (args.Length < 1)
            {
                SelectBathJobs sbj = new SelectBathJobs();
                System.Windows.Forms.Application.Run(sbj);
            }
            else
            {
                foreach (string s in args)
                {
                    esriUtil.batchCalculations btc = new esriUtil.batchCalculations();
                    btc.BatchPath = s;
                    btc.loadBatchFile();
                    btc.runBatch();
                }
            }
            m_AOLicenseInitializer.ShutdownApplication();
            Environment.Exit(0);
        }
        
    }
}
