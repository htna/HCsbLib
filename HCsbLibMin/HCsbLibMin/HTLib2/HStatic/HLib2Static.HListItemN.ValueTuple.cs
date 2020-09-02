﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T1[] HListItem1<T1,T2>(this IEnumerable<ValueTuple<T1,T2>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items.ToArray(); }
        public static T2[] HListItem2<T1,T2>(this IEnumerable<ValueTuple<T1,T2>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items.ToArray(); }

        public static List<T1> HListItem1<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items; }
        public static List<T2> HListItem2<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items; }
        public static List<T3> HListItem3<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<T3> items = new List<T3>(); foreach(var list_i in list) items.Add(list_i.Item3); return items; }

        public static List<T1> HListItem1<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items; }
        public static List<T2> HListItem2<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items; }
        public static List<T3> HListItem3<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<T3> items = new List<T3>(); foreach(var list_i in list) items.Add(list_i.Item3); return items; }
        public static List<T4> HListItem4<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<T4> items = new List<T4>(); foreach(var list_i in list) items.Add(list_i.Item4); return items; }

        public static T1[] HListItem1<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items.ToArray(); }
        public static T2[] HListItem2<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items.ToArray(); }
        public static T3[] HListItem3<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<T3> items = new List<T3>(); foreach(var list_i in list) items.Add(list_i.Item3); return items.ToArray(); }
        public static T4[] HListItem4<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<T4> items = new List<T4>(); foreach(var list_i in list) items.Add(list_i.Item4); return items.ToArray(); }
        public static T5[] HListItem5<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<T5> items = new List<T5>(); foreach(var list_i in list) items.Add(list_i.Item5); return items.ToArray(); }

        public static T1[] HListItem1<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items.ToArray(); }
        public static T2[] HListItem2<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items.ToArray(); }
        public static T3[] HListItem3<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T3> items = new List<T3>(); foreach(var list_i in list) items.Add(list_i.Item3); return items.ToArray(); }
        public static T4[] HListItem4<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T4> items = new List<T4>(); foreach(var list_i in list) items.Add(list_i.Item4); return items.ToArray(); }
        public static T5[] HListItem5<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T5> items = new List<T5>(); foreach(var list_i in list) items.Add(list_i.Item5); return items.ToArray(); }
        public static T6[] HListItem6<T1,T2,T3,T4,T5,T6>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6>> list) { List<T6> items = new List<T6>(); foreach(var list_i in list) items.Add(list_i.Item6); return items.ToArray(); }

        public static T1[] HListItem1<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T1> items = new List<T1>(); foreach(var list_i in list) items.Add(list_i.Item1); return items.ToArray(); }
        public static T2[] HListItem2<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T2> items = new List<T2>(); foreach(var list_i in list) items.Add(list_i.Item2); return items.ToArray(); }
        public static T3[] HListItem3<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T3> items = new List<T3>(); foreach(var list_i in list) items.Add(list_i.Item3); return items.ToArray(); }
        public static T4[] HListItem4<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T4> items = new List<T4>(); foreach(var list_i in list) items.Add(list_i.Item4); return items.ToArray(); }
        public static T5[] HListItem5<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T5> items = new List<T5>(); foreach(var list_i in list) items.Add(list_i.Item5); return items.ToArray(); }
        public static T6[] HListItem6<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T6> items = new List<T6>(); foreach(var list_i in list) items.Add(list_i.Item6); return items.ToArray(); }
        public static T7[] HListItem7<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5,T6,T7>> list) { List<T7> items = new List<T7>(); foreach(var list_i in list) items.Add(list_i.Item7); return items.ToArray(); }

        public static List<ValueTuple<T1,T2>> HListItem12<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<ValueTuple<T1,T2>> items = new List<ValueTuple<T1,T2>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2)); return items; }
        public static List<ValueTuple<T1,T3>> HListItem13<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<ValueTuple<T1,T3>> items = new List<ValueTuple<T1,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3)); return items; }
        public static List<ValueTuple<T2,T3>> HListItem23<T1,T2,T3>(this IEnumerable<ValueTuple<T1,T2,T3>> list) { List<ValueTuple<T2,T3>> items = new List<ValueTuple<T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3)); return items; }

        public static List<ValueTuple<T1,T2>> HListItem12<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T2>> items = new List<ValueTuple<T1,T2>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2)); return items; }
        public static List<ValueTuple<T1,T3>> HListItem13<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T3>> items = new List<ValueTuple<T1,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3)); return items; }
        public static List<ValueTuple<T1,T4>> HListItem14<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T4>> items = new List<ValueTuple<T1,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T4>(list_i.Item1, list_i.Item4)); return items; }
        public static List<ValueTuple<T2,T3>> HListItem23<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T2,T3>> items = new List<ValueTuple<T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3)); return items; }
        public static List<ValueTuple<T2,T4>> HListItem24<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T2,T4>> items = new List<ValueTuple<T2,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T4>(list_i.Item2, list_i.Item4)); return items; }
        public static List<ValueTuple<T3,T4>> HListItem34<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T3,T4>> items = new List<ValueTuple<T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T3,T4>(list_i.Item3, list_i.Item4)); return items; }

        public static ValueTuple<T1,T2>[] HListItem12<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T2>> items = new List<ValueTuple<T1,T2>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2>(list_i.Item1, list_i.Item2)); return items.ToArray(); }
        public static ValueTuple<T1,T3>[] HListItem13<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T3>> items = new List<ValueTuple<T1,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3>(list_i.Item1, list_i.Item3)); return items.ToArray(); }
        public static ValueTuple<T1,T4>[] HListItem14<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T4>> items = new List<ValueTuple<T1,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T4>(list_i.Item1, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T1,T5>[] HListItem15<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T5>> items = new List<ValueTuple<T1,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T5>(list_i.Item1, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T2,T3>[] HListItem23<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T3>> items = new List<ValueTuple<T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3>(list_i.Item2, list_i.Item3)); return items.ToArray(); }
        public static ValueTuple<T2,T4>[] HListItem24<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T4>> items = new List<ValueTuple<T2,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T4>(list_i.Item2, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T2,T5>[] HListItem25<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T5>> items = new List<ValueTuple<T2,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T5>(list_i.Item2, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T3,T4>[] HListItem34<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T3,T4>> items = new List<ValueTuple<T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T3,T4>(list_i.Item3, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T3,T5>[] HListItem35<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T3,T5>> items = new List<ValueTuple<T3,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T3,T5>(list_i.Item3, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T4,T5>[] HListItem45<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T4,T5>> items = new List<ValueTuple<T4,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T4,T5>(list_i.Item4, list_i.Item5)); return items.ToArray(); }

        public static List<ValueTuple<T1,T2,T3>> HListItem123<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T2,T3>> items = new List<ValueTuple<T1,T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T3>(list_i.Item1, list_i.Item2, list_i.Item3)); return items; }
        public static List<ValueTuple<T1,T2,T4>> HListItem124<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T2,T4>> items = new List<ValueTuple<T1,T2,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T4>(list_i.Item1, list_i.Item2, list_i.Item4)); return items; }
        public static List<ValueTuple<T1,T3,T4>> HListItem134<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T1,T3,T4>> items = new List<ValueTuple<T1,T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3,T4>(list_i.Item1, list_i.Item3, list_i.Item4)); return items; }
        public static List<ValueTuple<T2,T3,T4>> HListItem234<T1,T2,T3,T4>(this IEnumerable<ValueTuple<T1,T2,T3,T4>> list) { List<ValueTuple<T2,T3,T4>> items = new List<ValueTuple<T2,T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3,T4>(list_i.Item2, list_i.Item3, list_i.Item4)); return items; }

        public static ValueTuple<T1,T2,T3>[] HListItem123<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T2,T3>> items = new List<ValueTuple<T1,T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T3>(list_i.Item1, list_i.Item2, list_i.Item3)); return items.ToArray(); }
        public static ValueTuple<T1,T2,T4>[] HListItem124<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T2,T4>> items = new List<ValueTuple<T1,T2,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T4>(list_i.Item1, list_i.Item2, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T1,T2,T5>[] HListItem125<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T2,T5>> items = new List<ValueTuple<T1,T2,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T5>(list_i.Item1, list_i.Item2, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T1,T3,T4>[] HListItem134<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T3,T4>> items = new List<ValueTuple<T1,T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3,T4>(list_i.Item1, list_i.Item3, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T1,T3,T5>[] HListItem135<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T3,T5>> items = new List<ValueTuple<T1,T3,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T3,T5>(list_i.Item1, list_i.Item3, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T1,T4,T5>[] HListItem145<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T1,T4,T5>> items = new List<ValueTuple<T1,T4,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T4,T5>(list_i.Item1, list_i.Item4, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T2,T3,T4>[] HListItem234<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T3,T4>> items = new List<ValueTuple<T2,T3,T4>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3,T4>(list_i.Item2, list_i.Item3, list_i.Item4)); return items.ToArray(); }
        public static ValueTuple<T2,T3,T5>[] HListItem235<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T3,T5>> items = new List<ValueTuple<T2,T3,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T3,T5>(list_i.Item2, list_i.Item3, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T2,T4,T5>[] HListItem245<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T2,T4,T5>> items = new List<ValueTuple<T2,T4,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T2,T4,T5>(list_i.Item2, list_i.Item4, list_i.Item5)); return items.ToArray(); }
        public static ValueTuple<T3,T4,T5>[] HListItem345<T1,T2,T3,T4,T5>(this IEnumerable<ValueTuple<T1,T2,T3,T4,T5>> list) { List<ValueTuple<T3,T4,T5>> items = new List<ValueTuple<T3,T4,T5>>(); foreach(var list_i in list) items.Add(new ValueTuple<T3,T4,T5>(list_i.Item3, list_i.Item4, list_i.Item5)); return items.ToArray(); }

        public static IList<ValueTuple<T1,T2,T3>> HListItem123<T1,T2,T3>(this IEnumerable<ValueTuple<T1,ValueTuple<T2,T3>>> list) { List<ValueTuple<T1,T2,T3>> items = new List<ValueTuple<T1,T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T3>(list_i.Item1, list_i.Item2.Item1, list_i.Item2.Item2)); return items; }
        public static IList<ValueTuple<T1,T2,T3>> HListItem123<T1,T2,T3>(this IEnumerable<ValueTuple<ValueTuple<T1,T2>,T3>> list) { List<ValueTuple<T1,T2,T3>> items = new List<ValueTuple<T1,T2,T3>>(); foreach(var list_i in list) items.Add(new ValueTuple<T1,T2,T3>(list_i.Item1.Item1, list_i.Item1.Item2, list_i.Item2)); return items; }
    }
}