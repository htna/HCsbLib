using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static int[] HIdxWithinCutoff(this IList<Vector> coords, double cutoff, IList<Vector> queries)
        {
            double cutoff2 = cutoff * cutoff;

            HashSet<int> idxwithin = new HashSet<int>();
            foreach(Vector query in queries)
                for(int i=0; i<coords.Count; i++)
                {
                    double dist2 = (query - coords[i]).Dist2;
                    if(dist2 < cutoff2)
                        idxwithin.Add(i);
                }
            return idxwithin.ToArray();
        }
    }
}
