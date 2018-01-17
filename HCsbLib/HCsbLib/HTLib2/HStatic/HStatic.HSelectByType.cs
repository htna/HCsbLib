using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        //public static IList<T> HSelectByType<T>(this IList<object> list)
        //{
        //    Type type = typeof(T);
        //    List<T> select = new List<T>();
        //    foreach(object item in list)
        //        if(type.IsInstanceOfType(item))
        //            select.Add((T)item);
        //    return select;
        //}
        //public static IList<T> HSelectByType<T>(this IList<T> list, Type type)
        //{
        //    List<T> select = new List<T>();
        //    foreach(T item in list)
        //        if(type.IsInstanceOfType(item))
        //            select.Add(item);
        //    return select;
        //}

        public static T2 HFirstByType<T1, T2>(this IEnumerable<T1> values, T2 nullval)
            where T2 : class
        {
            HDebug.Assert(nullval == null);
            return values.HFirstByType<T1, T2>();
        }
        public static T2 HFirstByType<T1, T2>(this IEnumerable<T1> values)
            where T2 : class
        {
            foreach(T1 val1 in values)
            {
                T2 val2 = val1 as T2;
                if(val2 != null)
                    return val2;
            }
            return null;
        }
        public static T2[] HSelectByType<T1, T2>(this IList<T1> values, T2 nullval)
            where T2 : class
        {
            HDebug.Assert(nullval == null);
            return values.HSelectByType<T1, T2>();
        }
        public static T2[] HSelectByType<T1, T2>(this IList<T1> values)
            where T2 : class
        {
            List<T2> tovalues = new List<T2>();
            foreach(T1 val1 in values)
            {
                T2 val2 = val1 as T2;
                if(val2 != null)
                    tovalues.Add(val2);
            }
            return tovalues.ToArray();
        }

        public static int[] HIndexByType<T1, T2>(this IList<T1> values, T2 nullval)
            where T2 : class
        {
            HDebug.Assert(nullval == null);
            return values.HIndexByType<T1, T2>();
        }
        public static int[] HIndexByType<T1, T2>(this IList<T1> values)
            where T2 : class
        {
            List<int> indexes = new List<int>();
            for(int idx=0; idx<values.Count; idx++)
            {
                T2 val = values[idx] as T2;
                if(val != null)
                    indexes.Add(idx);
            }
            return indexes.ToArray();
        }
    }
}
