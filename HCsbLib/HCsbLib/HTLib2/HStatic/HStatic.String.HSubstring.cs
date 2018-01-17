using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static string HSubstringFromTo(this string str, int from, int to)
        {
            int leng = to-from+1;
            string nstr = str.Substring(from, leng);
            return nstr;
        }
    }
}
