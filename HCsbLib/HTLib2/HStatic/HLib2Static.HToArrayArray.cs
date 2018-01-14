using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static T[][] HToArrayArray<T>(this IList<Tuple<T, T>> values)
        {
            List<T[]> array = new List<T[]>(values.Count());
            for(int i=0; i<values.Count(); i++)
                array.Add(values[i].HToArray());
            return array.ToArray();
        }
        public static T[][] HToArrayArray<T>(this IList<Tuple<T, T, T>> values)
        {
            List<T[]> array = new List<T[]>(values.Count());
            for(int i=0; i<values.Count(); i++)
                array.Add(values[i].HToArray());
            return array.ToArray();
        }
        public static T[][] HToArrayArray<T>(this HashSet<T>[] values)
        {
            T[][] array = new T[values.Length][];
            for(int i=0; i<values.Length; i++)
                array[i] = values[i].ToArray();
            return array;
        }
    }
}
