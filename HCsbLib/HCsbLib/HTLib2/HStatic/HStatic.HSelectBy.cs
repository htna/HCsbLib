using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static TValue[] HSelectByKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keys)
        {
            List<TValue> values = new List<TValue>();
            foreach(TKey key in keys)
                values.Add(dict[key]);
            return values.ToArray();
        }
        public static T1[] HSelectBy<T1, T2>(this IList<T1> list, Func<T1, T2> GetValue, params T2[] query)
        {
            HashSet<T2> hashset = new HashSet<T2>(query);
            List<T1> sel = new List<T1>();
            foreach(T1 item in list)
                if(hashset.Contains(GetValue(item)))
                    sel.Add(item);
            return sel.ToArray();
        }
    }
}
