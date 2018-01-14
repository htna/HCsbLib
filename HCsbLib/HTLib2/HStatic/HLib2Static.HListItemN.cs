using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T1[] HListItem1<T1, T2>(this IList<Tuple<T1, T2>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return items; }
        public static T2[] HListItem2<T1, T2>(this IList<Tuple<T1, T2>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return items; }

        public static List<T1> HListItem1<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return new List<T1>(items); }
        public static List<T2> HListItem2<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return new List<T2>(items); }
        public static List<T3> HListItem3<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return new List<T3>(items); }

        public static List<T1> HListItem1<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return new List<T1>(items); }
        public static List<T2> HListItem2<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return new List<T2>(items); }
        public static List<T3> HListItem3<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return new List<T3>(items); }
        public static List<T4> HListItem4<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T4[] items = new T4[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item4; return new List<T4>(items); }

        public static T1[] HListItem1<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return items; }
        public static T2[] HListItem2<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return items; }
        public static T3[] HListItem3<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return items; }
        public static T4[] HListItem4<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { T4[] items = new T4[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item4; return items; }
        public static T5[] HListItem5<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { T5[] items = new T5[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item5; return items; }

        public static T1[] HListItem1<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return items; }
        public static T2[] HListItem2<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return items; }
        public static T3[] HListItem3<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return items; }
        public static T4[] HListItem4<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T4[] items = new T4[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item4; return items; }
        public static T5[] HListItem5<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T5[] items = new T5[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item5; return items; }
        public static T6[] HListItem6<T1, T2, T3, T4, T5, T6>(this IList<Tuple<T1, T2, T3, T4, T5, T6>> list) { T6[] items = new T6[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item6; return items; }

        public static T1[] HListItem1<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return items; }
        public static T2[] HListItem2<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return items; }
        public static T3[] HListItem3<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return items; }
        public static T4[] HListItem4<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T4[] items = new T4[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item4; return items; }
        public static T5[] HListItem5<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T5[] items = new T5[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item5; return items; }
        public static T6[] HListItem6<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T6[] items = new T6[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item6; return items; }
        public static T7[] HListItem7<T1, T2, T3, T4, T5, T6, T7>(this IList<Tuple<T1, T2, T3, T4, T5, T6, T7>> list) { T7[] items = new T7[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item7; return items; }

        public static List<Tuple<T1, T2>> HListItem12<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { Tuple<T1,T2>[] items = new Tuple<T1, T2>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T2>(list[i].Item1, list[i].Item2); return new List<Tuple<T1, T2>>(items); }
        public static List<Tuple<T1,T3>> HListItem13<T1,T2,T3>(this IList<Tuple<T1,T2,T3>> list) { Tuple<T1,T3>[] items = new Tuple<T1,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T3>(list[i].Item1, list[i].Item3); return new List<Tuple<T1,T3>>(items); }
        public static List<Tuple<T2,T3>> HListItem23<T1,T2,T3>(this IList<Tuple<T1,T2,T3>> list) { Tuple<T2,T3>[] items = new Tuple<T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2,T3>(list[i].Item2, list[i].Item3); return new List<Tuple<T2,T3>>(items); }

        public static List<Tuple<T1,T2>> HListItem12<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T2>[] items = new Tuple<T1,T2>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2>(list[i].Item1, list[i].Item2); return new List<Tuple<T1,T2>>(items); }
        public static List<Tuple<T1,T3>> HListItem13<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T3>[] items = new Tuple<T1,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T3>(list[i].Item1, list[i].Item3); return new List<Tuple<T1,T3>>(items); }
        public static List<Tuple<T1,T4>> HListItem14<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T4>[] items = new Tuple<T1,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T4>(list[i].Item1, list[i].Item4); return new List<Tuple<T1,T4>>(items); }
        public static List<Tuple<T2,T3>> HListItem23<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T2,T3>[] items = new Tuple<T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2,T3>(list[i].Item2, list[i].Item3); return new List<Tuple<T2,T3>>(items); }
        public static List<Tuple<T2,T4>> HListItem24<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T2,T4>[] items = new Tuple<T2,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2,T4>(list[i].Item2, list[i].Item4); return new List<Tuple<T2,T4>>(items); }
        public static List<Tuple<T3,T4>> HListItem34<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T3,T4>[] items = new Tuple<T3,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T3,T4>(list[i].Item3, list[i].Item4); return new List<Tuple<T3,T4>>(items); }

        public static Tuple<T1, T2>[] HListItem12<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T2>[] items = new Tuple<T1, T2>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T2>(list[i].Item1, list[i].Item2); return items; }
        public static Tuple<T1, T3>[] HListItem13<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T3>[] items = new Tuple<T1, T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T3>(list[i].Item1, list[i].Item3); return items; }
        public static Tuple<T1, T4>[] HListItem14<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T4>[] items = new Tuple<T1, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T4>(list[i].Item1, list[i].Item4); return items; }
        public static Tuple<T1, T5>[] HListItem15<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T5>[] items = new Tuple<T1, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T5>(list[i].Item1, list[i].Item5); return items; }
        public static Tuple<T2, T3>[] HListItem23<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T3>[] items = new Tuple<T2, T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T3>(list[i].Item2, list[i].Item3); return items; }
        public static Tuple<T2, T4>[] HListItem24<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T4>[] items = new Tuple<T2, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T4>(list[i].Item2, list[i].Item4); return items; }
        public static Tuple<T2, T5>[] HListItem25<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T5>[] items = new Tuple<T2, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T5>(list[i].Item2, list[i].Item5); return items; }
        public static Tuple<T3, T4>[] HListItem34<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T3,T4>[] items = new Tuple<T3, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T3, T4>(list[i].Item3, list[i].Item4); return items; }
        public static Tuple<T3, T5>[] HListItem35<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T3,T5>[] items = new Tuple<T3, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T3, T5>(list[i].Item3, list[i].Item5); return items; }
        public static Tuple<T4, T5>[] HListItem45<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T4,T5>[] items = new Tuple<T4, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T4, T5>(list[i].Item4, list[i].Item5); return items; }

        public static List<Tuple<T1,T2,T3>> HListItem123<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T2,T3>[] items = new Tuple<T1,T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2,T3>(list[i].Item1, list[i].Item2, list[i].Item3); return new List<Tuple<T1,T2,T3>>(items); }
        public static List<Tuple<T1,T2,T4>> HListItem124<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T2,T4>[] items = new Tuple<T1,T2,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2,T4>(list[i].Item1, list[i].Item2, list[i].Item4); return new List<Tuple<T1,T2,T4>>(items); }
        public static List<Tuple<T1,T3,T4>> HListItem134<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T1,T3,T4>[] items = new Tuple<T1,T3,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T3,T4>(list[i].Item1, list[i].Item3, list[i].Item4); return new List<Tuple<T1,T3,T4>>(items); }
        public static List<Tuple<T2,T3,T4>> HListItem234<T1,T2,T3,T4>(this IList<Tuple<T1,T2,T3,T4>> list) { Tuple<T2,T3,T4>[] items = new Tuple<T2,T3,T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2,T3,T4>(list[i].Item2, list[i].Item3, list[i].Item4); return new List<Tuple<T2,T3,T4>>(items); }

        public static Tuple<T1, T2, T3>[] HListItem123<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T2,T3>[] items = new Tuple<T1, T2, T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T2, T3>(list[i].Item1, list[i].Item2, list[i].Item3); return items; }
        public static Tuple<T1, T2, T4>[] HListItem124<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T2,T4>[] items = new Tuple<T1, T2, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T2, T4>(list[i].Item1, list[i].Item2, list[i].Item4); return items; }
        public static Tuple<T1, T2, T5>[] HListItem125<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T2,T5>[] items = new Tuple<T1, T2, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T2, T5>(list[i].Item1, list[i].Item2, list[i].Item5); return items; }
        public static Tuple<T1, T3, T4>[] HListItem134<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T3,T4>[] items = new Tuple<T1, T3, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T3, T4>(list[i].Item1, list[i].Item3, list[i].Item4); return items; }
        public static Tuple<T1, T3, T5>[] HListItem135<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T3,T5>[] items = new Tuple<T1, T3, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T3, T5>(list[i].Item1, list[i].Item3, list[i].Item5); return items; }
        public static Tuple<T1, T4, T5>[] HListItem145<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T1,T4,T5>[] items = new Tuple<T1, T4, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1, T4, T5>(list[i].Item1, list[i].Item4, list[i].Item5); return items; }
        public static Tuple<T2, T3, T4>[] HListItem234<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T3,T4>[] items = new Tuple<T2, T3, T4>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T3, T4>(list[i].Item2, list[i].Item3, list[i].Item4); return items; }
        public static Tuple<T2, T3, T5>[] HListItem235<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T3,T5>[] items = new Tuple<T2, T3, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T3, T5>(list[i].Item2, list[i].Item3, list[i].Item5); return items; }
        public static Tuple<T2, T4, T5>[] HListItem245<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T2,T4,T5>[] items = new Tuple<T2, T4, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T2, T4, T5>(list[i].Item2, list[i].Item4, list[i].Item5); return items; }
        public static Tuple<T3, T4, T5>[] HListItem345<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Tuple<T3,T4,T5>[] items = new Tuple<T3, T4, T5>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T3, T4, T5>(list[i].Item3, list[i].Item4, list[i].Item5); return items; }

        public static IList<Tuple<T1,T2,T3>> HListItem123<T1, T2, T3>(this IList<Tuple<T1,Tuple<T2,T3>>> list) { Tuple<T1,T2,T3>[] items = new Tuple<T1,T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2,T3>(list[i].Item1, list[i].Item2.Item1, list[i].Item2.Item2); return items; }
        public static IList<Tuple<T1,T2,T3>> HListItem123<T1, T2, T3>(this IList<Tuple<Tuple<T1,T2>,T3>> list) { Tuple<T1,T2,T3>[] items = new Tuple<T1,T2,T3>[list.Count]; for(int i=0; i<list.Count; i++) items[i] = new Tuple<T1,T2,T3>(list[i].Item1.Item1, list[i].Item1.Item2, list[i].Item2); return items; }
    }
}
