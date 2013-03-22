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
    class mosaicFunctionArguments
    {
        public mosaicFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public mosaicFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster[] inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster[] InRasterArr
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                ext = null;
                inrs = value;
                IRasterCollection rsColl = (IRasterCollection)mos;
                for (int i = 0; i < inrs.Length; i++)
                {
                    IRaster rs = inrs[i];
                    IRasterProps rsP = (IRasterProps)rs;
                    rsColl.Append(rs);
                    if (ext == null)
                    {
                        ext = rsP.Extent;
                    }
                    else
                    {
                        ext.Union(rsP.Extent);
                    }
                }
                
            } 
        }
        private IEnvelope ext = null;
        private IMosaicRaster mos = new MosaicRasterClass();
        public IMosaicRaster Mosaic { get { return mos; } }
        public IRaster OutRaster
        {
            get
            {
                IRaster rs = inrs[0];
                rs = rsUtil.constantRasterFunction(rs, 0);
                IRasterProps rsP = (IRasterProps)rs;
                double cX = rsP.MeanCellSize().X;
                double cY = rsP.MeanCellSize().Y;
                rsP.Extent = ext;
                rsP.Width = System.Convert.ToInt32(ext.Width / cX);
                rsP.Height = System.Convert.ToInt32(ext.Height / cY);
                return rs;
            }
        }
    }
}
