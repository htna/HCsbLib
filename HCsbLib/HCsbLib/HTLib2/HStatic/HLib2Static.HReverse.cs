using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] HReverse<T>(this IList<T> values)
        {
            List<T> list = new List<T>(values);
            list.Reverse();
            return list.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static Tuple<         T2,T1> HReverse<T1,T2         >(this Tuple<T1,T2         > val) { return new Tuple<         T2,T1>(                              val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static Tuple<      T3,T2,T1> HReverse<T1,T2,T3      >(this Tuple<T1,T2,T3      > val) { return new Tuple<      T3,T2,T1>(                    val.Item3,val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static Tuple<   T4,T3,T2,T1> HReverse<T1,T2,T3,T4   >(this Tuple<T1,T2,T3,T4   > val) { return new Tuple<   T4,T3,T2,T1>(          val.Item4,val.Item3,val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static Tuple<T5,T4,T3,T2,T1> HReverse<T1,T2,T3,T4,T5>(this Tuple<T1,T2,T3,T4,T5> val) { return new Tuple<T5,T4,T3,T2,T1>(val.Item5,val.Item4,val.Item3,val.Item2,val.Item1); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static ValueTuple<         T2,T1> HReverse<T1,T2         >(this ValueTuple<T1,T2         > val) { return new ValueTuple<         T2,T1>(                              val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static ValueTuple<      T3,T2,T1> HReverse<T1,T2,T3      >(this ValueTuple<T1,T2,T3      > val) { return new ValueTuple<      T3,T2,T1>(                    val.Item3,val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static ValueTuple<   T4,T3,T2,T1> HReverse<T1,T2,T3,T4   >(this ValueTuple<T1,T2,T3,T4   > val) { return new ValueTuple<   T4,T3,T2,T1>(          val.Item4,val.Item3,val.Item2,val.Item1); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static ValueTuple<T5,T4,T3,T2,T1> HReverse<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> val) { return new ValueTuple<T5,T4,T3,T2,T1>(val.Item5,val.Item4,val.Item3,val.Item2,val.Item1); }
    }
}
