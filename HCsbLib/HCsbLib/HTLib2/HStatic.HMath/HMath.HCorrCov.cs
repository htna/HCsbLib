using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        //public static double Corr(Vector vec1, Vector vec2, bool ignore_nan /*=false*/)
        //{
        //    return Corr(vec1.ToArray().ToList(), vec2.ToArray().ToList(), ignore_nan);
        //}
        public static double HCorr(double[] vec1, double[] vec2, bool ignore_nan /*=false*/)
        {
            if(HDebug.Selftest())
            {
                double tcorr = HCorr( new double[] { 1, 4, double.NaN, double.NaN,          5, 2, 8 }
                                   , new double[] { 2, 1,          3, double.NaN, double.NaN, 7, 9 }
                                   , true);
                double terr = 0.5784990588061418 - tcorr;
                HDebug.AssertTolerance(0.00000001, terr);
            }
            if(vec1.Length != vec2.Length)
                throw new Exception();
            if(ignore_nan == true)
            {
                int size = vec1.Length;
                bool[] isnan = new bool[size]; // initially false
                foreach(int idxnan in vec1.HListIndexEqualTo(double.NaN)) isnan[idxnan] = true;
                foreach(int idxnan in vec2.HListIndexEqualTo(double.NaN)) isnan[idxnan] = true;
                int[] idxnnon = isnan.HListIndexEqualTo(false).ToArray();
                if(idxnnon.Length == 0)
                    return double.NaN;
                vec1 = vec1.HSelectByIndex(idxnnon).ToArray();
                vec2 = vec2.HSelectByIndex(idxnnon).ToArray();
            }
            return HCorr(vec1, vec2);
        }
        //public static double Cov(Vector vec1, Vector vec2)
        //{
        //    return Cov(vec1.ToArray().ToList(), vec2.ToArray().ToList());
        //}
        public static double HCov(double[] vec1, double[] vec2)
        {
            return HCov(vec1 as IList<double>, vec2 as IList<double>);
        }
        public static double HCov(IList<double> vec1, IList<double> vec2)
        {
            if(HDebug.Selftest())
            {
                // check with mathematica
                double tcov = HCov(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 });
                double terr = 0.5 - tcov;
                HDebug.AssertTolerance(0.00000001, terr);
            }
            if(vec1.Count != vec2.Count)
                throw new Exception();
            double avg1 = vec1.HAvg();
            double avg2 = vec2.HAvg();
            double cov = 0;
            int size = vec1.Count;
            for(int i=0; i<size; i++)
                cov += (vec1[i] - avg1)*(vec2[i] - avg2);
            cov = cov / (size-1); /// the unbiased estimate of the covariance, which divide by (n-1)
            return cov;
        }
        public static double HCovSelected(IList<double> vec1, IList<double> vec2, IList<bool> sele)
        {
            if(HDebug.Selftest())
            {
                // check with mathematica
                double tcov = HCovSelected(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 }, new bool[] { true, true, true });
                double terr = 0.5 - tcov;
                HDebug.AssertTolerance(0.00000001, terr);
            }
            if((vec1.Count != vec2.Count) || (vec1.Count != sele.Count))
                throw new Exception("((vec1.Count != vec2.Count) || (vec1.Count != sele.Count))");
            double avg1 = HAvgSelected(vec1, sele);
            double avg2 = HAvgSelected(vec2, sele);

            int size = vec1.Count;
            int num_involved = 0;
            double cov = 0;
            for(int i=0; i<size; i++)
            {
                if(sele[i] == false)
                    continue;
                double v1 = vec1[i];
                double v2 = vec2[i];
                cov += (v1 - avg1)*(v2 - avg2);
                num_involved ++;
            }
            cov = cov / (num_involved-1); /// the unbiased estimate of the covariance, which divide by (n-1)
            return cov;
        }
        //public static double Corr(Vector vec1, Vector vec2)
        //{
        //    return Corr(vec1.ToArray().ToList(), vec2.ToArray().ToList());
        //}
        public static double HCorr(double[] vec1, double[] vec2)
        {
            return HCorr(vec1 as IList<double>, vec2 as IList<double>);
        }
        public static double HCorr(IList<double> vec1, IList<double> vec2)
        {
            if(HDebug.Selftest())
            {
                // check with mathematica
                double tcorr = HCorr(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 });
                double terr = 0.2401922307076307 - tcorr;
                HDebug.AssertTolerance(0.00000001, terr);
            }
            if(vec1.Count != vec2.Count)
                throw new Exception();
            double corr = HCov(vec1, vec2) / Math.Sqrt(vec1.HVar() * vec2.HVar());
            return corr;
        }
        public static double HCorrSelected(IList<double> vec1, IList<double> vec2, IList<bool> sele)
        {
            if(HDebug.Selftest())
            {
                // check with mathematica
                double tcorr = HCorrSelected(new double[] { 1, 2, 3 }, new double[] { 3, 7, 4 }, new bool[] { true, true, true });
                double terr = 0.2401922307076307 - tcorr;
                HDebug.AssertTolerance(0.00000001, terr);
            }
            if((vec1.Count != vec2.Count) || (vec1.Count != sele.Count))
                throw new Exception("((vec1.Count != vec2.Count) || (vec1.Count != sele.Count))");
            double cov  = HCovSelected(vec1, vec2, sele);
            double var1 = HVarSelected(vec1, sele);
            double var2 = HVarSelected(vec2, sele);
            double corr = cov / Math.Sqrt(var1 * var2);
            return corr;
        }
    }
}
