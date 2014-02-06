using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil;
using Accord.Statistics.Analysis;

namespace esriUtil
{
    public class normalization
    {
        public normalization(IRaster ReferenceRaster, IRaster TransformRaster, int PercentChange=20,rasterUtil rasterUtility=null)
        {
            referenceRaster = ReferenceRaster;
            IRasterBandCollection rsBc = (IRasterBandCollection)referenceRaster;
            IRasterProps rsProps = (IRasterProps)referenceRaster;
            rsType = rsProps.PixelType;
            cellCount = new int[rsBc.Count];
            minArray = new double[rsBc.Count];
            maxArray = new double[rsBc.Count];
            sumX2Array = new double[rsBc.Count];
            sumXArray = new double[rsBc.Count];
            sumXYArray = new double[rsBc.Count];
            sumYArray = new double[rsBc.Count];
            sumY2Array = new double[rsBc.Count];
            coef = new double[rsBc.Count][];
            blockCellCount = new int[rsBc.Count];
            difDic = new Dictionary<double, int>[rsBc.Count];
            for (int i = 0; i < rsBc.Count; i++)
            {
                difDic[i] = new Dictionary<double, int>();
            }
            transformRaster = TransformRaster;
            pChange = System.Convert.ToDouble(PercentChange) / 200d;
            rsUtil = rasterUtility;
        }
        private rstPixelType rsType = rstPixelType.PT_UCHAR;
        private rasterUtil rsUtil = null;
        private IRaster referenceRaster = null;
        private IRaster transformRaster = null;
        private IRaster clipRs = null;
        private double pChange;
        public double[][] Coefficients
        {
            get
            {
                return coef;
            }
        }
        public IRaster ReferenceRaster
        {
            get
            {
                return referenceRaster;
            }
            set
            {
                referenceRaster = value;
            }
        }
        public IRaster TransformRaster
        {
            get
            {
                return transformRaster;
            }
            set
            {
                transformRaster = value;
            }
        }
        public double PercentAreaChange
        {
            get
            {
                return pChange;
            }
            set
            {
                pChange = value;
            }
        }
        private IRaster outraster = null;
        public IRaster OutRaster
        {
            get
            {
                if (outraster == null) normalize();
                return outraster;
            }
            set
            {
                outraster = value;
            }
        }
      
        private IRaster normalize()
        {
            getUnChangeCells();
            getRegVals();
            return transform();
        }

