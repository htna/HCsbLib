using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HSelectLargest<T>(this IList<List<T>> groups)
        {
            List<T> largest = groups[0];
            foreach(List<T> group in groups)
                if(group.Count > largest.Count)
                    largest = group;
            return largest.ToArray();
        }
    }
}
