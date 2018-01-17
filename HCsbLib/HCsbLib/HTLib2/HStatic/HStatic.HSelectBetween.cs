using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static IList<T> HSelectGreat     <T>(this IList<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >  0; }); }
        public static IList<T> HSelectGreatEqual<T>(this IList<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >= 0; }); }
        public static IList<T> HSelectLess      <T>(this IList<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <  0; }); }
        public static IList<T> HSelectLessEqual <T>(this IList<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <= 0; }); }

        public static IList<T> HSelectBetween<T>(this IList<T> list, T? min, T? max)
            where T : struct, IComparable<T>
        {
            return list.HSelectByIndex(list.HIndexBetween(min, max));
        }
        public static IList<int> HIndexBetween<T>(this IList<T> list, T? min, T? max)
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

        public static IList<T> HSelectBetween<T>(this IList<T> list, T min, T max)
            where T : IComparable<T>
        {
            return list.HSelectByIndex(list.HIndexBetween(min, max));
        }
        public static IList<int> HIndexBetween<T>(this IList<T> list, T min, T max)
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
