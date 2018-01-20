using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;

namespace HTLib3
{
    public static partial class HMath
    {
        public static double HMedian(this IList<double> values)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            values = values.HSort();
            int idxmid = values.Count;
            if(idxmid %2 == 1) idxmid = (idxmid - 1) / 2;
            else               idxmid = idxmid / 2;
            return values[idxmid];
        }
        public static double HAvg(params double[] values)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            return values.Average();
        }
        public static double HAvg(this IList<double> values)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            return values.Average();
        }
        public static double HAvg(this Vector vec)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            return vec.ToArray().Average();
        }
        public static double HVar(this IList<double> values)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            if(Debug.SelftestDo())
            {
                double tvar = (new double[] { 1, 2, 3, 4, 5 }).HVar();
                double terr = 2.5 - tvar;
                Debug.AssertTolerant(0.0000000001, terr);
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
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            return vec.ToArray().HVar();
        }
    }
}
