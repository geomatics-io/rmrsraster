using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;


namespace esriUtil.FunctionRasters
{
    class polytomousLogisticFunctionArguments
    {
        public polytomousLogisticFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public polytomousLogisticFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                IRaster temp = value;
                inrs = rsUtil.returnRaster(temp, rstPixelType.PT_FLOAT);
            } 
        }
        private Dictionary<string,float[]> slopes = new Dictionary<string,float[]>();//dictionary of classes and float array = intercept followed by betas
        public Dictionary<string,float[]> Slopes 
        { 
            get 
            { 
                return slopes; 
            } 
            set 
            { 
                slopes = value;
            } 
        }
        private IRaster seedRaster = null;
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = seedRaster;
                if (rs == null)
                {
                    rs = rsUtil.getBand(inrs, 0);
                    rs = rsUtil.constantRasterFunction(rs, 0);
                    IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                    for (int i = 0; i < slopes.Count+1; i++)
                    {
                        rsBc.AppendBand(rsBc.Item(0));
                    }
                }
                else
                {
                    rs = rsUtil.returnRaster(rs, rstPixelType.PT_FLOAT);
                    IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                    int rsCnt = rsBc.Count;
                    int slCnt = slopes.Count + 2;
                    int dif = rsCnt - slCnt;
                    if (dif > 0)
                    {
                        for (int i = 0; i < dif; i++)
                        {
                            rsBc.Remove(0);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Math.Abs(dif); i++)
                        {
                            rsBc.AppendBand(rsBc.Item(0));
                        }
                    }
                }
                return rs;

            }
        }
        public IRaster SeedRaster{get{return seedRaster;}set{seedRaster=value;}}
     }
}