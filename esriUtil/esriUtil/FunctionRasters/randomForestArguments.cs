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
    class randomForestArguments
    {
        public randomForestArguments()
        {
            rsUtil = new rasterUtil();
        }
        public randomForestArguments(rasterUtil rasterUtility)
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
                inrs = rsUtil.createIdentityRaster(value, rstPixelType.PT_FLOAT);
            } 
        }
        private Statistics.dataPrepRandomForest df = null;
        public Statistics.dataPrepRandomForest RandomForestModel
        {
            get
            {
                return df;
            }
            set
            {
                df = value;
                //Console.WriteLine("Number of Classes = " + nclasses.ToString() + " Number of vars = " + nvars.ToString());
            }
        }
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IFunctionRasterDataset rs = rsUtil.getBand(inrs, 0);
                IRasterBandCollection rsBc = new RasterClass();
                int ncA = df.NumberOfClasses;
                for (int i = 0; i < ncA; i++)
                {
                    rsBc.AppendBands((IRasterBandCollection)rs);
                }
                return rsUtil.compositeBandFunction(rsBc);

            }
        }
    }
}