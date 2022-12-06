using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        /// source https://stackoverflow.com/questions/8137391/percentile-calculation
        public static double HQuantile(this IEnumerable<double> seq, double quantile) { return Quantile(seq, quantile); }
        public static double Quantile(IEnumerable<double> seq, double quantile)
        {
            List<double> sortedseq;
            return Quantile(seq, quantile, out sortedseq);
        }
        public static double HQuantile(this IEnumerable<double> seq, double quantile, out List<double> sortedseq) { return Quantile(seq, quantile, out sortedseq); }
        public static double Quantile(IEnumerable<double> seq, double quantile, out List<double> sortedseq)
        {
            sortedseq = null;
            return _Quantile(seq, quantile, ref sortedseq);
        }
        public static double _Quantile(IEnumerable<double> seq, double quantile, ref List<double> sortedseq)
        {
            if(sortedseq == null) { sortedseq = seq.ToList(); }
            else                  { sortedseq.Clear(); sortedseq.AddRange(seq); }
            sortedseq.Sort();
            double realIndex=quantile*(sortedseq.Count-1);
            int index=(int)realIndex;
            double frac=realIndex-index;
            if(index+1<sortedseq.Count)
                return sortedseq[index]*(1-frac)+sortedseq[index+1]*frac;
            else
                return sortedseq[index];
        }
        public static void HQuantile(this IEnumerable<double> seq, double[] quantiles, double[] rets) { Quantile(seq, quantiles, rets); }
        public static void Quantile(IEnumerable<double> seq, double[] quantiles, double[] rets)
        {
            List<double> sortedseq;
            Quantile(seq, quantiles, rets, out sortedseq);
        }
        public static void HQuantile(this IEnumerable<double> seq, double[] quantiles, double[] rets, out List<double> sortedseq) { Quantile(seq, quantiles, rets, out sortedseq); }
        public static void Quantile(IEnumerable<double> seq, double[] quantiles, double[] rets, out List<double> sortedseq)
        {
            sortedseq = null;
            _Quantile(seq, quantiles, rets, ref sortedseq);
        }
        public static void _Quantile(IEnumerable<double> seq, double[] quantiles, double[] rets, ref List<double> sortedseq)
        {
            if(sortedseq == null) { sortedseq = seq.ToList(); }
            else                  { sortedseq.Clear(); sortedseq.AddRange(seq); }
            sortedseq.Sort();
            
            HDebug.Assert(quantiles.Length == rets.Length);
            for(int i=0; i<quantiles.Length; i++)
            {
                double quantile = quantiles[i];
                double ret;

                double realIndex = quantile * (sortedseq.Count - 1);
                int index = (int)realIndex;
                if (index + 1 < sortedseq.Count)
                {
                    double frac = realIndex - index;
                    ret = sortedseq[index] * (1 - frac) + sortedseq[index + 1] * frac;
                }
                else
                {
                    ret = sortedseq[index];
                }

                rets[i] = ret;
            }
        }
        public static double[] HQuantile(this IEnumerable<double> seq, params double[] quantiles) { return Quantile(seq, quantiles); }
        public static double[] Quantile(IEnumerable<double> seq, params double[] quantiles)
        {
            List<double> sortedseq;
            double[] rets = new double[quantiles.Length];
            Quantile(seq, quantiles, rets, out sortedseq);
            return rets;
        }
        public static double[] HQuantile(this IEnumerable<double> seq, out List<double> sortedseq, params double[] quantiles) { return Quantile(seq, out sortedseq, quantiles); }
        public static double[] Quantile(IEnumerable<double> seq, out List<double> sortedseq, params double[] quantiles)
        {
            double[] rets = new double[quantiles.Length];
            Quantile(seq, quantiles, rets, out sortedseq);
            return rets;
        }
    }
}
