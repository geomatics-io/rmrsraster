using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using System.Data.SqlClient;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace esriUtil
{
    public class geoDatabaseUtility
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public geoDatabaseUtility()
        {
            setProjectPass();
            //updatedll();
        }
        private string server = "";
        private string instance = "";
        private string database = "";
        private string pVersion = "";
        private string mode = "";
        private string user = "";
        private object pass;
        /// <summary>
        /// default server
        /// </summary>
        public string Server { get { return server; } }
        /// <summary>
        /// default instance
        /// </summary>
        public string Instance { get { return instance; } }
        /// <summary>
        /// default database
        /// </summary>
        public string Database { get { return database; } }
        /// <summary>
        /// default version
        /// </summary>
        public string Version { get { return pVersion; } }
        /// <summary>
        /// default mode
        /// </summary>
        public string Mode { get { return mode; } }
        /// <summary>
        /// default PassWord
        /// </summary>
        public object PassWord { get { return pass; } }
        /// <summary>
        /// default users
        /// </summary>
        public string User{get{return user;}}
        /// <summary>
        /// returns a string minus unwanted charcters in a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string parsName(string name)
        {
            string n2 = name;
            string[] bc = { " ", ";", ":", "'", "?", "!", ".", "&", "*", "%", "$", "#", "@", "^", "(", ")", "-", "+" };
            foreach (string s in bc)
            {
                n2 = n2.Replace(s, "_");
            }
            return n2;
        }
        public bool isNumeric(string s)
        {
            double d;
            return double.TryParse(s, out d);
        }
        private IPropertySet getProjectPS()
        {
            IWorkspaceFactory wksF = new SdeWorkspaceFactoryClass();
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("SERVER", server);
            propSet.SetProperty("INSTANCE", instance);
            propSet.SetProperty("DATABASE", database);
            propSet.SetProperty("VERSION", pVersion);
            propSet.SetProperty("AUTHENTICATION_MODE", mode);
            propSet.SetProperty("USER", user);
            propSet.SetProperty("PASSWORD", pass);
            return propSet;
        }
        /// <summary>
        /// sets the project password
        /// </summary>
        public void setProjectPass()
        {
            IPropertySet pSet = getProjectPS();
            server = pSet.GetProperty("SERVER").ToString();
            instance = pSet.GetProperty("INSTANCE").ToString();
            database = pSet.GetProperty("DATABASE").ToString();
            pVersion = pSet.GetProperty("VERSION").ToString();
            mode = pSet.GetProperty("AUTHENTICATION_MODE").ToString();
            if(mode.ToLower()=="dbms")
            {
                pass = getProjectPS().GetProperty("PASSWORD");
                user = getProjectPS().GetProperty("USER").ToString();
            }

        }
        /// <summary>
        /// parses a DB string from a layer path
        /// </summary>
        /// <param name="lyrPath">full catelog path</param>
        /// <returns>string path of Database</returns>
        public string parseDbStr(string lyrPath)
        {
            string db = null;
            try
            {
                if (lyrPath.EndsWith(".shp") || lyrPath.EndsWith(".dbf"))
                {
                    db = System.IO.Path.GetDirectoryName(lyrPath);
                }
                else
                {
                    string dbExt = null;
                    string ftrL = lyrPath.ToLower();
                    if (ftrL.IndexOf(".gdb") > -1)
                    {
                        dbExt = ".gdb";
                    }
                    else if (ftrL.IndexOf(".mdb") > -1)
                    {
                        dbExt = ".mdb";
                    }
                    else if (ftrL.IndexOf(".sde") > -1)
                    {
                        dbExt = ".sde";
                    }
                    else
                    {
                        return System.IO.Path.GetDirectoryName(lyrPath);
                    }
                    db = lyrPath.Substring(0, ftrL.LastIndexOf(dbExt)) + dbExt;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return db;
        }
        public IWorkspace OpenRasterWorkspace(string dbPath)
        {
            IWorkspace wks = null;
            try
            {
                string dir = null;
                bool exst = false;
                if (dbPath.Split(new char[] { '\\' })[0].ToLower() == "database connections")
                {
                    dir = System.IO.Path.GetFullPath(dbPath).ToLower().Replace("database connections", System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ESRI\ArcCatalog");
                }
                else
                {
                    dir = System.IO.Path.GetFullPath(dbPath);
                }
                string ext = null;

                if (System.IO.Path.HasExtension(dir))
                {
                    ext = System.IO.Path.GetExtension(dir).ToLower();
                    switch (ext)
                    {
                        case ".mdb":
                        case ".sde":
                            System.IO.FileInfo flinfo = new System.IO.FileInfo(dir);
                            exst = flinfo.Exists;
                            if (!exst)
                            {
                                //Console.WriteLine("Database " + dir + " does not exist 72");
                                return wks;
                            }
                            break;
                        default:
                            System.IO.DirectoryInfo dlinfo = new System.IO.DirectoryInfo(dir);
                            exst = dlinfo.Exists;
                            if (!exst)
                            {
                                //Console.WriteLine("Database #" + dir + "# does not exist 80");
                                return wks;
                            }
                            break;

                    }

                }
                else
                {
                    ext = "";
                    System.IO.DirectoryInfo dlinfo = new System.IO.DirectoryInfo(dir);
                    if (!dlinfo.Exists)
                    {
                        //Console.WriteLine("database " + dir + " does not exist 94");
                        return wks;
                    }
                }
                IWorkspaceFactory2 wsFact;
                switch (ext)
                {
                    case ".gdb":
                        wsFact = (IWorkspaceFactory2)new FileGDBWorkspaceFactoryClass();
                        break;
                    case ".mdb":
                        wsFact = (IWorkspaceFactory2)new AccessWorkspaceFactoryClass();
                        break;
                    case ".sde":
                        wsFact = (IWorkspaceFactory2)new SdeWorkspaceFactoryClass();
                        break;
                    default:
                        wsFact = (IWorkspaceFactory2)new ESRI.ArcGIS.DataSourcesRaster.RasterWorkspaceFactoryClass();
                        //Console.WriteLine(dir);
                        wks = wsFact.OpenFromFile(dir, 0);
                        break;
                }
                if (wsFact.IsWorkspace(dir))
                {
                    try
                    {
                        wks = wsFact.OpenFromFile(dir, 0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error " + e.ToString());
                        wks = null;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                wks = null;
            }
            return wks;
        }
        /// <summary>
        /// opens a workspace based on database path
        /// </summary>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        public IWorkspace OpenWorkSpace(string dbPath)
        {
            //dbPath = System.IO.Path.GetFullPath(dbPath);
            IWorkspace wks = null;
            try
            {
                string dir = null;
                bool exst = false;
                if (dbPath.Split(new char[] { '\\' })[0].ToLower() == "database connections")
                {
                    dir = System.IO.Path.GetFullPath(dbPath).ToLower().Replace("database connections", System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ESRI\ArcCatalog");
                }
                else
                {
                    dir = System.IO.Path.GetFullPath(dbPath);
                }
                string ext = null;

                if (System.IO.Path.HasExtension(dir))
                {
                    ext = System.IO.Path.GetExtension(dir).ToLower();
                    switch (ext)
                    {
                        case ".mdb":
                        case ".sde":
                            System.IO.FileInfo flinfo = new System.IO.FileInfo(dir);
                            exst = flinfo.Exists;
                            if (!exst)
                            {
                                Console.WriteLine("Database " + dir + " does not exist 72");
                                return wks;
                            }
                            break;
                        default:
                            System.IO.DirectoryInfo dlinfo = new System.IO.DirectoryInfo(dir);
                            exst = dlinfo.Exists;
                            if (!exst)
                            {
                                Console.WriteLine("Database #" + dir + "# does not exist 80");
                                return wks;
                            }
                            break;

                    }

                }
                else
                {
                    ext = "";
                    System.IO.DirectoryInfo dlinfo = new System.IO.DirectoryInfo(dir);
                    if (!dlinfo.Exists)
                    {
                        Console.WriteLine("database " + dir + " does not exist 94");
                        return wks;
                    }
                }
                IWorkspaceFactory2 wsFact;
                switch (ext)
                {
                    case ".gdb":
                        wsFact = (IWorkspaceFactory2)new FileGDBWorkspaceFactoryClass();
                        break;
                    case ".mdb":
                        wsFact = (IWorkspaceFactory2)new AccessWorkspaceFactoryClass();
                        break;
                    case ".sde":
                        wsFact = (IWorkspaceFactory2)new SdeWorkspaceFactoryClass();
                        break;
                    default:
                        wsFact = (IWorkspaceFactory2)new ShapefileWorkspaceFactoryClass();
                        //wsFact = (IWorkspaceFactory2)new ESRI.ArcGIS.DataSourcesRaster.RasterWorkspaceFactoryClass();
                        //Console.WriteLine(dir);
                        wks = wsFact.OpenFromFile(dir, 0);
                        break;
                }
                if (wsFact.IsWorkspace(dir))
                {
                    try
                    {
                        wks = wsFact.OpenFromFile(dir, 0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error " + e.ToString());
                        wks = null;
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                wks = null;
            }
            return wks;

        }
        /// <summary>
        /// Creates a workspace give a directory and database name must include the extension of the db in the name
        /// </summary>
        /// <param name="dir">the directory</param>
        /// <param name="dbName">the database name</param>
        /// <returns>IWorkspace</returns>
        public IWorkspace CreateWorkSpace(string dir, string dbName)
        {
            check_dir(dir);
            IWorkspace wks = null;
            IWorkspaceName wksn = null;
            IWorkspaceFactory wksFact = null;
            string ext = "";
            try
            {
                if (System.IO.Path.HasExtension(dbName))
                {
                    ext = System.IO.Path.GetExtension(dbName).ToLower();
                }
                else
                {
                    ext = "";
                }
                switch (ext)
                {
                    case ".gdb":
                        wksFact = (IWorkspaceFactory2)new FileGDBWorkspaceFactoryClass();
                        break;
                    case ".mdb":
                        wksFact = (IWorkspaceFactory2)new AccessWorkspaceFactoryClass();
                        break;
                    case ".sde":
                        wksFact = (IWorkspaceFactory2)new SdeWorkspaceFactoryClass();
                        break;
                    default:
                        wksFact = (IWorkspaceFactory2)new ShapefileWorkspaceFactoryClass();
                        break;
                }
                if (ext == ".sde")
                {
                    wksn = wksFact.Create(dir, dbName, getProjectPS(), 0);
                }
                else
                {
                    wksn = wksFact.Create(dir, dbName, null, 0);
                }
                wks = wksFact.Open(wksn.ConnectionProperties, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return wks;
        }
        /// <summary>
        /// gets the data Type of a feature 
        /// </summary>
        /// <param name="ftr">path of the feature</param>
        /// <returns>data type</returns>
        public esriDatasetType getDataType(string ftr)
        {
            esriDatasetType x = esriDatasetType.esriDTAny;
            bool y = false;
            IWorkspace wks;
            string ftrname = System.IO.Path.GetFileName(ftr).Replace(".shp","").Replace(".dbf","").ToLower();
            if (ftrExists(ftr))
            {
                string db = parseDbStr(ftr);
                wks = OpenWorkSpace(db);
               
                IEnumDatasetName dtsne = wks.get_DatasetNames(esriDatasetType.esriDTAny);
                IDatasetName dtsn = dtsne.Next();
                IEnumDatasetName subdtsne = null;
                IDatasetName subdtsn = null;
                while (dtsn != null)
                {
                    if (dtsn.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        subdtsne = dtsn.SubsetNames;
                        subdtsn = subdtsne.Next();
                        while (subdtsn != null)
                        {
                            if (subdtsn.Name.ToLower() == ftrname)
                            {
                                x = subdtsn.Type;
                                y = true;
                                break;
                            }
                            subdtsn = subdtsne.Next();

                        }
                        if (y) break;
                    }
                    else
                    {
                        if (dtsn.Name.ToLower() == ftrname)
                        {
                            x = dtsn.Type;
                            break;
                        }
                    }
                    dtsn = dtsne.Next();
                }
            }
            return x;
        }
        /// <summary>
        /// determins if a feature exists
        /// wks = IWorkspace; ftr = ftrn name no extention
        /// </summary>
        /// <param name="wks">IWorkspace</param>
        /// <param name="ftr">name of the feature not extension</param>
        /// <returns>true if exist otherwise false</returns>
        public bool ftrExists(IWorkspace wks, string ftr)
        {
            bool x = false;
            try
            {
                if (wks != null)
                {
                    //Console.WriteLine(wks.PathName);
                    //Console.WriteLine(ftr);
                    //Console.WriteLine(wks.WorkspaceFactory.WorkspaceType);
                    IEnumDatasetName dtsne = wks.get_DatasetNames(esriDatasetType.esriDTAny);
                    IDatasetName dtsn = dtsne.Next();
                    IEnumDatasetName subdtsne = null;
                    IDatasetName subdtsn = null;
                    while (dtsn != null)
                    {
                        //Console.WriteLine(dtsn.Name);
                        if (dtsn.Type == esriDatasetType.esriDTFeatureDataset)
                        {
                            subdtsne = dtsn.SubsetNames;
                            subdtsn = subdtsne.Next();
                            while (subdtsn != null)
                            {
                                if (subdtsn.Name.ToLower() == ftr.ToLower())
                                {
                                    x = true;
                                    break;
                                }
                                subdtsn = subdtsne.Next();

                            }
                            if (x == true) break;
                        }
                        else
                        {
                            if (dtsn.Name.ToLower() == ftr.ToLower())
                            {
                                x = true;
                                break;
                            }
                        }
                        dtsn = dtsne.Next();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// returns a list of all Feature Names
        /// </summary>
        /// <param name="geoDatabase">path to the geodatabase</param>
        /// <returns> a list of string names</returns>
        public List<string> getAllFeatureNames(string geoDatabase)
        {
            return getElementNames(geoDatabase, esriDatasetType.esriDTFeatureClass);
        }
        /// <summary>
        /// returns a list of all table Names
        /// </summary>
        /// <param name="geoDatabase">path to the geodatabase</param>
        /// <returns> a list of table names</returns>
        public List<string> getAllTableNames(string geoDatabase)
        {
            return getElementNames(geoDatabase, esriDatasetType.esriDTTable);
        }
        /// <summary>
        /// returns a list of all Raster Names
        /// </summary>
        /// <param name="geoDatabase">path to the geodatabase</param>
        /// <returns> a list of string names</returns>
        public List<string> getAllRasterNames(string geoDatabase)
        {
            return getElementNames(geoDatabase,esriDatasetType.esriDTRasterDataset);
        }
        /// <summary>
        /// returns a list of all feature names for a given data type
        /// </summary>
        /// <param name="geoDatabase">path to database</param>
        /// <param name="dtype">esri data type</param>
        /// <returns>string list of names</returns>
        private List<string> getElementNames(string geoDatabase, esriDatasetType dtype)
        {
            List<string> lstFeatureClasses = new List<string>();
            if (wksExists(geoDatabase))
            {
                IWorkspace wks = OpenWorkSpace(geoDatabase);
                esriDatasetType dttype = esriDatasetType.esriDTAny;
                IEnumDataset eDatasets = wks.get_Datasets(dttype);
                IEnumDataset eDatasets2;
                IDataset dataset2;
                IDataset dataset = eDatasets.Next();
                while (dataset != null)
                {
                    dttype = dataset.Type;
                    switch (dttype)
                    {
                        case esriDatasetType.esriDTFeatureDataset:
                            eDatasets2 = dataset.Subsets;
                            dataset2 = eDatasets2.Next();
                            while (dataset2 != null)
                            {
                                if (dataset2.Type == dtype)
                                {
                                    lstFeatureClasses.Add(dataset2.Name);
                                }
                                dataset2 = eDatasets2.Next();
                            }
                            break;

                        default:
                            if (dataset.Type == dtype)
                            {
                                lstFeatureClasses.Add(dataset.Name);
                            }
                            break;
                    }
                    dataset = eDatasets.Next();
                }
            }
            return lstFeatureClasses;
        }
        /// <summary>
        /// returns a dictionary of elements by name/ elementobject
        /// </summary>
        /// <param name="geoDatabase">geodatabase path</param>
        /// <param name="dtype">data type</param>
        /// <returns>dicitonary key=name value=IName object</returns>
        public Dictionary<string, IName> getElementINames(string geoDatabase, esriDatasetType dtype)
        {
            if (wksExists(geoDatabase))
            {
                IWorkspace wks = OpenWorkSpace(geoDatabase);
                return getElementINames(wks, dtype);
            }
            else
            {
                return new Dictionary<string, IName>();
            }
            
        }
        /// <summary>
        /// returns a dictionary of elements by name/ elementobject
        /// </summary>
        /// <param name="geoDatabase">Iworkspace</param>
        /// <param name="dtype">data type</param>
        /// <returns>dicitonary key=name value=IName object</returns>
        public Dictionary<string,IName> getElementINames(IWorkspace wks, esriDatasetType dtype)
        {
            Dictionary<string,IName> lstFeatureClasses = new Dictionary<string,IName>();
            if (wks.Exists())
            {
                esriDatasetType dttype = esriDatasetType.esriDTAny;
                IEnumDataset eDatasets = wks.get_Datasets(dttype);
                IEnumDataset eDatasets2;
                IDataset dataset2;
                IDataset dataset = eDatasets.Next();
                while (dataset != null)
                {
                    dttype = dataset.Type;
                    switch (dttype)
                    {
                        case esriDatasetType.esriDTFeatureDataset:
                            eDatasets2 = dataset.Subsets;
                            dataset2 = eDatasets2.Next();
                            while (dataset2 != null)
                            {
                                if (dataset2.Type == dtype)
                                {
                                    lstFeatureClasses.Add(dataset2.BrowseName, dataset2.FullName);
                                }
                                dataset2 = eDatasets2.Next();
                            }
                            break;

                        default:
                            if (dttype == dtype)
                            {
                                lstFeatureClasses.Add(dataset.BrowseName, dataset.FullName);
                            }
                            break;
                    }
                    dataset = eDatasets.Next();
                }
            }
        
            return lstFeatureClasses;
        }
        /// <summary>
        /// returns a list of all Feature Classes given a geoDatabase
        /// </summary>
        /// <param name="geoDatabase">path</param>
        /// <returns>a list of feature classes</returns>
        public List<IFeatureClass> getAllFeatures(string geoDatabase)
        
        {
            List<IFeatureClass> lstFeatureClasses = new List<IFeatureClass>();
            if (wksExists(geoDatabase))
            {
                IWorkspace wks = OpenWorkSpace(geoDatabase);
                esriDatasetType dttype = esriDatasetType.esriDTAny;
                IEnumDataset eDatasets = wks.get_Datasets(dttype);
                IEnumDataset eDatasets2;
                IDataset dataset2;
                IDataset dataset = eDatasets.Next();
                while (dataset != null)
                {
                    dttype = dataset.Type;
                    switch (dttype)
                    {
                        case esriDatasetType.esriDTFeatureDataset:
                            eDatasets2 = dataset.Subsets;
                            dataset2 = eDatasets2.Next();
                            while (dataset2 != null)
                            {
                                if (dataset2.Type == esriDatasetType.esriDTFeatureClass)
                                {
                                    lstFeatureClasses.Add((IFeatureClass)dataset2.FullName.Open());
                                }
                                dataset2 = eDatasets2.Next();
                            }
                            break;

                        case esriDatasetType.esriDTFeatureClass:
                            lstFeatureClasses.Add((IFeatureClass)dataset.FullName.Open());
                            break;
                        default:
                            break;
                    }
                    dataset = eDatasets.Next();
                }
            }
            return lstFeatureClasses;
        }
        /// <summary>
        /// returns a list of all Rasers given a geoDatabase
        /// </summary>
        /// <param name="geoDatabase">path</param>
        /// <returns>a list of Rasters</returns>
        public List<IRasterDataset3> getAllRasters(string geoDatabase)
        {
            List<IRasterDataset3> lstRasterDataset = new List<IRasterDataset3>();
            if (wksExists(geoDatabase))
            {
                IWorkspace wks = OpenWorkSpace(geoDatabase);
                esriDatasetType dttype = esriDatasetType.esriDTAny;
                IEnumDataset eDatasets = wks.get_Datasets(dttype);
                IEnumDataset eDatasets2;
                IDataset dataset2;
                IDataset dataset = eDatasets.Next();
                while (dataset != null)
                {
                    dttype = dataset.Type;
                    switch (dttype)
                    {
                        case esriDatasetType.esriDTContainer:
                            eDatasets2 = dataset.Subsets;
                            dataset2 = eDatasets2.Next();
                            while (dataset2 != null)
                            {
                                if (dataset2.Type == esriDatasetType.esriDTRasterDataset)
                                {
                                    lstRasterDataset.Add((IRasterDataset3)dataset2.FullName.Open());
                                }
                                dataset2 = eDatasets2.Next();
                            }
                            break;

                        case esriDatasetType.esriDTRasterDataset:
                            lstRasterDataset.Add((IRasterDataset3)dataset.FullName.Open());
                            break;
                        default:
                            break;
                    }
                    dataset = eDatasets.Next();
                }
            }
            return lstRasterDataset;
        }
        /// <summary>
        /// Returns a list of all tables given a path to a geoDatabase
        /// </summary>
        /// <param name="geoDatabase">path</param>
        /// <returns>list of tables</returns>
        public List<ITable> getAllTables(string geoDatabase)
        {
            List<ITable> lstTable = new List<ITable>();
            if (wksExists(geoDatabase))
            {
                IWorkspace wks = OpenWorkSpace(geoDatabase);
                esriDatasetType dttype = esriDatasetType.esriDTAny;
                IEnumDataset eDatasets = wks.get_Datasets(dttype);
                IEnumDataset eDatasets2;
                IDataset dataset2;
                IDataset dataset = eDatasets.Next();
                while (dataset != null)
                {
                    dttype = dataset.Type;
                    switch (dttype)
                    {
                        case esriDatasetType.esriDTContainer:
                            eDatasets2 = dataset.Subsets;
                            dataset2 = eDatasets2.Next();
                            while (dataset2 != null)
                            {
                                if (dataset2.Type == esriDatasetType.esriDTTable)
                                {
                                    lstTable.Add((ITable)dataset2.FullName.Open());
                                }
                                dataset2 = eDatasets2.Next();
                            }
                            break;

                        case esriDatasetType.esriDTTable:
                            lstTable.Add((ITable)dataset.FullName.Open());
                            break;
                        default:
                            break;
                    }
                    dataset = eDatasets.Next();
                }
            }
            return lstTable;
        }
        /// <summary>
        /// determins if a feature exists
        /// </summary>
        /// <param name="ftr">path of feature</param>
        /// <returns>true if exist otherwise false</returns>
        public bool ftrExists(string ftr)
        {
            try
            {
                string db = parseDbStr(ftr);
                string ftrname = System.IO.Path.GetFileName(ftr).Replace(".shp","").Replace(".dbf","");
                //Console.WriteLine(ftrname);
                IWorkspace wks = OpenWorkSpace(db);
                if (wks != null)
                {
                    return ftrExists(wks, ftrname);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                return false;
            }
        }
        /// <summary>
        /// deters if a workspace exist given a database path
        /// </summary>
        /// <param name="dbPath">path</param>
        /// <returns>true if it exists false otherwise</returns>
        public bool wksExists(string dbPath)
        {
            IWorkspace wks = OpenWorkSpace(dbPath);
            if (wks == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Converst a string bounding box to a double array
        /// </summary>
        /// <param name="bbox">string bounding box (xmin,ymin,xmax,ymax)</param>
        /// <returns>doulble array of bounding box</returns>
        private double[] getBbox(string bbox)
        {
            List<double> bb = new List<double>();
            if (bbox != "#")
            {
                string[] bboxs = bbox.Split(new char[] { ' ' });
                foreach (string i in bboxs)
                {
                    try
                    {
                        bb.Add(System.Convert.ToDouble(i));
                    }
                    catch (Exception e)
                    {
                        bb.Add(0);
                        Console.WriteLine("Error: " + e.ToString());
                    }
                }
            }
            else
            {
                for (int i=0;i<4;i++)
                {
                    bb.Add(0);
                }
            }
            return bb.ToArray();
        }
        /// <summary>
        /// retruns a raster dataset given a path name
        /// </summary>
        /// <param name="rasterPath">full path name</param>
        /// <returns></returns>
        public IRasterDataset getRasterDataset(string rasterPath)
        {
            IRasterDataset rstDataSet = null;
            try
            {
                if (ftrExists(rasterPath) & getDataType(rasterPath) == esriDatasetType.esriDTRasterDataset)
                {
                    string rName = System.IO.Path.GetFileName(rasterPath);
                    string db = parseDbStr(rasterPath);
                    IWorkspace wks = OpenWorkSpace(db);
                    IRasterWorkspace2 rWks = (IRasterWorkspace2)wks;
                    rstDataSet = rWks.OpenRasterDataset(rName);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return rstDataSet;
        }
        /// <summary>
        /// returns a feature class given the full path to the feature
        /// </summary>
        /// <param name="ftr">path</param>
        /// <returns>Feature Class</returns>
        public IFeatureClass getFeatureClass(string ftr)
        {
            IFeatureClass nftrclass = null;
            try
            {
                if (ftrExists(ftr) & getDataType(ftr) == esriDatasetType.esriDTFeatureClass)
                {
                    IWorkspace wks = null;
                    IFeatureWorkspace fwks = null;
                    string db = parseDbStr(ftr);
                    string ftrname = System.IO.Path.GetFileName(ftr).Replace(".shp", "").Replace(".dbf", "");              
                    wks = OpenWorkSpace(db);
                    fwks = wks as IFeatureWorkspace;
                    nftrclass = fwks.OpenFeatureClass(ftrname);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return nftrclass;
        }
        /// <summary>
        /// returns a table given a path
        /// </summary>
        /// <param name="tbl">path</param>
        /// <returns>Table</returns>
        public ITable getTable(string tbl)
        {
            ITable ntbl = null;
            try
            {
                if (ftrExists(tbl) & getDataType(tbl) == esriDatasetType.esriDTTable)
                {
                    IWorkspace wks = null;
                    IFeatureWorkspace fwks = null;
                    string db = parseDbStr(tbl);
                    string ftrname = System.IO.Path.GetFileName(tbl).Replace(".shp", "").Replace(".dbf", "");
                    //get the correct workspace
                    wks = OpenWorkSpace(db);
                    fwks = wks as IFeatureWorkspace;
                    ntbl = fwks.OpenTable(ftrname);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return ntbl;
        }
        /// <summary>
        /// gets the name of the geometery field of a specified feature class
        /// </summary>
        /// <param name="ftr">path</param>
        /// <returns>the field name</returns>
        public string getGeoField(string ftr)
        {
            string geoFld = null;
            if (getDataType(ftr) == esriDatasetType.esriDTFeatureClass)
            {
                try
                {
                    geoFld = getFeatureClass(ftr).ShapeFieldName;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.ToString());
                }
            }
            return geoFld;
        }
        /// <summary>
        /// returns a seach cursor of all rows
        /// </summary>
        /// <param name="ftr">path</param>
        /// <returns>Cursor</returns>
        public ICursor getSearchCursor(string ftr)
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.WhereClause = null;
            return getCursor(ftr,"search",qry);
        }
        /// <summary>
        /// returns a seach cursor of all rows that meet a given query
        /// </summary>
        /// <param name="ftr">path</param>
        /// <param name="aqry">string representation of a query (name = 'abc')</param>
        /// <returns>Cursor</returns>
        public ICursor getSearchCursor(string ftr, string aqry)
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.WhereClause = aqry;
            return getCursor(ftr, "search", qry);
        }
        /// <summary>
        /// returns a seach cursor of all rows that meet a given query and bounding box
        /// </summary>
        /// <param name="ftr">path</param>
        /// <param name="aqry">string representation of a query (name = 'abc')</param>
        /// <param name="bbox">bounding box</param>
        /// <returns>Cursor</returns>
        public ICursor getSearchCursor(string ftr, string aqry, string bbox)
        {
            ISpatialFilter qry = new SpatialFilterClass();
            if (bbox != "#")
            {
                IEnvelope envelope = new EnvelopeClass();
                double[] bboxArr = getBbox(bbox);
                envelope.PutCoords(bboxArr[0], bboxArr[1], bboxArr[2], bboxArr[3]);
                qry.Geometry = envelope;
                qry.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                qry.GeometryField = getGeoField(ftr);
            }
            else
            {
                qry.Geometry = null;
                qry.GeometryField = null;                
            }

            if (aqry != "#") qry.WhereClause = aqry;
            else qry.WhereClause = null;
            return getCursor(ftr, "search", qry);
        }
        /// <summary>
        /// returns a update cursor of all rows
        /// </summary>
        /// <param name="ftr">path</param>
        /// <returns>Cursor</returns>
        public ICursor getUpdateCursor(string ftr)
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.WhereClause = null;
            return getCursor(ftr, "update", qry);
        }
        /// <summary>
        /// returns a update cursor of all rows that meet a given query
        /// </summary>
        /// <param name="ftr">path</param>
        /// <param name="aqry">string representation of a query (name = 'abc')</param>
        /// <returns>Cursor</returns>
        public ICursor getUpdateCursor(string ftr, string aqry)
        {
            IQueryFilter qry = new QueryFilterClass();
            qry.WhereClause = aqry;
            return getCursor(ftr, "update", qry);
        }
        /// <summary>
        /// returns a update cursor of all rows that meet a given query and bounding box
        /// </summary>
        /// <param name="ftr">path</param>
        /// <param name="aqry">string representation of a query (name = 'abc')</param>
        /// <param name="bbox">bounding box</param>
        /// <returns>Cursor</returns>
        public ICursor getUpdateCursor(string ftr, string aqry, string bbox)
        {
            ISpatialFilter qry = new SpatialFilterClass();
            if (bbox != "#")
            {
                IEnvelope envelope = new EnvelopeClass();
                double[] bboxArr = getBbox(bbox);
                envelope.PutCoords(bboxArr[0], bboxArr[1], bboxArr[2], bboxArr[3]);
                qry.Geometry = envelope;
                qry.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                qry.GeometryField = getGeoField(ftr);
            }
            else
            {
                qry.Geometry = null;
                qry.GeometryField = null;
            }

            if (aqry != "#") qry.WhereClause = aqry;
            else qry.WhereClause = null;
            return getCursor(ftr, "update", qry);
        }
        /// <summary>
        /// returns a insert cursor of all rows
        /// </summary>
        /// <param name="ftr">path</param>
        /// <returns>Cursor</returns>
        public ICursor getInsertCursor(string ftr)
        {
            IQueryFilter qry = new QueryFilterClass();
            return getCursor(ftr, "insert", qry);
        }
        private ICursor getCursor(string ftr,string curtyp,IQueryFilter qry)
        {
            ICursor cur = null;
            try
            {
                if (ftrExists(ftr))
                {
                    esriDatasetType datatype = getDataType(ftr);
                    IWorkspace wks = null;
                    IFeatureWorkspace fwks = null;
                    string db = parseDbStr(ftr);
                    //Console.WriteLine(db);
                    string ftrname = System.IO.Path.GetFileName(ftr).Replace(".shp", "").Replace(".dbf", ""); ;
                    wks = OpenWorkSpace(db);
                    fwks = wks as IFeatureWorkspace;
                    if (datatype == esriDatasetType.esriDTTable)
                    {
                        ITable tbl = fwks.OpenTable(ftrname);
                        switch (curtyp.ToLower())
                        {
                            case "search":
                                cur = tbl.Search(qry, false);
                                break;
                            case "update":
                                cur = tbl.Update(qry, false);
                                break;
                            case "insert":
                                cur = tbl.Insert(true);
                                break;
                            default:
                                cur = tbl.Search(qry, false);
                                break;
                        }
                    }
                    else if (datatype == esriDatasetType.esriDTFeatureClass)
                    {
                        IFeatureClass ftrc = fwks.OpenFeatureClass(ftrname);
                        switch (curtyp.ToLower())
                        {
                            case "search":
                                cur = ftrc.Search(qry, false) as ICursor;
                                break;
                            case "update":
                                cur = ftrc.Update(qry, false) as ICursor;
                                break;
                            case "insert":
                                cur = ftrc.Insert(true) as ICursor;
                                break;
                            default:
                                cur = ftrc.Search(qry, false) as ICursor;
                                break;
                        }
                    }
                    else
                    {
                        cur = null;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                cur = null;
            }
            return cur;
        }
        /// <summary>
        /// Recursivly creates a directory given a input directory
        /// </summary>
        /// <param name="dir">path</param>
        /// <returns>true if successful otherwise false</returns>
        private bool mkdirs(string dir)
        {
            bool x = false;
            try
            {
                dir = System.IO.Path.GetFullPath(dir);
                System.IO.DirectoryInfo ndir = new System.IO.DirectoryInfo(dir);
                if (!ndir.Exists)
                {
                    try
                    {
                        string nPath = System.IO.Path.GetDirectoryName(ndir.FullName);
                        ndir = new System.IO.DirectoryInfo(nPath);
                        while (nPath.Length != 0 && !ndir.Exists)
                        {
                            nPath = nPath.Substring(0, nPath.LastIndexOf("\\"));
                            //Console.WriteLine("'" + nPath + "'");
                            ndir = new System.IO.DirectoryInfo(nPath);
                            //Console.WriteLine(ndir.Exists);
                        }
                        if (nPath.Length <= 0)
                        {
                            Console.WriteLine("not a directory");
                            return false;
                        }
                        string[] dirs = dir.Replace(nPath + "\\", "").Split(new char[] { '\\' });
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            nPath = System.IO.Path.Combine(nPath, dirs[i]);
                            ndir = new System.IO.DirectoryInfo(nPath);
                            if (!ndir.Exists)
                            {
                                ndir.Create();
                                //Console.WriteLine("creating directory " + ndir.FullName);
                            }
                        }
                        x = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error: " + e.ToString());
                        x = false;
                    }

                }
                else
                {
                    x = true;
                }
            }
            catch (Exception e)
            {
                x = false;
                MessageBox.Show("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// checks to see if a directory exist and if it does not creates the directory
        /// </summary>
        /// <param name="dir">path</param>
        /// <returns>true if successful otherwise false</returns>
        public bool check_dir(string dir)
        {
            return mkdirs(dir);
        }
        /// <summary>
        /// creates a sde connection file outfl name of the file sv server ins instance db database ver version 
        /// owner.name method osa or dbms user and password
        /// </summary>
        /// <returns>path to connection file</returns>
        public string createSDEConnFile(string outfl, string sv, string ins, string db, string ver, string method,string user,object pass)
        
        {
            string path = null;
            //Console.WriteLine(outfl + "\n" + sv + "\n" + ins + "\n" + db + "\n" + ver + "\n" + method + "\n" + user + "\n" + pass); 
            try
            {
                string dir = System.IO.Path.GetDirectoryName(outfl);
                string fname = System.IO.Path.GetFileName(outfl);
                if (System.IO.Path.HasExtension(fname))
                {
                    string ext = System.IO.Path.GetExtension(outfl);
                    if (ext.ToLower() != ".sde")
                    {
                        fname = fname.Replace(ext, ".sde");
                    }
                }
                else
                {
                    fname = fname + ".sde";
                }
                path = System.IO.Path.Combine(dir, fname);
                System.IO.FileInfo fInfo = new System.IO.FileInfo(path);
                if (fInfo.Exists)
                {
                    fInfo.Delete();
                }
                IWorkspaceFactory2 wksF = new SdeWorkspaceFactoryClass();
                IPropertySet propSet = new PropertySetClass();
                propSet.SetProperty("SERVER", sv);
                propSet.SetProperty("INSTANCE", ins);
                propSet.SetProperty("DATABASE", db);
                propSet.SetProperty("VERSION", ver);
                propSet.SetProperty("AUTHENTICATION_MODE", method.ToUpper());
                if(!(user == null || user=="" || pass==null || pass.ToString()=="") & method.ToLower()=="dbms")
                { 
                    propSet.SetProperty("USER",user);
                    propSet.SetProperty("PASSWORD",pass);
                    //Console.WriteLine(pass.ToString());
                }
                wksF.Create(dir, fname, propSet, 0);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                path = null;
                
            }
            return path;
        }
        /// <summary>
        /// creates a sde connection file outfl name of the file sv server ins instance db database ver version 
        /// owner.name method osa
        /// </summary>
        /// <returns>path to connection file</returns>
        public string createSDEConnFile(string outfl, string sv, string ins, string db, string ver, string method)
        {
            return createSDEConnFile(outfl, sv, ins, db, ver, method, null, null);
        }
        /// <summary>
        /// creates a sde connection file outfl from default name of the file sv server ins instance db database ver version 
        /// owner.name method osa or dbms user and password
        /// </summary>
        /// <returns>path to connection file</returns>
        public string createSDEConnFile(string outfl)
        {
            return createSDEConnFile(outfl, server, instance, database, pVersion, mode, user, pass);
        }
        /// <summary>
        /// creates a sdeversion within a sde database
        /// </summary>
        /// <param name="outfl">output connection file</param>
        /// <param name="versname">name</param>
        /// <returns>path to connection file</returns>
        public string createSDEVersion(string outfl, string versname)
        {
            return createSDEVersion(outfl, versname, null);
        }
        /// <summary>
        /// Creates a SDE version on production database partent version is Project
        /// </summary>
        /// <param name="outfl">connection file</param>
        /// <param name="versname">verison name</param>
        /// <param name="sdeConn">sdeConneciton</param>
        /// <returns>new conneciton file</returns>
        public string createSDEVersion(string outfl,string versname,string sdeConn)
        {
            string x = null;
            try
            {
                IWorkspace wks;
                IWorkspaceFactory wsFact;
                IPropertySet propSet;
                string parentVersion = null;
                if (sdeConn == null)
                {
                    propSet = getProjectPS();
                    wsFact = new SdeWorkspaceFactoryClass();
                    wks = wsFact.Open(propSet, 0);
                }
                else
                {
                    Console.WriteLine("Creating version connection location  = " + sdeConn);
                    wks = OpenWorkSpace(sdeConn);
                    propSet = wks.ConnectionProperties;
                    wsFact = wks.WorkspaceFactory;
                }
                IVersionedWorkspace3 vwks = wks as IVersionedWorkspace3;
                IEnumVersionInfo eVinfo = vwks.Versions;
                IVersionInfo vinfo = eVinfo.Next();
                IVersion version = null;
                while (vinfo != null)
                {
                    string vName = vinfo.VersionName;
                    if (vName.ToLower().EndsWith(propSet.GetProperty("VERSION").ToString().ToLower()))
                    {
                        parentVersion = vName;
                        version = vwks.FindVersion(vName);
                        break;
                    }
                    vinfo = eVinfo.Next();    
                }
                if (versionExist(versname, sdeConn))
                {
                    Console.WriteLine("Error: Version " + versname + " already exists");
                    return null;
                }
                IVersion cversion = version.CreateVersion(versname);
                IWorkspace wks2 = (IWorkspace)cversion;
                wsFact.Create(System.IO.Path.GetDirectoryName(outfl), System.IO.Path.GetFileName(outfl), wks2.ConnectionProperties,0);
                x = outfl;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                x = null;
            }
            return x;
        }
        /// <summary>
        /// deletes a given version 
        /// </summary>
        /// <param name="versname">version name</param>
        /// <param name="sdeConn">parent sde connection</param>
        /// <returns>true if success otherwise false</returns>
        public bool deleteSDEVersion(string versname, string sdeConn)
        {
            bool x = true;
            try
            {
                IWorkspace wks;
                IWorkspaceFactory wsFact;
                IPropertySet propSet;
                string parentVersion = null;
                if (sdeConn == null)
                {
                    propSet = getProjectPS();
                    wsFact = new SdeWorkspaceFactoryClass();
                    wks = wsFact.Open(propSet, 0);
                }
                else
                {
                    wks = OpenWorkSpace(sdeConn);
                    propSet = wks.ConnectionProperties;
                    wsFact = wks.WorkspaceFactory;
                }
                IVersionedWorkspace3 vwks = wks as IVersionedWorkspace3;
                IEnumVersionInfo eVinfo = vwks.Versions;
                IVersionInfo vinfo = eVinfo.Next();
                IVersion version = null;
                while (vinfo != null)
                {
                    string vName = vinfo.VersionName;
                    if (vName.ToLower().EndsWith(versname.ToLower()))
                    {
                        parentVersion = vName;
                        version = vwks.FindVersion(vName);
                        break;
                    }
                    vinfo = eVinfo.Next();
                }
                if (version != null)
                {
                    version.Delete();
                }
                else
                {
                    Console.WriteLine("Error: Could not find Version");
                    x = false;
                }

            }
            catch(Exception e)
            {
                x = false;
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// unregisters a replica from a sde database
        /// </summary>
        /// <param name="repName">replicas name</param>
        /// <param name="sdeConn">parent sde conneciton</param>
        /// <returns>true if success otherwise false</returns>
        public bool unregisterReplica(string repName, string sdeConn)
        {
            bool x = true;
            try
            {
                IWorkspace wks = OpenWorkSpace(sdeConn);

                IWorkspaceReplicasAdmin2 wksRepAdmin = (IWorkspaceReplicasAdmin2)wks;
                IWorkspaceReplicas2 wksReps = (IWorkspaceReplicas2)wks;
                IEnumReplica eReps = wksReps.Replicas;
                IReplica rep = eReps.Next();
                while (rep != null)
                {
                    string rN = rep.Name;
                    if (rN.ToLower().EndsWith(repName.ToLower()))
                    {
                        wksRepAdmin.UnregisterReplica(rep, true);
                        break;
                    }
                    rep = eReps.Next();
                }
            }
            catch(Exception e)
            {
                x = false;
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// gets all version on a sde databse
        /// </summary>
        /// <returns>array of version infos</returns>
        public IVersionInfo[] getVersionsInfo()
        {
            return getVersionsInfo(null);
        }
        /// <summary>
        /// gets all version on a sde database
        /// </summary>
        /// <param name="sdeConn">sde connection</param>
        /// <returns>array of version info</returns>
        public IVersionInfo[] getVersionsInfo(string sdeConn)
        {
            List<IVersionInfo> vers = new List<IVersionInfo>();
            IWorkspace wks;
            if (sdeConn != null)
            {
                wks = OpenWorkSpace(sdeConn);
            }
            else
            {
                IPropertySet propSet = getProjectPS();
                IWorkspaceFactory wkFact = new SdeWorkspaceFactoryClass();
                wks = wkFact.Open(propSet, 0);
            }
            IVersionedWorkspace3 vwks = wks as IVersionedWorkspace3;
            IEnumVersionInfo vEnumInfo = vwks.Versions;
            IVersionInfo vInfo = vEnumInfo.Next();
            while (vInfo != null)
            {
                vers.Add(vInfo);
                vInfo = vEnumInfo.Next();
            }
            return vers.ToArray();
        }
        /// <summary>
        /// test whether a version exists
        /// </summary>
        /// <param name="versionName">version name</param>
        /// <returns>true if success otherwise false</returns>
        public bool versionExist(string versionName)
        {
            return versionExist(versionName, null);
        }
        /// <summary>
        /// test whether a version exists
        /// </summary>
        /// <param name="versionName">version name</param>
        /// <param name="sdeConn">sde connection file</param>
        /// <returns>true if success otherwise false</returns>
        public bool versionExist(string versionName, string sdeConn)
        {
            bool x = false;
            bool y = wksExists(sdeConn);
            if (y)
            {
                foreach (IVersionInfo versInfo in getVersionsInfo(sdeConn))
                {
                    string vName = versInfo.VersionName;
                    if (vName.ToLower().EndsWith(versionName.ToLower()))
                    {
                        x = true;
                        break;
                    }
                }
            }
            return x;
        }
        /// <summary>
        /// compress a sde databse given a sde connection string
        /// </summary>
        /// <param name="sdeConnection">conneciton string</param>
        /// <returns>true if it exist otherwise false</returns>
        public bool compressSdeDatabase(string sdeConnection)
        {
            bool x = false;
            try
            {
                IWorkspace wks = OpenWorkSpace(sdeConnection);
                if (wks!=null)
                {
                    if (wks.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    {
                        IVersionedWorkspace3 vwks = wks as IVersionedWorkspace3;
                        vwks.Compress();
                        x = true;
                    }
                    else
                    {
                        x = false;
                        Console.WriteLine("Not a valid sde workspace");
                    }
                }
                else
                {
                    Console.WriteLine("This is not a valid workspace!");
                    x = false;
                }



            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                x = false;
            }

            return x;
        }
        /// <summary>
        /// creates an sql connection string given a sde connection string to a sql database
        /// </summary>
        /// <param name="sdeConnection">connection file</param>
        /// <returns>connection file</returns>
        public string createSqlConnectionString(string sdeConnection)
        {
            string x = null;
            try
            {
                IWorkspace wks = OpenWorkSpace(sdeConnection);
                if (wks != null)
                {
                    IPropertySet ps = wks.ConnectionProperties;
                    string sv = ps.GetProperty("SERVER") as string;
                    string ins = ps.GetProperty("INSTANCE") as string;
                    string[] inss = ins.Split(new char[] { ':' });
                    ins = inss[inss.GetUpperBound(0)];
                    string db = ps.GetProperty("DATABASE") as string;
                    x = "Data Source=" + ins + ";Initial Catalog=" + db + ";Integrated Security=SSPI;Connection Timeout=30";
                }
                else
                {
                    Console.WriteLine("Could not get a workspace");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// shrink and rebuilds index on sde database via sqlconnection
        /// </summary>
        /// <param name="sdeConnection">connection file</param>
        /// <returns>true if sucess otherwise false</returns>
        public bool shrinkRebuildSdeIndex(string sdeConnection)
        {
            bool x = false;
            try
            {
                string sqlConStr = createSqlConnectionString(sdeConnection);
                if (sqlConStr != null)
                {
                    using (SqlConnection sqlcon = new SqlConnection(sqlConStr))
                    {
                        string db = sqlConStr.Split(new char[] { ';' })[1].Split(new char[]{'='})[1];
                        string tbl = null;
                        sqlcon.Open();
                        string cmd = "DBCC SHRINKDATABASE('" + db + "')";
                        SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
                        sqlcmd.CommandTimeout = 600;
                        sqlcmd.ExecuteNonQuery();
                        Console.WriteLine("Shrink Database Finished: True");
                        cmd = "select table_schema, table_name from information_schema.tables where table_type = 'BASE TABLE'";
                        sqlcmd.CommandText = cmd;
                        SqlDataReader sqlread =  sqlcmd.ExecuteReader();
                        List<string> tbllst = new List<string>();
                        while (sqlread.Read())
                        {
                            tbl = sqlread.GetString(0) + "." + sqlread.GetString(1);
                            tbllst.Add(tbl);
                        }
                        sqlread.Close();
                        foreach (string s in tbllst)
                        {
                            cmd = "Alter index all on " + s + " REBUILD";
                            sqlcmd.CommandText = cmd;
                            sqlcmd.ExecuteNonQuery(); 
                        }
                        sqlcon.Close();
                    }
                    x = true;
                }
                else
                {
                    Console.WriteLine("Could not make connection");
                    x = false;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                x = false;
            }
            return x;
        }
        /// <summary>
        /// backup an SDE database given a connection string and a backupfile
        /// </summary>
        /// <param name="sdeConnection">connection file</param>
        /// <param name="bkFile">name of the backupfile path</param>
        /// <returns>true if success otherwise false</returns>
        public bool backupSdeDatabase(string sdeConnection,string bkFile)
        {
            bool x = false;
            try
            {
                string sqlConStr = createSqlConnectionString(sdeConnection);
                if (sqlConStr != null)
                {
                    using (SqlConnection sqlcon = new SqlConnection(sqlConStr))
                    {
                        string db = sqlConStr.Split(new char[] { ';' })[1].Split(new char[] { '=' })[1];
                        string bk = null;
                        sqlcon.Open();
                        if (bkFile.ToLower().EndsWith(".bak"))
                        {
                            bk = bkFile.Substring(0,bkFile.Length-4) + "_" + DateTime.Now.DayOfWeek.ToString() + ".bak" ;
                        }
                        else
                        {
                            bk = bkFile + "_" + DateTime.Now.DayOfWeek.ToString() + ".bak" ;
                        }
                        mkdirs(System.IO.Path.GetDirectoryName(bk));
                        string cmd = "BACKUP DATABASE " + db + " TO DISK = '" + bk + "' WITH INIT, SKIP";
                        SqlCommand sqlcmd = new SqlCommand(cmd, sqlcon);
                        sqlcmd.CommandTimeout = 600;
                        sqlcmd.ExecuteNonQuery();
                        sqlcon.Close();
                        x = true;
                    }
                }
                else
                {
                    Console.WriteLine("Could not make connection");
                    x = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
                x = false;
            }
            return x;
        }
        /// <summary>
        /// synchronizes all 2 way child replicas with parent given sde conneciton string
        /// </summary>
        /// <param name="sdeConnection">connection string</param>
        /// <returns>true if success otherwise false</returns>
        public bool SynchronizeReplica(string sdeConnection)
        {
            return SynchronizeReplica(sdeConnection, esriReplicationAgentReconcilePolicy.esriRAResolveConflictsInFavorOfReplica1, esriReplicaSynchronizeDirection.esriReplicaSynchronizeBoth);
        }
        /// <summary>
        /// synchronizes all 2 way child replicas with parent given sde conneciton string
        /// </summary>
        /// <param name="sdeConnection">connection string</param>
        /// <param name="syncConflicts">conflict type</param>
        /// <param name="syncDirection">sync direction</param>
        /// <returns>true if success otherwise false</returns>
        public bool SynchronizeReplica(string sdeConnection, esriReplicationAgentReconcilePolicy syncConflicts,esriReplicaSynchronizeDirection syncDirection)
        {
            Console.WriteLine("Starting Synchronization");
            bool x = true;
            try
            {
                
                IWorkspace cwks = OpenWorkSpace(sdeConnection);
                if (cwks != null)
                {
                    
                    IGeoDataServer childGDS = new GeoDataServerClass();
                    IGeoDataServerInit childGDSInit = (IGeoDataServerInit)childGDS;
                    childGDSInit.InitWithWorkspace(cwks);
                    IReplicationAgent replicationAgent = new ReplicationAgentClass();
                    //need to get parent GDS and replica names
                    IGPReplicas childReps = childGDS.Replicas;
                    IGPReplica childRep = null;
                    IGPReplicas parentReps = null;
                    IGPReplica parentRep = null;
                    IGeoDataServer parentGDS = new GeoDataServerClass();
                    IWorkspaceFactory2 pWksFact = null;
                    IWorkspace pwks = null;
                    IGeoDataServerInit parentGDSInit = null;
                    bool repFind = false;
                    string msg = "";
                    string pConnectStr = null;
                    if (childReps.Count == 0)
                    {
                        Console.WriteLine("Could not find any Replicas in local database");
                    }
                    for (int i = 0; i < childReps.Count; i++)
                    {
                        childRep = childReps.get_Element(i);
                        if (childRep.Role == esriReplicaRole.esriReplicaRoleChild)
                        {
                            repFind = false;
                            msg = "";
                            pConnectStr = childRep.SibConnectionString;
                            Console.WriteLine(pConnectStr);
                            pWksFact = new SdeWorkspaceFactoryClass();
                            pwks = pWksFact.OpenFromString(pConnectStr, 0);
                            parentGDSInit = (IGeoDataServerInit)parentGDS;
                            parentGDSInit.InitWithWorkspace(pwks);
                            parentReps = parentGDS.Replicas;
                            if (parentReps.Count == 0)
                            {
                                Console.WriteLine("Could not find any replicas in Parent database");
                            }
                            for (int j = 0; j < parentReps.Count; j++)
                            {
                                parentRep = parentReps.get_Element(j);
                                if (j == 0)
                                {
                                    msg = parentRep.Name;
                                }
                                else
                                {
                                    msg = msg + "; " + parentRep.Name;
                                }
                                //use ubound
                                string[] prNameArr, crNameArr;
                                prNameArr = parentRep.Name.Split(new char[] { '.' });
                                crNameArr = childRep.Name.Split(new char[] { '.' });
                                string prName, crName;
                                prName = prNameArr[prNameArr.GetUpperBound(0)];
                                crName = crNameArr[crNameArr.GetUpperBound(0)];
                                Console.WriteLine("prName = " + prName + "\ncrName = " + crName);
                                if (prName.ToLower() == crName.ToLower())
                                {
                                    try
                                    {
                                        Console.WriteLine("\tSynchronizing with parent database");
                                        replicationAgent.SynchronizeReplica(childGDS, parentGDS, childRep, parentRep, syncConflicts, syncDirection, false);
                                        x = true;
                                        repFind = true;
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Error: Could not sync Replica " + childRep.Name + " error message = " + e.ToString());
                                        x = false;
                                        break;
                                    }
                                }
                            }
                            if (!repFind)
                            {
                                Console.WriteLine("\tCould not find Recplica named " + childRep.Name + " in Parent Database.\n\tValid names in parent include: " + msg);
                                x = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("\t" + childRep.Name + " is a parent replica. Skipping Synchronization.");
                        }
                    }  
                }
                else
                {
                    Console.WriteLine("Workspace not valid");
                    x = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                x = false;
            }
            return x;
        }
        /// <summary>
        /// returns a database path given a feature path
        /// </summary>
        /// <param name="path">the featureclass name</param>
        /// <returns></returns>
        public string getDatabasePath(string path)
        {
            return parseDbStr(path);
        }
        /// <summary>
        /// returns a dictionary of unique values of a given a layer path and field
        /// </summary>
        /// <param name="lyrPath">path to feature class</param>
        /// <param name="unqFld">unique field name</param>
        /// <param name="vlFld">the field with values</param>
        /// <returns>dictionary</returns>
        public Dictionary<string, double> getUniqueValuesByField(string lyrPath, string unqFld, string vlFld)
        {
            Dictionary<string, double> x = new Dictionary<string, double>();
            try
            {
                ICursor scur = getSearchCursor(lyrPath);
                int unFldIndex = scur.FindField(unqFld);
                int vlFldIndex = scur.FindField(vlFld);
                if (unFldIndex == -1 || vlFldIndex!= -1) return x;
                IRow srow = scur.NextRow();
                while (srow != null)
                {
                    string ky = srow.get_Value(unFldIndex).ToString();
                    if (!x.ContainsKey(ky))
                    {
                        x.Add(ky, System.Convert.ToDouble(srow.get_Value(vlFldIndex)));
                    }
                    srow = scur.NextRow();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// gets unique values of a layer give a field
        /// </summary>
        /// <param name="lyrPath"></param>
        /// <param name="Fld"></param>
        /// <returns></returns>
        public List<string> getUniqueValues(string lyrPath, string Fld)
        {
            List<string> x = new List<string>();
            try
            {
                ICursor scur = getSearchCursor(lyrPath);
                int fldIndex = scur.FindField(Fld);
                if (fldIndex == -1) return x;
                IRow srow = scur.NextRow();
                while (srow != null)
                {
                    string ky = srow.get_Value(fldIndex).ToString();
                    if (!x.Contains(ky))
                    {
                        x.Add(ky);
                    }
                    srow = scur.NextRow();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        public List<string> getUniqueValues(IFeatureClass ftrCls, string Fld)
        {
            List<string> x = new List<string>();
            try
            {
                IFeatureCursor scur = ftrCls.Search(null,false);
                int fldIndex = scur.FindField(Fld);
                if (fldIndex == -1) return x;
                IFeature srow = scur.NextFeature();
                while (srow != null)
                {
                    string ky = srow.get_Value(fldIndex).ToString();
                    if (!x.Contains(ky))
                    {
                        x.Add(ky);
                    }
                    srow = scur.NextFeature();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// gets unique summarized values (cnt, sum, min, max, mean) by unqFld for a value fld vlFld
        /// </summary>
        /// <param name="lyrPath">path</param>
        /// <param name="unqFld">unique field</param>
        /// <param name="vlFld">value field</param>
        /// <returns>dictionary of summarized values</returns>
        public Dictionary<string, Dictionary<string,double>> summarizeUniqueValuesByField(string lyrPath, string unqFld, string vlFld)
        {
            Dictionary<string, Dictionary<string,double>> x = new Dictionary<string, Dictionary<string,double>>();
            Dictionary<string, double> sumDic = new Dictionary<string,double>();
            try
            {
                ICursor scur = getSearchCursor(lyrPath);
                int unFldIndex = scur.FindField(unqFld);
                int vlFldIndex = scur.FindField(vlFld);
                if (unFldIndex == -1 || vlFldIndex != -1) return x;
                IRow srow = scur.NextRow();
                double vl;
                double cnt;
                double sumV;
                double min;
                double max;
                double mean;
                while (srow != null)
                {
                    vl = System.Convert.ToDouble(srow.get_Value(vlFldIndex));
                    string ky = srow.get_Value(unFldIndex).ToString();
                    bool kyExist = x.TryGetValue(ky, out sumDic);
                    if (!kyExist)
                    {
                        cnt = 1;
                        sumV = vl;
                        mean = sumV/cnt;
                        min = vl;
                        max = vl;
                        sumDic = new Dictionary<string, double>();
                        sumDic.Add("CNT", cnt);
                        sumDic.Add("SUM", sumV);
                        sumDic.Add("MEAN", mean);
                        sumDic.Add("MAX", max);
                        sumDic.Add("MIX", min);
                        x.Add(ky, sumDic);
                    }
                    else
                    {
                        sumDic.TryGetValue("CNT",out cnt);
                        sumDic.TryGetValue("SUM",out sumV);
                        sumDic.TryGetValue("MEAN", out mean);
                        sumDic.TryGetValue("MAX",out max);
                        sumDic.TryGetValue("MIN",out min);
                        cnt += 1;
                        sumV = sumV + vl;
                        mean = sumV / mean;
                        if (min > vl) min = vl;
                        if (max < vl) max = vl;
                        sumDic.Clear();
                        sumDic.Add("CNT", cnt);
                        sumDic.Add("SUM", sumV);
                        sumDic.Add("MEAN", mean);
                        sumDic.Add("MAX", max);
                        sumDic.Add("MIN", min);
                        x.Remove(ky);
                        x.Add(ky, sumDic);
                    }
                    srow = scur.NextRow();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// get a dicitonary of coded domain values
        /// </summary>
        /// <param name="dbPath">db path</param>
        /// <param name="domainName">name of the domain</param>
        /// <returns>Dictionary</returns>
        public Dictionary<string, object> getCodedDomainValues(string dbPath, string domainName)
        {
            Dictionary<string, object> domDic = new Dictionary<string, object>();
            IWorkspace wks = OpenWorkSpace(dbPath);
            IWorkspaceDomains2 wksDom= (IWorkspaceDomains2)wks;
            IEnumDomain eDom = wksDom.Domains;
            IDomain dom = eDom.Next();
            while (dom != null)
            {
                string domName = dom.Name.ToLower();
                if (domName.EndsWith(domainName.ToLower()))
                {
                    if (dom.Type == esriDomainType.esriDTCodedValue)
                    {
                        ICodedValueDomain cDom = (ICodedValueDomain)dom;
                        for (int i = 0; i < cDom.CodeCount; i++)
                        {
                            string cNm = cDom.get_Name(i);
                            object cVl = cDom.get_Value(i);
                            if(!domDic.ContainsKey(cNm))
                            {
                                domDic.Add(cNm, cVl);
                            }
                        }
                    }
                }
                dom = eDom.Next();
            }
            return domDic;
        }
        /// <summary>
        /// copies the feature Class table from one database to another
        /// </summary>
        /// <param name="inWks">inWorkspace</param>
        /// <param name="inObjName">object name</param>
        /// <param name="outWks">out workspace</param>
        /// <param name="dType">data type</param>
        /// <returns>messages</returns>
        public string copyFeatureClassTable(IWorkspace inWks, string inObjName, IWorkspace outWks, esriDatasetType dType)
        {
            if (!(dType == esriDatasetType.esriDTFeatureClass || dType == esriDatasetType.esriDTTable))
            {
                return "Error: must specify a FeatureClass or Table";
            }
            if (!((IWorkspace2)inWks).get_NameExists(dType, inObjName))
            {
                return "Error: Parent object does not exist!";
            }
            Console.WriteLine(inObjName);
            string x = "successfully copied " + inObjName;
            try
            {
                IDataset outWksDset = (IDataset)outWks;
                IName targetWks = outWksDset.FullName;
                IEnumName eName = new NamesEnumeratorClass();
                IEnumNameEdit eNameEdit = (IEnumNameEdit)eName;
                //need to get name of table
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)inWks;
                IObjectClass objCls;
                if (dType == esriDatasetType.esriDTTable)
                {
                    ITable inTbl = ftrWks.OpenTable(inObjName);
                    objCls = (IObjectClass)inTbl;
                }
                else
                {
                    IFeatureClass inFtr = ftrWks.OpenFeatureClass(inObjName);
                    objCls = (IObjectClass)inFtr;
                }
                IDataset tblDset = (IDataset)objCls;
                eNameEdit.Add(tblDset.FullName);
                IGeoDBDataTransfer geoTrans = new GeoDBDataTransferClass();
                IEnumNameMapping enumNameMapping = null;
                Boolean conflictsFound = geoTrans.GenerateNameMapping(eName, targetWks, out enumNameMapping);
                enumNameMapping.Reset();
                try
                {
                    if (conflictsFound)
                    {
                        Console.WriteLine("Conflicts found can't transfer");
                    }
                    else
                    {
                        geoTrans.Transfer(enumNameMapping, targetWks);
                    }
                }
                catch (Exception e)
                {
                    x = "Error in trasfering data: " + e.ToString();
                    MessageBox.Show(x);
                }
            }
            catch (Exception e)
            {
                x = "Error: " + e.ToString();
                Console.WriteLine(x);
            }
            return x;

        }
        /// <summary>
        /// returns a specific domain given a workspace and domainName
        /// </summary>
        /// <param name="wks">workspace</param>
        /// <param name="domName">name of the domain</param>
        /// <returns>domain</returns>
        private IDomain getDom(IWorkspace wks,string domName)
        {
            IDomain x = null;
            IWorkspaceDomains wksD = (IWorkspaceDomains)wks;
            try
            {
                x = wksD.get_DomainByName(domName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        private void updateDomains(ref IFields pFlds,IWorkspace cWks)
        {
            for (int i = 0; i < pFlds.FieldCount; i++)
            {
                IField fld = pFlds.get_Field(i);
                IDomain pDom = fld.Domain;
                if (pDom != null)
                {
                    string domName = pDom.Name;
                    IDomain cDom = getDom(cWks, domName);
                    if (cDom != null)
                    {
                        IFieldEdit fldE = (IFieldEdit)fld;
                        fldE.Domain_2 = cDom;
                    }

                }
            }
        }
        /// <summary>
        /// performs a simple copy of database object
        /// </summary>
        /// <param name="parentObjClass">the parent object</param>
        /// <param name="outWks">out Workspace</param>
        /// <returns>the new feature object</returns>
        public IObjectClass simpleCopyObj(object parentObjClass, IWorkspace outWks)
        {
            IObjectClass outCls = null;
            IQueryFilter qyf = new QueryFilterClass();
            IFeatureDataConverter2 fdConv2 = new FeatureDataConverterClass();
            IDataset dSet = (IDataset)parentObjClass;
            esriDatasetType pDsetType = dSet.Type;
            string bn = dSet.BrowseName;
            string sbn = bn.Substring(bn.LastIndexOf(".")+1);
            IDatasetName dSetN = (IDatasetName)dSet.FullName;
            IDatasetName dSetN2;
            IWorkspaceName wksN = (IWorkspaceName)((IDataset)outWks).FullName;
            IFields pfs;
            if (pDsetType == esriDatasetType.esriDTTable)
            {
                ITableName tbn = new TableNameClass();
                dSetN2 = (IDatasetName)tbn;
                dSetN2.Name = sbn;
                dSetN2.WorkspaceName = wksN;
                pfs = (IFields)((IClone)((IObjectClass)parentObjClass).Fields).Clone();
                updateDomains(ref pfs, outWks);
                fdConv2.ConvertTable(dSetN,qyf, null, dSetN2, pfs, "",1000, 0);
                outCls = (IObjectClass)((IName)dSetN2).Open();
            }
            else if (pDsetType == esriDatasetType.esriDTFeatureClass)
            {
                IFeatureClass pFtCls = (IFeatureClass)dSet; 
                IFeatureClassName fcn = new FeatureClassNameClass();
                IFeatureDatasetName fdn = new FeatureDatasetNameClass();
                IDatasetName dSetN3 = (IDatasetName)fdn;
                dSetN3.WorkspaceName = wksN;
                IFeatureDataset pFtDs = pFtCls.FeatureDataset;
                if (pFtDs == null)
                {
                    dSetN3 = null;
                    fdn = null;
                }
                else
                {
                    string ftDN = pFtCls.FeatureDataset.Name;
                    string ftDNs = ftDN.Substring(ftDN.LastIndexOf(".") + 1);
                    dSetN3.Name = ftDNs;
                }
                dSetN2 = (IDatasetName)fcn;
                dSetN2.Name = sbn;
                dSetN2.WorkspaceName = wksN;
                IField pSf = pFtCls.Fields.get_Field(pFtCls.FindField(pFtCls.ShapeFieldName));
                pfs = (IFields)((IClone)pFtCls.Fields).Clone();
                updateDomains(ref pfs, outWks);
                fdConv2.ConvertFeatureClass(dSetN, qyf, null, fdn, fcn, (IGeometryDef)((IClone)pSf.GeometryDef).Clone(), pfs, "", 1000, 0);
                outCls = (IObjectClass)((IName)dSetN2).Open();
            }
            else
            {
            }
            return outCls;
        }
        /// <summary>
        /// creates a tiled feature class given an input feature class for the extent and a maximum X and Y distance between tiles
        /// </summary>
        /// <param name="ftrClass">the feature class to tile path</param>
        /// <param name="maxDistX">max distance in the horizontal direction </param>
        /// <param name="maxDistY">max distance in the vertical direciton</param>
        /// <param name="outFtrCls">output of the new feature class</param>
        /// <returns>true if successful and false otherwise</returns>
        public bool tileArea(string ftrClass, int maxDistX, int maxDistY, string outFtrCls)
        {
            return tileArea(getFeatureClass(ftrClass),maxDistX,maxDistY,outFtrCls);
        }
        /// <summary>
        /// creates a tiled feature class given an input feature class for the extent and a maximum X and Y distance between tiles
        /// </summary>
        /// <param name="ftrClass">the feature class to tile</param>
        /// <param name="maxDistX">max distance in the horizontal direction </param>
        /// <param name="maxDistY">max distance in the vertical direciton</param>
        /// <param name="outFtrCls">output of the new feature class</param>
        /// <returns>true if successful and false otherwise</returns>
        public bool tileArea(IFeatureClass ftrClass, int maxDistX, int maxDistY, string outFtrCls)
        {
            bool x = true;
            try
            {
                IFields flds = new FieldsClass();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Type_2 = esriFieldType.esriFieldTypeString;
                fldE.Name_2 = "TILE";
                fldsE.AddField(fldE);
                IGeoDataset geoDs = (IGeoDataset)ftrClass;
                ISpatialReference sr = geoDs.SpatialReference;
                IEnvelope env = geoDs.Extent;
                double xMax = env.XMax;
                double xMin = env.XMin;
                double yMax = env.YMax;
                double yMin = env.YMin;
                IFeatureClass oFtrCls = createFeatureClass(outFtrCls, flds, esriGeometryType.esriGeometryPolygon, sr);
                int r = 1;
                int c = 1;
                for (int i = System.Convert.ToInt32(xMin); i < System.Convert.ToInt32(xMax); i += maxDistX)
                {
                    r = 1;
                    for (int j = System.Convert.ToInt32(yMin); j < System.Convert.ToInt32(yMax); j += maxDistY)
                    {

                        IFeature ftr = oFtrCls.CreateFeature();
                        int tileIndex = ftr.Fields.FindField("TILE");
                        IGeometry geo = ftr.Shape;
                        IPolygon poly = (IPolygon)geo;
                        IPoint pnt = new PointClass();
                        geo.SetEmpty();
                        IPointCollection pnt5 = (IPointCollection)poly;
                        pnt.X = i;
                        pnt.Y = j;
                        pnt5.AddPoint(pnt);
                        pnt.X = i+maxDistX;
                        pnt.Y = j;
                        pnt5.AddPoint(pnt);
                        pnt.X = i+maxDistX;
                        pnt.Y = j+maxDistY;
                        pnt5.AddPoint(pnt);
                        pnt.X = i;
                        pnt.Y = j+maxDistY;
                        pnt5.AddPoint(pnt);
                        poly.Close();
                        ftr.set_Value(tileIndex, "R_" + r.ToString() + " " + "C_" + c.ToString());
                        ftr.Store();
                        r += 1;
                    }
                    c += 1;
                }
                
            }
            catch (Exception e)
            {
                x = false;
                Console.WriteLine("Error: " + e.ToString());
            }
            return x;
        }
        /// <summary>
        /// creates a new featrue class
        /// </summary>
        /// <param name="outFtrCls">name of the feature class</param>
        /// <param name="atrflds">name of the fields</param>
        /// <param name="geoType">the geotype</param>
        /// <param name="sr">spatial reference</param>
        /// <returns>newly created featureclass</returns>
        public IFeatureClass createFeatureClass(string outFtrCls, IFields atrflds, esriGeometryType geoType, ISpatialReference sr)
        {
            string db = parseDbStr(outFtrCls);
            string polyFtrClsName = System.IO.Path.GetFileNameWithoutExtension(outFtrCls);
            IWorkspace2 wks = (IWorkspace2)OpenWorkSpace(db);
            return createFeatureClass(wks, polyFtrClsName,atrflds, geoType, sr);
        }
        /// <summary>
        /// creates a new featrue class
        /// </summary>
        /// <param name="wks">the workspace to create from</param>
        /// <param name="polyFtrClsName">name of the feature class</param>
        /// <param name="atrflds">name of the fields</param>
        /// <param name="geoType">the geotype</param>
        /// <param name="sr">spatial reference</param>
        /// <returns>newly created featureclass</returns>
        public IFeatureClass createFeatureClass(IWorkspace2 wks, string polyFtrClsName, IFields atrflds, esriGeometryType geoType, ISpatialReference sr)
        {
            IFeatureClass tFtr = null;
            try
            {
                IFeatureWorkspace ftrWks = (IFeatureWorkspace)wks;
                if(wks.get_NameExists(esriDatasetType.esriDTFeatureClass,polyFtrClsName))
                {
                    IDataset dSet = (IDataset)ftrWks.OpenFeatureClass(polyFtrClsName);
                    if(dSet.CanDelete())
                    {
                        dSet.Delete();
                    }
                }
                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;
                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
                for (int i = 0; i < atrflds.FieldCount; i++)
                {
                    IField fld = atrflds.get_Field(i);
                    fieldsEdit.AddField(fld);
                }
                // Find the shape field in the required fields and modify its GeometryDef to// use point geometry and to set the spatial reference.int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
                IField field = fields.get_Field(fields.FindField(fcDescription.ShapeFieldName));
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GeometryType_2 = geoType;
                geometryDefEdit.SpatialReference_2 = sr;

                // Use IFieldChecker to create a validated fields collection.
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IEnumFieldError enumFieldError = null;
                IFields validatedFields = null;
                fieldChecker.ValidateWorkspace = (IWorkspace)wks;
                fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
                tFtr = ftrWks.CreateFeatureClass(polyFtrClsName, validatedFields, ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID, esriFeatureType.esriFTSimple, fcDescription.ShapeFieldName, "");  
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
            return tFtr;

        }
        /// <summary>
        /// Creates a field within a given feature class. If the field already exist it does nothing
        /// </summary>
        /// <param name="inFtr">A valid feature class</param>
        /// <param name="nm">A string name</param>
        /// <param name="esriFieldType"> An ESRI field type</param>
        public string createField(IFeatureClass inFtr, string nm, esriFieldType FieldType)
        {
            return createField((ITable)inFtr, nm, FieldType);
        }
        public string createField(ITable inFtr, string nm, esriFieldType FieldType)
        {
            nm = getSafeFieldName(inFtr, nm);
            ISchemaLock schemaLock = (ISchemaLock)inFtr;
            try
            {
                // A try block is necessary, as an exclusive lock might not be available.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                IField fld = new FieldClass();
                IFieldEdit fldE = (IFieldEdit)fld;
                fldE.Name_2 = nm;
                fldE.Type_2 = FieldType;
                if (FieldType == esriFieldType.esriFieldTypeSmallInteger || FieldType == esriFieldType.esriFieldTypeSingle || FieldType == esriFieldType.esriFieldTypeInteger || FieldType == esriFieldType.esriFieldTypeDouble)
                {
                    fldE.DefaultValue_2 = 0;
                }
                inFtr.AddField(fldE);
            }
            catch (Exception exc)
            {
                // Handle appropriately for your application.
                Console.WriteLine(exc.Message);
                //MessageBox.Show(exc.Message);
            }
            finally
            {
                // Set the lock to shared, whether or not an error occurred.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
            return nm;


        }
        /// <summary>
        /// samples a featureclass given a sample location point file a featureclass to sample, and the sample field to get info from 
        /// </summary>
        /// <param name="sampleLocations">the sample locations</param>
        /// <param name="featureClassToSample">the feature class to sample</param>
        /// <param name="fld">the field to sample</param>
        public void sampleFeatureClass(IFeatureClass sampleLocations, IFeatureClass featureClassToSample, IField fld)
        {
            string als = fld.Name;
            if (als.Length > 12)
            {
                als = als.Substring(12);
            }
            IDataset dSet = (IDataset)featureClassToSample;
            createField(sampleLocations, als, fld.Type);
            int fldIndex = featureClassToSample.FindField(fld.Name);
            int uFldIndex = sampleLocations.FindField(als);
            IFeatureCursor sCur = featureClassToSample.Search(null, false);
            IFeature sRow = sCur.NextFeature();
            while (sRow != null)
            {
                IGeometry geo = sRow.Shape;
                object sFldV = sRow.get_Value(fldIndex);
                ISpatialFilter spFlt = new SpatialFilterClass();
                spFlt.Geometry = geo;
                spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spFlt.GeometryField = sampleLocations.ShapeFieldName;
                IFeatureCursor sCur2 = sampleLocations.Search(spFlt, false);
                IFeature sRow2 = sCur2.NextFeature();
                while (sRow2 != null)
                {
                    sRow2.set_Value(uFldIndex, sFldV);
                    sRow2.Store();
                    sRow2 = sCur2.NextFeature();
                }
                sRow = sCur.NextFeature();

            }

           
        }
        /// <summary>
        /// returns a featureclass
        /// </summary>
        /// <param name="inFeatureClass">inplut feature class IFeatureClass or string path</param>
        /// <returns>IFeature Class</returns>
        public IFeatureClass returnFeatureClass(object inFeatureClass)
        {
            IFeatureClass iF1 = null;
            if (inFeatureClass is IFeatureClass)
            {
                iF1 = (IFeatureClass)inFeatureClass;
            }
            else
            {
                iF1 = getFeatureClass(inFeatureClass.ToString());
            }
            return iF1;
        }
        /// <summary>
        /// Exports a given table to a .CSV file named after the input featureclass
        /// </summary>
        /// <param name="inTable">the input Table to export</param>
        /// <param name="outdir">the output directory to store the table</param>
        /// <returns>the name of the csv file</returns>
        public string exportTableToTxt(ITable inTable, string outdir)
        {
            string nm = ((IDataset)inTable).BrowseName + ".csv";
            string filePath = outdir + "\\" + nm;
            return exportTableToTxt(inTable, outdir, nm);
        }
        /// <summary>
        /// Exports a given table to a .CSV file named after the input featureclass
        /// </summary>
        /// <param name="inTable">the input Table to export</param>
        /// <param name="outdir">the output directory to store the table</param>
        /// <param name="fileName">the name of the csv file</param>
        /// <returns>the name of the csv file</returns>
        public string exportTableToTxt(ITable inTable,string outdir,string fileName)
        {
            string filePath = outdir + "\\" + fileName;
            List<string> vlLst = new List<string>(); 
            string vl = null;
            using (System.IO.StreamWriter sW = new System.IO.StreamWriter(filePath))
            {
                ICursor scur = inTable.Search(null, false);
                IFields flds = inTable.Fields;
                int fldCnt = flds.FieldCount;
                for(int i =0;i<fldCnt;i++)
                {
                    IField fld = flds.get_Field(i);
                    vlLst.Add(fld.Name);
                }
                sW.WriteLine(String.Join(",", vlLst.ToArray()));
                vlLst.Clear();
                IRow srow = scur.NextRow();
                while (srow != null)
                {
                    for (int i = 0; i < fldCnt; i++)
                    {
                        vl = srow.get_Value(i).ToString();
                        vlLst.Add(vl);
                    }
                    sW.WriteLine(String.Join(",", vlLst.ToArray()));
                    vlLst.Clear();
                    srow = scur.NextRow();
                }
                sW.Close();
            }
            return filePath;
        }

        public IGeometry createGeometry(IFeatureClass ftrCls)
        {
            IGeometry geometryBag = new GeometryBagClass();
            geometryBag.SpatialReference = ((IGeoDataset)ftrCls).SpatialReference;
            IGeometryCollection geoCol = (IGeometryCollection)geometryBag;
            IFeatureCursor fCur = ftrCls.Search(null, false);
            IFeature ftr = fCur.NextFeature();
            object objB = Type.Missing;
            object objA = Type.Missing;
            while (ftr != null)
            {
                IGeometry geo = ftr.Shape;
                geoCol.AddGeometry(geo, ref objB, ref objA);
                ftr = fCur.NextFeature();
            }
            return (IGeometry)geoCol;
        }

        public string getSafeFieldName(IFeatureClass ftrCls,string fldName)
        {
            string rstOut = fldName;
            foreach (string s in new string[] { " ", "\\", "/", "`", "~", "!", ".", ",", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "-" })
            {
                rstOut = rstOut.Replace(s, "_");
            }
            while (ftrCls.FindField(rstOut)>-1)
            {
                rstOut = "_" + rstOut;
            }
            return rstOut;
        }
        public string getSafeFieldName(ITable ftrCls, string fldName)
        {
            string rstOut = fldName;
            foreach (string s in new string[] { " ", "\\", "/", "`", "~", "!", ".", ",", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "-" })
            {
                rstOut = rstOut.Replace(s, "_");
            }
            while (ftrCls.FindField(rstOut) > -1)
            {
                rstOut = "_" + rstOut;
            }
            return rstOut;
        }

        public void deleteFiles(System.IO.DirectoryInfo dInfo)
        {
            foreach(System.IO.FileInfo fInfo in dInfo.GetFiles())
            {
                try
                {
                    fInfo.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public IFeatureClass explodePoints(IFeatureClass featureClass, string linkField, Dictionary<double, double> azDsDic, string outNm)
        {
            IFeatureClass outFtr = null;
            try
            {
                IFields flds = new FieldsClass();
                int linkIndex = featureClass.FindField(linkField);
                IField fld = featureClass.Fields.get_Field(linkIndex);
                IClone clFld = (IClone)fld;
                IField nFld = (IField)clFld.Clone();
                IFieldsEdit fldsE = (IFieldsEdit)flds;
                fldsE.AddField(nFld);
                IField fld2 = new FieldClass();
                IFieldEdit fld2E = (IFieldEdit)fld2;
                fld2E.Name_2 = "SubValue";
                fld2E.Type_2 = esriFieldType.esriFieldTypeInteger;
                fldsE.AddField(fld2);
                IWorkspace2 wks2 = (IWorkspace2)((IDataset)featureClass).Workspace;
                IGeoDataset geoDset = (IGeoDataset)featureClass;
                ISpatialReference sr = geoDset.SpatialReference;
                IFeatureClass newFtrCls = createFeatureClass(wks2,outNm,flds,esriGeometryType.esriGeometryPoint,sr);
                int newLinkIndex = newFtrCls.FindField(linkField);
                int subLinkIndex = newFtrCls.FindField("SubValue");
                IFeatureCursor sCur = featureClass.Search(null, false);
                IFeature sRow = sCur.NextFeature();
                IGeometry geo = null;
                while (sRow != null)
                {
                    geo = sRow.Shape;
                    IPoint pnt = (IPoint)geo;
                    int cnt = 1;
                    object linkVl = sRow.get_Value(linkIndex);
                    IFeature nFtr = newFtrCls.CreateFeature();
                    nFtr.Shape = geo;
                    nFtr.set_Value(newLinkIndex,linkVl);
                    nFtr.set_Value(subLinkIndex,cnt);
                    nFtr.Store();
                    try
                    {

                        foreach (KeyValuePair<double, double> kVp in azDsDic)
                        {
                            cnt ++;
                            double az = kVp.Key;
                            double ds = kVp.Value;
                            double nX = pnt.X + (System.Math.Sin(az * Math.PI / 180) * ds);
                            double nY = pnt.Y + (System.Math.Cos(az * Math.PI / 180) * ds);
                            IPoint nP = new PointClass();
                            nP.PutCoords(nX,nY);
                            nFtr = newFtrCls.CreateFeature();
                            nFtr.Shape = (IGeometry)nP;
                            nFtr.set_Value(newLinkIndex,linkVl);
                            nFtr.set_Value(subLinkIndex,cnt);
                            nFtr.Store();
                        }
                            
        
                    }
                    catch
                    {
                        Console.WriteLine(linkVl.ToString());
                    }
                    sRow = sCur.NextFeature();
                }
                outFtr = newFtrCls;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return outFtr;
        }

        public void delteField(IFeatureClass featureClass, string fldNm)
        {
            ISchemaLock schemaLock = (ISchemaLock)featureClass;
            try
            {
                // A try block is necessary, as an exclusive lock might not be available.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                int fldIndex = featureClass.FindField(fldNm);
                featureClass.DeleteField(featureClass.Fields.get_Field(fldIndex));
            }
            catch (Exception exc)
            {
                // Handle appropriately for your application.
                Console.WriteLine(exc.Message);
                //MessageBox.Show(exc.Message);
            }
            finally
            {
                // Set the lock to shared, whether or not an error occurred.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        }
    }
}
