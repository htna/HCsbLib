using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static List<((double min, double max) bin, int count)> HHist(this IEnumerable<double> values, double bin_min, double bin_max, double bin_width)
        {
            List<(double min, double max)> bins      = new List<(double min, double max)>();
            List<int>                      bin_count = new List<int>();
            // create bins
            for(int i=0; (bin_min + bin_width*i)<bin_max; i++)
            {
                double bmin = bin_min + bin_width*i;
                double bmax = bmin    + bin_width;
                bins.Add((bmin,bmax));
                bin_count.Add(0);
            }
            HDebug.Assert(bins.Count == bin_count.Count);
            // collect counts
            foreach(double value in values)
            {
                int ibin = (int)((value - bin_min)/bin_width);
                if((ibin < 0) || (bin_count.Count <= ibin)) continue;
                bin_count[ibin] ++;
            }
            // return format
            List<((double min, double max) bin, int count)> hist = new List<((double min, double max) bin, int count)>();
            for(int i=0; i<bins.Count; i++)
                hist.Add((bins[i], bin_count[i]));

            return hist;
        }
    }
}
