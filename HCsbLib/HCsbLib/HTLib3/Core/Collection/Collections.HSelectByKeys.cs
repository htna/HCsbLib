using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static TValue[] HSelectByKeys<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] keys)
        {
            List<TValue> values = new List<TValue>();
            foreach(TKey key in keys)
                values.Add(dict[key]);
            return values.ToArray();
        }
    }
}
