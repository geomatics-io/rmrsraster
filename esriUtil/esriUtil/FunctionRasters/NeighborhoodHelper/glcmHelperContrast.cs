using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperContrast : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string,int> glcmDic)
        {
            double outVl = 0;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach(KeyValuePair<string,int> kVp in glcmDic)
            {
                string pair = kVp.Key;
                string[] pairArr = pair.Split(new char[] { ':' });
                double p1 = System.Convert.ToDouble(pairArr[0]);
                double p2 = System.Convert.ToDouble(pairArr[1]);
                double dif = p1 - p2;
                double dif2 = dif * dif;
                double cnt = System.Convert.ToDouble(kVp.Value);
                //Console.WriteLine("Pair = " + pair.ToString() + " Count = " + cnt.ToString());
                double prob = cnt / n;
                outVl = outVl + (dif2 * prob);
            }
            return outVl;
        }
    }
}
