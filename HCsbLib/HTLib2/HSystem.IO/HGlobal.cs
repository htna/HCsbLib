using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace HTLib2
{
    public class HGlobal
    {
        static Dictionary<string, object> _dict = new Dictionary<string, object>();
        public static object Get(string key)
        {
            return _dict[key];
        }
        public static void Set(string key, object value)
        {
            if(_dict.ContainsKey(key) == false)
                _dict.Add(key, null);
            _dict[key] = value;
        }
        static HashSet<object> _set = new HashSet<object>();
        public static object Add(object value)
        {
            return _set.Add(value);
        }
        public static void Remove(object value)
        {
            _set.Remove(value);
        }
    }
}
