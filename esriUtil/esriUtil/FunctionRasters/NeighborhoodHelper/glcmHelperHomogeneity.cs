using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperHomogeneity : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            float outVl = 0;
            float n =  System.Convert.ToSingle(glcmDic.Values.Sum());
            foreach (KeyValuePair<string, int> kVp in glcmDic)
            {
                string pair = kVp.Key;
                string[] pairArr = pair.Split(new char[] { ':' });
                try
                { 
                    float p1 = System.Convert.ToSingle(pairArr[0]);
                    float p2 = System.Convert.ToSingle(pairArr[1]);
                    float dif = p1 - p2;
                    float dif2 = dif * dif;
                    float cnt = System.Convert.ToSingle(kVp.Value);
                    float prob = cnt / n;
                    outVl = outVl + (prob / (1 + dif2));
                }
                catch
                {
                }
            }
            return outVl;
        }
    }
}
