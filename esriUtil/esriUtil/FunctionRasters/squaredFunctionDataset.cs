using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace esriUtil.FunctionRasters
{
    public class squaredFunctionDataset : mathFunctionBase
    {
        public override double getFunctionValue(double inValue)
        {
            return System.Convert.ToDouble(inValue * inValue);
        }

    }
}