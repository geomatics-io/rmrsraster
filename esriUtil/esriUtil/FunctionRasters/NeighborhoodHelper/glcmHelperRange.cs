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
            double maxVl = 0;
            double minVl = 1;
            double n = System.Convert.ToDouble(glcmDic.Values.Sum());
            foreach (int i in glcmDic.Values)
            {
                double prob = System.Convert.ToDouble(i) / n;
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
