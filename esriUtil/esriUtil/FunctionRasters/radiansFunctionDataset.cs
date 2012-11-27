using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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