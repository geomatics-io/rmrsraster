using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperVariance : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            double s = 0;
            double s2 = 0;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach (KeyValuePair<string,int> kVp in glcmDic)
            {
                string[] pairArr = kVp.Key.Split(new char[] { ':' });
                double cnt = System.Convert.ToDouble(kVp.Value);
                double p1 = System.Convert.ToDouble(pairArr[0]);
                s = s + (p1 * cnt);
                s2 = s2 + (p1 * p1 * cnt);
            }
            return (s2 - ((s * s) / n)) / n;
        }
    }
}