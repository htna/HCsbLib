using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static Dictionary<T1, Tuple<T1, T2>> HToDictionaryWithKeyItem1<T1, T2>(this IEnumerable<Tuple<T1, T2>> list) { Dictionary<T1, Tuple<T1, T2>> dict = new Dictionary<T1, Tuple<T1, T2>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2>> HToDictionaryWithKeyItem2<T1, T2>(this IEnumerable<Tuple<T1, T2>> list) { Dictionary<T2, Tuple<T1, T2>> dict = new Dictionary<T2, Tuple<T1, T2>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem1<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> list) { Dictionary<T1, Tuple<T1, T2, T3>> dict = new Dictionary<T1, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem2<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> list) { Dictionary<T2, Tuple<T1, T2, T3>> dict = new Dictionary<T2, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3>> HToDictionaryWithKeyItem3<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> list) { Dictionary<T3, Tuple<T1, T2, T3>> dict = new Dictionary<T3, Tuple<T1, T2, T3>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem1<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> list) { Dictionary<T1, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T1, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem2<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> list) { Dictionary<T2, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T2, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem3<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> list) { Dictionary<T3, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T3, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1, T2, T3, T4>> HToDictionaryWithKeyItem4<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> list) { Dictionary<T4, Tuple<T1, T2, T3, T4>> dict = new Dictionary<T4, Tuple<T1, T2, T3, T4>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }

        public static Dictionary<T1, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem1<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T1, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T1, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem2<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T2, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T2, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem3<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T3, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T3, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem4<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T4, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T4, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }
        public static Dictionary<T5, Tuple<T1, T2, T3, T4, T5>> HToDictionaryWithKeyItem5<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> list) { Dictionary<T5, Tuple<T1, T2, T3, T4, T5>> dict = new Dictionary<T5, Tuple<T1, T2, T3, T4, T5>>(); foreach(var item in list) dict.Add(item.Item5, item); return dict; }

        public static Dictionary<T1, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem1<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T1, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem2<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T2, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem3<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T3, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem4<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T4, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }
        public static Dictionary<T5, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem5<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T5, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item5, item); return dict; }
        public static Dictionary<T6, Tuple<T1,T2,T3,T4,T5,T6>> HToDictionaryWithKeyItem6<T1,T2,T3,T4,T5,T6>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6>> list) { var dict = new Dictionary<T6, Tuple<T1,T2,T3,T4,T5,T6>>(); foreach(var item in list) dict.Add(item.Item6, item); return dict; }

        public static Dictionary<T1, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem1<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T1, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item1, item); return dict; }
        public static Dictionary<T2, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem2<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T2, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item2, item); return dict; }
        public static Dictionary<T3, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem3<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T3, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item3, item); return dict; }
        public static Dictionary<T4, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem4<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T4, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item4, item); return dict; }
        public static Dictionary<T5, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem5<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T5, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item5, item); return dict; }
        public static Dictionary<T6, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem6<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T6, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item6, item); return dict; }
        public static Dictionary<T7, Tuple<T1,T2,T3,T4,T5,T6,T7>> HToDictionaryWithKeyItem7<T1,T2,T3,T4,T5,T6,T7>(this IEnumerable<Tuple<T1,T2,T3,T4,T5,T6,T7>> list) { var dict = new Dictionary<T7, Tuple<T1,T2,T3,T4,T5,T6,T7>>(); foreach(var item in list) dict.Add(item.Item7, item); return dict; }

        public static Dictionary<T,int> HToDictionaryAsValueIndex<T>
            ( this IList<T> list
            , string option = null
            )
        {
            Dictionary<T,int> dict = new Dictionary<T, int>();
            for(int i=0; i<list.Count; i++)
            {
                if(dict.ContainsKey(list[i]))
                {
                    switch(option)
                    {
                        case "first index for same values": continue;
                        case  "last index for same values": dict[list[i]] = i; continue;
                        case    "-1 index for same values": dict[list[i]] = -1; continue;
                        case null: throw new HException(string.Format("a key {0} is already in the dictionary", list[i]));
                    }
                }
                dict.Add(list[i], i);
            }
            return dict;
        }

        public static Dictionary<T1, T2> HToDictionaryWithKey1Value2<T1, T2>(this IEnumerable<Tuple<T1, T2>> list) { Dictionary<T1, T2> dict = new Dictionary<T1, T2>(); foreach(var item in list) dict.Add(item.Item1, item.Item2); return dict; }
        public static Dictionary<T2, T1> HToDictionaryWithKey2Value1<T1, T2>(this IEnumerable<Tuple<T1, T2>> list) { Dictionary<T2, T1> dict = new Dictionary<T2, T1>(); foreach(var item in list) dict.Add(item.Item2, item.Item1); return dict; }

        public static Dictionary<K1, List<V1>> HToDictionaryWitKey1<K1, K2, V1>(this Dictionary<Tuple<K1, K2>, List<V1>> dict)
        {
            Dictionary<K1,List<V1>> ndict = new Dictionary<K1, List<V1>>();
            foreach(var k1_k2_v1 in dict)
            {
                var key = k1_k2_v1.Key.Item1;
                var val = k1_k2_v1.Value;
                if(ndict.ContainsKey(key) == false) ndict.Add(key, new List<V1>());
                ndict[key].AddRange(val);
            }
            return ndict;
        }
        public static Dictionary<K2, List<V1>> HToDictionaryWitKey2<K1, K2, V1>(this Dictionary<Tuple<K1, K2>, List<V1>> dict)
        {
            Dictionary<K2,List<V1>> ndict = new Dictionary<K2, List<V1>>();
            foreach(var k1_k2_v1 in dict)
            {
                var key = k1_k2_v1.Key.Item2;
                var val = k1_k2_v1.Value;
                if(ndict.ContainsKey(key) == false) ndict.Add(key, new List<V1>());
                ndict[key].AddRange(val);
            }
            return ndict;
        }

        public static Dictionary<T2, T1> HToDictionaryWithKey<T1, T2>(this IList<T1> values, IList<T2> keys)
        {
            if(values.Count != keys.Count)
                throw new HException();

            int count = keys.Count;
            Dictionary<T2, T1> dict = new Dictionary<T2, T1>();
            for(int i=0; i<count; i++)
                dict.Add(keys[i], values[i]);
            return dict;
        }
        public static Dictionary<T1, T2> HToDictionaryAsKey<T1, T2>(this IEnumerable<T1> list, Func<T2> GetDefValue)
        {
            Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
            foreach(var key in list)
                dict.Add(key, GetDefValue());
            return dict;
        }
        public static Dictionary<T1, T2> HToDictionaryAsKey<T1, T2>(this IEnumerable<T1> list, T2 defvalue)
        {
            Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
            foreach(var key in list)
                dict.Add(key, defvalue);
            return dict;
        }
    }
}
