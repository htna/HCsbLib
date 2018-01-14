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
    }
}
