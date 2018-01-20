using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
    public partial class HRoots
    {
        public static double? GetRootNewton(Func<double,double> func, Func<double,double> dfunc, double init)
        {
            double x1, x2 = init;
            double dx1 = 0;
            double dx2 = 1000000;
            int iteration = 100;
            while(iteration > 0)
            {
                iteration--;

                x1 = x2;
                x2 = x1 - (func(x1) / dfunc(x1));
                dx1 = dx2;
                dx2 = x2 - x1;
                if(Math.Abs(dx2 - dx1) < 0.00005)
                {
                    /* debug */ if(HDebug.IsDebuggerAttached)
                    /* debug */ {
                    /* debug */     double error = func(x2);
                    /* debug */     HDebug.AssertIf(false, Math.Abs(error) < 0.000001);
                    /* debug */ }
                    return x2;
                }
                if(double.IsInfinity(x2) || double.IsNaN(x2))
                {
                    return null;
                }
            };
            return x2;
        }

        //public static Pair<bool, double> Newton(PolynomialDouble poly, double init)
        //{
        //    double x1, x2 = init;
        //    double dx1 = 0;
        //    double dx2 = 1000000;
        //    int iteration = 100;
        //    while (iteration > 0)
        //    {
        //        iteration--;
        //
        //        x1 = x2;
        //        x2 = x1 - (poly.GetValue(x1) / poly.GetDerivative(x1));
        //        dx1 = dx2;
        //        dx2 = x2 - x1;
        //        if (Math.Abs(dx2 - dx1) < 0.00005)
        //        {
        //            /* debug */    if (Debugger.IsAttached)
        //            /* debug */    {
        //            /* debug */        double error = poly.GetValue(x2);
        //            /* debug */        Debug.AssertIf(false, Math.Abs(error) < 0.000001);
        //            /* debug */    }
        //            return new Pair<bool, double>(true, x2);
        //        }
        //    };
        //    return new Pair<bool, double>(false, x2);
        //}
        //public static Pair<bool,Complex> Newton(PolynomialComplex poly, Complex init)
        //{
        //    Complex x1, x2 = init;
        //    double dx1 = 0;
        //    double dx2 = 1000000;
        //    int iteration = 150;
        //    double err;
        //    while (iteration > 0)
        //    {
        //        iteration--;
        //
        //        x1 = x2;
        //        Complex val = poly.GetValue(x1);
        //        x2 = x1 - (val / poly.GetDerivative(x1));
        //        dx1 = dx2;
        //        dx2 = Complex.Abs(x2 - x1);
        //        err = Complex.Abs(poly.GetValue(x2));
        //        if (Math.Abs(dx2 - dx1) < 0.00005 && err < 0.000001)
        //        {
        //            Debug.Assert(err < 0.000001);
        //            return new Pair<bool, Complex>((err < 0.000001), x2);
        //        }
        //    };
        //    err = Complex.Abs(poly.GetValue(x2));
        //    return new Pair<bool, Complex>((err < 0.000001), x2);
        //}
    }
}
