using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumSorted<T>(this IEnumerable<T> list)
        {
            List<T> list_sorted = list.ToList();
            list_sorted.Sort();
            foreach(T item in list_sorted)
                yield return item;
        }
    }
}
