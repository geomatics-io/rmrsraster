using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    class glcmHelperRange : glcmFunctionDataset
    {
        public override object getTransformedValue(Dictionary<string, int> glcmDic)
        {
            float maxVl = 0;
            float minVl = 1;
            float n = System.Convert.ToSingle(glcmDic.Values.Sum());
            foreach (int i in glcmDic.Values)
            {
                float prob = System.Convert.ToSingle(i) / n;
                if (prob > maxVl)
                {
                    maxVl = prob;
                }
                if (prob < minVl)
                {
                    minVl = prob;
                }
            }
            return maxVl-minVl;
        }
    }
}
