using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T2[] HSelectByType<T1, T2>(this T1[] values)
            where T2 : class
        {
            return values.ToList().HSelectByType<T1, T2>().ToArray();
        }
        public static List<T2> HSelectByType<T1, T2>(this List<T1> values)
            where T2 : class
        {
            List<T2> tovalues = new List<T2>();
            foreach(T1 val1 in values)
            {
                T2 val2 = val1 as T2;
                if(val2 != null)
                    tovalues.Add(val2);
            }
            return tovalues;
        }
        public static List<T> HSelectByType<T>(this IList<object> list)
        {
            Type type = typeof(T);
            List<T> select = new List<T>();
            foreach(object item in list)
                if(type.IsInstanceOfType(item))
                    select.Add((T)item);
            return select;
        }
        public static List<T> HSelectByType<T>(this IList<T> list, Type type)
        {
            List<T> select = new List<T>();
            foreach(T item in list)
                if(type.IsInstanceOfType(item))
                    select.Add(item);
            return select;
        }
        public static List<U> HSelectByType<T, U>(this IList<T> list)
            where U : T
        {
            List<U> select = new List<U>();
            foreach(T item in list)
                if(item is U)
                    select.Add((U)item);
            return select;
        }
        public static List<U> HSelectByType<T, U>(this IList<T> list, U _)
            where U : T
        {
            List<U> select = new List<U>();
            foreach(T item in list)
                if(item is U)
                    select.Add((U)item);
            return select;
        }
    }
}
