using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        //public static List<T> SelectByIndex<T>(this List<T> values, IList<int> indexes)
        //{
        //    List<T> lvalues = new List<T>(indexes.Count);
        //    for(int i=0; i<indexes.Count; i++)
        //        lvalues.Add(values[indexes[i]]);
        //    return lvalues;
        //}
        public static U[]     HSelectByIndex<T,U>(this      T[] list, IList<int> idx) { return HSelectByIndex<T,U>(new List<T>(list), idx).ToArray(); }
        public static List<U> HSelectByIndex<T,U>(this IList<T> list, IList<int> idx) { return HSelectByIndex<T,U>(new List<T>(list), idx); }
        public static List<U> HSelectByIndex<T,U>(this  List<T> list, IList<int> idx) 
        {
            List<U> select = new List<U>(idx.Count);
            for(int i=0; i<idx.Count; i++)
            {
                object obj = list[idx[i]];
                select.Add((U)obj);
            }
            return select;
        }
        public static T[] HSelectByIndex<T>(this T[] list, IList<int> idx) { return HSelectByIndex(new List<T>(list), idx).ToArray(); }
        public static List<T> HSelectByIndex<T>(this IList<T> list, IList<int> idx) { return HSelectByIndex(new List<T>(list), idx); }
        public static List<T> HSelectByIndex<T>(this List<T> list, IList<int> idx)
        {
            List<T> select = new List<T>(idx.Count);
            for(int i=0; i<idx.Count; i++)
                select.Add(list[idx[i]]);
            return select;
        }
        public static T[,] HSelectByIndex<T>(this IList<T> list, int[,] idx)
        {
            int leng0 = idx.GetLength(0);
            int leng1 = idx.GetLength(1);
            T[,] select = new T[leng0, leng1];
            for(int i=0; i<leng0; i++)
                for(int j=0; j<leng1; j++)
                    select[i, j] = list[idx[i, j]];
            return select;
        }
    }
}
