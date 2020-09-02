using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib2
{
    public static partial class HMath
    {
        public static double HMedian(this IList<double> values)
        {
            values = values.HSort();
            int idxmid = values.Count;
            if(idxmid %2 == 1) idxmid = (idxmid - 1) / 2;
            else               idxmid = idxmid / 2;
            return values[idxmid];
        }
        public static double HAvg(params double[] values)
        {
            return values.Average();
        }
        public static double HAvg(this IList<double> values)
        {
            return values.Average();
        }
        public static double HAvg(this Vector vec)
        {
            return vec.ToArray().Average();
        }
        public static double HVar(this IList<double> values)
        {
            if(HDebug.Selftest())
            {
                double tvar = (new double[] { 1, 2, 3, 4, 5 }).HVar();
                double terr = 2.5 - tvar;
                HDebug.AssertTolerance(0.0000000001, terr);
            }
            double avg = values.HAvg();
            double var = 0;
            foreach(double value in values)
                var += (avg - value)*(avg - value);
            var /= (values.Count - 1); /// the unbiased estimate of variance, which divide by (n-1)
            return var;
        }
        public static double HVar(this Vector vec)
        {
            return vec.ToArray().HVar();
        }




        public static int HMedian(this IList<int> values)
        {
            values = values.HSort();
            int idxmid = values.Count;
            if(idxmid %2 == 1) idxmid = (idxmid - 1) / 2;
            else               idxmid = idxmid / 2;
            return values[idxmid];
        }
        public static double HAvg(params int[] values)
        {
            return values.Average();
        }
        public static double HAvg(this IList<int> values)
        {
            return values.Average();
        }
        public static double HVar(this IList<int> values)
        {
            if(HDebug.Selftest())
            {
                double tvar = (new int[] { 1, 2, 3, 4, 5 }).HVar();
                double terr = 2.5 - tvar;
                HDebug.AssertTolerance(0.0000000001, terr);
            }
            double avg = values.HAvg();
            double var = 0;
            foreach(double value in values)
                var += (avg - value)*(avg - value);
            var /= (values.Count - 1); /// the unbiased estimate of variance, which divide by (n-1)
            return var;
        }
    }
}
