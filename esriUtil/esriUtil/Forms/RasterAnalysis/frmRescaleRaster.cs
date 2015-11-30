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
using ESRI.ArcGIS.Geometry;

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmRescaleRaster : Form
    {
        public frmRescaleRaster(IMap map)
        {
            InitializeComponent();
            rsUtil = new rasterUtil();
            mp = map;
            frmHlp = new frmHelper(mp);
            geoUtil = frmHlp.GeoUtility;
            rsUtil = frmHlp.RasterUtility;
            populateComboBox();
        }
        public frmRescaleRaster(IMap map,ref rasterUtil rasterUtility, bool AddToMap)
        {
            InitializeComponent();
            rsUtil = rasterUtility;
            addToMap = AddToMap;
            mp = map;
            frmHlp = new frmHelper(mp);
            geoUtil = frmHlp.GeoUtility;
            rsUtil = frmHlp.RasterUtility;
            populateComboBox();
        }
        private frmHelper frmHlp = null;
        private bool addToMap = true;
        private IMap mp = null;
        private geoDatabaseUtility geoUtil = null;
        private rasterUtil rsUtil = null;
        public Dictionary<string, IFunctionRasterDataset> RasterDictionary { get { return frmHlp.FunctionRasterDictionary; } }
        private void getFeaturePath()
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            string[] nm;
            string outPath = frmHlp.getPath(flt, out nm, false)[0];
            string outName = nm[0];
            frmHlp.FunctionRasterDictionary[outName] = rsUtil.createIdentityRaster(outPath);
            cmbInRaster1.Text = outName;
            return;
        }
        private IFunctionRasterDataset outraster = null;
        public IFunctionRasterDataset OutRaster { get { return outraster; } }
        private string outrastername = "";
        public string OutRasterName { get { return outrastername; } }
        
        private void populateComboBox()
        {
            if (mp != null)
            {
                foreach (string s in frmHlp.FunctionRasterDictionary.Keys)
                {
                    cmbInRaster1.Items.Add(s);
                }                
            }
            foreach (string s in Enum.GetNames(typeof(rstPixelType)))
            {
                cmbPixel.Items.Add(s);
            }
        }
        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            getFeaturePath();
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            string rstNm = cmbInRaster1.Text;
            string outNm = txtOutName.Text;
            string pixelNm = cmbPixel.Text;
            if (pixelNm == "" || pixelNm == null || rstNm == "" || rstNm == null)
            {
                MessageBox.Show("You must have a raster layer selected and a pixel type selected", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(outNm==""||outNm==null)
            {
                MessageBox.Show("You must specify an output raster name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog(false);
            DateTime dt = DateTime.Now;
            rp.addMessage("Rescaling Raster. This may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            this.Visible = false;
            try
            {
                IFunctionRasterDataset rst = frmHlp.FunctionRasterDictionary[rstNm];
                rstPixelType pType = (rstPixelType)Enum.Parse(typeof(rstPixelType),pixelNm);
                if (stats == null)
                {
                    outraster = rsUtil.reScaleRasterFunction(rst, pType, esriRasterStretchType.esriRasterStretchMinimumMaximum);
                }
                else
                {
                    double[] min = new double[rst.RasterInfo.BandCount];
                    double[] max = new double[rst.RasterInfo.BandCount];
                    double[] mean = new double[rst.RasterInfo.BandCount];
                    double[] std = new double[rst.RasterInfo.BandCount];
                    for (int i = 0; i < rst.RasterInfo.BandCount; i++)
			        {
                        min[i] = stats[i][0];
                        max[i] = stats[i][1];
                        mean[i] = stats[i][2];
                        std[i] = stats[i][3];
			        }
                    //MessageBox.Show(String.Join(",", (from double d in stats[0] select d.ToString()).ToArray())); 
                    outraster = rsUtil.reScaleRasterFunction(rst, pType, esriRasterStretchType.esriRasterStretchMinimumMaximum,min,max,mean,std);
                }
                if (mp != null&&addToMap)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromDataset((IRasterDataset)outraster);
                    rstLyr.Name = outNm;
                    rstLyr.Visible = false;
                    mp.AddLayer(rstLyr);
                }
                outrastername = outNm;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                rp.addMessage(ex.ToString());
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2.Subtract(dt);
                string t = " in " + ts.Days.ToString() + " days " + ts.Hours.ToString() + " hours " + ts.Minutes.ToString() + " minutes and " + ts.Seconds.ToString() + " seconds .";
                rp.stepPGBar(100);
                rp.addMessage("Finished Scalling Raster" + t);
                rp.enableClose();
                this.Close();
            }

        }
        private double[][] stats = null;
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            if (frmHlp.FunctionRasterDictionary.ContainsKey(cmbInRaster1.Text))
            {
                frmSetStatistics frmStats = new frmSetStatistics();
                IFunctionRasterDataset rs = frmHlp.FunctionRasterDictionary[cmbInRaster1.Text];
                IRasterBandCollection rsbc = (IRasterBandCollection)rs;
                frmStats.dgvStats.Rows.Clear();
                
                for (int i = 0; i < rsbc.Count; i++)
                {
                    IRasterStatistics rsStats = rsbc.Item(i).Statistics;
                    object[] rVl = {i+1,0,0,0,0};
                    if (rsStats != null)
                    {
                        rVl = new object[]{ i + 1, rsStats.Minimum, rsStats.Maximum, rsStats.Mean, rsStats.StandardDeviation };
                    }
                    frmStats.dgvStats.Rows.Add(rVl);
                }
                if (frmStats.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //MessageBox.Show("Dialog = ok");
                    stats = new double[rsbc.Count][];
                    //List<string> vlLst = new List<string>();
                    for (int i = 0; i < frmStats.dgvStats.Rows.Count; i++)
                    {

                        DataGridViewRow rw = frmStats.dgvStats.Rows[i];
                        double min = System.Convert.ToDouble(rw.Cells[1].Value);
                        //vlLst.Add(min.ToString());
                        double max = System.Convert.ToDouble(rw.Cells[2].Value);
                        double mean = System.Convert.ToDouble(rw.Cells[3].Value);
                        double std = System.Convert.ToDouble(rw.Cells[4].Value);
                        double[] rwS = { min, max, mean, std };
                        stats[i] = rwS;

                    }
                    //MessageBox.Show(String.Join(",",vlLst.ToArray()));
                }
                else
                {
                    stats = null;
                }
                frmStats.Dispose();

            }
        }
    }
}
