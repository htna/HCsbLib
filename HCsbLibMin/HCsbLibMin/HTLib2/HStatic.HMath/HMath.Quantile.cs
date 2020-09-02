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
        public static double Quantile(IEnumerable<double> seq, double quantile)
        {
            var elements=seq.ToList();
            elements.Sort();
            double realIndex=quantile*(elements.Count-1);
            int index=(int)realIndex;
            double frac=realIndex-index;
            if(index+1<elements.Count)
                return elements[index]*(1-frac)+elements[index+1]*frac;
            else
                return elements[index];
        }
        public static void Quantile(IEnumerable<double> seq, double[] quantiles, double[] rets)
        {
            var elements = seq.ToList();
            elements.Sort();
            
            HDebug.Assert(quantiles.Length == rets.Length);
            for(int i=0; i<quantiles.Length; i++)
            {
                double quantile = quantiles[i];
                double ret;

                double realIndex = quantile * (elements.Count - 1);
                int index = (int)realIndex;
                if (index + 1 < elements.Count)
                {
                    double frac = realIndex - index;
                    ret = elements[index] * (1 - frac) + elements[index + 1] * frac;
                }
                else
                {
                    ret = elements[index];
                }

                rets[i] = ret;
            }
        }
        public static double[] Quantile(IEnumerable<double> seq, params double[] quantiles)
        {
            double[] rets = new double[quantiles.Length];
            Quantile(seq, quantiles, rets);
            return rets;
        }
    }
}
