using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static IList<T> HRemoveAt<T>(this IList<T> values, int index)
        {
            values = new List<T>(values);
            values.RemoveAt(index);
            return values;
        }
    }
}
