using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static TValue[] HListValueOf<TKey, TValue>(this Dictionary<TKey, TValue> dict, IList<TKey> keys)
        {
            TValue[] vals = new TValue[keys.Count];
            for(int i=0; i<keys.Count; i++)
                vals[i] = dict[keys[i]];
            return vals;
        }
    }
}
