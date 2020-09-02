using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static HashSet<T> HToHashSet<T>(this IEnumerable<T> list)
        {
            var hashset = new HashSet<T>(list);
            return hashset;
        }

        public static IList<HashSet<T>> HToListHashSet<T>(this IList<T[]> valuess)
        {
            HashSet<T>[] rvaluess = new HashSet<T>[valuess.Count];
            for(int i=0; i<valuess.Count; i++)
                rvaluess[i] = valuess[i].HToHashSet();
            return rvaluess;
        }
        public static IList<HashSet<T>> HToListHashSet<T>(this IList<IList<T>> valuess)
        {
            HashSet<T>[] rvaluess = new HashSet<T>[valuess.Count];
            for(int i=0; i<valuess.Count; i++)
                rvaluess[i] = valuess[i].HToHashSet();
            return rvaluess;
        }
        public static IList<HashSet<T>> HToListHashSet<T>(this IList<List<T>> valuess)
        {
            HashSet<T>[] rvaluess = new HashSet<T>[valuess.Count];
            for(int i=0; i<valuess.Count; i++)
                rvaluess[i] = valuess[i].HToHashSet();
            return rvaluess;
        }
    }
}
