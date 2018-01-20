using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib3
{
    public static partial class Collections
    {
        public static int[] HCount<T>(this IList<T[]> valuess)
        {
            if(Debug.SelftestDo())
            {
                double[][] tvaluess = new double[][] { new double[] { 1, 2, 3 }, new double[0], new double[] { 1, 2, 3, 4 } };
                int[] tcnt = HCount(tvaluess);
                Debug.Assert(new Vector<int>(tcnt) == new int[] { 3, 0, 4 });
            }

            int[] counts = new int[valuess.Count];
            for(int i=0; i<counts.Length; i++)
                counts[i] = valuess[i].Length;
            return counts;
        }
    }
}
