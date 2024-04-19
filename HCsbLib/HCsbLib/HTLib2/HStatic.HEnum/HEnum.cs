using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<TElem> HEnumEnumValues<TKey,TElem>(this Dictionary<TKey,List<TElem>> dict)
        {
            foreach(var list in dict.Values)
                foreach(var item in list)
                    yield return item;
        }
        public static IEnumerable<TElem> HEnumEnumValues<TKey,TElem>(this Dictionary<TKey,TElem[]> dict)
        {
            foreach(var list in dict.Values)
                foreach(var item in list)
                    yield return item;
        }
        public static IEnumerable<TElem> HEnumEnumValues<TKey,TElem>(this Dictionary<TKey,IList<TElem>> dict)
        {
            foreach(var list in dict.Values)
                foreach(var item in list)
                    yield return item;
        }
        public static IEnumerable<TElem> HEnumEnumValues<TKey,TElem>(this Dictionary<TKey,HashSet<TElem>> dict)
        {
            foreach(var list in dict.Values)
                foreach(var item in list)
                    yield return item;
        }
    }
}
