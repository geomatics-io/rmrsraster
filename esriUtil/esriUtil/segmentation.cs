using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using Accord.MachineLearning;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.Geoprocessing;

namespace esriUtil
{
    public class segmentation
    {
        public segmentation(rasterUtil rasterUtility, IRaster inRaster, double maximumArea, double minimumArea, int sensitivity, IWorkspace OutputWorkspace=null, string OutputFeatureClassName="segPoly", bool SmoothPolygon = false)
        {
            rsUtil = rasterUtility;
            inputRaster = inRaster;
            bCnt = ((IRasterBandCollection)inputRaster).Count;
            ftrMeans = new double[bCnt];
            maxArea = maximumArea;
            minArea = minimumArea;
            specificity = sensitivity;
            OutName = OutputFeatureClassName;
            if (OutputWorkspace == null)
            {
                IRaster2 rs2 = (IRaster2)inRaster;
                IDataset dSet = (IDataset)rs2.RasterDataset;
                OutWorkSpace = dSet.Workspace;
            }
            else
            {
                OutWorkSpace = OutputWorkspace;
            }
            Smooth = SmoothPolygon;
        }
        public segmentation()
        {
            rsUtil = new rasterUtil();
        }
        private rasterUtil rsUtil = null;
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        public rasterUtil RasterUtility { get { return rsUtil; } }
        private double maxArea = 40460;
        public double MaxArea
        {
            get
            {
                return maxArea;
            }
            set
            {
                maxArea = value;
            }
        }
        private double minArea = 4046;
        public double MinArea
        {
            get
            {
                return minArea;
            }
            set
            {
                minArea = value;
            }
        }
        private IRaster inputRaster = null;
        public IRaster InputRaster
        {
            get
            {
                return inputRaster;
            }
            set
            {
                inputRaster = value;
            }
        }
        private IFeatureClass outftr = null;
        private int specificity = 100;
        public int Sensitivity
        {
            get
            {
                return specificity;
            }
            set
            {
                specificity = value;
            }
        }
        public IFeatureClass OutFeatureClass
        {
            get
            {
                return outftr;
            }
            set
            {
                outftr = value;
            }
        }
        private int segIndex = 0;
        private int clustIndex = 0;
        //public IFeatureClass createPolygons3()
        //{
        //    createFeatureClass();

        //}
        public IFeatureClass createPolygons2()
        {
            createFeatureClass();
            IRasterProps rsProp = (IRasterProps)inputRaster;
            int bndCnt = ((IRasterBandCollection)inputRaster).Count;
            IPnt rsPnt = rsProp.MeanCellSize();
            double cellArea = rsPnt.X * rsPnt.Y;
            double tCells = minArea / cellArea;
            IFunctionRasterDataset sDset = rsUtil.createMeanShiftFuction(inputRaster, (int)tCells);
            FunctionRasters.meanShiftFunctionDataset rFunc = (FunctionRasters.meanShiftFunctionDataset)sDset.Function;
            IRaster2 rs2 = (IRaster2)rsUtil.createRaster(sDset);
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            IRasterDomainExtractor dExt = new RasterDomainExtractorClass();
            do
            {
                IPixelBlock pb = rsCur.PixelBlock;
                IFunctionRasterDataset pbDset = rsUtil.PixelBlockToRaster(pb, rsCur.TopLeft, sDset);
                IRaster rs = rsUtil.createRaster(pbDset);
                int numClusters = rFunc.NumClusters;
                for (int c = 0; c < numClusters; c++)
                {
                    IFunctionRasterDataset fd = rsUtil.calcEqualFunction(pbDset, c);
                    IFunctionRasterDataset fd2 = rsUtil.setNullValue(fd, 0);
                    IPolygon polys = dExt.ExtractDomain(rsUtil.createRaster(fd2), true);
                }
                

            } while (rsCur.Next() == true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rsCur);

            


            return outftr;
        }

