using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil.Statistics
{
    public abstract class dataPrepBase
    {
        public enum modelTypes { Accuracy, LinearRegression, MvlRegression, LogisticRegression, PLR, RandomForest, SoftMax, Cart, L3, CovCorr, PCA, Cluster, TTEST }
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        public geoDatabaseUtility GeoUtil { get { return geoUtil; } }
        private string intablepath = "";
        public string InTablePath
        {
            get
            {
                return intablepath;

            }
            set
            {
                intablepath = value;
                intable = geoUtil.getTable(intablepath);
            }
        }
        private ITable intable = null;
        public ITable InTable 
        {
            get
            {
                return intable;
            }
            set 
            { 
                intable=value;
                IDataset dSet = (IDataset)intable;
                intablepath = dSet.Workspace.PathName + "\\" + dSet.BrowseName;
            } 
        }
        public string[] DependentFieldNames { get;set;}
        public string[] IndependentFieldNames { get ;set; }
        public string[] ClassFieldNames { get; set; }
        public Dictionary<string, List<string>> UniqueClassValues
        {
            get
            {
                return getUniqueClassValues();
            }
        }

        private Dictionary<string, List<string>> getUniqueClassValues()
        {
            Dictionary<string, List<string>> outDic = new Dictionary<string, List<string>>();
            
            if (ClassFieldNames == null || ClassFieldNames.Length == 0) return outDic;
            string cFldn1 = ClassFieldNames[0];
            if (cFldn1 == "") return outDic;
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = String.Join(",", ClassFieldNames);
            HashSet<string>[] hshStrgLst = new HashSet<string>[ClassFieldNames.Length];
            //Console.WriteLine("HashStrgLstLength = " + hshStrgLst.Length);
            int[] fldIndexArr = new int[ClassFieldNames.Length];
            ICursor cur = InTable.Search(qf, false);
            for (int i = 0; i < ClassFieldNames.Length; i++)
            {
                fldIndexArr[i] = cur.FindField(ClassFieldNames[i]);
                hshStrgLst[i] = new HashSet<string>();
            }
            IRow rw = cur.NextRow();
            while (rw != null)
            {
                for (int i = 0; i < fldIndexArr.Length; i++)
                {
                    int indVl = fldIndexArr[i];
                    string vl = rw.get_Value(indVl).ToString();
                    //Console.WriteLine(vl);
                    //Console.WriteLine(indVl.ToString());
                    hshStrgLst[i].Add(vl);
                }
                rw = cur.NextRow();
            }
            
            for (int i = 0; i < ClassFieldNames.Length; i++)
            {
                outDic.Add(ClassFieldNames[i], hshStrgLst[i].ToList());
            }
            return outDic;
        }
        public abstract double[,] getMatrix();
        public abstract double[] getArray(string varName);
    }
}
