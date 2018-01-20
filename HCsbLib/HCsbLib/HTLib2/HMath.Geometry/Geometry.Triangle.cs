using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class Geometry
	{
        /// http://en.wikipedia.org/wiki/Triangle
        /// 
        /// a,b,c: the length of triangles
        /// A,B,C: the angle of a, b, and c
        /// pa, pb, pc: the point of A, B, and C
        /// 
        ///          pa
        ///          *      (pa,pb,pc)
        ///         /A\        |
        ///        / | \       r
        ///       c  |  b      |
        ///      /   *   \    po:origin
        ///     /B       C\
        /// pb *-----a-----* pc
        /// 
        public partial class Triangle
        {

            public static double GetDLengthByDAngle(double a, double b, double c)
            {
                HDebug.ToDo("check !!!");
                /// return d_a / d_A
                /// 
                ///     A = acos(b^2 + c^2 - a^2 / 2 b c)
                ///  cosA = (b^2 + c^2 - a^2 / 2 b c)
                ///   a^2 = b^2 + c^2 - 2 b c cos(A)
                ///   a   = sqrt(b^2 + c^2 - 2 b c cos(A))
                /// da_dA = (b c Sin[A])/Sqrt[b^2 + c^2 - 2 b c Cos[A]]
                /// 
                double a2 = a*a;
                double b2 = b*b;
                double c2 = c*c;
                double cosA = (b2 + c2 - a2) / (2*b*c);
                double A = Math.Acos(cosA);
                double sinA = Math.Sin(A);
                double da_dA = (b*c*sinA)/Math.Sqrt(b2 + c2 - 2*b*c*cosA);
                return da_dA;
            }
            public static double GetD2LengthByDAngle2(double a, double b, double c)
            {
                HDebug.ToDo("check !!!");
                /// return d^2_a / d_A^2
                /// 
                ///     A = acos(b^2 + c^2 - a^2 / 2 b c)
                ///  cosA = (b^2 + c^2 - a^2 / 2 b c)
                ///   a^2 = b^2 + c^2 - 2 b c cos(A)
                ///   la  = sqrt(b^2 + c^2 - 2 b c cos(A))
                ///       == a
                /// da_dA = (b c Sin[A])/Sqrt[b^2 + c^2 - 2 b c Cos[A]]
                ///       = (b c Sin[A])/(                          la)
                /// d2a_dA2 = d_dA (da_dA)
                ///         = (b c Cos[A])/Sqrt[b^2 + c^2 - 2 b c Cos[A]] - (b^2 c^2 Sin[A]^2)/(b^2 + c^2 - 2 b c Cos[A])^(3/2)
                ///         = (b c Cos[A])/Sqrt[b^2 + c^2 - 2 b c Cos[A]] - (b^2 c^2 Sin[A]^2)/(Sqrt[b^2 + c^2 - 2 b c Cos[A]]^3)
                ///         = (b c Cos[A])/(                          la) - (b^2 c^2 Sin[A]^2)/(                            la^3)
                ///         = (b c cosA  )/(                          la) - (b2  c2  sinA2   )/(                            la^3)
                /// 
                double a2 = a*a;
                double b2 = b*b;
                double c2 = c*c;
                double cosA = (b2 + c2 - a2) / (2*b*c);
                double la = Math.Sqrt(b2 + c2 - 2*b*c*cosA);
                double la3 = la*la*la;
                HDebug.AssertTolerance(0.00000001, la-a);
                double A = Math.Acos(cosA);
                double sinA = Math.Sin(A);
                double sinA2 = sinA * sinA;
                double da_dA = (b*c*sinA)/la;
                double d2a_dA2 = (b*c*cosA)/(la) - (b2*c2*sinA2)/(la3);
                return d2a_dA2;
            }
            public static double GetDLengthByDLength(double a, double b, double c)
            {
                HDebug.ToDo("check !!!");
                /// reurn da_dc
                ///       when angle C is 90'
                /// 
                ///    *
                ///    | \
                ///    |  \
                ///    b   c
                ///    |    \
                ///    |C    \
                ///    *--a---*
                /// 
                ///     c = sqrt(a^2 + b^2)
                /// dc_da = a / sqrt(a^2 + b^2)
                double dc_da = a / Math.Sqrt(a*a + b*b);
                return dc_da;
            }
            public static double GetDLengthByDAngle(Vector pa, Vector pb, Vector pc)
            {
                HDebug.ToDo("check !!!");
                double a = (pb-pc).Dist;
                double b = (pc-pa).Dist;
                double c = (pa-pb).Dist;
                return GetDLengthByDAngle(a, b, c);
            }
            public class CGetTriGeom
            {
                public double a, b, c;
                public double A, B, C;
                public double r;
                public Vector po;
            };
            public static CGetTriGeom GetTriGeom(Vector pa, Vector pb, Vector pc)
            {
                CGetTriGeom ret = new CGetTriGeom();
                GetTriGeom
                (pa, pb, pc
                , out ret.a, out ret.b, out ret.c
                , out ret.A, out ret.B, out ret.C
                , out ret.r
                , out ret.po
                );
                return ret;
            }
            public static void GetTriGeom(Vector pa, Vector pb, Vector pc
                                         , out double a, out double b, out double c
                                         , out double A, out double B, out double C
                                         , out double r
                                         , out Vector po
                                         )
            {
                Func<Vector, Vector, Vector, double> angle2 = delegate(Vector p1, Vector p2, Vector p3)
                {
                    Vector v21 = p1 - p2;
                    Vector v23 = p3 - p2;
                    double d21 = v21.Dist;
                    double d23 = v23.Dist;
                    double cos2 = LinAlg.DotProd(v21, v23) / (d21 * d23);
                    double ang2 = Math.Acos(cos2);
                    return ang2;
                };
                ////////////////////////////////////////////////////////////////
                //             1     (1,2,3)
                //            /A\      |
                //           / | \     r   
                //          c  |  b    |
                //         /   o   \   origin
                //        /B       C\
                //       2-----a-----3
                //
                /////////////////////////////////////////////////////////////////
                // http://schools-wikipedia.org/wp/t/Trigonometric_functions.htm
                // a/sinA = b/sinB = c/sinC = 2R
                Vector pt23 = pb - pc; double dist23 = pt23.Dist;
                Vector pt31 = pc - pa; double dist31 = pt31.Dist;
                Vector pt12 = pa - pb; double dist12 = pt12.Dist;
                a = dist23; // =(pt2-pt3).Dist; // distance between pt2 and pt3
                b = dist31; // =(pt3-pt1).Dist; // distance between pt2 and pt3
                c = dist12; // =(pt1-pt2).Dist; // distance between pt2 and pt3
                A = angle2(pc, pa, pb); // angle A = ∠pt3-pt1(a)-pt2
                B = angle2(pa, pb, pc); // angle B = ∠pt1-pt2(b)-pt3
                C = angle2(pb, pc, pa); // angle C = ∠pt2-pt3(c)-pt1
                r = 0.5 * a / Math.Sin(A);
                // Barycentric coordinates from cross- and dot-products
                // http://en.wikipedia.org/wiki/Circumscribed_circle#Barycentric_coordinates_as_a_function_of_the_side_lengths
                /// po = v1.p1 + v2.p2 + v3.p3
                /// v1 = (|p2-p3|^2 * (p1-p2).(p1-p3)) / (2*|(p1-p2)x(p2-p3)|^2) = a*a*Dot(
                /// v2 = (|p1-p3|^2 * (p2-p1).(p2-p3)) / (2*|(p1-p2)x(p2-p3)|^2) = b*b*Dot(
                /// v3 = (|p1-p2|^2 * (p3-p1).(p3-p2)) / (2*|(p1-p2)x(p2-p3)|^2) = c*c*Dot(
                {
                    double div = 2 * LinAlg.CrossProd(pa-pb, pb-pc).Dist2;
                    double v1  = dist23*dist23 * LinAlg.DotProd(pt12, -pt31) / div;
                    double v2  = dist31*dist31 * LinAlg.DotProd(-pt12, pt23) / div;
                    double v3  = dist12*dist12 * LinAlg.DotProd(pt31, -pt23) / div;
                    po = v1*pa + v2*pb + v3*pc;
                }
                if(HDebug.IsDebuggerAttached)
                {
                    double la, lb, lc;
                    double lA, lB, lC;
                    double lr;
                    Vector lpo;
                    Geometry.GetTriGeom(pa, pb, pc, out la, out lb, out lc, out lA, out lB, out lC, out lr, out lpo);
                    HDebug.Assert(la==a, lb==b, lc==c);
                    HDebug.Assert(lA==A, lB==B, lC==C);
                    HDebug.Assert(lr==r);
                    HDebug.Assert(lpo==po);
                }
            }
            public static Tuple<Vector,double> CenterRadiusOfTriangle(Vector pa, Vector pb, Vector pc)
            {
                double a, b, c, A, B, C, r;
                Vector pto;
                GetTriGeom(pa, pb, pc,
                           out a, out b, out c,
                           out A, out B, out C,
                           out r,
                           out pto
                          );
                if(HDebug.IsDebuggerAttached)
                {
                    double lr = Geometry.RadiusOfTriangle(pa, pb, pc);
                    HDebug.Assert(lr==r);
                }
                return new Tuple<Vector,double>(pto,r);
            }
        }
	}
}
