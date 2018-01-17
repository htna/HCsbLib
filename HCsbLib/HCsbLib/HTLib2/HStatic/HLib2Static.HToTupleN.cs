using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static Tuple<T, T      > HToTuple2<T>(this IList<T> values) { return new Tuple<T, T      >(values[0], values[1]                      ); }
        public static Tuple<T, T, T   > HToTuple3<T>(this IList<T> values) { return new Tuple<T, T, T   >(values[0], values[1], values[2]           ); }
        public static Tuple<T, T, T, T> HToTuple4<T>(this IList<T> values) { return new Tuple<T, T, T, T>(values[0], values[1], values[2], values[3]); }

        public static Tuple<T, U> HToTuple<T, U>(this KeyValuePair<T, U> values) { return new Tuple<T, U>(values.Key, values.Value); }
    }
}
