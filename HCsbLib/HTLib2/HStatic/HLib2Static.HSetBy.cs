using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[] HSetByIndex<T>(this IList<T> values, IList<int> idxs, IList<T> updates)
        {
            T[] nvalues = new T[values.Count];
            for(int i=0; i<values.Count; i++)
                nvalues[i] = values[i];

            HDebug.Assert(idxs.Count == updates.Count);
            for(int i=0; i<idxs.Count; i++)
            {
                nvalues[idxs[i]] = updates[i];
            }

            return nvalues;
        }
    }
}
