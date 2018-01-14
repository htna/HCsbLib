using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        //public static T[] HMerge<T>(this IList<List<T>> values) { var mrg = new List<T>(); foreach(var v in values) mrg.AddRange(v); return mrg.ToArray(); }
        //public static T[] HMerge<T>(this IList<    T[]> values) { var mrg = new List<T>(); foreach(var v in values) mrg.AddRange(v); return mrg.ToArray(); }
        //public static T[] HMerge<T>(this IEnumerable<IEnumerable<T>> values) { var mrg = new List<T>(); foreach(var v in values) mrg.AddRange(v); return mrg.ToArray(); }
        public static List<T> HMerge<T>(this IEnumerable<           T[]> lists) { List<T> merge = new List<T>(); foreach(var list in lists) merge.AddRange(list); return merge; }
        public static List<T> HMerge<T>(this IEnumerable<       List<T>> lists) { List<T> merge = new List<T>(); foreach(var list in lists) merge.AddRange(list); return merge; }
        public static List<T> HMerge<T>(this IEnumerable<      IList<T>> lists) { List<T> merge = new List<T>(); foreach(var list in lists) merge.AddRange(list); return merge; }
        public static List<T> HMerge<T>(this IEnumerable<IEnumerable<T>> lists) { List<T> merge = new List<T>(); foreach(var list in lists) merge.AddRange(list); return merge; }
    }
}
