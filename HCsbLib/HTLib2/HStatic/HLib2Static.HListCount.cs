using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int[] HListCount<T>(this IList<T[]> valuess)
        {
            int[] counts = new int[valuess.Count];
            for(int i=0; i<valuess.Count; i++)
                counts[i] = valuess[i].Length;
            return counts;
        }
        //public static IList<int> HListCount<T>(this IList<List<T>> valuess)
        //{
        //    int[] counts = new int[valuess.Count];
        //    for(int i=0; i<valuess.Count; i++)
        //        counts[i] = valuess[i].Count;
        //    return counts;
        //}
        public static int[] HListCount<T>(this IList<List<T>> valuess)
        {
            int[] counts = new int[valuess.Count];
            for(int i=0; i<valuess.Count; i++)
                counts[i] = valuess[i].Count;
            return counts;
        }
    }
}
