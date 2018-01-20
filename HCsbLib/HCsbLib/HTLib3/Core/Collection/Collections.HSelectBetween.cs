using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T[]     HSelectGreat     <T>(this T[]     list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >  0; }); }
        public static List<T> HSelectGreat     <T>(this List<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >  0; }); }
        public static T[]     HSelectGreatEqual<T>(this T[]     list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >= 0; }); }
        public static List<T> HSelectGreatEqual<T>(this List<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) >= 0; }); }
        public static T[]     HSelectLess      <T>(this T[]     list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <  0; }); }
        public static List<T> HSelectLess      <T>(this List<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <  0; }); }
        public static T[]     HSelectLessEqual <T>(this T[]     list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <= 0; }); }
        public static List<T> HSelectLessEqual <T>(this List<T> list, T comp) where T : struct, IComparable<T> { return list.HSelect(delegate(T val) { return val.CompareTo(comp) <= 0; }); }

        public static T[] HSelectBetween<T>(this T[] list, T? min, T? max)
            where T : struct, IComparable<T>
        {
            return list.HSelectByIndex(list.HIdxListBetween(min, max));
        }
        public static List<T> HSelectBetween<T>(this List<T> list, T? min, T? max)
            where T : struct, IComparable<T>
        {
            return list.HSelectByIndex(list.HIdxListBetween(min, max));
        }

        public static T[] HSelectBetween<T>(this T[] list, T min, T max)
            where T : IComparable<T>
        {
            return list.HSelectByIndex(list.HIdxListBetween(min, max));
        }
        public static List<T> HSelectBetween<T>(this List<T> list, T min, T max)
            where T : IComparable<T>
        {
            return list.HSelectByIndex(list.HIdxListBetween(min, max));
        }
    }
}
