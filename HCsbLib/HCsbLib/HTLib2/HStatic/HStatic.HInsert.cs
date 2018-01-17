using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HInsert<T>(this IList<T> values, int index, T item)
        {
            // index==0 will insert in the front of list.
            values = new List<T>(values);
            values.Insert(index, item);
            return values.ToArray();
        }
        public static Tuple<T1,T2      > HInsert1<T1,T2      >(this Tuple<T2      > tuple, T1 item1) { return new Tuple<T1,T2      >(item1, tuple.Item1                          ); }
        public static Tuple<T1,T2,T3   > HInsert1<T1,T2,T3   >(this Tuple<T2,T3   > tuple, T1 item1) { return new Tuple<T1,T2,T3   >(item1, tuple.Item1, tuple.Item2             ); }
        public static Tuple<T1,T2,T3,T4> HInsert1<T1,T2,T3,T4>(this Tuple<T2,T3,T4> tuple, T1 item1) { return new Tuple<T1,T2,T3,T4>(item1, tuple.Item1, tuple.Item2, tuple.Item3); }

        public static T[] HInsertRange<T>(this IList<T> values, int index, params T[] items)
        {
            // index==0 will insert in the front of list.
            values = new List<T>(values);
            foreach(T item in items.HReverse())
                values.Insert(index, item);
            return values.ToArray();
        }
    }
}
