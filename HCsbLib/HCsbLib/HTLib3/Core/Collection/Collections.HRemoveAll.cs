using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        //public static List<T> RemoveNulls<T>(this List<T> values)
        //                      where T : class
        //{
        //    List<T> _values = new List<T>(values);
        //    while(_values.Remove(null));
        //    return _values;
        //}
        //public static T[] RemoveNulls<T>(this T[] values)
        //                      where T : class
        //{
        //    List<T> _values = new List<T>(values);
        //    while(_values.Remove(null)) ;
        //    return _values.ToArray();
        //}
        public static T[] HRemoveAll<T>(this T[] values, T toremove)
        {
            List<T> _values = new List<T>(values);
            while(_values.Remove(toremove)) ;
            return _values.ToArray();
        }
        public static List<T> HRemoveAll<T>(this List<T> values, T toremove)
        {
            List<T> _values = new List<T>(values);
            while(_values.Remove(toremove)) ;
            return _values;
        }
        //public static int HtRemoveAll<T>(this List<string> valus, T toremove)
        //    where T : IEquatable<T>
        //{
        //    Predicate<string> match = delegate(string value)
        //    {
        //        return toremove.Equals(value);
        //    };
        //    return valus.RemoveAll(match);
        //}
    }
}
