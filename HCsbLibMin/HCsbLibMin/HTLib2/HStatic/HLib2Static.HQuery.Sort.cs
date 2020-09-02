using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static HQuery<KEY, VALUE>[] HSort<KEY, VALUE>(this IList<HQuery<KEY, VALUE>> query, Comparison<VALUE> comparison)
        {
            VALUE[] values = query.GetValue();
            int[] idxsort = values.HIdxSorted(comparison);
            return query.HSelectByIndex(idxsort);
        }
        public static HQuery<KEY, VALUE>[] HSort<KEY, VALUE>(this IList<HQuery<KEY, VALUE>> query)
            where VALUE : IComparable<VALUE>
        {
            VALUE[] values = query.GetValue();
            int[] idxsort = values.HIdxSorted();
            return query.HSelectByIndex(idxsort);
        }
    }
}
