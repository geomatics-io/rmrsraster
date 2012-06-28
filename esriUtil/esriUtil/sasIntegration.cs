using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    /// <summary>
    /// Sas Procedures to choose from enumerator
    /// </summary>
    public enum SasProcedure { GENERLIZED_LOGISTIC, LOGISTIC, REGRESSION, PCA, ANOVA, CORRELATION, MEANS, CLUSTERING, DISCRIMINANT, FREQ, MDS, MCMC, ACCURACYASSESSMENT };
    /// <summary>
    /// Creates a sasIntegration object that will run one of the selected SAS procedures.
    /// SAS programming and outputs are stored in the OutDirectory specified.
    /// The return values of the RunProcedure function is a path to an output parameter file which can be used
    /// as an input to a raster building model.
    /// </summary>
    public class sasIntegration
    {
        /// <summary>
        /// Constructor for class. Requires a featureClass and a string pointing to an out directory. By default the generalized logistic method is specified
        /// </summary>
        /// <param name="inFtr">Input featureClass</param>
        /// <param name="procedure">The procedure to run</param>
        public sasIntegration(IFeatureClass inFtr,SasProcedure procedure)
        {
            infeatureclass = inFtr;
            p = procedure;
            o = ((IDataset)inFtr).Workspace.PathName + "\\SASOUTPUT\\" + p.ToString();
            if (System.IO.Directory.Exists(o))
            {
                System.Windows.Forms.DialogResult rslt = System.Windows.Forms.MessageBox.Show(p.ToString() + " folder already exists. Do you want to rename the existing " + p.ToString()+" outputs (If you select no existing outputs will be lost)?", "Existing Directory", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (rslt == System.Windows.Forms.DialogResult.Yes)
                {
                    string newOutName = getNewName(o);
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(o);
                    dInfo.MoveTo(newOutName);
                    updateOldSasFile(newOutName);
                }
            }
            geoUtil.check_dir(o);
            if (!System.IO.File.Exists(sasexe))
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.DefaultExt = ".exe";
                ofd.Filter = "SAS Executable|*.exe";
                ofd.Title = "Can't find SAS exe!";
                ofd.Multiselect = false;
                System.Windows.Forms.DialogResult rslt = ofd.ShowDialog();
                if (rslt == System.Windows.Forms.DialogResult.OK)
                {
                    sasexe = ofd.FileName;
                    esriUtil.Properties.Settings.Default.SASEXE = sasexe;
                    esriUtil.Properties.Settings.Default.Save();
                }
                else
                {
                    sasexe = null;
                }
            }
            prStInfo = pr.StartInfo;
            prStInfo.CreateNoWindow = true;
            prStInfo.UseShellExecute = false;
            prStInfo.FileName = sasexe;
            prStInfo.Arguments = "-sysin \"" + OutSasPath + "\" -log \"" + OutDirectory + "\" -print \"" + OutDirectory + "\"";
        }

        private void updateOldSasFile(string newOutName)
        {
            string n = System.IO.Path.GetFileName(newOutName);
            string nm = newOutName + "\\prc.sas";
            List<string> lnsLst = new List<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(nm))
            {
                string ln = "";
                while ((ln = sr.ReadLine()) != null)
                {
                    ln.Replace("SASOUTPUT\\ACCURACYASSESSMENT", "SASOUTPUT\\" + n);
                    lnsLst.Add(ln);
                }
                sr.Close();
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(nm))
            {
                foreach (string s in lnsLst)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }
        }

        private string getNewName(string originalName)
        {
            string outPath = originalName;
            Forms.MapServices.frmInputBox inb = new Forms.MapServices.frmInputBox();
            inb.TopLevel = true;
            inb.Label = "Please type in a new name for the " + p.ToString() + " Directory";
            inb.Text = "New Output Name";
            System.Windows.Forms.DialogResult rstl = inb.ShowDialog();
            if (rstl == System.Windows.Forms.DialogResult.OK)
            {
                outPath = System.IO.Path.GetDirectoryName(originalName) + "\\" + inb.ArcGISConnection;
                outPath = checkNewName(outPath);
            }
            inb.Dispose();
            return outPath;


        }

        private string checkNewName(string outPath)
        {
            string newName = outPath;
            if (System.IO.Directory.Exists(newName))
            {
                string dir = System.IO.Path.GetDirectoryName(outPath);
                string nm = System.IO.Path.GetFileName(outPath);
                newName = "_" + nm;
                newName = checkNewName(newName);
            }
            return newName;
        }
        /// <summary>
        /// Runs the specified sas procedure and returns a string pointing to the procedure estimates.
        /// </summary>
        /// <returns>SAS estimates output file (CSV)</returns>
        public string RunProcedure()
        {
            try
            {
                if (sasexe == null || OutDirectory == null || InFeatureClass == null || DependentField == "" || IndependentFields == null)
                {
                    Console.WriteLine("Can't find SAS.exe, Outdirectory has not been specified, InFeatureClass has not been specfied, Dependent, or IndependentFields have not been specfied");
                    return null;
                }
                writeSasFile();
                pr.Start();
                pr.WaitForExit();
                //Console.WriteLine(outputDataFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            outputDataFile = OutEstimatesPath;
            return outputDataFile;
        }
        private bool stepwiseselection = true;
        public bool StepWiseSelection { get { return stepwiseselection; } set { stepwiseselection = value; } }
        private void writeSasFile()
        {
            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(o);
            geoUtil.deleteFiles(dInfo);
            writeSasImport();
            writeSasProcedure();
            writeSasExport();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(OutSasPath))
            {
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
        }
        private string sle = "0.15";
        private string sls = "0.05";
        public string SLE { get { return sle; } set { sle = value; } }
        public string SLS { get { return sls; } set { sls = value; } }
        private void writeSasImport()
        {
            string inputDataFile = geoUtil.exportTableToTxt((ITable)InFeatureClass,o);
            sb.Append("PROC IMPORT OUT= WORK.Sampledata DATAFILE= \"" + inputDataFile + "\" DBMS=CSV REPLACE;\nGETNAMES=YES;\nDATAROW=2;\nRUN;\n");
            if (validation == true)
            {
                sb.Append("data Work.Sampledata;\nset Work.Sampledata;\nlength random 8;\nrandom = ranuni(123);\nrun;\n");
                
            }
            
        }
        private void writeSasProcedure()
        {
            string inflds = "";
            string clflds = "";
            inflds = String.Join(" ", independentFields);
            //Console.WriteLine(inflds.Length);
            if (inflds.Length >= 100)
            {
                for (int i = 100; i < inflds.Length; i += 100)
                {
                    int i2 = inflds.IndexOf(" ", i);
                    inflds = inflds.Insert(i2, "\n");
                    //Console.WriteLine("inserting n at " + i.ToString());
                }
            }
            if (descretfields != null)
            {
                clflds = String.Join(" ", descretfields);
                if (clflds.Length >= 100)
                {
                    for (int i = 100; i < clflds.Length; i += 100)
                    {
                        int i2 = inflds.IndexOf(" ", i);
                        clflds = clflds.Insert(i, "\n");
                    }
                }
            }
            //Console.WriteLine(inflds);
            switch (Procedure)
            {
                case SasProcedure.LOGISTIC:
                    useLabel = "LABEL";
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\"\n;ods graphics on;\n");
                    sb.Append("proc logistic data=Work.Sampledata outest=outEstimates plots=effects;\nclass " + clflds + ";\nmodel " + dependentField + " = \n" + inflds);
                    sb.Append("\n/link=logit lackfit rsquare clparm=both clodds=both");
                    if (StepWiseSelection)
                    {
                        sb.Append(" selection=stepwise slentry=" + SLE + " slstay=" + SLS + ";");
                    }
                    else
                    {
                        sb.Append(";");
                    }
                    if(weightfield!=null)
                    {
                        sb.Append("\nweight " + weightfield + " / norm;");
                    }
                    if(Validation)
                    {
                        sb.Append("\nwhere random >= .2;");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.REGRESSION:
                    useLabel = "LABEL";
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    sb.Append("\nproc reg data=Work.Sampledata outest=outEstimates plots=ResidualByPredicted;");
                    sb.Append("\nmodel " + dependentField + " = \n" + inflds);
                    sb.Append("\n/ rsquare aic sp cp vif");
                    if (StepWiseSelection)
                    {
                        sb.Append(" selection=stepwise slentry=" + SLE + " slstay=" + SLS + ";");
                    }
                    else
                    {
                        sb.Append(";");
                    }
                    if (dependentField.Split(new char[] { ' ' }).Length > 1)
                    {
                        sb.Append("\nmtest / details print;");
                    }
                    if(weightfield!=null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    if(Validation)
                    {
                        sb.Append("\nwhere random >= .2;");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.PCA:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    sb.Append("proc princomp data=Work.Sampledata outstats=outEstimates cov;\nvar " + inflds + ";");
                    if (descretfields != null) sb.Append("\nby " + clflds +";");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + " / norm;");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.ANOVA:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    List<string> infldLst = new List<string>();
                    foreach (string s in descretfields)
                    {
                        if (independentFields.Contains(s)) infldLst.Add(s);
                    }
                    inflds = String.Join(" ", infldLst.ToArray());
                    if (inflds.Length > 256)
                    {
                        inflds.Insert(256, "\n");
                    } 
                    sb.Append("proc anova data=Work.Sampledata outstats=outEstimates ;\nclass " + clflds + ";\nmodel " + dependentField + " = " + inflds + ";");
                    sb.Append("\nmeans " +inflds + "/ bon;");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + " / norm;");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.CORRELATION:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    sb.Append("proc corr data=Work.Sampledata out=outEstimates plots=all;\nvar " + inflds + ";");
                    if (descretfields != null) sb.Append("\nby " + clflds +";");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.MEANS:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    sb.Append("proc means data=Work.Sampledata max mean min mode n stderr sum var;\nvar " + inflds + ";");
                    if (descretfields != null) sb.Append("\nby " + clflds +";");
                    sb.Append("\nout=outEstimates max mean min mode n stderr sum var;");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.FREQ:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;");
                    sb.Append("proc freq data=Work.Sampledata order=data;");
                    sb.Append("tables " + dependentField + "*" + independentFields[0] + " / all agree plots=all out=outEstimates;");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    sb.Append("\nrun;\n");
                    break;
                case SasProcedure.ACCURACYASSESSMENT:
                    useLabel = "";
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;\n");
                    sb.Append("proc freq data=Work.Sampledata;");
                    sb.Append("tables " + dependentField + "*" + independentFields[0] + " / outpct outexpect all agree chisq measures plots=all out=outEstimates alpha=" + Alpha.ToString() + ";\n");
                    if (stepwiseselection)
                    {
                        sb.Append("exact kappa fisher pchi lrchi;");
                    }
                    else
                    {
                        sb.Append("test kappa;");
                    }
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    sb.Append("\nrun;");
                    sb.Append("\nproc means data=Work.Sampledata n noprint;");
                    sb.Append("\nclass " + dependentField + ";");
                    sb.Append("\noutput out=SampleSub n=Count;");
                    sb.Append("\nrun;quit;");
                    sb.Append("\ndata Work.SampleSub;\nset Work.SampleSub;");
                    sb.Append("\nif _TYPE_ = 0 then delete;");
                    sb.Append("\ndrop _TYPE_ _FREQ_ Count;");
                    sb.Append("\nrun;quit;");
                    sb.Append("\nproc logistic data=Work.Sampledata;\nclass " + clflds + ";\nmodel " + inflds + " = " + dependentField + "\n/link=glogit rsquare clparm=both clodds=both alpha=" + Alpha.ToString()+ ";");
                    sb.Append("\nscore data=Work.SampleSub out=outEstimates clm fitstat alpha=" + Alpha.ToString()+";");
                    if (weightfield != null)
                    {
                        sb.Append("\nweight " + weightfield + ";");
                    }
                    sb.Append("\nrun;quit;\n"); 
                    break;
                case SasProcedure.CLUSTERING:
                case SasProcedure.DISCRIMINANT:
                case SasProcedure.MCMC:
                case SasProcedure.MDS:
                default:
                    sb.Append("ods listing gpath=\"" + OutDirectory + "\";\nods graphics on;");
                    sb.Append("proc logistic data=Work.Sampledata outest=outEstimates plots=effects;\nclass " + clflds + ";\nmodel " + dependentField + " = \n" + inflds);
                    sb.Append("\n/link=glogit rsquare clparm=both clodds=both");
                    if (StepWiseSelection)
                    {
                        sb.Append(" selection=stepwise slentry=" + SLE + " slstay=" + SLS + ";");
                    }
                    else
                    {
                        sb.Append(";");
                    }
                    if(weightfield!=null)
                    {
                        sb.Append("\nweight " + weightfield + " / norm;");
                    }
                    if(Validation)
                    {
                        sb.Append("\nwhere random >= .2;");
                    }
                    sb.Append("\nrun;\n");
                    break;
            }
        }
        private string useLabel = "LABEL";
        private void writeSasExport()
        {
            sb.Append("PROC EXPORT DATA = WORK.outEstimates OUTFILE = \"" + OutEstimatesPath + "\" DBMS=CSV " + useLabel + " REPLACE;\nPUTNAMES=YES;\nRUN;quit;\n");
        }
        private string outputDataFile = "";
        private bool validation = false;
        private double alpha = 0.05;
        public double Alpha { get { return alpha; } set { alpha = value; } }
        private string[] descretfields = null;
        public string[] DescretFields { get { return descretfields; } set { descretfields = value; } }
        /// <summary>
        /// boolean value used to determine if a training and validation set are going to be made. Training sets randomly choose 80% of the data to build the model and validate the model based on the remaining 20%
        /// </summary>
        public bool Validation { get { return validation; } set { validation = value; } }
        private System.Diagnostics.Process pr = new System.Diagnostics.Process();
        private System.Diagnostics.ProcessStartInfo prStInfo = null;
        private string sasexe = esriUtil.Properties.Settings.Default.SASEXE;
        private string o = null;
        /// <summary>
        /// the output directory where all files sas files will be created
        /// </summary>
        public string OutDirectory
        {
            get
            {
               return o;
            }
        }
        private IFeatureClass infeatureclass=null;
        /// <summary>
        /// the feature class that has the sampled values
        /// </summary>
        public IFeatureClass InFeatureClass
        {
            get
            {
                return infeatureclass;
            }
        }
        private SasProcedure p = SasProcedure.GENERLIZED_LOGISTIC;
        /// <summary>
        /// the sas procedure to run
        /// </summary>
        public SasProcedure Procedure { get { return p; }}
        private StringBuilder sb = new StringBuilder();
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private string dependentField = "";
        /// <summary>
        /// the string name of the dependent (y) field variable
        /// </summary>
        public string DependentField
        {
            get
            {
                return dependentField;
            }
            set
            {
                dependentField = value;
            }
        }
        private string[] independentFields = null;
        /// <summary>
        /// the string array of independent (x) field names
        /// </summary>
        public string[] IndependentFields { get { return independentFields; } set { independentFields = value; } }
        private string weightfield = null;
        /// <summary>
        /// the field name of the weighting field if there is a weighting field
        /// </summary>
        public string Weightfield{ get {return weightfield; } set {weightfield = value; }}
        public string OutLogPath { get { return o + "\\prc.log"; } }
        public string OutResultsPath { get { return o + "\\prc.lst"; } }
        public string OutSasPath { get {return o + "\\prc.sas";}}
        public string OutEstimatesPath { get { return o + "\\outest.csv"; } }
        public string SASExeLoction { get { return esriUtil.Properties.Settings.Default.SASEXE; } }
    }
}
