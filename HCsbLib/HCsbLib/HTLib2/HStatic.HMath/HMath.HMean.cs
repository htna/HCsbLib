using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HMath
    {
        public static double HMean(this IList<double> values)
        {
            return values.Mean();
        }
    }
}