        private void createFeatureClass()
        {
            if (outftr == null)
            {
                //mfldIndex = new int[bCnt];
                segIndex = 0;
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "SegID";
                fldE.Type_2 = esriFieldType.esriFieldTypeInteger;
                fldsE.AddField(fldE);
                //IField fld2 = new FieldClass();
                //IFieldEdit fldE2 = (IFieldEdit)fld2;
                //fldE2.Name_2 = "Cluster";
                //fldE2.Type_2 = esriFieldType.esriFieldTypeInteger;
                //fldsE.AddField(fldE2);
                //for (int i = 0; i < bCnt; i++)
                //{
                //    IField mFld = new FieldClass();
                //    IFieldEdit mFldE = (IFieldEdit)mFld;
                //    mFldE.Name_2 = "Band" + i.ToString();
                //    mFldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                //    fldsE.AddField(mFld);
                //}
                outftr = geoUtil.createFeatureClass((IWorkspace2)OutWorkSpace, OutName, flds, esriGeometryType.esriGeometryPolygon, ((IRasterProps)InputRaster).SpatialReference);
                segIndex = outftr.FindField("SegID");
                //clustIndex = outftr.FindField("Cluster");
                //for (int i = 0; i < bCnt; i++)
                //{
                //    mfldIndex[i] = outftr.FindField("Band" + i.ToString());
                //}
            }
        }
        public IFeatureClass createPolygons()
        {
            
            IRasterProps rsProp = (IRasterProps)inputRaster;
            
            IPnt rsPnt = rsProp.MeanCellSize();
            double cellArea = rsPnt.X*rsPnt.Y;
            double tCells = minArea/cellArea;
            int rws = (int)Math.Sqrt(tCells);
            if(rws<3) rws = 3;
            int clms = rws;
            
            esriUtil.Statistics.dataPrepClusterBinary dpClus = new Statistics.dataPrepClusterBinary(inputRaster, specificity,100000,0.001);
            IRaster clusRs = rsUtil.createRaster(rsUtil.calcClustFunctionBinary(inputRaster, dpClus));
            Console.WriteLine("Exracting Domains");
            extractDomains(clusRs,dpClus);
            //IFeatureCursor ftrCur = outftr.Search(null, false);
            //IFeature ftr = ftrCur.NextFeature();
            //Console.WriteLine("removing small polygons");
            //while (ftr != null)
            //{
            //    IArea pArea = (IArea)ftr.Shape;
            //    double area = pArea.Area;
            //    if (area < MinArea)
            //    {
            //        addToNeighboringFeature(ftr, outftr, dpClus);
            //    }
            //    ftr = ftrCur.NextFeature();
            //}
            //Console.WriteLine("splitting large polygons");
            //ftrCur = outftr.Search(null, false);
            //ftr = ftrCur.NextFeature();
            //while (ftr != null)
            //{
            //    IArea pArea = (IArea)ftr.Shape;
            //    double area = pArea.Area;
            //    if (area > MaxArea)
            //    {
            //        splitFeature(ftr, outftr, true);
            //    }
            //    ftr = ftrCur.NextFeature();
            //}
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            return outftr;
        }
        int segid = 1;
        int bCnt = 0;
        int[] mfldIndex = null;
        private void extractDomains(IRaster clusRs, Statistics.dataPrepClusterBinary dpClus)
        {
            
            if (outftr == null)
            {
                mfldIndex=new int[bCnt];
                segIndex = 0;
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = "SegID";
                fldE.Type_2 = esriFieldType.esriFieldTypeInteger;
                fldsE.AddField(fldE);
                //IField fld2 = new FieldClass();
                //IFieldEdit fldE2 = (IFieldEdit)fld2;
                //fldE2.Name_2 = "Cluster";
                //fldE2.Type_2 = esriFieldType.esriFieldTypeInteger;
                //fldsE.AddField(fldE2);
                //for (int i = 0; i < bCnt; i++)
                //{
                //    IField mFld = new FieldClass();
                //    IFieldEdit mFldE = (IFieldEdit)mFld;
                //    mFldE.Name_2 = "Band" + i.ToString();
                //    mFldE.Type_2 = esriFieldType.esriFieldTypeDouble;
                //    fldsE.AddField(mFld);
                //}
                outftr = geoUtil.createFeatureClass((IWorkspace2)OutWorkSpace, OutName, flds, esriGeometryType.esriGeometryPolygon, ((IRasterProps)clusRs).SpatialReference);
                segIndex = outftr.FindField("SegID");
                //clustIndex = outftr.FindField("Cluster");
                //for (int i = 0; i < bCnt; i++)
                //{
                //    mfldIndex[i] = outftr.FindField("Band" + i.ToString());
                //}
            }
            IRasterDomainExtractor domExtract = new RasterDomainExtractorClass();
            IRasterProps rsp = (IRasterProps)clusRs;
            IRasterCursor rsCur = ((IRaster2)clusRs).CreateCursorEx(null);
            IPnt cellSizeRs = rsp.MeanCellSize();
            double pSizeX = cellSizeRs.X;
            double pSizeY = cellSizeRs.Y;
            int blockCnt = 1;
            do
            {
                IPixelBlock pb = rsCur.PixelBlock;
                IFunctionRasterDataset fDset = rsUtil.PixelBlockToRaster(pb, rsCur.TopLeft, clusRs);
                Console.WriteLine(blockCnt);
                //rsUtil.saveRasterToDataset(rsUtil.createRaster(fDset), "bTest"+blockCnt.ToString(), OutWorkSpace);
                

                HashSet<IPolygon4>[] polys = new HashSet<IPolygon4>[2];//small,good polygons
                for (int h = 0; h < 2; h++)
                {
                    polys[h] = new HashSet<IPolygon4>();
                }
                for (int i = 0; i < specificity; i++)
                {
                    //double[] means = ((Accord.MachineLearning.BinarySplit)dpClus.Model).Clusters[i].Mean;
                    IFunctionRasterDataset bRs = rsUtil.calcEqualFunction(fDset, i);
                    IRaster mRs = rsUtil.createRaster(rsUtil.setNullValue(bRs, 0));
                    IPolygon4 poly = (IPolygon4)domExtract.ExtractDomain(mRs, false);
                    IGeometryBag geoBag = poly.ConnectedComponentBag;
                    IGeometryCollection geoColl = (IGeometryCollection)geoBag;
                    for (int j = 0; j < geoColl.GeometryCount; j++)
                    {
                        IPolygon4 subPoly = (IPolygon4)geoColl.get_Geometry(j);
                        double subPolyArea = ((IArea)subPoly).Area;
                        double subPolyLength = subPoly.Length;
                        if (subPolyArea > maxArea)
                        {
                            splitFeature(subPoly, ref polys); // should split polygons. smaller than acceptable go into polys[0] ok go into polys[1]
                        }
                        else if (subPolyArea < minArea)
                        {
                            polys[0].Add(subPoly);
                        }
                        else
                        {
                            polys[1].Add(subPoly);
                        }
                        //segid++;
                    }
                    
                    //addToNeighboringFeature(ref polys);
                    IFeatureCursor ftrCur = outftr.Insert(true);
                    IFeatureBuffer nFtr = outftr.CreateFeatureBuffer();
                    Console.WriteLine("\tFTR Count small " + polys[0].Count.ToString());
                    Console.WriteLine("\tFTR Count big " + polys[1].Count.ToString());
                    foreach (IPolygon4 p in polys[1])
                    {
                        nFtr.Shape = (IGeometry)p;
                        nFtr.set_Value(segIndex, segid);
                        ftrCur.InsertFeature(nFtr);
                        segid++;
                    }
                    foreach (IPolygon p in polys[0])
                    {
                        nFtr.Shape = (IGeometry)p;
                        nFtr.set_Value(segIndex, segid);
                        ftrCur.InsertFeature(nFtr);
                        segid++;
                    }
                    ftrCur.Flush();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(nFtr);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
                }
                blockCnt++;
            } while (rsCur.Next() == true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rsCur);
            
        }

