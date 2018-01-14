using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        //public static int[] HRank(this IList<double> values, string option="ascending")
        public static int[] HRank<T>(this IList<T> values, string option="ascending")
            where T : IComparable<T>
        {
            int[] idxsort = values.HIdxSorted();
            int[] rank = new int[values.Count];

            for(int i=0; i<values.Count; i++)
                rank[idxsort[i]] = i;

            //HOptions opt = option;
            switch(option)
            {
                case  "ascending": break; // do nothing
                case "descending":
                    for(int i=0; i<rank.Length; i++)
                        rank[i] = rank.Length - rank[i] - 1;
                    break;
                default:
                    goto case "ascending";
            }

            return rank;
        }
    }
}
