using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Forms.Sampling
{
    public partial class frmSelectSamplesFromTable : Form
    {
        public frmSelectSamplesFromTable(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private viewUtility vUtil = null;
        private Dictionary<string, ITable> ftrDic = new Dictionary<string, ITable>();
        private IFields flds = null;
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIFeatureLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                    IFeatureClass ftrCls = ftrLyr.FeatureClass;
                    if (!ftrDic.ContainsKey(lyrNm))
                    {
                        ftrDic.Add(lyrNm, (ITable)ftrCls);
                        cmbSampleFeatureClass.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();
                }
            }
        }
        private void getFeaturePath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature Class";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!ftrDic.ContainsKey(outName))
                {
                    ftrDic.Add(outName, geoUtil.getTable(outPath));
                    cmbSampleFeatureClass.Items.Add(outName);
                }
                else
                {
                    ftrDic[outName] = geoUtil.getTable(outPath);
                }
                cmbSampleFeatureClass.SelectedItem = outName;
            }
            return;
        }
        


        private void btnOpenFeture_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void cmbSampleFeatureClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cmbTxt = cmbSampleFeatureClass.Text;
            if (cmbTxt == "" || cmbTxt == null)
            {
                return;
            }
            ITable ftrCls = ftrDic[cmbTxt];
            flds = ftrCls.Fields;
            cmbMap.Text = "";
            cmbMap.Items.Clear();
            for (int i = 0; i < flds.FieldCount; i++)
            {
                IField fld = flds.get_Field(i);
                string fldNm = fld.Name;
                esriFieldType fldType = fld.Type;
                if (fldType != esriFieldType.esriFieldTypeBlob && fldType != esriFieldType.esriFieldTypeDate && fldType != esriFieldType.esriFieldTypeGeometry && fldType != esriFieldType.esriFieldTypeGlobalID && fldType != esriFieldType.esriFieldTypeXML && fldType != esriFieldType.esriFieldTypeGUID && fldType != esriFieldType.esriFieldTypeOID && fldType != esriFieldType.esriFieldTypeRaster)
                {
                    cmbMap.Items.Add(fldNm);
                    
                }
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string smpFtrNm = cmbSampleFeatureClass.Text;
            string mapFld = cmbMap.Text;
            
            double alpha = System.Convert.ToDouble(nudAlpha.Value);
            double prop = System.Convert.ToDouble(nudProp.Value);
            string outModelPath = txtOutputPath.Text;
            if (outModelPath == null || outModelPath == "")
            {
                MessageBox.Show("You must select an output model or specify a numeric integer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (smpFtrNm == null || smpFtrNm == "")
            {
                MessageBox.Show("You must select a feature Class or Table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
            
            this.Visible = false;
            this.Refresh();
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Selecting samples this may take a while...");
            rp.stepPGBar(10);
            rp.Show();
            rp.Refresh();
            ITable ftrCls = ftrDic[smpFtrNm];
            DateTime dt1 = DateTime.Now;
            try
            {
                featureUtil ftrUtil = new featureUtil();
                int ns;
                if(Int32.TryParse(outModelPath,out ns))
                {
                    //MessageBox.Show(ns.ToString());
                    if (mapFld == null || mapFld == "" || ftrCls.FindField(mapFld) == -1)
                    {
                        ftrUtil.selectRandomFeaturesToSample(ftrCls, ns);
                    }
                    else
                    {
                        ftrUtil.selectEqualFeaturesToSample(ftrCls, mapFld, ns, chbEqual.Checked);
                    }
                }
                else
                {
                    esriUtil.Statistics.dataPrepBase.modelTypes mType = esriUtil.Statistics.ModelHelper.getModelType(outModelPath);
                    if (mapFld == null || mapFld == "")
                    {
                        if (mType == Statistics.dataPrepBase.modelTypes.Accuracy || mType == Statistics.dataPrepBase.modelTypes.Cluster)
                        {
                            MessageBox.Show("You must select a map field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    switch (mType)
                    {
                        case esriUtil.Statistics.dataPrepBase.modelTypes.Accuracy:
                            ftrUtil.selectAccuracyFeaturesToSample(ftrCls, outModelPath, mapFld, prop, alpha,chbEqual.Checked);
                            break;
                        case esriUtil.Statistics.dataPrepBase.modelTypes.Cluster:
                            ftrUtil.selectClusterFeaturesToSample(ftrCls, outModelPath, mapFld, prop, alpha,chbEqual.Checked);
                            break;
                        case Statistics.dataPrepBase.modelTypes.KS:
                            rp.Visible = false;
                            ITable sts = null;
                            if (MessageBox.Show("Do you want to supplement samples if needed?", "Supplement samples", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                sts = getTableFromUser();
                            }
                            rp.Visible = true;
                            ftrUtil.selectKSFeaturesToSample(ftrCls,sts, outModelPath, mapFld);
                            break;
                        case esriUtil.Statistics.dataPrepBase.modelTypes.CovCorr:
                            ftrUtil.selectCovCorrFeaturesToSample(ftrCls, outModelPath, prop, alpha);
                            break;
                        case esriUtil.Statistics.dataPrepBase.modelTypes.StrataCovCorr:
                            ftrUtil.selectClusterFeaturesToSample(ftrCls, outModelPath, mapFld, prop, alpha, chbEqual.Checked);
                            break;
                        case esriUtil.Statistics.dataPrepBase.modelTypes.PCA:
                            ftrUtil.selectPcaFeaturesToSample(ftrCls, outModelPath, prop, alpha);
                            break;
                        case esriUtil.Statistics.dataPrepBase.modelTypes.LinearRegression:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.MvlRegression:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.LogisticRegression:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.PLR:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.RandomForest:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.SoftMax:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.Cart:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.L3:
                        case esriUtil.Statistics.dataPrepBase.modelTypes.TTEST:
                        default:
                            rp.addMessage("Sample selection for this model type is not currently supported!");
                            break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt1);
                int hr = ts.Hours;
                int min = ts.Minutes;
                int sec = ts.Seconds;
                rp.stepPGBar(100);
                rp.addMessage("Finished selecting samples in " + hr.ToString() + " hours " + min.ToString() + " minutes " + sec.ToString() + " seconds");
                rp.enableClose();
                this.Close();
            }
        }

        private ITable getTableFromUser()
        {
            ITable outTbl = null;
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterTablesAndFeatureClassesClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Supplemental Table";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                outTbl = geoUtil.getTable(outPath);
            }
            return outTbl;
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog sd = new OpenFileDialog();
            sd.Filter = "Model|*.mdl";
            sd.DefaultExt = "mdl";
            sd.AddExtension = true;
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputPath.Text = sd.FileName;
            }
        }

        

    }
}