        private void addToNeighboringFeature(ref HashSet<IPolygon4>[] polys)
        {
            foreach (IPolygon4 geoS in polys[0])
            {
                //polys[0].
                if (geoS.IsEmpty) continue;
                else
                {
                    bool bTest = false;
                    foreach (IPolygon4 geoB in polys[1])
                    {
                        
                        if (geoB.IsEmpty) continue;
                        else
                        {
                            IRelationalOperator2 ro2 = (IRelationalOperator2)geoB;
                            if (!ro2.Disjoint(geoS))
                            {
                                try
                                {
                                    ITopologicalOperator5 tp6 = (ITopologicalOperator5)geoB;
                                    tp6.Union(geoS);
                                    bTest = true;
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        }
                    }

                }
            }
        }

        private void splitFeature(IPolygon4 subPoly, ref HashSet<IPolygon4>[] polys)
        {
            IRaster clipRs = rsUtil.createRaster(rsUtil.clipRasterFunction(inputRaster, subPoly, esriRasterClippingType.esriRasterClippingOutside));
            esriUtil.Statistics.dataPrepClusterBinary dpClus = new Statistics.dataPrepClusterBinary(clipRs, specificity,100000,0.001);
            IFunctionRasterDataset clusRs = rsUtil.calcClustFunctionBinary(inputRaster, dpClus);
            IFunctionRasterDataset clipRs2 = rsUtil.clipRasterFunction(clusRs, subPoly, esriRasterClippingType.esriRasterClippingOutside);
            IRasterDomainExtractor domExtract = new RasterDomainExtractorClass();
            for (int i = 0; i < specificity; i++)
            {
                IFunctionRasterDataset bRs = rsUtil.calcEqualFunction(clipRs2, i);
                IRaster mRs = rsUtil.createRaster(rsUtil.setNullValue(bRs, 0));
                IPolygon4 poly = (IPolygon4)domExtract.ExtractDomain(mRs, false);
                IGeometryBag geoBag = poly.ConnectedComponentBag;
                IGeometryCollection geoColl = (IGeometryCollection)geoBag;
                for (int j = 0; j < geoColl.GeometryCount; j++)
                {
                    IPolygon4 sp = (IPolygon4)geoColl.get_Geometry(j);
                    double subPolyArea = ((IArea)sp).Area;
                    //double subPolyLength = subPoly.Length;
                    if (subPolyArea > maxArea)
                    {
                        splitFeature(sp, ref polys); // should split polygons. smaller than acceptable go into polys[0] ok go into polys[1]
                    }
                    else if (subPolyArea < minArea)
                    {
                        polys[0].Add(sp);
                    }
                    else
                    {
                        polys[1].Add(sp);
                    }
                    //segid++;
                }
            }
            
        }

        //private void splitFeature(IPolygon4 subPoly, int originalCluster,double[] originalMeans)
        //{

        //    try
        //    {
        //        IRaster clipRs = rsUtil.createRaster(rsUtil.clipRasterFunction(inputRaster, subPoly, esriRasterClippingType.esriRasterClippingOutside));
        //        esriUtil.Statistics.dataPrepClusterBinary dpClus = new Statistics.dataPrepClusterBinary(clipRs, specificity);
        //        IRaster clusRs = rsUtil.createRaster(rsUtil.calcClustFunctionBinary(inputRaster, dpClus));
        //        IRaster clipRs2 = rsUtil.createRaster(rsUtil.clipRasterFunction(clusRs, subPoly, esriRasterClippingType.esriRasterClippingOutside));
        //        extractDomains(clipRs2,dpClus);
        //    }
        //    catch (Exception e)
        //    {
        //        IFeature newFtr = outftr.CreateFeature();
        //        newFtr.Shape = subPoly;
        //        newFtr.set_Value(segIndex,segid);
        //        newFtr.set_Value(clustIndex,originalCluster);
        //        for (int b = 0; b < bCnt; b++)
        //        {
        //            newFtr.set_Value(mfldIndex[b], originalMeans[b]);
        //        }
        //        newFtr.Store();
        //        segid++;
        //        Console.WriteLine(e.ToString());
        //    }
        //}
        //int tempftrCnt = 0; 
        //private void splitFeature(IFeature ftr, IFeatureClass ftrCls,bool checkFeatureCount=false)
        //{
        //    IGeometry geoFtr = ftr.ShapeCopy;
        //    ftr.Delete();
        //    IPolygon4 polyFtr = (IPolygon4)geoFtr;
        //    IGeometryBag geoBag = polyFtr.ConnectedComponentBag;
        //    IGeometryCollection geoColl = (IGeometryCollection)geoBag;
        //    for (int i = 0; i < geoColl.GeometryCount; i++)
        //    {
        //        IGeometry geo = geoColl.get_Geometry(i);
        //        if (geo.IsEmpty)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            double geoArea = ((IArea)geo).Area;
        //            if (geoArea >= MinArea && geoArea <= MaxArea)
        //            {
        //                IFeature newFtr = outftr.CreateFeature();
        //                newFtr.Shape = geo;
        //                newFtr.Store();
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    IRaster clipRs = rsUtil.clipRasterFunction(inputRaster, geo, esriRasterClippingType.esriRasterClippingOutside);
        //                    esriUtil.Statistics.dataPrepClusterBinary dpClus = new Statistics.dataPrepClusterBinary(clipRs, specificity);
        //                    IRaster clusRs = rsUtil.calcClustFunctionBinary(inputRaster, dpClus);
        //                    IRaster clipRs2 = rsUtil.clipRasterFunction(clusRs, geo, esriRasterClippingType.esriRasterClippingOutside);
        //                    IFeatureClass tempFtrCls = rsUtil.convertRasterToPolygon(clipRs2, OutWorkSpace, "temp" + tempftrCnt.ToString(), Smooth);
        //                    Console.WriteLine("Creating temp_" + tempftrCnt.ToString());
        //                    tempftrCnt++;
        //                    IFeatureCursor ftrCur = tempFtrCls.Search(null, false);
        //                    IFeature ftr2 = ftrCur.NextFeature();
        //                    while (ftr2 != null)
        //                    {
        //                        IArea pArea = (IArea)ftr2.Shape;
        //                        double area = pArea.Area;
        //                        if (area < MinArea)
        //                        {
        //                            addToNeighboringFeature(ftr2, tempFtrCls, dpClus);
        //                        }
        //                        ftr2 = ftrCur.NextFeature();
        //                    }
        //                    bool ftrResult = true;
        //                    if (checkFeatureCount)
        //                    {
        //                        ftrResult = (tempFtrCls.FeatureCount(null) > 1);
        //                    }
        //                    if (ftrResult)
        //                    {
        //                        ftrCur = tempFtrCls.Search(null, false);
        //                        ftr2 = ftrCur.NextFeature();
        //                        while (ftr2 != null)
        //                        {
        //                            IGeometry geo2 = ftr2.ShapeCopy;
        //                            IArea pArea = (IArea)geo2;
        //                            double area = pArea.Area;
        //                            if (area > MaxArea)
        //                            {
        //                                splitFeature(ftr2, tempFtrCls, true);
        //                            }
        //                            else
        //                            {
        //                                IFeature newFtr = outftr.CreateFeature();
        //                                newFtr.Shape = geo2;
        //                                newFtr.Store();
        //                            }
        //                            ftr2 = ftrCur.NextFeature();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        IFeature newFtr = outftr.CreateFeature();
        //                        newFtr.Shape = geo;
        //                        newFtr.Store();
        //                    }
        //                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
        //                    //IDataset dSet = (IDataset)tempFtrCls;
        //                    //dSet.Delete();
        //                }
        //                catch (Exception e)
        //                {
        //                    IFeature newFtr = outftr.CreateFeature();
        //                    newFtr.Shape = geo;
        //                    newFtr.Store();
        //                    Console.WriteLine(e.ToString());
        //                }
        //            }
        //        }
        //    }
        //}
        private double[] ftrMeans = null;
        //private void addToNeighboringFeature(IFeature ftr, IFeatureClass ftrCls, Statistics.dataPrepClusterBinary dpClust)
        //{

        //    for (int i = 0; i < bCnt; i++)
        //    {
        //        ftrMeans[i] = System.Convert.ToDouble(ftr.get_Value(i));
        //    }
        //    IGeometry geoFtr = ftr.ShapeCopy;
        //    ftr.Delete();
        //    IPolygon4 polyFtr = (IPolygon4)geoFtr;
        //    double smallArea = ((IArea)polyFtr).Area;
        //    IGeometryBag geoBag = polyFtr.ConnectedComponentBag;
        //    IGeometryCollection geoColl = (IGeometryCollection)geoBag;
        //    for (int i = 0; i < geoColl.GeometryCount; i++)
        //    {
        //        IGeometry geo = geoColl.get_Geometry(i);
        //        if (geo.IsEmpty)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            ISpatialFilter spFilt = new SpatialFilter();
        //            spFilt.Geometry = geo;
        //            spFilt.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
        //            int ftrCnt = ftrCls.FeatureCount(spFilt);
        //            if (ftrCnt > 0)
        //            {
        //                IFeatureCursor ftrCur = ftrCls.Search(spFilt, false);
        //                IFeature ftr2 = ftrCur.NextFeature();
        //                double maxcDist = Double.MinValue;
        //                double maxcDist2 = Double.MinValue;
        //                IFeature finalFtr = null;
        //                IFeature finalFtr2 = null;
        //                if (ftrCnt == 1) finalFtr = ftr2;
        //                else
        //                {
        //                    while (ftr2 != null)
        //                    {
        //                        IGeometry pGeo = (IGeometry)ftr2.Shape;
        //                        double clusDist = getClusterDistance(ftr2);
        //                        double area = ((IArea)pGeo).Area;
        //                        double narea = smallArea + area;
        //                        if (narea <= maxArea && narea > minArea)
        //                        {
        //                            if (clusDist > maxcDist)
        //                            {
        //                                finalFtr = ftr2;
        //                                maxcDist = clusDist;
        //                            }

        //                        }
        //                        if (clusDist < maxcDist2)
        //                        {
        //                            finalFtr2 = ftr2;
        //                            maxcDist2 = clusDist;
        //                        }
        //                        ftr2 = ftrCur.NextFeature();
        //                    }
        //                }
        //                if (finalFtr == null) finalFtr = finalFtr2;
        //                if (finalFtr != null)
        //                {
        //                    IPolygon4 polyFinal = (IPolygon4)finalFtr.ShapeCopy;
        //                    ITopologicalOperator4 topo = (ITopologicalOperator4)polyFinal;
        //                    finalFtr.Shape = topo.Union(geo);
        //                    finalFtr.Store();
        //                }
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
        //            }
        //        }
        //    }
        //}

        //private double getClusterDistance(IFeature ftr2)
        //{
        //    double sumEucDif = 0;
        //    for (int i = 0; i < bCnt; i++)
        //    {
        //        double means1 = ftrMeans[i];
        //        double means2 = System.Convert.ToDouble(ftr2.get_Value(mfldIndex[i]));
        //        sumEucDif += Math.Pow(means1 - means2, 2);
        //    }
        //    return sumEucDif;
        //}


        public IWorkspace OutWorkSpace { get; set; }

        public string OutName { get; set; }

        public bool Smooth { get; set; }
    }
}
