using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperASM : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            float outVl = 0;
            float n = System.Convert.ToSingle(glcmDic.Values.Sum());
            foreach (int i in glcmDic.Values)
            {
                float prob = System.Convert.ToSingle(i) / n;
                outVl = outVl + (prob * prob);
            }
            return outVl;
        }
    }
}