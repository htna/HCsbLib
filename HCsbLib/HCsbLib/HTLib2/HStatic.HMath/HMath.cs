using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
        public static double HRound(double value, double resolution)
        {
            double rvalue = Math.Round(value / resolution) * resolution;
            return rvalue;
        }
    }
}
