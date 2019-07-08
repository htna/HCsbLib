using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static bool HContainsStartsWith(this IEnumerable<string> list, string startswith)
        {
            foreach(string str in list.HEnumByStartsWith(startswith))
                return true;
            return false;
        }
    }
}
