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
    class ttestFunctionArguments
    {
        public ttestFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public ttestFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IFunctionRasterDataset inrs = null;
        private rasterUtil rsUtil = null;
        public IFunctionRasterDataset InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                
                inrs = rsUtil.createIdentityRaster(value);
            } 
        }
        private Statistics.dataPrepTTest ttest = null;
        private Dictionary<string, double[]> tDic = null;
        public Dictionary<string, double[]> TTestDictionary { get { return tDic; } }
        public Statistics.dataPrepTTest TTestModel
        {
            get
            {
                return ttest;
            }
            set
            {
                ttest = value;
                tDic = new Dictionary<string, double[]>();
                foreach (string s in ttest.Labels)
                {
                    tDic.Add(s, ttest.computeNew(s));
                }
            }
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < ttest.VariableFieldNames.Length; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                //Console.WriteLine(rsBc.Count.ToString());
                return rsUtil.compositeBandFunction(rsBc);
            }
        }
    }
}

