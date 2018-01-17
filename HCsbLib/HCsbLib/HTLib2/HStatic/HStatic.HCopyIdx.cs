using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HCopyIdx<T>(this IList<T> list, IList<int> idxs, T defvalue)
        {
            T[] copy = new T[list.Count];
            copy.HUpdateValueAll(defvalue);
            foreach(int idx in idxs)
                copy[idx] = list[idx];
            return copy;
        }
    }
}
