using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        //public static int[] HIdxListNotEqual<T>(this IList<T> list, T comp) where T : IEquatable<T>
        //{
        //    List<int> idxs = new List<int>();
        //    for(int i=0; i<list.Count; i++)
        //        if(list[i].Equals(comp) == false)
        //            idxs.Add(i);
        //    return idxs.ToArray();
        //}
        //public static int[] HIdxListEqual<T>(this IList<T> list, T comp) where T : IEquatable<T>
        //{
        //    List<int> idxs = new List<int>();
        //    for(int i=0; i<list.Count; i++)
        //        if(list[i].Equals(comp) == true)
        //            idxs.Add(i);
        //    return idxs.ToArray();
        //}
        public static int[] HIdxListLess<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) < 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxListLessEqual<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) <= 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxListGreat<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) > 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxListGreatEqual<T>(this IList<T> list, T comp) where T : IComparable<T>
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(list[i].CompareTo(comp) >= 0)
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxListIn<T>(this IList<T> list, IList<T> comp) where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(comp);
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(set.Contains(list[i]))
                    idxs.Add(i);
            return idxs.ToArray();
        }
        public static int[] HIdxListNotIn<T>(this IList<T> list, IList<T> comp) where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(comp);
            List<int> idxs = new List<int>();
            for(int i=0; i<list.Count; i++)
                if(set.Contains(list[i]) == false)
                    idxs.Add(i);
            return idxs.ToArray();
        }


        public static int[] HIdxListBetween<T>(this T[] list, T? min, T? max)
            where T : struct, IComparable<T>
        {
            return list.ToList().HIdxListBetween(min, max).ToArray();
        }
        public static List<int> HIdxListBetween<T>(this List<T> list, T? min, T? max)
            where T : struct, IComparable<T>
        {
            List<int> select = new List<int>();
            for(int i=0; i<list.Count; i++)
            {
                T val = list[i];
                if(min != null && val.CompareTo(min.Value) < 0) continue;
                if(max != null && val.CompareTo(max.Value) > 0) continue;
                select.Add(i);
            }
            return select;
        }
        public static int[] HIdxListBetween<T>(this T[] list, T min, T max)
            where T : IComparable<T>
        {
            return list.ToList().HIdxListBetween(min, max).ToArray();
        }
        public static List<int> HIdxListBetween<T>(this List<T> list, T min, T max)
            where T : IComparable<T>
        {
            List<int> select = new List<int>();
            for(int i=0; i<list.Count; i++)
            {
                T val = list[i];
                if(min != null && val.CompareTo(min) < 0) continue;
                if(max != null && val.CompareTo(max) > 0) continue;
                select.Add(i);
            }
            return select;
        }
    }
}
