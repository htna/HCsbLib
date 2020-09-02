using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HTLib2
{
	public partial class Matlab
	{
		public static double GetMaxAbs(string name)
        {
            double abs_max_name = Math.Abs(Matlab.GetValue("max(max(max(max(BinvD_G))))"));
            double abs_min_name = Math.Abs(Matlab.GetValue("min(min(min(min(BinvD_G))))"));
            double max_abs_name = Math.Max(abs_max_name, abs_min_name);
            return max_abs_name;
        }
    }
}
