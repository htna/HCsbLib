using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static T[] HUnion<T>(this IList<T> values)
        {
            var union = values.HToHashSet().ToArray();
            return union;
        }
        public static T[] HUnionWith<T>(this IEnumerable<T> values, params T[] with)
        {
            IEnumerable<T> enumwith = with;
            return HUnionWith<T>(values, enumwith);
        }
        public static T[] HUnionWith<T>(this IEnumerable<T> values, IEnumerable<T> with)
        {
            bool treatnull = false;
            return HUnionWith(values, with, treatnull);
        }
        public static T[] HUnionWith<T>(this IEnumerable<T> values, IEnumerable<T> with, bool treatnull)
        {
            if(treatnull && values == null) values = new T[0];
            if(treatnull && with   == null) with   = new T[0];
            var union = new List<T>(values.Union(with));
            return union.ToArray();
        }
        public static T[] HUnionAll<T>(this IList<T[]> valuess)
        {
            var union = new HashSet<T>();
            foreach(var values in valuess)
                foreach(var value in values)
                    union.Add(value);
            return union.ToArray();
        }
    }
}
