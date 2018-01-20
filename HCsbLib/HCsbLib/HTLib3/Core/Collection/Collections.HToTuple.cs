using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static Tuple<T, T      > HToTuple12  <T>(this IList<T> values) { return new Tuple<T, T      >(values[0], values[1]                      ); }
        public static Tuple<T, T, T   > HToTuple123 <T>(this IList<T> values) { return new Tuple<T, T, T   >(values[0], values[1], values[2]           ); }
        public static Tuple<T, T, T, T> HToTuple1234<T>(this IList<T> values) { return new Tuple<T, T, T, T>(values[0], values[1], values[2], values[3]); }

        public static Tuple<T1, T2, T3> HToTuple123<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) { return new Tuple<T1, T2, T3>(tuple.Item1, tuple.Item2, tuple.Item3); }
    }
}
