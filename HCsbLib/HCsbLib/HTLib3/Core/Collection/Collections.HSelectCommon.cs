using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<T> HSelectCommon<T>(this List<T> list0, IEnumerable<T> list1=null) { return HSelectCommon(list0.ToArray(), list1).ToList(); }
        public static T[] HSelectCommon<T>(this T[] list0, IEnumerable<T> list1=null)
        {
            HashSet<T> common = new HashSet<T>(list0);
            if(list1 != null)
                common.IntersectWith(list1);
            return common.ToArray();
        }
    }
}
