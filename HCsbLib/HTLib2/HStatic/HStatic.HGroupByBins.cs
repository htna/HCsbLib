using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static List<Tuple<int,T>>[] HGroupByBins<T>(this IList<T> values, int numbin)
            where T : new()
        {
            double incr = ((double)numbin-1) / ((double)values.Count-1);
            // 0->0  , (cnt-1)->(100-1=99)
            // 0->0.5, (cnt-1)->(      99.5)
            // 0->0  , (cnt-1)->99           // floor
            // (0.5-1),(1-2),(2-3), ..., (98-99),(99-99.5)

            List<Tuple<int,T>>[] groups; HStatic.NewArray(out groups, numbin);
            for(int i=0; i<values.Count; i++)
            {
                double didx = i*incr+0.5;
                int idx = (int)Math.Floor(didx);
                groups[idx].Add(new Tuple<int, T>(i, values[i]));
            }

            if(HDebug.IsDebuggerAttached && values.Count >= numbin)
            {
                foreach(var bin in groups)
                    HDebug.Assert(bin.Count >= 1);
            }

            return groups;
        }
    }
}
