using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperCorrelation : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            double x = 0;
            double x2 = 0;
            double y = 0;
            double xy = 0;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach (KeyValuePair<string, int> kVp in glcmDic)
            {
                string[] pairArr = kVp.Key.Split(new char[] { ':' });
                double cnt = System.Convert.ToDouble(kVp.Value);
                double p1 = System.Convert.ToDouble(pairArr[0]);
                double p2 = System.Convert.ToDouble(pairArr[1]);
                xy = xy + (p1 * p2 * cnt);
                x = x + p1 * cnt;
                x2 = x2 + p1 * p1 * cnt;
                y = y + p2 * cnt;
            }
            double cov = (xy - ((x * y) / n)) / n;
            double var = (x2 - ((x * x) / n)) / n;
            return cov/var;
        }
    }
}