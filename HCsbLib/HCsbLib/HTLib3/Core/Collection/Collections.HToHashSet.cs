using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2x
{
    public static partial class Collections
    {
        public static HashSet<T> HToHashSet<T>(this IList<T> list)
        {
            return new HashSet<T>(list);
        }
    }
}
