using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
    public partial class HRoots
    {
        static bool VerifyRoots(Func<double,double> func, double[] roots)
        {
            for (int i = 0; i < roots.Length; i++)
            {
                double error = func(roots[i]);
                if (Math.Abs(error) > 0.005)
                    return false;
            }
            return true;
        }
        //static bool VerifyRoots(PolynomialComplex poly, Complex[] roots)
        //{
        //    for (int i = 0; i < roots.Length; i++)
        //    {
        //        Complex error = poly.GetValue(roots[i]);
        //        if (Complex.Abs(error) > 0.005)
        //            return false;
        //    }
        //    return true;
        //}
        public static double[] GetRootsClosedForm(double[] poly)
        {
            double[] roots;
            switch(poly.Length)
            {
                case 5:
                    roots = GetRootsClosedFormDegree4(poly[4], poly[3], poly[2], poly[1], poly[0]);
                    break;
                case 4:
                    roots = GetRootsClosedFormDegree3(poly[3], poly[2], poly[1], poly[0]);
                    break;
                case 3:
                    roots = GetRootsClosedFormDegree2(poly[2], poly[1], poly[0]);
                    break;
                case 2:
                    roots = GetRootsClosedFormDegree1(poly[1], poly[0]);
                    break;
                default:
                    Debug.Assert(false);
                    return null;
                    //{
                    //    RootsMuller muller = new RootsMuller(
                    //        (PolynomialComplex)poly,
                    //        new Complex[3] { new Complex(0), new Complex(1), new Complex(2) }
                    //        );
                    //    Complex[] croots = muller.GetRoots();
                    //    List<double> droots = new List<double>();
                    //    for (int i = 0; i < croots.Length; i++)
                    //    {
                    //        if (croots[i].IsApproximateDouble)
                    //            droots.Add(croots[i].real);
                    //    }
                    //    roots = droots.ToArray();
                    //    Debug.Assert(VerifyRoots(poly, roots));
                    //    break;
                    //}
            }
            return roots;
        }
        //public static Complex[] Rootsx(PolynomialComplex poly)
        //{
        //    Complex[] roots;
        //    switch (poly.Dimension)
        //    {
        //        case 4:
        //            roots = RootsClosedform.Roots4(poly[4], poly[3], poly[2], poly[1], poly[0]);
        //            break;
        //        case 3:
        //            roots = RootsClosedform.Roots3(poly[3], poly[2], poly[1], poly[0]);
        //            break;
        //        case 2:
        //            roots = RootsClosedform.Roots2(poly[2], poly[1], poly[0]);
        //            break;
        //        case 1:
        //            roots = RootsClosedform.Roots1(poly[1], poly[0]);
        //            break;
        //        default:
        //            RootsMuller2 muller = new RootsMuller2(
        //                poly, new Complex[3] { new Complex(0), new Complex(1), new Complex(3) }
        //                );
        //            roots = muller.GetRoots();
        //            break;
        //    }
        //    Debug.Assert(VerifyRoots(poly, roots));
        //    return roots;
        //}
    }
}
