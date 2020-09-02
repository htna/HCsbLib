using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        public static double HPow2(this double x)
        {
            return x * x;
        }
        public static double HPow3(this double x)
        {
            return x * x * x;
        }
        public static double HPow(this double x, double y)
        {
            return Math.Pow(x, y);
        }
    }
}
