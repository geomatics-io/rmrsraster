using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;


namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmCalculateStatistics : Form
    {
        public frmCalculateStatistics(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        public frmCalculateStatistics(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
            mp = map;
            if (mp != null)
            {
                vUtil = new viewUtility((IActiveView)mp);
            }
            populateComboBox();
        }
        private bool addToMap = true;
        private IMap mp = null;
        private viewUtility vUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private Dictionary<string, IRaster> rstDic = new Dictionary<string, IRaster>();
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
                        cmbInRaster1.Items.Add(lyrNm);
                    }
                    lyr = rstLyrs.Next();

                }
            }
        }
        private void btnOpenFunctionModelDataset_Click(object sender, EventArgs e)
        {
            functionModel fModel = new functionModel(rsUtil);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Add Function Model to map";
            ofd.Filter = "Function Dataset|*.fds";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fModel.FunctionDatasetPath = ofd.FileName;
                string outNm = "";
                IRaster outRS = null;
                string desc = "";
                fModel.createFunctionRaster(out outNm, out outRS, out desc);
                rstDic[outNm] = outRS;
                if (!cmbInRaster1.Items.Contains(outNm))
                {
                    cmbInRaster1.Items.Add(outNm);
                }
                cmbInRaster1.SelectedItem = outNm;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string rstNm = cmbInRaster1.Text;
            int pixelNm = System.Convert.ToInt32(nudSkip.Value);
            if (rstNm == "" || rstNm == null)
            {
                MessageBox.Show("You must select an input raster!", "No Input Raster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            DateTime dt = DateTime.Now;
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Calculating Statistics. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            rp.Refresh();
            try
            {
                IRaster rst = rstDic[rstNm];
                //Console.WriteLine(functionModel.FunctionDatasetPathDic.ContainsKey(rstNm));
                rst = rsUtil.calcStatsAndHist(rst, pixelNm);
                if (functionModel.FunctionDatasetPathDic.ContainsKey(rstNm))
                {
                    rp.addMessage("Updating Statistics for a function dataset");
                    rp.Refresh();
                    functionModel.buildStats(rst, functionModel.FunctionDatasetPathDic[rstNm]);
                    
                }
                if (mp != null)
                {
                    IRasterLayer rsLyr = new RasterLayerClass();
                    rsLyr.CreateFromRaster(rst);
                    rsLyr.Name = rstNm;
                    rsLyr.Visible = false;
                    for (int i = 0; i < mp.LayerCount; i++)
                    {
                        ILayer lyr = mp.get_Layer(i);
                        if (lyr.Name == rstNm)
                        {
                            mp.DeleteLayer(lyr);
                            break;
                        }
                    }
                    mp.AddLayer(rsLyr);
                    
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage("Error: could not calculate statistics. here is the error message:\n" + ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished building Statistics" + t);
                rp.enableClose();
                this.Close();
            }


        }
    }
}
