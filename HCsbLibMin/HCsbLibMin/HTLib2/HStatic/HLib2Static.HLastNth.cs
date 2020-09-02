using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T HLastNth<T>(this IList<T> values, int nth=0)
        {
            int idx = (values.Count-1) - nth;
            return values[idx];
        }
    }
}
