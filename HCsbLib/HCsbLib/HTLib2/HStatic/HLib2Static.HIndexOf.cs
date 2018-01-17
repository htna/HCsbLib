using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int HIndexOf(this string[] values, string item)
        {
            IList<string> _values = values;
            return _values.IndexOf(item);
        }

        public static int HIndexOf<T>(this IList<T> list, T item)
        {
            return list.IndexOf(item);
        }
        public static int[] HIndexOfContains<T>(this IList<HashSet<T>> valuess, T item)
        {
            List<int> idxs = new List<int>();
            for(int idx=0; idx<valuess.Count; idx++)
                if(valuess[idx].Contains(item))
                    idxs.Add(idx);
            return idxs.ToArray();
        }

        public static int HIndexOfReference<T>(this IList<T> values, T item)
            where T : class
        {
            for(int i=0; i<values.Count; i++)
                if(object.ReferenceEquals(item, values[i]))
                    return i;
            return -1;
        }
        public static int[] HIndexOfReference<T>(this IList<T> values, IList<T> items)
            where T : class
        {
            int[] indexs = new int[items.Count];
            for(int i=0; i<items.Count; i++)
                indexs[i] = values.HIndexOfReference(items[i]);
            return indexs;
        }
        public static int[][] HIndexOfReference<T>(this IList<T> values, IList<T[]> itemss)
            where T : class
        {
            if(itemss == null)
                return null;
            int[][] indexss = new int[itemss.Count][];
            for(int i=0; i<itemss.Count; i++)
                indexss[i] = values.HIndexOfReference(itemss[i]);
            return indexss;
        }
        public static int[][][] HIndexOfReference<T>(this IList<T> values, IList<T[][]> itemss)
            where T : class
        {
            if(itemss == null)
                return null;
            int[][][] indexss = new int[itemss.Count][][];
            for(int i=0; i<itemss.Count; i++)
                indexss[i] = values.HIndexOfReference(itemss[i]);
            return indexss;
        }

        public static int HIndexOfReference_depreciated<T>(this T values, IList<T> searchFrom)
            where T : class
        {
            HDebug.Depreciated();
            for(int i=0; i<searchFrom.Count; i++)
                if(object.ReferenceEquals(values, searchFrom[i]))
                    return i;
            return -1;
        }
        public static int[] HIndexOfReference_depreciated<T>(this IList<T> values, IList<T> searchFrom)
            where T : class
        {
            HDebug.Depreciated();
            int[] index = new int[values.Count];
            for(int i=0; i<values.Count; i++)
                index[i] = values[i].HIndexOfReference_depreciated(searchFrom);
            return index;
        }
        public static int[] HIndexOfNull<T>(this IList<T> values)
        {
            return ListIdxNull(values);
        }
        public static int[] ListIdxNull<T>(this IList<T> values)
        {
            List<int> idxnull = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(values[i] == null)
                    idxnull.Add(i);
            return idxnull.ToArray();
        }
        public static int[] ListIdxNotNull<T>(this IList<T> values)
        {
            List<int> idxs = new List<int>();
            for(int i=0; i<values.Count; i++)
                if(values[i] != null)
                    idxs.Add(i);
            return idxs.ToArray();
        }
    }
}
