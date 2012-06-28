using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    class regressionRaster
    {
        private rasterUtil rsUtil = new rasterUtil();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, Dictionary<string, double>> betasdic = null;
        /// <summary>
        /// A dictionary (key = dependent field names) of dictionary (key = parameter) estiamtes (value = double) created after the SasOutputFile is specfied.
        /// The SasOutput File must be a valid output csv output file.
        /// </summary>
        public Dictionary<string, Dictionary<string, double>> BetasDictionary { get { return betasdic; } }
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
        /// The name of the dependent field that has descrete values identifying each class for each record
        /// </summary>
        public string Dependentfield { get { return dependentfield; } set { dependentfield = value; } }
        private string[] independentfields = null;
        /// <summary>
        /// An array of Independent field names that might help predict the dependent field  
        /// </summary>
        public string[] IndependentFields { get { return independentfields; } set { independentfields = value; } }
        private Dictionary<string, Dictionary<string, double>> getBetas()
        {
            Dictionary<string, Dictionary<string, double>> cBetas = new Dictionary<string, Dictionary<string, double>>();
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
                Dictionary<string, double> betas = new Dictionary<string, double>();
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
                        if (betaVl != "" && betaVl != null && betaVl != ".")
                        {
                            //Console.WriteLine("\tadding parameter " + flVl + " = " + betaVl);
                            betas.Add(flVl,System.Convert.ToDouble(betaVl));
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
        /// An optional boolean property used to segment the data into a training and validation dataset (80%, 20%) defalt is false.
        /// </summary>
        public bool Validate { get { return validate; } set { validate = value; } }
        private string weightfield = null;
        /// <summary>
        /// An optianal string value that identifies a sample weighting value. Default is null
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
            IRaster2 inRs2 = (IRaster2)InRaster;
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(512, 512);
            IRasterCursor inRsCur = inRs2.CreateCursorEx(pntSize);
            int outBndCnt = Categories.Length;
            IRaster outRst = rsUtil.createNewRaster(InRaster, OutWorkspace, "REG", outBndCnt, rstPixelType.PT_FLOAT);
            IRasterEdit outRstE = (IRasterEdit)outRst;
            int inBndCnt = ((IRasterBandCollection)InRaster).Count;
            System.Array[] inBndArr = new System.Array[inBndCnt];
            System.Array[] outBndArr = new System.Array[outBndCnt];
            double[] xArr = new double[inBndCnt];
            while (inRsCur.Next() == true)
            {
                IPixelBlock3 inPb = (IPixelBlock3)inRsCur.PixelBlock;
                pntSize.SetCoords(inPb.Width, inPb.Height);
                IPixelBlock3 outPb = (IPixelBlock3)outRst.CreatePixelBlock(pntSize);
                #region set system arraies
                for (int i = 0; i < inBndCnt; i++)
                {
                    inBndArr[i] = (System.Array)inPb.get_PixelData(i);
                }
                for (int i = 0; i < outBndCnt; i++)
                {
                    outBndArr[i] = (System.Array)outPb.get_PixelData(i);
                }
                #endregion
                #region update pixel values in outBndArr
                for (int c = 0; c < inPb.Width; c++)
                {
                    for (int r = 0; r < inPb.Height; r++)
                    {

                        for (int i = 0; i < inBndCnt; i++)
                        {
                            double xArrVl = System.Convert.ToDouble(inBndArr[i].GetValue(c, r));
                            xArr[i] = xArrVl;
                        }

                        for (int i = 0; i < outBndCnt; i++)
                        {
                            double[] vls = betasdic[Categories[i]].Values.ToArray();
                            double sumReg = vls[0];
                            for (int j = 1; j < vls.Length; j++)
                            {
                                sumReg += vls[j] * xArr[j - 1];
                            }
                            try
                            {
                                if (Double.IsNaN(sumReg))
                                {
                                    sumReg = 0;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: vl = " + sumReg.ToString());
                                sumReg = 0;
                                Console.WriteLine(e.ToString());
                            }
                            outBndArr[i].SetValue(System.Convert.ToSingle(sumReg), c, r);
                        }
                    }
                }
                #endregion
                #region store pixel values in out raster
                for (int i = 0; i < outBndCnt; i++)
                {
                    outPb.set_PixelData(i, outBndArr[i]);
                }
                outRstE.Write(inRsCur.TopLeft, (IPixelBlock)outPb);
                #endregion
            }
            outRstE.Refresh();
            rsUtil.calcStatsAndHist(((IRasterBandCollection)outRst).Item(0).RasterDataset);
            return outRst;
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
                Console.WriteLine(path);
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
