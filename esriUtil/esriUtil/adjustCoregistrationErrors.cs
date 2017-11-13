using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;
using Accord.Statistics;

namespace esriUtil
{
    public class adjustCoregistrationErrors
    {
        public adjustCoregistrationErrors()
        {
            rsUtil = new rasterUtil();
            ftrUtil = new featureUtil(rsUtil);
        }
        public adjustCoregistrationErrors(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
            ftrUtil = new featureUtil(rsUtil);
        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = null;
        private featureUtil ftrUtil = null;
        private IFeatureClass plotFtrClass = null;
        public IFeatureClass PlotFeatureClass { get { return plotFtrClass; } set { plotFtrClass = value; } }
        private string dependentFld = null;
        public string DependentField { get { return dependentFld; } set { dependentFld = value; } }
        private IFunctionRasterDataset fRsDset = null;
        public IFunctionRasterDataset FunctionRasterDataset { get { return fRsDset; } set { fRsDset = value; bndCnt = fRsDset.RasterInfo.BandCount; } }
        private int bndCnt = 4;
        private int geoerro = 10;
        private int tPlots = 100;
        public int GeometricErrorCells { get { return geoerro; } set { geoerro = value*2+1; } }
        private double[,] dataArray = null;//rows by column
        private double[][][] allData = null;
        private string outftrclasspath = null;
        public string OutFtrClassPath { get { return outftrclasspath; } set { outftrclasspath = value; } }
        IFeatureClass outFtrClass = null;
        public IFeatureClass OutFtrClass { get { return outFtrClass; } }
        public void adjustErrors()
        {
            if (!checkInputs())
            {
                return;
            }
            int depIndex = plotFtrClass.FindField(DependentField);          
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = plotFtrClass.OIDFieldName+","+plotFtrClass.ShapeFieldName+","+DependentField;
            qf.WhereClause = "NOT " + dependentFld + " is NULL";
            //Console.WriteLine(qf.WhereClause);
            tPlots = plotFtrClass.FeatureCount(qf);
            //Console.WriteLine("Total Records = " + tPlots.ToString());
            dataArray = new double[tPlots, bndCnt + 1];
            allData = new double[tPlots][][];
            IFeatureCursor ftrCur = plotFtrClass.Search(qf, true);
            IFeature ftr = ftrCur.NextFeature();
            int ftrCnt = 0;
            while (ftr != null)
            {
                //Console.WriteLine("Getting values for point " + ftrCnt.ToString());
                object depVlobj = ftr.get_Value(depIndex);
                double depVl = System.Convert.ToDouble(depVlobj);
                IPoint pnt = (IPoint)ftr.Shape;
                double[] rwVls;
                double[][] rsPlotValues = getValues(pnt,out rwVls);//one row for each set of variables missing the center value; geoErro^2-1*bandCnt
                dataArray[ftrCnt, bndCnt] = depVl;
                allData[ftrCnt] = rsPlotValues;
                for (int i = 0; i < bndCnt; i++)
			    {
                    dataArray[ftrCnt, i] = rwVls[i];
			    }
                ftr = ftrCur.NextFeature();
                ftrCnt = ftrCnt+1;
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            int[] newPlotsRwClm = findBestCell(); //returns the row number of the cell that has the best correlation. will use this to determin new plot location.
            //Console.WriteLine("exporting feature class");
            outFtrClass = ftrUtil.exportFeatures(plotFtrClass, outftrclasspath, null);
            qf.SubFields = "*";
            IFeatureCursor uCur = outFtrClass.Update(qf, true);
            IFeature uFtr = uCur.NextFeature();
            ftrCnt = 0;
            while (uFtr != null)
            {
                //Console.WriteLine("Updating plot " + ftrCnt.ToString());
                int bestCell = newPlotsRwClm[ftrCnt];
                IPoint pnt = (IPoint)uFtr.ShapeCopy;
                IPoint nPnt = updatePnt(bestCell, pnt);
                uFtr.Shape = nPnt;
                uCur.UpdateFeature(uFtr);
                uFtr = uCur.NextFeature();
                ftrCnt = ftrCnt + 1;
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(uCur);

        }

        private IPoint updatePnt(int bestCell, IPoint pnt)
        {
            IPnt cellSize = FunctionRasterDataset.RasterInfo.CellSize;
            IPoint nPnt = new PointClass();
            double xSize = cellSize.X;
            double ySize = cellSize.Y;
            int g2 = geoerro / 2;
            double dv = System.Convert.ToDouble(bestCell) / System.Convert.ToDouble(geoerro);
            int rw = (int)dv;
            int clm = System.Convert.ToInt32((dv-rw)*geoerro);
            double shiftX = (clm - g2) * xSize;
            double shiftY = (rw - g2) * ySize;
            Console.WriteLine("shift x, y = " + shiftX.ToString() + ", " + shiftY.ToString());
            nPnt.PutCoords(pnt.X + shiftX, pnt.Y - shiftY);
            return nPnt;
        }

        private int[] findBestCell()
        {
            int[] bestRw = new int[allData.Length];
            for (int i = 0; i < allData.Length; i++) //total rows in the point ftrCls
            {
                double maxCorr = sumCorr();// sumCorr();
                double[][] nVlArr = allData[i]; //pixel values (Jagged Array) for each cell within the geoError
                bool foundBetter = false;
                for (int j = 0; j < nVlArr.Length; j++) 
                {
                    double[] nVlArr2 = nVlArr[j]; // values (Array) for each pixel
                    for (int k = 0; k < nVlArr2.Length; k++)
			        {
                        dataArray[i, k] = nVlArr2[k]; // replaceing values in dataArray
			        }
                    double newCorr = sumCorr();//sumCorr(); //recalculating summed Corr
                    //Console.WriteLine("Summed Corr = " + newCorr.ToString());
                    if (newCorr > maxCorr) //determining if new correlation is better and if so keep track
                    {
                        maxCorr = newCorr;
                        bestRw[i] = j;
                        foundBetter = true;
                    }
                }
                
                //reseting data array to best correlation values
                double[] bestVlArr = null;
                if (foundBetter)
                {
                    bestVlArr = nVlArr[bestRw[i]];
                }
                else
                {
                    Console.WriteLine("Could not find a better value for row = " + i.ToString());
                    bestVlArr = nVlArr[geoerro*geoerro / 2];
                }
                for (int k = 0; k < bestVlArr.Length; k++)
                {
                    dataArray[i, k] = bestVlArr[k];
                }                
            }
            return bestRw;

        }

        private double sumReg()
        {
            double[] output;
            double[][] input;
            adjustArray(out output, out input);
            Accord.Statistics.Analysis.MultipleLinearRegressionAnalysis mr = new Accord.Statistics.Analysis.MultipleLinearRegressionAnalysis(input, output, true);
            mr.Compute();
            return mr.RSquared;
        }

        private double sumCorr()
        {
            double[,] corr = Accord.Statistics.Tools.Correlation(dataArray);
            double outVl = 0;
            for (int i = 0; i < bndCnt + 1; i++)
            {
                outVl = outVl + Math.Abs(corr[bndCnt, i]);
            }
            return outVl;
        }

        private void adjustArray(out double[] output, out double[][] input)
        {
            output = new double[tPlots];
            input = new double[tPlots][];
            for (int i = 0; i < tPlots; i++)
            {
                output[i] = dataArray[i, bndCnt];
                double[] newArr = new double[bndCnt];
                for (int k = 0; k < bndCnt; k++)
                {
                    newArr[k] = dataArray[i, k];
                }
                input[i] = newArr; 
            }
        }

        private double[][] getValues(IPoint pnt, out double[] rwVls)
        {
            rwVls = new double[bndCnt];
            double[][] outJagArr = new double[(geoerro * geoerro)][];
            for (int i = 0; i < outJagArr.Length; i++)
            {
                outJagArr[i] = new double[bndCnt];
            }
            IRasterBandCollection rsBc = (IRasterBandCollection)FunctionRasterDataset;
            IRasterBand rsB;
            IPnt pSize = new PntClass();
            pSize.SetCoords(geoerro,geoerro);
            IPnt pLoc = new PntClass();
            int clm,rw;
            IGeometry geo = (IGeometry)FunctionRasterDataset.RasterInfo.Extent;
            IPoint ul = FunctionRasterDataset.RasterInfo.Extent.UpperLeft;
            IPnt cellSize = FunctionRasterDataset.RasterInfo.CellSize;
            int sub = (geoerro/2);
            rsUtil.getClmRw(ul, cellSize, pnt, out clm, out rw);
            int nclm = clm - sub;
            int nrw = rw - sub;
            pLoc.SetCoords(System.Convert.ToDouble(nclm),System.Convert.ToDouble(nrw));
            for (int i = 0; i < bndCnt; i++)
            {
                rsB = rsBc.Item(i);
                IRawPixels rpix = (IRawPixels)rsB;
                IPixelBlock pb = rpix.CreatePixelBlock(pSize);
                rpix.Read(pLoc, pb);
                int pbCnt = 0;
                for (int r = 0; r < pb.Height; r++)
                {
                    for (int c = 0; c < pb.Width; c++)
                    {
                        object vlObj = pb.GetVal(0, c, r);
                        if(vlObj!=null)
                        {
                            outJagArr[pbCnt][i] = System.Convert.ToDouble(vlObj);
                        }
                        pbCnt++;
                    }
                }
            }
            rwVls = outJagArr[(geoerro * geoerro / 2)];
            return outJagArr;
        }

        private bool checkInputs()
        {
            if (FunctionRasterDataset == null) return false;
            else if (DependentField == null) return false;
            else if (PlotFeatureClass == null) return false;
            else if (outftrclasspath == null) return false;
            else return true;

        }
    }
}
