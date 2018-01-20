using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class HRoots
    {
        public static double GetRootRegulaFalsi(Func<double, double> func, double a, double b, int maxiter, double tol=0.0000001)
        {
            Func<double,object,double> lfunc = delegate(double p, object etc)
            {
                HDebug.Assert(etc == null);
                return func(p);
            };
            return GetRootRegulaFalsi(lfunc, a, b, null, tol);
        }
        public static double GetRootRegulaFalsi(Func<double, object, double> func, double a, double b, object etc, double tol=0.0000001)
        {
            ////////////////////////////////////////////////////////
            /// for i = 0, 1, 2, . . ., until satisfied, do
            ///     w ← (f(bi) ai − f(ai) bi)/(f(bi) − f(ai))
            ///     if f(ai)f(w) ≤ 0
            ///        then ai+1 ← ai
            ///             bi+1 ← w
            ///        else ai+1 ← w
            ///             bi+1 ← bi
            ////////////////////////////////////////////////////////

            double fa = func(a, etc);
            double fb = func(b, etc);
            while(Math.Abs(fa) > tol || Math.Abs(fb) > tol)
            {
                double w  = (fb*a - fa*b)/(fb - fa);
                double fw = func(w, etc);
                if(fa*fw < 0)
                {
                    b  = w;
                    fb = fw;
                }
                else
                {
                    a  = w;
                    fa = fw;
                }
            }
            return (a+b)/2;
        }
    }
}
