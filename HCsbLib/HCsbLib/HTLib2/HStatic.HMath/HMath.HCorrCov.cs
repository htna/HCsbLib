using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

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
        public static double HCov(IEnumerable<double> list1, IEnumerable<double> list2)
        {
            throw new HException("selftest");
            IEnumerator<double> enum1 = list1.GetEnumerator();
            IEnumerator<double> enum2 = list2.GetEnumerator();
            enum1.Reset();
            enum2.Reset();
            int    cnt = 0;
            double E12 = 0;
            double E1  = 0;
            double E2  = 0;
            while(true)
            {
                bool mn1 = enum1.MoveNext();
                bool mn2 = enum2.MoveNext();
                if(mn1 == false && mn2 == false)
                    break;
                if(mn1 == true && mn2 == true)
                {
                    double v1 = enum1.Current;
                    double v2 = enum2.Current;
                    E1  += v1;
                    E2  += v2;
                    E12 += v1*v2;
                    cnt ++;
                    continue;
                }
                throw new HException("count mismatch in HCov(IEnumerable<double>,IEnumerable<double>)");
            }
            E12 /= cnt;
            E1  /= cnt;
            E2  /= cnt;
            double cov = E12 - E1*E2;
            return cov;
        }
        public static double HCorr(IEnumerable<double> list1, IEnumerable<double> list2)
        {
            //throw new HException("selftest");
            if(HDebug.Selftest())
            {
                // check with mathematica
                IEnumerable<double> tlist1 = new double[] { 1, 2, 3 };
                IEnumerable<double> tlist2 = new double[] { 3, 7, 4 };

                double tcorr = HCorr(tlist1, tlist2);
                double terr = 0.2401922307076307 - tcorr;
                HDebug.AssertTolerance(0.00000001, terr);
            }

            IEnumerator<double> enum1 = list1.GetEnumerator();
            IEnumerator<double> enum2 = list2.GetEnumerator();
            enum1.Reset();
            enum2.Reset();
            int    cnt = 0;
            double E12 = 0;
            double E1  = 0;
            double E2  = 0;
            double E11 = 0;
            double E22 = 0;
            while(true)
            {
                bool mn1 = enum1.MoveNext();
                bool mn2 = enum2.MoveNext();
                if(mn1 == false && mn2 == false)
                    break;
                if(mn1 == true && mn2 == true)
                {
                    double v1 = enum1.Current;
                    double v2 = enum2.Current;
                    E1  += v1;
                    E2  += v2;
                    E12 += v1*v2;
                    E11 += v1*v1;
                    E22 += v2*v2;
                    cnt ++;
                    continue;
                }
                throw new HException("count mismatch in HCov(IEnumerable<double>,IEnumerable<double>)");
            }
            E12 /= cnt;
            E1  /= cnt;
            E2  /= cnt;
            E11 /= cnt;
            E22 /= cnt;
            double corr = (E12 - E1*E2) / (Math.Sqrt(E11 - E1*E1)*Math.Sqrt(E22 - E2*E2));
            return corr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HCorrIter_Init(out (double E1, double E2, double E11, double E22, double E12, double cnt) corrinfo)
        {
            corrinfo.E1  = 0;
            corrinfo.E2  = 0;
            corrinfo.E11 = 0;
            corrinfo.E22 = 0;
            corrinfo.E12 = 0;
            corrinfo.cnt = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double E1, double E2, double E11, double E22, double E12, double cnt) HCorrIter_Init()
        {
            (double E1, double E2, double E11, double E22, double E12, double cnt) corrinfo;
            HCorrIter_Init(out corrinfo);
            return corrinfo;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HCorrIter_Accuum(ref (double E1, double E2, double E11, double E22, double E12, double cnt) corrinfo, double v1, double v2)
        {
            corrinfo.E1  += v1;
            corrinfo.E2  += v2;
            corrinfo.E11 += v1*v1;
            corrinfo.E22 += v2*v2;
            corrinfo.E12 += v1*v2;
            corrinfo.cnt ++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double HCorrIter_GetCorr(in (double E1, double E2, double E11, double E22, double E12, double cnt) corrinfo)
        {
            double E1  = corrinfo.E1  / corrinfo.cnt;
            double E2  = corrinfo.E2  / corrinfo.cnt;
            double E11 = corrinfo.E11 / corrinfo.cnt;
            double E22 = corrinfo.E22 / corrinfo.cnt;
            double E12 = corrinfo.E12 / corrinfo.cnt;
            double corr = (E12 - E1*E2) / (Math.Sqrt(E11 - E1*E1)*Math.Sqrt(E22 - E2*E2));
            return corr;
        }
    }
}
