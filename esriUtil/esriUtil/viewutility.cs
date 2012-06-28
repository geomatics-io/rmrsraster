using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    public class viewUtility
    {
        #region esriConstants
        public const string esriIACFeatureLayer = "{AD88322D-533D-4E36-A5C9-1B109AF7A346}";
        public const string esriIACLayer = "{74E45211-DFE6-11D3-9FF7-00C04F6BC6A5}";
        public const string esriIACImageLayer = "{495C0E2C-D51D-4ED4-9FC1-FA04AB93568D}";
        public const string esriIACAcetateLayer = "{65BD02AC-1CAD-462A-A524-3F17E9D85432}";
        public const string esriIAnnotationLayer = "{4AEDC069-B599-424B-A374-49602ABAD308}";
        public const string esriIAnnotationSublayer = "{DBCA59AC-6771-4408-8F48-C7D53389440C}";
        public const string esriICadLayer = "{E299ADBC-A5C3-11D2-9B10-00C04FA33299}";
        public const string esriICadastralFabricLayer = "{7F1AB670-5CA9-44D1-B42D-12AA868FC757}";
        public const string esriICompositeLayer = "{BA119BC4-939A-11D2-A2F4-080009B6F22B}";
        public const string esriICompositeGraphicsLayer = "{9646BB82-9512-11D2-A2F6-080009B6F22B}";
        public const string esriICoverageAnnotationLayer = "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}";
        public const string esriIDataLayer = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";
        public const string esriIDimensionLayer = "{0737082E-958E-11D4-80ED-00C04F601565}";
        public const string esriIFDOGraphicsLayer = "{48E56B3F-EC3A-11D2-9F5C-00C04F6BC6A5}";
        public const string esriIFeatureLayer = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
        public const string esriIGdbRasterCatalogLayer = "{605BC37A-15E9-40A0-90FB-DE4CC376838C}";
        public const string esriIGeoFeatureLayer = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
        public const string esriIGraphicsLayer = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}";
        public const string esriIGroupLayer = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
        public const string esriIIMSSubLayer = "{D090AA89-C2F1-11D3-9FEF-00C04F6BC6A5}";
        public const string esriIIMAMapLayer = "{DC8505FF-D521-11D3-9FF4-00C04F6BC6A5}";
        public const string esriILayer = "{34C20002-4D3C-11D0-92D8-00805F7C28B0}";
        public const string esriIMapServerLayer = "{E9B56157-7EB7-4DB3-9958-AFBF3B5E1470}";
        public const string esriIMapServerSublayer = "{B059B902-5C7A-4287-982E-EF0BC77C6AAB}";
        public const string esriINetworkLayer = "{82870538-E09E-42C0-9228-CBCB244B91BA}";
        public const string esriIRasterLayer = "{D02371C7-35F7-11D2-B1F2-00C04F8EDEFF}";
        public const string esriIRasterCatalogLayer = "{AF9930F0-F61E-11D3-8D6C-00C04F5B87B2}";
        public const string esriITemporaryLayer = "{FCEFF094-8E6A-4972-9BB4-429C71B07289}";
        public const string esriITerrainLayer = "{5A0F220D-614F-4C72-AFF2-7EA0BE2C8513}";
        public const string esriITinLayer = "{FE308F36-BDCA-11D1-A523-0000F8774F0F}";
        public const string esriITopologyLayer = "{FB6337E3-610A-4BC2-9142-760D954C22EB}";
        public const string esriIWMSLayer = "{005F592A-327B-44A4-AEEB-409D2F866F47}";
        public const string esriIWMSGroupLayer = "{D43D9A73-FF6C-4A19-B36A-D7ECBE61962A}";
        public const string esriIWMSMapLayer = "{8C19B114-1168-41A3-9E14-FC30CA5A4E9D}";

        #endregion
        private IActiveView acView = null;
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="acv">the active view</param>
        public viewUtility(IActiveView acv)
        {
            acView = acv;
        }
        /// <summary>
        /// the active view property
        /// </summary>
        public IActiveView AcView
        {
            get
            {
                return acView;
            }
        }
        private ESRI.ArcGIS.esriSystem.UID getUID(string uid)
        {
            ESRI.ArcGIS.esriSystem.IUID vl = new ESRI.ArcGIS.esriSystem.UIDClass();
            vl.Value = uid;
            return (ESRI.ArcGIS.esriSystem.UID)(vl);
        }
        /// <summary>
        /// gets all active layers within a view given a layer type constant
        /// </summary>
        /// <param name="lyrTyp">layer type constant</param>
        /// <returns>Layer enumartor</returns>
        public IEnumLayer getActiveViewLayers(string lyrTyp)
        {
            ESRI.ArcGIS.esriSystem.UID uid = getUID(lyrTyp);
            IMap eMap = acView as IMap;
            IEnumLayer eLayers = eMap.get_Layers(uid, true);
            return eLayers;
        }
        /// <summary>
        /// gets a specific feature layer give a layer name
        /// </summary>
        /// <param name="lyrName">layer name</param>
        /// <returns>ILayer</returns>
        public ILayer getFeatureLayer(string lyrName)
        {
            IEnumLayer lyrs = getActiveViewLayers(viewUtility.esriIFeatureLayer);
            ILayer flyr = null;
            string lyrName2 = null;
            ILayer i = lyrs.Next();
            while(i!=null)
            {
                lyrName2 = i.Name;
                if (lyrName2.ToLower() == lyrName.ToLower())
                {
                    flyr = i;
                    break;
                }
                i = lyrs.Next();
            }
            return flyr;
        }
        /// <summary>
        /// gets a specific mapservice layer
        /// </summary>
        /// <param name="lyrName">layer name</param>
        /// <returns>ILayer</returns>
        public ILayer getMapServerLayer(string lyrName)
        {
            IEnumLayer lyrs = getActiveViewLayers(viewUtility.esriIMapServerLayer);
            ILayer flyr = null;
            ILayer i = lyrs.Next();
            string lyrName2 = null;
            while(i!=null)
            {
                lyrName2 = i.Name;
                if (lyrName2.ToLower() == lyrName.ToLower())
                {
                    flyr = i;
                    break;
                }
                i = lyrs.Next();
            }
            return flyr;
        }
        /// <summary>
        /// gets the extent of the active View (xmin ymin xmax ymax)
        /// </summary>
        /// <returns>string extent that can be directly used by a geoprocessing object</returns>
        public string getStringViewExtent()
        {
            IEnvelope ext = acView.Extent;
            return ext.XMin.ToString() + " " + ext.YMin.ToString() + " " + ext.XMax.ToString() + " " + ext.YMax.ToString();
        }
        /// <summary>
        /// finds the db string for a give catalog path
        /// </summary>
        /// <param name="str">path of the spatial object</param>
        /// <returns></returns>
        private string getDbString(string str)
        {
            string vl = null;
            if (str.ToLower().EndsWith(".shp") || str.ToLower().EndsWith(".dbf"))
            {
                vl = System.IO.Path.GetDirectoryName(str);
            }
            else
            {
                List<string> cstr = new List<string>();
                foreach (string s in str.Split(new char[] { '\\' }))
                {
                    cstr.Add(s);
                    if (s.ToLower().EndsWith(".gdb") || s.ToLower().EndsWith(".mdb") || s.ToLower().EndsWith(".sde"))
                    {
                        break;
                    }
                }
                vl = String.Join("\\", cstr.ToArray());
            }
            return vl;
        }
        /// <summary>
        /// adds a layer to the active view
        /// </summary>
        /// <param name="path">full path name</param>
        /// <returns>Ilayer</returns>
        public ILayer addLayer(string path)
        {
            IMap map = (IMap)acView; 
            geoDatabaseUtility geoUtil = new geoDatabaseUtility();
            esriDatasetType dType = geoUtil.getDataType(path);
            ILayer lyr = null;
            switch(dType)
            {
                case esriDatasetType.esriDTFeatureClass:
                    IFeatureLayer ftrLayer = new FeatureLayerClass();
                    ftrLayer.FeatureClass = geoUtil.getFeatureClass(path);
                    lyr = (ILayer)ftrLayer;
                    lyr.Name = ftrLayer.FeatureClass.AliasName;
                    map.AddLayer(lyr);
                    break;
                case esriDatasetType.esriDTRasterBand:
                case esriDatasetType.esriDTRasterCatalog:
                case esriDatasetType.esriDTRasterDataset:
                    IRasterLayer rasterLayer = new RasterLayerClass();
                    rasterLayer.CreateFromDataset(geoUtil.getRasterDataset(path));
                    rasterLayer.Name = rasterLayer.Name;
                    map.AddLayer((ILayer)rasterLayer);
                    break;
                case esriDatasetType.esriDTTable:
                    ITable tbl = geoUtil.getTable(path);
                    ITableCollection tableCollection = (ITableCollection)map;
                    tableCollection.AddTable(tbl);
                    break;
                default:
                    break;
            }
            acView.Refresh();
            return lyr;
        }
        /// <summary>
        /// Removes a layer from the active view
        /// </summary>
        /// <param name="lyrName">layer name</param>
        /// <returns>bool expresing if it succeded</returns>
        public bool removeLayer(string lyrName)
        //Removes a layer from the map
        {
            bool x = true;
            IFeatureLayer ftrlyr = new FeatureLayerClass();
            try
            {
                IMap map = acView as IMap;
                ftrlyr = getFeatureLayer(lyrName) as IFeatureLayer;
                map.DeleteLayer(ftrlyr);
                acView.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.ToString());
                x = false;
            }
            return x;
        }
    }
}
