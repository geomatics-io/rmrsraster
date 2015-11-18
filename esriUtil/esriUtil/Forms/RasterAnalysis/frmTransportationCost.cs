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

namespace esriUtil.Forms.RasterAnalysis
{
    public partial class frmTransportationCost : Form
    {
        public frmTransportationCost(IMap Map, rasterUtil rasterUtility = null)
        {
            InitializeComponent();
            frmHlp = new frmHelper(Map, rasterUtility);
            geoUtil = frmHlp.GeoUtility;
            rsUtil = frmHlp.RasterUtility;
            ftrUtil = frmHlp.FeatureUtility;
            populateCmb();
        }

        
        public frmHelper frmHlp = null;
        geoDatabaseUtility geoUtil = null;
        rasterUtil rsUtil = null;
        featureUtil ftrUtil = null;
        private void populateCmb()
        {
            foreach (string s in frmHlp.FunctionRasterDictionary.Keys)
            {
                cmbDem.Items.Add(s);
                cmbOffRate.Items.Add(s);
                cmbOffRoadSpeed.Items.Add(s);
                cmbOnRate.Items.Add(s);
                cmbOps.Items.Add(s);
                cmbOther.Items.Add(s);
            }
            foreach (string s in frmHlp.FeatureDictionary.Keys)
            {
                IFeatureClass ftrCls = frmHlp.FeatureDictionary[s];
                ESRI.ArcGIS.Geometry.esriGeometryType geoType = ftrCls.ShapeType;
                if (geoType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine || geoType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    cmbRoad.Items.Add(s);
                    cmbBarrier.Items.Add(s);

                }
                else
                {
                    cmbFacility.Items.Add(s);
                }
            }
            foreach (string s in Enum.GetNames(typeof(TransRouting.SpeedUnits)))
            {
                cmbUnits.Items.Add(s);
            }
            cmbUnits.SelectedItem = cmbUnits.Items[0];
        }
        private void chbHours_CheckedChanged(object sender, EventArgs e)
        {
            if (chbHours.Checked)
            {
                lblSpeed.Visible = true;
                cmbSpeed.Visible = true;
                lblFacility.Visible = true;
                lblRoads.Text = "Road Layer";
                lblOffSpeed.Text = "Speed (Value or Raster)";
                lblBarriers.Visible = true;
                cmbBarrier.Visible = true;
                btnBarriers.Visible = true;
                btnFacility.Visible = true;
                cmbFacility.Visible = true;
                cmbRoad.Text = "";
                cmbOffRoadSpeed.Text = "";
                cmbRoad.Items.Clear();
                cmbOffRoadSpeed.Items.Clear();
                foreach (string s in frmHlp.FeatureDictionary.Keys)
                {
                    IFeatureClass ftrCls = frmHlp.FeatureDictionary[s];
                    if (ftrCls.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline||ftrCls.ShapeType== ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine)
                    {
                        cmbRoad.Items.Add(s);
                    }
                }
                foreach (string s in frmHlp.FunctionRasterDictionary.Keys)
                {
                    cmbOffRoadSpeed.Items.Add(s);
                }
            }
            else
            {
                lblSpeed.Visible = false;
                cmbSpeed.Visible = false;
                lblFacility.Visible = false;
                lblRoads.Text = "Allocated Hour Surface";
                lblOffSpeed.Text = "Off Road Hour Surface";
                lblBarriers.Visible = false;
                cmbBarrier.Visible = false;
                cmbFacility.Visible = false;
                btnBarriers.Visible = false;
                btnFacility.Visible = false;
                cmbRoad.Text = "";
                cmbOffRoadSpeed.Text = "";
                cmbRoad.Items.Clear();
                cmbOffRoadSpeed.Items.Clear();
                foreach (string s in frmHlp.FunctionRasterDictionary.Keys)
                {
                    cmbRoad.Items.Add(s);
                    cmbOffRoadSpeed.Items.Add(s);
                }
            }
        }

        private void cmbRoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (frmHlp.FeatureDictionary.ContainsKey(cmbRoad.Text))
            {
                IFeatureClass rdFtr = frmHlp.FeatureDictionary[cmbRoad.Text];
                cmbSpeed.Items.Clear();
                for (int i = 0; i < rdFtr.Fields.FieldCount; i++)
                {
                    IField fld = rdFtr.Fields.get_Field(i);
                    if (fld.Type == esriFieldType.esriFieldTypeDouble || fld.Type == esriFieldType.esriFieldTypeInteger || fld.Type == esriFieldType.esriFieldTypeSingle || fld.Type == esriFieldType.esriFieldTypeSmallInteger)
                    {
                        cmbSpeed.Items.Add(fld.Name);
                    }
                }
                cmbSpeed.SelectedItem = cmbSpeed.Items[0];
            }
        }

