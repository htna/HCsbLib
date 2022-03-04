using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HStatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals(double a, double b)
        {
            if(double.IsNaN             (a) && double.IsNaN             (b)) return true;
            if(double.IsPositiveInfinity(a) && double.IsPositiveInfinity(b)) return true;
            if(double.IsNegativeInfinity(a) && double.IsNegativeInfinity(b)) return true;
            return (a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals(double[] a, double[] b)
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Length != b.Length  ) return false;
            for(int i=0; i<a.Length; i++)
                if(HEquals(a[i], b[i]) == false)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HEquals<T>(T[] a, T[] b)
            where T : IEquatable<T>
        {
            if(a == null && b == null) return true;
            if(a == null && b != null) return false;
            if(a != null && b == null) return false;
            if(a.Length != b.Length  ) return false;
            for(int i=0; i<a.Length; i++)
                if(a[i].Equals(b[i]) == false)
                    return false;
            return true;
        }
    }
}
