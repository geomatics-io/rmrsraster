using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    public class accuracyAssessment
    {
        private rasterUtil rsUtil = new rasterUtil();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string sasoutputfile = null;
        /// <summary>
        /// The location of the SAS output file containing parameter estimates.
        /// </summary>
        private double alpha = 0.05;
        public double Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }
        public string SasOutputFile 
        { 
            get 
            { 
                return sasoutputfile; 
            } 
            set 
            { 
                sasoutputfile = value;
            } 
        }
        private IFeatureClass samplelocations = null;
        /// <summary>
        /// The location of the point file that has the sample location and the corresponding dependent and independent variables
        /// </summary>
        public IFeatureClass SampleLocations 
        { 
            get 
            { 
                return samplelocations; 
            } 
            set 
            { 
                samplelocations = value;
                IFields flds = samplelocations.Fields;
                List<string> fldsLst = new List<string>();
                for (int i = 0; i < flds.FieldCount; i++)
                {
                    IField fld = flds.get_Field(i);
                    if (fld.Type == esriFieldType.esriFieldTypeString) fldsLst.Add(fld.Name);
                }
                ClassFields = fldsLst.ToArray();
            } 
        }
        private string[] classfields = null;
        /// <summary>
        /// all fields that have discrete variables
        /// </summary>
        public string[] ClassFields { get { return classfields; } set { classfields = value; } }       
        /// <summary>
        /// The input raster or feature class' ITable that represents the classification.
        /// </summary>
        private ITable intable = null;
        public ITable InTable { get { return intable; } set { intable = value; } }
        private string weightfield = null;
        /// <summary>
        /// An optional string value that identifies a sample weighting value. Default is null
        /// </summary>
        public string WeightField { get { return weightfield; } set { weightfield = value; } }
        private string outRest, outLog, outSas;
        private bool exact = false;
        public bool Exact { get { return exact; } set { exact = value; } }
        /// <summary>
        /// Runs the SAS logistic procedure must have a SampleLocations, OutWorkspace, DependentField, IndependentFields, ClassFields, and optionally Validate and Weight must be specified to run
        /// Running the procedure will create a series of files within the SAS output directory (Workspace path \\SASOUTPUT) These files include; prc.log, prc.sas, prc.lst, outest.csv, and another csv
        /// file of the sample data named after the sample location feature class. Prc.log is the SAS log file. Prc.lst is the output from the SAS procedure. Prc.sas is the SAS code to run the logistic
        /// procedure. Outest.csv is the parameter output file, and [SampleLocationName].csv is the input data used by the sas procedure. 
        /// </summary>
        public void runSasProcedure()
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.SubFields = MapField;
            IFeatureCursor fCur = samplelocations.Search(qry, false);
            int dIndex = fCur.FindField(MapField);
            IFeature fRow = fCur.NextFeature();
            List<string> sLst = new List<string>();
            while (fRow != null)
            {
                string vl = fRow.get_Value(dIndex).ToString();
                if (!sLst.Contains(vl)) sLst.Add(vl);
                fRow = fCur.NextFeature();
            }
            sasIntegration sInt = new sasIntegration(SampleLocations, SasProcedure.ACCURACYASSESSMENT);
            sInt.Alpha = Alpha;
            sInt.DependentField = MapField;
            sInt.DescretFields = ClassFields;
            sInt.IndependentFields = new string[] {ReferenceField};
            sInt.Weightfield = WeightField;
            sInt.Validation = false;
            sInt.StepWiseSelection = Exact;
            SasOutputFile = sInt.RunProcedure();
            outRest = sInt.OutResultsPath;
            outLog = sInt.OutLogPath;
            outSas = sInt.OutSasPath;
            if (InTable != null)
            {
                addConfidenceIntervals();
            }

        }
        /// <summary>
        /// Adds Probabilities confidence intervals to the map given the plr model
        /// </summary>
        
        public void addConfidenceIntervals()
        {
            IEnumerable<string> hdArr = null;
            Dictionary<string,IEnumerable<string>> vlDic = new Dictionary<string,IEnumerable<string>>();
            using(System.IO.StreamReader sr = new System.IO.StreamReader(SasOutputFile))
            {
                string ln = sr.ReadLine();
                hdArr = ln.Split(new char[]{','}).Skip(2);
                foreach(string s in hdArr)
                {
                    if (InTable.FindField(s) == -1)
                    {
                        geoUtil.createField(InTable, s, esriFieldType.esriFieldTypeSingle);
                    }
                }
                while((ln=sr.ReadLine())!=null)
                {
                    string[] vlArr = ln.Split(new char[]{','});
                    string mpCls = vlArr[0];
                    IEnumerable<string> enumVls = vlArr.Skip(2);
                    vlDic[mpCls]=enumVls;
                }
                sr.Close();
            }
            IDataset dSet = (IDataset)InTable;
            IWorkspace wks = dSet.Workspace;
            IWorkspaceEdit wksE = (IWorkspaceEdit)wks;
            bool weStart = true;
            if (wksE.IsBeingEdited())
            {
                weStart = false;
            }
            else
            {
                wksE.StartEditing(false);
            }
            wksE.StartEditOperation();
            ICursor uCur = InTable.Update(null, false);
            int valIndex = uCur.FindField(MapField);
            IRow uRow = uCur.NextRow();
            while (uRow != null)
            {
                string mpVl = uRow.get_Value(valIndex).ToString();
                IEnumerable<string> vls;
                if(vlDic.TryGetValue(mpVl,out vls))
                {
                    for (int i = 0; i < hdArr.Count(); i++)
                    {
                        string fldNm = hdArr.ElementAt(i);
                        float fldVl = System.Convert.ToSingle(vls.ElementAt(i));
                        int fldIndex = uCur.FindField(fldNm);
                        uRow.set_Value(fldIndex, fldVl);
                    }
                    uCur.UpdateRow(uRow);
                }
                uRow = uCur.NextRow();
            }
            wksE.StopEditOperation();
            if (weStart) wksE.StopEditing(true); 
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
                    System.Windows.Forms.MessageBox.Show("Could not find SAS output. Opening log instead." + path);
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

        public string MapField { get; set; }

        public string ReferenceField { get; set; }

    }
}