        private IRaster transform()
        {
            OutRaster = new RasterClass();
            IRasterBandCollection rsBc = (IRasterBandCollection)OutRaster;
            for (int i = 0; i < coef.Length; i++)
            {
                double[] c = coef[i];
                double intercept = c[0];
                double slope = c[1];
                IRaster tRs = rsUtil.getBand(transformRaster, i);
                IRaster pRs = rsUtil.calcArithmaticFunction(tRs, slope, esriRasterArithmeticOperation.esriRasterMultiply);
                IRaster fRs = rsUtil.calcArithmaticFunction(pRs, intercept, esriRasterArithmeticOperation.esriRasterPlus);
                IRaster bRs = rsUtil.convertToDifFormatFunction(fRs, rsType);
                rsBc.AppendBand(((IRasterBandCollection)bRs).Item(0));
            }
            return OutRaster;
        }
        private int[] blockCellCount = null;
        private double[][] coef = null; // slope coefficients for each band second double array = {intercept,slope,R2}
        private void getRegVals()
        {
            IRaster2 mRs = (IRaster2)rsUtil.clipRasterFunction(referenceRaster, clipGeo, esriRasterClippingType.esriRasterClippingOutside);
            IRaster2 sRs = (IRaster2)rsUtil.clipRasterFunction(transformRaster, clipGeo, esriRasterClippingType.esriRasterClippingOutside);
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(250, 250);
            IRasterCursor mRsCur = mRs.CreateCursorEx(pntSize);
            IRasterCursor sRsCur = sRs.CreateCursorEx(pntSize);
            IRasterCursor cRsCur = ((IRaster2)clipRs).CreateCursorEx(pntSize);
            IPixelBlock mPb, sPb, cPb;
            int bndCnt = minArray.Length;
            //int curCnt = 1;
            do
            {
                mPb = mRsCur.PixelBlock;
                sPb = sRsCur.PixelBlock;
                cPb = cRsCur.PixelBlock;
                for (int r = 0; r < cPb.Height; r+=50)
                {
                    for (int c = 0; c < cPb.Width; c+=50)
                    {
                        for (int p = 0; p < bndCnt; p++)
                        {
                            double minVl = minArray[p];
                            double maxVl = maxArray[p];
                            int bCnt = 0;
                            double ySumVl = 0;
                            double xSumVl = 0;
                            int adw = (cPb.Width-c);
                            int adh = (cPb.Height - r);
                            if (adw > 50) adw = 50;
                            if (adh > 50) adh = 50;
                            for (int br = 0; br < adh; br++)
                            {
                                for (int bc = 0; bc < adw; bc++)
                                {
                                    int c2 = c + bc;
                                    int r2 = r + br;
                                    object vlObj = cPb.GetVal(p, c2, r2);
                                    if (vlObj == null)
                                    {
                                        //Console.WriteLine("Clip Not a number");
                                        continue;
                                    }
                                    else
                                    {
                                        double vl = System.Convert.ToDouble(vlObj);
                                        if (vl <= maxVl && vl >= minVl)
                                        {

                                            object mVlObj = mPb.GetVal(p, c2, r2);
                                            object sVlObj = sPb.GetVal(p, c2, r2);
                                            if (mVlObj == null || sVlObj == null)
                                            {
                                                //Console.WriteLine("master or slave is null");
                                                continue;
                                            }
                                            else
                                            {
                                                //Console.WriteLine(mVlObj.ToString() + ", " + sVlObj.ToString());
                                                ySumVl += System.Convert.ToDouble(mVlObj);
                                                xSumVl += System.Convert.ToDouble(sVlObj);
                                                bCnt += 1;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if (bCnt == 0) continue;
                            else
                            {
                                double yBlock = ySumVl / bCnt;
                                double xBlock = xSumVl / bCnt;
                                //Console.WriteLine(yBlock.ToString() + ", " + xBlock.ToString());
                                sumYArray[p] = sumYArray[p] + yBlock;
                                sumXArray[p] = sumXArray[p] + xBlock;
                                sumXYArray[p] = sumXYArray[p] + (yBlock * xBlock);
                                sumX2Array[p] = sumX2Array[p] + (xBlock * xBlock);
                                sumY2Array[p] = sumY2Array[p] + (yBlock * yBlock);
                                blockCellCount[p] = blockCellCount[p]+1;
                            }
                        }
                        
                    }
                }
                mRsCur.Next();
                sRsCur.Next();
                //Console.WriteLine(curCnt.ToString());
                //curCnt++;
            } while (cRsCur.Next() == true);
            for (int i = 0; i < bndCnt; i++)
            {
                double n = System.Convert.ToDouble(blockCellCount[i]);
                double meanX = sumXArray[i]/n;
                double meanY = sumYArray[i]/n;
                //double meanX2 = sumX2Array[i]/n;
                //double meanXY = sumXYArray[i]/n;
                //Console.WriteLine("numb of cells = " + n.ToString());
                //Console.WriteLine(meanX.ToString() + ", " + meanY.ToString() + ", " + meanX2.ToString() + ", " + meanXY.ToString());
                double slope = (n * sumXYArray[i] - (sumXArray[i] * sumYArray[i])) / (n * sumX2Array[i] - (System.Math.Pow(sumXArray[i], 2)));
                double intercept = meanY-(slope*meanX);
                double r2 = System.Math.Pow((n * sumXYArray[i] - (sumXArray[i] * sumYArray[i])) / (System.Math.Sqrt((n * sumX2Array[i] - (System.Math.Pow(sumXArray[i], 2))))*System.Math.Sqrt(n*sumY2Array[i] - System.Math.Pow(sumYArray[i],2))),2);
                //Console.WriteLine("Intercept and Slope = " + intercept.ToString() + ", " + slope.ToString());
                coef[i] = new double[3]{intercept,slope,r2};
            }
        }
        private double[] minArray = null;
        private double[] maxArray = null;
        private double[] sumXArray = null;
        private double[] sumYArray = null;
        private double[] sumXYArray = null;
        private double[] sumX2Array = null;
        private double[] sumY2Array = null;
        private Dictionary<double,int>[] difDic = null;
        private int[] cellCount = null;
        private IGeometry clipGeo = null;
        private void getUnChangeCells()
        {
            
            IRasterProps rsProp = (IRasterProps)transformRaster;
            IPnt cPnt = rsProp.MeanCellSize();
            IEnvelope env1 = ((IGeoDataset)referenceRaster).Extent;
            IEnvelope env2 = ((IGeoDataset)transformRaster).Extent;
            env1.Intersect(env2);
            clipGeo = (IGeometry)env1;
            IRaster minRs = rsUtil.calcArithmaticFunction(referenceRaster, transformRaster, esriRasterArithmeticOperation.esriRasterMinus);
            clipRs = rsUtil.clipRasterFunction(minRs,clipGeo,esriRasterClippingType.esriRasterClippingOutside);
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(512, 512);
            IRasterProps clipRsProps = (IRasterProps)clipRs;
            IRasterCursor rsCur = ((IRaster2)clipRs).CreateCursorEx(pntSize);
            int pCnt = rsCur.PixelBlock.Planes;
            do
            {
                IPixelBlock pbMinBlock = rsCur.PixelBlock;
                for (int r = 0; r < pbMinBlock.Height; r++)
			    {
                    for (int c = 0; c < pbMinBlock.Width; c++)
                    {
                        for (int p = 0; p < pCnt; p++)
                        {
                            object vlObj = pbMinBlock.GetVal(p, c, r);
                            if (vlObj == null)
                            {
                                continue;
                            }
                            else
                            {
                                double vl = System.Convert.ToDouble(vlObj);
                                Dictionary<double, int> cDic = difDic[p];
                                int cnt = 0;
                                if (!cDic.TryGetValue(vl, out cnt))
                                {
                                    cDic.Add(vl, 1);
                                }
                                else
                                {
                                    cDic[vl] = cnt + 1;
                                }
                                cellCount[p] += 1;
                            }
                        }
                    }
			    }
            } while (rsCur.Next() == true);
            for (int p = 0; p < pCnt; p++)
            {
                int cCnt = cellCount[p];
                int cutOffCnt = System.Convert.ToInt32(pChange * cCnt);
                Dictionary<double,int> cDic = difDic[p];
                List<double> kSort = cDic.Keys.ToList();
                kSort.Sort();
                int kSortLeng = kSort.Count;
                int lCnt = 0;
                double minDif = 0;
                double maxDif = 0;
                for (int i = 0; i < kSortLeng; i++)
                {
                    double minVl = kSort[i];
                    lCnt += cDic[minVl];
                    if (lCnt > cutOffCnt)
                    {
                        minDif = minVl;
                        break;
                    }
                }
                lCnt = 0;
                for (int i = kSortLeng-1; i >= 0; i--)
                {
                    double maxVl = kSort[i];
                    lCnt += cDic[maxVl];
                    if (lCnt > cutOffCnt)
                    {
                        maxDif = maxVl;
                        break;
                    }
                }
                //Console.WriteLine("mindif, maxdif(" + p.ToString() + ") = " + minDif.ToString() + ", " + maxDif.ToString());
                //Console.WriteLine("maxdif(" + p.ToString() + ") = " + maxDif.ToString());
                minArray[p] = minDif;
                maxArray[p] = maxDif;
            }


        }
        
    }
}
