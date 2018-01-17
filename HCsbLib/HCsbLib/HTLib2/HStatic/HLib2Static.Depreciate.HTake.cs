using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class Collections
    {
        public static List<TSource> HTake<TSource>(this IEnumerable<TSource> source, int from, int count)
        {
            HDebug.ToDo("Depreciate. Call HSelectCount");

            List<TSource> taken = new List<TSource>(count);
            int idx=0;
            int to = from + count - 1;
            foreach(TSource elem in source)
            {
                if(idx >= from && idx <= to)
                    taken.Add(elem);
                idx++;
            }
            return taken;
        }
    }
}
