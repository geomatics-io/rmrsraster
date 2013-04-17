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
    class pairedttestFunctionArguments
    {
        public pairedttestFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public pairedttestFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                IRaster temp = value;
                inrs = rsUtil.returnRaster(temp);
            } 
        }
        private Statistics.dataPrepPairedTTest ttest = null;
        private Dictionary<string, double[]> tDic = null;
        public Dictionary<string, double[]> TTestDictionary { get { return tDic; } }
        public Statistics.dataPrepPairedTTest TTestModel
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
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = rsUtil.getBand(inrs, 0);
                rs = rsUtil.constantRasterFunction(rs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                for (int i = 0; i < ttest.VariableFieldNames.Length; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                //Console.WriteLine(rsBc.Count.ToString());
                return (IRaster)rsBc;
            }
        }
    }
}