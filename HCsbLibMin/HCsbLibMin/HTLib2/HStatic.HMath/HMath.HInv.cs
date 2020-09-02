using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        public static double[] HInv(this IList<double> values)
        {
            double[] results = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                results[i] = 1.0 / values[i];
            return results;
        }
        public static double[] HSqrt(this IList<double> values)
        {
            double[] results = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                results[i] = Math.Sqrt(values[i]);
            return results;
        }
        public static double[] HSquare(this IList<double> values)
        {
            return values.HPow2();
        }
        public static double[] HPow(this IList<double> values, double pow)
        {
            double[] results = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                results[i] = Math.Pow(values[i], pow);
            return results;
        }
        public static double[] HPow2(this IList<double> values)
        {
            double[] results = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                results[i] = values[i] * values[i];
            return results;
        }
        public static double[] HMul(this IList<double> values, double mul)
        {
            double[] results = new double[values.Count];
            for(int i=0; i<values.Count; i++)
                results[i] = mul * values[i];
            return results;
        }
    }
}
