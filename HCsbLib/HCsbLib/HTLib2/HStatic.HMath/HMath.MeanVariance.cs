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
        public static double MeanInRange(this IEnumerable<double> values, double range_min, double range_max)
        {
            HDebug.Assert(range_min <= range_max + 1.0E-20);
            double mean = 0;
            int    cont = 0;
            foreach(var value in values)
            {
                if(value < range_min) continue;
                if(range_max < value) continue;
                mean += value;
                cont ++;
            }
            mean /= cont;
            return mean;
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
        public static Vector Variance(this IEnumerable<Vector> values, Vector mean)
        {
            int size = values.First().Size;
            Vector var = new double[size];

            foreach(Vector value in values)
            {
                for(int i=0; i<size; i++)
                {
                    double diffi = mean[i] - value[i];
                    var[i] += (diffi * diffi);
                }
            }
            var /= values.Count();
            return var;
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
        public static double MeanDist2(this IEnumerable<Vector> values)
        {
            double meandist2 = 0;
            foreach(Vector value in values)
            {
                double dist2 = value.Dist2;
                meandist2 += dist2;
            }
            meandist2 /= values.Count();
            return meandist2;
        }
        public static double MeanDist2(this IEnumerable<Vector> values, Vector reference)
        {
            double meandist2 = 0;
            foreach(Vector value in values)
            {
                double dist2 = (value - reference).Dist2;
                meandist2 += dist2;
            }
            meandist2 /= values.Count();
            return meandist2;
        }
    }
}
