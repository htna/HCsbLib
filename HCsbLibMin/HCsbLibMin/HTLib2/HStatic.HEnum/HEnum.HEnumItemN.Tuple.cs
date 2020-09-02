using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T1> HEnumItem1<T1,T2>(this IEnumerable<Tuple<T1,T2>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2>(this IEnumerable<Tuple<T1,T2>> list) { foreach(var list_i in list) yield return list_i.Item2; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3>(this IEnumerable<Tuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3>(this IEnumerable<Tuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3>(this IEnumerable<Tuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item3; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4>(this IEnumerable<Tuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4>(this IEnumerable<Tuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4>(this IEnumerable<Tuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4>(this IEnumerable<Tuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item4; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5>(this IEnumerable<Tuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5>(this IEnumerable<Tuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5>(this IEnumerable<Tuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5>(this IEnumerable<Tuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5>(this IEnumerable<Tuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item5; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item5; }
        public static IEnumerable<T6> HEnumItem6<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item6; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item5; }
        public static IEnumerable<T6> HEnumItem6<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item6; }
        public static IEnumerable<T7> HEnumItem7<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item7; }
    }
}
