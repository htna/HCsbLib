using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumExcept<T>(this IEnumerable<T> list, HashSet<T> except)
        {
            foreach(var item in list)
                if(except.Contains(item) == false)
                    yield return item;
        }
    }
}
