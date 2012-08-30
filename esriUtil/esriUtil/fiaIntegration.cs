using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.Data;
using ESRI.ArcGIS.Geodatabase;

namespace esriUtil
{
    public class fiaIntegration
    {
        public fiaIntegration(string dbLocation)
        {
            setDbProvider();
            string ext = System.IO.Path.GetExtension(dbLocation).ToLower();
            string firstP = "Provider=Microsoft.ACE.OLEDB."+ oleAccessdbprovider +";Data Source=";
            string lastP = ";Persist Security Info=False;";
            switch (ext)
            {
                case ".xls":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + ";HDR=YES\";";
                    break;
                case ".xlsx":
                    firstP = "Provider=Microsoft.ACE.OLEDB." + oleExceldbprovider + ";Data Source=";
                    lastP = ";Extended Properties=\"Excel " + oleExceldbprovider + " Xml;HDR=YES\";";
                    break;
                case ".mdb":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";User Id=admin;Password=;";
                    break;
                case ".txt":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
                    break;
                case ".dbf":
                    firstP = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                    lastP = ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
                    break;
                case ".dbc":
                    firstP = "Provider=vfpoledb;Data Source=";
                    lastP = ";Collating Sequence=machine;";
                    break;
                default:
                    break;
            }
            dbLc = firstP +dbLocation + lastP;
            //Console.WriteLine(dbLc);
        }

