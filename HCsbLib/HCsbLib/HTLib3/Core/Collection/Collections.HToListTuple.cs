using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<Tuple<T1, T2>> HToListTuple<T1, T2>(this IList<T1> list1, IList<T2> list2)
        {
            Debug.Assert(list1.Count == list2.Count);
            Tuple<T1, T2>[] array = new Tuple<T1, T2>[list1.Count];
            for(int i=0; i<array.Length; i++)
                array[i] = new Tuple<T1, T2>(list1[i], list2[i]);
            return new List<Tuple<T1,T2>>(array);
        }
        public static List<Tuple<T1, T2, T3>> HToListTuple<T1, T2, T3>(this IList<T1> list1, IList<T2> list2, IList<T3> list3)
        {
            Debug.Assert(list1.Count == list2.Count, list1.Count == list3.Count);
            Tuple<T1, T2, T3>[] array = new Tuple<T1, T2, T3>[list1.Count];
            for(int i=0; i<array.Length; i++)
                array[i] = new Tuple<T1, T2, T3>(list1[i], list2[i], list3[i]);
            return new List<Tuple<T1,T2,T3>>(array);
        }
    }
}
