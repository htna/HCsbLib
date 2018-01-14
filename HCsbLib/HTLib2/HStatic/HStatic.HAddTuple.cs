using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T1, T2, T3, T4, T5>[] HAddTuple<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5>[] list, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) { return list.HAdd(new Tuple<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5)); }
        public static Tuple<T1, T2, T3, T4    >[] HAddTuple<T1, T2, T3, T4    >(this Tuple<T1, T2, T3, T4    >[] list, T1 v1, T2 v2, T3 v3, T4 v4       ) { return list.HAdd(new Tuple<T1, T2, T3, T4    >(v1, v2, v3, v4    )); }
        public static Tuple<T1, T2, T3        >[] HAddTuple<T1, T2, T3        >(this Tuple<T1, T2, T3        >[] list, T1 v1, T2 v2, T3 v3              ) { return list.HAdd(new Tuple<T1, T2, T3        >(v1, v2, v3        )); }
        public static Tuple<T1, T2            >[] HAddTuple<T1, T2            >(this Tuple<T1, T2            >[] list, T1 v1, T2 v2                     ) { return list.HAdd(new Tuple<T1, T2            >(v1, v2            )); }
        public static Tuple<T1                >[] HAddTuple<T1                >(this Tuple<T1                >[] list, T1 v1                            ) { return list.HAdd(new Tuple<T1                >(v1                )); }

        public static void HUpdateAddTuple<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) { list.Add(new Tuple<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5)); }
        public static void HUpdateAddTuple<T1, T2, T3, T4    >(this IList<Tuple<T1, T2, T3, T4    >> list, T1 v1, T2 v2, T3 v3, T4 v4       ) { list.Add(new Tuple<T1, T2, T3, T4    >(v1, v2, v3, v4    )); }
        public static void HUpdateAddTuple<T1, T2, T3        >(this IList<Tuple<T1, T2, T3        >> list, T1 v1, T2 v2, T3 v3              ) { list.Add(new Tuple<T1, T2, T3        >(v1, v2, v3        )); }
        public static void HUpdateAddTuple<T1, T2            >(this IList<Tuple<T1, T2            >> list, T1 v1, T2 v2                     ) { list.Add(new Tuple<T1, T2            >(v1, v2            )); }
        public static void HUpdateAddTuple<T1                >(this IList<Tuple<T1                >> list, T1 v1                            ) { list.Add(new Tuple<T1                >(v1                )); }
    }
}
