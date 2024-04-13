using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static List<((T min, T max) bin, int count)> HHist<T>(this IEnumerable<T> values, T bin_min, T bin_max, T bin_width)
        {
            dynamic binmin = bin_min;
            dynamic binmax = bin_max;
            dynamic binwidth = bin_width;
            List<(T min, T max)> bins      = new List<(T min, T max)>();
            List<int>                      bin_count = new List<int>();
            // create bins
            for(int i=0; (binmin + binwidth*i)<binmax; i++)
            {
                T bmin = binmin + binwidth*i;
                T bmax = binmin + binwidth*(i+1);
                bins.Add((bmin,bmax));
                bin_count.Add(0);
            }
            HDebug.Assert(bins.Count == bin_count.Count);
            // collect counts
            foreach(T value in values)
            {
                int ibin = (int)((value - binmin)/binwidth);
                if((ibin < 0) || (bin_count.Count <= ibin)) continue;
                bin_count[ibin] ++;
            }
            // return format
            List<((T min, T max) bin, int count)> hist = new List<((T min, T max) bin, int count)>();
            for(int i=0; i<bins.Count; i++)
                hist.Add((bins[i], bin_count[i]));

            return hist;
        }
    }
}
