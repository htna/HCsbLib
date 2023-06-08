using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public partial class HObject
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsDouble(double a, double b)
        {
            if(double.IsNaN             (a) && double.IsNaN             (b)) return true;
            if(double.IsPositiveInfinity(a) && double.IsPositiveInfinity(b)) return true;
            if(double.IsNegativeInfinity(a) && double.IsNegativeInfinity(b)) return true;
            return (a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsFloat(float a, float b)
        {
            if(float.IsNaN             (a) && float.IsNaN             (b)) return true;
            if(float.IsPositiveInfinity(a) && float.IsPositiveInfinity(b)) return true;
            if(float.IsNegativeInfinity(a) && float.IsNegativeInfinity(b)) return true;
            return (a == b);
        }
    }
}
