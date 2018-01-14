using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static int[] HIdxEqual<T>(this IList<T> values, params T[] items)
            where T : IEquatable<T>
        {
            Func<T, T, bool> Equals = delegate(T v1, T v2) { return v1.Equals(v2); };
            int[] idxs = HIdxEqual(values, Equals, items);
            return idxs;
        }
        public static int[] HIdxEqual<T>(this IList<T> values, Func<T, T, bool> Equals, params T[] items)
        {
            List<int> listidx = new List<int>();
            for(int i=0; i<values.Count; i++)
                foreach(var item in items)
                {
                    if((item == null) || (values[i] == null))
                    {
                        if((item == null) && (values[i] == null))
                        {
                            // if both are null, equal
                            listidx.Add(i);
                            break;
                        }
                        else
                        {
                            // otherwise, different
                            continue;
                        }
                    }
                    if(Equals(item, values[i]))
                    {
                        listidx.Add(i);
                        break;
                    }
                }
            return listidx.ToArray();
        }
    }
}
