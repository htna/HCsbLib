using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static void HDynAdd<T>(this object list, T val)  { ((dynamic)list).Add(val); }
        public static int HDynLength(this object list)          { return ((dynamic)list).Length; }
    }
}
