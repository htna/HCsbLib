using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class HMath
	{
		public static int     Between(int     min, int     value, int     max) { return Math.Max(min, Math.Min(value, max)); }
		public static long    Between(long    min, long    value, long    max) { return Math.Max(min, Math.Min(value, max)); }
		public static double  Between(double  min, double  value, double  max) { return Math.Max(min, Math.Min(value, max)); }
		public static decimal Between(decimal min, decimal value, decimal max) { return Math.Max(min, Math.Min(value, max)); }

		public static double[] Between(double[] mins, double[] values, double[] maxs)
		{
			double[] between = new double[values.Length];
			for(int i=0; i<values.Length; i++)
				between[i] = Between(mins[i], values[i], maxs[i]);
			return between;
		}

	}
}
