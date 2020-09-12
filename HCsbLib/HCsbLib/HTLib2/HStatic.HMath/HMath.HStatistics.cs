using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	public static partial class HMath
	{
        public static object HStatics(this double[] list, params double[] qntls)
        {
            double                      min         = list.Min();
            double                      max         = list.Max();
            double                      avg         = list.Mean();
            double                      variance    = list.Variance();
            double                      stdvar      = Math.Sqrt(variance);
            double                      median      = list.Median();
            List<(double quantile, double value)>  qntl_value  = new List<(double quantile, double value)>();
            {
                if(qntls.Length == 0)
                    qntls = new double[] {0.01, 0.05, 0.10, 0.25, 0.50, 0.75, 0.90, 0.95, 0.99};
                double[] qntlvals = HMath.Quantile(list, qntls);
                for(int i=0; i<qntls.Length; i++)
                    qntl_value.Add((qntls[i], qntlvals[i]));
            }

            return new
            {
                min       = min       ,
                max       = max       ,
                avg       = avg       ,
                variance  = variance  ,
                stdvar    = stdvar    ,
                median    = median    ,
                quantiles = qntl_value,
            };
        }
    }
}
