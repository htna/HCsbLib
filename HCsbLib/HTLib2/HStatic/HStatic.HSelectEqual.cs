using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HSelectEqual<T>(this IEnumerable<T> list, T equalto)
            where T : IComparable
        {
            Func<T, bool> selector = delegate(T comp)
            {
                return (comp.CompareTo(equalto) == 0);
            };
            return HSelect(list, selector);
        }
    }
}
