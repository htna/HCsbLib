using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int[] HIdxSorted<T>(this IList<T> values, Comparison<T> comparison)
        {
            Comparison<Pair<T,int>> mycomparison = delegate(Pair<T, int> x, Pair<T, int> y)
            {
                return comparison(x.Item1, y.Item1);
            };

            List<Pair<T,int>> sorteds = new List<Pair<T, int>>(values.Count);
            for(int i=0; i<values.Count; i++)
                sorteds.Add(new Pair<T, int>(values[i], i));
            sorteds.Sort(mycomparison);
            int[] idxs = new int[values.Count];
            for(int i=0; i<values.Count; i++)
                idxs[i] = sorteds[i].Item2;

            string debug="false";
            if(debug == "true")
            {
                HashSet<int> hsIdxs = new HashSet<int>(idxs);
                HDebug.Assert(values.Count == hsIdxs.Count);
            }

            return idxs;
        }
        public static int[] HIdxSorted<T>(this IList<T> values)
            where T : IComparable<T>
        {
            Comparison<T> comparison = delegate(T x, T y)
                {
                    return x.CompareTo(y);
                };
            return HIdxSorted<T>(values, comparison);
        }
        public static Tuple<int,int>[] HIdxSorted<T>(this T[,] values, Comparison<T> comparison)
        {
            Comparison<Tuple<T, int, int>> mycomparison = delegate(Tuple<T, int, int> x, Tuple<T, int, int> y)
            {
                return comparison(x.Item1, y.Item1);
            };

            int colsize = values.GetLength(0);
            int rowsize = values.GetLength(1);
            List<Tuple<T, int, int>> sorteds = new List<Tuple<T, int, int>>(rowsize*colsize);
            for(int c=0; c<colsize; c++)
                for(int r=0; r<rowsize; r++)
                sorteds.Add(new Tuple<T, int, int>(values[c,r], c, r));
            sorteds.Sort(mycomparison);
            return sorteds.HListItem23().ToArray();
        }
        public static Tuple<int, int>[] HIdxSorted<T>(this T[,] values)
            where T : IComparable<T>
        {
            Comparison<T> comparison = delegate(T x, T y)
            {
                return x.CompareTo(y);
            };
            return HIdxSorted<T>(values, comparison);
        }
    }
}
