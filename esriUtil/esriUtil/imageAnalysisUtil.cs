using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.SpatialAnalystTools;

namespace esriUtil
{
    public class imageAnalysisUtil
    {
        public imageAnalysisUtil()
        {
            gp.OverwriteOutput = true;
            IAoInitialize a = new AoInitializeClass();
            if (!a.IsExtensionCheckedOut(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst))
            {
                weCheckedOutSpa = true;
                a.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }
        }
        ~imageAnalysisUtil()
        {
            if (weCheckedOutSpa)
            {
                IAoInitialize a = new AoInitializeClass();
                a.CheckInExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }

        }
        private bool weCheckedOutSpa = false;
        private rasterUtil rsUtil = new rasterUtil();
        public enum statType { MEAN, MAJORITY, MAXIMUM, MEDIAN, MINIMUM, MINORITY, RANGE, STD, SUM, VARIETY };
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private Geoprocessor gp = new Geoprocessor();
        /// <summary>
        /// runs the geoprocessor given a process
        /// </summary>
        /// <param name="process">a specified process</param>
        /// <returns>geoprocessing messages</returns>
        private IGeoProcessorResult gpExecute(IGPProcess process)
        {
            return (IGeoProcessorResult)gp.Execute(process, null);
        }
        /// <summary>
        /// Returns all geoprocessing messages
        /// </summary>
        /// <param name="gpRslt">the geoprocessign result</param>
        /// <returns>a string of all messages</returns>
        private string getMessages(IGeoProcessorResult gpRslt)
        {
            StringBuilder sB = new StringBuilder();
            for (int i = 0; i < gpRslt.MessageCount; i++)
            {
                sB.Append(gpRslt.GetMessage(i));
            }
            return sB.ToString();
        }
        /// <summary>
        /// Calulates band statics simple stats and corraltion and covariance matrix
        /// </summary>
        /// <param name="inRasterPath">path of the input raster (string, IRasterdataset, IRaster, IRasterDescription</param>
        /// <param name="outStats">full path of the output statistics</param>
        /// <returns>geoprocessing messages</returns>
        public string calcBandStats(object inRasterPath, string outStats)
        {

            ESRI.ArcGIS.SpatialAnalystTools.BandCollectionStats bcStats = new BandCollectionStats();
            bcStats.compute_matrices = "DETAILED";
            bcStats.in_raster_bands = inRasterPath;
            return getMessages(gpExecute(bcStats));


            //mulvOp.BandCollectionStats((IGeoDataset)inRasterDset, outStats, true);
        }
        /// <summary>
        /// Performs an unsupervised iso Clustering Maximum likelyhood classification
        /// </summary>
        /// <param name="inRasterDset"> input raster dataset (string, IRaster, IRasterDataset, IRasterDescription) used to perform the isoCluster</param>
        /// <param name="outWorkSpace">string identifing the output workspace</param>
        /// <param name="numClasses">int specifing the number of classes</param>
        /// <param name="outRstPath">the full path of the output Raster. If null will use outWorkSpace and name the raster iso</param>
        /// <param name="outSigPath">the full path fo the signature file. If null will use outWorkSpace and name signature file isoSig.gsg</param>
        /// <returns>geoprocessing messages</returns>
        public string isoClusterUnSupervised(object inRasterDset, IWorkspace outWorkSpace, int numClasses, string outRstPath, string outSigPath)
        {
            Forms.RunningProcess.frmRunningProcessDialog frmRd = new Forms.RunningProcess.frmRunningProcessDialog(false);
            frmRd.addMessage("Running Iso Cluster Unsupervised classification");
            frmRd.stepPGBar(10);
            frmRd.Show();
            string outRstStr = null;
            if (outRstPath == null)
            {
                outRstPath = outWorkSpace.PathName + "\\iso";
                outSigPath = System.IO.Path.GetDirectoryName(outWorkSpace.PathName) + "\\isoSig.gsg";
            }
            try
            {
                gp.OverwriteOutput = true;
                gp.SetEnvironmentValue("workspace", outWorkSpace.PathName);
                ESRI.ArcGIS.SpatialAnalystTools.IsoClusterUnsupervisedClassification isoCls = new IsoClusterUnsupervisedClassification();
                isoCls.Input_raster_bands = inRasterDset;
                isoCls.Minimum_class_size = numClasses * 10;
                isoCls.Number_of_classes = numClasses;
                isoCls.Output_classified_raster = outRstPath;
                isoCls.Output_signature_file = outSigPath;
                isoCls.Sample_interval = 10;
                IGeoProcessorResult gpRslt = (IGeoProcessorResult)gp.Execute(isoCls, null);
                frmRd.addMessage(getMessages(gpRslt));
                outRstStr = gpRslt.ReturnValue.ToString();
                frmRd.stepPGBar(60);
            }
            catch (Exception e)
            {
                string x = e.ToString();
                Console.WriteLine("New Error: " + x);
                frmRd.addMessage(x);
            }
            finally
            {
                frmRd.addMessage("Finished IsoCluster");
                frmRd.stepPGBar(100);
                frmRd.enableClose();
                frmRd.TopMost = false;
            }

            return outRstStr;
        }
        /// <summary>
        /// Creates a random raster given a template raster (defined extent and cell size)
        /// </summary>
        /// <param name="templateRasterPath">full path location of the template raster</param>
        /// <param name="outRasterPath">the full path location of the output random raster</param>
        /// <returns>geoprocessing messages</returns>
        public string createRandomRaster(string templateRasterPath, string outRasterPath)
        {
            ESRI.ArcGIS.SpatialAnalystTools.CreateRandomRaster cRndRst = new CreateRandomRaster();
            string bnd = "";
            IRasterDataset rDset = rsUtil.openRasterDataset(templateRasterPath,out bnd);
            IRaster rst = rsUtil.createRaster(rDset);
            IRasterProps rstProp = (IRasterProps)rst;
            IPnt pnt = rstProp.MeanCellSize();
            cRndRst.cell_size = pnt.X;
            cRndRst.out_raster = outRasterPath;
            return getMessages(gpExecute(cRndRst));
        }
        /// <summary>
        /// Calultes std using spatial analyst focal statistics for each raster band within a specified neighborhood (square). Naming convention is std_bandNumber
        /// </summary>
        /// <param name="inputRstPath">can be a IRaster IRasterDataset or string path to a raster</param>
        /// <param name="numCellsWide"> number of cells wide</param>
        /// <returns>a string of all geoprocessing messages</returns>
        public string calcBandSpatialVariation(object inputRstPath, int numCellsWide)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                gp.OverwriteOutput = true;
                IRasterDataset rsDset = null;
                string rsPath = null;
                if (inputRstPath is string)
                {
                    rsPath = inputRstPath.ToString();
                    string bnd = "";
                    rsDset = rsUtil.openRasterDataset(rsPath,out bnd);
                }
                else if (inputRstPath is IRasterDataset)
                {
                    rsDset = (IRasterDataset)inputRstPath;
                    IDataset dSet = (IDataset)rsDset;
                    rsPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                }
                else if (inputRstPath is IRaster2)
                {
                    rsDset = ((IRaster2)inputRstPath).RasterDataset;
                    IDataset dSet = (IDataset)rsDset;
                    rsPath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
                }
                else
                {
                    string x = "Not a string, RasterDataset, or Raster";
                    Console.WriteLine(x);
                    return x;
                }
                IRasterBandCollection rsBndColl = (IRasterBandCollection)rsDset;
                int bndCnt = rsBndColl.Count;
                for (int cnt = 0; cnt < bndCnt; cnt++)
                {
                    IRaster rst = rsUtil.returnRaster(rsUtil.getBand(inputRstPath, cnt));
                    string outRstPath = System.IO.Path.GetDirectoryName(rsPath) + "\\std_" + (cnt + 1).ToString();
                    IGPSANeighborhood nbr = new GPSANeighborhoodClass();
                    double cells = System.Convert.ToDouble(numCellsWide);
                    nbr.SetRectangle(cells, cells, esriGeoAnalysisUnitsEnum.esriUnitsCells);
                    IGeoProcessorResult rslt = focalRaster(rst, nbr, statType.STD, "std_" + cnt.ToString());
                    sb.Append(getMessages(rslt));
                }
            }
            catch (Exception e)
            {
                sb.Append(e.ToString());
                Console.WriteLine(e.ToString());

            }
            return sb.ToString();

        }
        /// <summary>
        /// Performs a polytomous Logist Regression analysis
        /// </summary>
        /// <param name="dbFile"></param>
        /// <returns></returns>
        public Dictionary<string, IRaster> classifyPolytomicLogisticRegresssion(string dbFile)
        {
            Dictionary<string, IRaster> outRasters = new Dictionary<string, IRaster>();
            try
            {


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return outRasters;
        }
        /// <summary>
        /// floats a raster
        /// </summary>
        /// <param name="inRaster">input raster can be a constant, raster, or string (full path)</param>
        /// <param name="outRst">string full path</param>
        public void floatRaster(object inRaster, string outRst)
        {
            Float fl = new Float();
            fl.in_raster_or_constant = inRaster;
            fl.out_raster = outRst;
            string rsl = getMessages(gpExecute(fl));
        }
        /// <summary>
        /// Negates a raster, multiplies by -1
        /// </summary>
        /// <param name="inRaster">input raster can be a constant, raster, or string (full path)</param>
        /// <param name="outRst">string full path</param>
        public void inversRaster(object inRaster, string outRst)
        {
            Negate ng = new Negate();
            ng.in_raster_or_constant = inRaster;
            ng.out_raster = outRst;
            string rsl = getMessages(gpExecute(ng));
        }
        public void lnRaster(object inRaster, string outRst)
        {
            Ln ln = new Ln();
            ln.in_raster_or_constant = inRaster;
            ln.out_raster = outRst;
            string rsl = getMessages(gpExecute(ln));
        }
        /// <summary>
        /// Performs focal statistics
        /// </summary>
        /// <param name="inRaster"></param>
        /// <param name="window">defined neighborhood window</param>
        /// <param name="stat">defined statistic from imageAnaysisUtil enum</param>
        /// <param name="outRst">the name of the output raster</param>
        public IGeoProcessorResult focalRaster(object inRaster, IGPSANeighborhood window, statType stat, string outRst)
        {
            FocalStatistics fstats = new FocalStatistics();
            fstats.neighborhood = window;
            fstats.out_raster = outRst;
            fstats.statistics_type = stat.ToString();
            fstats.in_raster = inRaster;
            return gpExecute(fstats);
        }
        /// <summary>
        /// Returns a neighborhood window
        /// </summary>
        public IGPSANeighborhood getWindow
        {
            get
            {
                return new GPSANeighborhoodClass();
            }
        }
    }
}
