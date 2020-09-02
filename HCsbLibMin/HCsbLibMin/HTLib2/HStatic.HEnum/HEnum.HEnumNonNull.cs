using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumNonNull<T>(this IEnumerable<T> list)
            where T : class
        {
            foreach(var item in list)
            {
                if(item == null)
                    continue;
                yield return item;
            }
        }
    }
}
