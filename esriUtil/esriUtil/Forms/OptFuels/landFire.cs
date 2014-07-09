using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;


namespace esriUtil.Forms.OptFuels
{
    public class landFire
    {
        public landFire()
        {
            rsUtil = new rasterUtil();
        }
        public landFire(IMap map)
        {
            mp = map;
            rsUtil = new rasterUtil();
        }
        public landFire(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        public landFire(rasterUtil rasterUtility, IMap map)
        {
            rsUtil = rasterUtility;
            mp = map;
        }
        private IFeatureClass ftrCls = null;
        public IFeatureClass LandFireFeatureClass { get { return ftrCls; } set { ftrCls = value; checkShapeArea(); } }

        private void checkShapeArea()
        {
            int shapeAreaIndex = ftrCls.FindField("Shape_Area");
            if (shapeAreaIndex > -1)
            {
                IField fld = ftrCls.Fields.get_Field(shapeAreaIndex);
                shapeAreaEdit = fld.Editable;
            }
            else
            {
                createShapeField();
                shapeAreaEdit = true;
            }
        }

        private void createShapeField()
        {
            geoUtil.createField(ftrCls,"Shape_Area",esriFieldType.esriFieldTypeDouble);
        }
        private bool useAspect = false;
        public bool UseAspect { get { return useAspect; } set { useAspect = value; } }
        private bool shapeAreaEdit = false;
        private IMap mp = null;
        private rasterUtil rsUtil = null;
        private IWorkspace landfireworkspace = null;
        public IWorkspace LandFireWorkspace { get { return landfireworkspace; } set { landfireworkspace = value; getLandFireRasters(); } }
        public int HeightLevels { get; set; }
        public int CoverLevels { get; set; }
        private IRaster vegRs = null;
        private IRaster htRs = null;
        private IRaster covRs = null;
        private IRaster demRs = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private double maxarea = 121398;
        private double minarea = 20233;
        public double MaxArea { get { return maxarea; } set { maxarea = value; } }
        public double MinArea { get { return minarea; } set { minarea = value; } }
        private void getLandFireRasters()
        {
            string wksPath = LandFireWorkspace.PathName;
            string vegPath = wksPath + "\\evt";
            string covPath = wksPath + "\\evc";
            string htPath = wksPath + "\\evh";
            string demPath = wksPath + "\\dem";
            vegRs = rsUtil.returnRaster(vegPath);
            htRs = rsUtil.returnRaster(htPath);
            covRs = rsUtil.returnRaster(covPath);
            demRs = rsUtil.returnRaster(demPath);
        }
        private IRaster modelrs = null;
        public IRaster SegementedRaster { get { return modelrs; } }
        public void segmentLandFireData()
        {
            try
            {
                int cnt = 0;
                foreach (IRaster rs in new IRaster[] { vegRs, htRs, covRs, demRs })
                {
                    if (rs==null)
                    {
                        Console.WriteLine("Null Raster found at iteration " + cnt.ToString());
                        cnt++;
                        return;
                    }
                }
                IRaster rs1 = vegRs;
                IRaster rs2 = rescaleHt();
                IRaster rs3 = rescaleCov();
                IRaster rs4 = calcTopo();
                IRaster rs5 = rsUtil.returnRaster(rsUtil.compositeBandFunction(new IRaster[]{rs1,rs2,rs3,rs4}));
                IRaster sMd = rsUtil.createRaster(rsUtil.localStatisticsfunction(rs5,rasterUtil.localType.SUM));
                modelrs = rsUtil.returnRaster(rsUtil.saveRasterToDataset(rsUtil.returnRaster(sMd, rstPixelType.PT_LONG),"lfSeg",landfireworkspace));
                //Console.WriteLine("Finished Segementing Raster");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private IRaster rescaleCov()
        {
            IRasterBand rsBand = ((IRasterBandCollection)covRs).Item(0);
            IRasterStatistics rsStats = rsBand.Statistics;
            double min = rsStats.Minimum;
            double max = rsStats.Maximum;
            double range = (max - min);
            double step = range / CoverLevels;
            IRemapFilter flt = new RemapFilterClass();
            double cnt = 0;
            double i = 0;
            for (i=min; i < max; i += step)
            {
                double nMax = i + step;
                if (nMax == max)
                {
                    nMax = max + 1;
                }
                flt.AddClass(i, nMax, cnt);
                cnt++;
            }
            IRaster rs2 = rsUtil.returnRaster(rsUtil.calcRemapFunction(covRs, flt));
            return rsUtil.returnRaster(rsUtil.calcArithmaticFunction(rs2,100000,esriRasterArithmeticOperation.esriRasterMultiply));
        }

        private IRaster rescaleHt()
        {
            IRasterBand rsBand = ((IRasterBandCollection)htRs).Item(0);
            IRasterStatistics rsStats = rsBand.Statistics;
            double min = rsStats.Minimum;
            double max = rsStats.Maximum;
            double range = (max - min);
            double step = range / HeightLevels;
            IRemapFilter flt = new RemapFilterClass();
            double cnt = 0;
            double i = 0;
            for (i = min; i < max; i += step)
            {
                double nMax = i + step;
                if (nMax == max)
                {
                    nMax = max + 1;
                }
                flt.AddClass(i, nMax, cnt);
                cnt++;
            }
            IRaster rs2 = rsUtil.returnRaster(rsUtil.calcRemapFunction(htRs, flt));
            return rsUtil.returnRaster(rsUtil.calcArithmaticFunction(rs2, 1000000, esriRasterArithmeticOperation.esriRasterMultiply));
        }

        private IRaster calcTopo()
        {
            IFunctionRasterDataset rsAspect = rsUtil.calcAspectFunction(demRs);
            IRemapFilter flt = new RemapFilterClass();
            if (useAspect)
            {
                flt.AddClass(225, 315, 3);
                flt.AddClass(315, 360, 0);
                flt.AddClass(0, 45, 0);
                flt.AddClass(45, 135, 2);
                flt.AddClass(135, 225, 2);
            }
            else
            {
                flt.AddClass(0, 361, 0);
            }
            IRaster rs2 = rsUtil.returnRaster(rsUtil.calcRemapFunction(rsAspect, flt));
            return rsUtil.returnRaster(rsUtil.calcArithmaticFunction(rs2, 10000, esriRasterArithmeticOperation.esriRasterMultiply));
        }

        private string returnSafeName(string outName)
        {
            string tempName = outName;
            if (rsUtil.rasterExists(landfireworkspace,tempName))
            {
                tempName = "_" + tempName;
                tempName = returnSafeName(tempName);
            }
            return tempName;
        }

        public void convertToPolygon()
        {
            if (modelrs == null)
            {
                Console.WriteLine("You must segment landfire data before you can covert to polygons");
                return;
            }
            try
            {
                if (modelrs != null)
                {
                    string rgNm = rsUtil.getSafeOutputName(landfireworkspace,"RG");
                    IRaster rgRs = rsUtil.createRaster(rsUtil.regionGroup(modelrs));
                    IRasterProps modelrsProps = (IRasterProps)modelrs;
                    IPnt pnt = modelrsProps.MeanCellSize();
                    double meanCellSize = pnt.X*pnt.Y;
                    //Console.WriteLine(meanCellSize);
                    int mincell = System.Convert.ToInt32((minarea/meanCellSize)+.5);
                    int maxcell = System.Convert.ToInt32((maxarea/meanCellSize)+.5);
                    //Console.WriteLine("number of cells = " + mincell.ToString());
                    Console.WriteLine("Eliminating slivers");
                    //IRaster rsE = rsUtil.eliminateSlivers(rgRs,mincell,maxcell);
                    Console.WriteLine("Splitting Polygons");
                    //IRaster rsS = rsUtil.splitRegions(rsE, mincell, maxcell);
                    string outNm = "LandFireStands";
                    outNm = returnSafeName(outNm);
                    //Console.WriteLine("Converting to polygon");
                    IConversionOp convOp = new RasterConversionOpClass();
                    //IGeoDataset geoDset = convOp.RasterDataToPolygonFeatureData((IGeoDataset)rsS, LandFireWorkspace, outNm, false);
                    //LandFireFeatureClass = (IFeatureClass)geoDset;
                }
                //Console.WriteLine("Finished Converting Polygons");
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.ToString());
            }

        }

        public void splitPolygons(bool MergeSmallPolysWithinBoundary)
        {
            if (ftrCls == null)
            {
                Console.WriteLine("You must convert segmented raster to polygons before you can split polygons");
                return;
            }
            //Console.WriteLine("Splitting Polygons");
            IQueryFilter qryFlt = new QueryFilterClass();
            string qry = "Shape_Area > " + maxarea.ToString();
            qryFlt.WhereClause = qry;
            IFeatureCursor ftrCur = ftrCls.Search(qryFlt, false);
            IFeature ftr = ftrCur.NextFeature();
            while (ftr != null)
            {
                string oid = ftr.OID.ToString();
                IGeometry geo = ftr.ShapeCopy;
                List<IGeometry> fGeoLst = splitGeometry(geo,MergeSmallPolysWithinBoundary);
                if(insertRecords(ftrCls, ftr, fGeoLst)) ftr.Delete();
                ftr = ftrCur.NextFeature();
            }
            //Console.WriteLine("Finished splitting Polygons");
        }

        private List<IGeometry> splitGeometry(IGeometry geo,bool mergePolys)
        {
            IGeometry geoM = null;
            if (mergePolys)
            {
                geoM = mergeSmallGeos(geo);
            }
            else
            {
                geoM = geo;
            }
            List<IGeometry> geoLst = new List<IGeometry>();
            IPolygon4 poly4 = (IPolygon4)geoM;
            IGeometryCollection geoColl = (IGeometryCollection)poly4.ConnectedComponentBag;
            for (int i = 0; i < geoColl.GeometryCount; i++)
            {
                IGeometry geo2 = geoColl.get_Geometry(i);
                IEnvelope env = geo2.Envelope;
                double xmax = env.XMax;
                double xmin = env.XMin;
                double ymax = env.YMax;
                double ymin = env.YMin;
                double xRange = xmax - xmin;
                double yRange = ymax - ymin;
                double nMinX, nMinY, nMaxX, nMaxY;
                if (xRange > yRange)
                {
                    nMinY = ymin - 1;
                    nMaxY = ymax + 1;
                    nMinX = xmin + (xRange / 2);
                    nMaxX = nMinX;
                }
                else
                {
                    nMinX = xmin - 1;
                    nMaxX = xmax + 1;
                    nMinY = ymin + (yRange / 2);
                    nMaxY = nMinY;
                }
                WKSPoint[] wksPoint = new WKSPoint[2];
                IPointCollection4 pointCollection4 = new PolylineClass();
                wksPoint[0].X = nMinX;
                wksPoint[0].Y = nMinY;
                wksPoint[1].X = nMaxX;
                wksPoint[1].Y = nMaxY;
                IGeometryBridge2 geometryBridge2 = new GeometryEnvironmentClass();
                geometryBridge2.AddWKSPoints(pointCollection4, ref wksPoint);
                IPolyline polyline = pointCollection4 as ESRI.ArcGIS.Geometry.IPolyline;
                polyline.SpatialReference = geo2.SpatialReference;
                ITopologicalOperator4 tp = (ITopologicalOperator4)geo2;
                IGeometry geoL = null;
                IGeometry geoR = null;
                try
                {
                    tp.Cut(polyline, out geoL, out geoR);
                    if (((IArea)geoL).Area > maxarea)
                    {
                        geoLst.AddRange(splitGeometry(geoL,false));
                    }
                    else
                    {
                        geoLst.Add(geoL);
                    }
                    if (((IArea)geoR).Area > maxarea)
                    {
                        geoLst.AddRange(splitGeometry(geoR,false));
                    }
                    else
                    {
                        geoLst.Add(geoR);
                    }
                }
                catch (Exception e)
                {
                    geoLst.Add(geo2);
                    Console.WriteLine(e.ToString());
                }
            }
            return geoLst;
            
        }

        private IGeometry mergeSmallGeos(IGeometry geo)
        {
            ITopologicalOperator4 tp4 = (ITopologicalOperator4)geo;
            IGeometryCollection geoCol = new PolygonClass();
            IGeometry bGeo = tp4.Boundary;
            ISpatialFilter spFilt = new SpatialFilter();
            spFilt.Geometry = bGeo;
            spFilt.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
            spFilt.GeometryField = ftrCls.ShapeFieldName;
            spFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            spFilt.WhereClause = "Shape_Area <= " + minarea;
            spFilt.SubFields = ftrCls.ShapeFieldName;
            IFeatureCursor ftrCur = ftrCls.Search(spFilt, false);
            IFeature ftr = ftrCur.NextFeature();
            int cntTest = 0;
            while (ftr != null)
            {
                IGeometry sGeo = ftr.ShapeCopy;
                geoCol.AddGeometry(sGeo);
                ftr.Delete();
                ftr = ftrCur.NextFeature();
                cntTest++;
            }
            if (cntTest > 0)
            {
                tp4.ConstructUnion((IEnumGeometry)geoCol);
            }
            return (IGeometry)tp4;
        }

        private bool insertRecords(IFeatureClass ftrCls, IFeature ftr, List<IGeometry> geoLst)
        {
            bool x = false;
            try
            {
                IFields flds = ftr.Fields;
                foreach (IGeometry geo in geoLst)
                {
                    IFeature nFtr = ftrCls.CreateFeature();
                    for (int f = 0; f < flds.FieldCount; f++)
                    {
                        IField fld = flds.get_Field(f);
                        esriFieldType fldType = fld.Type;
                        int nFldIndex = nFtr.Fields.FindField(fld.Name);
                        if (fld.Editable==true && (fldType == esriFieldType.esriFieldTypeString || fldType == esriFieldType.esriFieldTypeSmallInteger || fldType == esriFieldType.esriFieldTypeSingle || fldType == esriFieldType.esriFieldTypeInteger || fldType == esriFieldType.esriFieldTypeDouble))
                        {
                            nFtr.set_Value(nFldIndex, ftr.get_Value(f));
                            if (fld.Name == "Shape_Area")
                            {
                                nFtr.set_Value(nFtr.Fields.FindField("Shape_Area"),((IArea)geo).Area);
                            }
                        }
                    }
                    nFtr.Shape = geo;
                    nFtr.Store();
                }
                x = true;
            }
            catch (Exception e)
            {
                x = false;
                Console.WriteLine(e.ToString());
            }
            return x;
        }
        
        public void eliminateSlivers()
        {
            if (ftrCls == null)
            {
                Console.WriteLine("You must convert segemented raster to a polygon before you can eliminate slivers");
                return;
            }
            Console.WriteLine("Eliminating slivers");
            IQueryFilter qryFlt = new QueryFilterClass();
            qryFlt.WhereClause = "Shape_Area < " + minarea.ToString();
            IFeatureCursor ftrCur = ftrCls.Search(qryFlt, false);
            IFeature ftr = ftrCur.NextFeature();
            while (ftr != null)
            {
                int oid = ftr.OID;
                if(updatePolygon(ftrCls,oid)) ftr.Delete();
                ftr = ftrCur.NextFeature();
            }
            Console.WriteLine("Finished Eliminating Slivers");
        }

        private bool updatePolygon(IFeatureClass ftrCls,int oid)
        {
            bool x = false;
            IGeometry geo = ftrCls.GetFeature(oid).ShapeCopy;
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.WhereClause = ftrCls.OIDFieldName + " <> " + oid.ToString();
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IQueryFilterDefinition2 qryDef2 = (IQueryFilterDefinition2)spFlt;
            qryDef2.PostfixClause = "Order by Shape_Area DESC";
            spFlt.Geometry = geo;
            spFlt.GeometryField = ftrCls.ShapeFieldName;
            spFlt.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
            IFeatureCursor ftrCur = ftrCls.Search(spFlt, false);
            int shapeAreaIndex = ftrCur.FindField("Shape_Area");
            IFeature ftr = ftrCur.NextFeature();
            while (ftr!=null)
            {
                IGeometry geoMax = ftr.ShapeCopy;
                int beforeCnt = ((IGeometryCollection)geoMax).GeometryCount;
                ITopologicalOperator4 tp = (ITopologicalOperator4)geoMax;
                IGeometry uGeo = tp.Union(geo);
                int afterCnt = ((IGeometryCollection)uGeo).GeometryCount;
                if (beforeCnt >= afterCnt)
                {
                    try
                    {
                        ftr.Shape = uGeo;
                        if (shapeAreaEdit)
                        {
                            ftr.set_Value(shapeAreaIndex, ((IArea)uGeo).Area);
                        }
                        ftr.Store();
                        x = true;
                        return x;
                    }
                    catch (Exception e)
                    {
                        x = false;
                        Console.WriteLine(e.ToString());
                        return x;
                    }

                }
                ftr = ftrCur.NextFeature();
            }
            return x;
        }
        public void addFeatureClassToMap()
        {
            if (mp != null && ftrCls!=null)
            {
                IFeatureLayer ftrLyr = new FeatureLayerClass();
                ftrLyr.FeatureClass = ftrCls;
                ftrLyr.Name = ((IDataset)ftrCls).BrowseName;
                mp.AddLayer((ESRI.ArcGIS.Carto.ILayer)ftrLyr);
            }
        }
    }
}
