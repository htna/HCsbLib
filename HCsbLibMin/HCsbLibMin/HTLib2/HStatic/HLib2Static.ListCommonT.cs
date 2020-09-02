using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static IList<T> HListCommonT<T>(this IList<T> list0, IList<T> list1=null)
        {
            HashSet<T> common = new HashSet<T>(list0);
            if(list1 != null)
                common.IntersectWith(list1);
            return common.ToList();
        }
    }
}
