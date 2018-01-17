using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HMath
    {
        public static double Mean(this IEnumerable<double> values)
        {
            return values.Average();
        }
        public static double MeanSquared(this IEnumerable<double> values)
        {
            double mean = 0;
            foreach(double value in values)
                mean += (value*value);
            mean /= values.Count();
            return mean;
        }
        public static double Median(this IList<double> values)
        {
            values = values.HSort();
            int count = values.Count();
            int index = count / 2;
            if(count%2 == 1)
            {
                return values[index];
            }
            HDebug.Assert(count%2 == 0);
            double median = (values[index-1] + values[index]) / 2;
            return median;
        }
        public static double Variance(this IEnumerable<double> values)
        {
            // Var[X] = E[X^2] - E[X]^2
            double mean = Mean(values);
            double mean2 = MeanSquared(values);
            double var = mean2 - mean*mean;
            return var;
        }
        public static Vector Mean(this IEnumerable<Vector> values)
        {
            Vector mean = new double[values.First().Size];
            foreach(Vector value in values)
                mean += value;
            mean /= values.Count();
            return mean;
        }
        public static Vector MeanWeighted(this IList<Vector> values, IList<double> weights)
        {
            Vector mean = new double[values.First().Size];
            HDebug.Assert(values.Count == weights.Count);
            for(int i=0; i<values.Count; i++)
            {
                mean += (values[i] * weights[i]);
            }
            mean /= weights.Sum();
            return mean;
        }
    }
}
