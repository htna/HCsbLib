using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static int HIndexEqual(this string[] values, string item)
        {
            IList<string> _values = values;
            return _values.IndexOf(item);
        }
        public static int HIndexEqualReference<T>(this T values, IList<T> searchFrom)
            where T : class
        {
            for(int i=0; i<searchFrom.Count; i++)
                if(object.ReferenceEquals(values, searchFrom[i]))
                    return i;
            return -1;
        }
        public static List<int> HIndexEqualReference<T>(this IList<T> values, IList<T> searchFrom)
            where T : class
        {
            int[] index = new int[values.Count];
            for(int i=0; i<values.Count; i++)
                index[i] = values[i].HIndexEqualReference(searchFrom);
            return new List<int>(index);
        }
    }
}
