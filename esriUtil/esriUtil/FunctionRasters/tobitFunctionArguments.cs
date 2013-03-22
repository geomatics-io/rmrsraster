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
    class tobitFunctionArguments
    {
        public tobitFunctionArguments()
        {
            rsUtil = new rasterUtil();
        }
        public tobitFunctionArguments(rasterUtil rasterUtility)
        {
            rsUtil = rasterUtility;
        }
        private IRaster inrs = null;
        private rasterUtil rsUtil = null;
        public ESRI.ArcGIS.Geodatabase.IRaster InRasterCoefficients 
        { 
            get 
            { 
                return inrs; 
            } 
            set 
            {
                inrs = rsUtil.returnRaster(value, rstPixelType.PT_FLOAT);
            } 
        }
        private List<float[]> slopes = new List<float[]>();//float array = intercept followed by betas
        public List<float[]> Slopes 
        { 
            get 
            { 
                return slopes; 
            } 
            set 
            { 
                slopes = value;
            } 
        }
        private string tobitfl = "";
        public string TobitModelPath { set { tobitfl = value; setSlopes(); } }
        private string[] slrsNm = null;
        private string[] outrasterbandnames = null;
        public string[] OutRasterNamesOrder { get { return outrasterbandnames; } }
        public string[] SlopeRasterNames { get { return slrsNm; } }
        private void setSlopes()
        {
            slopes.Clear();
            using (System.IO.StreamReader sR = new System.IO.StreamReader(tobitfl))
            {
                string ln = sR.ReadLine();
                string[] oFlds = ln.Split(new char[] { ',' });
                List<string> fldNames = (from string s in oFlds select s.ToUpper()).ToList();
                int intIndex = fldNames.IndexOf("\"INTERCEPT\"");
                int intName = fldNames.IndexOf("\"NAME OF VARIABLE\"");
                if (intName == -1) intName = fldNames.IndexOf("\"DEPENDENT VARIABLE\"");
                List<string> fn = new List<string>();
                for (int i = intIndex; i < oFlds.Count(); i++)
                {
                    fn.Add(fldNames[i]);
                }
                slrsNm = fn.ToArray();
                ln = sR.ReadLine();
                List<string> outBandLst = new List<string>();
                while (ln != null)
                {
                    string[] sVls = ln.Split(new char[] { ',' });
                    outBandLst.Add(sVls[intName]);
                    List<float> fVls = new List<float>();
                    for (int i = intIndex; i < oFlds.Count(); i++)
                    {
                        string vl = sVls[i];
                        if (!rsUtil.isNumeric(vl)) vl = "0";
                        fVls.Add(System.Convert.ToSingle(vl));

                    }
                    slopes.Add(fVls.ToArray());
                    ln = sR.ReadLine();
                }
                outrasterbandnames = outBandLst.ToArray();
                sR.Close();
            }

        }
        private double censoredValue = 0;
        public double CensoredValue { get { return censoredValue; } set { censoredValue = value; } }
        public IRaster OutRaster
        {
            get
            {
                
                    IRaster rs = rsUtil.getBand(inrs, 0);
                    IRaster rsC = rsUtil.constantRasterFunction(rs, 0);
                    IRasterBandCollection rsBc = new RasterClass();
                    for (int i = 0; i < slopes.Count; i++)
                    {
                        rsBc.AppendBands((IRasterBandCollection)rsC);
                    }
                
                return (IRaster)rsBc;

            }
        }
     }
}