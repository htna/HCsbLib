using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static Tuple<T1,T2>[] HAddItem<T1,T2>(this IList<T1> value1s, T2 value2)
        {
            List<Tuple<T1,T2>> value12s = new List<Tuple<T1, T2>>();
            foreach(var value1 in value1s)
                value12s.Add(new Tuple<T1, T2>(value1, value2));
            return value12s.ToArray();
        }
    }
}
