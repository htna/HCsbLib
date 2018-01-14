using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[] HReverse<T>(this IList<T> values)
        {
            List<T> list = new List<T>(values);
            list.Reverse();
            return list.ToArray();
        }
    }
}
