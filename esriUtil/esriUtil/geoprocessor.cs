using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

namespace esriUtil
{
    public class geoprocessor
    {
        #region Constants
                public const string esriRasterCompressionLZZ = "";
                public const string esriRasterPixelType1_BIT = "1_BIT";
                public const string esriRasterPixelType2_BIT = "2_BIT";
                public const string esriRasterPixelType4_BIT = "4_BIT";
                public const string esriRasterPixelType8_BITU = "8_BIT_UNSIGNED";
                public const string esriRasterPixelType8_BITS = "8_BIT_SIGNED";
                public const string esriRasterPixelType16_BITU = "16_BIT_UNSIGNED";
                public const string esriRasterPixelType16_BITS = "16_BIT_SIGNED";
                public const string esriRasterPixelType32_BITU = "32_BIT_UNSIGNED";
                public const string esriRasterPixelType32_BITS = "32_BIT_SIGNED";
                public const string esriRasterPyramidTypeNearest = "Pyramid -1 NEAREST";
                public const string esriRasterPyramidTypeBilinear = "Pyramid -1 BILINEAR";
                public const string esriRasterPyramidTypeCubic = "Pyramid -1 CUBIC";
                public const string esriRasterCompressTypeLZ77 = "LZ77";
                public const string esriRasterCompressTypeJPEG = "JPEG 75";
                public const string esriRasterCompressTypeJPEG2000 = "JPEG2000 75";
                public const string esriRasterCompressTypeNone = "NONE";
                public const string esriGeometryTypePolygon = "POLYGON";
                public const string esriGeometryTypePoint = "POINT";
                public const string esriGeometryTypePolyline = "POLYLINE";
                public const string esriGeometryTypeMultipoint = "MULTIPOINT";
                public const string esriFieldTypeText = "TEXT";
                public const string esriFieldTypeDouble = "DOUBLE";
                public const string esriFieldTypeFloat = "FLOAT";
                public const string esriFieldTypeLong = "LONG";
                public const string esriFieldTypeShort = "SHORT";
                public const string esriFieldTypeBlob = "BLOB";
                public const string esriFieldTypeRaster = "RASTER";
                public const string esriFieldTypeDate = "DATE";
        #endregion  
        private IGeoProcessor gp = new GeoProcessorClass();
        private IGPUtilities2 gpUtl = new GPUtilitiesClass() as IGPUtilities2;
        public IGeoProcessor getGP()
        {
            IGeoProcessor gp2 = new GeoProcessorClass();
            return gp2;
        }
        private geoDatabaseUtility geoDbUtil = new geoDatabaseUtility();
        private IVariantArray param = new VarArrayClass();
        public bool exists(string catalogpath)
        {
            object dt = "";
            return gp.Exists(catalogpath, ref dt);
        }
        public IMap Map
        {
            get
            {
                return gpUtl.GetMap();
            }
        }
        public IActiveView ActiveView
        {
            get
            {
                return gpUtl.GetActiveView();
            }
        }
        public IEnumLayer MapLayers
        {
            get
            {
                return gpUtl.GetMapLayers();
            }
        }
        public IEnumTable MapTables
        {
            get
            {
                return gpUtl.GetMapTables();
            }
        }
        public IFeatureClass getFeatureClass(string catalogpath)
        {
            IFeatureClass ftrclass = null;
            if (exists(catalogpath))
            {
                ftrclass =  gpUtl.OpenFeatureClassFromString(catalogpath);
            }
            return ftrclass;
        }
        public IRasterDataset3 getRasterDataset(string catalogpath)
        {
            IRasterDataset3 rasterDataset = null;
            if (exists(catalogpath))
            {
                rasterDataset = (IRasterDataset3)gpUtl.OpenRasterDatasetFromString(catalogpath);
            }
            return rasterDataset;
        }
        public ITable getTable(string catalogpath)
        {
            ITable tbl = null;
            if (exists(catalogpath))
            {
                tbl = gpUtl.OpenTableFromString(catalogpath);
            }
            return tbl;
        }
        public IFields getFields(string catalogpath)
        {
            IFields flds = null;
            try
            {

                if (exists(catalogpath))
                {
                    param.RemoveAll();
                    param.Add(catalogpath);
                    IGPValue gpValue = (IGPValue)param;
                    flds = gpUtl.GetFields(gpValue);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return flds;
        }
        public string copyFeatures(object lyr, string outFeatureClass)
        {
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(lyr);
            param.Add(outFeatureClass);
            IGeoProcessorResult rslt = gp.Execute("CopyFeatures_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public string copyRows(object lyr, string outTable)
        {
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(lyr);
            param.Add(outTable);
            IGeoProcessorResult rslt = gp.Execute("CopyRows_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public string copyRaster(object lyr, string outRaster)
        {
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(lyr);
            param.Add(outRaster);
            IGeoProcessorResult rslt = gp.Execute("CopyRaster_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public string createNewTable(string geoDatabase, string tblName)
        {
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(geoDatabase);
            param.Add(tblName);
            IGeoProcessorResult rslt = gp.Execute("CreateTable_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public string createNewField(string ftrPath, string name, string type)
        {
            param.RemoveAll();
            param.Add(ftrPath);
            param.Add(name);
            param.Add(type);
            IGeoProcessorResult rslt = gp.Execute("AddField_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public double getLinearConversion(IProjectedCoordinateSystem prjSys)
        {
            IUnitConverter converter = new UnitConverterClass();
            ILinearUnit linearUnit = prjSys.CoordinateUnit;
            double meter = linearUnit.MetersPerUnit;
            double unitToFoot = converter.ConvertUnits(meter, esriUnits.esriMeters, esriUnits.esriFeet);
            return unitToFoot;   
        }
        public string createNewRasterDataset(string geoDatabase, string name)
        {
            return createNewRasterDataset(geoDatabase, name, "#", "#", "#", "#", "#", "#", "#", "#", "#");
        }
        public string createNewRasterDataset(string geoDatabase, string name,string cellSize,string pixelType,string coordSystem,string bands,string keywords,string pyramids,string tileSize, string compression, string pyramidRefPoint)
        {
            IGeoProcessor gp = getGP();
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(geoDatabase);
            param.Add(name);
            param.Add(cellSize);
            param.Add(pixelType);
            param.Add(coordSystem);
            param.Add(bands);
            param.Add(keywords);
            param.Add(pyramids);
            param.Add(tileSize);
            param.Add(compression);
            param.Add(pyramidRefPoint);
            IGeoProcessorResult rslt = gp.Execute("createraster_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public string createNewFeatureClass(string geoDatabase,string name, string type)
        {
            return createNewFeatureClass(geoDatabase,name,type,"#","#","#","#","#","#","#","#");
        }
        public string createNewFeatureClass(string geoDatabase, string name, string type,string template, string m, string z, string spatialRef, string keyworkds, string grid1, string grid2, string grid3)
        {
            gp.OverwriteOutput = true;
            param.RemoveAll();
            param.Add(geoDatabase);
            param.Add(name);
            param.Add(type);
            IGeoProcessorResult rslt = gp.Execute("createfeatureclass_management", param, null);
            string x = getMessages(rslt);
            
            return x;
        }
        public IGeoProcessorResult execute(string processName, params string[] args)
        {
            IGeoProcessorResult rslt = null;
            try
            {
                param.RemoveAll();
                foreach (string s in args)
                {
                    param.Add(s);
                }
                rslt = gp.Execute(processName, param, null);
            }
            catch (Exception e)
            {
                gp.AddError("Error: " + e.ToString());
            }
            return rslt;
        }
        public ISpatialReference getSpatialRef(string catalogpath)
        {
            object dtype = "";
            ISpatialReference spRef = null;
            IDataElement dtE = gp.GetDataElement(catalogpath, ref dtype);
            string datatype = dtE.Type.ToLower();
            Console.WriteLine(datatype);
            try
            {
                if (datatype.IndexOf("raster")>-1)
                {
                    IGeoDataset rstd = (IGeoDataset)getRasterDataset(catalogpath);
                    spRef = rstd.SpatialReference;
                }
                else
                {
                    IGeoDataset ftrs = (IGeoDataset)getFeatureClass(catalogpath);
                    spRef = ftrs.SpatialReference;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return spRef;
        }
        public string getMessages(IGeoProcessorResult result)
        {
            StringBuilder x = new StringBuilder();
            for (int i = 0; i < result.MessageCount; i++)
            {
                x.AppendLine(result.GetMessage(i));
            }
            return x.ToString();
        }
        public string getOutput(IGeoProcessorResult result, int index)
        {
            return result.GetOutput(index).GetAsText();
        }
        public int getOutputCount(IGeoProcessorResult result)
        {
            return result.OutputCount;
        }
        public void refreshView()
        {
            IGPUtilities2 gpUtl = new GPUtilitiesClass() as IGPUtilities2;
            gpUtl.RefreshView();
        }
        public void refreshCatalog(string path)
        {
            IGPUtilities2 gpUtl = new GPUtilitiesClass() as IGPUtilities2;
            IGeoProcessor gp = getGP();
            IGPDataType nm = new GPDateTypeClass();
            object dt = "";
            IDataElement dtE = gp.GetDataElement(path,ref dt);
            gpUtl.RefreshCatalog(dtE);
        }
        public void addToolBox(IGeoProcessor gp, string path)
        {
            try
            {
                gp.AddToolbox(path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void removeToolBox(string path)
        {
            IGeoProcessor gp = getGP();
            gp.RemoveToolbox(path);
        }
        private int fieldExist(IFields flds, string fldName)
        {
            int x = -1;
            for (int i = 0; i < flds.FieldCount; i++)
            {
                if (flds.get_Field(i).Name.ToLower().StartsWith(fldName.ToLower().Replace("*","")))
                {
                    x = i;
                    break;
                }
            }
            return x;

        }
        public bool deleteGeoDatabase(string dbPath)
        {
            bool x = true;
            try
            {
                IGeoProcessor gp = getGP();
                gp.OverwriteOutput = true;
                param.RemoveAll();
                param.Add(dbPath);
                IGeoProcessorResult rslt = gp.Execute("Delete_management", param, null);
                Console.WriteLine(getMessages(rslt));
            }
            catch (Exception e)
            {
                x = false;
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        } 
    }
}
