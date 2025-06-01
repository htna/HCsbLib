using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumItems<T>(this Tuple<T        > tup) { yield return tup.Item1; }
        public static IEnumerable<T> HEnumItems<T>(this Tuple<T,T      > tup) { yield return tup.Item1; yield return tup.Item2; }
        public static IEnumerable<T> HEnumItems<T>(this Tuple<T,T,T    > tup) { yield return tup.Item1; yield return tup.Item2; yield return tup.Item3; }
        public static IEnumerable<T> HEnumItems<T>(this Tuple<T,T,T,T  > tup) { yield return tup.Item1; yield return tup.Item2; yield return tup.Item3; yield return tup.Item4; }
        public static IEnumerable<T> HEnumItems<T>(this Tuple<T,T,T,T,T> tup) { yield return tup.Item1; yield return tup.Item2; yield return tup.Item3; yield return tup.Item4; yield return tup.Item5; }

        public static ValueTuple<T1,T2> HTupleItem12<T1,T2,T3>(this ValueTuple<T1,T2,T3> tuple) { return new ValueTuple<T1,T2>(tuple.Item1, tuple.Item2); }
        public static ValueTuple<T1,T3> HTupleItem13<T1,T2,T3>(this ValueTuple<T1,T2,T3> tuple) { return new ValueTuple<T1,T3>(tuple.Item1, tuple.Item3); }
        public static ValueTuple<T2,T3> HTupleItem23<T1,T2,T3>(this ValueTuple<T1,T2,T3> tuple) { return new ValueTuple<T2,T3>(tuple.Item2, tuple.Item3); }

        public static ValueTuple<T1,T2> HTupleItem12<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T1,T2>(tuple.Item1, tuple.Item2); }
        public static ValueTuple<T1,T3> HTupleItem13<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T1,T3>(tuple.Item1, tuple.Item3); }
        public static ValueTuple<T1,T4> HTupleItem14<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T1,T4>(tuple.Item1, tuple.Item4); }
        public static ValueTuple<T2,T3> HTupleItem23<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T2,T3>(tuple.Item2, tuple.Item3); }
        public static ValueTuple<T2,T4> HTupleItem24<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T2,T4>(tuple.Item2, tuple.Item4); }
        public static ValueTuple<T3,T4> HTupleItem34<T1,T2,T3,T4>(this ValueTuple<T1,T2,T3,T4> tuple) { return new ValueTuple<T3,T4>(tuple.Item3, tuple.Item4); }

        public static ValueTuple<T1,T2> HTupleItem12<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T1,T2>(tuple.Item1, tuple.Item2); }
        public static ValueTuple<T1,T3> HTupleItem13<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T1,T3>(tuple.Item1, tuple.Item3); }
        public static ValueTuple<T1,T4> HTupleItem14<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T1,T4>(tuple.Item1, tuple.Item4); }
        public static ValueTuple<T1,T5> HTupleItem15<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T1,T5>(tuple.Item1, tuple.Item5); }
        public static ValueTuple<T2,T3> HTupleItem23<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T2,T3>(tuple.Item2, tuple.Item3); }
        public static ValueTuple<T2,T4> HTupleItem24<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T2,T4>(tuple.Item2, tuple.Item4); }
        public static ValueTuple<T2,T5> HTupleItem25<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T2,T5>(tuple.Item2, tuple.Item5); }
        public static ValueTuple<T3,T4> HTupleItem34<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T3,T4>(tuple.Item3, tuple.Item4); }
        public static ValueTuple<T3,T5> HTupleItem35<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T3,T5>(tuple.Item3, tuple.Item5); }
        public static ValueTuple<T4,T5> HTupleItem45<T1,T2,T3,T4,T5>(this ValueTuple<T1,T2,T3,T4,T5> tuple) { return new ValueTuple<T4,T5>(tuple.Item4, tuple.Item5); }

        public static ValueTuple<T1,T2> HTupleItem12<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T1,T2>(tuple.Item1, tuple.Item2); }
        public static ValueTuple<T1,T3> HTupleItem13<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T1,T3>(tuple.Item1, tuple.Item3); }
        public static ValueTuple<T2,T3> HTupleItem23<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T2,T3>(tuple.Item2, tuple.Item3); }
        public static ValueTuple<T1,T4> HTupleItem14<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T1,T4>(tuple.Item1, tuple.Item4); }
        public static ValueTuple<T2,T4> HTupleItem24<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T2,T4>(tuple.Item2, tuple.Item4); }
        public static ValueTuple<T3,T4> HTupleItem34<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T3,T4>(tuple.Item3, tuple.Item4); }
        public static ValueTuple<T1,T5> HTupleItem15<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T1,T5>(tuple.Item1, tuple.Item5); }
        public static ValueTuple<T2,T5> HTupleItem25<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T2,T5>(tuple.Item2, tuple.Item5); }
        public static ValueTuple<T3,T5> HTupleItem35<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T3,T5>(tuple.Item3, tuple.Item5); }
        public static ValueTuple<T4,T5> HTupleItem45<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T4,T5>(tuple.Item4, tuple.Item5); }
        public static ValueTuple<T1,T6> HTupleItem16<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T1,T6>(tuple.Item1, tuple.Item6); }
        public static ValueTuple<T2,T6> HTupleItem26<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T2,T6>(tuple.Item2, tuple.Item6); }
        public static ValueTuple<T3,T6> HTupleItem36<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T3,T6>(tuple.Item3, tuple.Item6); }
        public static ValueTuple<T4,T6> HTupleItem46<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T4,T6>(tuple.Item4, tuple.Item6); }
        public static ValueTuple<T5,T6> HTupleItem56<T1,T2,T3,T4,T5,T6>(this ValueTuple<T1,T2,T3,T4,T5,T6> tuple) { return new ValueTuple<T5,T6>(tuple.Item5, tuple.Item6); }
    }
}
