using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static Dictionary<T1, Tuple<T1, T2>> HToDictionaryWithKeyItem1<T1, T2>(this IList<Tuple<T1, T2>> list) { Dictionary<T1, Tuple<T1, T2>> dict = new Dictionary<T1, Tuple<T1, T2>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2>> HToDictionaryWithKeyItem2<T1, T2>(this IList<Tuple<T1, T2>> list) { Dictionary<T2, Tuple<T1, T2>> dict = new Dictionary<T2, Tuple<T1, T2>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem1<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { Dictionary<T1, Tuple<T1, T2, T3>> dict = new Dictionary<T1, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem2<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { Dictionary<T2, Tuple<T1, T2, T3>> dict = new Dictionary<T2, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem3<T1, T2, T3>(this IList<Tuple<T1, T2, T3>> list) { Dictionary<T3, Tuple<T1, T2, T3>> dict = new Dictionary<T3, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem1<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { Dictionary<T1, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T1, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem2<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { Dictionary<T2, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T2, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem3<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { Dictionary<T3, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T3, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem4<T1, T2, T3, T4>(this IList<Tuple<T1, T2, T3, T4>> list) { Dictionary<T4, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T4, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem1<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T1, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T1, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem2<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T2, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T2, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem3<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T3, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T3, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem4<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T4, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T4, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }
        public static Dictionary<T5, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem5<T1, T2, T3, T4, T5>(this IList<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T5, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T5, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item5, item); return dict; }
    }
}
