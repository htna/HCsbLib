using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<T> HToList<T>(this IList<T> values) { return values.ToList(); }
    }
}
