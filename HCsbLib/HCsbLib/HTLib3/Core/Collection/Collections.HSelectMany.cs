using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static List<T> HSelectMany<T>(this List<T> values, int count)
        {
            List<T> select = new List<T>(count);
            for(int i=0; i<count; i++)
                select.Add(values[i]);
            Debug.Assert(select.Count == count);
            return select;
        }
        public static List<T> HSelectMany<T>(this List<T> values, int from, int count)
        {
            List<T> select = new List<T>(count);
            for(int i=0; i<count; i++)
                select.Add(values[from+i]);
            Debug.Assert(select.Count == count);
            return select;
        }
        public static T[] HSelectMany<T>(this T[] values, int count)
        {
            T[] select = new T[count];
            for(int i=0; i<count; i++)
                select[i] = values[i];
            Debug.Assert(select.Length == count);
            return select;
        }
        public static T[] HSelectMany<T>(this T[] values, int from, int count)
        {
            T[] select = new T[count];
            for(int i=0; i<count; i++)
                select[i] = values[from+i];
            Debug.Assert(select.Length == count);
            return select;
        }
        //public static List<TSource> Take<TSource>(this IEnumerable<TSource> source, int from, int count)
        //{
        //    List<TSource> taken = new List<TSource>(count);
        //    int idx=0;
        //    int to = from + count - 1;
        //    foreach(TSource elem in source)
        //    {
        //        if(idx >= from && idx <= to)
        //            taken.Add(elem);
        //        idx++;
        //    }
        //    return taken;
        //}
    }
}
