using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HRoots
    {
        public static double? GetRootSecant(Func<double, double> func, double p0, double p1, int maxiter, double tolFunc=0.00000001, double tolP=0.00000001)
        {
            Func<double,object,double> lfunc = delegate(double p, object etc)
            {
                HDebug.Assert(etc == null);
                return func(p);
            };
            return GetRootSecant(lfunc, p0, p1, null, maxiter, tolFunc, tolP);
        }
        public static double? GetRootSecant(Func<double, object, double> func, double p0, double p1, object etc, int maxiter, double tolFunc=0.00000001, double tolP=0.00000001)
        {
            double v0 = func(p0, etc);
            double v1 = func(p1, etc);

            for(int iter=0; iter<maxiter; iter++)
            {
                if(Math.Abs(v1 - v0) < tolFunc)
                {
                    //HDebug.AssertSimilar(v1, 0, 0.00000001);
                    return p1;
                }
                if(Math.Abs(p0 - p1) < tolP)
                {
                    //HDebug.AssertSimilar(v1, 0, 0.00000001);
                    return p1;
                }
                double dv = -1 * v1 * (p1 - p0) / (v1 - v0);
                p0 = p1; v0 = v1;
                p1 = p1 + dv;
                v1 = func(p1, etc);
                HDebug.Assert(double.IsNaN(v1) == false);
            }

            if(Math.Abs(v1-0)<tolFunc)
                return p1;

            return null;
        }
    }
}
