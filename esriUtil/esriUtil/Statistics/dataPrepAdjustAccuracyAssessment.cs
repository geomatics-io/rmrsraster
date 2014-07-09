using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Accord.Statistics.Analysis;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Statistics
{
    public class dataPrepAdjustAccuracyAssessment
    {
        public dataPrepAdjustAccuracyAssessment(IFeatureClass ProjectArea, IFeatureClass Map, string originalAccuracyAssessmentModel, string adjustedAccuracyAssessmentModel)
        {
            projectArea = ProjectArea;
            ftrMap = Map;
            oModel = originalAccuracyAssessmentModel;
            aModel = adjustedAccuracyAssessmentModel;
        }
        public dataPrepAdjustAccuracyAssessment(IFeatureClass ProjectArea, IRaster Map, string originalAccuracyAssessmentModel, string adjustedAccuracyAssessmentModel)
        {
            projectArea = ProjectArea;
            rstMap = Map;
            oModel = originalAccuracyAssessmentModel;
            aModel = adjustedAccuracyAssessmentModel;
            rsUtil = new rasterUtil();
        }
        private IFeatureClass projectArea = null;
        private IRaster rstMap = null;
        private IFeatureClass ftrMap = null;
        private rasterUtil rsUtil = null;
        private string oModel = "";
        private string aModel = "";
        private int[,] xTable = null;
        public int[,] XTable {get{return xTable;}}
        private double kappa = 0;
        private double ste = 0;
        public ci getConfidenceInterval(double alpha)
        {
            double a = Accord.Math.Normal.Inverse(1 - (alpha / 2)) * STE;
            double u = Kappa + a;
            double l = Kappa - a;
            ci CI = new ci();
            CI.UpperBound = u;
            CI.LowerBound = l;
            return CI;
        }
        public double STE { get { return ste; } }
        public double Kappa { get { return kappa; } }
        Accord.Statistics.Analysis.GeneralConfusionMatrix gc = null;
        dataGeneralConfusionMatirx dgc = null;
        public void buildModel()
        {
            IGeometryBag geoBag = new GeometryBagClass();
            IGeometryCollection geoColl = (IGeometryCollection)geoBag;
            IFeatureCursor ftCur = projectArea.Search(null, false);
            IFeature ftr = ftCur.NextFeature();
            while (ftr != null)
            {
                geoColl.AddGeometry(ftr.Shape);
                ftr = ftCur.NextFeature();
            }
            dgc = new dataGeneralConfusionMatirx();
            dgc.getXTable(oModel);
            olabels = dgc.Labels.ToList();
            xTable = dgc.XTable;
            oClmProp = dgc.GeneralConfusionMatrix.ColumnProportions;
            nCnts = new double[oClmProp.Length];
            adjustXTable((IGeometry)geoBag);
            gc = new GeneralConfusionMatrix(xTable);
            kappa = gc.Kappa;
            ste = gc.StandardError;
            writeXTable();
        }
        private List<string> olabels = null;
        private double[] oClmProp = null;
        private double[] nCnts = null;
        private void adjustXTable(IGeometry iGeometry)
        {
            
            if (rstMap == null)
            {
                adjustXTableFtr(iGeometry);
            }
            else
            {
                adjustXTableRst(iGeometry);
            }
        }

        private void adjustXTableFtr(IGeometry iGeometry)
        {
            string clFldName = dgc.IndependentFieldNames[0];
            ISpatialFilter sf = new SpatialFilterClass();
            sf.Geometry = iGeometry;
            sf.GeometryField = ftrMap.ShapeFieldName;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            double tN = 0;
            IFeatureCursor ftrCur = ftrMap.Search(sf, false);
            IFeature ftr = ftrCur.NextFeature();
            int fldIndex = ftrCur.FindField(clFldName);
            while (ftr != null)
            {
                object vlObj = ftr.get_Value(fldIndex);
                if (vlObj == null)
                {
                }
                else
                {
                    string vl = vlObj.ToString();
                    int vlIndex = olabels.IndexOf(vl);
                    nCnts[vlIndex] = nCnts[vlIndex] + 1;
                    tN += 1;
                }
                ftr = ftrCur.NextFeature();
            }
            updateXTable(tN);
            
        }

        private void updateXTable(double tN)
        {
            for (int r = 0; r < nCnts.Length; r++)
            {
                for (int c = 0; c < nCnts.Length; c++)
                {
                    double w = (nCnts[c] / tN) / oClmProp[c];
                    int vl = System.Convert.ToInt32(xTable[c, r] * w);
                    xTable[c, r] = vl;
                }

            }
        }

        private void adjustXTableRst(IGeometry iGeometry)
        {
            double tN = 0;
            IRaster rs = rsUtil.createRaster(rsUtil.clipRasterFunction(rstMap, iGeometry, ESRI.ArcGIS.DataSourcesRaster.esriRasterClippingType.esriRasterClippingOutside));
            IRasterCursor rsCur = ((IRaster2)rs).CreateCursorEx(null);
            while (rsCur.Next())
            {
                IPixelBlock pb = rsCur.PixelBlock;
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vlObj = pb.GetVal(0, c, r);
                        if (vlObj == null)
                        {
                            continue;
                        }
                        else
                        {
                            string vl = vlObj.ToString();
                            int vlIndex = olabels.IndexOf(vl);
                            nCnts[vlIndex] = nCnts[vlIndex] + 1;
                            tN += 1;
                        }
                    }
                }
            }
            updateXTable(tN);
        }
        private string writeXTable()
        {
            string outPath = aModel;//outDirectory + "\\" + ((IDataset)InTable).BrowseName + "_xt.xtb";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine(dataPrepBase.modelTypes.Accuracy.ToString());
                sw.WriteLine(String.Join(",", dgc.ClassFieldNames));
                sw.WriteLine(String.Join(",", dgc.Labels));
                sw.WriteLine(gc.OverallAgreement.ToString());
                sw.WriteLine(Kappa.ToString());
                sw.WriteLine(STE.ToString());
                int rws = xTable.GetUpperBound(1);
                int clms = xTable.GetUpperBound(0);
                for (int r = 0; r <= rws; r++)
                {
                    string[] ln = new string[clms + 1];
                    for (int c = 0; c <= clms; c++)
                    {
                        ln[c] = xTable[c, r].ToString();
                    }
                    sw.WriteLine(String.Join(" ", ln));
                }
                sw.Close();
            }
            return outPath;
        }
    }
}
