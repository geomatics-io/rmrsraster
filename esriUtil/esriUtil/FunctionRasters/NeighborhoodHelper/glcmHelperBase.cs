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
        public IRaster InRaster { get; set; }
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
        private IRaster outraster = null;
        public IRaster OutRaster { get { return RasterUtility.returnRaster(outraster,rstPixelType.PT_FLOAT); } set { outraster = value; } }
        public rasterUtil.glcmMetric GlCM_Metric {get;set;}
        public IRaster calcGLCM()
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

        private IRaster makeDefaultGLCMFuntionRaster()
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

        private IRaster calCov()
        {
            double N = (SumRws * SumClms * 2);
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster pRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster pRs2T = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster pRs2 = RasterUtility.calcArithmaticFunction(pRs2T, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster mRsT = RasterUtility.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster mRsT2 = RasterUtility.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster mRs = RasterUtility.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster mRs2 = RasterUtility.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster dif = RasterUtility.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            return RasterUtility.calcArithmaticFunction(dif, N, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IRaster calcCorr()
        {
            //Cov
            double N = (SumRws * SumClms * 2);
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster pRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster pRs2T = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster pRs2 = RasterUtility.calcArithmaticFunction(pRs2T, 2, esriRasterArithmeticOperation.esriRasterMultiply);
            IRaster mRsT = RasterUtility.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster mRsT2 = RasterUtility.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster mRs = RasterUtility.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster mRs2 = RasterUtility.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster cov = RasterUtility.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            //IRaster cov2 = RasterUtility.calcEqualFunction(cov1, 0);
            //IRaster cov = RasterUtility.calcArithmaticFunction(cov2, cov1, esriRasterArithmeticOperation.esriRasterPlus);
            //Var
            IRaster x2 = RasterUtility.calcArithmaticFunction(InRaster, 2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster mvRst2 = getShiftedRaster(x2);
            pRs2 = RasterUtility.calcArithmaticFunction(x2, mvRst2, esriRasterArithmeticOperation.esriRasterPlus);
            mRs2 = RasterUtility.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster var = RasterUtility.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            //IRaster var2 = RasterUtility.calcEqualFunction(var1, 0);
            //IRaster var = RasterUtility.calcArithmaticFunction(var2, var1, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster cor = RasterUtility.calcArithmaticFunction(cov, var, esriRasterArithmeticOperation.esriRasterDivide);
            return RasterUtility.setnullToValueFunction(cor, 1);
        }

        private IRaster calcVar()
        {
            double N = (SumRws * SumClms * 2);
            IRaster x2 = RasterUtility.calcArithmaticFunction(InRaster,2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster mvRst2 = getShiftedRaster(x2);
            IRaster pRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster pRs2 = RasterUtility.calcArithmaticFunction(x2, mvRst2, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster mRsT = RasterUtility.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster mRsT2 = RasterUtility.calcArithmaticFunction(mRsT,2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster mRs = RasterUtility.calcArithmaticFunction(mRsT2, N, esriRasterArithmeticOperation.esriRasterDivide);
            IRaster mRs2 = RasterUtility.calcFocalStatisticsFunction(pRs2, SumClms, SumRws, rasterUtil.focalType.SUM);
            IRaster dif = RasterUtility.calcArithmaticFunction(mRs2, mRs, esriRasterArithmeticOperation.esriRasterMinus);
            return RasterUtility.calcArithmaticFunction(dif, N, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IRaster calcMean()
        {
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster pRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster mRs = RasterUtility.calcFocalStatisticsFunction(pRs, SumClms, SumRws, rasterUtil.focalType.MEAN);
            return RasterUtility.calcArithmaticFunction(mRs, 2, esriRasterArithmeticOperation.esriRasterDivide);
        }

        private IRaster calcHomog()
        {
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster difRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster dif2 = RasterUtility.calcArithmaticFunction(difRs,2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster p1 = RasterUtility.calcArithmaticFunction(1, dif2, esriRasterArithmeticOperation.esriRasterPlus);
            IRaster div1 = RasterUtility.calcArithmaticFunction(1, p1, esriRasterArithmeticOperation.esriRasterDivide);
            return RasterUtility.calcFocalStatisticsFunction(div1, SumClms, SumRws, rasterUtil.focalType.MEAN);
            
        }

        private IRaster calcDis()
        {
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster difRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster dif2t = RasterUtility.calcArithmaticFunction(difRs, 2,esriRasterArithmeticOperation.esriRasterPower);
            IRaster dif2 = RasterUtility.calcArithmaticFunction(dif2t, 0.5, esriRasterArithmeticOperation.esriRasterPower);
            return RasterUtility.calcFocalStatisticsFunction(dif2, SumClms, SumRws, rasterUtil.focalType.MEAN);
        }

        private IRaster calcContrast()
        {
            IRaster mvRst = getShiftedRaster(InRaster);
            IRaster difRs = RasterUtility.calcArithmaticFunction(InRaster, mvRst, esriRasterArithmeticOperation.esriRasterMinus);
            IRaster dif2 = RasterUtility.calcArithmaticFunction(difRs,2,esriRasterArithmeticOperation.esriRasterPower);
            return RasterUtility.calcFocalStatisticsFunction(dif2, SumClms,SumRws, rasterUtil.focalType.MEAN);
        }
        public IRaster getShiftedRaster(IRaster rasterToShift)
        {
            IRaster mvRst = null;
            if (Horizontal)
            {
                mvRst = rsUtil.shiftRasterFunction(rasterToShift, -1, 0);
            }
            else
            {
                mvRst = rsUtil.shiftRasterFunction(rasterToShift, 0, 1);
            }
            return mvRst;
        }
    }
}
