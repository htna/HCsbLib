using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static Tuple<T1, T2>[] HToIListTuple<T1, T2>(this IList<T1> list1, IList<T2> list2)
        {
            HDebug.Assert(list1.Count == list2.Count);
            Tuple<T1, T2>[] array = new Tuple<T1, T2>[list1.Count];
            for(int i=0; i<array.Length; i++)
                array[i] = new Tuple<T1, T2>(list1[i], list2[i]);
            return array;
        }
        public static Tuple<T1, T2, T3>[] HToIListTuple<T1, T2, T3>(this IList<T1> list1, IList<T2> list2, IList<T3> list3)
        {
            HDebug.Assert(list1.Count == list2.Count, list1.Count == list3.Count);
            Tuple<T1, T2, T3>[] array = new Tuple<T1, T2, T3>[list1.Count];
            for(int i=0; i<array.Length; i++)
                array[i] = new Tuple<T1, T2, T3>(list1[i], list2[i], list3[i]);
            return array;
        }
        public static Tuple<T1,T2>[] HToListTuple<T1,T2>(this Dictionary<T1,T2> dict)
        {
            List<Tuple<T1,T2>> list = new List<Tuple<T1,T2>>();
            foreach(var item in dict)
                list.Add(new Tuple<T1, T2>(item.Key, item.Value));
            return list.ToArray();
        }
        public static Tuple<T2, T1>[] HToListTupleValueKey<T1, T2>(this Dictionary<T1, T2> dict)
        {
            List<Tuple<T2, T1>> list = new List<Tuple<T2, T1>>();
            foreach(var item in dict)
                list.Add(new Tuple<T2, T1>(item.Value, item.Key));
            return list.ToArray();
        }
        public static List<(T1, T2)> HToListValueTuple<T1, T2>(this ValueTuple<T1[], T2[]> tuples)
        {
            int n = tuples.Item1.Length;
            HDebug.Exception(n == tuples.Item1.Length);
            HDebug.Exception(n == tuples.Item2.Length);
            List<(T1, T2)> list = new List<(T1, T2)>();
            for(int i=0; i<n; i++)
                list.Add((tuples.Item1[i], tuples.Item2[i]));
            return list;
        }
        public static List<(T1, T2, T3)> HToListValueTuple<T1, T2, T3>(this ValueTuple<T1[], T2[], T3[]> tuples)
        {
            int n = tuples.Item1.Length;
            HDebug.Exception(n == tuples.Item1.Length);
            HDebug.Exception(n == tuples.Item2.Length);
            HDebug.Exception(n == tuples.Item3.Length);
            List<(T1, T2, T3)> list = new List<(T1, T2, T3)>();
            for(int i=0; i<n; i++)
                list.Add((tuples.Item1[i], tuples.Item2[i], tuples.Item3[i]));
            return list;
        }
    }
}
