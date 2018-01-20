using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public static partial class Collections
    {
        public static T[]     HClone<T>(this T[]     values)   { return (T[])(values.Clone()); }
        public static List<T> HClone<T>(this List<T> values)   { return new List<T>(values); }
        public static List<T> HCloneDeep<T>(this List<T> values)
            where T : ICloneable
        {
            return new List<T>(values.ToArray().HCloneDeep());
        }
        public static T[] HCloneDeep<T>(this T[] values)
            where T : ICloneable
        {
            T[] clone = new T[values.Length];
            for(int i=0; i<values.Length; i++)
                clone[i] = (T)values[i].Clone();
            return clone;
        }
    }
}
