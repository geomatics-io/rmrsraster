using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
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
        }
        string av = "0.0";
        string ev = "0.0";
        string oleAccessdbprovider = "12.0";
        string oleExceldbprovider = "12.0";
        private string dbLc = "";
        public enum biomassTypes { BAA, TPA, J_AGB, J_SAGB, J_BAGB, J_FAGB, J_TPAGB, MDBH, MHT };
        public IFeatureClass SampleFeatureClass{get;set;}
        public string PlotCnField{get;set;}
        public string SubPlotField{get;set;}
        private geoDatabaseUtility geoDbUtil = new geoDatabaseUtility();
        private biomassTypes[] fldArr = null;
        public biomassTypes[] BiomassTypes { get { return fldArr; } set { fldArr = value; } }
        HashSet<string> unSp = new HashSet<string>();
        Dictionary<string,object[]> vlDic = new Dictionary<string,object[]>();
        //Dictionary<string, object[]> grVlDic = new Dictionary<string, object[]>();
        Dictionary<string,string> grpDic = new Dictionary<string,string>();
        bool grp = false;
        public Dictionary<string,string> GroupDic { get { return grpDic; } set { grp = true; grpDic = value; } }
        HashSet<string> unPlots = new HashSet<string>();
        Dictionary<string, object[]> bmDic = new Dictionary<string, object[]>();
        public void summarizeBiomass()
        {
            try
            {
                if (SubPlotField == null) SubPlotField = "";
                //Console.WriteLine("Creating FIlling Plots Table");
                createAndFillPlotsTable();
                //Console.WriteLine("Creating And Filling Tree Ref Table");
                createAndFillTreesRef();
                //Console.WriteLine("Adding fields");
                //need to add in groups and update unSp
                addFields();
                IQueryFilter qryFlt = new QueryFilterClass();
                string nFldNm = "";
                string fldNm = "";
                IFeatureCursor ftrCur = SampleFeatureClass.Update(null,false);
                IFeature ftr = ftrCur.NextFeature();
                int cnIndex = ftrCur.FindField(PlotCnField);
                int subIndex = ftrCur.FindField(SubPlotField);
                double divs = (43560 / (Math.PI * Math.Pow(24, 2)));
                if (SubPlotField == "" || SubPlotField == null) divs = divs / 4;
                List<string> allEstFld = Enum.GetNames(typeof(biomassTypes)).ToList();
                while (ftr!=null)
                {
                    string pCn = ftr.get_Value(cnIndex).ToString();
                    int sPl = 0;
                    if(subIndex>-1) sPl = System.Convert.ToInt32(ftr.get_Value(subIndex));
                    foreach (string t in unSp)//(biomassTypes s in fldArr)
	                {
                        string ky = pCn+"_"+sPl+"_"+t;//CN_SubPlot_species
                        object[] vls;
                        if(vlDic.TryGetValue(ky,out vls))
                        {
                        }
                        else
                        {
                            vls = new object[12];
                            vls[0] = pCn;
                            vls[1] = sPl;
                            vls[2] = t;
                            vls[3] = 0;//MDBH
                            vls[4] = 0;//AGB
                            vls[5] = 0;//StemAGB
                            vls[6] = 0;//ButtAGB
                            vls[7] = 0;//FoliageAGB
                            vls[8] = 0;//TopAGB
                            vls[9] = 0;//BAA
                            vls[10] = 0;//TPA
                            vls[11] = 0;//MHT
                        }
                        foreach (biomassTypes s in fldArr)
                        {
                            string btStr = s.ToString();
                            int arrayIndex = allEstFld.IndexOf(btStr);
                            double divs2 = divs;
                            if (arrayIndex <= 1)
                            {
                                arrayIndex = 9 + arrayIndex;
                            }
                            else
                            {
                                if (arrayIndex <= 6)
                                {
                                    arrayIndex = 2 + arrayIndex;
                                    divs2 = divs / 2000;//tons
                                }
                                else if (arrayIndex == 7)
                                {
                                    arrayIndex = 3;
                                    double d = System.Convert.ToDouble(vls[10]);
                                    if (d < 1) divs2 = 1;
                                    else
                                    {
                                        divs2 = 1 / d;
                                    }
                                }
                                else
                                {
                                    arrayIndex = 11;
                                    double d = System.Convert.ToDouble(vls[10]);
                                    if (d < 1) divs2 = 1;
                                    else
                                    {
                                        divs2 = 1 / d;
                                    }
                                }
                                
                            }

                            string spCd = vls[2].ToString();
                            fldNm = s + "_" + spCd;
                            //Console.WriteLine(fldNm);
                            int fldIndex = ftrCur.FindField(fldNm);
                            if (fldIndex == -1)
                            {
                                fldIndex = ftrCur.FindField(nFldNm.Substring(0, 10));
                            }
                            object vl = (System.Convert.ToDouble(vls[arrayIndex]))*divs2;
                            ftr.set_Value(fldIndex, vl);
                        }		 
	                }
                    ftrCur.UpdateFeature(ftr);
                    ftr = ftrCur.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ftrCur);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                //System.Environment.OSVersion.
                if (av == "0.0" || ev == "0.0")
                {
                    System.Windows.Forms.MessageBox.Show("You do not have OleDB 12.0 installed. Please download and install Office 2007 data provider from:\nhttp://www.microsoft.com/en-us/download/confirmation.aspx?id=23734", "No data provider found", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        
        private void createAndFillTreesRef()
        {
            //DataTable dtTrees = null;
            using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(dbLc))
            {
                con.Open();
                //Console.WriteLine("opened the database");
                string sql = "SELECT SPCD, JENKINS_TOTAL_B1, JENKINS_TOTAL_B2, JENKINS_STEM_WOOD_RATIO_B1, JENKINS_STEM_WOOD_RATIO_B2, JENKINS_STEM_BARK_RATIO_B1, JENKINS_STEM_BARK_RATIO_B2, JENKINS_FOLIAGE_RATIO_B1, JENKINS_FOLIAGE_RATIO_B2 FROM REF_SPECIES";
                System.Data.OleDb.OleDbCommand oleCom = new System.Data.OleDb.OleDbCommand(sql, con);
                System.Data.OleDb.OleDbDataReader oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[9];
                    oleRd.GetValues(vls);
                    bmDic.Add(vls[0].ToString(),vls);
                }
                oleRd.Close();
                sql = "SELECT CN, PLT_CN, SUBP, TREE, SPCD, DIA, HT FROM TREE WHERE DIA >= 5 and SPCD > 0";
                oleCom.CommandText = sql;
                oleRd = oleCom.ExecuteReader();
                while (oleRd.Read())
                {
                    object[] vls = new object[7];
                    oleRd.GetValues(vls);
                    string pltCn = vls[1].ToString();
                    string subp = vls[2].ToString();
                    if(SubPlotField==""||SubPlotField==null)subp="0";
                    string linkVl = pltCn+"_"+subp;
                    if(unPlots.Contains(linkVl))
                    {
                        string spcd = vls[4].ToString();
                        object[] bmVls;
                        if(bmDic.TryGetValue(spcd,out bmVls))
                        {
                            if (grp) spcd = findGrp(spcd);
                            unSp.Add(spcd);
                            string lk = linkVl+"_"+spcd.ToString();
                            object[] treeVls;
                            if (vlDic.TryGetValue(lk, out treeVls))
                            {
                    
                            }
                            else
                            {
                                treeVls = new object[12];
                                treeVls[0] = pltCn;
                                treeVls[1] = subp;
                                treeVls[2] = spcd;
                                treeVls[3] = 0;//dbh
                                treeVls[4] = 0;//AGB
                                treeVls[5] = 0;//StemAGB
                                treeVls[6] = 0;//ButtAGB
                                treeVls[7] = 0;//FoliageAGB
                                treeVls[8] = 0;//TopAGB
                                treeVls[9] = 0;//BAA
                                treeVls[10] = 0;//TPA
                                treeVls[11] = 0;//MeanHT
                                vlDic.Add(lk,treeVls);
                            }
                            //Console.WriteLine(System.Convert.ToDouble(vls[5]).ToString());
                            double diam = System.Convert.ToDouble(vls[5]);//diameter
                            double ht = System.Convert.ToDouble(vls[6]);//height
                            treeVls[3] = System.Convert.ToDouble(treeVls[3])+diam;
                            double j_tbm = Math.Exp(System.Convert.ToDouble(bmVls[1])+System.Convert.ToDouble(bmVls[2])*Math.Log(diam*2.54))*2.2046;//AGB
                            double j_sbm = j_tbm*Math.Exp(System.Convert.ToDouble(bmVls[3])+System.Convert.ToDouble(bmVls[4])/ (2.54*diam));//SAGB
                            double j_bbm = j_tbm*Math.Exp(System.Convert.ToDouble(bmVls[5])+System.Convert.ToDouble(bmVls[6])/ (2.54*diam));//BAGB
                            double j_fbm = j_tbm*Math.Exp(System.Convert.ToDouble(bmVls[7])+System.Convert.ToDouble(bmVls[8])/ (2.54*diam));//FAGB
                            double j_tpbm = j_tbm - j_sbm - j_bbm - j_fbm;//TPAGB
                            treeVls[4] = System.Convert.ToDouble(treeVls[4]) + j_tbm;
                            treeVls[5] = System.Convert.ToDouble(treeVls[5]) + j_sbm;
                            treeVls[6] = System.Convert.ToDouble(treeVls[6]) + j_bbm;
                            treeVls[7] = System.Convert.ToDouble(treeVls[7]) + j_fbm;
                            treeVls[8] = System.Convert.ToDouble(treeVls[8]) + j_tpbm;
                            treeVls[9] = System.Convert.ToDouble(treeVls[9]) + 0.005454 * Math.Pow(diam, 2);//BA
                            treeVls[10] = System.Convert.ToDouble(treeVls[10]) + 1.0;//trees
                            treeVls[11] = System.Convert.ToDouble(treeVls[11]) + ht;
                        }
                    }
                }
                oleRd.Close();
                con.Close();
            }
        }

        private string findGrp(string spcd)
        {
            string outVl;
            if(grpDic.TryGetValue(spcd,out outVl))
            {
                return outVl;
            }
            else
            {
                return spcd;
            }
        }


        private void createAndFillPlotsTable()
        {
            //DataTable dTblPlots = dSet.Tables.Add("Table", "tblPlots");
            //dTblPlots.Columns.Add("PLT_CN",System.Type.GetType("System.String"));
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = PlotCnField;
            if (SubPlotField != "" && SubPlotField != null) qf.SubFields = PlotCnField + "," + SubPlotField;
            IFeatureCursor fCur = SampleFeatureClass.Search(qf, true);
            int cnIndex = fCur.FindField(PlotCnField);
            int subIndex = fCur.FindField(SubPlotField);
            IFeature ftr = fCur.NextFeature();
            while (ftr != null)
            {
                string plCn = ftr.get_Value(cnIndex).ToString();
                string subPlt = "0";
                if (subIndex > -1)
                {
                    object subPltObj = ftr.get_Value(subIndex);
                    if (subPltObj != null)
                    {
                        subPlt = subPltObj.ToString();
                    }
                }
                string pltID = plCn+"_"+subPlt;
                unPlots.Add(pltID);
                ftr = fCur.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fCur);
        }
        private void addFields()
        {
            foreach (string s in unSp )
            {
                foreach (biomassTypes t in fldArr)
                {
                    string fldNm = t.ToString() + "_" + s;
                    if (SampleFeatureClass.FindField(fldNm) > -1)
                    {
                    }
                    else
                    {
                        geoDbUtil.createField(SampleFeatureClass, fldNm, esriFieldType.esriFieldTypeDouble);
                    }
                }
            }
        }
    }
}
