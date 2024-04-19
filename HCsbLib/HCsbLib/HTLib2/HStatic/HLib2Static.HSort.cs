using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        //public static int[] Sort(this int[] values)
        //{
        //    List<int> lvalues = new List<int>(values);
        //    lvalues.Sort();
        //    return lvalues.ToArray();
        //}
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] HSort<T>(this IList<T> values)
            where T : IComparable<T>
        {
            List<T> lvalues = new List<T>(values);
            lvalues.Sort();
            return lvalues.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] HSort<T>(this IList<T> values, Comparison<T> comparison)
        {
            List<T> lvalues = new List<T>(values);
            lvalues.Sort(comparison);
            return lvalues.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyValuePair<T, U>[] HSort<T, U>(this IList<KeyValuePair<T, U>> values)
            where T : IComparable<T>
        {
            Comparison<KeyValuePair<T, U>> comparison = delegate(KeyValuePair<T, U> x, KeyValuePair<T, U> y)
            {
                return x.Key.CompareTo(y.Key);
            };
            return values.HSort(comparison);
        }
        public static bool HSortValueTupleTT_selftest = HDebug.IsDebuggerAttached;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTuple<T, T> HSort<T>(this ValueTuple<T, T> values)
            where T : IComparable<T>
        {
            if(HSortValueTupleTT_selftest)
            {
                HSortValueTupleTT_selftest = false;
                (int,int) _test = (2, 1).HSort();
                HDebug.Assert(_test.Item1 == 1);
                HDebug.Assert(_test.Item2 == 2);
            }

            T v1 = values.Item1;
            T v2 = values.Item2;
            if(v1.CompareTo(v2) <= 0)
                return (v1, v2);
            return (v2, v1);
        }
        public static bool HSortTupleTT_selftest = HDebug.IsDebuggerAttached;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tuple<T, T> HSort<T>(this Tuple<T, T> values)
            where T : IComparable<T>
        {
            if(HSortTupleTT_selftest)
            {
                HSortTupleTT_selftest = false;
                var _test = (new Tuple<int, int>(2, 1)).HSort();
                HDebug.Assert(_test.Item1 == 1);
                HDebug.Assert(_test.Item2 == 2);
            }

            T v1 = values.Item1;
            T v2 = values.Item2;
            if(v1.CompareTo(v2) <= 0)
                return new Tuple<T, T>(v1, v2);
            return new Tuple<T, T>(v2, v1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T,T,T    > HSort<T>(this Tuple<T,T,T    > values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new Tuple<T,T,T    >(sort[0], sort[1], sort[2]                  ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T,T,T,T  > HSort<T>(this Tuple<T,T,T,T  > values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new Tuple<T,T,T,T  >(sort[0], sort[1], sort[2], sort[3]         ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T,T,T,T,T> HSort<T>(this Tuple<T,T,T,T,T> values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new Tuple<T,T,T,T,T>(sort[0], sort[1], sort[2], sort[3], sort[4]); }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T,T,T    > HSort<T>(this ValueTuple<T,T,T    > values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new ValueTuple<T,T,T    >(sort[0], sort[1], sort[2]                  ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T,T,T,T  > HSort<T>(this ValueTuple<T,T,T,T  > values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new ValueTuple<T,T,T,T  >(sort[0], sort[1], sort[2], sort[3]         ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T,T,T,T,T> HSort<T>(this ValueTuple<T,T,T,T,T> values) where T : IComparable<T> { IList<T> sort = values.HToArray().HSort(); return new ValueTuple<T,T,T,T,T>(sort[0], sort[1], sort[2], sort[3], sort[4]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2>[] HSortByItem1<T1, T2>(this IList<Tuple<T1, T2>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2>[] HSortByItem2<T1, T2>(this IList<Tuple<T1, T2>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3>[] HSortByItem1<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3>[] HSortByItem2<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3>[] HSortByItem3<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4>[] HSortByItem1<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4>[] HSortByItem2<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4>[] HSortByItem3<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4>[] HSortByItem4<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5>[] HSortByItem1<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5>[] HSortByItem2<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5>[] HSortByItem3<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5>[] HSortByItem4<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5>[] HSortByItem5<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem1<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem2<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem3<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem4<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem5<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6>[] HSortByItem6<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> values) where T6 : IComparable<T6> { int[] idxsort = values.HListItem6().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem1<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem2<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem3<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem4<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem5<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem6<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T6 : IComparable<T6> { int[] idxsort = values.HListItem6().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Tuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem7<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> values) where T7 : IComparable<T7> { int[] idxsort = values.HListItem7().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2>[] HSortByItem1<T1, T2>(this IList<ValueTuple<T1, T2>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2>[] HSortByItem2<T1, T2>(this IList<ValueTuple<T1, T2>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3>[] HSortByItem1<T1, T2, T3>(this IList<ValueTuple<T1, T2, T3>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3>[] HSortByItem2<T1, T2, T3>(this IList<ValueTuple<T1, T2, T3>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3>[] HSortByItem3<T1, T2, T3>(this IList<ValueTuple<T1, T2, T3>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4>[] HSortByItem1<T1, T2, T3, T4>(this IList<ValueTuple<T1, T2, T3, T4>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4>[] HSortByItem2<T1, T2, T3, T4>(this IList<ValueTuple<T1, T2, T3, T4>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4>[] HSortByItem3<T1, T2, T3, T4>(this IList<ValueTuple<T1, T2, T3, T4>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4>[] HSortByItem4<T1, T2, T3, T4>(this IList<ValueTuple<T1, T2, T3, T4>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5>[] HSortByItem1<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5>[] HSortByItem2<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5>[] HSortByItem3<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5>[] HSortByItem4<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5>[] HSortByItem5<T1, T2, T3, T4, T5>(this IList<ValueTuple<T1, T2, T3, T4, T5>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem1<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem2<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem3<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem4<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem5<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6>[] HSortByItem6<T1, T2, T3, T4, T5, T6>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6>> values) where T6 : IComparable<T6> { int[] idxsort = values.HListItem6().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem1<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T1 : IComparable<T1> { int[] idxsort = values.HListItem1().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem2<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T2 : IComparable<T2> { int[] idxsort = values.HListItem2().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem3<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T3 : IComparable<T3> { int[] idxsort = values.HListItem3().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem4<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T4 : IComparable<T4> { int[] idxsort = values.HListItem4().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem5<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T5 : IComparable<T5> { int[] idxsort = values.HListItem5().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem6<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T6 : IComparable<T6> { int[] idxsort = values.HListItem6().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] HSortByItem7<T1, T2, T3, T4, T5, T6, T7>(this IList<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> values) where T7 : IComparable<T7> { int[] idxsort = values.HListItem7().HIdxSorted(); var  nvalues = values.HSelectByIndex(idxsort); return nvalues; }
    }
}
