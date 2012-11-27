using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    /// <summary>
    /// Performs a polytomous Logistic Regression Classification using SAS proc logistic procedure. SAS input, code, and output files are stored in
    /// a SAS folder nested within the workspace folder. To run the logistic procedure a workspace, dependent, independent, class, and sample location (weight, validate, optional)
    /// must be specified. Running the procedure creates an output file with parameter estimates that can be used to create 2 raster surfaces (prob and MLC).
    /// To create the probability and MLC surface an additionally input raster must be specified that has bands in the same order as significant variables.
    /// Significant variables can be read from the OutParameters property after the SAS procedure has been ran. This class has 2 main methods runSasProcedure and createModelRaster.
    /// runSasProcedure must be ran at least once prior to running createModelRaster. This class is intended to be ran in stages. First create the sasoutput and then build a input
    /// raster that has raster bands that represent the significant variables of the logistic procedure in the same order as the output parameters variables (not including the intercept).
    /// </summary>
    public class polytomousLogisticRaster
    {
        public polytomousLogisticRaster()
        {
            rsUtil = new rasterUtil();
        }
        public polytomousLogisticRaster(ref rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Dictionary<string, Dictionary<string, float>> betasdic = null;
        /// <summary>
        /// A dictionary (key = categories) of dictionary (key = parameter) estimates (value = double) created after the SasOutputFile is specified.
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
        /// a list of dependent categories in the order of the output file
        /// </summary>
        public string[] Categories
        {
            get
            {
                return categories;
            }
        }
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
        private IFeatureClass samplelocations = null;
        /// <summary>
        /// The location of the point file that has the sample location and the corresponding dependent and independent variables
        /// </summary>
        public IFeatureClass SampleLocations { get { return samplelocations; } set { samplelocations = value; } }
        private bool stepsel = true;
        public bool StepWiseSelection { get { return stepsel; } set { stepsel = value; } }
        private int sle = 10;
        private int sls = 5;
        public int SlEnter { get { return sle; } set { sle = value; } }
        public int SlStay { get { return sls; } set { sls = value; } }
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
        private string[] classfields = null;
        /// <summary>
        /// all fields that have discrete variables
        /// </summary>
        public string[] ClassFields { get { return classfields; } set { classfields = value; } }
        private Dictionary<string, Dictionary<string, float>> getBetas()
        {
            Dictionary<string, Dictionary<string, float>> cBetas = new Dictionary<string, Dictionary<string, float>>();
            Dictionary<string, float> betas = new Dictionary<string, float>();
            string[] oFlds = null;
            string[] bVls = null;
            using (System.IO.StreamReader sR = new System.IO.StreamReader(SasOutputFile))
            {
                string ln = sR.ReadLine();
                oFlds = ln.Split(new char[] { ',' });
                while (ln != null)
                {
                    bVls = ln.Split(new char[] { ',' });
                    ln = sR.ReadLine();
                }
            }
            for (int i = 0; i < oFlds.Length; i++)
            {
                string fld = oFlds.GetValue(i).ToString().Trim('"');
                string[] splFlds = fld.Split(new char[] { ':' });
                string param = splFlds[0].Trim();
                string category = splFlds[splFlds.GetUpperBound(0)].TrimStart(new char[] { ' ' });
                if (category.ToLower().StartsWith(dependentfield.ToLower().TrimEnd(new char[] { ' ' })))
                {
                    category = category.Split(new char[] { '=' })[1];
                    string vl = bVls[i];
                    if (vl != "" && vl != null && vl != ".")
                    {
                        try
                        {
                            float vld = System.Convert.ToSingle(vl);
                            if (cBetas.TryGetValue(category, out betas))
                            {
                            }
                            else
                            {
                                betas = new Dictionary<string, float>();
                            }
                            betas.Add(param, vld);
                            cBetas[category] = betas;

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
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
        /// the out workspace used to store all results
        /// </summary>
        public IWorkspace OutWorkspace { get; set; }
        /// <summary>
        /// The input raster who's raster bands are in the same order as the output parameter estimates.
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
        /// Runs the SAS logistic procedure must have a SampleLocations, OutWorkspace, DependentField, IndependentFields, ClassFields, and optionally Validate and Weight must be specified to run
        /// Running the procedure will create a series of files within the SAS output directory (Workspace path \\SASOUTPUT) These files include; prc.log, prc.sas, prc.lst, outest.csv, and another csv
        /// file of the sample data named after the sample location feature class. Prc.log is the SAS log file. Prc.lst is the output from the SAS procedure. Prc.sas is the SAS code to run the logistic
        /// procedure. Outest.csv is the parameter output file, and [SampleLocationName].csv is the input data used by the sas procedure. 
        /// </summary>
        public void runSasProcedure()
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.SubFields = dependentfield;
            IFeatureCursor fCur = samplelocations.Search(qry, false);
            int dIndex = fCur.FindField(dependentfield);
            IFeature fRow = fCur.NextFeature();
            List<string> sLst = new List<string>();
            while (fRow != null)
            {
                string vl = fRow.get_Value(dIndex).ToString();
                if (!sLst.Contains(vl)) sLst.Add(vl);
                fRow = fCur.NextFeature();
            }
            sasIntegration sInt = null;
            if (sLst.Count > 2)
            {
                sInt = new sasIntegration(SampleLocations, SasProcedure.GENERLIZED_LOGISTIC);
            }
            else
            {
                sInt = new sasIntegration(SampleLocations, SasProcedure.LOGISTIC);
            }
            sInt.SLS = (System.Convert.ToDouble(SlStay) / 100).ToString();
            sInt.SLE = (System.Convert.ToDouble(SlEnter) / 100).ToString();
            sInt.StepWiseSelection = StepWiseSelection;
            sInt.DependentField = Dependentfield;
            sInt.DescretFields = ClassFields;
            sInt.IndependentFields = IndependentFields;
            sInt.Validation = Validate;
            sInt.Weightfield = WeightField;
            SasOutputFile = sInt.RunProcedure();
            outRest = sInt.OutResultsPath;
            outLog = sInt.OutLogPath;
            outSas = sInt.OutSasPath;

        }
        /// <summary>
        /// Builds a probabilistic raster (PROB) of all the categories (1st raster is the baseline category) and a maximum likelihood raster (MLC) depicting the
        /// most probable raster band from the PROB raster. MCL raster also has the category names appended to the Raster's VAT. Required properties to run this
        /// procedure include; SASoutput file, InRater, OutWorkspace, and the DependentField Name.
        /// </summary>
        /// <returns>a IRaster Array with the first Raster being the probability raster and the second raster being the maximum likelihood raster</returns>
        public IRaster createModelRaster(IRaster seedRaster)
        {
            Dictionary<string,float[]> slopesDic = new Dictionary<string,float[]>();
            foreach (string s in Categories)
            {
                slopesDic.Add(s, betasdic[s].Values.ToArray());
            }

            return rsUtil.calcPolytomousLogisticRegressFunction(InRaster, slopesDic, seedRaster);

            #region oldcode
            //IRaster2 inRs2 = (IRaster2)InRaster;
            //IPnt pntSize = new PntClass();
            //pntSize.SetCoords(512, 512);
            //IRasterCursor inRsCur = inRs2.CreateCursorEx(pntSize);
            //int outBndCnt = Categories.Length + 1;
            //int betasCnt = betasdic.Count;
            //IRaster outRst = rsUtil.createNewRaster(InRaster, OutWorkspace, "PROB", outBndCnt, rstPixelType.PT_UCHAR);
            //IRaster outRstML = rsUtil.createNewRaster(InRaster, OutWorkspace, "MLC", 1, rstPixelType.PT_UCHAR);
            //IRasterEdit outRstE = (IRasterEdit)outRst;
            //IRasterEdit outRstMLE = (IRasterEdit)outRstML;
            //int inBndCnt = ((IRasterBandCollection)InRaster).Count;
            
            //System.Array[] inBndArr = new System.Array[inBndCnt];
            //System.Array[] outBndArr = new System.Array[outBndCnt];
            //double[] xArr = new double[inBndCnt];
            //double[] regArr = new double[betasCnt];
            ////int curCnt = 1;
            //while (inRsCur.Next() == true)
            //{
            //    //curCnt++;
            //    IPixelBlock3 inPb = (IPixelBlock3)inRsCur.PixelBlock;
            //    pntSize.SetCoords(inPb.Width, inPb.Height);
            //    IPixelBlock3 outPb = (IPixelBlock3)outRst.CreatePixelBlock(pntSize);
            //    IPixelBlock3 outPb2 = (IPixelBlock3)outRstML.CreatePixelBlock(pntSize);
            //    #region set system arraies
            //    System.Array mlArr = (System.Array)outPb2.get_PixelData(0);
            //    for (int i = 0; i < inBndCnt; i++)
            //    {
            //        inBndArr[i] = (System.Array)inPb.get_PixelData(i);
            //    }
            //    for (int i = 0; i < outBndCnt; i++)
            //    {
            //        outBndArr[i] = (System.Array)outPb.get_PixelData(i);
            //    }
            //    #endregion
            //    #region update pixel values in outBndArr
            //    for (int c = 0; c < inPb.Width; c++)
            //    {
            //        for (int r = 0; r < inPb.Height; r++)
            //        {

            //            for (int i = 0; i < inBndCnt; i++)
            //            {
            //                double xArrVl = System.Convert.ToDouble(inBndArr[i].GetValue(c, r));
            //                xArr[i] = xArrVl;
            //            }

            //            for (int i = 0; i < betasCnt; i++)
            //            {
            //                double[] vls = betasdic[Categories[i]].Values.ToArray();
            //                double sumReg = vls[0];
            //                for (int j = 1; j < vls.Length; j++)
            //                {
            //                    sumReg += vls[j] * xArr[j-1];
            //                }
            //                regArr[i] = Math.Exp(sumReg);
            //            }
            //            double sumExp1 = 1 + regArr.Sum();
            //            double sumProb = 0;
            //            double maxProbBnd = 0;
            //            double maxProb = 0;
            //            double vl = 0;
            //            byte vlbyte = 0;
            //            for (int i = 0; i < regArr.Length; i++)
            //            {
            //                vl = regArr[i] / sumExp1;
            //                try
            //                {
            //                    if (Double.IsNaN(vl))
            //                    {
            //                        vl = 0;
            //                    }
            //                    vlbyte = System.Convert.ToByte(System.Math.Round(vl * 100));
            //                }
            //                catch (Exception e)
            //                {
            //                    Console.WriteLine("Error: vl = " + vl.ToString());
            //                    vl = 0;
            //                    vlbyte = System.Convert.ToByte(0);
            //                    Console.WriteLine(e.ToString());
            //                }
            //                int otBnd = i + 1;
            //                outBndArr[otBnd].SetValue(vlbyte, c, r);
            //                sumProb += vl;
            //                if (vl > maxProb)
            //                {
            //                    maxProb = vl;
            //                    maxProbBnd = otBnd;
            //                }
            //            }
            //            vl = 1 - sumProb;
            //            vlbyte = System.Convert.ToByte(System.Math.Round(vl * 100));
            //            if (vl > maxProb)
            //            {
            //                maxProbBnd = 0;
            //            }
            //            outBndArr[0].SetValue(vlbyte, c, r);
            //            mlArr.SetValue(System.Convert.ToByte(maxProbBnd+1), c, r);
            //        }
            //    }
            //    #endregion
            //    #region store pixel values in out raster
            //    for (int i = 0; i < outBndCnt; i++)
            //    {
            //        outPb.set_PixelData(i, outBndArr[i]);
            //    }
            //    outRstE.Write(inRsCur.TopLeft, (IPixelBlock)outPb);
            //    outPb2.set_PixelData(0, mlArr);
            //    outRstMLE.Write(inRsCur.TopLeft, (IPixelBlock)outPb2);
            //    #endregion
            //}
            //outRstE.Refresh();
            //outRstMLE.Refresh();
            //rsUtil.calcStatsAndHist(((IRasterBandCollection)outRst).Item(0).RasterDataset);
            //IRasterDataset mlDset = ((IRasterBandCollection)outRstML).Item(0).RasterDataset;
            //rsUtil.calcStatsAndHist(mlDset);
            //#region update MLC attribute table
            //IRasterDatasetEdit2 mlDsetE = (IRasterDatasetEdit2)((IRasterDataset2)mlDset);
            //mlDsetE.BuildAttributeTable();
            //ITable rTbl = ((IRasterBandCollection)mlDset).Item(0).AttributeTable;
            //IField fld = new FieldClass();
            //IFieldEdit fldE = (IFieldEdit)fld;
            //fldE.Name_2 = Dependentfield;
            //fldE.Type_2 = esriFieldType.esriFieldTypeString;
            //rTbl.AddField(fld);
            //ICursor scur = rTbl.Search(null,false);
            //int vlIndex = scur.FindField("Value");
            //int uIndex = scur.FindField(dependentfield);
            //IRow srow = scur.NextRow();
            //while(srow!=null)
            //{
            //    int vl = System.Convert.ToInt32(srow.get_Value(vlIndex));
            //    if (vl > 1)
            //    {
            //        srow.set_Value(uIndex, Categories[vl - 2]);
            //    }
            //    else
            //    {
            //        srow.set_Value(uIndex, "BASE");
            //    }
            //    srow.Store();
            //    srow= scur.NextRow();
            //}
            //#endregion
            //return new IRaster[]{outRst,outRstML};
            #endregion
        }

        public void showModelOutput()
        {
            viewInNotePad(outRest);
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
