using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Dictionary<Key, Value[]> HToArray<Key, Value>(this Dictionary<Key, List<Value>> dict)
        {
            Dictionary<Key, Value[]> dict2 = new Dictionary<Key, Value[]>();
            foreach(var key_values in dict)
            {
                dict2.Add(key_values.Key, key_values.Value.ToArray());
            }
            return dict2;
        }
    }
}