        private void setDbProvider()
        {
            av = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Access)+".0";
            ev = oleExceldbprovider = msofficeHelper.returnMajorVersion(msofficeHelper.OfficeComponent.Excel) + ".0";
            //if (av != "0.0")
            //{
            //    oleAccessdbprovider = av;
            //}
            //if (ev != "0.0")
            //{
            //    oleExceldbprovider = ev;
            //}
            //Console.WriteLine(av);
            //Console.WriteLine(ev);

        }
        ~fiaIntegration()
        {
            dSet.Dispose();
        }
        string av = "0.0";
        string ev = "0.0";
        string oleAccessdbprovider = "12.0";
        string oleExceldbprovider = "12.0";
        private string dbLc = "";
        public enum biomassTypes { BAA, TPA, J_AGB, J_SAGB, J_BAGB, J_FAGB, J_TPAGB };
        public IFeatureClass SampleFeatureClass{get;set;}
        public string PlotCnField{get;set;}
        public string SubPlotField{get;set;}
        private geoDatabaseUtility geoDbUtil = new geoDatabaseUtility();
        private string[] uniqueSpecies = null;
        private biomassTypes[] fldArr = null;
        public biomassTypes[] BiomassTypes { get { return fldArr; } set { fldArr = value; } }
        DataSet dSet = new DataSet("AGB");
        public void summarizeBiomass()
        {
            try
            {
                List<string> unCn = geoDbUtil.getUniqueValues(SampleFeatureClass, PlotCnField);
                string ps = "";
                if (SampleFeatureClass.Fields.get_Field(SampleFeatureClass.FindField(PlotCnField)).Type == esriFieldType.esriFieldTypeString)
                {
                    ps = "\"";
                }
                string treeWhereClause = "Tree.Plt_CN = " + ps + String.Join(ps + " or Tree.PLT_CN = " + ps, unCn.ToArray()) + ps;
                string sqlTreeAGB = "SELECT TREE.PLT_CN, TREE.PLOT, TREE.SUBP, TREE.TREE, TREE.INVYR, TREE.STATUSCD, TREE.SPCD, REF_SPECIES.COMMON_NAME, TREE.DIA, TREE.ACTUALHT, Exp(REF_SPECIES.JENKINS_TOTAL_B1+REF_SPECIES.JENKINS_TOTAL_B2*Log(TREE.DIA*2.54))*2.2046 AS J_TBM, J_TBM*Exp(REF_SPECIES.JENKINS_STEM_WOOD_RATIO_B1+(REF_SPECIES.JENKINS_STEM_WOOD_RATIO_B2/(2.54*TREE.DIA))) AS J_SBM, J_TBM*Exp(REF_SPECIES.JENKINS_STEM_BARK_RATIO_B1+(REF_SPECIES.JENKINS_STEM_BARK_RATIO_B2/(2.54*TREE.DIA))) AS J_BBM, J_TBM*Exp(REF_SPECIES.JENKINS_FOLIAGE_RATIO_B1+(REF_SPECIES.JENKINS_FOLIAGE_RATIO_B2/(2.54*TREE.DIA))) AS J_FBM, J_TBM-J_SBM-J_BBM-J_FBM AS J_TPBM, 0.005454*DIA^2 AS BA FROM TREE INNER JOIN REF_SPECIES ON TREE.SPCD = REF_SPECIES.SPCD WHERE (TREE.DIA >= 5 and (" + treeWhereClause + ")) ORDER BY TREE.PLT_CN, TREE.SUBP, TREE.TREE";
                string sqlAGBPlot = "SELECT qryLbsByTree.PLT_CN, qryLbsByTree.SUBP, qryLbsByTree.TREE.INVYR, qryLbsByTree.SPCD, qryLbsByTree.COMMON_NAME, ((Sum([BA]))*(43560/(3.141592654*24^2))) AS BAA, (Count([DIA]))*(43560/(3.141592654*24^2)) AS TPA, ((Sum([J_TBM]))*(43560/(3.141592654*24^2)))/2000 AS J_AGB, (Sum([J_SBM])*(43560/(3.141592654*24^2)))/2000 AS J_SAGB, (Sum([J_BBM])*(43560/(3.141592654*24^2)))/2000 AS J_BAGB, (Sum([J_FBM])*(43560/(3.141592654*24^2)))/2000 AS J_FAGB, (Sum([J_TPBM])*(43560/(3.141592654*24^2)))/2000 AS J_TPAGB FROM (" + sqlTreeAGB + ") AS qryLbsByTree GROUP BY qryLbsByTree.PLT_CN, qryLbsByTree.SUBP, qryLbsByTree.TREE.INVYR, qryLbsByTree.SPCD, qryLbsByTree.COMMON_NAME ORDER BY qryLbsByTree.PLT_CN, qryLbsByTree.SUBP, qryLbsByTree.TREE.INVYR;";
                //esriUtil.Forms.RunningProcess.frmRunningProcessDialog rd = new Forms.RunningProcess.frmRunningProcessDialog(false);
                //rd.addMessage(sqlAGBPlot);
                //rd.enableClose();
                //rd.Show();
                using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
                {
                    //Console.WriteLine(con.ConnectionString);
                    con.Open();
                    //Console.WriteLine("opened the database");
                    System.Data.OleDb.OleDbDataAdapter olDbAdp = new System.Data.OleDb.OleDbDataAdapter(sqlAGBPlot, con);
                    olDbAdp.TableMappings.Add("Table", "tblAGB");
                    olDbAdp.Fill(dSet);
                    string qry = "SELECT DISTINCT TREE.SPCD FROM TREE WHERE (TREE.DIA >= 5 and (" + treeWhereClause + "))";
                    System.Data.OleDb.OleDbDataAdapter olDbAdpSp = new System.Data.OleDb.OleDbDataAdapter(qry, con);
                    olDbAdpSp.TableMappings.Add("Table", "tblSp");
                    olDbAdpSp.Fill(dSet);
                    con.Close();
                    getUniqueSpecies(dSet.Tables[1]);
                    addFields();
                    updateFetureClass();
                    dSet.Tables.Remove("tblSp");

                }
            }
            catch
            {
                //System.Environment.OSVersion.
                if (av == "0.0" || ev == "0.0")
                {
                    System.Windows.Forms.MessageBox.Show("You do not have OleDB 12.0 installed. Please download and install Office 2007 data provider from:\nhttp://www.microsoft.com/en-us/download/confirmation.aspx?id=23734", "No data provider found", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        private void getUniqueSpecies(DataTable tbl)
        {
            List<string> spLst = new List<string>();
            int cnt = tbl.Rows.Count;
            for (int i = 0; i < cnt; i++)
            {
                DataRow dr = tbl.Rows[i];
                spLst.Add(dr[0].ToString());
            }
            uniqueSpecies = spLst.ToArray();
        }
        private void addFields()
        {
            foreach (string s in uniqueSpecies)
            {
                foreach (biomassTypes t in fldArr)
                {
                    string fldNm = t.ToString() + "_" + s;
                    if (SampleFeatureClass.FindField(fldNm) > -1)
                    {
                        geoDbUtil.delteField(SampleFeatureClass,fldNm);
                    }
                   geoDbUtil.createField(SampleFeatureClass, fldNm, esriFieldType.esriFieldTypeDouble);

                }
            }
        }
        private void updateFetureClass()
        {
            DataTable dTbl = dSet.Tables["tblAGB"];
            IQueryFilter qryFlt = new QueryFilterClass();
            int rCnt = dTbl.Rows.Count;
            string nFldNm = "";
            string fldNm = "";
            IFeatureCursor ftrCur = null;
            IFeature ftr = null;
            for (int i = 0; i < rCnt; i++)
            {
                DataRow dr = dTbl.Rows[i];
                string spCd = dr["SPCD"].ToString();
                string pltCn = dr["PLT_CN"].ToString();
                string subPlt = dr["SUBP"].ToString();
                qryFlt.WhereClause = PlotCnField + " = '" + pltCn + "' and " + SubPlotField + " = " + subPlt;
                ftrCur = SampleFeatureClass.Search(qryFlt,false);
                ftr = ftrCur.NextFeature();
                foreach (biomassTypes s in fldArr)
                {
                    foreach (string t in uniqueSpecies)
                    {
                        if (t != spCd)
                        {
                            nFldNm = s.ToString() + "_" + t;
                            int nFldIndex = ftrCur.FindField(nFldNm);
                            if (nFldIndex == -1)
                            {
                                nFldIndex = ftrCur.FindField(nFldNm.Substring(0, 10));
                            }
                            
                            object cVl = ftr.get_Value(nFldIndex);
                            if (Convert.IsDBNull(cVl) || cVl == null || Double.IsNaN(Convert.ToDouble(cVl)))
                            {
                                ftr.set_Value(nFldIndex, 0);
                            }
                            
                        }
                        else
                        {
                            fldNm = s + "_" + spCd;
                            int fldIndex = ftrCur.FindField(fldNm);
                            if (fldIndex == -1)
                            {
                                fldIndex = ftrCur.FindField(nFldNm.Substring(0, 10));
                            }
                            object vl = dr[s.ToString()];
                            ftr.set_Value(fldIndex, vl);
                        }
                    }
                }
                ftr.Store();
            }
            qryFlt.WhereClause = fldNm + " is NULL";
            ftrCur = SampleFeatureClass.Search(qryFlt,false);
            ftr = ftrCur.NextFeature();
            while (ftr != null)
            {
                foreach (string s in uniqueSpecies)
                {
                    foreach (biomassTypes t in fldArr)
                    {
                        fldNm = t.ToString() + "_" + s;
                        //Console.WriteLine(fldNm);
                        int fldIndex = ftrCur.FindField(fldNm);
                        ftr.set_Value(fldIndex, 0);
                    }
                }
                ftr.Store();
                ftr = ftrCur.NextFeature();
            }


        }


    }
}
