using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static bool HContains<T>(this IList<T> values, T item)
        {
            if(values == null)
                // consider as empty
                return false;
            return values.Contains(item);
        }
        public static bool HContains<T>(this IEnumerable<T> values, T item)
        {
            if(values == null)
                // consider as empty
                return false;
            return values.Contains(item);
        }
        public static bool HContainsStartsWith(this IEnumerable<string> list, string startswith)
        {
            foreach(string str in list.HEnumByStartsWith(startswith))
                return true;
            return false;
        }
    }
}
