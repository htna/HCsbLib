using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
	public static partial class HMath
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SortIncr<T>(ref T a, ref T b) where T : IComparable<T> { if(a.CompareTo(b) > 0) Swap(ref a, ref b); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SortDecr<T>(ref T a, ref T b) where T : IComparable<T> { if(a.CompareTo(b) < 0) Swap(ref a, ref b); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SortIncr<T>(ref (T,T) ab) where T : IComparable<T> { if(ab.Item1.CompareTo(ab.Item2) > 0) Swap(ref ab.Item1, ref ab.Item2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SortDecr<T>(ref (T,T) ab) where T : IComparable<T> { if(ab.Item1.CompareTo(ab.Item2) < 0) Swap(ref ab.Item1, ref ab.Item2); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double) HSort((double a, double b) tuple) { return HSort(tuple.a, tuple.b); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double) HSort(double a, double b)
        {
            if(a < b) return (a,b);
            else      return (b,a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double,double) HSort((double a, double b, double c) tuple) { return HSort(tuple.a, tuple.b, tuple.c); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double,double) HSort(double a, double b, double c)
        {
            if(a < b)
            {
                // a < b
                if(b < c) // b < c
                    return (a,b,c);
                else
                {
                    // c < b
                    if(a < c) return (a,c,b);
                    else      return (c,a,b);
                }
            }
            else
            {
                // b < a
                if(a < c) // a < c
                    return (b,a,c);
                else
                {
                    // c < a
                    if(b < c) return (b,c,a);
                    else      return (c,b,a);
                }
            }
        }
    }
}
