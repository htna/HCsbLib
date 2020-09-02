using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HLib2Static
    {
        public static int[] HCount<T>(this IList<T[]> valuess)
        {
            if(HDebug.Selftest())
            {
                double[][] tvaluess = new double[][] { new double[] { 1, 2, 3 }, new double[0], new double[] { 1, 2, 3, 4 } };
                int[] tcnt = HCount(tvaluess);
                HDebug.Assert(new TVector<int>(tcnt) == new int[] { 3, 0, 4 });
            }

            int[] counts = new int[valuess.Count];
            for(int i=0; i<counts.Length; i++)
                counts[i] = valuess[i].Length;
            return counts;
        }
        public static int HCountEqual<T>(this IList<T> valuess, T equalto)
        {
            int count = 0;
            dynamic lequalto = equalto;
            foreach(var value in valuess)
            {
                if(lequalto == value)
                    count++;
            }
            return count;
        }
    }
}
