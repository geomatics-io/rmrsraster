using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperMean : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            double outVl = 0;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach (KeyValuePair<string, int> kVp in glcmDic)
            {
                string pair = kVp.Key;
                string[] pairArr = pair.Split(new char[] { ':' });
                double p1 = System.Convert.ToDouble(pairArr[0]);
                double cnt = System.Convert.ToDouble(kVp.Value);
                outVl = outVl + p1*cnt;
            }
            return outVl/n;
        }
    }
}