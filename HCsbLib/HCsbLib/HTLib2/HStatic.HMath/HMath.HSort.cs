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
    }
}
