using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HSelectCount<T>(this IList<T> values, int count)
        {
            T[] select = new T[count];
            for(int i=0; i<count; i++)
                select[i] = values[i];
            return select;
        }
        public static T[] HSelectFrom<T>(this IList<T> values, int from)
        {
            List<T> select = new List<T>();
            for(int i=from; i<values.Count; i++)
                select.Add(values[i]);
            return select.ToArray();
        }
        public static T[] HSelectFromTo<T>(this IList<T> values, int from, int to)
        {
            int count = (to - from + 1);
            return HSelectFromCount(values, from, count);
        }
        public static T[] HSelectFromCount<T>(this IList<T> values, int from, int count)
        {
            T[] select = new T[count];
            for(int i=0; i<count; i++)
                select[i] = values[from+i];
            return select;
        }
    }
}
