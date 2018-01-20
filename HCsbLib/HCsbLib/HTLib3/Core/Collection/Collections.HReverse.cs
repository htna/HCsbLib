using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T[] HReverse<T>(this T[] values)
        {
            return ((IEnumerable<T>)values).Reverse().ToArray();
        }
        public static List<T> HReverse<T>(this List<T> values)
        {
            return ((IEnumerable<T>)values).Reverse().ToList();
        }
    }
}
