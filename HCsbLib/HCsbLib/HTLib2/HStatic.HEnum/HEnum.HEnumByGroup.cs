using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<int> HEnumByGroup<T>(this IEnumerable<T> list, IList<T[]> groups, bool bSkipEnumed, bool bSkipUngrouped)
        {
            Dictionary<T, int> value2group = new Dictionary<T, int>();
            for(int ig=0; ig<groups.Count; ig++)
                foreach(T val in groups[ig])
                    value2group.Add(val, ig);

            HashSet<int> enumeds = null;
            if(bSkipEnumed)
                enumeds = new HashSet<int>();

            foreach(var val in list)
            {
                if(bSkipUngrouped == true)
                    if(value2group.ContainsKey(val) == false)
                        continue;
                int ig = value2group[val];

                if(bSkipEnumed)
                {
                    if(enumeds.Contains(ig))
                        continue;
                    enumeds.Add(ig);
                }

                yield return ig;
            }
        }
        public static IEnumerable<Tuple<int, int>> HEnumByGroup<T>(this IEnumerable<Tuple<T, T>> list, IList<T[]> groups, bool bSkipEnumed, bool bSkipUngrouped)
        {
            Dictionary<T, int> value2group = new Dictionary<T, int>();
            for(int ig=0; ig<groups.Count; ig++)
                foreach(T val in groups[ig])
                    value2group.Add(val, ig);

            HashSet<Tuple<int, int>> enumeds = null;
            if(bSkipEnumed)
                enumeds = new HashSet<Tuple<int, int>>();

            foreach(var val in list)
            {
                T val1 = val.Item1;
                T val2 = val.Item2;

                if(bSkipUngrouped == true)
                    if((value2group.ContainsKey(val1) == false) || (value2group.ContainsKey(val2) == false))
                        continue;
                int ig1 = value2group[val1];
                int ig2 = value2group[val2];
                Tuple<int, int> enuming = new Tuple<int, int>(ig1, ig2);

                if(bSkipEnumed)
                {
                    if(enumeds.Contains(enuming))
                        continue;
                    enumeds.Add(enuming);
                }

                yield return enuming;
            }
        }
    }
}
