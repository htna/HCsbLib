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
        public static double HMean(this System.Runtime.CompilerServices.ITuple values)
        {
            double mean = 0;
            for(int i=0; i<values.Length; i++)
                mean += (double)values[i];
            mean /= values.Length;
            return mean;
        }
    }
}
