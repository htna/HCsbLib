using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static int[] HIdxListEqual<T>(this IList<T> values, T item)
            where T : IEquatable<T>
        {
            Func<T, T, bool> Equals = delegate(T v1, T v2) { return v1.Equals(v2); };
            return HIdxListEqual(values, item, Equals);
        }
        public static int[] HIdxListEqual<T>(this IList<T> values, T item, Func<T, T, bool> Equals)
        {
            List<int> listidx = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(Equals(item, values[i]))
                    listidx.Add(i);
            return listidx.ToArray();
        }
        public static int[] HIdxListNotEqual<T>(this IList<T> list, T item)
            where T : IEquatable<T>
        {
            Func<T, T, bool> NotEquals = delegate(T v1, T v2) { return (v1.Equals(v2) == false); };
            return HIdxListEqual(list, item, NotEquals);
        }
    }
}
