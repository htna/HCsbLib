using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<string> HEnumByStartsWith(this IEnumerable<string> list, string startswith)
        {
            foreach(string str in list)
            {
                if(str.StartsWith(startswith))
                    yield return str;
            }
        }
    }
}
