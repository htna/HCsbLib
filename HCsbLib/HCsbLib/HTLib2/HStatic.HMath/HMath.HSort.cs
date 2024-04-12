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
        public static (double,double) HSort((double a, double b) tuple)
        {
            var ab = HSort(tuple.a, tuple.b);
            HDebug.Assert(ab.Item1 <= ab.Item2);
            return ab;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double) HSort(double a, double b)
        {
            if(a < b) return (a,b);
            else      return (b,a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double,double,double) HSort((double a, double b, double c) tuple)
        {
            var abc = HSort(tuple.a, tuple.b, tuple.c);
            HDebug.Assert(abc.Item1 <= abc.Item2 && abc.Item2 <= abc.Item3);
            return abc;
        }
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
