using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HSelect<T>(this IEnumerable<T> list, Func<T,bool> selector)
        {
            List<T> select = new List<T>();
            foreach(var value in list)
                if(selector(value))
                    select.Add(value);
            return select.ToArray();
        }
    }
}
