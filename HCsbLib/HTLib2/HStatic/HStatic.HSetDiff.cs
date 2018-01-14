using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HExcept<T>(this IEnumerable<T> values, params T[] except)
        {
            return HExceptImpl(values, except);
        }
        public static T[] HExcept<T>(this IEnumerable<T> values, IEnumerable<T> except)
        {
            return HExceptImpl(values, except);
        }
        public static T[] HExceptImpl<T>(this IEnumerable<T> values, IEnumerable<T> except)
        {
            //var result = new List<T>(values.Except(except));

            HashSet<T> setexcept = new HashSet<T>(except);
            List<T> result = new List<T>();
            foreach(T value in values)
                if(setexcept.Contains(value) == false)
                    result.Add(value);

            return result.ToArray();
        }
        public static T[] HSetDiff<T>(this IEnumerable<T> values, IEnumerable<T> except)
        {
            return HExceptImpl(values, except);
        }
    }
}
