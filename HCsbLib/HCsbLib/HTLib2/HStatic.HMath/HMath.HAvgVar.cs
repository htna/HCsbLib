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
        public static bool HAvgSelected_selftest = HDebug.IsDebuggerAttached;
        public static double HAvgSelected(this IList<double> values, IList<bool> seles)
        {
            if(HAvgSelected_selftest)
            {
                HAvgSelected_selftest = false;
                HDebug.Assert(HAvgSelected(new double[] { 1, 2 }, new bool[] { true , true } ) == 1.5);
                HDebug.Assert(HAvgSelected(new double[] { 1, 2 }, new bool[] { false, true } ) == 2  );
            }
            if(values.Count != seles.Count)
                throw new Exception();
            double avg = 0;
            int count = values.Count;
            int avg_involved = 0;
            for(int i=0; i<count; i++)
                if(seles[i] == true)
                {
                    avg += values[i];
                    avg_involved ++;
                }
            avg = avg / avg_involved;
            return avg;
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
        public static double HVarSelected(this IList<double> values, IList<bool> seles)
        {
            if(HDebug.Selftest())
            {
                double tvar = HVarSelected(new double[] { 1, 2, 3, 4, 5 }, new bool[] { true, true, true, true, true });
                double terr = 2.5 - tvar;
                HDebug.AssertTolerance(0.0000000001, terr);
            }
            if(values.Count != seles.Count)
                throw new Exception();
            double avg = HAvgSelected(values, seles);
            double var = 0;
            int    var_involved = 0;
            for(int i=0; i<values.Count; i++)
                if(seles[i] == true)
                {
                    double value = values[i];
                    var += (avg - value)*(avg - value);
                    var_involved ++;
                }
                
            var /= (var_involved - 1); /// the unbiased estimate of variance, which divide by (n-1)
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
