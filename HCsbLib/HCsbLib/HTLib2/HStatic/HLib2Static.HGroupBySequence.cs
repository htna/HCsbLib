using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static List<List<int>> HGroupBySequence(this IList<int> values, Func<int, int, bool> checkNext=null)
        {
            if(checkNext == null)
                checkNext = delegate(int v0, int v1)
                            {
                                return (v0+1 == v1);
                            };
            
            return HGroupBySequence<int>(values, checkNext);
        }
        public static List<List<T>> HGroupBySequence<T>(this IList<T> values, Func<T, T, bool> checkNext)
        {
            List<List<T>> groups = new List<List<T>>();
            groups.Add(new List<T>());
            groups.Last().Add(values[0]);

            for(int i=1; i<values.Count; i++)
            {
                T v0 = groups.Last().Last();
                T v1 = values[i];
                if(checkNext(v0, v1) == false)
                    groups.Add(new List<T>());
                groups.Last().Add(v1);
            }

            return groups;
        }
    }
}
