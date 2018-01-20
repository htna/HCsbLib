using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static Tuple<int, int>[] HIdxListCommon<T>(this IList<T> list1, IList<T> list2)
        {
            Dictionary<T, List<int>> dict2 = new Dictionary<T, List<int>>(list2.Count);
            for(int i=0; i<list2.Count; i++)
            {
                if(dict2.ContainsKey(list2[i]) == false)
                    dict2.Add(list2[i], new List<int>());
                dict2[list2[i]].Add(i);
            }

            List<Tuple<int, int>> listidx = new List<Tuple<int, int>>();
            for(int i=0; i<list1.Count; i++)
            {
                if(dict2.ContainsKey(list1[i]) == false)
                    continue;
                foreach(int j in dict2[list1[i]])
                    listidx.Add(new Tuple<int, int>(i, j));
            }
            return listidx.ToArray();
        }
    }
}
