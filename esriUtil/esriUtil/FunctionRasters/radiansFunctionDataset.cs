using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class radiansFunctionDataset : mathFunctionBase
    {
        public override double getFunctionValue(double inValue)
        {
            return inValue*Math.PI/180;
        }

    }
}