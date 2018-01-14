using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static bool HContainsKey<TKey1, TKey2, TValue>(this Dictionary<TKey1, Dictionary<TKey2, TValue>> dict, TKey1 key1, TKey2 key2)
        {
            if(dict.ContainsKey(key1) == false) return false;
            if(dict[key1].ContainsKey(key2) == false) return false;
            return true;
        }
    }
}
