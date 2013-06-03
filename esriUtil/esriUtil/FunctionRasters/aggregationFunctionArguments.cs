using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil.FunctionRasters.NeighborhoodHelper;

namespace esriUtil.FunctionRasters
{
    class aggregationFunctionArguments
    {
        public aggregationFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public aggregationFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private rasterUtil rsUtil = null;
        private ESRI.ArcGIS.Geodatabase.IRaster inrs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRaster 
        { 
            get 
            {
                IRaster rs = rsUtil.returnRaster(origRs, rstPixelType.PT_FLOAT);
                IRasterProps rsP = (IRasterProps)rs;
                IPnt cSize = rsP.MeanCellSize();
                cSize.X = cSize.X*cells;
                cSize.Y = cSize.Y*cells;
                double w = System.Convert.ToInt32((rsP.Extent.Width / cSize.X) + 1)*cSize.X+rsP.Extent.XMin;
                double h = System.Convert.ToInt32((rsP.Extent.Height / cSize.Y) + 1)*cSize.Y+rsP.Extent.YMin;
                IEnvelope env = new EnvelopeClass();
                env.PutCoords(rsP.Extent.XMin, rsP.Extent.YMin, w, h);
                inrs = rsUtil.constantRasterFunction(rs,env,0,cSize);
                return inrs;
            } 
            set 
            {
                origRs = rsUtil.returnRaster(value);
            } 
        }
        
        private int cells = 3;
        public int Cells { get { return cells; } set { cells = value;} }
        private ESRI.ArcGIS.Geodatabase.IRaster origRs = null;
        public ESRI.ArcGIS.Geodatabase.IRaster OriginalRaster 
        { 
            get 
            {
                return origRs;
            } 
        }
    }
}

