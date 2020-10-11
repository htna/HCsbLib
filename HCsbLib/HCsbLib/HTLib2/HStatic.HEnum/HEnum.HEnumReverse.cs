using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<T> HEnumReverse<T>(this IList<T> list)
        {
            for(int i=list.Count-1; i>=0; i--)
                yield return list[i];
        }
    }
}
