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

    class glcmHelperVariance : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            float s = 0;
            float s2 = 0;
            float n = System.Convert.ToSingle(glcmDic.Values.Sum());
            foreach (KeyValuePair<string, int> kVp in glcmDic)
            {
                string[] pairArr = kVp.Key.Split(new char[] { ':' });
                float cnt = System.Convert.ToSingle(kVp.Value);
                float p1 = System.Convert.ToSingle(pairArr[0]);
                s = s + (p1 * cnt);
                s2 = s2 + (p1 * p1 * cnt);
            }
            return (s2 - ((s * s) / n)) / n;
        }
    }
}