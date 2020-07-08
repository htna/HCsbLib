using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T1> HEnumItem1<T1,T2>(this IEnumerable<ValueTuple<T1,T2>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2>(this IEnumerable<ValueTuple<T1,T2>> list) { foreach(var list_i in list) yield return list_i.Item2; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return list_i.Item3; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return list_i.Item4; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return list_i.Item5; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item5; }
        public static IEnumerable<T6> HEnumItem6<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { foreach(var list_i in list) yield return list_i.Item6; }

        public static IEnumerable<T1> HEnumItem1<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item1; }
        public static IEnumerable<T2> HEnumItem2<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item2; }
        public static IEnumerable<T3> HEnumItem3<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item3; }
        public static IEnumerable<T4> HEnumItem4<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item4; }
        public static IEnumerable<T5> HEnumItem5<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item5; }
        public static IEnumerable<T6> HEnumItem6<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item6; }
        public static IEnumerable<T7> HEnumItem7<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { foreach(var list_i in list) yield return list_i.Item7; }

        public static IEnumerable<ValueTuple<T1,T2>> HEnumItem12<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2); }
        public static IEnumerable<ValueTuple<T1,T3>> HEnumItem13<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3); }
        public static IEnumerable<ValueTuple<T2,T3>> HEnumItem23<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3); }

        public static IEnumerable<ValueTuple<T1,T2>> HEnumItem12<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2); }
        public static IEnumerable<ValueTuple<T1,T3>> HEnumItem13<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3); }
        public static IEnumerable<ValueTuple<T1,T4>> HEnumItem14<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T4>(list_i.Item1, list_i.Item4); }
        public static IEnumerable<ValueTuple<T2,T3>> HEnumItem23<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3); }
        public static IEnumerable<ValueTuple<T2,T4>> HEnumItem24<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T4>(list_i.Item2, list_i.Item4); }
        public static IEnumerable<ValueTuple<T3,T4>> HEnumItem34<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { foreach(var list_i in list) yield return new ValueTuple<T3,T4>(list_i.Item3, list_i.Item4); }

        public static IEnumerable<ValueTuple<T1,T2>> HEnumItem12<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2); }
        public static IEnumerable<ValueTuple<T1,T3>> HEnumItem13<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3); }
        public static IEnumerable<ValueTuple<T1,T4>> HEnumItem14<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T4>(list_i.Item1, list_i.Item4); }
        public static IEnumerable<ValueTuple<T1,T5>> HEnumItem15<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T1,T5>(list_i.Item1, list_i.Item5); }
        public static IEnumerable<ValueTuple<T2,T3>> HEnumItem23<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3); }
        public static IEnumerable<ValueTuple<T2,T4>> HEnumItem24<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T4>(list_i.Item2, list_i.Item4); }
        public static IEnumerable<ValueTuple<T2,T5>> HEnumItem25<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T2,T5>(list_i.Item2, list_i.Item5); }
        public static IEnumerable<ValueTuple<T3,T4>> HEnumItem34<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T3,T4>(list_i.Item3, list_i.Item4); }
        public static IEnumerable<ValueTuple<T3,T5>> HEnumItem35<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T3,T5>(list_i.Item3, list_i.Item5); }
        public static IEnumerable<ValueTuple<T4,T5>> HEnumItem45<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { foreach(var list_i in list) yield return new ValueTuple<T4,T5>(list_i.Item4, list_i.Item5); }
    }
}
