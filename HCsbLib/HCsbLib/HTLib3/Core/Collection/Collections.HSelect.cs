using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T[] HSelect<T>(this T[] list, Func<T, bool> selector)
        {
            return list.ToList().HSelect(selector).ToArray();
        }
        public static List<T> HSelect<T>(this List<T> list, Func<T,bool> selector)
        {
            List<T> select = new List<T>();
            for(int i=0; i<list.Count; i++)
                if(selector(list[i]))
                    select.Add(list[i]);
            return select;
        }
    }
}
