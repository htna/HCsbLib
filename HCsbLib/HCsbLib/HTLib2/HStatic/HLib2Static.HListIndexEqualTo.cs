using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static List<int> HListIndexEqualTo<T>(this IList<T> values, T equalto)
            where T : IEquatable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(equalto.Equals(values[i]))
                    idxs.Add(i);
            return idxs;
        }
    }
}
