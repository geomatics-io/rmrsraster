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
    class regressionFunctionArguments
    {
        public regressionFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public regressionFunctionArguments(rasterUtil rasterUtility)
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
                inrs = temp;
            } 
        }
        private List<double[]> slopes = new List<double[]>();//double array = intercept followed by betas
        public List<double[]> Slopes 
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
                    IRasterProps rsProp = (IRasterProps)rs;
                    if (rsProp.PixelType != rstPixelType.PT_DOUBLE)
                    {
                        rs = rsUtil.convertToDifFormatFunction(rs, rstPixelType.PT_DOUBLE);
                    }
                    IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                    for (int i = 1; i < slopes.Count; i++)
                    {
                        rsBc.AppendBand(rsBc.Item(0));
                    }
                }
                else
                {
                    IRasterProps rsProp = (IRasterProps)rs;
                    if (rsProp.PixelType != rstPixelType.PT_DOUBLE)
                    {
                        rs = rsUtil.convertToDifFormatFunction(rs, rstPixelType.PT_DOUBLE);
                    }
                    IRasterBandCollection rsBc = (IRasterBandCollection)rs;
                    int rsCnt = rsBc.Count;
                    int slCnt = slopes.Count;
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