using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        //public static int[] Sort(this int[] values)
        //{
        //    List<int> lvalues = new List<int>(values);
        //    lvalues.Sort();
        //    return lvalues.ToArray();
        //}
        public static IList<T> HSort<T>(this IList<T> values)
            where T : IComparable<T>
        {
            return values.HToList().HSort().HToArray();
        }
        public static T[] HSort<T>(this T[] values)
            where T : IComparable<T>
        {
            return values.HToList().HSort().HToArray();
        }
        public static List<T> HSort<T>(this List<T> values)
            where T : IComparable<T>
        {
            List<T> lvalues = new List<T>(values);
            lvalues.Sort();
            return lvalues;
        }
        public static Tuple<T, T> HSort<T>(this Tuple<T, T> values)
            where T : IComparable<T>
        {
            T[] sort = values.HToArray().HSort();
            return new Tuple<T, T>(sort[0], sort[1]);
        }
        public static Tuple<T, T, T> HSort<T>(this Tuple<T, T, T> values)
            where T : IComparable<T>
        {
            T[] sort = values.HToArray().HSort();
            return new Tuple<T, T, T>(sort[0], sort[1], sort[2]);
        }
    }
}
