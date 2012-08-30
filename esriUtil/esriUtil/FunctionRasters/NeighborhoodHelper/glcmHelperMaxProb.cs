using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperMaxProb : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            double outVl = 0;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach (int i in glcmDic.Values)
            {
                double prob = System.Convert.ToDouble(i) / n;
                if (prob > outVl)
                {
                    outVl = prob;
                }
            }
            return outVl;
        }
    }
}