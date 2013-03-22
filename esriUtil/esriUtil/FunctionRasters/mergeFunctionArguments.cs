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
    class mergeFunctionArguments
    {
        public mergeFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public mergeFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private IRaster[] inrs = null;
        private rasterUtil rsUtil = null;
        public IRaster[] InRaster
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                inrs = value;
                env = null;
                for (int i = 0; i < inrs.Length; i++)
                {
                    IRaster rs = inrs[i];
                    IRasterProps rsP = (IRasterProps)rs;
                    IEnvelope ext = rsP.Extent;
                    if (env == null)
                    {
                        env = ext;
                    }
                    else
                    {
                        env.Union(ext);
                    }
                }
                sr = ((IRasterProps)inrs[0]).SpatialReference;
                createFeatureClass();
            } 
        }

        private void createFeatureClass()
        {
            string ftClsPath = rsUtil.TempMosaicDir+"\\catBnd.shp";
            IFields flds = new FieldsClass();
            IFieldsEdit fldsE = (IFieldsEdit)flds;
            IField fld = new FieldClass();
            IFieldEdit fldE = (IFieldEdit)fld;
            fldE.Name_2 = "catIndex";
            fldE.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fldsE.AddField(fldE);
            ftCls = geoUtil.createFeatureClass(ftClsPath, fldsE, esriGeometryType.esriGeometryPolygon, sr);
            int catInd = ftCls.FindField("catIndex");
            int cnt = 0;
            foreach (IRaster rs in inrs)
            {
                IFeature ftr = ftCls.CreateFeature();
                ftr.set_Value(catInd, cnt);
                IEnvelope ext = ((IRasterProps)rs).Extent;
                IPolygon poly = new PolygonClass();
                IPointCollection pColl = (IPointCollection)poly;
                pColl.AddPoint(ext.UpperLeft);
                pColl.AddPoint(ext.UpperRight);
                pColl.AddPoint(ext.LowerRight);
                pColl.AddPoint(ext.LowerLeft);
                poly.Close();
                ftr.Shape = poly;
                ftr.Store();
                cnt++;
            }
            
        }


        private ISpatialReference sr = null;
        private IFeatureClass ftCls = null;
        public IFeatureClass Boundary
        {
            get
            {
                return ftCls;
            }
            
        }
        private IEnvelope env = null;
        public IRaster OutRaster
        {
            get
            {
                
                IRaster rs = rsUtil.constantRasterFunction(inrs[0], 0);
                IRasterProps rsP = (IRasterProps)rs;
                double cX = rsP.MeanCellSize().X;
                double cY = rsP.MeanCellSize().Y;
                rsP.Extent = env;
                rsP.Width = System.Convert.ToInt32(env.Width / cX);
                rsP.Height = System.Convert.ToInt32(env.Height / cY);
                return rs;
            }
        }
    }
}