        private void btnDem_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbDem.Items.Add(nm);
            cmbDem.SelectedItem = nm;
            
        }

        private string getRaster()
        {
            string[] nm;
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterRasterDatasetsClass();
            string rstPath = frmHlp.getPath(flt, out nm, false)[0];
            frmHlp.FunctionRasterDictionary[nm[0]] = rsUtil.createIdentityRaster(rstPath);
            return nm[0];
        }

        private void btnOpenRoad_Click(object sender, EventArgs e)
        {
            string nm = getFeatureClass(new ESRI.ArcGIS.Catalog.GxFilterPolylineFeatureClassesClass());
            cmbRoad.Items.Add(nm);
            cmbRoad.SelectedItem = nm;
        }

        private string getFeatureClass(ESRI.ArcGIS.Catalog.IGxObjectFilter flt )
        {
            string[] nm;
            string ftrPath = frmHlp.getPath(flt, out nm, false)[0];
            frmHlp.FeatureDictionary[nm[0]] = geoUtil.getFeatureClass(ftrPath);
            return nm[0];
        }

        private void btnFacility_Click(object sender, EventArgs e)
        {
            string nm = getFeatureClass(new ESRI.ArcGIS.Catalog.GxFilterFeatureClassesClass());
            cmbFacility.Items.Add(nm);
            cmbFacility.SelectedItem = nm;
        }

        private void btnOnRate_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbOnRate.Items.Add(nm);
            cmbOnRate.SelectedItem = nm;
        }

        private void btnOffRoadSpeed_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbOffRoadSpeed.Items.Add(nm);
            cmbOffRoadSpeed.SelectedItem = nm;
        }

        private void btnOffRate_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbOffRate.Items.Add(nm);
            cmbOffRate.SelectedItem = nm;
        }

        private void btnBarriers_Click(object sender, EventArgs e)
        {
            string nm = getFeatureClass(new ESRI.ArcGIS.Catalog.GxFilterPolylineFeatureClassesClass());
            cmbBarrier.Items.Add(nm);
            cmbBarrier.SelectedItem = nm;
        }

        private void btnOps_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbOps.Items.Add(nm);
            cmbOps.SelectedItem = nm;
        }

        private void btnOther_Click(object sender, EventArgs e)
        {
            string nm = getRaster();
            cmbOther.Items.Add(nm);
            cmbOther.SelectedItem = nm;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string demStr = cmbDem.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(demStr))
            {
                MessageBox.Show("Dem must be specified before executing");
                return;
            }
            string unitsStr = cmbUnits.Text;
            if (unitsStr == "")
            {
                MessageBox.Show("Speed Units must be specified before executing");
                return;
            }
            string roadsStr = cmbRoad.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(roadsStr) && !frmHlp.FeatureDictionary.ContainsKey(roadsStr))
            {
                MessageBox.Show("Roads Layer or Accumulation Raster must be specified before executing");
                return;
            }
            string facilityStr = cmbFacility.Text;
            string speedFldStr = cmbSpeed.Text;
            if (chbHours.Checked)
            {
                
                if (!frmHlp.FeatureDictionary.ContainsKey(facilityStr))
                {
                    MessageBox.Show("Facility Layer must be specified before executing");
                    return;
                }
                
                if (speedFldStr=="")
                {
                    MessageBox.Show("Speed Fld must be specified before executing");
                    return;
                }
            }
            string onRateStr = cmbOnRate.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(onRateStr)&&!rsUtil.isNumeric(onRateStr))
            {
                MessageBox.Show("On Road Machine Rate must be specified before executing");
                return;
            }
            float onPay = System.Convert.ToSingle(nudOnPayload.Value);
            string offSpeedStr = cmbOffRoadSpeed.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(offSpeedStr) && !rsUtil.isNumeric(offSpeedStr))
            {
                MessageBox.Show("Off Road Speed or Off Road hours Raster must be specified before executing");
                return;
            }
            string offRateStr = cmbOffRate.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(offRateStr)&&!rsUtil.isNumeric(offRateStr))
            {
                MessageBox.Show("Off Road Machine Rate must be specified before executing");
                return;
            }
            string offBarriersStr = cmbBarrier.Text;
            float offPay = System.Convert.ToSingle(nudOffPayload.Value);
            string opsStr = cmbOps.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(opsStr)&&!rsUtil.isNumeric(opsStr))
            {
                MessageBox.Show("Operation Rate must be specified before executing");
                return;
            }
            string otherStr = cmbOther.Text;
            if (!frmHlp.FunctionRasterDictionary.ContainsKey(otherStr) && !rsUtil.isNumeric(otherStr))
            {
                MessageBox.Show("Other rate must be specified before executing");
                return;
            }
            string wksStr = txtOutWks.Text;
            IWorkspace wks = geoUtil.OpenWorkSpace(wksStr);
            if (wks == null)
            {
                MessageBox.Show("A valid file geodatabase must be specified before executing");
                return;
            }
            RunningProcess.frmRunningProcessDialog rp = new RunningProcess.frmRunningProcessDialog();
            rp.addMessage("Calculating delivered cost surface...\nThis may take a while...");
            rp.stepPGBar(10);
            rp.TopMost = true;
            rp.Show();
            DateTime dt = DateTime.Now;
            this.Visible = false;
            try
            {
                IFunctionRasterDataset Dem = frmHlp.FunctionRasterDictionary[demStr];
                TransRouting.SpeedUnits units = (TransRouting.SpeedUnits)Enum.Parse(typeof(TransRouting.SpeedUnits), unitsStr);
                TransRouting tr = new TransRouting(wks, Dem, units);
                if (rsUtil.isNumeric(onRateStr))
                {
                    tr.OnRoadMachineRate = System.Convert.ToSingle(onRateStr);
                }
                else
                {
                    tr.OnRoadMachineRateRaster = frmHlp.FunctionRasterDictionary[onRateStr];
                }
                tr.OnRoadPayLoad = onPay;
                if (rsUtil.isNumeric(offRateStr))
                {
                    tr.OffRoadMachineRate = System.Convert.ToSingle(offRateStr);
                }
                else
                {
                    tr.OffRoadMachineRateRaster = frmHlp.FunctionRasterDictionary[offRateStr];
                }
                tr.OffRoadPayLoad = offPay;
                if (rsUtil.isNumeric(opsStr))
                {
                    tr.OperationsCost = System.Convert.ToSingle(opsStr);
                }
                else
                {
                    tr.OperationsCostRaster = frmHlp.FunctionRasterDictionary[opsStr];
                }
                if (rsUtil.isNumeric(otherStr))
                {
                    tr.OtherCost = System.Convert.ToSingle(otherStr);
                }
                else
                {
                    tr.OtherCostRaster = frmHlp.FunctionRasterDictionary[otherStr];
                }
                if (chbHours.Checked)
                {
                    tr.RoadFeatureClass = frmHlp.FeatureDictionary[roadsStr];
                    tr.FacilityFeatureClass = frmHlp.FeatureDictionary[facilityStr];
                    tr.RoadsSpeedField = speedFldStr;
                    if (rsUtil.isNumeric(offSpeedStr))
                    {
                        tr.OffRoadSpeed = System.Convert.ToSingle(offSpeedStr);
                    }
                    else
                    {
                        tr.OffRoadSpeedRaster = frmHlp.FunctionRasterDictionary[offSpeedStr];
                    }
                    if (frmHlp.FeatureDictionary.ContainsKey(offBarriersStr))
                    {
                        tr.BarriersFeatureClass = frmHlp.FeatureDictionary[offBarriersStr];
                    }

                }
                else
                {
                    tr.FunctionAccumulatedPathAllocation = frmHlp.FunctionRasterDictionary[roadsStr];
                    tr.FunctionAccumulatedFromPathDistance = frmHlp.FunctionRasterDictionary[offSpeedStr];
                }
                IFunctionRasterDataset DollarsPerTon = tr.OutDollarsTonsRaster;
                //IFunctionRasterDataset outraster = rsUtil.createRaster(DollarsPerTon);
                if (frmHlp.TheMap != null)
                {
                    rp.addMessage("Calculating Statistics...");
                    rp.Refresh();
                    IRasterLayer rstLyr = new RasterLayerClass();
                    //rsUtil.calcStatsAndHist(((IRaster2)outraster).RasterDataset);
                    rstLyr.CreateFromDataset((IRasterDataset)DollarsPerTon);
                    rstLyr.Name = "DollarsPerTon";
                    rstLyr.Visible = false;
                    frmHlp.TheMap.AddLayer(rstLyr);
                }
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
                rp.addMessage("Finished creating delivered cost surfaces" + t);
                rp.enableClose();
                this.Close();
            }




        }

        private void btnOutWks_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Catalog.IGxObjectFilter flt = new ESRI.ArcGIS.Catalog.GxFilterFileGeodatabasesClass();
            string[] nm;
            txtOutWks.Text = frmHlp.getPath(flt, out nm, false)[0];
        }

        
    }
}
