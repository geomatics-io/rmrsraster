using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    public class regressionRaster
    {
        public regressionRaster()
        {
            rsUtil = new rasterUtil();
        }
        public regressionRaster(ref rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = new rasterUtil();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, Dictionary<string, float>> betasdic = null;
        /// <summary>
        /// A dictionary (key = dependent field names) of dictionary (key = parameter) estiamtes (value = double) created after the SasOutputFile is specfied.
        /// The SasOutput File must be a valid output csv output file.
        /// </summary>
        public Dictionary<string, Dictionary<string, float>> BetasDictionary { get { return betasdic; } }
        private string[] outparam = null;
        /// <summary>
        /// a list of significant independent variables in the order of the output file
        /// </summary>
        public string[] OutParameters
        {
            get
            {
                return outparam;
            }
        }
        private string[] categories = null;
        /// <summary>
        /// a list of dependent Fields in te order of the output file
        /// </summary>
        public string[] Categories
        {
            get
            {
                return categories;
            }
        }
        private bool stepwise = true;
        public bool StepWiseSelection { get { return stepwise; } set { stepwise = value; } }
        private string sasoutputfile = null;
        /// <summary>
        /// The location of the SAS output file containing parameter estimates.
        /// </summary>
        public string SasOutputFile
        {
            get
            {
                return sasoutputfile;
            }
            set
            {
                sasoutputfile = value;
                if (System.IO.File.Exists(sasoutputfile))
                {
                    betasdic = getBetas();
                    categories = getCategories();
                    outparam = getParameters();
                }
            }
        }
        private int sle = 10;
        private int sls = 5;
        public int SlEnter { get { return sle; } set { sle = value; } }
        public int SlStay { get { return sls; } set { sls = value; } }
        private string sasoutdir = "";
        public string SasOutputDirectory { get { return sasoutdir; } } 
        private IFeatureClass samplelocations = null;
        /// <summary>
        /// The location of the point file that has the sample location and the corresponding dependent and independent variables
        /// </summary>
        public IFeatureClass SampleLocations { get { return samplelocations; } set { samplelocations = value; } }
        private string dependentfield = null;
        /// <summary>
        /// The name of the dependent field that has discrete values identifying each class for each record
        /// </summary>
        public string Dependentfield { get { return dependentfield; } set { dependentfield = value; } }
        private string[] independentfields = null;
        /// <summary>
        /// An array of Independent field names that might help predict the dependent field  
        /// </summary>
        public string[] IndependentFields { get { return independentfields; } set { independentfields = value; } }
        private Dictionary<string, Dictionary<string, float>> getBetas()
        {
            Dictionary<string, Dictionary<string, float>> cBetas = new Dictionary<string, Dictionary<string, float>>();
            string[] oFlds = null;
            List<string> bVls = new List<string>();
            string[] depSp = dependentfield.Split(new char[]{' '});
            using (System.IO.StreamReader sR = new System.IO.StreamReader(SasOutputFile))
            {
                string ln = sR.ReadLine();
                oFlds = ln.Split(new char[] { ',' });
                ln = sR.ReadLine();
                while (ln != null)
                {
                    //Console.WriteLine(ln);
                    bVls.Add(ln);
                    ln = sR.ReadLine();
                }
            }
            for (int i = 0; i < bVls.Count; i++)
            {
                string[] bVlsSp = bVls[i].Split(new char[]{','});
                string cls = bVlsSp[2];
                bool getVls = false;
                Dictionary<string, float> betas = new Dictionary<string, float>();
                for (int j = 0; j < oFlds.Length; j++)
                {
                    string flVl = oFlds[j].Replace("\"","");
                    //Console.WriteLine("~" + flVl + "~");
                    if(flVl.ToLower()=="intercept")
                    {
                        //Console.WriteLine("Setting getVls to True");
                        getVls = true;
                    }
                    //need to work on this part lookup to see if fld name is within dependent name
                    if (depSp.Contains(flVl))
                    {
                        //Console.WriteLine("Setting getVls to false");
                        getVls = false;
                        break;
                    }
                    if (getVls==true)
                    {
                        string betaVl = bVlsSp[j].Replace("\"","");
                        //Console.WriteLine(betaVl.ToString());
                        if (betaVl != "" && betaVl != null && betaVl != ".")
                        {
                            //Console.WriteLine("\tadding parameter " + flVl + " = " + betaVl);
                            
                            betas.Add(flVl,System.Convert.ToSingle(betaVl));
                        }
                    }

                }
                cBetas.Add(cls, betas);
                //Console.WriteLine("adding dependent variable " + cls);
            }
            return cBetas;
        }
        private string[] getParameters()
        {
            return betasdic[Categories[0]].Keys.ToArray();
        }
        private string[] getCategories()
        {
            return betasdic.Keys.ToArray();
        }
        /// <summary>
        /// the out workspace used to store raster results
        /// </summary>
        public IWorkspace OutWorkspace { get; set; }
        /// <summary>
        /// The input raster. The raster bands must be in the same order as the output parameter estimates.
        /// </summary>
        public IRaster InRaster { get; set; }
        private bool validate = false;
        /// <summary>
        /// An optional Boolean property used to segment the data into a training and validation dataset (80%, 20%) defalt is false.
        /// </summary>
        public bool Validate { get { return validate; } set { validate = value; } }
        private string weightfield = null;
        /// <summary>
        /// An optional string value that identifies a sample weighting value. Default is null
        /// </summary>
        public string WeightField { get { return weightfield; } set { weightfield = value; } }
        private string outRest, outLog, outSas;
        /// <summary>
        /// Runs the SAS Regression procedure must have a SampleLocations, OutWorkspace, DependentField, IndependentFields, and optinally Validate and Weight must be specified to run
        /// Running the procedure will create a series of files within the SAS output directory (Workspace path \\SASOUTPUT\Regression) These files include; prc.log, prc.sas, prc.lst, outest.csv, and another csv
        /// file of the sample data named after the sample location feature class. Prc.log is the SAS log file. Prc.lst is the output from the SAS procedure. Prc.sas is the SAS code used to run the regression 
        /// procedure. Outest.csv is the parameter output file, and [SampleLocationName].csv is the input data used by the sas procedure. 
        /// </summary>
        public void runSasProcedure()
        {
            sasIntegration sInt = new sasIntegration(SampleLocations, SasProcedure.REGRESSION);
            sInt.DependentField = Dependentfield;
            sInt.IndependentFields = IndependentFields;
            sInt.Validation = Validate;
            sInt.Weightfield = WeightField;
            sInt.StepWiseSelection = StepWiseSelection;
            sInt.SLE = (System.Convert.ToDouble(SlEnter)/100).ToString();
            sInt.SLS = (System.Convert.ToDouble(SlStay) / 100).ToString();
            SasOutputFile = sInt.RunProcedure();
            outRest = sInt.OutResultsPath;
            outLog = sInt.OutLogPath;
            outSas = sInt.OutSasPath;
            sasoutdir = sInt.OutDirectory;

        }
        public IRaster createModelRaster()
        {
            List<float[]> slopesLst = new List<float[]>();
            foreach(string s in Categories)
            {
                List<float> slp = new List<float>();
                Dictionary<string, float> dic = betasdic[s];
                foreach (KeyValuePair<string, float> d in dic)
                {
                    slp.Add(d.Value);
                }
                slopesLst.Add(slp.ToArray());
            }

            return rsUtil.calcRegressFunction(InRaster, slopesLst);
        }

        public void showModelOutput()
        {
            viewInNotePad(outRest);
            viewGraphs();
        }

        public void editSasFile()
        {
            editInSas(outSas);
        }
        public void showSasLog()
        {
            viewInNotePad(outLog);
        }
        public void showEstimates()
        {
            viewInNotePad(SasOutputFile);
        }
        private void viewInNotePad(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = "notepad.exe";
                    prc.StartInfo.Arguments = path;
                    prc.Start();
                    prc.WaitForExit();
                }
                else
                {
                    path = path.Substring(0, path.LastIndexOf("\\")) + "\\prc.log";
                    System.Windows.Forms.MessageBox.Show("Could not find SAS output. Openning log instead." + path);
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = "notepad.exe";
                    prc.StartInfo.Arguments = path;
                    prc.Start();
                    prc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void viewGraphs()
        {
            try
            {
                System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(sasoutdir);
                string path = "";
                foreach (System.IO.FileInfo fInfo in dInfo.GetFiles("*.png"))
                {
                    path = fInfo.FullName;
                    break;
                }
                if (System.IO.File.Exists(path))
                {
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = path;
                    prc.Start();
                    prc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void editInSas(string path)
        {
            try
            {
                //Console.WriteLine(path);
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = path;
                //prc.StartInfo.Arguments = "'" + path + "' FILEOPEN";// "FILEOPEN";// '" + path + "'";
                prc.Start();
                prc.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
