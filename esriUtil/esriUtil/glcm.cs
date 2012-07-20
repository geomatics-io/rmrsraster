using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;


namespace esriUtil
{
    /// <summary>
    /// a class used to either sample or create gray level co-occurrence metrix values or surfaces.
    /// </summary>
    public class glcm
    {
        /// <summary>
        /// The different GLCM metric types
        /// </summary>
        public enum glcmMetric {CONTRAST,DISSIMILARITY,HOMOGENEITY,ASM,ENERGY,MAXPROBABILITY,MINPROBABILITY,RANGE,ENTROPY,MEAN,VARIANCE,CORRELATION,COVARIANCE}
        /// <summary>
        /// Neighborhood window types Rectangle or Circle
        /// </summary>
        public enum windowType { RECTANGLE, CIRCLE };
        /// <summary>
        /// Surface shift direction
        /// </summary>
        private enum direction { LEFT, RIGHT, UP, DOWN };
        /// <summary>
        /// Default Constructor
        /// </summary>
        public glcm()
        {
            rsUtil = new rasterUtil();
        }
        public glcm(ref rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        /// <summary>
        /// calculates the number of cells within a given circle radius
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        private int calcCircleCells(int radius)
        {
            rad = true;
            List<int[]> iter = new List<int[]>();
            xAr = rsUtil.createFocalWindowCircle(radius, out iter);
            clms = xAr.GetUpperBound(0)+1;
            rws = xAr.GetUpperBound(1)+1;
            int sum = 0;
            int px = 0;
            int cntx = 0;
            for (int x = 0; x < clms;x++ )
            {
                string ln = "";
                for (int y = 0; y < rws; y++)
                {
                    int cD = System.Convert.ToInt16(xAr.GetValue(x, y));
                    if (cD == 1)
                    {
                        px = 1;
                        ln = ln + "1";
                    }
                    else
                    {
                        if (px == 1) cntx = cntx + 1;
                        px = 0;
                        ln = ln + "0";
                    }
                    sum = sum + px;
                }
                //Console.WriteLine(ln);
            }
            windowT = windowType.CIRCLE;
            N = (sum - cntx) * 2;
            prob = System.Convert.ToSingle(1) / N;
            return sum-cntx;
        }
        private int radius = 0;
        /// <summary>
        /// optional the number of cells making up the radius of the circle neighborhood window 
        /// </summary>
        public int RADIUS
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                calcCircleCells(radius);
            }
        }
        /// <summary>
        /// Calculates the total number of observations N given the specifications of the GLCM
        /// </summary>
        private void calcDV()
        {
            windowT = windowType.RECTANGLE;
            if (HORIZONTAL)
            {
                N = (COLUMNS - 1) * ROWS * 2;
                
            }
            else
            {
                N = 2 * COLUMNS * (ROWS - 1);
            }
            prob = System.Convert.ToSingle(1) / N;
        }
        private bool horizontal = true;
        private bool rad = false;
        private int[,] xAr = null;
        private string outRstName = "ProbGLCM";
        /// <summary>
        /// The default name of the probablistic rasters if needed to be created (Read only)
        /// </summary>
        public string OUTRASTERNAME
        {
            get
            {
                return outRstName;
            }
        }
        private IRaster outraster = null;
        public IRaster OUTRASTER { get { return outraster; } }
        /// <summary>
        /// Direction of the GLCM
        /// </summary>
        public bool HORIZONTAL 
        { 
            get 
            { 
                return horizontal;

            } 
            set 
            { 
                horizontal = value;

            } 
        }
        private glcmMetric[] glmc = null;
        /// <summary>
        /// the array of GLCM metrics to calculate
        /// </summary>
        public glcmMetric[] GLCM_METRIC
        {
            get
            {
                return glmc;
            }
            set
            {
                glmc = value;
            }
        }
        private IRaster rst = null;
        private IRasterDataset rstDset = null;
        private IWorkspace wks = null;
        private IRasterProps rstProps = null;
        private IRasterStatistics rstStats = null;
        /// <summary>
        /// the Raster Dataset used to generate the GLCM. If there are multiple bands this will only run the first band within the Raster
        /// </summary>
        public IRaster InRaster
        {
            get
            {
                return rst;
            }
            set
            {

                rst = value;
                rstProps = (IRasterProps)rst;
                rstStats = ((IRasterBandCollection)rst).Item(0).Statistics;
                rstDset = ((IRasterBandCollection)rst).Item(0).RasterDataset;
                wks = ((IDataset)rstDset).Workspace;
            }
        }
        private int N = 6;
        /// <summary>
        /// The number of observations within the GLCM window (Read only)
        /// </summary>
        public int NumCells { get { return N; } }
        private float prob = System.Convert.ToSingle(1) / 6;
        private int clms = 3;
        /// <summary>
        /// the number of columns within the neighborhood window (default 3)
        /// </summary>
        public int COLUMNS
        {
            get
            {
                return clms;

            }
            set
            {
                clms = value;
                calcDV();
            }
        }
        private int rws = 3;
        /// <summary>
        /// the number of rows within the neighborhood windwo (default 3)
        /// </summary>
        public int ROWS
        {
            get
            {
                return rws;
            }
            set
            {
                rws = value;
                calcDV();
            }
        }
        private windowType windowT = windowType.RECTANGLE;
        /// <summary>
        /// the type of neighborhood window being used (Rectangle or Circle). Read only
        /// </summary>
        public windowType WINDOW_TYPE
        {
            get
            {
                return windowT;
            }
        }
        private IRaster createContrast()
        {
            double[] minK2 = createMinPlusArray(true);
            double[] sumK2 = createSumArray();
            IRaster cRst = rsUtil.getBand(InRaster, 0);
            IRaster mRst = rsUtil.convolutionRasterFunction(cRst, 3, 3, minK2);
            IRaster rsSqur = rsUtil.calcArithmaticFunction(mRst, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumRst = rsUtil.convolutionRasterFunction(rsSqur, COLUMNS, ROWS, sumK2);
            return rsUtil.calcArithmaticFunction(sumRst, N / 2, esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createDissimilarity()
        {
            double[] minK2 = createMinPlusArray(true);
            double[] sumK2 = createSumArray();
            IRaster cRstD = rsUtil.getBand(InRaster, 0);
            IRaster mRstD = rsUtil.convolutionRasterFunction(cRstD, 3, 3, minK2);
            IRaster mRsSqr = rsUtil.calcArithmaticFunction(mRstD, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster mRsSqrd = rsUtil.calcArithmaticFunction(mRsSqr, 0.5, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumRst = rsUtil.convolutionRasterFunction(mRsSqrd, COLUMNS, ROWS, sumK2);
            return rsUtil.calcArithmaticFunction(sumRst, (N / 2), esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createHomogeneity()
        {
            double[] minK2 = createMinPlusArray(true);
            double[] sumK2 = createSumArray();
            IRaster hcRst = rsUtil.getBand(InRaster, 0);
            IRaster hmRst = rsUtil.convolutionRasterFunction(hcRst, 3, 3, minK2);
            IRaster hrsSqur = rsUtil.calcArithmaticFunction(hmRst, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster prst = rsUtil.calcArithmaticFunction(hrsSqur, 1, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster invrst = rsUtil.calcArithmaticFunction(prst, -1, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumRst = rsUtil.convolutionRasterFunction(invrst, COLUMNS, ROWS, sumK2);
            return rsUtil.calcArithmaticFunction(sumRst, (N / 2), esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createMean()
        {
            double[] sumK2 = createStatSumArray();
            IRaster mcRst = rsUtil.getBand(InRaster, 0);
            IRaster meanRst = rsUtil.convolutionRasterFunction(mcRst, COLUMNS, ROWS, sumK2);
            return rsUtil.calcArithmaticFunction(meanRst, N, esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createVariance()
        {
            //sum(x) squared
            double[] sumK2 = createStatSumArray();
            IRaster vcRst = rsUtil.getBand(InRaster, 0);
            IRaster vSum = rsUtil.convolutionRasterFunction(vcRst, COLUMNS, ROWS, sumK2);
            //sum(x squared)
            IRaster vSumSquar = rsUtil.calcArithmaticFunction(vSum, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster vSumSquardiv = rsUtil.calcArithmaticFunction(vSumSquar, N, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster xSquar = rsUtil.calcArithmaticFunction(vcRst, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster vSquarSum = rsUtil.convolutionRasterFunction(xSquar, COLUMNS, ROWS, sumK2);
            //subtract the 2 and divide by N
            IRaster sub = rsUtil.calcArithmaticFunction(vSquarSum, vSumSquardiv, esriRasterArithmeticOperation.esriRasterMinus);
            return rsUtil.calcArithmaticFunction(sub, N, esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createCoVariance()
        {
            double[] shift1 = null;
            double[] shift2 = null;
            double[] sumK2 = createSumArray();
            double[] sumStat = createStatSumArray();
            if (HORIZONTAL)
            {
                shift1 = createShiftArray(direction.LEFT);
                shift2 = createShiftArray(direction.RIGHT);
            }
            else
            {
                shift1 = createShiftArray(direction.UP);
                shift2 = createShiftArray(direction.DOWN);
            }
            //sumXY
            IRaster ccRst = rsUtil.getBand(InRaster, 0);
            IRaster y = rsUtil.convolutionRasterFunction(ccRst, 3, 3, shift1);
            IRaster xy = rsUtil.calcArithmaticFunction(ccRst, y, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster sumXY = rsUtil.convolutionRasterFunction(xy, COLUMNS, ROWS, sumK2);
            IRaster sumXYa = rsUtil.convolutionRasterFunction(sumXY, 3, 3, shift2);
            IRaster sumXY2 = rsUtil.calcArithmaticFunction(sumXYa, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            //((sumXsumY)/N)
            IRaster sumXPlusSumY = rsUtil.convolutionRasterFunction(ccRst,COLUMNS,ROWS,sumStat);
            IRaster sumXsumY = rsUtil.calcArithmaticFunction(sumXPlusSumY, 2, esriRasterArithmeticOperation.esriRasterPower);
            IRaster sumXsumYDiv = rsUtil.calcArithmaticFunction(sumXsumY, N, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster dif = rsUtil.calcArithmaticFunction(sumXY2, sumXsumYDiv, esriRasterArithmeticOperation.esriRasterMinus);
            return rsUtil.calcArithmaticFunction(dif, N, esriRasterArithmeticOperation.esriRasterDivide);
        }
        private IRaster createCorrelation()
        {
            IRaster cov = createCoVariance();
            IRaster var = createVariance();
            IRemapFilter rmFilt = new RemapFilterClass();
            rmFilt.AddClass((-1 * 0.000000001), 0.0000000001, 1);
            IRaster rmapV = rsUtil.calcRemapFunction(var, rmFilt);
            IRaster rmapC = rsUtil.calcRemapFunction(cov, rmFilt);
            IRaster corr = rsUtil.calcArithmaticFunction(rmapC, rmapV, esriRasterArithmeticOperation.esriRasterDivide);
            return corr;
        }
        private void createProbBlock(IPixelBlock3 dataPixelBlock, List<glcmMetric> glcmM, ref IPixelBlock3 probPixelBlock)
        {
            int c, r;
            c = dataPixelBlock.Width;
            r = dataPixelBlock.Height;
            int shiftI,shiftJ;
            shiftI = 0;
            shiftJ = 0;
            int halfC = COLUMNS / 2;
            int halfR = ROWS / 2;
            float halfN = System.Convert.ToSingle(N) / 2;
            float tN = System.Convert.ToSingle(N);
            if (HORIZONTAL == true)
            {
                shiftI = 1;
            }
            else
            {
                shiftJ = 1;
            }
            #region set arrays
            System.Array dArr = (System.Array)dataPixelBlock.get_PixelData(0);
            System.Array asmArr = null;
            System.Array maxArr = null;
            System.Array minArr = null;
            System.Array lnArr = null;
            System.Array engArr = null;
            System.Array rangeArr = null;
            if (glcmM.Contains(glcmMetric.ASM))
            {
                asmArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.ASM));
            }
            if (glcmM.Contains(glcmMetric.MAXPROBABILITY))
            {
                maxArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.MAXPROBABILITY));
            }
            if (glcmM.Contains(glcmMetric.MINPROBABILITY))
            {
                minArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.MINPROBABILITY));
            }
            if (glcmM.Contains(glcmMetric.ENTROPY))
            {
                lnArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.ENTROPY));
            }
            if (glcmM.Contains(glcmMetric.ENERGY))
            {
                engArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.ENERGY));
            }
            if (glcmM.Contains(glcmMetric.RANGE))
            {
                rangeArr = (System.Array)probPixelBlock.get_PixelData(glcmM.IndexOf(glcmMetric.RANGE));
            }
            # endregion
            for (int i = 0; i < (c-COLUMNS); i++)
            {
                for (int j = 0; j < (r-ROWS); j++)
                {
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    for (int i2 = 0; i2 < COLUMNS-shiftI; i2++)
                    {
                        
                        for (int j2 = 0; j2 < ROWS-shiftJ; j2++)
                        {
                            if (rad==true)
                            {
                                int rV1 = System.Convert.ToInt32(xAr.GetValue(i2, j2));
                                int rV2 = System.Convert.ToInt32(xAr.GetValue(i2 + shiftI, j2 + shiftJ));
                                if (rV1 == 0 || rV2 == 0)
                                {
                                    continue;
                                }
                            }
                            int ia, ja, ias, jas;
                            int cnt = 1;
                            ia = i + i2;
                            ja = j + j2;
                            ias = ia+shiftI;
                            jas = ja+shiftJ;
                            int vl1 = System.Convert.ToInt32(dArr.GetValue(ia, ja));
                            int vl2 = System.Convert.ToInt32(dArr.GetValue(ias, jas));
                            string k = vl1.ToString() + ";" + vl2.ToString();
                            int dicVl = 0;
                            if (vl1 == vl2)
                            {
                                cnt = 2;                               
                            }
                            else
                            {
                                string k2 = vl2.ToString() + ";" + vl1.ToString();
                                if (dic.TryGetValue(k2, out dicVl))
                                {
                                    dic[k2] = cnt + dicVl;
                                }
                                else
                                {
                                    dic.Add(k2, cnt);
                                }
                            }
                            if (dic.TryGetValue(k, out dicVl))
                            {
                                dic[k] = cnt + dicVl;
                            }
                            else
                            {
                                dic.Add(k, cnt);
                            }


                        }
                    }
                    float sumSqurV = 0;
                    float sumLnV = 0;
                    float maxP = 0;
                    float minP = 1;
                    foreach (int v in dic.Values)
                    {
                        float vl = v * prob;
                        if (vl > maxP) maxP = vl;
                        if (vl < minP) minP = vl;
                        sumSqurV = (sumSqurV + (vl * vl));
                        sumLnV = (sumLnV + System.Convert.ToSingle(vl*-1*Math.Log(vl)));
                    }
                    try
                    {
                        if (asmArr != null) asmArr.SetValue(sumSqurV, i, j);
                        if (lnArr != null) lnArr.SetValue(sumLnV, i, j);
                        if (maxArr != null) maxArr.SetValue(maxP, i, j);
                        if (minArr != null) minArr.SetValue(minP, i, j);
                        if (rangeArr != null) rangeArr.SetValue((maxP - minP), i, j);
                        if (engArr != null) engArr.SetValue(System.Convert.ToSingle(Math.Sqrt(sumSqurV)), i, j);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error:\nc = " + j.ToString() + " r = " + i.ToString());
                        Console.WriteLine(e.ToString());
                        return;
                    }
                }
            }
            if (asmArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.ASM), asmArr);
            if (lnArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.ENTROPY), lnArr);
            if (maxArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.MAXPROBABILITY), maxArr);
            if (minArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.MINPROBABILITY), minArr);
            if (rangeArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.RANGE), rangeArr);
            if (engArr != null) probPixelBlock.set_PixelData(glcmM.IndexOf(glcmMetric.ENERGY), engArr);
            return;
        }
        private IRaster createProbability(out List<glcmMetric> pGlcm)
        {
            IRaster cir = rsUtil.getBand(InRaster, 0);
            int r, c;
            c = rstProps.Width;
            r = rstProps.Height;
            IRasterDataset3 rstDset3 = (IRasterDataset3)rstDset;
            IPnt pntSizeR = new PntClass();
            IPnt pntSizeW = new PntClass();
            IPnt pntLocR = new PntClass();
            IPnt pntLocW = new PntClass();
            int hC = (COLUMNS+1) / 2;
            int hR = (ROWS+1) / 2;
            pGlcm = new List<glcmMetric>();
            #region set arrays and lists
            if (GLCM_METRIC.Contains(glcmMetric.ASM) || GLCM_METRIC.Contains(glcmMetric.ENERGY))
            {
                pGlcm.Add(glcmMetric.ASM);
            }
            if (GLCM_METRIC.Contains(glcmMetric.MAXPROBABILITY) || GLCM_METRIC.Contains(glcmMetric.RANGE))
            {
                pGlcm.Add(glcmMetric.MAXPROBABILITY);
            }
            if (GLCM_METRIC.Contains(glcmMetric.MINPROBABILITY) || GLCM_METRIC.Contains(glcmMetric.RANGE))
            {
                pGlcm.Add(glcmMetric.MINPROBABILITY);
            }
            if (GLCM_METRIC.Contains(glcmMetric.ENTROPY))
            {
                pGlcm.Add(glcmMetric.ENTROPY);
            }
            if (GLCM_METRIC.Contains(glcmMetric.ENERGY))
            {
                pGlcm.Add(glcmMetric.ENERGY);
            }
            if (GLCM_METRIC.Contains(glcmMetric.RANGE))
            {
                pGlcm.Add(glcmMetric.RANGE);
            }
            #endregion
            string outN = OUTRASTERNAME + "_prob";
            if (outN.Length > 12) outN = outN.Substring(outN.Length - 12);
            IRaster nrs = rsUtil.createNewRaster((IRaster)cir, wks, outN, pGlcm.Count, rstPixelType.PT_FLOAT);
            IRasterEdit nrsE = (IRasterEdit)nrs;
            int rSx, rSy, wSx, wSy, rLx, rLy, wLx, wLy;
            wSx = 512;
            wSy = 512;
            rSx = wSx + COLUMNS;
            rSy = wSy + ROWS;
            int xEnd = c - rSx;
            int yEnd = r - rSy;
            rSx = wSx + COLUMNS;
            pntSizeR.SetCoords(rSx, rSy);
            pntSizeW.SetCoords(wSx, wSy);
            int hC1 = hC-1;
            int hR1 = hR-1;
            IPixelBlock3 pBlock = (IPixelBlock3)cir.CreatePixelBlock(pntSizeR);
            IPixelBlock3 pBlockN = (IPixelBlock3)nrs.CreatePixelBlock(pntSizeW);
            for (int i = 0 - hC; i < c; i += wSx)
            {
                rLx = i;
                wLx = i + hC1;
                for (int j = 0 - hR; j < r; j += wSy)
                {
                    rLy = j;
                    wLy = j + hR1;
                    pntLocR.SetCoords(rLx, rLy);
                    pntLocW.SetCoords(wLx, wLy);
                    cir.Read(pntLocR, (IPixelBlock)pBlock);
                    createProbBlock(pBlock, pGlcm, ref pBlockN);
                    nrsE.Write(pntLocW, (IPixelBlock)pBlockN);
                }
            }            
            nrsE.Refresh();
            rsUtil.calcStatsAndHist(((IRaster2)nrs).RasterDataset);
            return (IRaster)nrs;
        }
        private double[] createShiftArray(direction dir)
        {
            List<double> rLst = new List<double>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double vl = 0;
                    switch (dir)
                    {
                        case direction.LEFT:
                            if (j == 1 && i == 2) vl = 1;
                            break;
                        case direction.RIGHT:
                            if (j == 1 && i == 0) vl = 1;
                            break;
                        case direction.UP:
                            if (j == 2 && i == 1) vl = 1;
                            break;
                        case direction.DOWN:
                            if (j == 0 && i == 1) vl = 1;
                            break;
                        default:
                            break;
                    }
                    
                    rLst.Add(vl);
                }
            }
            return rLst.ToArray();
        }
        private double[] createMinPlusArray(bool minus)
        {
            double x = 1;
            if (minus==true)
            {
                x = -1;
            }
            List<double> rLst = new List<double>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double vl = 0;
                    if (i == 1 && j == 1)
                    {
                        vl = 1;
                    }
                    if (HORIZONTAL==true)
                    {
                        if (i == 0 && j == 1)
                        {
                            vl = x;
                        }

                    }
                    else
                    {
                        if (j == 0 && i==1)
                        {
                            vl = x;
                        }
                    }
                    rLst.Add(vl);
                }
            }
            return rLst.ToArray();
        }
        private double[] createSumArray()
        {
            List<double> rLst = new List<double>();
            for (int i = 0; i < COLUMNS; i++)
            {
                for (int j = 0; j < ROWS; j++)
                {
                    double vl = 1;
                    if (HORIZONTAL==true)
                    {
                        if (i == 0)
                        {
                            vl = 0;
                        }
                        else
                        {
                            if (rad == true)
                            {
                                int v1 = System.Convert.ToInt32(xAr.GetValue(i, j));
                                int v2 = System.Convert.ToInt32(xAr.GetValue(i - 1, j));
                                if (v1 == 0 || v2 == 0)
                                {
                                    vl = 0;
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        if (j == 0)
                        {
                            vl = 0;
                        }
                        else
                        {
                            if (rad == true)
                            {
                                int v1 = System.Convert.ToInt32(xAr.GetValue(i, j));
                                int v2 = System.Convert.ToInt32(xAr.GetValue(i, j-1));
                                if (v1 == 0 || v2 == 0)
                                {
                                    vl = 0;
                                }
                            }
                        }
                    }
                    rLst.Add(vl);
                }
            }
            return rLst.ToArray();
        }
        private double[] createStatSumArray()
        {
            List<double> rLst = new List<double>();
            for (int i = 0; i < COLUMNS; i++)
            {
                for (int j = 0; j < ROWS; j++)
                {
                    double vl = 2;
                    if (HORIZONTAL==true)
                    {
                        if (i == 0||i==(COLUMNS-1))
                        {
                            vl = 1;
                        }
                        if (rad == true)
                        {
                            int aV = -1;
                            if (i == 0) aV = 1;
                            int v1 = System.Convert.ToInt32(xAr.GetValue(i, j));
                            int v2 = System.Convert.ToInt32(xAr.GetValue(i + aV, j));
                            if (v1 == 0 || v2 == 0)
                            {
                                vl = 0;
                            }
                        }
                    }
                    else
                    {
                        if (j == 0 || j == (ROWS - 1))
                        {
                            vl = 1;
                        }
                        if (rad == true)
                        {
                            int aV = -1;
                            if (i == 0) aV = 1;
                            int v1 = System.Convert.ToInt32(xAr.GetValue(i, j));
                            int v2 = System.Convert.ToInt32(xAr.GetValue(i, j + aV));
                            if (v1 == 0 || v2 == 0)
                            {
                                vl = 0;
                            }
                        }
                    }
                    rLst.Add(vl);
                }
            }
            return rLst.ToArray();
        }
        /// <summary>
        /// Creates the texture rasters specified. Requires the following to be specified; InRaster, Radius, or Rows, Columns, and GLCM_METRICS.
        /// If GLCM_METRICS asm, entropy, energy, minProb, maxProb, or Range then a perminant raster will automatically be created. Otherwise the 
        /// output will be a IRaster. To persist the output you must save it to a perminant dataset (rsUtility.saveRasterToDataset() method).
        /// </summary>
        /// <returns></returns>
        public void createTexture()
        {
            List<glcmMetric> glcmLst = GLCM_METRIC.ToList();
            IRasterBandCollection rstOut = new RasterClass();
            IRasterBand rsB = null;
            IRaster dRst = null;
            bool cP = false;
            foreach (glcmMetric m in GLCM_METRIC)
            {
                //Console.WriteLine("Calculating metric " + m.ToString() + " sample size = " + N.ToString());
                switch (m)
                {
                    case glcmMetric.CONTRAST:
                        dRst = createContrast();
                        break;
                    case glcmMetric.DISSIMILARITY:
                        dRst = createDissimilarity();
                        break;
                    case glcmMetric.HOMOGENEITY:
                        dRst = createHomogeneity();
                        break;
                    case glcmMetric.ASM:
                    case glcmMetric.ENERGY:
                    case glcmMetric.ENTROPY:
                    case glcmMetric.MAXPROBABILITY:
                    case glcmMetric.MINPROBABILITY:
                    case glcmMetric.RANGE:
                        dRst = null;
                        cP = true;
                        break;
                    case glcmMetric.MEAN:
                        dRst = createMean();
                        break;
                    case glcmMetric.VARIANCE:
                        dRst = createVariance();
                        break;
                    case glcmMetric.CORRELATION:
                        dRst = createCorrelation();
                        break;
                    case glcmMetric.COVARIANCE:
                        dRst = createCoVariance();
                        break;
                    default:
                        break;
                }

                if (dRst != null)
                {
                    rsB = ((IRasterBandCollection)dRst).Item(0);
                    rstOut.Add(rsB, glcmLst.IndexOf(m));
                } 
            }
            if (cP==true)
            {
                List<glcmMetric> pGlcm;
                IRaster p = createProbability(out pGlcm);
                IRasterBandCollection rsBC = (IRasterBandCollection)p;
                foreach (glcmMetric g in pGlcm)
                {
                    IRasterBand rsb = rsBC.Item(pGlcm.IndexOf(g));
                    int i = glcmLst.IndexOf(g);
                    if (i > -1)
                    {
                        rstOut.Add(rsb, i);
                    }
                }
            }
            outraster = (IRaster)rstOut;
        }
        private Dictionary<glcmMetric, double> getMetricValues(Dictionary<string, int> freqCombo)
        {
            Dictionary<glcmMetric, double> outDic = new Dictionary<glcmMetric, double>();
            double prob = 1.0 / NumCells;
            double contrast, dissimilarity, homogeneity, asm, energy, entropy, variance, correlation, mean, maxprob, minprob;
            contrast = 0;
            dissimilarity = 0;
            homogeneity = 0;
            asm = 0;
            energy = 0;
            entropy = 0;
            variance = 0;
            correlation = 0;
            mean = 0;
            maxprob = 0;
            minprob = 1;
            double sumVl1 = 0;
            double sumSqrVl1 = 0;
            double sumXY = 0;
            double cov = 0;
            int tCnt = 0;
            foreach (KeyValuePair<string, int> kVp in freqCombo)
            {
                string ky = kVp.Key;
                int cnt = kVp.Value;
                tCnt = tCnt + cnt;
                double mlt = prob * cnt;
                double vl1, vl2, distance, distSqr;
                string[] kySplit = ky.Split(new char[] { ';' });
                vl1 = System.Convert.ToDouble(kySplit[0]);
                sumVl1 = sumVl1+(vl1 * cnt);
                sumSqrVl1 = sumSqrVl1 + (Math.Pow(vl1,2) * cnt);
                vl2 = System.Convert.ToDouble(kySplit[1]);
                sumXY = sumXY + (vl1*vl2*cnt);
                distance = Math.Abs(vl1 - vl2);
                distSqr = Math.Pow(distance, 2);
                contrast = contrast + (mlt * distSqr);
                dissimilarity = dissimilarity + (mlt * distance);
                homogeneity = homogeneity + (mlt / (1 + distSqr));
                asm = asm + Math.Pow((mlt), 2);
                entropy = entropy + mlt * (-1 * Math.Log(mlt));
                mean = mean + mlt * vl1;
                if (mlt < minprob) minprob = mlt;
                if (maxprob < mlt) maxprob = mlt;
            }
            variance = (sumSqrVl1 - (Math.Pow(sumVl1, 2) / tCnt)) / tCnt;
            cov = (sumXY - ((sumVl1 * sumVl1) / tCnt)) / tCnt;
            correlation = cov / Math.Pow(Math.Sqrt(variance),2);
            energy = Math.Sqrt(asm);
            //Console.WriteLine("\tv = " + variance.ToString() + " cov = " + cov.ToString() + " cor = " + correlation.ToString());
            outDic[glcmMetric.CONTRAST] = contrast;
            outDic[glcmMetric.DISSIMILARITY] = dissimilarity;
            outDic[glcmMetric.HOMOGENEITY] = homogeneity;
            outDic[glcmMetric.ASM] = asm;
            outDic[glcmMetric.ENERGY] = energy;
            outDic[glcmMetric.ENTROPY] = entropy;
            outDic[glcmMetric.MEAN] = mean;
            outDic[glcmMetric.VARIANCE] = variance;
            outDic[glcmMetric.CORRELATION] = correlation;
            outDic[glcmMetric.MAXPROBABILITY] = maxprob;
            outDic[glcmMetric.MINPROBABILITY] = minprob;
            outDic[glcmMetric.RANGE] = maxprob-minprob;
            return outDic;
        }
        /// <summary>
        /// Samples the GLCM texture for a given set of point locations specfied witihin the inPointFeature. This method can be used
        /// to create a sample set without specifically creating the GLCM surfaces. Metric values are stored within the inPointFeature Class
        /// and are named after the direction of the metric and the meteric name itself. Requires the following to be specified
        /// Requires the following to be specified; InRaster, Radius, or Rows, Columns, and GLCM_METRICS. No surface will be created using this
        /// method.
        /// </summary>
        /// <param name="inPointFeature"> the point feature class used to sample the metrics. This can be a path string to a featureclass or a IFeatureClass object </param>
        /// <returns>a string array of metric values sampled. These names correspond to the names of the field within the featureclass that store the 
        /// metric values for each record within the inPointFeature</returns>
        public string[] sampleTexture(object inPointFeature)
        {
            List<string> outLst = new List<string>();
            glcmMetric[] metrics = GLCM_METRIC;
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            IFeatureClass inFtr = geoUtil.returnFeatureClass(inPointFeature);
            string f = "";
            foreach(glcmMetric m in metrics)
            {
                f = "H_";
                if (!HORIZONTAL)
                {
                    f = "V_";
                }
                string nm = f + m.ToString();
                nm = geoUtil.createField(inFtr,nm,esriFieldType.esriFieldTypeDouble);
                outLst.Add(nm);
            }
            IRaster2 inRst = (IRaster2)InRaster;
            IRasterProps rsP = (IRasterProps)inRst;
            IGeometry rGeo = (IGeometry)rsP.Extent;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = rGeo;
            spFlt.GeometryField = inFtr.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor sCur = inFtr.Search(spFlt,false);
            IFeature sRow = sCur.NextFeature();
            IPnt pntSize = new PntClass();
            pntSize.X = COLUMNS;
            pntSize.Y = ROWS;
            int pX,pY; //location of neighbor relative to current location
            if (HORIZONTAL)
            {
                pX = 1;
                pY = 0;
            }
            else
            {
                pX = 0;
                pY = 1;
            }
            int hR1, hC1;
            hR1 = ((ROWS+1) / 2) - 1;
            hC1 = ((COLUMNS + 1) / 2) - 1;
            int eC, eR;
            eC = COLUMNS - pX;
            eR = ROWS - pY;
            IRaster ir = (IRaster)inRst;
            IPixelBlock pb = ir.CreatePixelBlock(pntSize);
            IPnt pntLoc = new PntClass();
            float halfN = System.Convert.ToSingle(N) / 2;
            float tN = System.Convert.ToSingle(N);
            while (sRow!=null)
            {

                IGeometry geo = sRow.Shape;
                IPoint pnt = (IPoint)geo;
                double x = pnt.X;
                double y = pnt.Y;
                int clm,rw;
                inRst.MapToPixel(x,y,out clm,out rw);
                int xLoc, yLoc;
                xLoc = clm - hC1;
                yLoc = rw - hR1;
                pntLoc.X = xLoc;
                pntLoc.Y = yLoc;
                ir.Read(pntLoc, pb);
                System.Array pbArr = (System.Array)((IPixelBlock3)pb).get_PixelData(0);
                int cnt = 1;
                float cont = 0;
                float dis = 0;
                float hom = 0;
                float mean = 0;
                float sumX = 0;
                float sumY = 0;
                float sumXY = 0;
                float sumXSquar = 0;
                float sumYSquar = 0;
                float var = 0;
                float cov = 0;
                float corr = 0;
                Dictionary<string, int> dic = new Dictionary<string, int>();
                for(int c = 0;c<eC;c++)
                {
                    for(int r = 0;r<eR;r++)
                    {
                        if (rad)
                        {
                            int rv1 = System.Convert.ToInt32(xAr.GetValue(c, r));
                            int rv2 = System.Convert.ToInt32(xAr.GetValue((c + pX), (r + pY)));
                            if (rv1 == 0 || rv2 == 0)
                            {
                                continue;
                            }

                        }
                        int vl1 = System.Convert.ToInt32(pbArr.GetValue(c, r));
                        int vl2 = System.Convert.ToInt32(pbArr.GetValue(c+pX, r+pY));
                        float dif = System.Convert.ToSingle(Math.Abs(vl1 - vl2));
                        float difSquar = dif * dif;
                        dis = dis + dif;
                        cont = cont + difSquar;
                        hom = hom + (1 / (1 + difSquar));
                        sumX = sumX + vl1;
                        sumY = sumY + vl2;
                        sumXY = sumXY + (vl1 * vl2);
                        sumXSquar = sumXSquar + (vl1 * vl1);
                        sumYSquar = sumYSquar + (vl2 * vl2);
                        string k = vl1.ToString() + ";" + vl2.ToString();
                        cnt = 1;
                        int dicVl = 0;
                        if (vl1 == vl2)
                        {
                            cnt = 2;                               
                        }
                        else
                        {
                            string k2 = vl2.ToString() + ";" + vl1.ToString();
                            if (dic.TryGetValue(k2, out dicVl))
                            {
                                dic[k2] = cnt + dicVl;
                            }
                            else
                            {
                                dic.Add(k2, cnt);
                            }
                        }
                        if (dic.TryGetValue(k, out dicVl))
                        {
                            dic[k] = cnt + dicVl;
                        }
                        else
                        {
                            dic.Add(k, cnt);
                        }


                    }
                }
                dis = dis / halfN;
                cont = cont / halfN;
                hom = hom / halfN;
                float sumXPsumYSquarDivN = System.Convert.ToSingle(Math.Pow((sumX + sumY), 2)) / tN;
                var = ((sumXSquar + sumYSquar) - (sumXPsumYSquarDivN)) / tN;
                cov = ((sumXY * 2) - sumXPsumYSquarDivN) / tN;
                if (var == 0)
                {
                    var = 0.000001F;
                }
                if (cov == 0)
                {
                    cov = 0.000001F;
                }
                corr = cov / var;
                mean = (sumX + sumY) / tN;
                float sumSqurV = 0;
                float sumLnV = 0;
                float maxP = 0;
                float minP = 1;
                foreach (int v in dic.Values)
                {
                    float vl = v * prob;
                    if (vl > maxP) maxP = vl;
                    if (vl < minP) minP = vl;
                    sumSqurV = (sumSqurV + (vl * vl));
                    sumLnV = (sumLnV + System.Convert.ToSingle(vl*-1*Math.Log(vl)));
                }
                foreach (string s in outLst)
                {
                    int fldIndex = sRow.Fields.FindField(s);
                    if (s.ToUpper().EndsWith("CONTRAST")) sRow.set_Value(fldIndex, cont);
                    if (s.ToUpper().EndsWith("ASM")) sRow.set_Value(fldIndex, sumSqurV);
                    if (s.ToUpper().EndsWith("CORRELATION")) sRow.set_Value(fldIndex, corr);
                    if (s.ToUpper().EndsWith("COVARIANCE")) sRow.set_Value(fldIndex, cov);
                    if (s.ToUpper().EndsWith("DISSIMILARITY")) sRow.set_Value(fldIndex, dis);
                    if (s.ToUpper().EndsWith("ENERGY")) sRow.set_Value(fldIndex, System.Convert.ToSingle(Math.Sqrt(sumSqurV)));
                    if (s.ToUpper().EndsWith("ENTROPY")) sRow.set_Value(fldIndex, sumLnV);
                    if (s.ToUpper().EndsWith("HOMOGENEITY")) sRow.set_Value(fldIndex, hom);
                    if (s.ToUpper().EndsWith("MAXPROBABILITY")) sRow.set_Value(fldIndex, maxP);
                    if (s.ToUpper().EndsWith("MEAN")) sRow.set_Value(fldIndex, mean);
                    if (s.ToUpper().EndsWith("MINPROBABILITY")) sRow.set_Value(fldIndex, minP);
                    if (s.ToUpper().EndsWith("RANGE")) sRow.set_Value(fldIndex, maxP - minP);
                    if (s.ToUpper().EndsWith("VARIANCE")) sRow.set_Value(fldIndex, var);
                }
                sRow.Store();
                sRow = sCur.NextFeature();
            }
            return outLst.ToArray();
        }
    }
    
    public class glcmNeighborhoodHelper
    {
        /// <summary>
        /// Rectangle constructor for neighborhood helper.
        /// </summary>
        /// <param name="columns">number of columns of the window</param>
        /// <param name="rows">number of rows of the window</param>
        private glcmNeighborhoodHelper(int columns, int rows, int numberOfCells,bool horizontal)
        {
            horz = horizontal;
            clms = columns;
            rws = rows;
            n = numberOfCells;
            setStartupValues();
        }
        /// <summary>
        /// Circle constructor for neighborhood helper.
        /// </summary>
        /// <param name="radius">number of cells that makeup the radius of a circle neighborhood</param>
        private glcmNeighborhoodHelper(int[,] circleArr, int numberOfCells, bool horizontal)
        {
            horz = horizontal;
            rad = true;
            window = circleArr;
            clms = circleArr.GetUpperBound(0) + 1;
            rws = circleArr.GetUpperBound(1) + 1;
            n = numberOfCells;
            setStartupValues();
        }
        private void setStartupValues()
        {
            if (rad)
            {
                for (int i = 0; i < clms; i++)
                {
                    List<int> oneList = new List<int>();
                    for (int j = 0; j < rws; j++)
                    {
                        int vl = window[i, j];
                        if (vl == 1) oneList.Add(j);
                    }
                    addColumns.Add(i, oneList.ToArray());
                }
            }
            matrix = new double[clms, rws];
        }
        private bool horz = true;
        private bool rad = false;
        /// <summary>
        /// stores the position of zeros in the circle window used to subtract from rows and columns 
        /// </summary>
        private Dictionary<int, int[]> addColumns = new Dictionary<int, int[]>();
        private rasterUtil rsUtil = new rasterUtil();
        private int[,] window = null;
        /// <summary>
        /// retreive the current window
        /// </summary>
        public int[,] Window { get { return window; } }
        private int currentClm = 0;
        private int currentRw = 0;
        private int CurrentEditRow
        {
            get
            {
                return currentRw;
            }
            set
            {
                if (value < rws - 1)
                {
                    currentRw = value;
                }
                else
                {
                    currentRw = 0;
                }
            }
        }
        private int CurrentEditColumn
        {
            get
            {
                return currentClm;
            }
            set
            {
                if (value < clms - 1)
                {
                    currentClm = value;
                }
                else
                {
                    currentClm = 0;
                }
            }
        }
        private int clms;
        private int rws;
        private int n;
        private double[,] matrix;
        private Dictionary<string, int> tuniqvalues = new Dictionary<string, int>();
        /// <summary>
        /// gets the number of unique values within the windwo and their associted counts
        /// </summary>
        public Dictionary<string, int> WindowUniqueValuesByCount { get { return tuniqvalues; } }
        private int getWindowColumn(int column)
        {
            int c = CurrentEditColumn + column;
            if (c > clms - 1)
            {
                c = c - clms;
            }
            return c;
        }
        private int getWindowRow(int row)
        {
            int r = CurrentEditRow + row;
            if (r > rws - 1)
            {
                r = r - rws;
            }
            return r;
        }
        /// <summary>
        /// gets the value within the window given a column row
        /// </summary>
        /// <param name="column">int column</param>
        /// <param name="row">int row</param>
        /// <returns>the value at that location</returns>
        public double getValue(int column, int row)
        {
            int c = getWindowColumn(column);
            int r = getWindowRow(row);
            return matrix[c, r];
        }
        /// <summary>
        /// appends a new row to the window
        /// </summary>
        /// <param name="rowArr"></param>
        public void appendRow(double[] rowArr)
        {
            double vl = 0;
            double oldvl = 0;
            int c, r;
            c = 0;
            r = 0;
            for (int i = 0; i < rowArr.Length; i++)
            {
                int cnt;
                int cn, rn;
                c = getWindowColumn(i);
                r = CurrentEditRow;
                vl = rowArr[i];
                oldvl = matrix[c, r];
                oldvl = matrix[c, r];
                string oldlc = oldvl.ToString();
                string vllc = vl.ToString();
                string oldlcN = "";
                string vllcN = "";
                if (horz)
                {
                    cn = c + 1;
                    rn = r;
                    
                }
                else
                {
                    cn = c;
                    rn = r + 1;
                }
                if (cn < clms && rn < rws)
                {
                    #region old values
                    oldlcN = System.Convert.ToInt32(matrix[cn, rn]).ToString();
                    if (horz)
                    {
                        vllcN = oldlcN;
                    }
                    else
                    {
                        vllcN = System.Convert.ToInt32(rowArr[i + 1]).ToString();
                    }
                    string oLC = oldlc + ";" + oldlcN;
                    int cntAdjst = 1;
                    if (oldlc == oldlcN)
                    {
                        cntAdjst = 2;
                    }
                    if (tuniqvalues.TryGetValue(oLC, out cnt))
                    {
                        int ncnt = cnt - cntAdjst;
                        if (ncnt == 0)
                        {
                            tuniqvalues.Remove(oLC);
                        }
                        else
                        {
                            tuniqvalues[oLC] = ncnt;
                        }
                        if (cntAdjst == 1)
                        {
                            oLC = oldlcN+";"+oldlc;
                            if (tuniqvalues.TryGetValue(oLC, out cnt))
                            {
                                ncnt = cnt - cntAdjst;
                                if (ncnt == 0)
                                {
                                    tuniqvalues.Remove(oLC);
                                }
                                else
                                {
                                    tuniqvalues[oLC] = ncnt;
                                }
                            }

                        }
                    }
                    #endregion
                    #region new values
                    string nLC = vllc + ";" + vllcN;
                    cntAdjst = 1;
                    if (vllc == vllcN)
                    {
                        cntAdjst = 2;
                    }
                    if (tuniqvalues.TryGetValue(nLC, out cnt))
                    {
                        tuniqvalues[nLC] = cnt + cntAdjst;
                    }
                    else
                    {
                        tuniqvalues.Add(nLC, cntAdjst);
                    }
                    if (cntAdjst == 1)
                    {
                        nLC = vllcN + ";" + vllc;
                        if (tuniqvalues.TryGetValue(nLC, out cnt))
                        {
                            tuniqvalues[nLC] = cnt + cntAdjst;
                        }
                        else
                        {
                            tuniqvalues.Add(nLC, cntAdjst);
                        }
                    }
                    #endregion
                }
                matrix[c, r] = vl;
            }
            CurrentEditRow += 1;
        }
        /// <summary>
        /// appends a new column to the window
        /// </summary>
        /// <param name="columnArr"></param>
        public void appendColumns(double[] columnArr)
        {
            int c, r;
            c = 0;
            r = 0;
            for (int i = 0; i < columnArr.Length; i++)
            {
                c = CurrentEditColumn;
                r = getWindowRow(i);
                int cn, rn; 
                int cnt;
                double vld = columnArr[i];
                string vl = System.Convert.ToInt32(vld).ToString();
                string vlN;
                string oldvl = System.Convert.ToInt32(matrix[c, r]).ToString();
                string oldvlN;
                if (horz)
                {
                    cn = c + 1;
                    rn = r;
                    
                }
                else
                {
                    cn = c;
                    rn = r + 1;
                }
                if (cn < clms && rn < rws)
                {
                    oldvlN = System.Convert.ToInt32(matrix[cn, rn]).ToString();
                    if (horz)
                    {
                        vlN = oldvlN;
                    }
                    else
                    {
                        vlN = System.Convert.ToInt32(columnArr[i + 1]).ToString();
                    }
                    #region old values
                    string oldlc = oldvl + ";" + oldvlN;
                    int cntAdjust = 1;
                    if (oldvl == oldvlN)
                    {
                        cntAdjust = 2;
                    }
                    if (tuniqvalues.TryGetValue(oldlc, out cnt))
                    {
                        int ncnt = cnt - cntAdjust;
                        if (ncnt == 0)
                        {
                            tuniqvalues.Remove(oldlc);
                        }
                        else
                        {
                            tuniqvalues[oldlc] = ncnt;
                        }
                        if (cntAdjust == 1)
                        {
                            oldlc = oldvlN + ";" + oldvl;
                            if (tuniqvalues.TryGetValue(oldlc, out cnt))
                            {
                                ncnt = cnt - cntAdjust;
                                if (ncnt == 0)
                                {
                                    tuniqvalues.Remove(oldlc);
                                }
                                else
                                {
                                    tuniqvalues[oldlc] = ncnt;
                                }
                            }
                        }

                    }
                    #endregion
                    #region new values
                    string newlc = vl + ";" + vlN;
                    cntAdjust = 1;
                    if (vl == vlN)
                    {
                        cntAdjust = 2;
                    }
                    if (tuniqvalues.TryGetValue(newlc, out cnt))
                    {
                        tuniqvalues[newlc] = cnt + cntAdjust;
                    }
                    else
                    {
                        tuniqvalues.Add(newlc, cntAdjust);
                    }
                    if (cntAdjust == 1)
                    {
                        newlc = vlN + ";" + vl;
                        if (tuniqvalues.TryGetValue(newlc, out cnt))
                        {
                            tuniqvalues[newlc] = cnt + cntAdjust;
                        }
                        else
                        {
                            tuniqvalues.Add(newlc, cntAdjust);
                        }
                    }
                    #endregion
                }
                matrix[c, r] = vld;
            }
            CurrentEditColumn += 1;
        }
    }
}
