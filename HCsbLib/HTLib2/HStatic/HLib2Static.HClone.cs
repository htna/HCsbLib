using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[] HClone<T>(this T[] values)
        {
            return (T[])(values.Clone());
        }
        public static List<T> HClone<T>(this List<T> values)
        {
            return new List<T>(values);
        }
        public static IList<T> HClone<T>(this IList<T> values)
        {
            return new List<T>(values);
        }

        public static Tuple<T1            > HClone<T1            >(this Tuple<T1            > v) { return new Tuple<T1            >(v.Item1                                    ); }
        public static Tuple<T1,T2         > HClone<T1,T2         >(this Tuple<T1,T2         > v) { return new Tuple<T1,T2         >(v.Item1, v.Item2                           ); }
        public static Tuple<T1,T2,T3      > HClone<T1,T2,T3      >(this Tuple<T1,T2,T3      > v) { return new Tuple<T1,T2,T3      >(v.Item1, v.Item2, v.Item3                  ); }
        public static Tuple<T1,T2,T3,T4   > HClone<T1,T2,T3,T4   >(this Tuple<T1,T2,T3,T4   > v) { return new Tuple<T1,T2,T3,T4   >(v.Item1, v.Item2, v.Item3, v.Item4         ); }
        public static Tuple<T1,T2,T3,T4,T5> HClone<T1,T2,T3,T4,T5>(this Tuple<T1,T2,T3,T4,T5> v) { return new Tuple<T1,T2,T3,T4,T5>(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5); }
    }
}
