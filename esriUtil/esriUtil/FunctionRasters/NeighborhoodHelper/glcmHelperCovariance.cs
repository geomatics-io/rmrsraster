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
    class glcmHelperCovariance : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            float x = 0;
            float y = 0;
            float xy = 0;
            float n = System.Convert.ToSingle(glcmDic.Values.Sum());
            foreach (KeyValuePair<string, int> kVp in glcmDic)
            {
                string[] pairArr = kVp.Key.Split(new char[] { ':' });
                float cnt = System.Convert.ToSingle(kVp.Value);
                float p1 = System.Convert.ToSingle(pairArr[0]);
                float p2 = System.Convert.ToSingle(pairArr[1]);
                xy = xy + (p1 * p2 * cnt);
                x = x + p1 * cnt;
                y = y + p2 * cnt;
            }
            return (xy - ((x * y) / n)) / n;
        }
    }
}