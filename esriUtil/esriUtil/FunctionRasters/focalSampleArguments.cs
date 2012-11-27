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
    class focalSampleArguments
    {
        focalSampleArguments()
        {
            rsUtil = new rasterUtil();
        }
        public focalSampleArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster { 
            get 
            { 
                return inrs;
            } 
            set 
            { 
                IRaster inrsT = value;
                inrs = rsUtil.constantRasterFunction(inrsT,0);
                origRs = rsUtil.returnRaster(value,rstPixelType.PT_FLOAT);
            } 
        }
        public rasterUtil.focalType Operation { get; set; }
        private HashSet<string> offsets = new HashSet<string>();//azimuth:distance in degrees:map units
        public HashSet<string> OffSets { get { return offsets; } set { offsets = value; } }
        private ESRI.ArcGIS.Geodatabase.IRaster origRs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OriginalRaster { get { return origRs; } }
    }
}
