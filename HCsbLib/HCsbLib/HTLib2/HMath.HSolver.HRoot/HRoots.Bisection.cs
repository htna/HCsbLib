using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
    public partial class HRoots
    {
        public static double? GetRootBisection(Func<double, double> func, double x0, double x1)
		{
			double v0 = func(x0);
			double v1 = func(x1);
			Debug.Assert(v1 * v0 < 0);
			int iter = 0;
			while(Math.Abs(v0) > 0.0000001 || Math.Abs(v1) > 0.0000001)
			{
				double x = (x1 + x0) / 2.0;
				double v = func(x);
				iter++;
				if(v == 0)
				{
					return x;
				}
				else if(v * v0 < 0)
				{
					Debug.Assert(v * v0 < 0);
					Debug.Assert(v * v1 > 0);
					x1 = x;
					v1 = v;
					Debug.Assert(x0 < x1);
				}
				else
				{
					Debug.Assert(v * v0 > 0);
					Debug.Assert(v * v1 < 0);
					x0 = x;
					v0 = v;
					Debug.Assert(x0 < x1);
				}

				if(iter > 1000)
					return null;
			}
			double xx = (x1 + x0) / 2.0;
			Debug.Assert(Math.Abs(func(xx)) < 0.00001);
			return xx;
		}
    }
}
