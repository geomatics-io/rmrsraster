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
        private void createTable()
        {
            string tblName = "catTbl.dbf";
            IWorkspace wks = geoUtil.OpenWorkSpace(rsUtil.TempMosaicDir);
            IFields flds = new FieldsClass();
            IFieldsEdit fldsE = (IFieldsEdit)flds;
            IField fld = new FieldClass();
            IFieldEdit fldE = (IFieldEdit)fld;
            fldE.Name_2 = "catIndex";
            fldE.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fldsE.AddField(fldE);
            
            IField fld2 = new FieldClass();
            IFieldEdit fldE2 = (IFieldEdit)fld2;
            fldE2.Name_2 = "XMIN";
            fldE2.Type_2 = esriFieldType.esriFieldTypeDouble;
            fldsE.AddField(fldE2);

            IField fld3 = new FieldClass();
            IFieldEdit fldE3 = (IFieldEdit)fld3;
            fldE3.Name_2 = "XMAX";
            fldE3.Type_2 = esriFieldType.esriFieldTypeDouble;
            fldsE.AddField(fldE3);

            IField fld4 = new FieldClass();
            IFieldEdit fldE4 = (IFieldEdit)fld4;
            fldE4.Name_2 = "YMIN";
            fldE4.Type_2 = esriFieldType.esriFieldTypeDouble;
            fldsE.AddField(fldE4);

            IField fld5 = new FieldClass();
            IFieldEdit fldE5 = (IFieldEdit)fld5;
            fldE5.Name_2 = "YMAX";
            fldE5.Type_2 = esriFieldType.esriFieldTypeDouble;
            fldsE.AddField(fldE5);

            tbl = geoUtil.createTable(wks,tblName,flds);
            int catInd = tbl.FindField("catIndex");
            int xMinInd = tbl.FindField("XMIN");
            int xMaxInd = tbl.FindField("XMAX");
            int yMinInd = tbl.FindField("YMIN");
            int yMaxInd = tbl.FindField("YMAX");
            int cnt = 0;
            foreach (IRaster rs in inrs)
            {
                IRow rw = tbl.CreateRow();
                rw.set_Value(catInd, cnt);
                IEnvelope ext = ((IRasterProps)rs).Extent;
                rw.set_Value(xMinInd, ext.XMin);
                rw.set_Value(xMaxInd, ext.XMax);
                rw.set_Value(yMinInd, ext.YMin);
                rw.set_Value(yMaxInd, ext.YMax);
                rw.Store();
                cnt++;
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
        private ITable tbl = null;
        public ITable Catalog { get { return tbl; } }
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
        public IFunctionRasterDataset OutRaster
        {
            get
            {
                IRaster rsT = inrs[0];
                IRasterProps rsP = (IRasterProps)rsT;
                IPnt cellSize = rsP.MeanCellSize();
                ISpatialReference sr = rsP.SpatialReference;
                int bndCnt = ((IRasterBandCollection)rsT).Count;
                IFunctionRasterDataset rs = rsUtil.constantRasterFunction(rsT,env,0,cellSize);
                return rs;
            }
        }
    }
}