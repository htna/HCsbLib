using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib3
{
	public static partial class HMath
	{
        //public static double Corr(Vector vec1, Vector vec2, bool ignore_nan /*=false*/)
        //{
        //    return Corr(vec1.ToArray().ToList(), vec2.ToArray().ToList(), ignore_nan);
        //}
        public static double HCorr(double[] vec1, double[] vec2, bool ignore_nan /*=false*/)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");

            if(Debug.SelftestDo())
            {
                double tcorr = HCorr( new double[] { 1, 4, double.NaN, double.NaN,          5, 2, 8 }
                                   , new double[] { 2, 1,          3, double.NaN, double.NaN, 7, 9 }
                                   , true);
                double terr = 0.5784990588061418 - tcorr;
                Debug.AssertTolerant(0.00000001, terr);
            }
            if(vec1.Length != vec2.Length)
                throw new Exception();
            if(ignore_nan == true)
            {
                int size = vec1.Length;
                bool[] isnan = new bool[size]; // initially false
                foreach(int idxnan in vec1.ToArray().HIdxListEqual(double.NaN)) isnan[idxnan] = true;
                foreach(int idxnan in vec2.ToArray().HIdxListEqual(double.NaN)) isnan[idxnan] = true;
                int[] idxnnon = isnan.HIdxListEqual(false);
                vec1 = vec1.ToArray().HSelectByIndex(idxnnon);
                vec2 = vec2.ToArray().HSelectByIndex(idxnnon);
            }
            return HCorr(vec1, vec2);
        }
        //public static double Cov(Vector vec1, Vector vec2)
        //{
        //    return Cov(vec1.ToArray().ToList(), vec2.ToArray().ToList());
        //}
        public static double HCov(double[] vec1, double[] vec2)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            if(Debug.SelftestDo())
            {
                // check with mathematica
                double tcov = HCov(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 });
                double terr = 0.5 - tcov;
                Debug.AssertTolerant(0.00000001, terr);
            }
            if(vec1.Length != vec2.Length)
                throw new Exception();
            double avg1 = vec1.HAvg();
            double avg2 = vec2.HAvg();
            double cov = 0;
            int size = vec1.Length;
            for(int i=0; i<size; i++)
                cov += (vec1[i] - avg1)*(vec2[i] - avg2);
            cov = cov / (size-1); /// the unbiased estimate of the covariance, which divide by (n-1)
            return cov;
        }
        //public static double Corr(Vector vec1, Vector vec2)
        //{
        //    return Corr(vec1.ToArray().ToList(), vec2.ToArray().ToList());
        //}
        public static double HCorr(double[] vec1, double[] vec2)
        {
            Debug.ToDo("Depreciate. Use HTLib2.HMath.xxx");
            if(Debug.SelftestDo())
            {
                // check with mathematica
                double tcorr = HCorr(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 });
                double terr = 0.2401922307076307 - tcorr;
                Debug.AssertTolerant(0.00000001, terr);
            }
            if(vec1.Length != vec2.Length)
                throw new Exception();
            double corr = HCov(vec1, vec2) / Math.Sqrt(vec1.HVar() * vec2.HVar());
            return corr;
        }
    }
}
