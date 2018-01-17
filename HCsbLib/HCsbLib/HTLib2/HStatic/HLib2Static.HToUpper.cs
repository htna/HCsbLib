using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static IList<string> HToUpper(this IList<string> list) { list = list.HClone(); for(int i=0; i<list.Count; i++) list[i] = list[i].ToUpper(); return list; }
        public static IList<string> HToLower(this IList<string> list) { list = list.HClone(); for(int i=0; i<list.Count; i++) list[i] = list[i].ToLower(); return list; }
    }
}
