using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static List<U> HSelectByTypeDeprec<T, U>(this IList<T> list)
            where U : T
        {
            HDebug.ToDo("depreciated");

            List<U> select = new List<U>();
            foreach(T item in list)
                if(item is U)
                    select.Add((U)item);
            return select;
        }
        public static List<U> HSelectByTypeDeprec<T, U>(this IList<T> list, U _)
            where U : T
        {
            HDebug.ToDo("depreciated");

            List<U> select = new List<U>();
            foreach(T item in list)
                if(item is U)
                    select.Add((U)item);
            return select;
        }
    }
}
