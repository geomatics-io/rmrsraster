using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace esriUtil.Forms
{
    public class frmHelper
    {
        public frmHelper(IMap map,rasterUtil rasterUtility=null)
        {
            TheMap = map;
            GeoUtility = new geoDatabaseUtility();
            FeatureUtility = new featureUtil();
            if (rasterUtility == null) RasterUtility = new rasterUtil();
            else RasterUtility = rasterUtility;
            fillDictionary();
        }

       
        public IMap TheMap{get;set;}
        public geoDatabaseUtility GeoUtility { get; set; }
        public rasterUtil RasterUtility { get; set; }
        public featureUtil FeatureUtility { get; set; }
        public Dictionary<string, IFeatureClass> FeatureDictionary{get;set;}
        public Dictionary<string, ITable> TableDictionary { get; set; }
        public Dictionary<string, IFunctionRasterDataset> FunctionRasterDictionary { get; set; }
        public void fillDictionary()
        {
            FeatureDictionary = new Dictionary<string, IFeatureClass>();
            TableDictionary = new Dictionary<string, ITable>();
            FunctionRasterDictionary = new Dictionary<string, IFunctionRasterDataset>();
            if (TheMap != null)
            {
                for (int i = 0; i < TheMap.LayerCount; i++)
                {
                    try
                    {
                        ILayer lyr = TheMap.Layer[i];
                        if (lyr is FeatureLayer)
                        {
                            IFeatureLayer ftrLyr = (IFeatureLayer)lyr;
                            FeatureDictionary[lyr.Name] = ftrLyr.FeatureClass;
                        }
                        else if (lyr is RasterLayer)
                        {
                            IRasterLayer rsLyr = (IRasterLayer)lyr;
                            FunctionRasterDictionary[lyr.Name] = RasterUtility.createIdentityRaster(((IRaster2)rsLyr.Raster).RasterDataset);
                        }
                        else
                        {
                        }
                    }
                    catch
                    {
                    }
                }
                IStandaloneTableCollection tblCol = (IStandaloneTableCollection)TheMap;
                for (int i = 0; i < tblCol.StandaloneTableCount; i++)
                {
                    try
                    {
                        IStandaloneTable StTbl = tblCol.StandaloneTable[i];
                        TableDictionary[StTbl.Name] = StTbl.Table;
                    }
                    catch
                    {
                    }
                }
            }
        }
        public string[] getPath(ESRI.ArcGIS.Catalog.IGxObjectFilter filter,out string[] outName, bool MultiSelect = false)
        {
            List<string> outLst = new List<string>();
            List<string> outNameLst = new List<string>();
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.AllowMultiSelect = MultiSelect;
            gxDialog.ObjectFilter = filter;
            gxDialog.Title = filter.Description;
            ESRI.ArcGIS.Catalog.IEnumGxObject eGxObj;
            if (gxDialog.DoModalOpen(0, out eGxObj))
            {
                ESRI.ArcGIS.Catalog.IGxObject gxObj = eGxObj.Next();
                while (gxObj != null)
                {
                    outLst.Add(gxObj.FullName);
                    outNameLst.Add(gxObj.BaseName);
                    gxObj = eGxObj.Next();
                }
            }
            outName = outNameLst.ToArray();
            return outLst.ToArray();
        }
        public string getPathSave(ESRI.ArcGIS.Catalog.IGxObjectFilter filter, out string outName)
        {
            string outPath = "";
            outName = "";
            ESRI.ArcGIS.CatalogUI.IGxDialog gxDialog = new ESRI.ArcGIS.CatalogUI.GxDialogClass();
            gxDialog.ObjectFilter = filter;
            gxDialog.Title = filter.Description;
            if (gxDialog.DoModalSave(0))
            {
                outName = gxDialog.Name;
                outPath = gxDialog.FinalLocation.FullName  + "\\" + outName;
            }
            return outPath;
        }
    }
}
