using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<T1> HToListByItem1<T1, T2>(this IList<Tuple<T1, T2>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return new List<T1>(items); }
        public static List<T2> HToListByItem2<T1, T2>(this IList<Tuple<T1, T2>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return new List<T2>(items); }

        public static List<T1> HToListByItem1<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return new List<T1>(items); }
        public static List<T2> HToListByItem2<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return new List<T2>(items); }
        public static List<T3> HToListByItem3<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return new List<T3>(items); }

        public static List<T1> HToListByItem1<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T1[] items = new T1[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item1; return new List<T1>(items); }
        public static List<T2> HToListByItem2<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T2[] items = new T2[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item2; return new List<T2>(items); }
        public static List<T3> HToListByItem3<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T3[] items = new T3[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item3; return new List<T3>(items); }
        public static List<T4> HToListByItem4<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { T4[] items = new T4[list.Count]; for(int i=0; i<list.Count; i++) items[i]=list[i].Item4; return new List<T4>(items); }
    }
}
