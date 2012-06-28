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
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Forms.Texture
{
    public partial class frmCreateGlcmSurface : Form
    {
        public frmCreateGlcmSurface(IMap map)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            rstUtil = new rasterUtil();
            glcmTexture = new glcm(ref rstUtil);
            populateComboBox();
        }
        public frmCreateGlcmSurface(IMap map, ref rasterUtil rasterUtility, bool addToMap)
        {
            InitializeComponent();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            rstUtil = rasterUtility;
            glcmTexture = new glcm(ref rstUtil);
            populateComboBox();
            perm = addToMap;
        }
        private IRaster rst = null;
        private string oNm = null;
        public IRaster OutRaster { get { return rst; } }
        public string OutRasterName { get { return oNm; } }
        private bool perm = true; 
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rstUtil = null;
        private glcm glcmTexture = null;
        private viewUtility vUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
        public Dictionary<string, IRaster> RasterDictionary { get { return rstDic; } }
        private void populateComboBox()
        {
            if (mp != null)
            {
                IEnumLayer rstLyrs = vUtil.getActiveViewLayers(viewUtility.esriIRasterLayer);
                ILayer lyr = rstLyrs.Next();
                while (lyr != null)
                {
                    string lyrNm = lyr.Name;
                    IRasterLayer rstLyr = (IRasterLayer)lyr;
                    IRaster rst = rstLyr.Raster;
                    if (!rstDic.ContainsKey(lyrNm))
                    {
                        rstDic.Add(lyrNm, rst);
                        cmbRaster.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.windowType)))
            {
                cmbWindowType.Items.Add(s);
            }
            foreach (string s in Enum.GetNames(typeof(rasterUtil.glcmMetric)))
            {
                cmbGlcmTypes.Items.Add(s);
            }
        }
        private void getRasterPath()
        {
            string outPath = null;
            string outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = false;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = null;
            flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            gxDialog.ObjectFilter = flt;
            gxDialog.Title = "Select a Feature";
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                outPath = gxObj.FullName;
                outName = gxObj.BaseName;
                if (!rstDic.ContainsKey(outName))
                {
                    rstDic.Add(outName, rstUtil.returnRaster(outPath));
                    cmbRaster.Items.Add(outName);
                }
                else
                {
                    rstDic[outName] = rstUtil.returnRaster(outPath);
                }
                cmbRaster.Text = outName;
            }
            return;
        }

        public void addRasterToComboBox(string rstName, IRaster rst)
        {
            if (!cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Add(rstName);
                rstDic.Add(rstName, rst);
            }
        }
        public void removeRasterFromComboBox(string rstName)
        {
            if (cmbRaster.Items.Contains(rstName))
            {
                cmbRaster.Items.Remove(rstName);
                rstDic.Remove(rstName);
            }
        }

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getRasterPath();
        }

        private void cmbWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWindowType.Text.ToUpper() == "CIRCLE")
            {
                nudRows.Visible = false;
                lblRows.Visible = false;
                lblColumns.Text = "Radius";
            }
            else
            {
                nudRows.Visible = true;
                lblRows.Visible = true;
                lblColumns.Text = "Columns";
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            string txt = cmbGlcmTypes.Text;
            if (txt != null && txt != "")
            {
                cmbGlcmTypes.Items.Remove(txt);
                if (!lsbGlcmType.Items.Contains(txt))
                {
                    lsbGlcmType.Items.Add(txt);
                }
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            string txt = lsbGlcmType.SelectedItem.ToString();
            if (txt != null && txt != "")
            {
                lsbGlcmType.Items.Remove(txt);
                if (!cmbGlcmTypes.Items.Contains(txt))
                {
                    cmbGlcmTypes.Items.Add(txt);
                }
            }


        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            int glcmTypesCnt = cmbGlcmTypes.Items.Count;
            for (int i = 0; i < glcmTypesCnt; i++)
            {
                string st = cmbGlcmTypes.Items[i].ToString();
                lsbGlcmType.Items.Add(st);
            }
            cmbGlcmTypes.Items.Clear();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            int glcmTypesCnt = lsbGlcmType.Items.Count;
            for (int i = 0; i < glcmTypesCnt; i++)
            {
                string st = lsbGlcmType.Items[i].ToString();
                cmbGlcmTypes.Items.Add(st);
            }
            lsbGlcmType.Items.Clear();

        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            createRaster();
        }
        private void createRaster()
        {
            string rstNm = cmbRaster.Text;
            string rstOut = mtbOutName.Text.Trim();
            string windType = cmbWindowType.Text;
            string windDir = cmbDirections.Text;
            int clms = System.Convert.ToInt32(nudColumns.Value);
            int rows = System.Convert.ToInt32(nudRows.Value);
            int lsbCnt = lsbGlcmType.Items.Count;
            List<glcm.glcmMetric> glcmArr = new List<glcm.glcmMetric>();
            if (rstNm == "" || rstNm == null||rstOut==""||rstOut==null)
            {
                MessageBox.Show("You must have both a raster and output name specified to use this tool!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (windType == null || windType == "" || windDir == "" || windDir == null)
            {
                MessageBox.Show("Window Type and Direction are not specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (lsbCnt < 1)
            {
                MessageBox.Show("You do not have any GLCM metrics selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (windType.ToUpper() == "CIRCLE")
            {
                glcmTexture.RADIUS = clms;
            }
            else
            {
                glcmTexture.COLUMNS = clms;
                glcmTexture.ROWS = rows;
            }
            if (windDir.ToUpper() == "HORIZONTAL")
            {
                glcmTexture.HORIZONTAL = true;
            }
            else
            {
                glcmTexture.HORIZONTAL = false;
            }
            //this.Enabled = false;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog((perm==false));
            System.DateTime t1 = System.DateTime.Now;
            rp.addMessage("Start date and time = " + t1.Month.ToString() + "\\" + t1.Day.ToString() + "\\" + t1.Year.ToString() + " " + t1.Hour.ToString() + ":" + t1.Minute.ToString() + ":" + t1.Second.ToString());
            rp.addMessage("Creating the following " + windDir + " GLCM surfaces from the first band of the specified Raster:");
            rp.stepPGBar(15);
            if (perm)
            {
                rp.Show();
            }
            rp.Refresh();
            rp.TopMost = true;
            foreach (glcm.glcmMetric g in glcmArr)
            {
                rp.addMessage("\t" + g.ToString());
            }
            rp.Refresh();
            IRaster inRs = rstDic[rstNm];
            try
            {
                bool horz = true;
                if (windDir.ToUpper()!="HORIZONTAL")
                {
                    horz = false;
                }
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0;i<lsbCnt;i++)
                {
                    glcm.glcmMetric mtr = (glcm.glcmMetric)Enum.Parse(typeof(glcm.glcmMetric),lsbGlcmType.Items[i].ToString());
                    switch (mtr)
                    {
                        case glcm.glcmMetric.CONTRAST:
                            if(windType.ToUpper()=="CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmContrast(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmContrast(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.DISSIMILARITY:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmDissimilarity(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmDissimilarity(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.HOMOGENEITY:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmHomogeneity(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmHomogeneity(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.ASM:
                        case glcm.glcmMetric.ENERGY:
                        case glcm.glcmMetric.MAXPROBABILITY:
                        case glcm.glcmMetric.MINPROBABILITY:
                        case glcm.glcmMetric.RANGE:
                        case glcm.glcmMetric.ENTROPY:
                            glcmArr.Add(mtr);
                            break;
                        case glcm.glcmMetric.MEAN:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmMean(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmMean(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.VARIANCE:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmVariance(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmVariance(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.CORRELATION:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmCorrelation(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmCorrelation(inRs, clms, rows, horz));
                            }
                            break;
                        case glcm.glcmMetric.COVARIANCE:
                            if (windType.ToUpper() == "CIRCLE")
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmCoVariance(inRs, clms, horz));
                            }
                            else
                            {
                                rsBc.AppendBands((IRasterBandCollection)rstUtil.glcmCoVariance(inRs, clms, rows, horz));
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (glcmArr.Count > 0)
                {
                    glcmTexture.GLCM_METRIC = glcmArr.ToArray();
                    glcmTexture.InRaster = inRs;
                    glcmTexture.createTexture();
                    rsBc.AppendBands((IRasterBandCollection)glcmTexture.OUTRASTER);
                }
                rst = (IRaster)rsBc;
                if (mp != null&&perm)
                {
                    IRasterLayer rsLyr = new RasterLayerClass();
                    //rstUtil.calcStatsAndHist(rst);
                    rsLyr.CreateFromRaster(rst);
                    rsLyr.Name = rstOut;
                    rsLyr.Visible = false;
                    mp.AddLayer((ILayer)rsLyr);
                }
                System.DateTime t2 = System.DateTime.Now;
                rp.addMessage("End date and time = " + t2.Month.ToString() + "\\" + t2.Day.ToString() + "\\" + t2.Year.ToString() + " " + t2.Hour.ToString() + ":" + t2.Minute.ToString() + ":" + t2.Second.ToString());
                rp.stepPGBar(100);
                rp.Refresh();
                oNm = rstOut;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                rp.enableClose();
                this.Close();
            }
            return;
        }
    }
}
