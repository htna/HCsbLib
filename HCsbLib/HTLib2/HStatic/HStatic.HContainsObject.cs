using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static bool HContainsObject(this IEnumerable<object> source, object value)
        {
            foreach(object obj in source)
                if(object.ReferenceEquals(obj, value))
                    return true;
            return false;
        }
    }
}
