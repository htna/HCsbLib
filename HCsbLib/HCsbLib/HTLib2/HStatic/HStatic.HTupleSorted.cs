using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T,T  > HTuple2Sorted<T>(this IEnumerable<T> list) where T : IComparable<T> { HDebug.Assert(list.Count()==2); return list.ToArray().HSort().HToTuple2(); }
        public static Tuple<T,T,T> HTuple3Sorted<T>(this IEnumerable<T> list) where T : IComparable<T> { HDebug.Assert(list.Count()==3); return list.ToArray().HSort().HToTuple3(); }
    }
}
