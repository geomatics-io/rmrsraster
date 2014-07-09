using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public class glcmHelperBase
    {
        private rasterUtil rsUtil = null;
        public rasterUtil RasterUtility { get { return rsUtil; } set { rsUtil = value; } }
        public IFunctionRasterDataset InRaster { get; set; }
        private int sumClms, sumRws;
        public int SumClms
        {
            get
            {
                if (Horizontal)
                {
                    sumClms = clms - 1;
                }
                else
                {
                    sumClms = clms;
                }
                return sumClms;
            }
        }
        public int SumRws
        {
            get
            {
                if (Horizontal)
                {
                    sumRws = rws;
                }
                else
                {
                    sumRws = rws-1;
                }
                return sumRws;
            }
        }
        private int clms = 3;
        private int rws = 3;
        private int radius = 2;
        private bool horz = true;
        private rasterUtil.windowType windowtype = rasterUtil.windowType.RECTANGLE;
        public rasterUtil.windowType WindowType { get { return windowtype; } set { windowtype = value; } }
        public bool Horizontal 
        { 
            get 
            { 
                return horz; 
            } 
            set 
            { 
                horz = value;
            } 
        }
        public int Columns { get { return clms; } set { clms = value; } }
        public int Rows { get { return rws; } set { rws = value; } }
        public int Radius 
        { 
            get 
            { 
                return radius; 
            } 
            set 
            { 
                radius = value;
                clms = ((radius - 1) * 2) + 1;
                rws = clms;
            } 
        }
        private IFunctionRasterDataset outraster = null;
        public IFunctionRasterDataset OutRaster { get { return outraster; } set { outraster = value; } }
        public rasterUtil.glcmMetric GlCM_Metric {get;set;}
        public IFunctionRasterDataset calcGLCM()
        {
            if (windowtype == rasterUtil.windowType.RECTANGLE)
            {
                calcRectangleGLCM();
            }
            else
            {
                calcCircleGLCM();
            }
            return OutRaster;
        }

        private void calcCircleGLCM()
        {
            switch (GlCM_Metric)
            {
                case rasterUtil.glcmMetric.CONTRAST:
                case rasterUtil.glcmMetric.DIS:
                case rasterUtil.glcmMetric.HOMOG:
                case rasterUtil.glcmMetric.ASM:
                case rasterUtil.glcmMetric.ENERGY:
                case rasterUtil.glcmMetric.MAXPROB:
                case rasterUtil.glcmMetric.MINPROB:
                case rasterUtil.glcmMetric.RANGE:
                case rasterUtil.glcmMetric.ENTROPY:
                case rasterUtil.glcmMetric.MEAN:
                case rasterUtil.glcmMetric.VAR:
                case rasterUtil.glcmMetric.CORR:
                case rasterUtil.glcmMetric.COV:
                default:
                    makeDefaultGLCMFuntionRaster();
                    break;
            }
        }

        private void calcRectangleGLCM()
        {
            switch (GlCM_Metric)
            {
                case rasterUtil.glcmMetric.CONTRAST:
                    OutRaster = calcContrast();
                    break;
                case rasterUtil.glcmMetric.DIS:
                    OutRaster = calcDis();
                    break;
                case rasterUtil.glcmMetric.HOMOG:
                    OutRaster = calcHomog();
                    break;
                case rasterUtil.glcmMetric.MEAN:
                    OutRaster = calcMean();
                    break;
                case rasterUtil.glcmMetric.VAR:
                    OutRaster = calcVar();
                    break;
                case rasterUtil.glcmMetric.CORR:
                    OutRaster = calcCorr();
                    break;
                case rasterUtil.glcmMetric.COV:
                    OutRaster = calCov();
                    break;
                case rasterUtil.glcmMetric.ASM:
                case rasterUtil.glcmMetric.ENERGY:
                case rasterUtil.glcmMetric.ENTROPY:
                case rasterUtil.glcmMetric.RANGE:
                case rasterUtil.glcmMetric.MINPROB:
                case rasterUtil.glcmMetric.MAXPROB:
                default:
                    OutRaster = makeDefaultGLCMFuntionRaster();
                    break;
            }
        }

        private IFunctionRasterDataset makeDefaultGLCMFuntionRaster()
        {
            if (WindowType == rasterUtil.windowType.RECTANGLE)
            {
                return RasterUtility.calcGLCMFunction(InRaster, Columns, Rows, Horizontal, GlCM_Metric);
            }
            else
            {
                return RasterUtility.calcGLCMFunction(InRaster, Radius, Horizontal, GlCM_Metric);
            }
        }

        private IFunctionRasterDataset calCov()
        {
            double N = (SumRws * SumClms * 2);
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset pRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset pRs2T = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset pRs2 = rsUtil.calcArithmaticFunction(pRs2T, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset mRsT = rsUtil.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset mRsT2 = rsUtil.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset mRs = rsUtil.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IFunctionRasterDataset mRs2 = rsUtil.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset dif = rsUtil.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            return rsUtil.calcArithmaticFunction(dif, N, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IFunctionRasterDataset calcCorr()
        {
            //Cov
            double N = (SumRws * SumClms * 2);
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset pRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset pRs2T = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset pRs2 = rsUtil.calcArithmaticFunction(pRs2T, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            IFunctionRasterDataset mRsT = rsUtil.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset mRsT2 = rsUtil.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset mRs = rsUtil.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IFunctionRasterDataset mRs2 = rsUtil.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset cov = rsUtil.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            //IRaster cov2 = RasterUtility.calcEqualFunction(cov1, 0);
            //IRaster cov = RasterUtility.calcArithmaticFunction(cov2, cov1, esriRasterArithmeticOperation.esriRasterPlus);
            //Var
            IFunctionRasterDataset x2 = rsUtil.calcArithmaticFunction(InRaster, 2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset mvRst2 = getShiftedRaster(x2);
            pRs2 = rsUtil.calcArithmaticFunction(x2, mvRst2, esriRasterArithmeticOperation.esriRasterPlus);
            mRs2 = rsUtil.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset var = rsUtil.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            //IRaster var2 = RasterUtility.calcEqualFunction(var1, 0);
            //IRaster var = RasterUtility.calcArithmaticFunction(var2, var1, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset cor = rsUtil.calcArithmaticFunction(cov, var, esriRasterArithmeticOperation.esriRasterDivide);
            return RasterUtility.setnullToValueFunction(cor, 1);
        }

        private IFunctionRasterDataset calcVar()
        {
            double N = (SumRws * SumClms * 2);
            IFunctionRasterDataset x2 = rsUtil.calcArithmaticFunction(InRaster, 2, esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset mvRst2 = getShiftedRaster(x2);
            IFunctionRasterDataset pRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset pRs2 = rsUtil.calcArithmaticFunction(x2, mvRst2, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset mRsT = rsUtil.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset mRsT2 = rsUtil.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset mRs = rsUtil.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IFunctionRasterDataset mRs2 = rsUtil.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IFunctionRasterDataset dif = rsUtil.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            return rsUtil.calcArithmaticFunction(dif, N, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IFunctionRasterDataset calcMean()
        {
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset pRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset mRs = rsUtil.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.MEAN);
            return rsUtil.calcArithmaticFunction(mRs, 2, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IFunctionRasterDataset calcHomog()
        {
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset difRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IFunctionRasterDataset dif2 = rsUtil.calcArithmaticFunction(difRs,2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset p1 = rsUtil.calcArithmaticFunction(1, dif2, esriRasterArithmeticOperation.esriRasterPlus);
            IFunctionRasterDataset div1 = rsUtil.calcArithmaticFunction(1, p1, esriRasterArithmeticOperation.esriRasterDivide);
            return rsUtil.calcFocalStatisticsFunction(div1, SumClms, SumRws, rasterUtil.focalType.MEAN);
            
        }

        private IFunctionRasterDataset calcDis()
        {
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset difRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IFunctionRasterDataset dif2t = rsUtil.calcArithmaticFunction(difRs, 2,esriRasterArithmeticOperation.esriRasterPower);
            IFunctionRasterDataset dif2 = rsUtil.calcArithmaticFunction(dif2t, 0.5, esriRasterArithmeticOperation.esriRasterPower);
            return rsUtil.calcFocalStatisticsFunction(dif2, SumClms, SumRws, rasterUtil.focalType.MEAN);
        }

        private IFunctionRasterDataset calcContrast()
        {
            IFunctionRasterDataset mvRst = getShiftedRaster(InRaster);
            IFunctionRasterDataset difRs = rsUtil.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IFunctionRasterDataset dif2 = rsUtil.calcArithmaticFunction(difRs,2,esriRasterArithmeticOperation.esriRasterPower);
            return rsUtil.calcFocalStatisticsFunction(dif2, SumClms,SumRws, rasterUtil.focalType.MEAN);
        }
        public IFunctionRasterDataset getShiftedRaster(IFunctionRasterDataset rasterToShift)
        {
            IFunctionRasterDataset mvRst = null;
            double x = 0;
            double y = 0;
            if (Horizontal)
            {
                x = -1 * rasterToShift.RasterInfo.CellSize.X;
            }
            else
            {
                y = rasterToShift.RasterInfo.CellSize.Y;
            }
            mvRst = rsUtil.shiftRasterFunction(rasterToShift, x, y);
            return mvRst;
        }
    }
}
